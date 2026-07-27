using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;

namespace Scot.Massie.KDEConfigParser.Entry;

/// <summary>
/// A key and the value (or values, if multiple locales are specified) in a KDE configuration.
/// </summary>
public interface IKdeConfigEntry
{
    /// <summary>
    /// The key for this entry.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// The assignments for each locale.
    /// </summary>
    IDictionary<string, KdeConfigEntryAssignment> Localisations { get; }

    /// <summary>
    /// Whether this entry has no value/values assigned in. (Across any locales)
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Gets the value of this entry.
    /// </summary>
    /// <returns>The value assigned to this entry, appropriate for the user's current locale.</returns>
    string? Get();

    /// <summary>
    /// Gets the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to get the value for. An empty string to indicate specifically where no locale is provided.
    /// </param>
    /// <returns>The value assigned to this entry, appropriate for the specified locale.</returns>
    string? Get(string locale);

    /// <summary>
    /// Gets the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to get the value for. The invariant culture to indicate specifically where no locale is provided.
    /// </param>
    /// <returns>The value assigned to this entry, appropriate for the specified locale.</returns>
    string? Get(CultureInfo locale);

    /// <summary>
    /// Gets information about the value of this entry.
    /// </summary>
    /// <returns>
    /// Information about the value assigned to this entry, appropriate for the user's current locale.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo();

    /// <summary>
    /// Gets information about the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to get the value for. An empty string to indicate specifically where no locale is provided.
    /// </param>
    /// <returns>
    /// Information about the value assigned to this entry, appropriate for the specified locale.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(string locale);

    /// <summary>
    /// Gets information about the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to get the value for. The invariant culture to indicate specifically where no locale is provided.
    /// </param>
    /// <returns>
    /// Information about the value assigned to this entry, appropriate for the specified locale.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(CultureInfo locale);

    /// <summary>
    /// Sets the value of this entry.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <remarks>
    /// This sets the key for the invariant locale. i.e. with no locale specified, not even the user's current locale.
    ///
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value of this entry, for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to set the value for. An empty string to set it without providing a locale.
    /// </param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(string locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value of this entry, for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to set the value for. The invariant culture to set it without providing a locale.
    /// </param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(CultureInfo locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the raw value of this entry. This is the value before being de-escaped or expanded.
    /// </summary>
    /// <param name="rawValue">The raw value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <exception cref="FormatException">If the raw value contains a newline or carriage return.</exception>
    /// <remarks>
    /// This sets the key for the invariant locale. i.e. with no locale specified, not even the user's current locale.
    ///
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void SetRaw(string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the raw value for this entry, for the given locale. This is the value before being de-escaped or expanded.
    /// </summary>
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
    void SetRaw(string locale, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the raw value for this entry, for the given locale. This is the value before being de-escaped or expanded.
    /// </summary>
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
    void SetRaw(CultureInfo locale, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value of this entry.
    /// </summary>
    /// <param name="value">The value to set.</param>
    void Set(KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the value of this entry, for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to set the value for. An empty string to set it without providing a locale.
    /// </param>
    /// <param name="value">The value to set.</param>
    void Set(string locale, KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the value of this entry, for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to set the value for. The invariant culture to set it without providing a locale.
    /// </param>
    /// <param name="value">The value to set.</param>
    void Set(CultureInfo locale, KdeConfigEntryAssignment value);

    /// <summary>
    /// Re-evaluates the shell expansions used in the value(s) of this entry.
    /// </summary>
    void ReEvaluateExpansions();

    /// <summary>
    /// Re-evaluates the shell expansions used in the value of this entry with the specified locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to re-evaluate the values for. An empty string to re-evaluate specifically where no locale is
    /// provided.
    /// </param>
    void ReEvaluateExpansions(string locale);

    /// <summary>
    /// Re-evaluates the shell expansions used in the value of this entry with the specified locale. An empty string to
    /// re-evaluate specifically where no locale is provided.
    /// </summary>
    void ReEvaluateExpansions(CultureInfo locale);

    /// <summary>
    /// Removes the value of this entry.
    /// </summary>
    /// <remarks>Removes all values, for all locales.</remarks>
    void Clear();

    /// <summary>
    /// Removes the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to remove the value for. An empty string to clear specifically where no locale is provided. (i.e. the
    /// default value)
    /// </param>
    void Clear(string locale);

    /// <summary>
    /// Removes the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">
    /// The locale to remove the value for.  The invariant culture to clear specifically where no locale is provided.
    /// (i.e. the default value)
    /// </param>
    void Clear(CultureInfo locale);

    /// <summary>
    /// Writes this entry to the given writer.
    /// </summary>
    /// <param name="writer">The text writer to write to.</param>
    /// <remarks>
    /// Where there are different values for different locales, this writes the default first, then all other locales in
    /// string order.
    /// </remarks>
    void WriteToTextWriter(TextWriter writer);
}
