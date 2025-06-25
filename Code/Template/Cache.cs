using System.Collections.Generic;
// Template for a cache of game objects 
// which can be constructed from an identifier.
// The 'get' factory method is static.

namespace MapleStory
{
    public class Cache<T> where T : Cache<T>, new()
    {
        private static Dictionary<int, T> cache = [];

        // Return a ref to the game object with the specified id.
        // If the object is not in cache, it is created.
        public static T Get(int id)
        {
            if (!cache.TryGetValue(id, out var instance))
            {
                instance = new T();
                instance.Init(id);
                cache[id] = instance;
            }
            return instance;
        }

        // This method is meant to be overridden in subclasses
        protected virtual void Init(int id) { }
    }
}