This game has 4 puzzle mechanics that the player has to solve. Each puzzle is based on the same main logic.

## Interaction Flow

### InteractionManager
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

### 2. Interactable
Each type of interactable object has its corresponding puzzle type's interactable script (such as puzzle 1 object having Interactable1stPuzzle.cs). These objects have their next possible behaviours in the name of behavioursList so that which behaviour can be applied after the one performed. Therefore, trying to apply an impossible behaviour is prevented beforehand.   
  
Each interactable object has their *GetInteracted_Interactable* method which calls the behaviour that is about to be applied to that object. This method is based on the specific puzzle, however they are mainly same and updating behaviours, states. 

For specific puzzles, there are special interactable objects that have their own interactable scripts attached to them.

### 3. InteractableBehaviour
Each puzzle has their own behaviour types based on that puzzle mechanics. However, each behaviour mainly has its own key to press for performing it, and has a method called *Interact* to manage those interactables' current states.  
  
Generally, in the *Interact* method, the interacted object's location is updated to where it is supposed to be after that behaviour, updates its parent if necessary, updates the player's hand being occupied with that object or not, and updates the next possible behaviours that could be applied with that object's new state.


## Puzzles' Distinct Logics ##

### Puzzle 0 ###
There are representative books in shelves. Each book represents a few feelings in some proportions. For instance, book 1 may contain 20% of feeling A, 45% of feeling B, and 35% of feeling D. There is only one book that is 100% representative of each feeling. The final objective in this puzzle is to put the books that are representing each feeling **100%** on the *book platform*.  

**Puzzle0Manager.cs:** Mainly bound to the superclass of it, manages the process of detecting near objects, their types, and what to do at that exact moment.  
Main Update loop can be deducted as following:  
&emsp;*Detecting approached colliders if any exists  
&emsp;*Detecting any interaction conditions being met  
&emsp;&emsp;-Detecting interactions in *world view* (moving character around)  
&emsp;&emsp;&emsp;*When there is an **object in hand**, check being near either bookshelf or book platform to put it on  
&emsp;&emsp;&emsp;*When **hands are not occupied**, check being near either a bookshelf or book platform to take a book from it  
&emsp;&emsp;-Detecting interactions in *book platform view* (adjusting books on platform)  
&emsp;&emsp;&emsp;*When exit key is pressed, switch back to world view  
&emsp;&emsp;&emsp;*If not, detect player clicking on a book to take it  

Interactions are performed via *IInteractableBehaviour0thPuzzle* extending classes. There are **4** types of behaviours in this case: Picking up a book from its shelf, putting a book back on its shelf, putting a book on the book platform, taking a book back from the platform. Each behaviour gets called from manager class, and adjusts the books' parents and locations based on the specific situation. As addition to these, updates the platform's book list, and manages player's hand being occupied by that book or not. Following that behaviour, it updates the possible next behaviours (behavioursList for each Interactable).

### Puzzle 1 ###
There are objects that can be switched as pairs. Each object has its correct locations which are mostly different at the start of the puzzle. The main objective is to switch objects to reach their rightful locations by swapping them.  

### Puzzle 5 ###
There are 5 spots on a table where collected interactable objects can be placed onto. Each object can be placed on any of the spots on the table. The ultimate goal is to place the correct object onto the right spots.

### Puzzle 7 ###
There is a main key part on player, which can be obtained by **pressing B**. There are also little parts that can be attached to the main key's designated spots. The main objective in this puzzle is to create a key that suits the door keyframe correctly.
