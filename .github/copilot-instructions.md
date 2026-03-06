# GitHub Copilot Instructions for NPDTiers - The Nutrient Paste Expansion Mod (Continued)

## Mod Overview and Purpose

**NPDTiers - The Nutrient Paste Expansion Mod (Continued)** is an extensive framework designed to provide modders with the capability to create and modify Nutrient Paste Dispensers (NPD) in RimWorld. It addresses the limitations of the original NPD, which is hardcoded into the game, by granting users the ability to configure custom dispensers through XML, allowing customization of dispensed meals and ingredient effects.

## Key Features and Systems

The mod introduces four distinct types of Nutrient Paste Dispensers and three unique hoppers, each designed with specific gameplay mechanics:

- **Chocolate Paste Dispenser**
  - Dispenses a chocolate meal that grants a +6 mood boost.
  - Utilizes chocolate exclusively as an ingredient.

- **Prisoner Nutrient Paste Dispenser**
  - Dispenses a meal with a -10 mood, unaffected by human or insect meat.
  - Requires fewer ingredients compared to standard meals.

- **Improved Nutrient Paste Dispenser**
  - Produces a meal similar to a Simple Meal.
  - Affected by human and insect meat.
  - Ingredient consumption is comparable to the original NPD.

- **Fine Nutrient Paste Dispenser**
  - Creates a meal that provides a +2 mood enhancement.
  - Affected by human and insect meat.
  - Consumes more ingredients than both the Original and Improved NPDs.

Additionally, the mod adds hoppers:

- **Refrigerator Hopper**: Utilizes 75 power to chill food, extending its preservation.
- **Freezer Hopper**: Consumes 150 power to freeze food indefinitely.
- **Grinder Hopper**: Not yet implemented.

## Coding Patterns and Conventions

Coding within this mod follows typical C# conventions, using static classes and methods to handle logic associated with various components of the dispensers and hoppers. Take note of file naming conventions - class names such as `Building_NutrientPasteDispenser_GetGizmos` suggest functionality tied to specific components.

## XML Integration

To add or modify nutrient paste dispensers, users should navigate to:
`steamapps/workshop/content/294100/2043895447/Defs/Buildings/NewNPDExample.xml`

The format for making changes or additions is XML, allowing for extensive customization and integration with new or existing dispensers.

## Harmony Patching

The mod employs Harmony patches to extend and modify base game functionality seamlessly. `NPDHarmony` is the internal static class for managing these patches, allowing for dynamic changes without altering the original game code directly.

## Suggestions for Copilot

When using GitHub Copilot for further development or modification:

- **XML Editing**: Use Copilot to suggest XML structures by defining typical nutrient dispenser properties.
- **Harmony Patches**: Let Copilot assist in generating harmony patches by creating transpilers or prefix/postfix patches for desired methods.
- **Static Method Optimization**: Implement utilities or helper functions using Copilot for recurring functionalities in static methods.
- **Debugging**: Allow Copilot to propose logging or debugging strategies when testing new components or features.
- **Custom Dispenser**: Get insights on introducing new dispenser types by encouraging Copilot to automate parts of JSON or XML generation based on documented patterns.

## FAQs and Notes

- **Compatibility**: The mod is compatible with most setups after version 1.3, though it can conflict with RimFridge due to custom hoppers.
- **Save Game Compatibility**: Fully compatible, but custom NPDs must be removed before uninstallation.
- **Expansion Ideas**: Users are encouraged to suggest new features – expanding functionality through Copilot can assist here by brainstorming implementations.
- **Bug Reporting**: Use the Discord channel for discussion; GitHub issues can be used for more structured reports.

**Credits**: 
- Smash Phil: Coding, XML sorting, and organization.
- Mistor Love: Sprites and textures.

Use `RimSort` to manage mod load order for optimal performance.
