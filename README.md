# Surreal Touch: VR Gaming Controller for Apple Vision Pro



Surreal Touch, developed by [Surreal Interactive](https://surreal-interactive.com), is a VR gaming controller specifically designed for the Apple Vision Pro. This product aims to bridge the gap between the most immersive hardware and the vibrant VR game ecosystem.

We are dedicated to provide seamless support for developers, enabling them to port their exceptional VR games to this new platform effortlessly. With Surreal Touch, you can bring the most engaging and interactive VR experiences to life on the Apple Vision Pro.

# Get Started
In this tutorial, we will introduce how to create a visionOS application powered by Surreal Touch.

You can choose how to use it based on your situation.

1. You can use Surreal VR SDK based on a brand new project, please follow Step-by-Step Instruction below.

2. You can port your developed applications based on other SDKs(for example Oculus) to Surreal Touch visionOS in a simple way, a neat [Unity Starter Sample](https://github.com/surreal-interactive/Unity-StarterSamples) which is pretty similar to Oculus [Unity-StaterSamples](https://github.com/oculus-samples/Unity-StarterSamples/blob/d1df2ece3ed7fcc572ac645cbe240beabd611547/Assets/StarterSamples/Usage/DistanceGrab.unity) is provided, there's a full-functionality scene where players can interact with and throw various game objects, showcasing the system's capabilities.

You can try to build and run this [example](https://github.com/surreal-interactive/Unity-StarterSamples/tree/master/Assets/Scenes) first before you dive into Surreal VR SDK.

<p align="center">
   <img src="https://github.com/surreal-interactive/SDK/assets/73978606/cf3eedd2-b7db-4da8-9d3a-df89aa83a3aa" alt="description" width="80%">
</p>

__Note__

__Versions of Unity prior to 2022.3 are not supported.__

# Step-by-Step Instruction
Surreal VR SDK (SVR) is designed to offer a plug-and-play experience for developers.

To ensure a smooth experience for developers who are familiar with Oculus SDK, an overview of the one-to-one mapping for controller-related operations is shown below:


## One-to-one mapping

| | Oculus VR SDK | Surreal VR SDK |
|--|--|--|
| Unity Package | [Oculus Unity Documentation](https://developer.oculus.com/documentation/unity/unity-ovrinput/) | [Surreal VR SDK GitHub](https://github.com/surreal-interactive/SDK) |
| Camera Rig Prefab | `OVRCameraRig` | `SVRCameraRig` |
| Button Down | `OVRInput.GetDown(OVRInput.RawButton.A)` | `SVRInput.GetDown(SVRInput.Button.A)` |
| Button Up | `OVRInput.GetUp(OVRInput.RawButton.X)` | `SVRInput.GetUp(SVRInput.RawButton.X)` |
| Thumbstick State | `OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)` | `SVRInput.Get(SVRInput.Axis2D.LThumbstick)` |
| Thumbstick Pressed | `OVRInput.Get(OVRInput.Button.PrimaryThumbstick)` | `SVRInput.Get(SVRInput.Button.LThumbstick)` |
| Thumbstick Up | `OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)` | `SVRInput.Get(SVRInput.Button.LThumbstickUp)` |
| Index Trigger State | `OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)` | `SVRInput.Get(SVRInput.Axis1D.RIndexTrigger)` |
| Left Index Trigger Pressed | `OVRInput.Get(OVRInput.RawButton.LIndexTrigger)` | `SVRInput.GetDown(SVRInput.RawButton.LIndexTrigger)` |



## Step-by-Step Instruction

Then, step-by-step operations are as follows:
1. Install visionOS required packages which including:

   - `"com.unity.polyspatial"`
   
   - `"com.unity.polyspatial.visionos"`
   
   - `"com.unity.polyspatial.xr"`

   It's recommended to follow [Install a UPM package from a Git URL](https://docs.unity3d.com/Manual/upm-ui.html).

   __Note__

   __Project is tested under PolySpatial version 1.1.4.__

<p align="center">
   <img src="https://github.com/surreal-interactive/SDK/assets/73978606/c0146b4b-795c-443d-b24f-67bcb3174939" alt="description" width="80%">
</p>

2. Install Surreal Touch Unity package

   Install package `"https://github.com/surreal-interactive/SDK.git"`.

   It's recommended to follow [Install a package from a Git URL by manifest](https://docs.unity3d.com/Manual/upm-manifestPrj.html).

<p align="center">
   <img src="https://github.com/user-attachments/assets/722d5aee-017b-4c50-9385-f4d072339b5f" alt="description" width="80%">
</p>

3. Check `visionOS` in `Project Settings`

<p align="center">
   <img src="https://github.com/user-attachments/assets/e481f855-a2d6-401a-9e40-46eae04dcc76" alt="description" width="80%">
</p>

4. Put `SVRCameraRig` into your scene

   `SVRCameraRig` accurately mirrors the real-world poses of the controllers.

<p align="center">
   <img src="https://github.com/surreal-interactive/SDK/assets/73978606/ae2faf44-33b4-4884-b498-c5c8f7563204" alt="description" width="80%">
</p>

5. Build Unity project

   Everything is setup in Unity, switch your target platform to visionOS, a xcode project will be created after building is finished.

<p align="center">
   <img src="https://github.com/user-attachments/assets/ee35418d-96d9-4add-b79d-314fd72e46a7" alt="description" width="80%">
</p>

6. Request bluetooth permission

   Open the xcode project, request bluetooth permission by adding `"Privacy - Bluetooth Always Usage Description"` in plist document.

<p align="center">
   <img src="https://github.com/user-attachments/assets/40d00cf7-4fac-473b-b5ac-e6c89d8002c4" alt="description" width="80%">
</p>

7. All Done!

   Everything is done, now you can build your xcode project and don't forget to connect Surreal Touch controllers to Apple Vision Pro in `Settings/Bluetooth` in your device!

A complete sample project is provided in [`Unity-StarterSamples`](https://github.com/surreal-interactive/Unity-StarterSamples).

Video here:

[Distance grab.webm](https://github.com/surreal-interactive/SDK/assets/73978606/a706a050-3729-40ec-af5e-baa2421e1634)
