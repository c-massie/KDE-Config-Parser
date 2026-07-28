using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Category;
using Scot.Massie.KDEConfigParser.Config;
using Scot.Massie.KDEConfigParser.Utils;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Entry;

/// <inheritdoc cref="IKdeConfigEntry"/>
public class KdeConfigEntry
    : IKdeConfigEntry
{
    private readonly IKdeConfig _parentConfig;

    private readonly IKdeConfigCategory _parentCategory;
    
    private KdeConfigEntryAssignment? _defaultLocalisation = null;

    private readonly IDictionary<string, KdeConfigEntryAssignment> _localisations;

    private bool EscapingIsEnabled
        => !_parentConfig.GetWhetherEscapingIsDisabled(_parentCategory.IsDefaultCategory ? null : _parentCategory.Name,
                                                       Key);

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public IDictionary<string, KdeConfigEntryAssignment> Localisations { get; }

    /// <inheritdoc />
    public bool IsEmpty => _defaultLocalisation is null && _localisations.Count == 0;

    /// <summary>
    /// Creates a new entry.
    /// </summary>
    /// <param name="parentConfig">The configuration this entry is for.</param>
    /// <param name="parentCategory">The configuration category this entry is for.</param>
    /// <param name="key">The key of the entry.</param>
    /// <exception cref="FormatException">If the key is invalid.</exception>
    public KdeConfigEntry(IKdeConfig parentConfig, IKdeConfigCategory parentCategory, string key)
    {
        if(key.ContainsAnyOf('[', ']', '='))
            throw new FormatException("Keys cannot contain square brackets or equals signs.");

        _parentConfig   = parentConfig;
        _parentCategory = parentCategory;
        Key             = key;
        _localisations  = new Dictionary<string, KdeConfigEntryAssignment>();
        Localisations   = _localisations.AsReadOnly();
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
    public KdeConfigEntryAssignment? GetInfo()
    {
        return _localisations.GetOrDefault(GetCurrentCultureCode()) ?? _defaultLocalisation;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string locale)
    {
        if(locale is "")
            return _defaultLocalisation;

        return _localisations.GetOrDefault(locale) ?? _defaultLocalisation;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(CultureInfo locale)
    {
        return GetInfo(GetCultureCode(locale));
    }

    private string GetCurrentCultureCode()
    {
        return GetCultureCode(CultureInfo.CurrentUICulture);
    }

    /// <inheritdoc />
    public void Set(string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(new KdeConfigEntryAssignment(value, EscapingIsEnabled, expansionsAreEnabled, isLockedDown));
    }

    /// <inheritdoc />
    public void Set(string locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(locale, new KdeConfigEntryAssignment(value, EscapingIsEnabled, expansionsAreEnabled, isLockedDown));
    }

    /// <inheritdoc />
    public void Set(CultureInfo locale, string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(locale, new KdeConfigEntryAssignment(value, EscapingIsEnabled, expansionsAreEnabled, isLockedDown));
    }

    /// <inheritdoc/>
    public void SetRaw(string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(KdeConfigEntryAssignment.NewFromEscapedValue(rawValue,
                                                         EscapingIsEnabled,
                                                         expansionsAreEnabled,
                                                         isLockedDown));
    }

    /// <inheritdoc/>
    public void SetRaw(string locale, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        Set(locale, KdeConfigEntryAssignment.NewFromEscapedValue(rawValue,
                                                                 EscapingIsEnabled,
                                                                 expansionsAreEnabled,
                                                                 isLockedDown));
    }

    /// <inheritdoc/>
    public void SetRaw(CultureInfo locale,
                       string      rawValue,
                       bool        expansionsAreEnabled = false,
                       bool        isLockedDown         = false)
    {
        Set(locale, KdeConfigEntryAssignment.NewFromEscapedValue(rawValue,
                                                                 EscapingIsEnabled,
                                                                 expansionsAreEnabled,
                                                                 isLockedDown));
    }

    /// <inheritdoc />
    public void Set(KdeConfigEntryAssignment value)
    {
        _defaultLocalisation = value;
    }

    /// <inheritdoc />
    public void Set(string locale, KdeConfigEntryAssignment value)
    {
        if(locale is "")
        {
            Set(value);
            return;
        }

        _localisations[locale] = value;
    }

    /// <inheritdoc />
    public void Set(CultureInfo locale, KdeConfigEntryAssignment value)
    {
        Set(GetCultureCode(locale), value);
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string locale)
    {
        if(locale is "")
        {
            if(_defaultLocalisation?.ShellExpansionIsEnabled ?? false)
                _defaultLocalisation = _defaultLocalisation?.ReExpanded();

            return;
        }

        if(_localisations.TryGetValue(locale, out var assignment) && assignment.ShellExpansionIsEnabled)
            _localisations[locale] = assignment.ReExpanded();
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(CultureInfo locale)
    {
        ReEvaluateExpansions(GetCultureCode(locale));
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
        if(locale is "" or null)
        {
            _defaultLocalisation = null;
            return;
        }

        _localisations.Remove(locale);
    }

    /// <inheritdoc />
    public void Clear(CultureInfo locale)
    {
        Clear(GetCultureCode(locale));
    }

    private string GetCultureCode(CultureInfo locale)
    {
        if(Equals(locale, CultureInfo.InvariantCulture))
            return "";

        return locale.TwoLetterISOLanguageName;
    }

    /// <inheritdoc />
    public void WriteToTextWriter(TextWriter writer)
    {
        var keyDeEscaped = KdeConfigTextUtils.EscapeText(Key);

        if(_defaultLocalisation is not null)
        {
            var tags          = BehaviourTagsToString(_defaultLocalisation);
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
