using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;

namespace Scot.Massie.KDEConfigParser.Config;

/// <summary>
/// Basic configuration type for the KDE configuration format.
/// </summary>
/// <remarks>
/// This type implements the capacity to manage a configuration as specified by the KDE configuration file format, but
/// does not handle file loading/saving. For that, use <see cref="IKdeConfigFile"/> instead.
///
/// As localisation codes supported by C# is an exact subset of those supported by KDE (except `iv`, the invariant
/// culture), I directly pass the two-letter ISO code from the <see cref="CultureInfo"/> object, if used instead of an
/// explicit two-letter code.
/// </remarks>
/// <seealso href="https://userbase.kde.org/KDE_System_Administration/Configuration_Files"/>
/// <seealso href="https://community.kde.org/KDE_Core/ISO_Codes"/>
public interface IKdeConfig
{
    /// <summary>
    /// Whether there are no entries in this configuration.
    /// <remarks>Categories can't exist without entries, even if they appeared in the source.</remarks>
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Accesses the given key in the given category.
    /// </summary>
    /// <param name="category">The category to look for the key in. If null, the default category is looked in.</param>
    /// <param name="key">The key to access the value of.</param>
    /// <returns>
    /// The value associated with the given key in the given category, or null if there is no such value.
    /// </returns>
    /// <remarks>Assigning null to this removes the value. (for the default locale)</remarks>
    string? this[string? category, string key]
    {
        get;
        set;
    }

    /// <summary>
    /// Accesses the given key in the given category, for the given locale.
    /// </summary>
    /// <param name="category">The category to look for the key in. If null, the default category is looked in.</param>
    /// <param name="key">The key to access the value of.</param>
    /// <param name="locale">The locale to access the key for.</param>
    /// <returns>
    /// The value associated with the given key in the given category, or null if there is no such value.
    /// </returns>
    /// <remarks>
    /// Assigning null to this removes the value for the given locale. Note that doing this doesn't remove the value for
    /// the default locale.
    /// </remarks>
    string? this[string? category, string key, string locale]
    {
        get;
        set;
    }

    /// <summary>
    /// Accesses the given key in the given category, for the given locale.
    /// </summary>
    /// <param name="category">The category to look for the key in. If null, the default category is looked in.</param>
    /// <param name="key">The key to access the value of.</param>
    /// <param name="locale">The locale to access the key for.</param>
    /// <returns>
    /// The value associated with the given key in the given category, or null if there is no such value.
    /// </returns>
    /// <remarks>
    /// Assigning null to this removes the value for the given locale. Note that doing this doesn't remove the value for
    /// the default locale.
    /// </remarks>
    string? this[string? category, string key, CultureInfo locale]
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the value assigned to the given key, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get the value of.</param>
    /// <returns>
    /// The value assigned to the given key, appropriate for the user's current locale, or null if there is no such
    /// entry.
    /// </returns>
    string? Get(string? category, string key);
    
    /// <summary>
    /// Gets the value assigned to the given key for the given locale, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get the value of.</param>
    /// <param name="locale">The two-letter code representing the locale to get the value for.</param>
    /// <returns>
    /// The value assigned to the given key, appropriate for the given locale, or null if there is no such entry.
    /// </returns>
    /// <seealso href="https://community.kde.org/KDE_Core/ISO_Codes"/>
    string? Get(string? category, string key, string locale);
    
    /// <summary>
    /// Gets the value assigned to the given key for the given locale, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get the value of.</param>
    /// <param name="locale">The culture info representing the locale to get the value for.</param>
    /// <returns>
    /// The value assigned to the given key, appropriate for the given locale, or null if there is no such entry.
    /// </returns>
    string? Get(string? category, string key, CultureInfo locale);
    
    /// <summary>
    /// Gets the value assigned to the given key, specifically with no locale specified, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get the value of.</param>
    /// <returns>
    /// The value assigned to the given key, specifically with no locale tags, or null if there is no such entry.
    /// </returns>
    string? GetInvariant(string? category, string key);

    /// <summary>
    /// Gets the assignment info for the given key, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get information about the value of.</param>
    /// <returns>
    /// Information about the value assigned to the given key, appropriate for the user's current locale, or null if
    /// there is no such entry.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(string? category, string key);

    /// <summary>
    /// Gets the assignment info for the given key for the given locale, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get information about the value of.</param>
    /// <param name="locale">The two-letter code representing the locale to get information about the value for.</param>
    /// <returns>
    /// Information about the value assigned to the given key, appropriate for the given locale, or null if there is no
    /// such entry.
    /// </returns>
    /// <seealso href="https://community.kde.org/KDE_Core/ISO_Codes"/>
    KdeConfigEntryAssignment? GetInfo(string? category, string key, string locale);

    /// <summary>
    /// Gets the assignment info for the given key for the given locale, in the given category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get information about the value of.</param>
    /// <param name="locale">The culture info representing the locale to get information about the value for.</param>
    /// <returns>
    /// Information about the value assigned to the given key, appropriate for the given locale, or null if there is no
    /// such entry.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(string? category, string key, CultureInfo locale);

    /// <summary>
    /// Gets information about the value assigned to the given key, specifically with no locale specified, in the given
    /// category.
    /// </summary>
    /// <param name="category">The category to look for the given key in. Null specifies the default category.</param>
    /// <param name="key">The key to get information about the value of.</param>
    /// <returns>
    /// Information about the value assigned to the given key, specifically with no locale tags, or null if there is no
    /// such entry.
    /// </returns>
    KdeConfigEntryAssignment? GetInvariantInfo(string? category, string key);

    /// <summary>
    /// Gets whether a category of the given name exists with any keys.
    /// </summary>
    /// <param name="categoryName">The name of the category to check for.</param>
    /// <returns>True if this contains a category of the given name. Otherwise, false.</returns>
    /// <remarks>A category cannot exist with no keys.</remarks>
    bool CategoryExists(string categoryName);

    /// <summary>
    /// Gets the names of all categories with keys.
    /// </summary>
    /// <returns>A collection of the names of all existing categories with keys.</returns>
    ICollection<string> GetCategories();

    /// <summary>
    /// Gets all keys in the given category.
    /// </summary>
    /// <param name="category">The category to get the keys of.</param>
    /// <returns>A collection of the keys in the given category.</returns>
    /// <remarks>
    /// Keys are given if *any* localisation exists, even if none are appropriate for the user's current locale.
    /// </remarks>
    ICollection<string> GetKeysInCategory(string category);

    /// <summary>
    /// Gets all keys in the default category.
    /// </summary>
    /// <returns>A collection of the keys in the default category.</returns>
    /// <remarks>
    /// Keys are given if *any* localisation exists, even if none are appropriate for the user's current locale.
    /// </remarks>
    ICollection<string> GetKeysInDefaultCategory();

    /// <summary>
    /// Sets the value for the given key in the given category.
    /// </summary>
    /// <param name="category">The category to set the value of a key in.</param>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansion is enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked down.</param>
    /// <remarks>
    /// This sets the key for the invariant locale. i.e. with no locale specified, not even the user's current locale.
    ///
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string? category, string key, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value for the given key for the given locale in the given category.
    /// </summary>
    /// <param name="category">The category to set the value of a key in.</param>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="locale">The specific locale to set a value for the key for.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansion is enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked down.</param>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string? category, string key, string locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);
    
    /// <summary>
    /// Sets the value for the given key for the given locale in the given category.
    /// </summary>
    /// <param name="category">The category to set the value of a key in.</param>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="locale">The specific locale to set a value for the key for.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansion is enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked down.</param>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string? category, string key, CultureInfo locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);
    
    /// <summary>
    /// Removes everything in this configuration.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes everything in the given category.
    /// </summary>
    /// <param name="category">The name of the category to clear. Null specifies the default category.</param>
    void Clear(string? category);

    /// <summary>
    /// Clears the value of the given key in the given category.
    /// </summary>
    /// <param name="category">The category to clear a key in. Null specifies the default category.</param>
    /// <param name="key">The key to clear.</param>
    /// <remarks>
    /// This clears all localisations. To clear only the default localisation, use
    /// <see cref="ClearDefaultLocalisation"/> instead.
    /// </remarks>
    void Clear(string? category, string key);

    /// <summary>
    /// Clears the value of the given key, specifically for the given locale, in the given category.
    /// </summary>
    /// <param name="category">The category to clear a key in. Null specifies the default category.</param>
    /// <param name="key">The key to clear.</param>
    /// <param name="locale">The specific locale to clear.</param>
    void Clear(string? category, string key, string locale);

    /// <summary>
    /// Clears the value of the given key, specifically for the given locale, in the given category.
    /// </summary>
    /// <param name="category">The category to clear a key in. Null specifies the default category.</param>
    /// <param name="key">The key to clear.</param>
    /// <param name="locale">The specific locale to clear.</param>
    void Clear(string? category, string key, CultureInfo locale);

    /// <summary>
    /// Clears the value of the given key, specifically for the default locale, in the given category.
    /// </summary>
    /// <param name="category">The category to clear a key in. Null specifies the default category.</param>
    /// <param name="key">The key to clear.</param>
    void ClearDefaultLocalisation(string? category, string key);

    /// <summary>
    /// Reads the remaining lines from the given text reader, handling them as either category names or entries.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <remarks>This does not handle disposal.</remarks>
    void ReadFromTextReader(TextReader reader);

    /// <summary>
    /// Replaces the contents of this configuration with what can be read from the given string.
    /// </summary>
    /// <param name="source">The configuration code to read into the configuration.</param>
    void LoadFromString(string source);

    /// <summary>
    /// Writes the contents of this configuration to the given writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <remarks>Orders the categories in string order by category name.</remarks>
    void WriteToTextWriter(TextWriter writer);

    /// <summary>
    /// Outputs the full contents of this configuration as a string.
    /// </summary>
    /// <returns>
    /// A string containing the contents of this configuration formatted as text, in the KDE configuration format.
    /// </returns>
    /// <remarks>
    /// Although a file representation should have a trailing newline, the string representation should not.
    /// </remarks>
    string ToStringInFull();
}
