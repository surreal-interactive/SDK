using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
#if UNITY_EDITOR
using UnityEditor;
#endif
public struct SVRDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('C', 'U', 'S', 'T');

    [InputControl(
        name = "PrimaryButton",
        layout = "Button",
        bit = 0,
        displayName = "Primary Button",
        usage = "primaryButton",
        aliases = new[] { "A", "X" }
    )]
    [InputControl(
        name = "SecondaryButton",
        layout = "Button",
        bit = 1,
        displayName = "Second Button",
        usage = "SecondaryButton",
        aliases = new[] { "B", "Y" }
    )]
    [InputControl(
        name = "MenuButton",
        layout = "Button",
        bit = 2,
        displayName = "Menu Button",
        aliases = new[] { "Menu" }
    )]
    [InputControl(
        name = "TriggerButton",
        layout = "Button",
        bit = 3,
        displayName = "Trigger Button",
        usage = "TriggerButton"
    )]
    [InputControl(
        name = "GripButton",
        layout = "Button",
        bit = 4,
        displayName = "Grip Button",
        aliases = new[] { "GripButton", "GripPress" },
        usage = "GripButton"
    )]
    [InputControl(
        name = "Primary2DAxisClick",
        layout = "Button",
        bit = 5,
        displayName = "Primary2DAxisClick",
        usage = "Primary2DAxisClick",
        aliases = new[] { "Primary2DAxisClick", "TouchpadPress" }
    )]
    public int buttons_;

    [InputControl(
        name = "Trigger",
        layout = "Axis",
        displayName = "Trigger",
        format = "BYTE",
        parameters = "normalize,normalizeMax = 0.5, normalizeMin=0",
        usage = "Trigger"
    )]
    public byte trigger_;

    [InputControl(
        name = "Grip",
        layout = "Axis",
        displayName = "Grip",
        format = "BYTE",
        parameters = "normalize,normalizeMax = 0.5, normalizeMin=0",
        usage = "Grip"
    )]
    public byte grip_;

    [InputControl(name = "Primary2DAxis", format = "VC2B", layout = "Stick")]
    [InputControl(
        name = "Primary2DAxis/x",
        defaultState = 127,
        format = "BYTE",
        offset = 0,
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5"
    )]
    public byte x_;

    [InputControl(
        name = "Primary2DAxis/y",
        defaultState = 127,
        format = "BYTE",
        offset = 1,
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5"
    )]
    public byte y_;

    [InputControl(name = "deviceRotation", layout = "Quaternion", displayName = "SVRRotation")]
    public UnityEngine.Quaternion rotation_;

    [InputControl(name = "devicePosition", layout = "Vector3", displayName = "SVRPosition")]
    public UnityEngine.Vector3 position_;

    [InputControl(name = "trackingState", layout = "Integer", displayName = "SVRTrackingState")]
    public InputTrackingState tracking_state_;

    [InputControl(
        name = "deviceVelocity",
        layout = "Vector3",
        displayName = "DeviceVelocity",
        usage = "controllerVelocity"
    )]
    public UnityEngine.Vector3 deviceVelocity_;

    [InputControl(
        name = "deviceAngularVelocity",
        layout = "Vector3",
        displayName = "DeviceAngularVelocity",
        usage = "controllerAngularVelocity"
    )]
    public UnityEngine.Vector3 deviceAngularVelocity_;
}

#if UNITY_EDITOR
[InitializeOnLoad] // Call static class constructor in editor.
#endif
[InputControlLayout(
    stateType = typeof(SVRDeviceState),
    displayName = "SVRController",
    commonUsages = new[] { "LeftHand", "RightHand" }
)]
public class SVRInputDevice : XRControllerWithRumble, IInputUpdateCallbackReceiver
{
#if UNITY_EDITOR
    static SVRInputDevice()
    {
        // Trigger our RegisterLayout code in the editor.
        Initialize();
    }
#endif

    // In the player, [RuntimeInitializeOnLoadMethod] will make sure our
    // initialization code gets called during startup.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Register our device with the input system. We also register
        // a "device matcher" here. These are used when a device is discovered
        // by the input system. Each device is described by an InputDeviceDescription
        // and an InputDeviceMatcher can be used to match specific properties of such
        // a description. See the documentation of InputDeviceMatcher for more
        // details.
        //
        // NOTE: In case your device is more dynamic in nature and cannot have a single
        //       static layout, there is also the possibility to build layouts on the fly.
        //       Check out the API documentation for InputSystem.onFindLayoutForDevice and
        //       for InputSystem.RegisterLayoutBuilder.
        InputSystem.RegisterLayout<SVRInputDevice>(
            matches: new InputDeviceMatcher().WithInterface("SVRInputDevice")
        );
    }

    public ButtonControl PrimaryButton { get; protected set; }
    public ButtonControl SecondaryButton { get; protected set; }
    public ButtonControl MenuButton { get; protected set; }
    public ButtonControl TriggerButton { get; protected set; }
    public ButtonControl GripButton { get; protected set; }
    public AxisControl Trigger { get; protected set; }
    public AxisControl Grip { get; protected set; }
    public StickControl Stick { get; protected set; }
    public Vector3Control DeviceVelocity { get; protected set; }
    public Vector3Control DeviceAngularVelocity { get; protected set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        PrimaryButton = GetChildControl<ButtonControl>("PrimaryButton");
        SecondaryButton = GetChildControl<ButtonControl>("SecondaryButton");
        MenuButton = GetChildControl<ButtonControl>("MenuButton");
        Trigger = GetChildControl<AxisControl>("Trigger");
        Grip = GetChildControl<AxisControl>("Grip");
        Stick = GetChildControl<StickControl>("Primary2DAxis");

        deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
        devicePosition = GetChildControl<Vector3Control>("devicePosition");
        trackingState = GetChildControl<IntegerControl>("trackingState");

        TriggerButton = GetChildControl<ButtonControl>("TriggerButton");
        GripButton = GetChildControl<ButtonControl>("GripButton");

        DeviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
        DeviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
    }

    protected override void OnAdded() { }

    public static SVRInputDevice current { get; private set; }

    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
            current = null;
    }

    public void OnUpdate() { }

#if UNITY_EDITOR
    [MenuItem("Tools/SVRInputDevice/Create Device")]
    private static void CreateDevice()
    {
        // This is the code that you would normally run at the point where
        // you discover devices of your custom type.
        InputSystem.AddDevice(
            new InputDeviceDescription { interfaceName = "SVRInputDevice", product = "DeepMirror" }
        );
    }

    // For completeness sake, let's also add code to remove one instance of our
    // custom device. Note that you can also manually remove the device from
    // the input debugger by right-clicking in and selecting "Remove Device".
    [MenuItem("Tools/SVRInputDevice/Remove Device")]
    private static void RemoveDevice()
    {
        var customDevice = InputSystem.devices.FirstOrDefault(x => x is SVRInputDevice);
        if (customDevice != null)
            InputSystem.RemoveDevice(customDevice);
    }
#endif
}
