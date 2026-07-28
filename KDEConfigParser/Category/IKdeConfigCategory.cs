using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Entry;

namespace Scot.Massie.KDEConfigParser.Category;

/// <summary>
/// Representation of a category within a KDE configuration. e.g. in a KDE configuration file, this would be the names
/// in square brackets, and everything below that should be sorted into that category.
/// </summary>
public interface IKdeConfigCategory
{
    /// <summary>
    /// The name of the category.
    /// </summary>
    /// <remarks>
    /// In a KDE configuration file, this is the part represented in square brackets. e.g. `[Category name]`
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Whether this category contains any entries.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Whether this category is the default category for the config it's from.
    /// </summary>
    bool IsDefaultCategory { get; }

    /// <summary>
    /// Accesses the value of the given key.
    /// </summary>
    /// <param name="key">The key to access the value of.</param>
    /// <returns>
    /// The value associated with the given key, appropriate for the user's current locale. If there is no such entry,
    /// this returns null.
    /// </returns>
    /// <exception cref="FormatException">When setting, if the key is not a valid key.</exception>
    /// <remarks>Assigning null to this removes the value. (for the default locale)</remarks>
    string? this[string key] { get; set; }

    /// <summary>
    /// Accesses the value for the given key, for the given locale.
    /// </summary>
    /// <param name="key">The key to access the value of.</param>
    /// <param name="locale">
    /// The locale to access the value for. An empty string to access specifically where no locale is provided.
    /// </param>
    /// <returns>
    /// The value associated with the given key, appropriate for the given locale. If there is no such entry, this
    /// returns null.
    /// </returns>
    /// <exception cref="FormatException">When setting, if the key is not a valid key.</exception>
    /// <remarks>
    /// Assigning null to this removes the value for the given locale. Note that doing this doesn't remove the value for
    /// the default locale.
    /// </remarks>
    string? this[string key, string locale] { get; set; }

    /// <summary>
    /// Accesses the value for the given key, for the given locale.
    /// </summary>
    /// <param name="key">The key to access the value of.</param>
    /// <param name="locale">
    /// The locale to access the value for. The invariant culture to access specifically where no locale is provided.
    /// </param>
    /// <returns>
    /// The value associated with the given key, appropriate for the given locale. If there is no such entry, this
    /// returns null.
    /// </returns>
    /// <exception cref="FormatException">When setting, if the key is not a valid key.</exception>
    /// <remarks>
    /// Assigning null to this removes the value for the given locale. Note that doing this doesn't remove the value for
    /// the default locale.
    /// </remarks>
    string? this[string key, CultureInfo locale] { get; set; }

    /// <summary>
    /// Gets whether an entry with the given key exists in this category.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if any entry exists in this category with the given key.</returns>
    bool KeyExists(string key);
    
    /// <summary>
    /// Gets the value assigned to the given key.
    /// </summary>
    /// <param name="key">The key to get the value of.</param>
    /// <returns>
    /// The value assigned to the given key, appropriate for the user's current locale, or null if there is no such
    /// entry.
    /// </returns>
    string? Get(string key);

    /// <summary>
    /// Gets the value assigned to the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to get the value of.</param>
    /// <param name="locale">
    /// The two-letter code representing the locale to get the value for. An empty string to get specifically where no
    /// locale is provided.
    /// </param>
    /// <returns>
    /// The value assigned to the given key, appropriate for the given locale, or null if there is no such entry.
    /// </returns>
    /// <seealso href="https://community.kde.org/KDE_Core/ISO_Codes"/>
    string? Get(string key, string locale);

    /// <summary>
    /// Gets the value assigned to the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to get the value of.</param>
    /// <param name="locale">
    /// The two-letter code representing the locale to get the value for. The invariant culture to get specifically
    /// where no locale is provided.
    /// </param>
    /// <returns>
    /// The value assigned to the given key, appropriate for the given locale, or null if there is no such entry.
    /// </returns>
    string? Get(string key, CultureInfo locale);

    /// <summary>
    /// Gets the assignment info for the given key.
    /// </summary>
    /// <param name="key">The key to get information about the value of.</param>
    /// <returns>
    /// Information about the value assigned to the given key, appropriate for the user's current locale, or null if
    /// there is no such entry.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(string key);

    /// <summary>
    /// Gets the assignment info for the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to get information about the value of.</param>
    /// <param name="locale">
    /// The two-letter code representing the locale to get information about the value for. An empty string to get
    /// specifically where no locale is provided.
    /// </param>
    /// <returns>
    /// Information about the value assigned to the given key, appropriate for the given locale, or null if there is no
    /// such entry.
    /// </returns>
    /// <seealso href="https://community.kde.org/KDE_Core/ISO_Codes"/>
    KdeConfigEntryAssignment? GetInfo(string key, string locale);

    /// <summary>
    /// Gets the assignment info for the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to get information about the value of.</param>
    /// <param name="locale">
    /// The culture info representing the locale to get information about the value for. The invariant culture to get
    /// specifically where no locale is provided.
    /// </param>
    /// <returns>
    /// Information about the value assigned to the given key, appropriate for the given locale, or null if there is no
    /// such entry.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(string key, CultureInfo locale);

    /// <summary>
    /// Gets the keys in to which values are assigned.
    /// </summary>
    /// <returns>A collection of the keys to which values are assigned.</returns>
    /// <remarks>
    /// Keys are given if *any* localisation exists, even if none are appropriate for the user's current locale.
    /// </remarks>
    ICollection<string> GetKeys();

    /// <summary>
    /// Sets the value for the given key.
    /// </summary>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansion is enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked down.</param>
    /// <exception cref="FormatException">If the key is not a valid key.</exception>
    /// <remarks>
    /// This sets the key for the invariant locale. i.e. with no locale specified, not even the user's current locale.
    ///
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string key, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value for the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="locale">
    /// The specific locale to set a value for the key for. An empty string to set specifically for where no locale is
    /// provided.
    /// </param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansion is enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked down.</param>
    /// <exception cref="FormatException">If the key is not a valid key.</exception>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string key, string locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value for the given key for the given locale in the given category.
    /// </summary>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="locale">
    /// The specific locale to set a value for the key for. The invariant culture to set specifically for where no
    /// locale is provided.
    /// </param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansion is enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked down.</param>
    /// <exception cref="FormatException">If the key is not a valid key.</exception>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string      key,
             CultureInfo locale,
             string      value,
             bool        expansionsAreEnabled = false,
             bool        isLockedDown         = false);

    /// <summary>
    /// Sets the value for the given key.
    /// </summary>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="FormatException">If the key is not a valid key.</exception>
    /// <remarks>
    /// This sets the key for the invariant locale. i.e. with no locale specified, not even the user's current locale.
    /// </remarks>
    void Set(string key, KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the value for the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="locale">
    /// The specific locale to set a value for the key for. An empty string to set specifically for where no locale is
    /// provided.
    /// </param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="FormatException">If the key is not a valid key.</exception>
    void Set(string key, string locale, KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the value for the given key for the given locale.
    /// </summary>
    /// <param name="key">The key to set the value of.</param>
    /// <param name="locale">
    /// The specific locale to set a value for the key for. The invariant culture to set specifically for where no
    /// locale is provided.
    /// </param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="FormatException">If the key is not a valid key.</exception>
    void Set(string key, CultureInfo locale, KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the raw value for the given key. This is the value before being de-escaped or expanded.
    /// </summary>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="rawValue">The raw value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <exception cref="FormatException">If the raw value contains a newline or carriage return.</exception>
    /// <remarks>
    /// This sets the key for the invariant locale. i.e. with no locale specified, not even the user's current locale.
    ///
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void SetRaw(string key, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false);
    
    /// <summary>
    /// Sets the raw value for the given key for the given locale. This is the value before being de-escaped or
    /// expanded.
    /// </summary>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="locale">
    /// The locale to set the raw value for. An empty string to set it without providing a locale.
    /// </param>
    /// <param name="rawValue">The raw value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <exception cref="FormatException">If the raw value contains a newline or carriage return.</exception>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void SetRaw(string key, string locale, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false);
    
    /// <summary>
    /// Sets the raw value for the given key for the given locale. This is the value before being de-escaped or
    /// expanded.
    /// </summary>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="locale">
    /// The locale to set the raw value for. The invariant culture to set it without providing a locale.
    /// </param>
    /// <param name="rawValue">The raw value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <exception cref="FormatException">If the raw value contains a newline or carriage return.</exception>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void SetRaw(string key, CultureInfo locale, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false);
    
    /// <summary>
    /// Re-evaluates the shell expansions used in the values in this category.
    /// </summary>
    void ReEvaluateExpansions();
    
    /// <summary>
    /// Re-evaluates the shell expansions used in the value(s) for the specified key.
    /// </summary>
    /// <param name="key">The key to re-evaluate the shell expansions for.</param>
    void ReEvaluateExpansions(string key);
    
    /// <summary>
    /// Re-evaluates the shell expansions used in the value(s) for the specified key, specifically for the given locale.
    /// </summary>
    /// <param name="key">The key to re-evaluate the shell expansions for.</param>
    /// <param name="locale">
    /// The locale to re-evaluate values for. An empty string to re-evaluate specifically where no locale is provided.
    /// </param>
    void ReEvaluateExpansions(string key, string locale);
    
    /// <summary>
    /// Re-evaluates the shell expansions used in the value(s) for the specified key, specifically for the given locale.
    /// </summary>
    /// <param name="key">The key to re-evaluate the shell expansions for.</param>
    /// <param name="locale">
    /// The locale to re-evaluate values for. The invariant culture to re-evaluate specifically where no locale is
    /// provided.
    /// </param>
    void ReEvaluateExpansions(string key, CultureInfo locale);

    /// <summary>
    /// Removes all entries in this category.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes all values assigned to the given key.
    /// </summary>
    /// <param name="key">The key to clear.</param>
    void Clear(string key);

    /// <summary>
    /// Removes the value assigned to the given guy for the given locale.
    /// </summary>
    /// <param name="key">The key to clear.</param>
    /// <param name="locale">
    /// The locale to clear the key for. An empty string to remove specifically the values for where no locale is
    /// provided.
    /// </param>
    void Clear(string key, string locale);

    /// <summary>
    /// Removes the value assigned to the given guy for the given locale.
    /// </summary>
    /// <param name="key">The key to clear. Null or indicate the default locale.</param>
    /// <param name="locale">
    /// The locale to clear the key for. An empty string to remove specifically the values for where no locale is
    /// provided.
    /// </param>
    void Clear(string key, CultureInfo locale);

    /// <summary>
    /// Writes the contents of this category to the given writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="includeHeader">
    /// Whether to write the category's name before writer any of the rest of the contents.
    /// </param>
    /// <remarks>
    /// Where there's more than one entry, writes them in string order of the keys.
    /// <see cref="IKdeConfigEntry.WriteToTextWriter">See the entry type for how localisations are ordered.</see>
    /// </remarks>
    void WriteToTextWriter(TextWriter writer, bool includeHeader = true);
}
