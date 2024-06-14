using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SVRControllerType
{
    None = 0,
    LController = 1,
    RController = 2
}

public class SVRManager : MonoBehaviour
{
    Collider grabVolume;

    public GameObject lModelSVRController;
    public GameObject rModelSVRController;

    private bool prevLControllerConnected = false;
    private bool prevRControllerConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckControllerConnection();
    }

    private void CheckControllerConnection()
    {
        if (SVRInput.IsControllerInitialized())
        {
            if (lModelSVRController)
            {
                bool lControllerConnected = SVRInput.IsControllerConnected(0);
                if (lControllerConnected != prevLControllerConnected)
                {
                    lModelSVRController.SetActive(lControllerConnected);
                    prevLControllerConnected = lControllerConnected;
                }
            }
            
            if (rModelSVRController)
            {
                bool rControllerConnected = SVRInput.IsControllerConnected(1);
                if (rControllerConnected != prevRControllerConnected)
                {
                    rModelSVRController.SetActive(rControllerConnected);
                    prevRControllerConnected = rControllerConnected;
                }
            }
        }
    }

    public Transform GetControllerTransform()
    {
        return transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        SVRDistanceGrabbable grabbable = other.GetComponentInChildren<SVRDistanceGrabbable>();
        if (grabbable)
        {
            grabbable.InRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SVRDistanceGrabbable grabbable = other.GetComponentInChildren<SVRDistanceGrabbable>();
        if (grabbable)
        {
            grabbable.InRange = false;
        }
    }
}
