using System.Text.RegularExpressions;
using Scot.Massie.KDEConfigParser.Utils;

namespace Scot.Massie.KDEConfigParser.Config.Selector;

/// <inheritdoc/>
public sealed class KdeConfigSelector : IKdeConfigSelector
{
    private bool _matchesDefaultCategory = false;

    private readonly ISet<string> _namedCategoriesMatched = new HashSet<string>();

    private readonly ISet<string> _keysMatchedInDefaultCategory = new HashSet<string>();
    
    private readonly IDictionary<string, ISet<string>> _keysMatchedInCategories
        = new Dictionary<string, ISet<string>>();

    private readonly ISet<string> _keysMatchedInAnyCategory = new HashSet<string>();

    private readonly ICollection<Regex> _regexesForCategories = new List<Regex>();

    private readonly ICollection<Regex> _regexesForKeysInDefaultCategory = new List<Regex>();

    private readonly ICollection<(Regex categoryMatcher, Regex keyMatcher)> _regexesForKeysInRegexedCategories
        = new List<(Regex categoryMatcher, Regex keyMatcher)>();

    private readonly IDictionary<string, ICollection<Regex>> _regexesForKeysInNamedCategories
        = new Dictionary<string, ICollection<Regex>>();
    
    private readonly ICollection<Regex> _regexesForKeysInAnyCategory = new List<Regex>();

    /// <inheritdoc/>
    public bool MatchesEverything { get; set; } = false;

    /// <inheritdoc/>
    public bool MatchesCategory(string? category)
    {
        if(MatchesEverything)
            return true;

        if(category is null)
            return _matchesDefaultCategory;

        if(_namedCategoriesMatched.Contains(category))
            return true;

        if(_regexesForCategories.Any(regex => regex.IsExactMatch(category)))
            return true;

        return false;
    }

    /// <inheritdoc/>
    public bool MatchesKey(string? category, string key)
    {
        if(MatchesCategory(category))
            return true;

        if(_keysMatchedInAnyCategory.Contains(key))
            return true;

        if(category is null)
        {
            if(_keysMatchedInDefaultCategory.Contains(key))
                return true;
        }
        else
        {
            if(_keysMatchedInCategories.TryGetValue(category, out var keysetForNamedCategory))
                if(keysetForNamedCategory.Contains(key))
                    return true;
        }

        if(_regexesForKeysInAnyCategory.Any(r => r.IsExactMatch(key)))
            return true;

        if(category is null)
        {
            if(_regexesForKeysInDefaultCategory.Any(r => r.IsExactMatch(key)))
                return true;
        }
        else
        {
            if(_regexesForKeysInNamedCategories.TryGetValue(category, out var namedCategoryRegexes))
                if(namedCategoryRegexes.Any(r => r.IsExactMatch(key)))
                    return true;

            if(_regexesForKeysInRegexedCategories.Any(rr => rr.categoryMatcher.IsExactMatch(category)
                                                         && rr.keyMatcher.IsExactMatch(key)))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public void AddCategory(string? category)
    {
        if(category is null)
        {
            _matchesDefaultCategory = true;
            return;
        }

        _namedCategoriesMatched.Add(category);
    }

    /// <inheritdoc/>
    public void AddKey(string? category, string key)
    {
        if(category is null)
        {
            _keysMatchedInDefaultCategory.Add(key);
            return;
        }

        if(!_keysMatchedInCategories.TryGetValue(category, out var keySet))
            _keysMatchedInCategories[category] = keySet = new HashSet<string>();

        keySet.Add(key);
    }

    /// <inheritdoc/>
    public void AddKey(string key)
    {
        _keysMatchedInAnyCategory.Add(key);
    }

    /// <inheritdoc/>
    public void AddCategoryMatcher(Regex categoryMatcher)
    {
        _regexesForCategories.Add(categoryMatcher);
    }

    /// <inheritdoc/>
    public void AddKeyMatcher(Regex keyMatcher)
    {
        _regexesForKeysInAnyCategory.Add(keyMatcher);
    }

    /// <inheritdoc/>
    public void AddKeyMatcher(string? category, Regex keyMatcher)
    {
        if(category is null)
        {
            if(!_regexesForKeysInDefaultCategory.Contains(keyMatcher))
                _regexesForKeysInDefaultCategory.Add(keyMatcher);
            
            return;
        }

        if(!_regexesForKeysInNamedCategories.TryGetValue(category, out var keySet))
            _regexesForKeysInNamedCategories[category] = keySet = new List<Regex>();

        if(!keySet.Contains(keyMatcher))
            keySet.Add(keyMatcher);
    }

    /// <inheritdoc/>
    public void AddCategoryAndKeyMatcher(Regex categoryMatcher, Regex keyMatcher)
    {
        _regexesForKeysInRegexedCategories.Add((categoryMatcher, keyMatcher));
    }

    /// <inheritdoc/>
    public void RemoveCategory(string? category)
    {
        if(category is null)
        {
            _matchesDefaultCategory = false;
            return;
        }

        _namedCategoriesMatched.Remove(category);
    }

    /// <inheritdoc/>
    public void RemoveKey(string? category, string key)
    {
        if(category is null)
        {
            _keysMatchedInDefaultCategory.Remove(key);
            return;
        }

        if(!_keysMatchedInCategories.TryGetValue(category, out var keySet))
            return;

        keySet.Remove(key);

        if(keySet.Count == 0)
            _keysMatchedInCategories.Remove(category);
    }

    /// <inheritdoc/>
    public void RemoveKey(string key)
    {
        _keysMatchedInAnyCategory.Remove(key);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        MatchesEverything       = false;
        _matchesDefaultCategory = false;
        _namedCategoriesMatched.Clear();
        _keysMatchedInDefaultCategory.Clear();
        _keysMatchedInCategories.Clear();
        _keysMatchedInAnyCategory.Clear();
        _regexesForCategories.Clear();
        _regexesForKeysInDefaultCategory.Clear();
        _regexesForKeysInRegexedCategories.Clear();
        _regexesForKeysInNamedCategories.Clear();
        _regexesForKeysInAnyCategory.Clear();
    }
}
