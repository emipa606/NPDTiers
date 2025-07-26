# GitHub Copilot Instructions for RimWorld Mod Development

## Mod Overview and Purpose

This RimWorld mod aims to enhance the game by refining the mechanics and capabilities of the nutrient paste dispenser and related systems. It offers customizable nutrient paste tiers and storage options aimed at providing a more flexible and immersive experience for players. This mod leverages C#, XML, and Harmony for integration and compatibility with the existing RimWorld framework.

## Key Features and Systems

- **Custom Nutrient Paste Tiers:** Allows for multiple tiers of nutrient paste with varying resource costs and benefits.
- **Hopper Customization:** Provides functionality to customize hoppers used for storing ingredients for nutrient paste dispensers.
- **Storage Systems:** Introduces additional storage solutions with configurable settings to manage resources efficiently.
- **Integration with RimWorld Mechanics:** Seamless integration with RimWorld’s existing mechanics through careful Harmony patching and XML definitions.

## Coding Patterns and Conventions

- **Class Structure:** Classes are typically structured with the use of public and internal access levels, with methods encapsulated appropriately to ensure modularity and ease of maintenance.
- **Naming Conventions:** Follow C# conventions with PascalCase for class names and method names, camelCase for local variables, and CAPITAL_SNAKE_CASE for constants.
- **Documentation and Comments:** Methods and classes should be adequately documented using XML comments. Important logic sections should be accompanied by inline comments to aid in understanding.

## XML Integration

- **DefModExtension:** Used for extending existing definitions in RimWorld, such as nutrient paste and hopper settings.
- **XML Parsing:** Custom methods such as `LoadDataFromXmlCustom` are used to handle XML configuration data specific to the mod.
- **Configuration Files:** XML files should define mod-specific data such as ingredient lists, cost multipliers, and storage settings. Ensure these files are properly formatted and placed according to RimWorld’s directory structure.

## Harmony Patching

- **Purpose:** Harmony is utilized for patching game methods, allowing for modifications without altering the base game code.
- **Implementation:** Centralized in files like `NPDHarmony.cs`, where methods and game functions are overridden or extended.
- **Use-Case:** Particularly useful for modifying behavior related to nutrient paste dispensers or when implementing storage behavior enhancements.

## Suggestions for Copilot

- **Code Completion:** Copilot can be utilized to streamline coding tasks by providing suggestions based on context, reducing repetitive coding patterns.
- **XML Assistance:** Use Copilot to generate XML snippet templates and ensure they conform to the necessary structure with correct tags and formatting.
- **Harmony Patch Generation:** Leverage Copilot to suggest patches, ensuring they target the right game methods. It can offer useful prompts for setting up prefix, postfix, or transpiler patches.
- **Error Handling and Debugging:** Rely on Copilot for efficient error handling patterns and to suggest debugging outputs that help track down bugs in the mod.

By adhering to these guidelines and leveraging GitHub Copilot’s capabilities, development and enhancement of the mod can be both efficient and effective, promoting a high-quality experience for RimWorld players.
