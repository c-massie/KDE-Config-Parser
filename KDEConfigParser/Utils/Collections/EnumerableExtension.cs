namespace Scot.Massie.KDEConfigParser.Utils.Collections;

internal static class EnumerableExtension
{
    /// <summary>
    /// Gets the indices in the given enumerable where the given item can be found.
    /// </summary>
    /// <param name="enumerable">The enumerable to look in.</param>
    /// <param name="lookingFor">The item being looked for.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <returns>An enumeration of the indices in the given enumerable where the item can be found.</returns>
    public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> enumerable, T lookingFor)
    {
        var i = 0;

        foreach(var item in enumerable)
        {
            if(Equals(item, lookingFor))
                yield return i;

            i++;
        }
    }

    /// <summary>
    /// Checks if an enumerable contains any of the given items.
    /// </summary>
    /// <param name="enumerable">The enumerable to look in.</param>
    /// <param name="lookingFor">An array of the items to look for.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <returns>True if any of the items in the given array appear in the enumerable. Otherwise, false.</returns>
    public static bool ContainsAnyOf<T>(this IEnumerable<T> enumerable, params T[] lookingFor)
    {
        foreach(var item in enumerable)
        {
            for(int i = 0; i < lookingFor.Length; i++)
            {
                var other = lookingFor[i];

                if(Equals(item, other))
                    return true;
            }
        }

        return false;
    }
}
