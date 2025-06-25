using System;
using System.Collections.Generic;

namespace MapleStory
{
    // Wraps an array so that it is addressable by enumeration values
    public class EnumMap<K, V> where K : Enum
    {
        private readonly K[] _keys;
        private readonly V?[] _values;
        private int _itemCount; // To track the number of non-default items, if desired

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumMap{K, V}"/> class.
        /// All values are initialized to their default for type V (null for reference types,
        /// or default(V) for value types).
        /// </summary>
        public EnumMap()
        {
            _keys = (K[])Enum.GetValues(typeof(K));
            _values = new V?[_keys.Length];
            _itemCount = 0; // Initialize item count
                            // The _values array is already default-initialized, so no need for a loop here.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumMap{K, V}"/> class
        /// with an initial set of values.
        /// </summary>
        /// <param name="initialValues">An array of values to initialize the map.
        /// The order must correspond to the enum's underlying integer values.</param>
        /// <exception cref="ArgumentException">Thrown if more initial values are provided than enum members.</exception>
        public EnumMap(params V[] initialValues) : this() // Call the default constructor first
        {
            if (initialValues.Length > _keys.Length)
            {
                throw new ArgumentException("Too many initial values provided for the EnumMap.");
            }
            for (int i = 0; i < initialValues.Length; i++)
            {
                _values[i] = initialValues[i];
                // If V is a value type and initialValues[i] is default, this still increments count.
                // Adjust logic here if you strictly want to count 'non-default non-null' items.
                _itemCount++;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified enum key.
        /// </summary>
        /// <param name="key">The enum key.</param>
        /// <returns>The value associated with the key, or default(V) if the key is not found.</returns>
        public V? this[K key]
        {
            get
            {
                // Convert.ToInt32 handles the underlying value of the enum member.
                // No bounds check needed as Enum.GetValues provides the exact range.
                return _values[Convert.ToInt32(key)];
            }
            set
            {
                int index = Convert.ToInt32(key);
                // Update item count based on value change
                if (_values[index] == null && value != null) // Item was null, now not null
                {
                    _itemCount++;
                }
                else if (_values[index] != null && value == null) // Item was not null, now null
                {
                    _itemCount--;
                }
                _values[index] = value;
            }
        }

        /// <summary>
        /// Tries to get the value associated with the specified enum key.
        /// </summary>
        /// <param name="key">The enum key.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key,
        /// if the key is found and the value is not null; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the <see cref="EnumMap{K, V}"/> contains an element with the specified key and a non-null value; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(K key, out V? value)
        {
            int index = Convert.ToInt32(key);
            value = _values[index];
            // Returns true if the value is not null. For value types, default(V) (e.g., 0 for int)
            // is considered a valid value, so this check works correctly for nullables/reference types.
            return value != null;
        }

        /// <summary>
        /// Clears all values in the map, setting them to their default for type V, and resets the item count.
        /// </summary>
        public void Clear()
        {
            // Array.Clear is optimized for this
            Array.Clear(_values, 0, _values.Length);
            _itemCount = 0;
        }

        /// <summary>
        /// Removes the value associated with the specified key, setting it to its default for type V.
        /// </summary>
        /// <param name="key">The enum key of the element to remove.</param>
        public void Erase(K key)
        {
            int index = Convert.ToInt32(key);
            if (_values[index] != null) // Only decrement if a non-null value existed
            {
                _itemCount--;
            }
            _values[index] = default; // Sets to null for reference types/Nullable<V>, or default(V) for value types
        }

        /// <summary>
        /// Sets or updates the value associated with the specified key.
        /// </summary>
        /// <param name="key">The enum key.</param>
        /// <param name="value">The value to associate with the key.</param>
        public void Emplace(K key, V value)
        {
            int index = Convert.ToInt32(key);
            // Only increment count if the existing slot was null and the new value is not null
            if (_values[index] == null && value != null)
            {
                _itemCount++;
            }
            _values[index] = value;
        }

        /// <summary>
        /// Gets a collection of all enum keys in the map.
        /// </summary>
        public IEnumerable<K> Keys => _keys;

        /// <summary>
        /// Gets a collection of all values in the map.
        /// </summary>
        public IEnumerable<V?> Values => _values;

        /// <summary>
        /// Gets a collection of key-value pairs (entries) in the map.
        /// </summary>
        public IEnumerable<(K Key, V? Value)> Entries
        {
            get
            {
                for (int i = 0; i < _values.Length; i++)
                {
                    yield return (_keys[i], _values[i]);
                }
            }
        }

        /// <summary>
        /// Gets the number of enum members (capacity) in the map.
        /// If you want the count of non-null/non-default items, use <see cref="CurrentItemCount"/>.
        /// </summary>
        public int Capacity => _keys.Length;

        /// <summary>
        /// Gets the current number of non-null items stored in the map.
        /// </summary>
        public int CurrentItemCount => _itemCount;
    }
}