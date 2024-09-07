# [NPDTiers - The Nutrient Paste Expansion Mod (Continued)](https://steamcommunity.com/sharedfiles/filedetails/?id=2043895447)

![Image](https://i.imgur.com/buuPQel.png)

Update of Mistor Love n Smash Phils mod
https://steamcommunity.com/sharedfiles/filedetails/?id=1864411034

- Should now work with all hoppers that use the isHopper-property, or mods that add such a property like [Fridges Are Hoppers](https://steamcommunity.com/sharedfiles/filedetails/?id=2894860548)

![Image](https://i.imgur.com/pufA0kM.png)
	
![Image](https://i.imgur.com/Z4GOv8H.png)

![Image](https://i.imgur.com/4eOBD6w.png)

NPDTiers is a base mod that adds the framework for creating your own Nutrient Paste Dispenser (NPD). Originally, the NPD is hard coded into the game making it difficult to replicate or change.  With this mod however, you have more access to things in the xml files such as the meal it will dispense or even whether or not ingredients affect debuffs (like human meat). If you want to create or change the original NPD or any of the ones added by this mod, they are in Defs/Buildings/NewNPDExample.xml inside the mod folder.
 
#  4 Nutrient Paste Dispensers added by this mod 

Chocolate Paste Dispenser


    - Produces tasty chocolate meal (+6 mood)
    - Only uses chocolate as a ingredient


Prisoner Nutrient Paste Dispenser


    - Produces disgusting meal (-10 mood)
    - Not affected by human or insect meat
    - Requires less ingredients to produce a meal


Improved Nutrient Paste Dispenser


    - Produces Improved NPD meal (similar to Simple Meal)
    - Affected by human and insect meat
    - Consumes same amount of ingredients as original NPD


Fine Nutrient Paste Dispenser


    - Produces Fine NPD meal (+2 mood)
    - Affected by human and insect meat
    - Consumes more amount of ingredients than both Original and Improved NPD (12 food)



#  3 Hoppers added by this mod 

Refrigerator Hopper


    - Uses 75 power
    - Chills food, preserving the food and making it last longer


Freezer Hopper


    - Uses 150 power
    - Freezes Food, keeping it from spoiling as long as its kept frozen.


Grinder Hopper - Not yet Implemented


    - N/A
    - N/A



![Image](https://i.imgur.com/WPOySFN.png)

Q: Where do I go to add more nutrient paste dispensers or to edit the current ones?
A: steamapps/workshop/content/294100/2043895447/Defs/Buildings/NewNPDExample.xml

Q: What format do I add the Nutrient paste dispensers?
A: XML

Q: Can I suggest a feature?
A: Please do! We are looking in ways to further expand this mod.

Q: Is this mod incompatible with anything?
A: ~~RimFridge currently is overwritten by the custom hoppers added by this mod.~~ No longer an issue in 1.3
 
Q: Is this mod save game compatible? Can I remove it without issue?
A: Yes and yes. But all custom NPD's must be removed from the map before you do so (original NPD is fine)

Q: Are the default dispensers added by the mod OP?
A: That is for you to decide. they are more a proof of concept and can be modified by the user if they wish.

Q: Can I make my dispenser a drug / beer dispenser?
A: Actually, Yes, You can. Replace the line isMealSourcetrue/isMealSource with designationCategoryJoy/designationCategory and during assigned rec time they will take the item it dispenses. This was found out by Cegorach.

![Image](https://i.imgur.com/FSusaPE.png)

N/A

![Image](https://i.imgur.com/XMsjox3.png)

1) Grinder Hoppers

![Image](https://i.imgur.com/agSDUsC.png)

Smash Phil - Coding, Sorted the XML Folders and Organised
Mistor Love - Sprites / Textures

![Image](https://i.imgur.com/PwoNOj4.png)



-  See if the the error persists if you just have this mod and its requirements active.
-  If not, try adding your other mods until it happens again.
-  Post your error-log using [HugsLib](https://steamcommunity.com/workshop/filedetails/?id=818773962) or the standalone [Uploader](https://steamcommunity.com/sharedfiles/filedetails/?id=2873415404) and command Ctrl+F12
-  For best support, please use the Discord-channel for error-reporting.
-  Do not report errors by making a discussion-thread, I get no notification of that.
-  If you have the solution for a problem, please post it to the GitHub repository.
-  Use [RimSort](https://github.com/RimSort/RimSort/releases/latest) to sort your mods



[![Image](https://img.shields.io/github/v/release/emipa606/NPDTiers?label=latest%20version&style=plastic&color=9f1111&labelColor=black)](https://steamcommunity.com/sharedfiles/filedetails/changelog/2043895447)
