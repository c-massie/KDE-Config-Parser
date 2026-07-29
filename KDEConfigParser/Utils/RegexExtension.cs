using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Scot.Massie.KDEConfigParser.Utils;

public static class RegexExtension
{
    [Pure]
    public static bool IsExactMatch(this Regex regex, string input)
    {
        var match = regex.Match(input);
        return match.Success && match.Length == input.Length;
    }
}
