# Evolving Motion Machines
Creating evolving agents that modify both parameters and structure between generations.

## About
This system uses an evolutionary approach to create agents that learn to move. Agents start as a simple, non-moving cube. 
They then mutate additional shapes that can themselves move and create additional shapes, creating simple motion machines. 
Over several generations, complex agents with complex movements form.

For a detailed write-up on the system, see the pdf in the 'paper' branch.

## Setup
Setup is fairly simple. 
First you'll need Unity 2018.3 to run the system.
From Unity, select the option to open an existing project, and load the root directory of this repo. This will get the project set up for editing and set up visual studio for editing the source code properly.

## Usage
Running the system requires some tweaking of settings within Unity's development environment. There are two main modes that the system can run in, one that controls the evolution system and another that can recreate agents from a previous execution of the evolution system.
* **Evolution System**
  
  This will control the selection, evaluation, and mutation in a loop. Getting this running will require the following steps.

   * Within the Unity Edtor, select the 'Plane' object in the object hierarchy.
   * In the inspector menu, ensure that the Evolution Manager script is enabled.
   * With the Evolution Manager enabled, run the program to get the software running.
   * Finally, press 'tab' on the keyboard to begin the main loop of the system.
   * Upon closing the program, a '.genlog' file will be written, which is necessary for the Replay system.

* **Replay System**
  
  This system can show succesful agents from a previous execution of the evolution system. The only agents shown are those that did better than all of their ancestor generations. The list of succesful agents can be iterated over by pressing the up and down keys on the keyboard.

   * Again, within the Unity Editor, select the 'Plane' object in the object hierarchy.
   * In the inspector menu, ensure that the Bests Viewer script is enabled.
   * With the Bests Viewer script enabled, run the program to get the software running.
   * Pressing up on the keyboard will show the first effective agent generated.
   * From there, effective agents can be iterated over by pressing up and down on the keyboard.
   * As a note: the exact same agent is instantiated in the same position four times in this mode. This can be used to see the effect of variations in the physics engine.
   More information can be found in the formal write up in the 'paper' branch of this repo.