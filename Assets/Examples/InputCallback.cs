using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class InputCallback : MonoBehaviour, SVRInputControl.ISVRControlActions
{


    void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
    {
        if (Application.isPlaying)
            property.DisableDirectAction();

        property = value;

        if (Application.isPlaying && isActiveAndEnabled)
            property.EnableDirectAction();
    }

    protected virtual float ReadValue(InputAction action)
    {
        if (action == null)
            return default;

        if (action.activeControl is AxisControl)
            return action.ReadValue<float>();

        if (action.activeControl is Vector2Control)
            return action.ReadValue<Vector2>().magnitude;

        return IsPressed(action) ? 1f : 0f;
    }

    protected virtual bool IsPressed(InputAction action)
    {
        if (action == null)
            return false;

#if INPUT_SYSTEM_1_1_OR_NEWER || INPUT_SYSTEM_1_1_PREVIEW // 1.1.0-preview.2 or newer, including pre-release
                return action.phase == InputActionPhase.Performed;
#else
        if (action.activeControl is ButtonControl buttonControl)
            return buttonControl.isPressed;

        if (action.activeControl is AxisControl)
            return action.ReadValue<float>() >= 0.5;

        return action.triggered || action.phase == InputActionPhase.Performed;
#endif
    }

    public TMP_Text LeftControllerInputText;
    public TMP_Text RightControllerInputText;
    class ControllerData
    {

        private Vector3 position = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private bool primary_value = false;
        private bool secondary_value = false;
        private bool menu_value = false;
        private bool grip_button = false;
        private bool trigger_button = false;

        private float trigger = -100;
        private float grip = -100;

        private float x_axis = 0.0f;
        private float y_axis = 0.0f;
        private int track_state = -100;

        private bool thumbstick_clicked_= false;
        private Vector3 device_velocity_ = Vector3.zero;
        private Vector3 device_angular_velocity_ = Vector3.zero;

        public float Grip
        {
            get => grip;
            set => grip = value;
        }

        public float XAxis
        {
            get => x_axis;
            set => x_axis = value;
        }

        public float YAxis
        {
            get => y_axis;
            set => y_axis = value;
        }

        public bool MenuButton
        {
            get => menu_value;
            set => menu_value = value;
        }
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }
        public bool PrimaryButton
        {
            get => primary_value;
            set => primary_value = value;
        }

        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        public bool SecondaryButton
        {
            get => secondary_value;
            set => secondary_value = value;
        }

        public int TrackState
        {
            get => track_state;
            set => track_state = value;
        }

        public float Trigger
        {
            get => trigger;
            set => trigger = value;
        }

        public bool GripButton
        {
            get => grip_button;
            set => grip_button = value;
        }

        public bool TriggerButton
        {
            get => trigger_button;
            set => trigger_button = value;
        }

        public Vector3 DeviceVelocity {
            get => device_velocity_;
            set => device_velocity_ = value;
	    }

        public Vector3 DeviceAngularVelocity {
            get => device_angular_velocity_;
            set => device_angular_velocity_ = value;
	    }

        public bool ThumbstickClicked {
            get => thumbstick_clicked_;
            set => thumbstick_clicked_ = value;
	    }
    }

    ControllerData left_controller_data;
    ControllerData right_controller_data;

    SVRInputControl dm_input_controller;

    public void OnLeftControllerGrip(InputAction.CallbackContext context)
    {
        left_controller_data.Grip = context.action.ReadValue<float>();
    }

    public void OnRightControllerGrip(InputAction.CallbackContext context)
    {
        right_controller_data.Grip = context.action.ReadValue<float>();
    }

    public void OnLeftControllerMenuButton(InputAction.CallbackContext context)
    {
        left_controller_data.MenuButton = context.action.triggered;
    }

    public void OnRightControllerMenuButton(InputAction.CallbackContext context)
    {
        right_controller_data.MenuButton = context.action.triggered;

    }

    public void OnLeftControllerPosition(InputAction.CallbackContext context)
    {
        left_controller_data.Position = context.action.ReadValue<Vector3>();
    }

    public void OnRightControllerPosition(InputAction.CallbackContext context)
    {
        right_controller_data.Position = context.action.ReadValue<Vector3>();
    }

    public void OnLeftControllerPrimaryButton(InputAction.CallbackContext context)
    {
        Debug.Log("Primary Call");
        left_controller_data.PrimaryButton = context.action.triggered;
    }
    public void OnRightControllerPrimaryButton(InputAction.CallbackContext context)
    {
        right_controller_data.PrimaryButton = context.action.triggered;
    }

    public void OnLeftControllerRotation(InputAction.CallbackContext context)
    {
        left_controller_data.Rotation = context.action.ReadValue<Quaternion>();
    }
    public void OnRightControllerRotation(InputAction.CallbackContext context)
    {
        right_controller_data.Rotation = context.action.ReadValue<Quaternion>();
    }

    public void OnLeftControllerSecondaryButton(InputAction.CallbackContext context)
    {
        left_controller_data.SecondaryButton = context.action.triggered;
    }

    public void OnRightControllerSecondaryButton(InputAction.CallbackContext context)
    {
        right_controller_data.SecondaryButton = context.action.triggered;
    }

    public void OnLeftControllerStick(InputAction.CallbackContext context)
    {
        Vector2 axis = context.action.ReadValue<Vector2>();
        left_controller_data.XAxis =  axis.x;
        left_controller_data.YAxis = axis.y;
    }

    public void OnRightControllerStick(InputAction.CallbackContext context)
    {
        Vector2 axis = context.action.ReadValue<Vector2>();
        right_controller_data.XAxis = axis.x;
        right_controller_data.YAxis = axis.y;
    }

    public void OnLeftControllerTrackingState(InputAction.CallbackContext context)
    {
        left_controller_data.TrackState = context.action.ReadValue<int>();
    }
    public void OnRightControllerTrackingState(InputAction.CallbackContext context)
    {
        right_controller_data.TrackState = context.action.ReadValue<int>();
    }

    public void OnLeftControllerTrigger(InputAction.CallbackContext context)
    {
        left_controller_data.Trigger = context.action.ReadValue<float>();
    }

    public void OnRightControllerTrigger(InputAction.CallbackContext context)
    {
        right_controller_data.Trigger = context.action.ReadValue<float>();
    }

    public void OnLeftControllerGripButton(InputAction.CallbackContext context)
    {
        left_controller_data.GripButton = context.action.triggered;
    }

    public void OnRightControllerGripButton(InputAction.CallbackContext context)
    {
        right_controller_data.GripButton = context.action.triggered;
    }

    public void OnLeftControllerTriggerButton(InputAction.CallbackContext context)
    {
        left_controller_data.TriggerButton = context.action.triggered;
    }

    public void OnRightControllerTriggerButton(InputAction.CallbackContext context)
    {
        right_controller_data.TriggerButton = context.action.triggered;
    }

    public void OnLeftControllerDeviceVelocity(InputAction.CallbackContext context) {
        left_controller_data.DeviceVelocity = context.action.ReadValue<Vector3>();
    }

    public void OnRightControllerDeviceVelocity(InputAction.CallbackContext context) { 
        right_controller_data.DeviceVelocity = context.action.ReadValue<Vector3>();
    }

    public void OnLeftControllerDeviceAngularVelocity(InputAction.CallbackContext context) { 
        left_controller_data.DeviceAngularVelocity= context.action.ReadValue<Vector3>();
    }

    public void OnRightControllerDeviceAngularVelocity(InputAction.CallbackContext context) { 
        right_controller_data.DeviceAngularVelocity= context.action.ReadValue<Vector3>();
    }

    public void OnLeftControllerPrimary2DAxisClick(InputAction.CallbackContext context) {
        left_controller_data.ThumbstickClicked = context.action.triggered;
    }

    public void OnRightControllerPrimary2DAxisClick(InputAction.CallbackContext context) { 
        right_controller_data.ThumbstickClicked = context.action.triggered;
    }

    void UpdateText()
    {
        if (LeftControllerInputText != null)
        {
            string text = string.Format(
        "X:{0}\nY: {1}\n Menu:{2}\n ThumbStick:{14}\nTriggerValue:{3}\nTrigger:{10}\n GripValue:{4}\n Grip:{11}\n JoyStick-X:{5}\n Joystick-Y:{6}\n TrackingState:{7}\n Rotation:{8}\n Position:{9}\n Velocity:{12}\n AngularVelocity:{13}\n",
                left_controller_data.PrimaryButton, left_controller_data.SecondaryButton, left_controller_data.MenuButton,
                left_controller_data.Trigger, left_controller_data.Grip, left_controller_data.XAxis, left_controller_data.YAxis, left_controller_data.TrackState,
                left_controller_data.Rotation, left_controller_data.Position, left_controller_data.TriggerButton, left_controller_data.GripButton,
                left_controller_data.DeviceVelocity,left_controller_data.DeviceAngularVelocity, left_controller_data.ThumbstickClicked
            );
            LeftControllerInputText.text = text;
        }

        if (RightControllerInputText != null)
        {
            string text = string.Format(
        "A:{0}\nB: {1}\n Menu:{2}\n ThumStick:{14}\nTriggerValue:{3}\nTrigger:{10}\n GripValue:{4}\n Grip:{11}\n JoyStick-X:{5}\n Joystick-Y:{6}\n TrackingState:{7}\n Rotation:{8}\n Position:{9}\n Velocity:{12}\n AngularVelocity:{13}\n",
                right_controller_data.PrimaryButton, right_controller_data.SecondaryButton, right_controller_data.MenuButton,
                right_controller_data.Trigger, right_controller_data.Grip, right_controller_data.XAxis, right_controller_data.YAxis, right_controller_data.TrackState,
                right_controller_data.Rotation, right_controller_data.Position, right_controller_data.TriggerButton, right_controller_data.GripButton,
                right_controller_data.DeviceVelocity,right_controller_data.DeviceAngularVelocity, right_controller_data.ThumbstickClicked
                );
            RightControllerInputText.text = text;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        left_controller_data = new ControllerData();
        right_controller_data = new ControllerData();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    void OnEnable()
    {
        if (dm_input_controller == null)
        {
            dm_input_controller = new SVRInputControl();
            dm_input_controller.SVRControl.SetCallbacks(this);
        }
        dm_input_controller.SVRControl.Enable();
    }

    void OnDisable()
    {
        dm_input_controller.Disable();
    }
}
