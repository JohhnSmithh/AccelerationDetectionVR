# **Overview** 
Remapping stuff for Meta Quest Pro.

# **Requirements** 
Unity Version 2021.3.31f1

Unity store assets to import:
- [Meta XR All-in-One SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-interaction-sdk-264559)
- [Meta XR Interaction SDK OVR Samples](https://assetstore.unity.com/packages/tools/integration/meta-xr-interaction-sdk-ovr-samples-268521)

# **Contents**
## Scenes
A single scene, *DemoScene*, contains a modified OVRCameraRig that can support remapping (*V1 Remappable OVR Camera Rig*).
The scene contains a control panel for all currently implemented remapping techniques, with options to automatically set them to various detection thresholds from prior work.
There are also some objects that can be picked up with the controller by pressing the back trigger button while touching the object.

## V1 Remappable OVR Camera Rig
This is a modified rig, that places objects in the appropriate locations in the hierarchy to support remapping without breaking other interactions (_as far as I know_).

Right now, only the controllers are supported for remapping (see *Remapped Left OVRControllerVisual* for an example).
As far as I know, any head-based remapping can be implemented by manipulating the *V1 Remappable OVR Camera Rig* object itself (see scripts on the GameObect).

## Supported Techniques
Four techniques are currently supported:
- [Rotation Gain](https://ieeexplore.ieee.org/abstract/document/5072212)
- [Translation Gain](https://ieeexplore.ieee.org/abstract/document/5072212)
- [Strafing Gain](https://ieeexplore.ieee.org/document/9994980)
- [Scaled Hand Movement](https://ieeexplore.ieee.org/document/9089480)
