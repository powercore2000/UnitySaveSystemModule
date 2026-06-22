using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveSystem
{
    [JsonObject(MemberSerialization.OptIn), Serializable]
    public class SaveID
    {
        [JsonProperty] readonly Guid Id;
        [SerializeField, JsonProperty] string characterName;
        [JsonProperty, SerializeField] string cloudSaveToken;
        public Guid ID => Id;
        public string profileID => Id.ToString().Substring(0, 30);
        [SerializeField] string idDisplay;
        public string CharacterName => characterName;
        public string CloudSaveToken => cloudSaveToken;
        public void SetCharacterName(string newName) => characterName = newName;
        public void SetCloudSaveToken(string newToken) => cloudSaveToken = newToken;
        [JsonConstructor]
        public SaveID(Guid Id, string characterName) { this.Id = Id; this.characterName = characterName; idDisplay = Id.ToString();}
        
        public SaveID(Guid Id, string characterName, string token) { this.Id = Id; this.characterName = characterName; idDisplay = Id.ToString(); cloudSaveToken = token;}

        public override string ToString()
        {
            return $"{characterName} | {idDisplay}";
        }
    }

    [Serializable] 
    public class GameData{ 
        public string CurrentLevelName;
        public string SaveFileName;
        public SaveID saveID;

        [SerializeReference] public List<Savable> systemSaveData = new List<Savable>();

       
       
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Name: {saveID.CharacterName} | Level: {CurrentLevelName} | \n");
            sb.Append($"GameData GUID: {saveID.ID} |\n");
            foreach (ISaveable systemData in systemSaveData)
            {
                sb.Append($"RPG Data: {systemData.ToString()} |\n");
            }
            
            return sb.ToString();
        }
        
        public ISaveable GetSystemSaveData<SaveType>() where SaveType : ISaveable{
            return systemSaveData.FirstOrDefault(data => data.GetType() == typeof(SaveType));
        }

        
    }


}