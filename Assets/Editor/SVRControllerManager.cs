using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR;

public class SVRControllerManager : MonoBehaviour
{
    UnityEngine.InputSystem.InputDevice left_device;
    UnityEngine.InputSystem.InputDevice right_device;

    static DMDeviceState left_state = new DMDeviceState();
    static DMDeviceState right_state = new DMDeviceState();

    private SVRInputControl svr_input_control;
    void Start()
    {

        InputSystem.onDeviceChange +=
                (device, change) =>
                        {
                            switch (change)
                            {
                                case InputDeviceChange.Added:
                                    Debug.LogFormat("New device added: {0}, usage : {1}, path : {2}", device, device.usages, device.path);
                                    break;

                                case InputDeviceChange.Removed:
                                    Debug.Log("Device removed: " + device);
                                    break;
                            }
                        };

        InputSystem.RegisterLayout<DMInputDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("DMInputDevice"));

        left_device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "DMInputDevice",
            product = "DeepMirror"
        });
        InputSystem.SetDeviceUsage(left_device, UnityEngine.InputSystem.CommonUsages.LeftHand);
        right_device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "DMInputDevice",
            product = "DeepMirror"
        });
        InputSystem.SetDeviceUsage(right_device, UnityEngine.InputSystem.CommonUsages.RightHand);


#if UNITY_IOS || UNITY_VISIONOS
        dm.DMInputApi.DMControllerStart();
        dm.DMInputApi.DMControllerSetButtonCallback(Marshal.GetFunctionPointerForDelegate((dm.DMInputApi.ButtonCallbackDelegate)ButtonInputCallback));
#endif
        Application.onBeforeRender += OnBeforeRender;

    }

    private void OnDestroy()
    {
        Application.onBeforeRender -= OnBeforeRender;
        dm.DMInputApi.DMControllerStop();
    }

    // OnBeforeRender is called once per frame
    void OnBeforeRender()
    {
#if UNITY_IOS || UNITY_VISIONOS
        if (left_device != null) {
            DMUpdatePose(0,ref left_state);
            left_state.tracking_state_ = dm.DMInputApi.DMControllerIsConnected(0) ? (InputTrackingState.Position | InputTrackingState.Rotation) : InputTrackingState.None;
            InputSystem.QueueStateEvent<DMDeviceState>(left_device, left_state);
        }

        if (right_device != null) {
            DMUpdatePose(1,ref right_state);
            right_state.tracking_state_ = dm.DMInputApi.DMControllerIsConnected(1) ? (InputTrackingState.Position | InputTrackingState.Rotation) : InputTrackingState.None;
            InputSystem.QueueStateEvent<DMDeviceState>(right_device, right_state);
	    }
#endif
    }

    [MonoPInvokeCallback(typeof(dm.DMInputApi.ButtonCallbackDelegate))]
    static void ButtonInputCallback(long timestamp, int hand_type, dm.DMInputApi.Buttons buttons) {
        if (hand_type == 0) {
            UpdateButtonInput(ref left_state, buttons);
	    }
        if (hand_type == 1) {
            UpdateButtonInput(ref right_state, buttons);
	    }
    }

    static void UpdateButtonInput(ref DMDeviceState state, dm.DMInputApi.Buttons buttons) {
        state.buttons_ = (buttons.primary_button) |
	                       (buttons.secondary_button << 1) |
			                    (buttons.menu_button << 2) |
		                    (buttons.stick_z_button) << 5;
        state.grip_ = buttons.grip_button_value;
        state.trigger_ = buttons.trigger_button_value;
        state.x_ = buttons.stick_x_value;
        state.y_ = buttons.stick_y_value;
        bool GripPerform = state.grip_ >= 100;
        bool TriggerPerfom = state.trigger_ >= 100;

        if (TriggerPerfom) {
            state.buttons_ |= 0x1 << 3;
	    } else {
            state.buttons_ &= 0xff ^ (0x1 << 3);
	    }

        if (GripPerform) {
            state.buttons_ |= 0x1 << 4;
	    } else {
            state.buttons_ &= 0xff ^ (0x1 << 4);
	    }

    }

    void DMUpdatePose(int hand_type,ref DMDeviceState state) {
        dm.DMInputApi.DMPose pose = new dm.DMInputApi.DMPose();
        dm.DMInputApi.DMVector3f linear_velocity = new dm.DMInputApi.DMVector3f();
        var angular_velocity = new dm.DMInputApi.DMVector3f();
        DMControllerPose(hand_type, ref pose, ref linear_velocity, ref angular_velocity);
        state.rotation_ = new Quaternion(pose.qx, pose.qy, pose.qz, pose.qw);
        state.position_ = new Vector3(pose.x, pose.y, pose.z);
        state.deviceVelocity_ = new Vector3(linear_velocity.x, linear_velocity.y, linear_velocity.z);
        state.deviceAngularVelocity_ = new Vector3(angular_velocity.x, angular_velocity.y, angular_velocity.z);
    }

    void DMControllerPose(int hand_type, ref dm.DMInputApi.DMPose pose, ref dm.DMInputApi.DMVector3f linear_velocity, ref dm.DMInputApi.DMVector3f angular_velocity) {

#if UNITY_IOS || UNITY_VISIONOS
        dm.DMInputApi.DMControllerQueryPose(0, hand_type, ref pose, ref linear_velocity, ref angular_velocity, IntPtr.Zero);
#endif
    }

    void OnEnable()
    {
        if (svr_input_control == null) {
            svr_input_control = new SVRInputControl();
        }
        svr_input_control.SVRControl.Enable();
        Debug.Log("Manager Enable");
    }

    void OnDisable() {
        svr_input_control.SVRControl.Disable();
        Debug.Log("Manager Disable");
    }

    public bool IsControllerInitialized()
    {
        if (svr_input_control != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsControllerConnected(int handType)
    {
        return dm.DMInputApi.DMControllerIsConnected(handType);
    }
}
