using System.Collections.Generic;

namespace MongoDB.Entities
{
    public static partial class DB
    {
        /// <summary>
        /// Retrieves the 'change-stream' watcher instance for a given unique name. 
        /// If an instance for the name does not exist, it will return a new instance. 
        /// If an instance already exists, that instance will be returned.
        /// </summary>
        /// <typeparam name="T">The entity type to get a watcher for</typeparam>
        /// <param name="name">A unique name for the watcher of this entity type. Names can be duplicate among different entity types.</param>
        public static Watcher<T, T> Watcher<T>(string name) where T : IEntity
            => Watcher<T, T>(name);

        /// <summary>
        /// Retrieves the 'change-stream' watcher instance for a given unique name. 
        /// If an instance for the name does not exist, it will return a new instance. 
        /// If an instance already exists, that instance will be returned.
        /// </summary>
        /// <typeparam name="T">The entity type to get a watcher for</typeparam>
        /// <typeparam name="TProjection">The type of the projected result</typeparam>
        /// <param name="name">A unique name for the watcher of this entity type. Names can be duplicate among different entity types.</param>
        public static Watcher<T, TProjection> Watcher<T, TProjection>(string name) where T : IEntity
        {
            if (Entities.Watcher<T, TProjection>.Watchers.TryGetValue(name.ToLower().Trim(), out Watcher<T, TProjection> watcher))
                return watcher;

            watcher = new Watcher<T, TProjection>(name.ToLower().Trim());
            Entities.Watcher<T, TProjection>.Watchers.TryAdd(name, watcher);
            return watcher;
        }

        /// <summary>
        /// Returns all the watchers for a given entity type
        /// </summary>
        /// <typeparam name="T">The entity type to get the watcher of</typeparam>
        public static IEnumerable<Watcher<T, T>> Watchers<T>() where T : IEntity
            => Watchers<T, T>();

        /// <summary>
        /// Returns all the watchers for a given entity type
        /// </summary>
        /// <typeparam name="T">The entity type to get the watcher of</typeparam>
        /// <typeparam name="TProjection">The type of the projected result</typeparam>
        public static IEnumerable<Watcher<T, TProjection>> Watchers<T, TProjection>() where T : IEntity
            => Entities.Watcher<T, TProjection>.Watchers.Values;
    }
}
