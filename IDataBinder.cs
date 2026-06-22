namespace SaveSystem
{
    
    public interface IDataBinder<TData> where TData : ISaveable {
        SerializableGuid Id { get; set; }
        /// <summary>
        /// Provides relevant system data to the system binder.
        /// If it's new game data, will be mutated to reflect system state.
        /// If it's loaded data, will mutate system state to reflect loaded data.
        /// </summary>
        /// <param name="data"></param>
        void Bind(ref TData data);

    }

}