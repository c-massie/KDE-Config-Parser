using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Category;
using Scot.Massie.KDEConfigParser.Utils;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Config;

/// <inheritdoc cref="IKdeConfig"/>
public class KdeConfig : IKdeConfig
{
    private readonly KdeConfigCategory _defaultCategory = new("");

    private readonly IDictionary<string, KdeConfigCategory> _namedCategories
        = new Dictionary<string, KdeConfigCategory>();

    /// <inheritdoc />
    public bool IsEmpty => _defaultCategory.IsEmpty && _namedCategories.Count == 0;

    /// <inheritdoc />
    public string? this[string? category, string key]
    {
        get => Get(category, key);

        set
        {
            if(value is null)
                ClearDefaultLocalisation(category, key);
            else
                Set(category, key, value);
        }
    }

    /// <inheritdoc />
    public string? this[string? category, string key, string locale]
    {
        get => Get(category, key, locale);

        set
        {
            if(value is null)
                Clear(category, key, locale);
            else
                Set(category, key, locale, value);
        }
    }

    /// <inheritdoc />
    public string? this[string? category, string key, CultureInfo locale]
    {
        get => Get(category, key, locale);

        set
        {
            if(value is null)
                Clear(category, key, locale);
            else
                Set(category, key, locale, value);
        }
    }

    private KdeConfigCategory? GetCategory(string? category)
    {
        return category is null ? _defaultCategory : _namedCategories.GetOrDefault(category);
    }

    private KdeConfigCategory GetOrCreateCategory(string? category)
    {
        if(category is null)
            return _defaultCategory;

        if(_namedCategories.TryGetValue(category, out var cat))
            return cat;

        return _namedCategories[category] = new KdeConfigCategory(category);
    }

    /// <inheritdoc />
    public string? Get(string? category, string key)
    {
        return GetInfo(category, key)?.Value;
    }

    /// <inheritdoc />
    public string? Get(string? category, string key, string locale)
    {
        return GetInfo(category, key, locale)?.Value;
    }

    /// <inheritdoc />
    public string? Get(string? category, string key, CultureInfo locale)
    {
        return GetInfo(category, key, locale)?.Value;
    }

    /// <inheritdoc />
    public string? GetInvariant(string? category, string key)
    {
        return GetInvariantInfo(category, key)?.Value;
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string? category, string key)
    {
        return GetCategory(category)?.GetInfo(key);
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string? category, string key, string locale)
    {
        return GetCategory(category)?.GetInfo(key, locale);
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInfo(string? category, string key, CultureInfo locale)
    {
        return GetCategory(category)?.GetInfo(key, locale);
    }

    /// <inheritdoc />
    public KdeConfigEntryAssignment? GetInvariantInfo(string? category, string key)
    {
        return GetCategory(category)?.GetInvariantInfo(key);
    }

    /// <inheritdoc />
    public bool CategoryExists(string categoryName)
    {
        return _namedCategories.ContainsKey(categoryName);
    }

    /// <inheritdoc />
    public ICollection<string> GetCategories()
    {
        return _namedCategories.Keys.ToArray();
    }

    /// <inheritdoc />
    public ICollection<string> GetKeysInCategory(string category)
    {
        return _namedCategories.GetOrDefault(category)?.GetKeys() ?? [];
    }

    /// <inheritdoc />
    public ICollection<string> GetKeysInDefaultCategory()
    {
        return _defaultCategory.GetKeys();
    }

    /// <inheritdoc />
    public void Set(string? category,
                    string  key,
                    string  value,
                    bool    expansionsAreEnabled = false,
                    bool    isLockedDown         = false)
    {
        GetOrCreateCategory(category).Set(key, value, isLockedDown, expansionsAreEnabled);
    }

    /// <inheritdoc />
    public void Set(string? category,
                    string  key,
                    string  locale,
                    string  value,
                    bool    expansionsAreEnabled = false,
                    bool    isLockedDown         = false)
    {
        GetOrCreateCategory(category).Set(key, locale, value, isLockedDown, expansionsAreEnabled);
    }

    /// <inheritdoc />
    public void Set(string?     category,
                    string      key,
                    CultureInfo locale,
                    string      value,
                    bool        expansionsAreEnabled = false,
                    bool        isLockedDown         = false)
    {
        GetOrCreateCategory(category).Set(key, locale, value, isLockedDown, expansionsAreEnabled);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _defaultCategory.Clear();
        _namedCategories.Clear();
    }

    /// <inheritdoc />
    public void Clear(string? category)
    {
        if(category is null)
        {
            _defaultCategory.Clear();
            return;
        }

        _namedCategories.Remove(category);
    }

    /// <inheritdoc />
    public void Clear(string? category, string key)
    {
        if(category is null)
        {
            _defaultCategory.Clear(key);
            return;
        }

        if(!_namedCategories.TryGetValue(category, out var cat))
            return;

        cat.Clear(key);

        if(cat.IsEmpty)
            _namedCategories.Remove(category);
    }

    /// <inheritdoc />
    public void Clear(string? category, string key, string locale)
    {
        if(category is null)
        {
            _defaultCategory.Clear(key, locale);
            return;
        }

        if(!_namedCategories.TryGetValue(category, out var cat))
            return;

        cat.Clear(key, locale);

        if(cat.IsEmpty)
            _namedCategories.Remove(category);
    }

    /// <inheritdoc />
    public void Clear(string? category, string key, CultureInfo locale)
    {
        if(category is null)
        {
            _defaultCategory.Clear(key, locale);
            return;
        }

        if(!_namedCategories.TryGetValue(category, out var cat))
            return;

        cat.Clear(key, locale);

        if(cat.IsEmpty)
            _namedCategories.Remove(category);
    }

    /// <inheritdoc />
    public void ClearDefaultLocalisation(string? category, string key)
    {
        if(category is null)
        {
            _defaultCategory.Clear(key);
            return;
        }

        if(!_namedCategories.TryGetValue(category, out var cat))
            return;

        cat.Clear(key);

        if(cat.IsEmpty)
            _namedCategories.Remove(category);
    }

    /// <inheritdoc />
    public void ReadFromTextReader(TextReader reader)
    {
        var currentCategory = _defaultCategory;

        for(string? line; (line = reader.ReadLine()) is not null;)
        {
            if(line.Trim().Length is 0)
                continue;

            if(KdeConfigTextUtils.TextIsCategoryHeader(line, out var categoryName))
            {
                currentCategory = GetOrCreateCategory(categoryName);
                continue;
            }

            ReadFromSingleLineString(currentCategory, line);
        }

        ClearEmptyCategories();
    }

    private void ClearEmptyCategories()
    {
        var categoriesToRemove = _namedCategories.Where(kv => kv.Value.IsEmpty).Select(kv => kv.Key).ToArray();

        for(int i = 0; i < categoriesToRemove.Length; i++)
            _namedCategories.Remove(categoriesToRemove[i]);
    }

    private static void ReadFromSingleLineString(KdeConfigCategory category, string source)
    {
        var (key, locales, value) = KdeConfigTextUtils.ParsePartsOfEntry(source);

        if(locales.Count == 0)
        {
            category.Set(key, value);
            return;
        }

        foreach(var locale in locales)
            category.Set(key, locale, value);
    }

    /// <inheritdoc />
    public void LoadFromString(string source)
    {
        Clear();
        using var reader = new StringReader(source);
        ReadFromTextReader(reader);
    }

    /// <inheritdoc />
    public void WriteToTextWriter(TextWriter writer)
    {
        if(IsEmpty)
        {
            writer.WriteLine();
            return;
        }

        if(!_defaultCategory.IsEmpty)
            _defaultCategory.WriteToTextWriter(writer, false);

        using var iter = _namedCategories.GetEnumerator();

        {
            if(!iter.MoveNext())
                return;

            if(!_defaultCategory.IsEmpty)
                writer.WriteLine();

            iter.Current.Value.WriteToTextWriter(writer);
        }

        while(iter.MoveNext())
        {
            writer.WriteLine();
            iter.Current.Value.WriteToTextWriter(writer);
        }
    }

    /// <inheritdoc />
    public string ToStringInFull()
    {
        var sw = new StringWriter();
        WriteToTextWriter(sw);
        return sw.ToString()[..^1]; // Remove trailing newline.
    }
}
