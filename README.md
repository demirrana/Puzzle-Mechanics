# Project: 4 Puzzle Mechanics
This project has 4 puzzle mechanics that the player has to solve. Each puzzle is based on the same main logic.

## How Do Mechanics Work?
### Puzzle 0: Placing Books
https://github.com/user-attachments/assets/2d955700-e4c1-4b23-9c02-ea72c26244d5

### Puzzle 1: Snapping Key Parts
https://github.com/user-attachments/assets/e7f25bfc-24d0-4844-b02a-4bf0236ceed3

### Puzzle 5: Placing Objects on Table Slots
https://github.com/user-attachments/assets/ded1db09-a49e-4873-911a-2fa25704fca3

### Puzzle 7: Swapping Objects
https://github.com/user-attachments/assets/b39e0098-7476-4d3c-8326-f1729520871c

## Technical Requirements and Dependencies
### Unity Version
**Unity Engine:** Unity 6.5 (6000.5.3f1)

### Packages & Libraries
Before running the project, ensure the following dependencies are installed via the Unity Package Manager or integrated into the project:  

- **Core & Gameplay Systems:**  
    - **Cinemachine:** 3.1.7  
- **UI & Rendering:**  
    - **TextMeshPro**
 
### Getting Started
- Clone the project: `git clone https://github.com/demirrana/Puzzle-Mechanics.git`
- Open project with Unity version 6.5 (6000.5.3f1) on Unity Hub.
- Navigate to Assets/Scenes/3DScene.unity and press Play.
- Activate/deactivate scripts on Player to apply that specific puzzle mechanic (each puzzle requirement is stated below, in their parts with the banner "**Note**").

## Interaction Flow

### 1. InteractionManager
Each step that has been taken by the player, has its corresponding event. For the interaction process, these steps are generated as **events** under these headings:  
  
- OnObjectCollidersApproached -> Being close to any object  
- OnInteractableApproached -> Detecting that close object being an interactable object  
- OnInteractionConditionsMet -> Detecting if conditions are met for interacting with near interactables  
- OnInteractionKeyPressed -> Detecting any interaction is performed by checking the required key being pressed  
- OnInteractableInteracted -> Interacting with an interactable object  
- OnNoInteractableNear -> Having no interactable objects near the player  
- OnInteractableInHandChanged -> Change in the hand-held interactable object
  
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
**Note:** To execute this mechanic on **Unity**, *all interaction manager classes* on **Player** should be *deactivated* **except** for **InteractionManager0thPuzzle**. **PlayerUIManager** should stay *activated*. As **UI_Puzzle1Manager and SnapHandler** is related to puzzle 1, they should be also *deactivated*.  
  
There are representative books in shelves. Each book represents a few feelings in some proportions. For instance, book 1 may contain 20% of feeling A, 45% of feeling B, and 35% of feeling D. There is only one book that is 100% representative of each feeling. The final objective in this puzzle is to put the books that are representing each feeling **100%** on the *book platform*.  

**InteractionManager0thPuzzle.cs:** Mainly bound to the superclass of it, manages the process of detecting near objects, their types, and what to do at that exact moment.  
Main Update loop can be deducted as following:  
- Detecting approached colliders if any exists  
- Detecting any interaction conditions being met
    - Detecting interactions in *world view* (moving character around)  
       - When there is an **object in hand**, check being near either bookshelf or book platform to put it on  
       - When **hands are not occupied**, check being near either a bookshelf or book platform to take a book from it  
    - Detecting interactions in *book platform view* (adjusting books on platform)  
       - When exit key is pressed, switch back to world view  
       - If not, detect player clicking on a book to take it  

**Behaviours:**  
Interactions are performed via *IInteractableBehaviour0thPuzzle* extending classes. There are **4** types of behaviours in this case: 
- **InteractableBehaviourPickUpFromShelf:** Picking up a book from its shelf
- **InteractableBehaviourPutOnShelf:** Putting a book back on its shelf
- **InteractableBehaviourPutOnBookPlatform:** Putting a book on the book platform
- **InteractableBehaviourPickUpFromBookPlatform:** Taking a book back from the platform.
  
Each behaviour gets called from manager class, and adjusts the books' parents and locations based on the specific situation. As addition to these, updates the platform's book list, and manages player's hand being occupied by that book or not. Following that behaviour, it updates the possible next behaviours (behavioursList for each Interactable).  

**Scriptable Objects:** There are 2 types of scriptable objects: feeling that is represented (**SOFeeling**) and book data (**SOBookData**). Since each book can represent one or more feelings to some extent for each feeling, each book has its own list of feelings. For the very same reason, book data has a *serializable* struct named *FeelingAffinity*. This structure contains feeling data and the book's representation degree of that feeling.

### Puzzle 1 ###
**Note:** To execute this mechanic on **Unity**, *all interaction manager classes* on **Player** should be *deactivated* **except** for **InteractionManager1stPuzzle, UI_Puzzle1Manager, and SnapHandler**. **PlayerUIManager** should stay *activated*
  
There is a main key part on player, which can be obtained by **pressing B**. There are also little parts that can be attached to the main key's designated spots. The main objective in this puzzle is to create a key that suits the door lock's key hole shape correctly. 

**InteractionManager1stPuzzle:** This script manages obtaining key parts and taking the main key part in hand, in general.  
Main Update loop can be deducted as following:

 - Detection of obtaining the **main key part** by pressing the required key
 - Detecting interaction conditions being met or not
   - In *world view*, detect near colliders and trigger events if they are interactables
   - In *editing key view*, either exit to world view, or move key parts on screen based on player's mouse inputs
 - Detecting the completion of finding the right key combination for that door

Key parts are collected by pressing **E** into an inventory which can be activated/deactivated via the key **I** during world view.  
After key parts are collected and are being used, snapping them on the main key is only possible when in edit view which can be opened by pressing **2** on keypad while being near the target door for that puzzle. 
Key parts have their Interactable1stPuzzleObject script for each of them. As a difference from other puzzle objects, they have their data embedded in scriptable objects for each.  

**SOCollectibleKeyPart:** As each key part can be found more than one time, and have their distinct elements (such as ID, name, inventory icon), scriptable object structure is used for each key part.  

**SnapHandler:** This class has the methods related to snapping/unsnapping the key parts to the main key. It has its own EventArgs named *SnapToSocketEventArgs* that holds the values of the *socket* and the *key* that is snapped onto that socket. In order to further manage the snapping process, there are 2 additional main events:

- OnKeySnappedToSocket
- OnKeyUnsnappedFromKey

Aside from these events' names are self-explanatory, they are subscribed from the *InteractionManager1stPuzzle* class to check if that snap is a correct step toward finding the door's target key. 

**Detecting Correct Snaps:** Each key part has a list of sockets. That stands for the sockets that key has on it as other keys may have socket to snap onto each other, not only main key. That is represented as *List<SocketData>* in the script *Interactable1stPuzzleObject*. As *SocketData* is a serializable class, it holds the target IDs for each door in a list. Therefore, based on the current door, this list is checked for each socket in the method *RecountCorrectSnaps* in the script *InteractionManager1stPuzzle*. 

### Puzzle 5 ###
**Note:** To execute this mechanic on **Unity**, *all interaction manager classes* on **Player** should be *deactivated* **except** for **InteractionManager5thPuzzle**. **PlayerUIManager** should stay *activated*. As **UI_Puzzle1Manager and SnapHandler** is related to puzzle 1, they should be also *deactivated*.  
  
There are 5 spots on a table where collected interactable objects can be placed onto. Each object can be placed on any of the spots on the table. The ultimate goal is to place the correct object onto the right spots. 

**InteractionManager5thPuzzle:** Detects near interactables (including object table), lets player obtain or drop an interactable object. Provides the switch between table view and game view. 
Update cycle is mainly as the following:

- Detecting interaction conditions being met or not
  - Detecting conditions in *world view*
    - When *hands are occupied* by an interactable
      - Detecting dropping that interactable on floor
    - When *hands are empty*
      - Detecting near colliders
      - When the table is near with at least one interactable on it, detecting switch to table view
  - Detecting conditions in *table view*
    - When *hands are occupied* by an interactable
      - Moving the interactable with mouse movements
      - Detecting switching back to game view (manages object being in hand afterwards in the game view)
      - Detecting taking an interactable from a slot of the table
    - When *hands are empty*
      - Detecting occupied slots to obtain the interactable on that slot
      - Detecting switching back to game view

**Behaviours:** After the detections are complete, changes in the interactables (such as their positions, parent objects etc.) are managed by **behaviours**. There are 6 behaviours that can be applied on the interactables:

- InteractableBehaviourPickUpFromFloor
- InteractableBehaviourPickUpFromTableToHand
- InteractableBehaviourDropOnFloor
- InteractableBehaviourPutOnTableSlot
- InteractableBehaviourDragOnTableFromHand -> After entering table view with an interactable in hand, dragging that with mouse movements
- InteractableBehaviourDragOnTableFromSlot -> After obtaining an interactable from a table slot, dragging that with mouse movements

The ones changing the table slots' states, update the slot accordingly.  
All of the behaviours update held-interactable.  
They provide new position, new parent, and next possible behaviours list. Therefore, these information are obtained by *Interactable5thPuzzleObject* script of that interactable and its state is updated using these in the method *GetInteracted_Interactable*.  
Activating/deactivating the table view is also managed by behaviours.  

**Interactable5thPuzzleTable:** Contains methods that activate/deactivate table view. Holds the lists of empty and occupied slots on it. The other methods update these lists accordingly after a behaviour is performed. Aside from these, has a method named *GetPointedSlot* that casts a ray on screen to detect the interactable pointed by mouse.

### Puzzle 7 ###
**Note:** To execute this mechanic on **Unity**, *all interaction manager classes* on **Player** should be *deactivated* **except** for **InteractionManager7thPuzzle**. **PlayerUIManager** should stay *activated*. As **UI_Puzzle1Manager and SnapHandler** is related to puzzle 1, they should be also *deactivated*.  
  
There are objects that can be switched as pairs. Each object has its correct locations which are mostly different at the start of the puzzle. The main objective is to switch objects to reach their correct locations by swapping them. 
  
**InteractionManager7thPuzzle:** Manages selecting/deselecting process of the interactable objects that will be swapped. Swapping process is conducted by this script's methods, unlike some of other puzzles which are using behaviours for the same goal.  
Main Update loop can be deducted as following:  

- Detecting interaction conditions being met or not
  - Detecting near colliders to control it through event subscription method of the event OnInteractableApproached
  - When *no interactable is selected* to be swapped
    - Detecting choosing an interactable
  - When *only one interactable is selected* to be swapped
    - When near interactable is the previously selected one, detecting deselecting it
    - When otherwise, detecting selecting near interactable
  - When *two interactables are selected* to be swapped
      - Detecting deselecting the approached interactable
      - Detecting swap between 2 selected interactables
     
Through behaviours of this puzzle, only the selection and deselection methods of manager class are called. 
