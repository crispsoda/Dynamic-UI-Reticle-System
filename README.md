# Dynamic-UI-Reticle-System
A shader-based UI reticle system that can be animated at runtime by changing material properties. <br>
Contains data-driven hit reaction animations and a progress bar for charge attacks. <br>
Allows for smooth transition of material properties and reticle state changes.

This system was initially created for my Honours Project, but has now been expanded and refactored.

## Demo
![ReticleSystem_DemoGIF](README_Media/ReticleSystem_DemoGIF.gif) 

## Features
- Enum-driven system to dynamically change reticle modes at runtime
    - Can be changed instantly or with smooth transitions
- Data-driven hit reaction animations
- Charge attack progress bar support
- Custom UI reticle shaders for runtime animation
    - Contains no textures and is completely dynamic
- ScriptableObject driven profiles
    - Tweak settings and properties without editing code
- Automatic profile creator
    - Automatically generate profiles in editor, instead of copying and pasting dozens of properties
- Custom Property Drawer for ScriptableObject profiles
    - Colour-codes and conditionally reveals fields for better workflow

## Overview
The reticle changes based on an enum state. Each reticle state has a ReticleProfile entry in a ScriptableObject that stores settings for the reticle state, as well as its properties.

![ReticleSystem_Profile](README_Media/ReticleSystem_Profile.png) 

To transition between reticle states and apply the profiles, you only have to call
```
//from ReticleController
ChangeReticuleMode(ReticleMode newMode, bool transitionToProfile)
```

The reticle is made up of an inner and outer layer, with a custom shader for each. <br>
These shaders are animated at runtime by changing the property values of their materials.<br>
The profiles contain information on all properties of both materials and settings for animations.

These properties can be set to a dynamic type which will reveal a second end value in the inspector. Dynamic properties are added to a separate list that is passed on to any interested subsystem that will then handle its runtime logic.<br>
Setting a dynamic property type to HitReact for example will add it to a list of properties that will automatically animate from their start to end value when a hit reaction is triggered, without any additional setup.

Instead of copying and pasting every property of a material, you can automatically create a profile in the editor.<br>
The profile creator takes in materials for each layer and then gets all relevant properties and their current values from that material. <br>
You will still have to set whether this mode supports hit react animations or charge bar, as well as their durations.

![ReticleSystem_ProfileCreator](README_Media/ReticleSystem_ProfileCreator.png) 

## Packages
This system uses DOTween (free version).