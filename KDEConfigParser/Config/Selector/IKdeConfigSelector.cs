using System.Text.RegularExpressions;

namespace Scot.Massie.KDEConfigParser.Config.Selector;

/// <summary>
/// Tool for keeping track of/identifying portions of a <see cref="IKdeConfig">KDE config.</see>
/// </summary>
/// <remarks>
/// This is primarily to be able to associate traits with particular keys or portions of the config separately from the
/// data actually stored in it.
/// </remarks>
public interface IKdeConfigSelector
{
    /// <summary>
    /// Whether this matches everything in the entire config.
    /// </summary>
    bool MatchesEverything { get; set; }

    /// <summary>
    /// Gets whether this matches everything in a particular category.
    /// </summary>
    /// <param name="category">The category name to check. Null to check the default category.</param>
    /// <returns>True if the given category is matched. Otherwise, false.</returns>
    /// <remarks>
    /// Even if the category hasn't specifically been matched, it'll be matched if this
    /// <see cref="MatchesEverything">matches everything.</see>
    /// </remarks>
    bool MatchesCategory(string? category);

    /// <summary>
    /// Gets whether a particular key is matched.
    /// </summary>
    /// <param name="category">The category the key is in. Null for the key to be in the default category.</param>
    /// <param name="key">The key to checked.</param>
    /// <returns>True if the key is matched. Otherwise, false.</returns>
    /// <remarks>
    /// Even if the key hasn't specifically been matched, it'll still be matched if the
    /// <see cref="MatchesCategory">category is matched,</see> or if
    /// <see cref="MatchesEverything">everything is matched.</see>
    /// </remarks>
    bool MatchesKey(string? category, string key);

    /// <summary>
    /// Makes this start matching everything in the given category.
    /// </summary>
    /// <param name="category">The category to match. Null to match the default category.</param>
    void AddCategory(string? category);

    /// <summary>
    /// Makes this start matching the given key in the given category.
    /// </summary>
    /// <param name="category">The category the key is in. Null for the default category.</param>
    /// <param name="key">The key to match.</param>
    void AddKey(string? category, string key);

    /// <summary>
    /// Makes this start matching the given key in any category.
    /// </summary>
    /// <param name="key">The key to match.</param>
    void AddKey(string key);
    
    /// <summary>
    /// Makes this start matching any key whose name the given regex matches, in any category.
    /// </summary>
    /// <param name="keyMatcher">The regex to compare keys against.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions"/>
    void AddKeyMatcher(Regex keyMatcher);
    
    /// <summary>
    /// Makes this start matching any key whose name the given regex matches, in the given category.
    /// </summary>
    /// <param name="category">The category the key should be in. Null for the default category.</param>
    /// <param name="keyMatcher">The regex to compare keys against.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions"/>
    void AddKeyMatcher(string? category, Regex keyMatcher);
    
    /// <summary>
    /// Makes this start matching any key whose name the given key regex matches, in any category the given category
    /// regex matches.
    /// </summary>
    /// <param name="categoryMatcher">The regex to compare categories against.</param>
    /// <param name="keyMatcher">The regex to compare keys against.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions"/>
    /// <remarks>Both the key matcher and the category matcher have to match.</remarks>
    void AddCategoryAndKeyMatcher(Regex categoryMatcher, Regex keyMatcher);

    /// <summary>
    /// Makes this start matching everything in any category whose name the given regex matches.
    /// </summary>
    /// <param name="categoryMatcher">The regex to compare categories against.</param>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions"/>
    void AddCategoryMatcher(Regex categoryMatcher);
    
    /// <summary>
    /// If a category has been added by name (not by regex), this makes this stop matching specifically that. It may
    /// still be matched if it's being matched by a regex or by everything.
    /// </summary>
    /// <param name="category">The category to stop matching. Null for the default category.</param>
    void RemoveCategory(string? category);
    
    /// <summary>
    /// If a key has been added by name (not by regex), this makes this stop matching specifically that. It may still
    /// be matched by a regex or if the category it's in is matched.
    /// </summary>
    /// <param name="category">The category the key is in. Null for the default category.</param>
    /// <param name="key">The key to stop matching.</param>
    void RemoveKey(string? category, string key);

    /// <summary>
    /// If a key has been added by name (not by regex) for any category, this makes this stop matching specifically
    /// that. It may still be matched by a regex or if the category of the particular key being queried is matched.
    /// </summary>
    /// <param name="key">The key to stop matching.</param>
    void RemoveKey(string key);

    /// <summary>
    /// Removes all matching. This will no longer match anything after this method is called.
    /// </summary>
    void Clear();
}
