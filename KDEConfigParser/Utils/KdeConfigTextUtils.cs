using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using Scot.Massie.KDEConfigParser.Assignment;
using Scot.Massie.KDEConfigParser.Utils.Collections;

namespace Scot.Massie.KDEConfigParser.Utils;

/// <summary>
/// Utility methods for working with text in the context of converting KDE configs to and from their text
/// representations.
/// </summary>
public static class KdeConfigTextUtils
{
    /// <summary>
    /// Produces a copy of the given string where escape codes according to the KDE config format are replaced with the
    /// characters they're escape codes for.
    /// </summary>
    /// <param name="unescapedText">The string to be de-escaped.</param>
    /// <returns>
    /// A copy of the given string, where escape codes are replaced with the characters they represent.
    /// </returns>
    /// <exception cref="FormatException">If the given string contains any invalid escape sequences.</exception>
    [Pure]
    public static string DeEscapeText(string unescapedText)
    {
        if(!unescapedText.Contains('\\'))
            return unescapedText;

        var sb           = new StringBuilder();
        var max          = unescapedText.Length - 2;
        var currentStart = 0;
        
        for(int i = 0; i <= max; i++)
        {
            if(unescapedText[i] is not '\\')
                continue;

            sb.Append(unescapedText[currentStart..i]);
            var escapeCodeChar = unescapedText[i + 1];

            var replacement = escapeCodeChar switch
            {
                's' => ' ',
                't' => '\t',
                'r' => '\r',
                'n' => '\n',
                '\\' => '\\',
                _ => throw new FormatException($"Invalid escape sequence: \\{escapeCodeChar}")
            };

            sb.Append(replacement);
            i++;
            currentStart = i + 1;
        }
        
        var lastCharIsUnescapedBackslash = (unescapedText[^1] is '\\' && currentStart != unescapedText.Length);
        sb.Append(unescapedText[lastCharIsUnescapedBackslash ? (currentStart..^1) : (currentStart..)]);
        return sb.ToString();
    }

    /// <summary>
    /// Produces a copy of the given string where special characters are replaced with escape codes, as specified by the
    /// KDE configuration format.
    /// </summary>
    /// <param name="escapedText">The string to be escaped.</param>
    /// <param name="escapeStartAndEndSpaces">Whether to escape the first and/or last character if it's a space.</param>
    /// <returns></returns>
    /// <remarks>Spaces do not need to be escaped, unless possibly at the start or end.</remarks>
    [Pure]
    public static string EscapeText(string escapedText, bool escapeStartAndEndSpaces = true)
    {
        var result = escapedText.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");

        if(escapeStartAndEndSpaces)
        {
            if(result.StartsWith(' '))
                result = "\\s" + result[1..];

            if(result.EndsWith(' '))
                result = result[..^1] + "\\s";
        }

        return result;
    }
    
    /// <summary>
    /// Gets the ranges within the given string of sections that can be read as shell expansions.
    /// </summary>
    /// <param name="unexpandedVal">The string to look for shell expansions in.</param>
    /// <returns>An enumeration of ranges of the given string that can be read as shell expansions.</returns>
    [Pure]
    public static IEnumerable<Range> GetShellExpansionRanges(string unexpandedVal)
    {
        int GetEndPosExclWithCurlyBrackets(int dollarSignPos)
        {
            for(int i = dollarSignPos + 2; i < unexpandedVal.Length; i++)
                if(unexpandedVal[i] is '}')
                    return i + 1;

            return unexpandedVal.Length;
        }

        int GetEndPosExclWithNoCurlyBrackets(int dollarSignPos)
        {
            for(int i = dollarSignPos + 1; i < unexpandedVal.Length; i++)
            {
                var iChar = unexpandedVal[i];

                if(!char.IsLetterOrDigit(iChar) && iChar is not '_')
                    return i;
            }

            return unexpandedVal.Length;
        }

        int GetEndPosExcl(int dollarSignPos)
        {
            return unexpandedVal[dollarSignPos + 1] is '{'
                       ? GetEndPosExclWithCurlyBrackets(dollarSignPos)
                       : GetEndPosExclWithNoCurlyBrackets(dollarSignPos);
        }
        
        
        var max = unexpandedVal.Length - 2;
        
        for(int i = 0; i <= max; i++)
        {
            if(unexpandedVal[i] is not '$')
                continue;

            var endPos = GetEndPosExcl(i);
            yield return new Range(i, endPos);
            i = endPos - 1;
        }
    }

    /// <summary>
    /// Converts a shell expansion into the name of the shell variable it references.
    /// </summary>
    /// <param name="expansionText">The shell expansion text.</param>
    /// <returns>The name of the shell variable that the shell expansion refers to.</returns>
    [Pure]
    public static string? ExpansionToShellVarName(string expansionText)
    {
        var countToTrimStart = expansionText.StartsWith("${") ? 2 : 1;
        var countToTrimEnd   = (countToTrimStart == 2 && expansionText.EndsWith('}')) ? 1 : 0;
        var varName          = expansionText[countToTrimStart..^countToTrimEnd];
        return varName.All(c => char.IsLetterOrDigit(c) || c is '_') ? varName : null;
    }

    /// <summary>
    /// Gets the value of the shell variable of the given name.
    /// </summary>
    /// <param name="varName">The name of the shell variable to get.</param>
    /// <returns>The result of querying the given variable name in bash.</returns>
    [Pure]
    public static string GetShellVar(string varName)
    {
        // See: https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output

        var proc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                ArgumentList = { "-c", "echo $" + varName },
                UseShellExecute = false,
                RedirectStandardOutput = true,
            },
        };

        proc.Start();
        var output = proc.StandardOutput.ReadToEnd();
        return output;
    }

    /// <summary>
    /// Produces a copy of the given string where shell expansions are replaced with the contents of the shell variables
    /// they represent.
    /// </summary>
    /// <param name="unexpandedVal">The text possibly containing shell expansions.</param>
    /// <returns>
    /// A copy of the given string where shell expansions are replaced with the contents of the shell variables they're
    /// for.
    /// </returns>
    [Pure]
    public static string ShellExpand(string unexpandedVal)
    {
        return ShellExpand(unexpandedVal, GetShellVar);
    }
    
    /// <summary>
    /// Produces a copy of the given string where shell expansions are replaced with the contents of the shell variables
    /// they represent.
    /// </summary>
    /// <param name="unexpandedVal">The text possibly containing shell expansions.</param>
    /// <param name="shellVarGetter">The method to get shell vars.</param>
    /// <returns>
    /// A copy of the given string where shell expansions are replaced with the contents of the shell variables they're
    /// for.
    /// </returns>
    /// <remarks>This overload is primarily to allow for unit-testing without relying on external shell vars.</remarks>
    [Pure]
    public static string ShellExpand(string unexpandedVal, Func<string, string> shellVarGetter)
    {
        var sb          = new StringBuilder();
        int startOfPart = 0;

        foreach(var shellExpansionRange in GetShellExpansionRanges(unexpandedVal))
        {
            sb.Append(unexpandedVal[startOfPart..shellExpansionRange.Start]);
            startOfPart = shellExpansionRange.End.GetOffset(unexpandedVal.Length);
            var expansion = unexpandedVal[shellExpansionRange];
            var varName   = ExpansionToShellVarName(expansion);

            if(varName is null)
                continue;

            var val = shellVarGetter(varName);
            sb.Append(val);
        }

        sb.Append(unexpandedVal[startOfPart..]);
        return sb.ToString();
    }
    
    /// <summary>
    /// Parses a given key as it appears in the configuration format, producing the key name, behaviours, and locales.
    /// </summary>
    /// <param name="unparsedKey">The key as it appears in the configuration format.</param>
    /// <returns>
    /// A tuple containing the key name, whether the value is locked down, whether expansions are enabled, and the
    /// locales read.
    /// </returns>
    /// <exception cref="FormatException">If the unparsed key is not a valid key.</exception>
    [Pure]
    public static (string Key, bool IsLockedDown, bool ExpansionsAreEnabled, IList<string> LocalisationTags)
        ParsePartsOfEntryKey(string unparsedKey)
    {
        var sqOpeners = unparsedKey.IndicesOf('[').ToArray();
        var sqClosers = unparsedKey.IndicesOf(']').ToArray();

        if(sqOpeners.Length != sqClosers.Length)
            throw new FormatException("Not all square brackets are paired.");

        for(int i = 0; i < sqOpeners.Length; i++)
            if(sqClosers[i] < sqOpeners[i])
                throw new FormatException("Closing square bracket appears before matching opening square bracket.");

        for(int i = 0; i < sqOpeners.Length - 1; i++)
            if(sqOpeners[i + 1] < sqClosers[i])
                throw new FormatException("Opening square brackets appears between previous set of square brackets.");


        var firstOpenSqBracketPos = unparsedKey.IndexOf('[');

        var key = (firstOpenSqBracketPos < 0 ? unparsedKey : unparsedKey[..firstOpenSqBracketPos]).Trim();
        key = KdeConfigTextUtils.DeEscapeText(key);

        if(key is "")
            throw new FormatException("Empty key.");


        var unparsedTags = new string[sqOpeners.Length];

        for(int i = 0; i < unparsedTags.Length; i++)
            unparsedTags[i] = unparsedKey[(sqOpeners[i] + 1)..sqClosers[i]].Trim();

        var behaviourTags    = new HashSet<char>(2);
        var localisationTags = new List<string>(2);

        for(int i = 0; i < unparsedTags.Length; i++)
        {
            var unparsedTag = unparsedTags[i];

            if(unparsedTag.StartsWith('$'))
            {
                foreach(var c in unparsedTag[1..].Where(c => !char.IsWhiteSpace(c)))
                    behaviourTags.Add(c);
            }
            else
            {
                localisationTags.Remove(unparsedTag);
                localisationTags.Add(unparsedTag);
            }
        }

        if(behaviourTags.Any(c => c is not 'e' and not 'i'))
            throw new FormatException("Unrecognised behaviour tag.");

        var isLockedDown         = behaviourTags.Contains('i');
        var expansionsAreEnabled = behaviourTags.Contains('e');


        return (key, isLockedDown, expansionsAreEnabled, localisationTags);
    }
    
    /// <summary>
    /// Parses a given entry as it appears in the configuration format, producing the key, value, and locales.
    /// </summary>
    /// <param name="unparsedEntry">The entry as it appears in the configuration format.</param>
    /// <returns>A tuple containing the name of the key, a list of the locale read, and the value.</returns>
    /// <param name="keySupportsEscapingTest">
    /// Test to determine whether the value for a given key should have escaping enabled.
    /// See: <see cref="IKdeConfigEntryAssignment.EscapingIsEnabled"/></param>
    /// <exception cref="FormatException">If the string could not be parsed as an entry.</exception>
    [Pure]
    public static (string Key, IList<string> Locales, KdeConfigEntryAssignment Value)
        ParsePartsOfEntry(string unparsedEntry, Func<string, bool> keySupportsEscapingTest)
    {
        var equalsPosition = unparsedEntry.IndexOf('=');

        if(equalsPosition < 0)
            throw new FormatException($"No equals sign, not a key-value pair.\n{unparsedEntry}");

        var unparsedKey   = unparsedEntry[..equalsPosition].Trim();
        var unparsedValue = unparsedEntry[(equalsPosition + 1)..].Trim();

        var (key, isLockedDown, expansionsAreEnabled, locales) = ParsePartsOfEntryKey(unparsedKey);
        var escapingIsEnabledInValue = keySupportsEscapingTest(key);
        
        var value = KdeConfigEntryAssignment.NewFromEscapedValue(unparsedValue,
                                                                 escapingIsEnabledInValue,
                                                                 expansionsAreEnabled,
                                                                 isLockedDown);
        
        return (key, locales, value);
    }

    /// <summary>
    /// Tests whether the given string is valid as a category header.
    /// </summary>
    /// <param name="text">The string to test.</param>
    /// <param name="categoryName">The name of the category the given string would be the header for.</param>
    /// <returns>
    /// True if the given text is valid as a category header according to the configuration format. i.e. it looks
    /// `[like this]`. Otherwise, false.
    /// </returns>
    [Pure]
    public static bool TextIsCategoryHeader(string text, [NotNullWhen(true)] out string? categoryName)
    {
        categoryName = null;
        text         = text.Trim();

        if(!text.StartsWith('['))
            return false;

        var closerIndex = text.IndexOf(']');

        if(closerIndex < 0)
            return false;

        var rawName = text[1..closerIndex];
        categoryName = DeEscapeText(rawName);
        return true;
    }
}
