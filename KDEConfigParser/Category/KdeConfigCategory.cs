using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Config;
using Scot.Massie.KDEConfigParser.Entry;
using Scot.Massie.KDEConfigParser.Utils;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Category;

/// <inheritdoc cref="IKdeConfigCategory"/>
public class KdeConfigCategory
    : IKdeConfigCategory
{
    private readonly IKdeConfig _parentConfig;

    private readonly IDictionary<string, KdeConfigEntry> _entries = new Dictionary<string, KdeConfigEntry>();

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsEmpty => _entries.Count == 0;

    public bool IsDefaultCategory { get; }

    /// <inheritdoc />
    public string? this[string key]
    {
        get => Get(key);

        set
        {
            if(value is null)
                Clear(key, "");
            else
                Set(key, value);
        }
    }

    /// <inheritdoc />
    public string? this[string key, string locale]
    {
        get => Get(key, locale);

        set
        {
            if(value is null)
                Clear(key, locale);
            else
                Set(key, locale, value);
        }
    }

    /// <inheritdoc />
    public string? this[string key, CultureInfo locale]
    {
        get => Get(key, locale);

        set
        {
            if(value is null)
                Clear(key, locale);
            else
                Set(key, locale, value);
        }
    }

    /// <summary>
    /// Creates a new config category.
    /// </summary>
    /// <param name="parentConfig">The configuration this category is for.</param>
    /// <param name="isDefaultCategory">Whether this is the default category for the configuration this is for.</param>
    /// <param name="name">The name of the category.</param>
    /// <exception cref="FormatException">If the given name is not a valid category name.</exception>
    public KdeConfigCategory(IKdeConfig parentConfig, bool isDefaultCategory, string name)
    {
        if(name.Contains(']'))
            throw new FormatException("Category names may not contain closing square brackets.");

        _parentConfig     = parentConfig;
        IsDefaultCategory = isDefaultCategory;
        Name              = name;
    }

    /// <inheritdoc />
    public bool KeyExists(string key)
    {
        return _entries.ContainsKey(key);
    }

    /// <inheritdoc />
    public string? Get(string key)
    {
        return GetInfo(key)?.Value;
    }

    /// <inheritdoc />
    public string? Get(string key, string locale)
    {
        return GetInfo(key, locale)?.Value;
    }

    /// <inheritdoc />
    public string? Get(string key, CultureInfo locale)
    {
        return GetInfo(key, locale)?.Value;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string key)
    {
        return _entries.GetOrDefault(key)?.GetInfo();
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string key, string locale)
    {
        return _entries.GetOrDefault(key)?.GetInfo(locale);
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string key, CultureInfo locale)
    {
        return _entries.GetOrDefault(key)?.GetInfo(locale);
    }

    /// <inheritdoc />
    public ICollection<string> GetKeys()
    {
        return _entries.Keys.ToArray();
    }

    private KdeConfigEntry GetOrCreateEntry(string key)
    {
        if(!_entries.TryGetValue(key, out var entry))
            _entries[key] = entry = new KdeConfigEntry(_parentConfig, this, key);

        return entry;
    }

    /// <inheritdoc />
    public void Set(string key, string value, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        GetOrCreateEntry(key).Set(value, isLockedDown, expansionsAreEnabled);
    }

    /// <inheritdoc />
    public void Set(string key,
                    string locale,
                    string value,
                    bool   expansionsAreEnabled = false,
                    bool   isLockedDown         = false)
    {
        GetOrCreateEntry(key).Set(locale, value, isLockedDown, expansionsAreEnabled);
    }

    /// <inheritdoc />
    public void Set(string      key,
                    CultureInfo locale,
                    string      value,
                    bool        expansionsAreEnabled = false,
                    bool        isLockedDown         = false)
    {
        GetOrCreateEntry(key).Set(locale, value, isLockedDown, expansionsAreEnabled);
    }

    /// <inheritdoc />
    public void Set(string key, KdeConfigEntryAssignment value)
    {
        GetOrCreateEntry(key).Set(value);
    }

    /// <inheritdoc />
    public void Set(string key, string locale, KdeConfigEntryAssignment value)
    {
        GetOrCreateEntry(key).Set(locale, value);
    }

    /// <inheritdoc />
    public void Set(string key, CultureInfo locale, KdeConfigEntryAssignment value)
    {
        GetOrCreateEntry(key).Set(locale, value);
    }

    /// <inheritdoc />
    public void SetRaw(string key, string rawValue, bool expansionsAreEnabled = false, bool isLockedDown = false)
    {
        GetOrCreateEntry(key).SetRaw(rawValue, expansionsAreEnabled, isLockedDown);
    }

    /// <inheritdoc />
    public void SetRaw(string key,
                       string locale,
                       string rawValue,
                       bool expansionsAreEnabled = false,
                       bool isLockedDown = false)
    {
        GetOrCreateEntry(key).SetRaw(locale, rawValue, expansionsAreEnabled, isLockedDown);
    }

    /// <inheritdoc />
    public void SetRaw(string      key,
                       CultureInfo locale,
                       string      rawValue,
                       bool        expansionsAreEnabled = false,
                       bool        isLockedDown         = false)
    {
        GetOrCreateEntry(key).SetRaw(locale, rawValue, expansionsAreEnabled, isLockedDown);
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions()
    {
        foreach(var (_, entry) in _entries)
            entry.ReEvaluateExpansions();
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string key)
    {
        _entries.GetOrDefault(key)?.ReEvaluateExpansions();
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string key, string locale)
    {
        _entries.GetOrDefault(key)?.ReEvaluateExpansions(locale);
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string key, CultureInfo locale)
    {
        _entries.GetOrDefault(key)?.ReEvaluateExpansions(locale);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _entries.Clear();
    }

    /// <inheritdoc />
    public void Clear(string key)
    {
        _entries.Remove(key);
    }

    /// <inheritdoc />
    public void Clear(string key, string locale)
    {
        if(!_entries.TryGetValue(key, out var entry))
            return;

        entry.Clear(locale);

        if(entry.IsEmpty)
            _entries.Remove(key);
    }

    /// <inheritdoc />
    public void Clear(string key, CultureInfo locale)
    {
        if(!_entries.TryGetValue(key, out var entry))
            return;

        entry.Clear(locale);

        if(entry.IsEmpty)
            _entries.Remove(key);
    }

    /// <inheritdoc />
    public void WriteToTextWriter(TextWriter writer, bool includeHeader = true)
    {
        if(includeHeader)
            writer.WriteLine($"[{KdeConfigTextUtils.EscapeText(Name, false)}]");

        foreach(var entry in _entries.OrderBy(kv => kv.Key).Select(kv => kv.Value))
            entry.WriteToTextWriter(writer);
    }
}
