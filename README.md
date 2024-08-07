# **Overview** 
Experimental application for the detection of increasing velocity gain for walking (acceleration) in virtual reality

# **Requirements** 
Unity Version 2021.3.31f1

Unity store assets to import:
- [Meta XR All-in-One SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-interaction-sdk-264559)
- [Meta XR Interaction SDK OVR Samples](https://assetstore.unity.com/packages/tools/integration/meta-xr-interaction-sdk-ovr-samples-268521)

# **Contents**
## V1 Remappable OVR Camera Rig
This is a modified rig, that places objects in the appropriate locations in the hierarchy to support remapping without breaking other interactions (_as far as I know_).

## Scenes
### _0_EnterPID_
Control panel for entering participant ID. This is the starting scene.
### _1_Alignment_
Provides UI prompts for user to situate themselves in physical space before beginning the walking trial. Must be configured by an experimenter to safely align with the room.
### _2_Trial_
Includes scene within which participant will walk. This same scene is used for the experimenter alignment trial, 2 training trials, and 80 experimental walking trials.
### _3_ThankYou_
Prompts participant to remove headset.

## Logging
### Trial Logging
For each individual trial, stores the following information
- PID: participant identification
- TrainingTrial: 1 = training trial; 0 = standard experimental trial. Useful to filter real data from entire list of data.
- Trial Number: starts counting INCLUDING training trials, but NOT experimenter alignment trial
- Acceleration: per-trial value used to determine rate of change of velocity gain
- GainValueReported: velocity gain value at the time that detection was reported
- TimeWhenReported: time since entering _2_Trial_ scene when detection was reported
- Detection: 0 = no reported detection; 1 = reported detection
- TotalTime: time from entering _2_Trial_ scene to exiting scene
- Forward: 0 = forward alignment in virtual scene; 1 = backward alignment in virtual scene. Possibly useful for determining no confounding impacts of environmental differences.
- MaxGainValue: velocity gain value at the end of the trial
- TimeWhenStartedWalking: time, since entering the scene, at which the participant passes the threshold for velocity gain to begin

### Motion Logging
For every frame while in the _2_Trial_ scene, motion logging will log the following information
- PID, TrainingTrial, TrialNumber: same as for trial logging
- TimeSinceStart: time since entering _2_Trial_ scene
- RealX: x position in physical space
- VirtualX: x position in virtual space
- Y: y position in both physical and virtual space. Note: these will always be the same because velocity gain is not applied to movements along the vertical axis.
- RealZ: z position in physical space
- VirtualZ: z position in virtual space
- HeadForwardX, HeadForwardY, HeadForwardZ: components of the unit vector representing the headset's rotational alignment in space

## Scripts
### _TrialManager_
persistent singleton object that creates itself upon the first static access. Responsible for tracking data for trial randomization and other data useful for logging or scene transitions.

### _VelocityGain_
handles calculations of velocity gain from a constant acceleration as well as translation gains based on current velocity gain. Also sends certain motion-related values to _TrialManager_ for logging purposes.

### _AccelerationLogger_
Creates both trial log files and motion log files, as described above. Also handles storing detection values and ending trials once the proper walking distance has been travelled.

### _PIDEntryUI_, _AlignmentUI_, _ThankYouUI_
Mainly contain button function references for UI components. _PIDEntryUI_ and _ThankYouUI_ align UI panels to the headset. _AlignmentUI_ aligns UI panels based on physically aligning the participant between trials.

### _FadeHandler_, _SceneTransitions_
Handles fading to black between scenes. Integrated mainly with Unity animator. _SceneTransitions_ also handles resetting velocity and translation gain parameters for the next scene.

## Important Developer Notes
### If changing the total trial distance
You MUST change an inspector variable in two different scenes to ensure proper trial distances and alignment. These two values MUST match:
- Within _1_Alignment_, the _ALignmentPanels_ object's _BackwardPos_ property
- Within _2_Trial_, the _Acceleration Logger_ object's _Physical Distance Per Trial_ property
### If changing the distance delay for when gain begins
You MUST change an inspector variable on two different objects in the _2_Trial_ scene to ensure proper congruency between logging and motion behavior. These two values MUST match:
- The _VelocityGain_ script on the V1 Remappable OVR Camera Rig object has a _Distance Delay_ parameter
- The _Acceleration Logger_ object has a _Distance Delay_ parameter within the _AccelerationLogger_ component

## Imported Environment Assets
- [Terrain Textures](https://assetstore.unity.com/packages/2d/textures-materials/free-fantasy-terrain-textures-233640)
- [Terrain Textures](https://assetstore.unity.com/packages/2d/textures-materials/floors/yughues-free-ground-materials-13001)
- [Tree Models](https://assetstore.unity.com/packages/3d/vegetation/trees/free-trees-103208)
- [SFX](https://assetstore.unity.com/packages/audio/sound-fx/free-ui-click-sound-pack-244644)

## Experimenter Script
Thank you for coming. Before we begin I’m going to tell you a bit about what you will be doing.

During each trial you will physically walk across the room. In the virtual world, your motion will always match your physical walking at first, but, over time, will sometimes become faster than your motion in the real world. If you ever detect this change between your physical and virtual walking, please press the back button on either hand controller. Each Trial may experience a different amount of change as you are walking, and some trials will not experience any.

Between trials you will be prompted to turn around and align with the expected starting position for the next trial, as indicated by a circle below you in the virtual world. Once you select the button to start the trial and the path scene loads, you should begin walking in a straight line along the path at a steady, slow pace until the scene fades to black.

Do you have any questions?

…….

Your first couple of trials will be to train you in how to use the application. The first trial will experience no change so you can familiarize yourself with the process. Your next trial will experience a very high amount of change so that you can see what it looks and feels like. This trial will repeat until you successfully report your detection of the change at some point while walking.

After this, you will begin the actual experimental trials, repeating this process until you are prompted to remove the headset.

Are you ready to begin?

……..

Reminders Throughout:
- Make sure that you try to walk at a slow, steady pace through each trial.
- Help with alignment and understanding issues whenever needed
