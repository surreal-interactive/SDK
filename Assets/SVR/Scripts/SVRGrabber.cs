using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SVRGrabber : MonoBehaviour
{
    // Grip trigger thresholds for picking up objects, with some hysteresis.
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    // Demonstrates parenting the held object to the hand's transform when grabbed.
    // When false, the grabbed object is moved every FixedUpdate using MovePosition.
    // Note that MovePosition is required for proper physics simulation. If you set this to true, you can
    // easily observe broken physics simulation by, for example, moving the bottom cube of a stacked
    // tower and noting a complete loss of friction.
    [SerializeField]
    protected bool m_parentHeldObject = false;

    // Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
    // Also used for ranking grab targets in case of multiple candidates.
    [SerializeField]
    protected Transform m_gripTransform = null;

    // Child/attached Colliders to detect candidate grabbable objects.
    [SerializeField]
    protected Collider[] m_grabVolumes = null;

    [SerializeField]
    protected SVRControllerType m_controllerType = SVRControllerType.None;

    // You can set this explicitly in the inspector if you're using m_moveHandPosition.
    // Otherwise, you should typically leave this null and simply parent the hand to the hand anchor
    // in your scene, using Unity's inspector.
    [SerializeField]
    protected Transform m_parentTransform;

    [SerializeField]
    protected GameObject m_player;

    protected bool m_grabVolumeEnabled = true;
    protected Vector3 m_lastPos;
    protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
    protected SVRGrabbable m_grabbedObj = null;
    protected Vector3 m_grabbedObjectPosOff;
    protected Quaternion m_grabbedObjectRotOff;
    protected Dictionary<SVRGrabbable, int> m_grabCandidates = new Dictionary<SVRGrabbable, int>();

    /// <summary>
    /// The currently grabbed object.
    /// </summary>
    public SVRGrabbable grabbedObject
    {
        get { return m_grabbedObj; }
    }

    public void ForceRelease(SVRGrabbable grabbable)
    {
        bool canRelease = ((m_grabbedObj != null) && (m_grabbedObj == grabbable));
        if (canRelease)
        {
            GrabEnd();
        }
    }

    protected virtual void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;
    }

    protected virtual void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if (m_parentTransform == null)
        {
            m_parentTransform = gameObject.transform;
        }

        // We're going to setup the player collision to ignore the hand collision.
        SetPlayerIgnoreCollision(gameObject, true);
    }

    // Using Update instead of FixedUpdate. Doing this in FixedUpdate causes visible judder even with
    // somewhat high tick rates, because variable numbers of ticks per frame will give hand poses of
    // varying recency. We want a single hand pose sampled at the same time each frame.
    // Note that this can lead to its own side effects. For example, if m_parentHeldObject is false, the
    // grabbed objects will be moved with MovePosition. If this is called in Update while the physics
    // tick rate is dramatically different from the application frame rate, other objects touched by
    // the held object will see an incorrect velocity (because the move will occur over the time of the
    // physics tick, not the render tick), and will respond to the incorrect velocity with potentially
    // visible artifacts.
    virtual public void Update()
    {
        OnUpdatedAnchors();
    }

    // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
    // This is done instead of parenting to achieve workable physics. If you don't require physics on
    // your hands or held objects, you may wish to switch to parenting.
    void OnUpdatedAnchors()
    {
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
        Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;

        if (!m_parentHeldObject)
        {
            MoveGrabbedObject(destPos, destRot);
        }

        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        float prevFlex = m_prevFlex;
        // Update values from inputs
        if (m_controllerType == SVRControllerType.LController)
        {
            m_prevFlex = SVRInput.Get(SVRInput.Axis1D.LIndexTrigger);
        }
        else if (m_controllerType == SVRControllerType.RController)
        {
            m_prevFlex = SVRInput.Get(SVRInput.Axis1D.RIndexTrigger);
        }

        CheckForGrabOrRelease(prevFlex);
    }

    void OnDestroy()
    {
        if (m_grabbedObj != null)
        {
            GrabEnd();
        }
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
        SVRGrabbable grabbable =
            otherCollider.GetComponent<SVRGrabbable>()
            ?? otherCollider.GetComponentInParent<SVRGrabbable>();
        if (grabbable == null)
            return;

        // Add the grabbable
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;
    }

    void OnTriggerExit(Collider otherCollider)
    {
        SVRGrabbable grabbable =
            otherCollider.GetComponent<SVRGrabbable>()
            ?? otherCollider.GetComponentInParent<SVRGrabbable>();
        if (grabbable == null)
            return;

        // Remove the grabbable
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found)
        {
            return;
        }

        if (refCount > 1)
        {
            m_grabCandidates[grabbable] = refCount - 1;
        }
        else
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    protected void CheckForGrabOrRelease(float prevFlex)
    {
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            GrabEnd();
        }
    }

    protected virtual void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
        SVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (SVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(
                    m_gripTransform.position
                );
                float grabbableMagSq = (
                    m_gripTransform.position - closestPointOnBounds
                ).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if (m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controllerType == SVRControllerType.LController)
                    {
                        snapOffset.x = -snapOffset.x;
                    }
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff =
                        m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri =
                    Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // NOTE: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);

            // NOTE: This is to get around having to setup collision layers, but in your own project you might
            // choose to remove this line in favor of your own collision layer setup.
            SetPlayerIgnoreCollision(m_grabbedObj.gameObject, true);

            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }

    protected virtual void MoveGrabbedObject(
        Vector3 pos,
        Quaternion rot,
        bool forceTeleport = false
    )
    {
        if (m_grabbedObj == null)
        {
            return;
        }

        Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
        Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
        Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

        if (forceTeleport)
        {
            grabbedRigidbody.transform.position = grabbablePosition;
            grabbedRigidbody.transform.rotation = grabbableRotation;
        }
        else
        {
            grabbedRigidbody.MovePosition(grabbablePosition);
            grabbedRigidbody.MoveRotation(grabbableRotation);
        }
    }

    protected void GrabEnd()
    {
        if (m_grabbedObj != null)
        {
            Pose localPose;
            if (m_controllerType == SVRControllerType.LController)
            {
                localPose = new Pose
                {
                    position = SVRInput.GetLeftControllerPosition(),
                    rotation = SVRInput.GetLeftControllerRotation()
                };
            }
            else if (m_controllerType == SVRControllerType.RController)
            {
                localPose = new Pose
                {
                    position = SVRInput.GetRightControllerPosition(),
                    rotation = SVRInput.GetRightControllerRotation()
                };
            }
            else
            {
                localPose = Pose.identity;
            }

            Pose offsetPose = new Pose
            {
                position = m_anchorOffsetPosition,
                rotation = m_anchorOffsetRotation
            };
            localPose = SVRCommon.PoseMultiply(localPose, offsetPose);

            Pose pose = new Pose(transform.position, transform.rotation);
            Pose trackingSpace = SVRCommon.PoseMultiply(pose, SVRCommon.PoseInverse(localPose));
            Vector3 linearVelocity;
            if (m_controllerType == SVRControllerType.LController)
            {
                linearVelocity = SVRCommon.QuatMulVec(
                    trackingSpace.rotation,
                    SVRInput.GetLeftControllerVelocity()
                );
            }
            else if (m_controllerType == SVRControllerType.RController)
            {
                linearVelocity = SVRCommon.QuatMulVec(
                    trackingSpace.rotation,
                    SVRInput.GetRightControllerVelocity()
                );
            }
            else
            {
                linearVelocity = Vector3.zero;
            }

            Vector3 angularVelocity;
            if (m_controllerType == SVRControllerType.LController)
            {
                angularVelocity = SVRCommon.QuatMulVec(
                    trackingSpace.rotation,
                    SVRInput.GetLeftControllerAngularVelocity()
                );
            }
            else if (m_controllerType == SVRControllerType.RController)
            {
                angularVelocity = SVRCommon.QuatMulVec(
                    trackingSpace.rotation,
                    SVRInput.GetRightControllerAngularVelocity()
                );
            }
            else
            {
                angularVelocity = Vector3.zero;
            }

            GrabbableRelease(linearVelocity, angularVelocity);
        }

        // Re-enable grab volumes to allow overlap events
        GrabVolumeEnable(true);
    }

    protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
        if (m_parentHeldObject)
            m_grabbedObj.transform.parent = null;
        m_grabbedObj = null;
    }

    protected virtual void GrabVolumeEnable(bool enabled)
    {
        if (m_grabVolumeEnabled == enabled)
        {
            return;
        }

        m_grabVolumeEnabled = enabled;
        for (int i = 0; i < m_grabVolumes.Length; ++i)
        {
            Collider grabVolume = m_grabVolumes[i];
            grabVolume.enabled = m_grabVolumeEnabled;
        }

        if (!m_grabVolumeEnabled)
        {
            m_grabCandidates.Clear();
        }
    }

    protected virtual void OffhandGrabbed(SVRGrabbable grabbable)
    {
        if (m_grabbedObj == grabbable)
        {
            GrabbableRelease(Vector3.zero, Vector3.zero);
        }
    }

    protected void SetPlayerIgnoreCollision(GameObject grabbable, bool ignore)
    {
        if (m_player != null)
        {
            Collider[] playerColliders = m_player.GetComponentsInChildren<Collider>();
            foreach (Collider pc in playerColliders)
            {
                Collider[] colliders = grabbable.GetComponentsInChildren<Collider>();
                foreach (Collider c in colliders)
                {
                    if (!c.isTrigger && !pc.isTrigger)
                        Physics.IgnoreCollision(c, pc, ignore);
                }
            }
        }
    }
}
