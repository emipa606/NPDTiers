# NPDTiers

Update of Honey Badgers mod for RimWorld 1.1
https://steamcommunity.com/sharedfiles/filedetails/?id=1812077718

[img]https://raw.githubusercontent.com/emipa606/RimWorld-Wrapper/master/Notice.png[/img]

Support-chat:
https://invite.gg/mlie

Non-steam version:
https://github.com/emipa606/NPDTiers
	
--- Original Description ---
NPDTiers is a base mod that adds the framework for creating your own Nutrient Paste Dispenser (NPD). Originally, the NPD is hard coded into the game making it difficult to replicate or change. With this mod however, you have more access to things in the xml files such as the meal it will dispense or even whether or not ingredients affect debuffs (like human meat). If you want to create or change the original NPD or any of the ones added by this mod, they are in Defs/Buildings/NewNPDExample.xml inside the mod folder.

4 Nutrient Paste Dispensers added by this mod
Chocolate Paste Dispenser
Produces tasty chocolate meal (+6 mood)
Only uses chocolate as a ingredient
Prisoner Nutrient Paste Dispenser
Produces disgusting meal (-10 mood)
Not affected by human or insect meat
Requires less ingredients to produce a meal
Improved Nutrient Paste Dispenser
Produces Improved NPD meal (similar to Simple Meal)
Affected by human and insect meat
Consumes same amount of ingredients as original NPD
Fine Nutrient Paste Dispenser
Produces Fine NPD meal (+2 mood)
Affected by human and insect meat
Consumes more amount of ingredients than both Original and Improved NPD (12 food)

3 Hoppers added by this mod
Refrigerator Hopper
Uses 75 power
Chills food, preserving the food and making it last longer
Freezer Hopper
Uses 150 power
Freezes Food, keeping it from spoiling as long as its kept frozen.
Grinder Hopper - Not yet Implemented
N/A
N/A



Q: Where do I go to add more nutrient paste dispensers or to edit the current ones?
A: steamapps/workshop/content/294100/1864411034/Defs/Buildings/NewNPDExample.xml

Q: What format do I add the Nutrient paste dispensers?
A: XML

Q: Can I suggest a feature?
A: Please do! We are looking in ways to further expand this mod.

Q: Is this mod incompatible with anything?
A: RimFridge currently is overwritten by the custom hoppers added by this mod.

Q: Is this mod save game compatible? Can I remove it without issue?
A: Yes and yes. But all custom NPD's must be removed from the map before you do so (original NPD is fine)

Q: Are the default dispensers added by the mod OP?
A: That is for you to decide. they are more a proof of concept and can be modified by the user if they wish.

Q: Can I make my dispenser a drug / beer dispenser?
A: Actually, Yes, You can. Replace the line &gt;isMealSource&gt;true&gt;/isMealSource&gt; with &gt;designationCategory&gt;Joy&gt;/designationCategory&gt; and during assigned rec time they will take the item it dispenses. This was found out by Cegorach.



N/A



1) Grinder Hoppers



Smash Phil - Coding, Sorted the XML Folders and Organised
Mistor Love - Sprites / Textures

GitHub: https://github.com/SmashPhil/NPDTiers
