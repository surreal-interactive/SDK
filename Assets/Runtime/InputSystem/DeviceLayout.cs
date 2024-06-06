#if UNITY_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.DMProvider.Input
{
    /// <summary>
    /// A PICO Controller
    /// </summary>
    [Preserve]
    [InputControlLayout(displayName = "PICO Controller", commonUsages = new[] { "LeftHand", "RightHand" })]
    public class PXR_Controller : XRControllerWithRumble
    {
        [Preserve]
        [InputControl(aliases = new[] { "Primary2DAxis", "Touchpad" })]
        public Vector2Control thumbstick { get; private set; }

        [Preserve]
        [InputControl]
        public AxisControl trigger { get; private set; }
        [Preserve]
        [InputControl]
        public AxisControl grip { get; private set; }

        [Preserve]
        [InputControl(aliases = new[] { "A", "X" })]
        public ButtonControl primaryButton { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "B", "Y" })]
        public ButtonControl secondaryButton { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "GripButton", "GripPress" })]
        public ButtonControl gripPressed { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "Primary2DAxisClick", "TouchpadPress" })]
        public ButtonControl thumbstickClicked { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "ATouch", "XTouch" })]
        public ButtonControl primaryTouched { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "BTouch", "YTouch" })]
        public ButtonControl secondaryTouched { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "TriggerTouch" })]
        public ButtonControl triggerTouched { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "TriggerPress" })]
        public ButtonControl triggerPressed { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "Menu" })]
        public ButtonControl menu { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "TouchpadTouch" })]
        public ButtonControl touchpadTouched { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "ThumbRestTouch" })]
        public ButtonControl thumbstickTouched { get; private set; }

        [Preserve]
        [InputControl(aliases = new[] { "controllerTrackingState" })]
        public new IntegerControl trackingState { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "ControllerIsTracked" })]
        public new ButtonControl isTracked { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "controllerPosition" })]
        public new Vector3Control devicePosition { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "controllerRotation" })]
        public new QuaternionControl deviceRotation { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "controllerVelocity" })]
        public Vector3Control deviceVelocity { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "controllerAngularVelocity" })]
        public Vector3Control deviceAngularVelocity { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "controllerAcceleration" })]
        public Vector3Control deviceAcceleration { get; private set; }
        [Preserve]
        [InputControl(aliases = new[] { "controllerAngularAcceleration" })]
        public Vector3Control deviceAngularAcceleration { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            thumbstick = GetChildControl<Vector2Control>("thumbstick");
            trigger = GetChildControl<AxisControl>("trigger");
            triggerTouched = GetChildControl<ButtonControl>("triggerTouched");
            grip = GetChildControl<AxisControl>("grip");

            primaryButton = GetChildControl<ButtonControl>("primaryButton");
            secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
            gripPressed = GetChildControl<ButtonControl>("gripPressed");
            thumbstickClicked = GetChildControl<ButtonControl>("thumbstickClicked");
            primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
            secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
            thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");
            triggerPressed = GetChildControl<ButtonControl>("triggerPressed");

            trackingState = GetChildControl<IntegerControl>("trackingState");
            isTracked = GetChildControl<ButtonControl>("isTracked");
            devicePosition = GetChildControl<Vector3Control>("devicePosition");
            deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
            deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
            deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
            deviceAcceleration = GetChildControl<Vector3Control>("deviceAcceleration");
            deviceAngularAcceleration = GetChildControl<Vector3Control>("deviceAngularAcceleration");
        }
    }
}
}
#endif
