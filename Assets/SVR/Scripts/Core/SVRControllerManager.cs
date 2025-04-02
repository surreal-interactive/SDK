using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR;
using System.Collections.Generic;

public class SVRControllerManager : MonoBehaviour
{
    UnityEngine.InputSystem.InputDevice left_device;
    UnityEngine.InputSystem.InputDevice right_device;

    static SVRDeviceState left_state = new SVRDeviceState();
    static SVRDeviceState right_state = new SVRDeviceState();

    private SVRInputControl svr_input_control;

    static Queue<long> left_poll_timestamp_queue_ = new Queue<long>();
    static Queue<long> right_poll_timestamp_queue_ = new Queue<long>();
    static int queue_windows_size = 3;

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

        InputSystem.RegisterLayout<SVRInputDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("SVRInputDevice"));

        left_device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "SVRInputDevice",
            product = "Surreal-Interactive"
        });
        InputSystem.SetDeviceUsage(left_device, UnityEngine.InputSystem.CommonUsages.LeftHand);
        right_device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "SVRInputDevice",
            product = "Surreal-Interactive"
        });
        InputSystem.SetDeviceUsage(right_device, UnityEngine.InputSystem.CommonUsages.RightHand);
        left_state.x_ = 128;
        left_state.y_ = 128;
        right_state.x_ = 128;
        right_state.y_ = 128;

#if UNITY_IOS || UNITY_VISIONOS
        svr.SVRInputApi.SVRStart();
        svr.SVRInputApi.SVRSetButtonCallback(Marshal.GetFunctionPointerForDelegate((svr.SVRInputApi.ButtonCallbackDelegate)ButtonInputCallback));
#endif
        Application.onBeforeRender += OnBeforeRender;

    }

    private void OnDestroy()
    {
        Application.onBeforeRender -= OnBeforeRender;
#if UNITY_IOS || UNITY_VISIONOS
        svr.SVRInputApi.SVRStop();
#endif
    }

    // OnBeforeRender is called once per frame
    void OnBeforeRender()
    {

        if (left_poll_timestamp_queue_.Count == queue_windows_size) {
            long poll_timestamp = left_poll_timestamp_queue_.Dequeue();
            long render_timestamp = svr.SVRInputApi.SVRTimeNow();
            svr.SVRInputApi.SVRRenderFinish(svr.SVRInputApi.Chirality.Left, poll_timestamp, render_timestamp);
        }

        if (right_poll_timestamp_queue_.Count == queue_windows_size) {
            long poll_timestamp = right_poll_timestamp_queue_.Dequeue();
            long render_timestamp = svr.SVRInputApi.SVRTimeNow();
            svr.SVRInputApi.SVRRenderFinish(svr.SVRInputApi.Chirality.Right, poll_timestamp, render_timestamp);
	    }


#if UNITY_IOS || UNITY_VISIONOS
        if (left_device != null) {
            long left_poll_timestamp = svr.SVRInputApi.SVRTimeNow();
            SVRUpdatePose(left_poll_timestamp, svr.SVRInputApi.Chirality.Left, ref left_state);
            left_poll_timestamp_queue_.Enqueue(left_poll_timestamp);
            left_state.tracking_state_ = svr.SVRInputApi.SVRIsConnected(0) ? (InputTrackingState.Position | InputTrackingState.Rotation) : InputTrackingState.None;
            InputSystem.QueueStateEvent<SVRDeviceState>(left_device, left_state);
        }

        if (right_device != null) {
            long right_poll_timestamp = svr.SVRInputApi.SVRTimeNow();
            SVRUpdatePose(right_poll_timestamp, svr.SVRInputApi.Chirality.Right, ref right_state);
            right_poll_timestamp_queue_.Enqueue(right_poll_timestamp);

            right_state.tracking_state_ = svr.SVRInputApi.SVRIsConnected(1) ? (InputTrackingState.Position | InputTrackingState.Rotation) : InputTrackingState.None;
            InputSystem.QueueStateEvent<SVRDeviceState>(right_device, right_state);

	    }
#endif
    }

    [MonoPInvokeCallback(typeof(svr.SVRInputApi.ButtonCallbackDelegate))]
    static void ButtonInputCallback(long timestamp, int chirality, svr.SVRInputApi.Buttons buttons) {
        if (chirality == 0) {
            UpdateButtonInput(ref left_state, buttons);
        }
        if (chirality == 1) {
            UpdateButtonInput(ref right_state, buttons);
        }
    }

    static void UpdateButtonInput(ref SVRDeviceState state, svr.SVRInputApi.Buttons buttons) {
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

    void SVRUpdatePose(long poll_timestamp,int chirality,ref SVRDeviceState state) {
        svr.SVRInputApi.SVRPose pose = new svr.SVRInputApi.SVRPose();
        svr.SVRInputApi.SVRVector3f linear_velocity = new svr.SVRInputApi.SVRVector3f();
        var angular_velocity = new svr.SVRInputApi.SVRVector3f();
        SVRControllerPose(poll_timestamp, chirality, ref pose, ref linear_velocity, ref angular_velocity);
        state.rotation_ = new Quaternion(pose.qx, pose.qy, pose.qz, pose.qw);
        state.position_ = new Vector3(pose.x, pose.y, pose.z);
        state.deviceVelocity_ = new Vector3(linear_velocity.x, linear_velocity.y, linear_velocity.z);
        state.deviceAngularVelocity_ = new Vector3(angular_velocity.x, angular_velocity.y, angular_velocity.z);
    }

    void SVRControllerPose(long poll_timestamp, int chirality, ref svr.SVRInputApi.SVRPose pose, ref svr.SVRInputApi.SVRVector3f linear_velocity, ref svr.SVRInputApi.SVRVector3f angular_velocity)
    {
#if UNITY_IOS || UNITY_VISIONOS
        svr.SVRInputApi.SVRPollDevicePose(chirality, poll_timestamp, ref pose, ref linear_velocity, ref angular_velocity, IntPtr.Zero);
#endif
    }

    void OnEnable()
    {
        if (svr_input_control == null)
        {
            svr_input_control = new SVRInputControl();
        }
        svr_input_control.SVRControl.Enable();
        Debug.Log("Manager Enable");
    }

    void OnDisable()
    {
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

    public bool IsControllerConnected(int chirality)
    {
#if UNITY_IOS || UNITY_VISIONOS
        return svr.SVRInputApi.SVRIsConnected(chirality);
#else
        return false;
#endif
    }

    public void SVRHaptic(int chirality, float amplitude, float frequency, double duration_seconds)
    {
#if UNITY_IOS || UNITY_VISIONOS
        svr.SVRInputApi.SVRHapticContinuous(chirality, amplitude, frequency, duration_seconds);
#endif
    }

    public void SVRStop()
    {
        svr.SVRInputApi.SVRStop();
    }
}
