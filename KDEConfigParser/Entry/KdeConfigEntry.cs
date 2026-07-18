using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Utils;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Entry;

/// <inheritdoc cref="IKdeConfigEntry"/>
public class KdeConfigEntry
    : IKdeConfigEntry
{
    private KdeConfigEntryAssignment? _defaultLocalisation = null;

    private readonly IDictionary<string, KdeConfigEntryAssignment> _localisations;

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public IDictionary<string, KdeConfigEntryAssignment> Localisations { get; }

    /// <inheritdoc />
    public bool IsEmpty => _defaultLocalisation is null && _localisations.Count == 0;

    /// <summary>
    /// Creates a new entry.
    /// </summary>
    /// <param name="key">The key of the entry.</param>
    /// <exception cref="FormatException">If the key is invalid.</exception>
    public KdeConfigEntry(string key)
    {
        if(key.ContainsAnyOf('[', ']', '='))
            throw new FormatException("Keys cannot contain square brackets or equals signs.");
        
        Key            = key;
        _localisations = new Dictionary<string, KdeConfigEntryAssignment>();
        Localisations  = _localisations.AsReadOnly();
    }

    /// <inheritdoc />
    public string? Get()
    {
        return GetInfo()?.Value;
    }

    /// <inheritdoc />
    public string? Get(string locale)
    {
        return GetInfo(locale)?.Value;
    }

    /// <inheritdoc />
    public string? Get(CultureInfo locale)
    {
        return GetInfo(locale)?.Value;
    }

    /// <inheritdoc />
    public string? GetInvariant()
    {
        return GetInvariantInfo()?.Value;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo()
    {
        return _localisations.GetOrDefault(GetCurrentCultureCode()) ?? _defaultLocalisation;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string locale)
    {
        return locale is "iv"
                   ? _defaultLocalisation
                   : _localisations.GetOrDefault(locale) ?? _defaultLocalisation;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(CultureInfo locale)
    {
        return GetInfo(locale.TwoLetterISOLanguageName);
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInvariantInfo()
    {
        return _defaultLocalisation;
    }
    
    private string GetCurrentCultureCode()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    }

    /// <inheritdoc />
    public void Set(string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(new KdeConfigEntryAssignment(value, expansionsAreEnabled, isLockedDown));
    }

    /// <inheritdoc />
    public void Set(string locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(locale, new KdeConfigEntryAssignment(value, expansionsAreEnabled, isLockedDown));
    }

    /// <inheritdoc />
    public void Set(CultureInfo locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(locale, new KdeConfigEntryAssignment(value, expansionsAreEnabled, isLockedDown));
    }

    /// <inheritdoc />
    public void Set(KdeConfigEntryAssignment value)
    {
        _defaultLocalisation = value;
    }

    /// <inheritdoc />
    public void Set(string locale, KdeConfigEntryAssignment value)
    {
        if(locale is "iv")
        {
            Set(value);
            return;
        }
        
        _localisations[locale] = value;
    }

    /// <inheritdoc />
    public void Set(CultureInfo locale, KdeConfigEntryAssignment value)
    {
        Set(locale.TwoLetterISOLanguageName, value);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _defaultLocalisation = null;
        _localisations.Clear();
    }

    /// <inheritdoc />
    public void Clear(string locale)
    {
        if(locale is "iv")
        {
            ClearDefaultLocalisation();
            return;
        }

        _localisations.Remove(locale);
    }

    /// <inheritdoc />
    public void Clear(CultureInfo locale)
    {
        Clear(locale.TwoLetterISOLanguageName);
    }

    /// <inheritdoc />
    public void ClearDefaultLocalisation()
    {
        _defaultLocalisation = null;
    }

    /// <inheritdoc />
    public void WriteToTextWriter(TextWriter writer)
    {
        var keyDeEscaped = KdeConfigTextUtils.EscapeText(Key);
        
        if(_defaultLocalisation is not null)
        {
            var tags = BehaviourTagsToString(_defaultLocalisation);
            var defaultAsText = $"{keyDeEscaped}{tags}={_defaultLocalisation.RawValue}";
            writer.WriteLine(defaultAsText);
        }

        foreach(var (k, v) in _localisations.OrderBy(kv => kv.Key))
        {
            var tags   = BehaviourTagsToString(v);
            var asText = $"{keyDeEscaped}[{k}]{tags}={v.RawValue}";
            writer.WriteLine(asText);
        }
    }

    private static string BehaviourTagsToString(KdeConfigEntryAssignment localisation)
    {
        return (localisation.ShellExpansionIsEnabled, localisation.IsLockedDown) switch
        {
            (true, true)  => "[$ei]",
            (true, false) => "[$e]",
            (false, true) => "[$i]",
            _             => "",
        };
    }
}
