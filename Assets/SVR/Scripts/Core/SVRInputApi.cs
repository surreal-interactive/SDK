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

        public static class Chirality
        {
            public static readonly int Left = 0;
            public static readonly int Right = 1;
        }

#if UNITY_IOS || UNITY_VISIONOS
        [DllImport("__Internal")]
        public static extern void SVRStart();

        [DllImport("__Internal")]
        public static extern void SVRStop();

        [DllImport("__Internal")]
        public static extern bool SVRPollDevicePose(int hand_type, long poll_timestamp, ref SVRPose pose, ref SVRVector3f linear_velocity, ref SVRVector3f angular_velocity, IntPtr hand_skeletons);

        [DllImport("__Internal")]
        public static extern void SVRSetButtonCallback(IntPtr callback_ptr);
        
        // amplitude : [0, 1]
        // frequency : (20, 300]
        [DllImport("__Internal")]
        public static extern bool SVRHapticContinuous(int hand_type, float amplitude, float frequency, double duration_seconds);

        [DllImport("__Internal")]
        public static extern bool SVRIsConnected(int hand_type);

        [DllImport("__Internal")]
        public static extern long SVRTimeNow();

        [DllImport("__Internal")]
        public static extern void SVRRenderFinish(int hand_type, long poll_timestamp, long render_finish_timestamp);

#endif
    }
}
