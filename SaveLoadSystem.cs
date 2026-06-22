using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilityScripts;
//using Systems.Inventory;

namespace SaveSystem {
    
    [Serializable]
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem> {
        [SerializeField] public GameData gameData;
        IDataService dataService;

        int currentSaveSlotIndex;
        public int CurrentSaveSlotIndex => currentSaveSlotIndex;
        public void SetCurrentSaveSlotIndex(int index) => currentSaveSlotIndex = index;
        
        /// <summary>
        /// Only subscribe OnStart, this list is cleared ever SceneManager.onSceneLoaded
        /// </summary>
        public static Action<GameData> OnGameLoaded;
        public static Action<GameData> OnGameSaved;
                
        public static Action<SaveID> OnFinalizedNewCharacterSlot;

        protected override void Awake() {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
            dataService.Initialize();
        }

        protected void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        protected void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
        
        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        { }
        
       public void Bind<T, TData>(TData data) where T : MonoBehaviour, IDataBinder<TData> where TData : ISaveable, new() {
            T entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null) {
                if (data == null) {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(ref data);
            }
        }
        public void Bind<T, TData>(TData data, T binderEntity) where T : IDataBinder<TData> where TData : ISaveable, new() {
            T entity = binderEntity;
            if (entity != null) {
                if (data == null) {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(ref data);
            }
        }

        public virtual void NewCharacterSlot(int saveSlotId)
        {
            Debug.Log("Creating new character slot");
            SaveID oldSlot = dataService.LoadSaveId(
                saveSlotId);
            gameData.saveID = new SaveID(
                oldSlot.ID,
                String.IsNullOrEmpty(gameData.saveID.CharacterName) ? "Blank Character" : gameData.saveID.CharacterName,
                oldSlot.CloudSaveToken);
            OnFinalizedNewCharacterSlot?.Invoke(gameData.saveID);
            dataService.NewSaveId(gameData.saveID, saveSlotId);
            currentSaveSlotIndex = saveSlotId;
            gameData.SaveFileName = "Initial Auto Save";
        }

        public virtual void OverwriteCharacterSlot(SaveID newSaveId, int saveSlotId)
        {
            dataService.NewSaveId(newSaveId, saveSlotId);
        }
        /// <summary>
        /// Sets the current save ID to the one at the given slot index.
        /// </summary>
        /// <param name="saveSlotId"></param>
        public void LoadCharacterSlot(int saveSlotId)
        {
            SaveID saveId = dataService.LoadSaveId(saveSlotId);
            currentSaveSlotIndex = saveSlotId;
            gameData.saveID = saveId;
            dataService.SetCurrentSaveID(gameData.saveID);
            
        }

        public void ResetCurrentCharacter()
        {
            Guid newGuid = gameData.saveID.ID;
            gameData = new GameData();
            gameData.saveID = new SaveID(newGuid,String.Empty);
            dataService.SetCurrentSaveID(gameData.saveID);
        }
        /// <summary>
        /// Returns the save ID at the given slot index.
        /// </summary>
        /// <param name="saveSlotId"></param>
        /// <returns></returns>
        public SaveID GetCharacterSlot(int saveSlotId)
        {
            SaveID saveId = dataService.LoadSaveId(saveSlotId);
            return saveId;
        }
        public List<SaveID> LoadAllCharacterSlots()
        {
            return dataService.ListSaveIds().ToList();
        }
        public void DeleteCharacterSlot(int saveSlotId)
        {
            dataService.DeleteSaveId(dataService.LoadSaveId(saveSlotId).ID);
        }
       

        public void SaveGame()
        {
            OnGameSaved?.Invoke(gameData);
            dataService.Save(gameData);
        }

        public void LoadGame(string fileName) {
            
            
            gameData = dataService.Load(fileName);
            OnGameLoaded?.Invoke(gameData);
            Debug.Log("Got Game data: " + gameData.ToString());
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
        }
        
        public List<GameData> LoadAllGameData()
        {
            return dataService.ListGameData().ToList();
        }

        public void DeleteGame(string fileName) => dataService.Delete(fileName);

        protected override void  OnApplicationQuit()
        {
            currentSaveSlotIndex = -1;
            OnFinalizedNewCharacterSlot = null;
            OnGameLoaded = null;
            OnGameSaved = null;
            instance = null;
        }
    }
    
    
}