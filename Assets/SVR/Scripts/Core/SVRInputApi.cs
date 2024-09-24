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
using UnityEngine.Assertions;
namespace svr {
    public class SVRInputApi : MonoBehaviour
    {

        [StructLayout(LayoutKind.Sequential)] 
        public struct SVRPose{

            public float x, y, z;
            public float qw, qx, qy, qz;
	    }

        [StructLayout(LayoutKind.Sequential)] 
        public struct SVRVector3f {
            public float x, y, z;
	    }

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

        public static class Chirality
        {
            public static readonly int Left = 0;
            public static readonly int Right = 0;
        }

#if UNITY_IOS || UNITY_VISIONOS
        [DllImport("__Internal")]
        public static extern void SVRStart();

        [DllImport("__Internal")]
        public static extern void SVRStop();

        [DllImport("__Internal")]
        public static extern void SVRQueryDevicePose(long timestamp, int hand_type, ref SVRPose pose, ref SVRVector3f linear_velocity, ref SVRVector3f angular_velocity, IntPtr hand_skeletons);

        [DllImport("__Internal")]
        public static extern void SVRSetButtonCallback(IntPtr callback_ptr);
        
        [DllImport("__Internal")]
        public static extern void SVRHaptic(int hand_type, int mode, int duration_ms);

        [DllImport("__Internal")]
        public static extern bool SVRIsConnected(int hand_type);

        [DllImport("__Internal")]
        public static extern long SVRTimeNow();
	
#endif
    }
}
