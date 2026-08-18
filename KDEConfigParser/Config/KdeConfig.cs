using System.Collections.Immutable;
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

    private readonly HashSet<string> _categoryNamesPreservedByRules = new();

    private readonly IKdeConfigSelector _escapingDisabledSelector = new KdeConfigSelector();

    private readonly ICollection<Func<string, bool>> _categoryPreservationTests = [];

    private bool _preservesAllCategories;
    
    /// <inheritdoc />
    public bool IsEmpty => _defaultCategory.IsEmpty && _namedCategories.Count == 0;

    /// <inheritdoc />
    public ICollection<string> EnforcedCategories => _enforcedCategories.ToImmutableArray();

    /// <inheritdoc />
    public bool PreservesAllCategories
    {
        get => _preservesAllCategories;

        set
        {
            if(value != _preservesAllCategories)
            {
                _preservesAllCategories = value;

                if(!_preservesAllCategories)
                {
                    foreach(var (catName, cat) in _namedCategories.Select(kv => (kv.Key, kv.Value)).ToArray())
                        if(!CategoryIsBeingIndividuallyPreserved(catName) && cat.IsEmpty)
                            _namedCategories.Remove(catName);
                }
            }
        }
    }

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

    private bool CategoryShouldBePreservedByRule(string categoryName)
    {
        return _categoryPreservationTests.Any(test => test(categoryName));
    }

    private bool CategoryIsBeingIndividuallyPreserved(string categoryName)
    {
        return _enforcedCategories.Contains(categoryName)
            || _categoryNamesPreservedByRules.Contains(categoryName);
    }

    private KdeConfigCategory? GetCategory(string? categoryName)
    {
        return categoryName is null ? _defaultCategory : _namedCategories.GetOrDefault(categoryName);
    }

    private KdeConfigCategory GetOrCreateCategory(string? categoryName)
    {
        if(categoryName is null)
            return _defaultCategory;

        if(_namedCategories.TryGetValue(categoryName, out var cat))
            return cat;

        if(CategoryShouldBePreservedByRule(categoryName))
            _categoryNamesPreservedByRules.Add(categoryName);
        
        return _namedCategories[categoryName] = new KdeConfigCategory(this, false, categoryName);
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
    public void PreserveCategoriesWhere(Func<string, bool> test)
    {
        _categoryPreservationTests.Add(test);

        foreach(var catName in _namedCategories.Keys)
            if(test(catName))
                _categoryNamesPreservedByRules.Add(catName);
    }

    /// <inheritdoc />
    public void EnforceCategory(string categoryName)
    {
        _enforcedCategories.Add(categoryName);

        if(!_namedCategories.ContainsKey(categoryName))
            _namedCategories[categoryName] = new KdeConfigCategory(this, false, categoryName);
    }

    /// <inheritdoc />
    public void EnforceCategories(IEnumerable<string> categoryNames)
    {
        foreach(var name in categoryNames)
            EnforceCategory(name);
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
        // TO DO: Think about whether this method should remove all content to do with value escaping. I think this
        //        should be fine, even though it's intended to only be applied when initially created? Because the
        //        config will be empty after calling this anyway.
        
        PreservesAllCategories = false;
        _enforcedCategories.Clear();
        _categoryNamesPreservedByRules.Clear();
        _categoryPreservationTests.Clear();
        _defaultCategory.Clear();
        _namedCategories.Clear();
    }

    /// <inheritdoc />
    public void ClearRecords()
    {
        _defaultCategory.Clear();
        
        if(PreservesAllCategories)
            foreach(var cat in _namedCategories.Values)
                cat.Clear();
        else if(_enforcedCategories.Count == 0 && _categoryNamesPreservedByRules.Count == 0)
            _namedCategories.Clear();
        else
        {
            var keys = _namedCategories.Keys.ToArray();

            foreach(var key in keys)
            {
                if(CategoryIsBeingIndividuallyPreserved(key))
                    _namedCategories[key].Clear();
                else
                    _namedCategories.Remove(key);
            }
        }
    }

    /// <inheritdoc />
    public void Clear(string? category)
    {
        if(category is null)
        {
            _defaultCategory.Clear();
            return;
        }

        if(CategoryIsBeingIndividuallyPreserved(category))
            _namedCategories.GetOrDefault(category)?.Clear();
        else
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
        RemoveCategoryIfNeeded(cat);
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
        RemoveCategoryIfNeeded(cat);
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
        RemoveCategoryIfNeeded(cat);
    }

    /// <inheritdoc />
    public void ClearEnforcedCategories()
    {
        if(!PreservesAllCategories)
        {
            var keysToRemove = _enforcedCategories
                              .Select(catName => (Found: _namedCategories.TryGetValue(catName, out var cat),
                                                  CatName: catName,
                                                  Cat: cat))
                              .Where(x => x.Found)
                              .Where(x => x.Cat!.IsEmpty)
                              .Where(x => !_categoryNamesPreservedByRules.Contains(x.CatName));

            foreach(var (_, catName, _) in keysToRemove)
                _namedCategories.Remove(catName);
        }

        _enforcedCategories.Clear();
    }

    /// <inheritdoc />
    public void ClearCategoryPreservations()
    {
        if(!PreservesAllCategories)
        {
            var keysToRemove = _categoryNamesPreservedByRules
                              .Select(catName => (Found: _namedCategories.TryGetValue(catName, out var cat),
                                                  CatName: catName,
                                                  Cat: cat))
                              .Where(x => x.Found)
                              .Where(x => x.Cat!.IsEmpty)
                              .Where(x => !_enforcedCategories.Contains(x.CatName));
            
            foreach(var (_, catName, _) in keysToRemove)
                _namedCategories.Remove(catName);
        }
        
        _categoryPreservationTests.Clear();
        _categoryNamesPreservedByRules.Clear();
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
        if(PreservesAllCategories)
            return;

        var keysToRemove = _namedCategories.Where(kv => kv.Value.IsEmpty)
                                           .Where(kv => !CategoryIsBeingIndividuallyPreserved(kv.Key))
                                           .Select(kv => kv.Key)
                                           .ToArray();

        foreach(var key in keysToRemove)
            _namedCategories.Remove(key);
    }

    private void RemoveCategoryIfNeeded(IKdeConfigCategory category)
    {
        if(!PreservesAllCategories && category.IsEmpty && !CategoryIsBeingIndividuallyPreserved(category.Name))
            _namedCategories.Remove(category.Name);
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
