namespace Scot.Massie.KDEConfigParser.Utils.Collections;

internal static class DictionaryExtension
{
    /// <summary>
    /// Gets the value in the dictionary with the given key, or the default value of the dictionary's key type if that
    /// key does not exist.
    /// </summary>
    /// <param name="dict">The dictionary to look in.</param>
    /// <param name="key">The key to look for.</param>
    /// <typeparam name="TKey">The dictinoary's key type.</typeparam>
    /// <typeparam name="TValue">The dictionary's value type.</typeparam>
    /// <returns>
    /// The value assigned to the given key in this dictionary, or the default value (e.g. null for reference types) if
    /// no such key exists.
    /// </returns>
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    {
        return dict.TryGetValue(key, out var value) ? value : default;
    }
}
