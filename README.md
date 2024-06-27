# Surreal Touch: VR Gaming Controller for Apple Vision Pro



Surreal Touch, developed by [Surreal Interactive](https://surreal-interactive.com), is a VR gaming controller specifically designed for the Apple Vision Pro. This product aims to bridge the gap between the most immersive hardware and the vibrant VR game ecosystem.

We are dedicated to providing seamless support for developers, enabling them to port their exceptional VR games to this new platform effortlessly. With Surreal Touch, you can bring the most engaging and interactive VR experiences to life on the Apple Vision Pro.

# Get Started
In this tutorial, we will introduce how to port an Oculus project to a VisionOS application powered by Surreal Touch.

The [Oculus Starter Samples](https://github.com/oculus-samples/Unity-StarterSamples) provide a full-functionality scene where players can interact with and throw various game objects, showcasing the system's capabilities.

<img src="https://github.com/surreal-interactive/SDK/assets/73978606/a1173c4a-e16f-4e5b-a56c-181bac135df4" alt="description" width="30%">


# Step-by-Step Adaptation
To ensure a smooth development experience, we provide the Surreal VR SDK (SVR), designed to offer Oculus developers a plug-and-play experience.

To achieve this, we provide an overview of the one-to-one mapping for controller-related operations:


## one-to-one mapping

To ensure a smooth development experience, we provide the Surreal VR SDK (SVR), designed to offer Oculus developers a plug-and-play experience.

To achieve this, we provide an overview of the one-to-one mapping for controller-related operations:

| | Oculus VR SDK | Surreal VR SDK |
|--|--|--|
| Unity Package | [Oculus Unity Documentation](https://developer.oculus.com/documentation/unity/unity-ovrinput/) | [Surreal VR SDK GitHub](https://github.com/surreal-vr-sdk) |
| Camera Rig Prefab | OVRCamRig | SVRCamRig |
| Button Down | `OVRInput.GetDown(OVRInput.RawButton.A)` | `SVRInput.GetDown(SVRInput.Button.A)` |
| Button Up | `OVRInput.GetUp(OVRInput.RawButton.X)` | `SVRInput.GetUp(SVRInput.RawButton.X)` |
| Thumbstick State | `OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)` | `SVRInput.Get(SVRInput.Axis2D.LThumbstick)` |
| Thumbstick Pressed | `OVRInput.Get(OVRInput.Button.PrimaryThumbstick)` | `SVRInput.Get(SVRInput.Button.LThumbstick)` |
| Thumbstick Up | `OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)` | `SVRInput.Get(SVRInput.Button.LThumbstickUp)` |
| Index Trigger State | `OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)` | `SVRInput.Get(SVRInput.Axis1D.RIndexTrigger)` |
| Left Index Trigger Pressed | `OVRInput.Get(OVRInput.RawButton.LIndexTrigger)` | `SVRInput.GetDown(SVRInput.RawButton.LIndexTrigger)` |

## Simple Adaption

Then, step-by-step operations are as follows:
1. Install VisionPro depended packages including:

   i.   "com.unity.polyspatial"
   
   ii.  "com.unity.polyspatial.visionos"
   
   iii. "com.unity.polyspatial.xr"

   It's recommended to follow [Install a UPM package from a Git URL](https://docs.unity3d.com/Manual/upm-ui.html)

   <img src="https://github.com/surreal-interactive/SDK/assets/73978606/81ac325f-0088-40b3-af75-477b2dd30920" alt="description" width="30%">


2. Install Surreal Touch Unity package
Install package "https://github.com/surreal-interactive/SDK.git"

3. Replace OVRCamRig with SVRCamRig, to create a game object that accurately mirrors the real-world poses of the controllers.

   <img src="https://github.com/surreal-interactive/SDK/assets/73978606/f271bef6-b7f7-4ba2-92f4-20bef6fd9e6d" alt="description" width="30%">

4. Implement grab operations:

   SVRDistanceGrabbable.cs: scripts for gameobjects ready for grabbing.

   <img src="https://github.com/surreal-interactive/SDK/assets/73978606/73218926-defe-4570-ab97-f70245d91efd" alt="description" width="30%">

All Done!

Grabbable objects are cyan highlighted if they are within grabbing range, and objects pointed by controllers are yellow highlighted

<img src="https://github.com/surreal-interactive/SDK/assets/73978606/d8d6dfb4-4869-46d0-8023-8f36bdc86096" alt="description" width="30%">

Video here:


https://github.com/surreal-interactive/SDK/assets/73978606/41277564-a7dd-42bd-88d6-404513caec10

