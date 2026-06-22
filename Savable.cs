using System;


namespace SaveSystem
{
    /// <summary>
    /// Exists to make ISaveable implementations visible in the inspector.
    /// </summary>
    [Serializable]
    public abstract class Savable : ISaveable
    {
        public SerializableGuid Id { get; set; }
    }
}