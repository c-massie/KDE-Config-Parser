using System.Globalization;
using System.Text.RegularExpressions;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Category;
using Scot.Massie.KDEConfigParser.Config.Selector;
using Scot.Massie.KDEConfigParser.Utils;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Config;

/// <inheritdoc cref="IKdeConfig"/>
public class KdeConfig : IKdeConfig
{
    private readonly KdeConfigCategory _defaultCategory;

    private readonly IDictionary<string, KdeConfigCategory> _namedCategories
        = new Dictionary<string, KdeConfigCategory>();

    private readonly HashSet<string> _enforcedCategories = new();

    private readonly IKdeConfigSelector _escapingDisabledSelector = new KdeConfigSelector();

    private readonly ICollection<Func<string, bool>> _categoryPreservationTests = [];

    /// <inheritdoc />
    public bool IsEmpty => _defaultCategory.IsEmpty && _namedCategories.Count == 0 && _enforcedCategories.Count == 0;

    /// <inheritdoc />
    public ICollection<string> EnforcedCategories => _enforcedCategories.ToArray();

    /// <inheritdoc />
    public bool PreservesAllCategories { get; set; }

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

    private bool CategoryShouldBePreserved(string categoryName)
    {
        if(PreservesAllCategories)
            return true;

        if(_categoryPreservationTests.Any(test => test(categoryName)))
            return true;

        return false;
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

        
        // Creating new category.
        
        if(CategoryShouldBePreserved(category))
            _enforcedCategories.Add(category);
        
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
        return _escapingDisabledSelector.MatchesEverything;
    }

    /// <inheritdoc />
    public bool GetWhetherEscapingIsDisabled(string? categoryName)
    {
        return _escapingDisabledSelector.MatchesCategory(categoryName);
    }

    /// <inheritdoc />
    public bool GetWhetherEscapingIsDisabled(string? categoryName, string key)
    {
        return _escapingDisabledSelector.MatchesKey(categoryName, key);
    }

    /// <inheritdoc />
    public bool CategoryExists(string categoryName)
    {
        return _namedCategories.ContainsKey(categoryName) || _enforcedCategories.Contains(categoryName);
    }

    /// <inheritdoc />
    public ICollection<string> GetCategories()
    {
        return _namedCategories.Keys.Concat(_enforcedCategories).Distinct().Order().ToArray();
    }

    /// <inheritdoc />
    public ICollection<string> GetKeysInCategory(string? category)
    {
        if(category is null)
            return _defaultCategory.GetKeys();
        
        return _namedCategories.GetOrDefault(category)?.GetKeys() ?? [];
    }

    /// <inheritdoc />
    public void PreserveCategoriesWhere(Func<string, bool> test)
    {
        _categoryPreservationTests.Add(test);
    }

    /// <inheritdoc />
    public void EnforceCategory(string categoryName)
    {
        _enforcedCategories.Add(categoryName);
    }

    /// <inheritdoc />
    public void EnforceCategories(IEnumerable<string> categoryNames)
    {
        foreach(var name in categoryNames)
            _enforcedCategories.Add(name);
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
    [Obsolete("Use DisableEscaping instead.")]
    public void SetWhetherEscapingIsDisabled(bool value)
    {
        if(!IsEmpty)
            throw new NotSupportedException("Setting whether escaping is disabled on an already-populated config.");

        _escapingDisabledSelector.MatchesEverything = value;
    }

    /// <inheritdoc />
    [Obsolete("Use DisableEscaping instead.")]
    public void SetWhetherEscapingIsDisabled(string? category, bool value)
    {
        if(category is null)
        {
            if(!_defaultCategory.IsEmpty)
                throw new NotSupportedException(
                    "Setting whether escaping is disabled on an already-populated category.");
        }
        else
        {
            if(CategoryExists(category))
                throw new NotSupportedException(
                    "Setting whether escaping is disabled on an already-populated category.");
        }
        
        if(value)
            _escapingDisabledSelector.AddCategory(category);
        else
            _escapingDisabledSelector.RemoveCategory(category);
    }

    /// <inheritdoc />
    [Obsolete("Use DisableEscaping instead.")]
    public void SetWhetherEscapingIsDisabled(string? category, string key, bool value)
    {
        if(KeyExists(category, key))
            throw new NotSupportedException("Setting whether escaping is disabled on an already-populated key.");
        
        if(value)
            _escapingDisabledSelector.AddKey(category, key);
        else
            _escapingDisabledSelector.RemoveKey(category, key);
    }

    /// <inheritdoc />
    public void DisableValueEscaping()
    {
        if(!IsEmpty)
            throw new NotSupportedException("Disabling value-escaping on an already-populated config.");
        
        _escapingDisabledSelector.MatchesEverything = true;
    }

    /// <inheritdoc />
    public void DisableValueEscapingForKey(string? category, string key)
    {
        if(KeyExists(category, key))
            throw new NotSupportedException("Disabling value-escaping on an already-populated key.");
        
        _escapingDisabledSelector.AddKey(category, key);
    }

    /// <inheritdoc />
    public void DisableValueEscapingForKey(string key)
    {
        if(_defaultCategory.KeyExists(key) || _namedCategories.Values.Any(x => x.KeyExists(key)))
            throw new NotSupportedException("Disabling value-escaping on an already-populated key.");
        
        _escapingDisabledSelector.AddKey(key);
    }

    /// <inheritdoc />
    public void DisableValueEscapingForCategory(string? category)
    {
        if(category is null)
        {
            if(!_defaultCategory.IsEmpty)
                throw new NotSupportedException("Disabling value-escaping on an already-populated category.");
        }
        else
        {
            if(CategoryExists(category))
                throw new NotSupportedException("Disabling value-escaping on an already-populated category.");
        }
        
        _escapingDisabledSelector.AddCategory(category);
    }

    /// <inheritdoc />
    public void DisableValueEscapingForMatchingKeys(string? category, Regex keyMatcher)
    {
        if(!IsEmpty)
            throw new NotSupportedException("Disabling value-escaping via regex on an already-populated config.");
        
        _escapingDisabledSelector.AddKeyMatcher(category, keyMatcher);
    }

    /// <inheritdoc />
    public void DisableValueEscapingForMatchingKeys(Regex keyMatcher)
    {
        if(!IsEmpty)
            throw new NotSupportedException("Disabling value-escaping via regex on an already-populated config.");
        
        _escapingDisabledSelector.AddKeyMatcher(keyMatcher);
    }

    /// <inheritdoc />
    public void DisableValueEscapingForMatchingKeysInMatchingCategories(Regex categoryMatcher, Regex keyMatcher)
    {
        if(!IsEmpty)
            throw new NotSupportedException("Disabling value-escaping via regex on an already-populated config.");
        
        _escapingDisabledSelector.AddCategoryAndKeyMatcher(categoryMatcher, keyMatcher);
    }

    /// <inheritdoc />
    public void DisableValueEscapingForMatchingCategories(Regex categoryMatcher)
    {
        if(!IsEmpty)
            throw new NotSupportedException("Disabling value-escaping via regex on an already-populated config.");
        
        _escapingDisabledSelector.AddCategoryMatcher(categoryMatcher);
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
        ClearRecords();
        ClearEnforcedCategories();
        ClearCategoryPreservations();
    }

    /// <inheritdoc />
    public void ClearRecords()
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
    public void ClearEnforcedCategories()
    {
        _enforcedCategories.Clear();
    }

    /// <inheritdoc />
    public void ClearCategoryPreservations()
    {
        PreservesAllCategories = false;
        _categoryPreservationTests.Clear();
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
        ClearRecords();
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

        using var iter = GetCategories().GetEnumerator();

        {
            if(!iter.MoveNext())
                return;

            if(!_defaultCategory.IsEmpty)
                writer.WriteLine();

            if(_namedCategories.TryGetValue(iter.Current, out var cat))
                cat.WriteToTextWriter(writer);
            else
                (new KdeConfigCategory(this, false, iter.Current)).WriteToTextWriter(writer);
        }

        while(iter.MoveNext())
        {
            writer.WriteLine();
            
            if(_namedCategories.TryGetValue(iter.Current, out var cat))
                cat.WriteToTextWriter(writer);
            else
                (new KdeConfigCategory(this, false, iter.Current)).WriteToTextWriter(writer);
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
