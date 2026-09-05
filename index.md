# OSK.Petra.Inputs

Provides a framework for creating, managing, processing, handling, editing, etc. an input system for an application. It does NOT implement the input system itself,
but rather the processing, handling, etc. of the inputs, the data and state, actions, and so much more that are associated with the input system. This system is meant to be
engine agnostic so that it can be reused across various C# style game engines.

### Abstractions
Various input related data structures and base code access without the input system implementation

### Inputs
The core service and input proccessing layer

### Configuration Extensions
Provides access to configuration builders and setup to create input configurations to work with an input system

### Devices
Implementation of various input devices and input types to work with an input system

### Supported Engines
* There is a [Godot implementation](https://github.com/OpenSourceKingdom/OSK.Petra.Godot.Inputs) for this input system that can be used within that game engine.
* Unity3D support is being considered for a future implementation


For more information on the project architecture, please see related information on the design in the [Hub](https://opensourcekingdom.github.io/OSK.Hub/)