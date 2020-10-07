# Genetic Neural Network
Unity project about genetically selected neural networks

## GIFs because why not
![NN1](https://raw.githubusercontent.com/JonasBeduschi/Genetic-Neural-Network/master/Images/NN1.gif)
First Generation

![NN2](https://raw.githubusercontent.com/JonasBeduschi/Genetic-Neural-Network/master/Images/NN2.gif)
5 minutes later

![NN3](https://raw.githubusercontent.com/JonasBeduschi/Genetic-Neural-Network/master/Images/NN3.gif)
10 minutes later

![NN4](https://raw.githubusercontent.com/JonasBeduschi/Genetic-Neural-Network/master/Images/NN4.gif)
30 minutes later

## What is this project
Made on free time, it is an experimentation on Neural Networks and Genetic Algorithms. Long story short: The little guys "evolve" to navigate the map

## How to use it?
Download Unity and clone/download the repo and open the project! You'll see a single scene that can be used as is or changed.
It contains the following:
* SCRIPTS: General information and options to control the Population.
  * On the context menu, an option "Build" can be found, to create the desired scenarion, as selected on Map Number
  * Pause - If you want to pause the simulation at the end of every generation, to check on progress or individual creatures
  * Load save file - Loads a specific save file. Creatures will be copied based on the NN saved on the file of this index (-1 cancels this option)
  * Save on better fitness - If a creature achieves a better fitness than the one saved on this save file `SaveFiles[MapNumber]`, their NN will be saved
  * Input, hidden and output nodes - Shouldn't be changed, will be hidden by a new `PopulationEditor` soon
  * Creature mutation rate - Chance of each creature to be mutated, when creating a new generation (ignored on the first generation)
  * Connection mutation rate - Chance of each node to be mutated, if the creature was selected as per above
  * Mutation amount - How much the value of the node will be mutated
  * Procreation percentage - How many of a new generation should be created from clonning, instead of randomly
  * Randomize starting height - If the starting height of the creatures should be random
  * Starting height - Used if the above condition is false
  * Number of creatures - Used both to create creatures and maps
  * Creature original - Acts as a prefab, but should be left on the scene and will be deactivated on play
  * Map number - Defines both the map to be built by the context menu and the save file to be used when saving
* Creature Original: Acts as a prefab and can be changed to fit your needs. Do not delete.
* Maze Parent: Parent object for the maps/obstacles/mazes to keep things organized
* Creatures Parent: Parent object for the creatures to keep things organized  

## Goals
I will be updating this for easy of usage and clarity when possible. Making it easily costumizable and simple to use are the goals, so it can be used in a variety of different games and experiments.
