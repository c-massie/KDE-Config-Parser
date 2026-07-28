using System.Globalization;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Category;
using Scot.Massie.KDEConfigParser.Utils;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Config;

/// <inheritdoc cref="IKdeConfig"/>
public class KdeConfig : IKdeConfig
{
    private readonly KdeConfigCategory _defaultCategory;

    private readonly IDictionary<string, KdeConfigCategory> _namedCategories
        = new Dictionary<string, KdeConfigCategory>();

    private bool _escapingIsDisabled = false;

    private bool _escapingIsDisabledForDefaultCategory = false;

    private readonly ISet<string> _categoriesEscapingIsDisabledFor = new HashSet<string>();

    private readonly ISet<string> _keysInDefaultCategoryEscapingIsDisabledFor = new HashSet<string>();

    private readonly IDictionary<string, ISet<string>> _keysEscapingIsDisabledFor
        = new Dictionary<string, ISet<string>>();

    /// <inheritdoc />
    public bool IsEmpty => _defaultCategory.IsEmpty && _namedCategories.Count == 0;

    /// <inheritdoc />
    public string? this[string? category, string key]
    {
        get => Get(category, key);

        set
        {
            if(value is null)
                Clear(category, key, "");
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

    /// <summary>
    /// Creates a new KDE config.
    /// </summary>
    public KdeConfig()
    {
        _defaultCategory = new(this, true, "");
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

        return _namedCategories[category] = new KdeConfigCategory(this, false, category);
    }

    /// <inheritdoc />
    public bool KeyExists(string? category, string key)
    {
        var cat = GetCategory(category);

        if(cat is null)
            return false;

        return cat.KeyExists(key);
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
    public bool GetWhetherEscapingIsDisabled()
    {
        return _escapingIsDisabled;
    }

    /// <inheritdoc />
    public bool GetWhetherEscapingIsDisabled(string? categoryName)
    {
        return _escapingIsDisabled
            || categoryName is null
                   ? _escapingIsDisabledForDefaultCategory
                   : _categoriesEscapingIsDisabledFor.Contains(categoryName);
    }

    /// <inheritdoc />
    public bool GetWhetherEscapingIsDisabled(string? categoryName, string key)
    {
        if(GetWhetherEscapingIsDisabled(categoryName))
            return true;

        var keySet = categoryName is null
                         ? _keysInDefaultCategoryEscapingIsDisabledFor
                         : _keysEscapingIsDisabledFor.GetOrDefault(categoryName);

        return keySet?.Contains(key) ?? false;
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
    public ICollection<string> GetKeysInCategory(string? category)
    {
        if(category is null)
            return _defaultCategory.GetKeys();
        
        return _namedCategories.GetOrDefault(category)?.GetKeys() ?? [];
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
    public void SetRaw(string? category,
                       string  key,
                       string  rawValue,
                       bool    expansionsAreEnabled = false,
                       bool    isLockedDown         = false)
    {
        GetOrCreateCategory(category).SetRaw(key, rawValue, expansionsAreEnabled, isLockedDown);
    }

    /// <inheritdoc />
    public void SetRaw(string? category,
                       string  key,
                       string  locale,
                       string  rawValue,
                       bool    expansionsAreEnabled = false,
                       bool    isLockedDown         = false)
    {
        GetOrCreateCategory(category).SetRaw(key, locale, rawValue, expansionsAreEnabled, isLockedDown);
    }

    /// <inheritdoc />
    public void SetRaw(string?     category,
                       string      key,
                       CultureInfo locale,
                       string      rawValue,
                       bool        expansionsAreEnabled = false,
                       bool        isLockedDown         = false)
    {
        GetOrCreateCategory(category).SetRaw(key, locale, rawValue, expansionsAreEnabled, isLockedDown);
    }

    /// <inheritdoc />
    public void SetWhetherEscapingIsDisabled(bool value)
    {
        if(!IsEmpty)
            throw new NotSupportedException("Setting whether escaping is disabled on an already-populated config.");
        
        _escapingIsDisabled = value;
    }

    /// <inheritdoc />
    public void SetWhetherEscapingIsDisabled(string? category, bool value)
    {
        if(category is null)
        {
            if(!_defaultCategory.IsEmpty)
                throw new NotSupportedException(
                    "Setting whether escaping is disabled on an already-populated category.");
            
            _escapingIsDisabledForDefaultCategory = value;
            return;
        }

        if(CategoryExists(category))
            throw new NotSupportedException("Setting whether escaping is disabled on an already-populated category.");
        
        if(value)
            _categoriesEscapingIsDisabledFor.Add(category);
        else
            _categoriesEscapingIsDisabledFor.Remove(category);
    }

    /// <inheritdoc />
    public void SetWhetherEscapingIsDisabled(string? category, string key, bool value)
    {
        if(KeyExists(category, key))
            throw new NotSupportedException("Setting whether escaping is disabled on an already-populated key.");
        
        ISet<string>? keySet;
        
        if(category is null)
        {
            keySet = _keysInDefaultCategoryEscapingIsDisabledFor;
        }
        else
        {
            if(!_keysEscapingIsDisabledFor.TryGetValue(category, out keySet))
                keySet = _keysEscapingIsDisabledFor[category] = new HashSet<string>();
        }
        
        if(value)
            keySet.Add(key);
        else
            keySet.Remove(key);
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions()
    {
        _defaultCategory.ReEvaluateExpansions();

        foreach(var (_, category) in _namedCategories)
            category.ReEvaluateExpansions();
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string? category)
    {
        GetCategory(category)?.ReEvaluateExpansions();
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string? category, string key)
    {
        GetCategory(category)?.ReEvaluateExpansions(key);
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string? category, string key, string locale)
    {
        GetCategory(category)?.ReEvaluateExpansions(key, locale);
    }

    /// <inheritdoc />
    public void ReEvaluateExpansions(string? category, string key, CultureInfo locale)
    {
        GetCategory(category)?.ReEvaluateExpansions(key, locale);
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

    private void ReadFromSingleLineString(KdeConfigCategory category, string source)
    {
        var categoryIsDefault = ReferenceEquals(category, _defaultCategory);

        var (key, locales, value)
            = KdeConfigTextUtils.ParsePartsOfEntry(
                source, key => !GetWhetherEscapingIsDisabled(categoryIsDefault ? null : category.Name, key));

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
