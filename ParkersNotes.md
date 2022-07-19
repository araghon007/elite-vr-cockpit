### 1 
`Please remove the CanvasRenderer component from the \[Text\] GameObject as this component is no longer necessary.`
`UnityEngine.Debug:LogWarning (object,UnityEngine.Object)`
`TMPro.TextMeshPro:Awake () (at Library/PackageCache/com.unity.textmeshpro@3.0.6/Scripts/Runtime/TMPro_Private.cs:121)`

- EVRC > ToolTipTextGeneration > Text
    - removed Canvas component (looked empty) between Mesh Renderer and TextMeshPro - Text


### 2 
`Lighting data asset ‘LightingData’ is incompatible with the current Unity version. Please use Generate Lighting to rebuild the lighting data. Realtime Global Illumination cannot be used until the lighting data is rebuilt.`

- 


### 3
`Assets\SteamVR\Editor\SteamVR_AutoEnableVR.cs(64,68): error CS0117: 'VREditor' does not contain a definition for 'GetVREnabledDevicesOnTargetGroup'`


From Unity: This API is obsolete, and should no longer be used. Please use XRManagerSettings in the XR Management package instead.
- installed XR plugin management

OpenXR could not be installed from XR Management section, script errors were preventing this
- changed name to SteamVR_AutoEnableVR.backup
- when re-compiling, unity restarted the console automatically

### 4
When enabling OpenXR: `At least one interation profile must be added. Please select which controllers you will be testing against in the features menu.`

XR Plug-in Management > Open XR > Interaction Profiles > (+) 
- selected Oculus Touch Controller Profile


### 5 SO MANY DLL EXCEPTIONS
`DllNotFoundException: openvr_api assembly:<unknown assembly> type:<unknown type> member:(null)`
`Valve.VR.OpenVR.GetInitToken () (at Assets/SteamVR/Plugins/openvr_api.cs:6555)`
`Valve.VR.OpenVR+COpenVRContext.CheckClear () (at Assets/SteamVR/Plugins/openvr_api.cs:6854)`
`Valve.VR.OpenVR+COpenVRContext.VROverlay () (at Assets/SteamVR/Plugins/openvr_api.cs:6915)`
`Valve.VR.OpenVR.get_Overlay () (at Assets/SteamVR/Plugins/openvr_api.cs:7102)`
`EVRC.HolographicImage.Update () (at Assets/Scripts/HolographicImage.cs:43)`

- deleted SteamVR folder
- got most recent SDK from unity.com (2.7.3) and imported into project

### 6 ResetSeatedZeroPose
`Assets\Scripts\SeatedPositionResetAction.cs(36,27): error CS1061: 'CVRSystem' does not contain a definition for 'ResetSeatedZeroPose' and no accessible extension method 'ResetSeatedZeroPose' accepting a first argument of type 'CVRSystem' could be found (are you missing a using directive or an assembly reference?)`

- 'ResetZeroPose' was present in default SteamVR code, but not 'ResetSeatedZeroPose', so I added it wherever I saw the original
- commented out line 36: 'OpenVR.System.ResetSeatedZeroPose();'
    - resetting zero pose now does nothing (didn't work on Oculus anyways)

### 7 InitializeStandalone
`Assets\Scripts\OverlayController.cs(78,21): error CS0117: 'SteamVR' does not contain a definition for 'InitializeStandalone'`

- copied commit diffs from dantman - custom SteamVR code to remove Unity XR requirements
- https://github.com/dantman/steamvr_unity_plugin/commit/d3cd13b3c97f95d0a76dcf15d3c5daeeabe26012 


## Overlay Mode
Steam Settings > Auto Enable VR  = False

Project Settings > XR Plug-in Management > Initialize XR on Startup = unchecked

### 8  InvalidHandle Handle: 1153021766323405304. Input source: LeftHand UnityEngine.Debug:LogError (object)
Open Bindings UI
Change Nothing
Save Personal Binding
NOTE: Will say \[Testing\] in the name of the binding

Also maybe: Event System > Input System UI Input Module > Set XR Tracking Origin as `SeatedOrigin`

# betterThrottle
- change highlight color to not be trasparent


1. different color for reverse
2. haptic feedback when changing directions
3. deadzone color
----Done----
4. controls for haptics, deadzone in the holographic menu
5. haptic deadzone release


# Ideas/Todo
1. add POV bindings section to throttle side
2. Allow rotation on throttle side, like the joystick
3. User setup - presets (HOTAS, Dual Stick, etc)
4. micro-adjustment of view 



