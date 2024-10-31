using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SVRInput : MonoBehaviour, SVRInputControl.ISVRControlActions
{
    public enum Button
    {
        A,
        B,
        X,
        Y,
        Start,
        Back,
        LIndexTrigger,
        LHandTrigger,
        LThumbstick,
        LThumbstickUp,
        LThumbstickDown,
        LThumbstickLeft,
        LThumbstickRight,
        RIndexTrigger,
        RHandTrigger,
        RThumbstick,
        RThumbstickUp,
        RThumbstickDown,
        RThumbstickLeft,
        RThumbstickRight,
    }

    public enum Axis2D
    {
        LThumbstick,
        RThumbstick
    }

    public enum Axis1D
    {
        LIndexTrigger,
        LHandTrigger,
        RIndexTrigger,
        RHandTrigger
    }

    public static SVRInput instance;
    public SVRControllerManager svrControllerManager;
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

    SVRInputControl svr_input_controller;

    public static bool GetDown(SVRInput.Button button)
    {
        if (button == Button.A)
        {
            return instance.right_controller_data.PrimaryButton;
        }
        else if (button == Button.B)
        {
            return instance.right_controller_data.SecondaryButton;
        }
        else if (button == Button.X)
        {
            return instance.left_controller_data.PrimaryButton;
        }
        else if (button == Button.Y)
        {
            return instance.left_controller_data.SecondaryButton;
        }
        else if (button == Button.LIndexTrigger)
        {
            return instance.left_controller_data.TriggerButton;
        }
        else if (button == Button.LHandTrigger)
        {
            return instance.left_controller_data.GripButton;
        }
        else if (button == Button.RIndexTrigger)
        {
            return instance.right_controller_data.TriggerButton;
        }
        else if (button == Button.RHandTrigger)
        {
            return instance.right_controller_data.GripButton;
        }
        else if (button == Button.Start)
        {
            return instance.left_controller_data.MenuButton;
        }
        else if (button == Button.Back)
        {
            return instance.right_controller_data.MenuButton;
        }
        else if (button == Button.LThumbstick)
        {
            return instance.left_controller_data.ThumbstickClicked;
        }
        else if (button == Button.RThumbstick)
        {
            return instance.right_controller_data.ThumbstickClicked;
        }
        else if (button == Button.LThumbstickUp)
        {
            if (instance.left_controller_data.YAxis > 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.LThumbstickDown)
        {
            if (instance.left_controller_data.YAxis < 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.LThumbstickRight)
        {
            if (instance.left_controller_data.XAxis > 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.LThumbstickLeft)
        {
            if (instance.left_controller_data.XAxis < 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickUp)
        {
            if (instance.right_controller_data.YAxis > 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickDown)
        {
            if (instance.right_controller_data.YAxis < 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickLeft)
        {
            if (instance.right_controller_data.XAxis < 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickRight)
        {
            if (instance.right_controller_data.XAxis > 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static bool GetUp(SVRInput.Button button)
    {
        if (button == Button.A)
        {
            return !instance.right_controller_data.PrimaryButton;
        }
        else if (button == Button.B)
        {
            return !instance.right_controller_data.SecondaryButton;
        }
        else if (button == Button.X)
        {
            return !instance.left_controller_data.PrimaryButton;
        }
        else if (button == Button.Y)
        {
            return !instance.left_controller_data.SecondaryButton;
        }
        else if (button == Button.LIndexTrigger)
        {
            return !instance.left_controller_data.TriggerButton;
        }
        else if (button == Button.LHandTrigger)
        {
            return !instance.left_controller_data.GripButton;
        }
        else if (button == Button.RIndexTrigger)
        {
            return !instance.right_controller_data.TriggerButton;
        }
        else if (button == Button.RHandTrigger)
        {
            return !instance.right_controller_data.GripButton;
        }
        else if (button == Button.Start)
        {
            return !instance.left_controller_data.MenuButton;
        }
        else if (button == Button.Back)
        {
            return !instance.right_controller_data.MenuButton;
        }
        else if (button == Button.LThumbstick)
        {
            return !instance.left_controller_data.ThumbstickClicked;
        }
        else if (button == Button.RThumbstick)
        {
            return !instance.right_controller_data.ThumbstickClicked;
        }
        else if (button == Button.LThumbstickUp)
        {
            if (instance.left_controller_data.YAxis < 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.LThumbstickDown)
        {
            if (instance.left_controller_data.YAxis > 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.LThumbstickLeft)
        {
            if (instance.left_controller_data.XAxis > 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.LThumbstickRight)
        {
            if (instance.left_controller_data.XAxis < 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickUp)
        {
            if (instance.right_controller_data.YAxis < 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickDown)
        {
            if (instance.right_controller_data.YAxis > 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickLeft)
        {
            if (instance.right_controller_data.XAxis > 88.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (button == Button.RThumbstickRight)
        {
            if (instance.right_controller_data.XAxis < 168.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static Vector2 Get(SVRInput.Axis2D thumbstick)
    {
        if (thumbstick == Axis2D.LThumbstick)
        {
            Vector2 axisVal = new Vector2(instance.left_controller_data.XAxis, instance.left_controller_data.YAxis);
            return axisVal;
        }
        else
        {
            Vector2 axisVal = new Vector2(instance.right_controller_data.XAxis, instance.right_controller_data.YAxis);
            return axisVal;
        }
    }

    public static float Get(SVRInput.Axis1D button)
    {
        if (button == Axis1D.LIndexTrigger)
        {
            return instance.left_controller_data.Trigger;
        }
        else if (button == Axis1D.LHandTrigger)
        {
            return instance.left_controller_data.Grip;
        }
        else if (button == Axis1D.RIndexTrigger)
        {
            return instance.right_controller_data.Trigger;
        }
        else if (button == Axis1D.RHandTrigger)
        {
            return instance.right_controller_data.Grip;
        }
        else
        {
            return 0.0f;
        }
    }

    public static bool IsControllerInitialized()
    {
        return instance.svrControllerManager.IsControllerInitialized();
    }

    public static bool IsControllerConnected(int handType)
    {
        return instance.svrControllerManager.IsControllerConnected(handType);
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
        if (svr_input_controller == null)
        {
            svr_input_controller = new SVRInputControl();
            svr_input_controller.SVRControl.SetCallbacks(this);
        }
        svr_input_controller.SVRControl.Enable();
    }

    void OnDisable()
    {
        svr_input_controller.Disable();
    }

    public Camera GetXRMainCamera()
    {
        return xrCamera;
    }

    public static Vector3 GetLeftControllerPosition()
    {
        return instance.left_controller_data.Position;
    }

    public static Quaternion GetLeftControllerRotation()
    {
        return instance.left_controller_data.Rotation;
    }

    public static Vector3 GetRightControllerPosition()
    {
        return instance.right_controller_data.Position;
    }

    public static Quaternion GetRightControllerRotation()
    {
        return instance.right_controller_data.Rotation;
    }

    public static Vector3 GetLeftControllerVelocity()
    {
        return instance.left_controller_data.DeviceVelocity;
    }

    public static Vector3 GetRightControllerVelocity()
    {
        return instance.right_controller_data.DeviceVelocity;
    }

    public static Vector3 GetLeftControllerAngularVelocity()
    {
        return instance.left_controller_data.DeviceAngularVelocity;
    }

    public static Vector3 GetRightControllerAngularVelocity()
    {
        return instance.right_controller_data.DeviceAngularVelocity;
    }

    /**
     * In case controllers may have latency, this api tries to predict 
     *  pose of controllers in certain time.
     *  
     *  predictTime: predicted time in nano seconds
     */

    /*
    public static void SetupPredictTimeInNanoSeconds(int predictTime)
    {
        instance.svrControllerManager.SetupPredictTime(predictTime);
    }
    */

    public static void SVRStop()
    {
        instance.svrControllerManager.SVRStop();
    }
}
