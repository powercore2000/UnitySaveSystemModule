using System;
using System.Collections.Generic;
using System.IO;
using SaveSystem;
using UnityEngine;

namespace SaveSystem {
    public class FileDataService : IDataService {
        const int SaveSlotCount = 10;

        ISerializer serializer;
        /// <summary>
        /// Where all game saves are held.
        /// </summary>
        string saveGameDirectory;
        /// <summary>
        /// Where the save for the current character is held.
        /// </summary>
        string characterSaveDirectory;
        /// <summary>
        /// Where the save ID file list is held.
        /// </summary>
        string saveIdPath;
        string fileExtension = ".json";

        SaveID currentSaveID;
        List<SaveID> saveIds = new List<SaveID>();

        bool hasCurrentCharacterSaveID() => currentSaveID != null;

        string GetPathToSaveFile(string fileName) {
            Debug.Log(characterSaveDirectory);
            Debug.Log(fileName);
            return Path.Combine(characterSaveDirectory, string.Concat(fileName, fileExtension));
        }

        public FileDataService(ISerializer serializer) {
            saveGameDirectory = Path.Combine(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games"),
                "Interview Game");

            saveIdPath = Path.Combine(saveGameDirectory, string.Concat("characterIds", fileExtension));

            this.serializer = serializer;
        }

        /// <summary>
        /// Ensures the save ID file exists and contains exactly <c>SaveSlotCount</c> entries.
        /// Missing slots are filled with newly generated GUIDs and empty character names.
        /// </summary>
        public void Initialize() {
            if (!Directory.Exists(saveGameDirectory))
                Directory.CreateDirectory(saveGameDirectory);

            if (File.Exists(saveIdPath))
                saveIds = serializer.Deserialize<List<SaveID>>(File.ReadAllText(saveIdPath));
            else
                saveIds = new List<SaveID>();

            int missing = SaveSlotCount - saveIds.Count;
            for (int i = 0; i < missing; i++)
                saveIds.Add(new SaveID(Guid.NewGuid(), string.Empty));

            if (missing > 0)
                File.WriteAllText(saveIdPath, serializer.Serialize(saveIds));
        }
        

        public void Save(GameData data, bool overwrite = true) {
            if (!hasCurrentCharacterSaveID() || data.saveID != currentSaveID)
                SetCurrentSaveID(data.saveID);

            string fileLocation = GetPathToSaveFile(data.SaveFileName);

            if (!overwrite && File.Exists(fileLocation))
                throw new IOException($"The file '{data.SaveFileName}.{fileExtension}' already exists and cannot be overwritten.");

            if (!Directory.Exists(characterSaveDirectory))
                Directory.CreateDirectory(characterSaveDirectory);

            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load(string fileName) {
            if (!hasCurrentCharacterSaveID())
                throw new ArgumentException("No save ID has been set");

            string fileLocation = GetPathToSaveFile(fileName);

            if (!File.Exists(fileLocation))
                throw new ArgumentException($"No persisted GameData with name '{fileName}'");

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name) {
            if (!hasCurrentCharacterSaveID())
                throw new ArgumentException("No save ID has been set");

            string fileLocation = GetPathToSaveFile(name);

            if (File.Exists(fileLocation))
                File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            if (!Directory.Exists(characterSaveDirectory))
                return;
            foreach (string filePath in Directory.GetFiles(characterSaveDirectory))
                if (Path.GetExtension(filePath) == fileExtension)
                {
                    Debug.Log("Deleting " + filePath);
                    File.Delete(filePath);
                }
                    
        }

        
        public IEnumerable<string> ListSaves() {
            foreach (string path in Directory.EnumerateFiles(characterSaveDirectory)) {
                if (Path.GetExtension(path) == fileExtension)
                    yield return Path.GetFileNameWithoutExtension(path);
            }
        }

        public IEnumerable<SaveID> ListSaveIds() {
            Debug.Log("List Save IDs:" + saveIdPath);
            if (File.Exists(saveIdPath))
                saveIds = serializer.Deserialize<List<SaveID>>(File.ReadAllText(saveIdPath));
            else
                throw new FileNotFoundException($"Save ID file not found at '{saveIdPath}'. Call Initialize first.");
            
            return saveIds;
        }

        public IEnumerable<GameData> ListGameData() {
            if(!hasCurrentCharacterSaveID())
                throw new ArgumentException("No save ID has been set");

            if (Directory.Exists(characterSaveDirectory))
            {
                List<GameData> gameDataList = new List<GameData>();
                foreach (string filePath in Directory.GetFiles(characterSaveDirectory))
                {
                    Debug.Log($"Looking at {filePath} with extension {Path.GetExtension(filePath)}");
                    if (Path.GetExtension(filePath) == fileExtension)
                    {
                        
                        GameData gameData = serializer.Deserialize<GameData>(File.ReadAllText(filePath));
                        if(gameData != null)
                            gameDataList.Add(gameData);
                        Debug.Log("Adding " + filePath);
                    }
                }

                return gameDataList;
            }
            
            Debug.LogError("Character save directory does not exist");
            return new List<GameData>();
        }

        
        /// <summary>
        /// Sets the active save ID and updates the character save directory accordingly.
        /// </summary>
        public void SetCurrentSaveID(SaveID saveID) {
            currentSaveID = saveID;

            // Use the GUID as the folder name when the character has no name yet
            string folderName = string.IsNullOrEmpty(saveID.CharacterName)
                ? saveID.ID.ToString()
                : saveID.CharacterName;

            characterSaveDirectory = Path.Combine(saveGameDirectory, folderName);
        }
        
        public void NewSaveId(SaveID saveID, int saveSlotIndex) {
            currentSaveID = saveID;

            if (File.Exists(saveIdPath))
                saveIds = serializer.Deserialize<List<SaveID>>(File.ReadAllText(saveIdPath));
            Debug.Log($"Replacing save slot name of {saveIds[saveSlotIndex].CharacterName} with {saveID.CharacterName}");
            saveIds[saveSlotIndex] = saveID;
            File.WriteAllText(saveIdPath, serializer.Serialize(saveIds));
            SetCurrentSaveID(saveID);
        }

        /// <summary>
        /// Returns the save ID at the given zero-based slot index by parsing the save ID file.
        /// </summary>
        public SaveID LoadSaveId(int index) {
            if (!File.Exists(saveIdPath))
                throw new FileNotFoundException($"Save ID file not found at '{saveIdPath}'. Call Initialize first.");

            List<SaveID> slots = serializer.Deserialize<List<SaveID>>(File.ReadAllText(saveIdPath));

            if (index < 0 || index >= slots.Count)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Index {index} is out of range. Save slot count: {slots.Count}");

            return slots[index];
        }
        
        public void DeleteSaveId(Guid id) {
            saveIds = serializer.Deserialize<List<SaveID>>(File.ReadAllText(saveIdPath));

            for (int i = 0; i < saveIds.Count; i++) {
                if (saveIds[i].ID == id)
                {
                    string oldDir = characterSaveDirectory;
                    characterSaveDirectory = Path.Combine(saveGameDirectory, saveIds[i].CharacterName);
                    DeleteAll();
                    saveIds[i].SetCharacterName(string.Empty);
                    characterSaveDirectory = oldDir;
                    Debug.Log("Deleted Save ID");
                    break;
                }
            }

            File.WriteAllText(saveIdPath, serializer.Serialize(saveIds));
        }


    }
}
