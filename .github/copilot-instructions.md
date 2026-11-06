# NPDTiers - The Nutrient Paste Expansion Mod (Continued)
NPDTiers is a base mod that adds the framework for creating your own Nutrient Paste Dispenser (NPD). It provides more access to things in the XML files such as the meal it will dispense or whether or not ingredients affect debuffs (like human meat). This mod is compatible with all hoppers that use the isHopper-property or mods that add such a property like Fridges Are Hoppers.
## Key Features and Systems
NPDTiers adds the following systems:

  * Nutrient Paste Dispensers (NPD): 4 NPDs are added by this mod including Chocolate Paste Dispenser, Prisoner Nutrient Paste Dispenser, Improved Nutrient Paste Dispenser, and Fine Nutrient Paste Dispenser.
  * Hoppers: 3 Hoppers are added by this mod including Refrigerator Hopper, Freezer Hopper, and Grinder Hopper (not yet implemented).

## Coding Patterns and Conventions
The coding patterns and conventions used in this mod follow the RimWorld modding conventions. The source code is written in C# and uses the Harmony patching framework to modify game behavior.
## XML Integration
NPDTiers uses XML files to define new buildings, research projects, and thoughts. The XML files are located in the `Defs` folder of the mod and are named after the type of def they define (e.g. `Buildings.xml`, `Research.xml`). The XML files use the standard RimWorld XML format.
## Harmony Patching
NPDTiers uses the Harmony patching framework to modify game behavior. Harmony patches are used to add new functionality, remove existing functionality, or modify existing functionality. The Harmony patches are defined in C# files located in the `Source` folder of the mod.
## Suggestions for Copilot
Here are some suggestions for using Copilot with this mod:

  * Use Copilot to quickly generate XML code for new buildings, research projects, or thoughts.
  * Use Copilot to generate Harmony patches to modify game behavior.
  * Use Copilot to quickly generate C# code for new classes or methods.
  * Use Copilot to suggest improvements to existing code.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

