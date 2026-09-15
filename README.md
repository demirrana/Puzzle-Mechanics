This game has 4 puzzle mechanics that the player has to solve. Each puzzle is based on the same main logic.

## Main Logic: InteractionManager

Each step that has been taken by the player, has its corresponding event. For the interaction process, these steps are generated as **events** under these headings:  
  
  *OnObjectCollidersApproached -> Being close to any object  
  *OnInteractableApproached -> Detecting that close object being an interactable object  
  *OnInteractionConditionsMet -> Detecting if conditions are met for interacting with near interactables  
  *OnInteractionKeyPressed -> Detecting any interaction is performed by checking the required key being pressed  
  *OnInteractableInteracted -> Interacting with an interactable object  
  *OnNoInteractableNear -> Having no interactable objects near the player  
  *OnInteractableInHandChanged -> Change in the hand-held interactable object
  
Each interactable object has their behaviours. After an interaction is performed on an object, the next possible interaction behaviours' list is held for each interactable in *Interactable.cs* file.

**Interaction process' main logic flow** is constructed as the below diagrams:  
<img width="1257" height="771" alt="game main flow" src="https://github.com/user-attachments/assets/14cc50c9-5484-4382-9d73-56c27470e61a" />
<img width="1248" height="471" alt="image" src="https://github.com/user-attachments/assets/4b818ad5-dfe7-4a19-89c6-f402adc0c7c0" />

This flow is provided within *InteractionManager.cs* file. Manager scripts of each puzzle (InteractionManager0thPuzzle.cs, InteractionManager1stPuzzle.cs, InteractionManager5thPuzzle.cs, and InteractionManager7thPuzzle.cs) extend from that class to elaborate their distinct mechanics.  

## Puzzles' Distinct Logics ##

### Puzzle 0 ###
There are representative books in shelves. Each book represents a few feelings in some proportions. For instance, book 1 may contain 20% of feeling A, 45% of feeling B, and 35% of feeling D. There is only one book that is 100% representative of each feeling. The final objective in this puzzle is to put the books that are representing each feeling **100%** on the *book platform*.  

### Puzzle 1 ###
There are objects that can be switched as pairs. Each object has its correct locations which are mostly different at the start of the puzzle. The main objective is to switch objects to reach their rightful locations by swapping them.  

### Puzzle 5 ###
There are 5 spots on a table where collected interactable objects can be placed onto. Each object can be placed on any of the spots on the table. The ultimate goal is to place the correct object onto the right spots.

### Puzzle 7 ###
There is a main key part on player, which can be obtained by **pressing B**. There are also little parts that can be attached to the main key's designated spots. The main objective in this puzzle is to create a key that suits the door keyframe correctly.
