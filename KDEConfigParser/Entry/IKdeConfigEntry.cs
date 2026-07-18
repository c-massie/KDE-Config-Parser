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
    /// <param name="locale">The locale to get the value for.</param>
    /// <returns>The value assigned to this entry, appropriate for the specified locale.</returns>
    string? Get(string locale);

    /// <summary>
    /// Gets the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">The locale to get the value for.</param>
    /// <returns>The value assigned to this entry, appropriate for the specified locale.</returns>
    string? Get(CultureInfo locale);

    /// <summary>
    /// Gets the value of this entry for the default locale.
    /// </summary>
    /// <returns>
    /// The value assigned to this entry, for the default locale. i.e. specifically where no locale is specified.
    /// </returns>
    string? GetInvariant();

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
    /// <returns>
    /// Information about the value assigned to this entry, appropriate for the specified locale.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(string locale);

    /// <summary>
    /// Gets information about the value of this entry for the given locale.
    /// </summary>
    /// <returns>
    /// Information about the value assigned to this entry, appropriate for the specified locale.
    /// </returns>
    KdeConfigEntryAssignment? GetInfo(CultureInfo locale);

    /// <summary>
    /// Gets information about the value of this entry for the default locale.
    /// </summary>
    /// <returns>
    /// Information about the value assigned to this entry, specifically where no locale is specified.
    /// </returns>
    KdeConfigEntryAssignment? GetInvariantInfo();

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
    /// <param name="locale">The locale to set the value for.</param>
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
    /// <param name="locale">The locale to set the value for.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expansionsAreEnabled">Whether shell expansions are enabled.</param>
    /// <param name="isLockedDown">Whether the value is locked-down.</param>
    /// <remarks>
    /// For more information on shell expansion and being "locked down", see the userbase page on the file format.
    /// </remarks>
    void Set(CultureInfo locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false);

    /// <summary>
    /// Sets the value of this entry.
    /// </summary>
    /// <param name="value">The value to set.</param>
    void Set(KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the value of this entry, for the given locale.
    /// </summary>
    /// <param name="locale">The locale to set the value for.</param>
    /// <param name="value">The value to set.</param>
    void Set(string locale, KdeConfigEntryAssignment value);

    /// <summary>
    /// Sets the value of this entry, for the given locale.
    /// </summary>
    /// <param name="locale">The locale to set the value for.</param>
    /// <param name="value">The value to set.</param>
    void Set(CultureInfo locale, KdeConfigEntryAssignment value);

    /// <summary>
    /// Removes the value of this entry.
    /// </summary>
    /// <remarks>Removes all values, for all locales.</remarks>
    void Clear();

    /// <summary>
    /// Removes the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">The locale to remove the value for.</param>
    void Clear(string locale);

    /// <summary>
    /// Removes the value of this entry for the given locale.
    /// </summary>
    /// <param name="locale">The locale to remove the value for.</param>
    void Clear(CultureInfo locale);

    /// <summary>
    /// Removes the value of this entry for the default locale.
    /// </summary>
    void ClearDefaultLocalisation();

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
