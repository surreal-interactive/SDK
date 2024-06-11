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
    private bool hadFocus = false;
    public static event Action inputFocusLost;
    public static event Action inputFocusAcquired;

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
            bool lControllerConnected = SVRInput.IsControllerConnected(0);
            if (lControllerConnected != prevLControllerConnected)
            {
                lModelSVRController.SetActive(lControllerConnected);
                prevLControllerConnected = lControllerConnected;
            }

            bool rControllerConnected = SVRInput.IsControllerConnected(1);
            if (rControllerConnected != prevRControllerConnected)
            {
                rModelSVRController.SetActive(rControllerConnected);
                prevRControllerConnected = rControllerConnected;
            }
        }
    }

    public Transform GetControllerTransform()
    {
        return transform;
    }


    public void RaycastCheck()
    {
        Transform controllerTransform = GetControllerTransform();
        RaycastHit raycastHit;
        bool hitFlag = Physics.Raycast(controllerTransform.position, controllerTransform.forward, out raycastHit);
        if (hitFlag &&
            raycastHit.transform &&
            raycastHit.transform.gameObject)
        {
            if (!hadFocus)
            {
                if (inputFocusAcquired != null)
                {
                    inputFocusAcquired();
                }
            }
            hadFocus = true;
        }
        else
        {
            if (hadFocus)
            {
                if (inputFocusLost != null)
                {
                    inputFocusLost();
                }
            }
            hadFocus = false;
        }
    }
}
