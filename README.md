# UnityXRProvider
This is the Unity Plugin for DeepMirror Controller.

## Import Plugin into Unity

Add plugin from git url in UnityPlugin Manager with
```
https://github.com/deepmirrorinc/UnityXRProvider.git?path=/Assets
```
## Dependency
This Plugin depends on:
- com.unity.xr.management
- com.unity.xr.interaction.toolkit
- com.unity.xr.hands

Since only support for VisionPro, your project also need to **Unity PolySpatial** Plugin to export VisionOS Xcode Project

## Set up controllers
- In the **Hierarchy** window, expand **XR Oringin > Camera Offset**
- Select **LeftHand Controller**.
- In the **Inspector** window, click the Preset button in the upper-right corner of the **XR Controller (Action-Based)**

![HandController](IMG/Controller.png)

- Double-click **XRI Default Left Controller** to add default settings to the left-hand controller.

![Default Selection](IMG/SelectPreset.png)

- Configure the **RightHand Controller** through the same steps as above

## Enable Action Assets
As the latest Unity input system is applied, you need to add the **Input Action Manager** script for input control. Below are the steps to follow:
- In the Hierarchy window, select **XR Origin**
- Click **Add Component** at the bottom of the **Inspector** window.
- Search for the **InputAction Manager** script and double-click to add it
- In the **Input Action Manager** area, expand the **Action Assets** list, then click **+** to add an element(such as **Element 0**)
- Click the **Circle** icon

![InputActionManager](IMG/InputActionManager.png)

- Double-click **XRI Default Input Actions** to add it to **Element 0**.

![InputActionAssets](IMG/InputActionAssets.png)


## Enable DM Controller System
- In the Hierarchy window, **Right-Click > Create Empty**, create a empty game object and rename it as **DMVisionOSManager**
- In Project Windows, add **Package > DM InputSystem Provider > Editor > DMManager.cs** to the **DMVisionOSManager** game object created as above.

![DMVisionOSManager](IMG/DMVisionOSManager.png)

## Enable AR session
The **AR Session** Script must be added to enable the **ARKit** 
- Click the game object **DMVisionOSManager**, in the inspector window, click the **Add Component** button and search the **AR Session**,then add it.

![AR Session](IMG/ARSession.png)

## Xcode plist
you may add **Privacy - Bluetooth Always Usage Description** and **NSHandsTrackingUsageDescription** in your **Info.plist** file of xcode project exported by unity to grant the permission.
![Info_plist](IMG/Info_plist.png)

