using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Runtime.InteropServices;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.LowLevel;

using UnityEngine.InputSystem.Controls;
using TMPro;
using UnityEngine.Assertions;
namespace dm {
    public class DMInputApi : MonoBehaviour
    {
#if UNITY_IOS || UNITY_VISIONOS
        [DllImport("__Internal")]
        public static extern void DMControllerStart();

        [DllImport("__Internal")]
        public static extern void DMControllerStop();

        [StructLayout(LayoutKind.Sequential)] 
        public struct DMPose{

            public float x, y, z;
            public float qw, qx, qy, qz;
	    }

        [StructLayout(LayoutKind.Sequential)] 
        public struct DMVector3f {
            public float x, y, z;
	    };

        [DllImport("__Internal")]
        public static extern void DMControllerQueryPose(long timestamp, int hand_type, ref DMPose pose, ref DMVector3f linear_velocity, ref DMVector3f angular_velocity);

        [StructLayout(LayoutKind.Sequential)] 
        public struct Buttons{
            public byte primary_button;
            public byte secondary_button;
            public byte menu_button;
            public byte stick_z_button;
            public byte trigger_button_value;
            public byte grip_button_value;
            public byte stick_x_value;
            public byte stick_y_value;
	    }



        public delegate void ButtonCallbackDelegate(long timestamp, int hand_type, Buttons buttons);

        [DllImport("__Internal")]
        public static extern void DMControllerSetButtonCallback(IntPtr callback_ptr);
        
        public static class HapticMode {
            public static readonly int kAMPLITUDE_15_FREQUENCY_100 = 0x0000;
            public static readonly int kAMPLITUDE_15_FREQUENCY_170 = 0x0100;
            public static readonly int kAMPLITUDE_15_FREQUENCY_300 = 0x0200;
            public static readonly int kAMPLITUDE_20_FREQUENCY_100 = 0x0001;
            public static readonly int kAMPLITUDE_20_FREQUENCY_170 = 0x0101;
            public static readonly int kAMPLITUDE_20_FREQUENCY_300 = 0x0201;
            public static readonly int kAMPLITUDE_30_FREQUENCY_100 = 0x0002;
            public static readonly int kAMPLITUDE_30_FREQUENCY_170 = 0x0102;
            public static readonly int kAMPLITUDE_30_FREQUENCY_300 = 0x0202;
        }
        [DllImport("__Internal")]
        public static extern void DMControllerHaptic(int hand_type, int mode, int duration_ms);

        [DllImport("__Internal")]
        public static extern bool DMControllerIsConnected(int hand_type);

        [DllImport("__Internal")]
        public static extern bool DMControllerBatteryLevel(int hand_type, ref byte battery_level);

        [DllImport("__Internal")]
        public static extern void GotHandPose(int hand_type, double[] pose);
#endif
    }
}
