Unity Custom Tools

This repository adds a new top-menu category in the Unity Editor called Custom Tools, containing several productivity utilities designed to speed up everyday development work.

Included Tools
1. Batch Rename Tool

Select multiple objects and rename them using custom naming rules.
Useful for quickly organizing scene hierarchies and large collections of objects.

2. Custom Hierarchy Toggle

Adds extra buttons to objects in the Hierarchy window, allowing you to:

Display additional info about the object

Focus the camera on it

Delete the object

Create a prefab directly from the selected object

A simple set of quality-of-life features to make scene navigation faster.

3. Missing References Detector

Scans all objects in the currently open scene and detects fields with missing or null references.
Helps quickly locate broken links that can cause errors during gameplay.

4. Project Asset Organizer
   
Automatically organizes Unity project assets into folders based on file extensions.

Allows you to:

Define custom asset types with multiple extensions

Assign target folders per asset type

Persist configuration between editor sessions

Batch-move assets safely using Unity’s AssetDatabase

5. Editor Autosaver

Automatically saves the active Unity scene at configurable time intervals.

Allows you to:

Enable or disable autosave via a menu toggle
Choose save intervals from a simple settings window
Automatically save scenes only when changes are detected
Skip unsaved scenes to avoid disruptive dialogs
Persist autosave settings between editor sessions

Requirements:

Unity 2022+ or Unity 6

Editor scripting enabled

Installation:

Clone or download the repository and place the Editor folder inside your Unity project.

License

MIT License
