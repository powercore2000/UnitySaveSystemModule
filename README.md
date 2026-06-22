# UnitySaveSystemModule
A json based save system that uses generics and the data binding pattern to support serializing and retriving data models from other systems in a game project. Uses components and was inspired by [Git Amend](https://github.com/adammyhre/Unity-Inventory-System)

Uses the `CompositeSaveLoadSystem.cs` file to act as a  coupling to specific systems in the game project with a custom .asdmef file to be defined by the user, or ommited if desired. To add data models to the SaveLoad system, see example written in `CompositeSaveLoadSystem.cs`  
