using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class DMControllerManager : MonoBehaviour, DMInputControl.IDMControlActions
{
    public static DMControllerManager instance;
    public DMManager dmManager;
    public Camera xrCamera;

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

        private bool thumbstick_clicked_ = false;
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

        public Vector3 DeviceVelocity
        {
            get => device_velocity_;
            set => device_velocity_ = value;
        }

        public Vector3 DeviceAngularVelocity
        {
            get => device_angular_velocity_;
            set => device_angular_velocity_ = value;
        }

        public bool ThumbstickClicked
        {
            get => thumbstick_clicked_;
            set => thumbstick_clicked_ = value;
        }
    }

    ControllerData left_controller_data;
    ControllerData right_controller_data;

    DMInputControl dm_input_controller;

    public bool IsControllerInitialized()
    {
        return dmManager.IsControllerInitialized();
    }

    public bool IsControllerConnected(int handType)
    {
        return dmManager.IsControllerConnected(handType);
    }

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
        left_controller_data.XAxis = axis.x;
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

    public void OnLeftControllerDeviceVelocity(InputAction.CallbackContext context)
    {
        left_controller_data.DeviceVelocity = context.action.ReadValue<Vector3>();
    }

    public void OnRightControllerDeviceVelocity(InputAction.CallbackContext context)
    {
        right_controller_data.DeviceVelocity = context.action.ReadValue<Vector3>();
    }

    public void OnLeftControllerDeviceAngularVelocity(InputAction.CallbackContext context)
    {
        left_controller_data.DeviceAngularVelocity = context.action.ReadValue<Vector3>();
    }

    public void OnRightControllerDeviceAngularVelocity(InputAction.CallbackContext context)
    {
        right_controller_data.DeviceAngularVelocity = context.action.ReadValue<Vector3>();
    }

    public void OnLeftControllerPrimary2DAxisClick(InputAction.CallbackContext context)
    {
        left_controller_data.ThumbstickClicked = context.action.triggered;
    }

    public void OnRightControllerPrimary2DAxisClick(InputAction.CallbackContext context)
    {
        right_controller_data.ThumbstickClicked = context.action.triggered;
    }

    private void Awake()
    {
        instance = this;
        left_controller_data = new ControllerData();
        right_controller_data = new ControllerData();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        if (dm_input_controller == null)
        {
            dm_input_controller = new DMInputControl();
            dm_input_controller.DMControl.SetCallbacks(this);
        }
        dm_input_controller.DMControl.Enable();
    }

    void OnDisable()
    {
        dm_input_controller.Disable();
    }

    public Camera GetXRMainCamera()
    {
        return xrCamera;
    }

    public Vector3 GetLeftControllerPosition()
    {
        return left_controller_data.Position;
    }

    public Quaternion GetLeftControllerRotation()
    {
        return left_controller_data.Rotation;
    }

    public Vector3 GetRightControllerPosition()
    {
        return right_controller_data.Position;
    }

    public Quaternion GetRightControllerRotation()
    {
        return right_controller_data.Rotation;
    }

    public Vector3 GetLeftControllerVelocity()
    {
        return left_controller_data.DeviceVelocity;
    }

    public Vector3 GetRightControllerVelocity()
    {
        return right_controller_data.DeviceVelocity;
    }

    public Vector3 GetLeftControllerAngularVelocity()
    {
        return left_controller_data.DeviceAngularVelocity;
    }

    public Vector3 GetRightControllerAngularVelocity()
    {
        return right_controller_data.DeviceAngularVelocity;
    }

    public Vector2 GetLeftControllerStickValue()
    {
        Vector2 stickValues = new Vector2();
        stickValues.x = left_controller_data.XAxis;
        stickValues.y = left_controller_data.YAxis;
        return stickValues;
    }

    public Vector2 GetRightControllerStickValue()
    {
        Vector2 stickValues = new Vector2();
        stickValues.x = right_controller_data.XAxis;
        stickValues.y = right_controller_data.YAxis;
        return stickValues;
    }

    public bool GetLeftTriggerDown()
    {
        return left_controller_data.TriggerButton;
    }

    public bool GetRightTriggerDown()
    {
        return right_controller_data.TriggerButton;
    }

    public float GetLeftControllerTrigger()
    {
        return left_controller_data.Trigger;
    }

    public float GetRightControllerTrigger()
    {
        return right_controller_data.Trigger;
    }

    public bool GetLeftGripDown()
    {
        return left_controller_data.GripButton;
    }

    public bool GetRightGripDown()
    {
        return right_controller_data.GripButton;
    }

    public float GetLeftControllerGrip()
    {
        return left_controller_data.Grip;
    }

    public float GetRightControllerGrip()
    {
        return right_controller_data.Grip;
    }
}
