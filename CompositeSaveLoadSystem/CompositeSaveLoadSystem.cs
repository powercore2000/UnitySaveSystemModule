using System;
using BozoSaveSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


namespace SaveSystem.CompositeSaveSystem
{
    public class CompositeSaveLoadSystem : SaveLoadSystem
    {
        [SerializeField] UnityEvent<Guid> onNewGameStartEvent;
        [SerializeField] UnityEvent<string> onNewGameFinishEvent;
        
        protected override void Awake()
        {
            base.Awake();
            //Debug.Log("Build inital game data save");
            gameData.systemSaveData.Add(new CustomDataModel());
            OnFinalizedNewCharacterSlot += (saveID) => onNewGameStartEvent.Invoke(saveID.ID);
            OnFinalizedNewCharacterSlot += (saveID) => onNewGameFinishEvent.Invoke(saveID.ID.ToString());
        }

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Debug.Log("New scene, starting data binding!");
            base.OnSceneLoaded(scene, mode);
            
            if (gameData.GetSystemSaveData<CustomDataModel>() is CustomDataModel dataModel)
                Bind<CustomDataBinder, CustomDataModel>(dataModel);
            
        }

    }
}