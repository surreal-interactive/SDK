using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetLeftControllerPositionExample()
    {
        Vector3 leftControllerPosition = DMControllerManager.instance.GetLeftControllerPosition();
    }

    void GetLeftControllerRotationExample()
    {
        Quaternion leftControllerRotation = DMControllerManager.instance.GetLeftControllerRotation();
    }

    void GetRightControllerPositionExample()
    {
        Vector3 rightControllerPosition = DMControllerManager.instance.GetRightControllerPosition();
    }

    void GetRightControllerRotationExample()
    {
        Quaternion rightControllerRotation = DMControllerManager.instance.GetRightControllerRotation();
    }

    void GetLeftControllerVelocity()
    {
        Vector3 leftVelocity = DMControllerManager.instance.GetLeftControllerVelocity();
    }

    void GetRightControllerVelocity()
    {
        Vector3 rightVelocity = DMControllerManager.instance.GetRightControllerVelocity();
    }

    void GetLeftAngularVelocity()
    {
        Vector3 leftAngularVelocity = DMControllerManager.instance.GetLeftControllerAngularVelocity();
    }

    void GetRightAngularVelocity()
    {
        Vector3 rightAngularVelocity = DMControllerManager.instance.GetRightControllerAngularVelocity();
    }
}
