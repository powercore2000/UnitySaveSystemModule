using System;
using System.Collections.Generic;
using SaveSystem;

namespace SaveSystem {
    public interface IDataService {
        void Save(GameData data, bool overwrite = true);
        GameData Load(string fileName);
        void Delete(string fileName);
        void DeleteAll();
        IEnumerable<string> ListSaves();

        /// <summary>
        /// Ensures the save ID file exists and contains exactly <c>SaveSlotCount</c> entries.
        /// Any missing slots are filled with new GUIDs and empty character names.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sets the active save ID used for subsequent save/load operations.
        /// </summary>
        void SetCurrentSaveID(SaveID saveID);
        
        IEnumerable<SaveID> ListSaveIds();
        IEnumerable<GameData> ListGameData();
        void NewSaveId(SaveID saveID, int saveSlotIndex);
        void DeleteSaveId(Guid id);

        /// <summary>
        /// Returns the <see cref="SaveID"/> at the given zero-based slot index.
        /// </summary>
        SaveID LoadSaveId(int index);
    }
}