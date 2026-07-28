namespace Scot.Massie.KDEConfigParser.Utils;

[TestSubject(typeof(KdeConfigTextUtils))]
public class KdeConfigTextUtilsTest(KdeConfigTextUtilsTest.Fixture fixture)
    : IClassFixture<KdeConfigTextUtilsTest.Fixture>
{
    public class Fixture
    {
        public IDictionary<string, string> DummyShellVars = new Dictionary<string, string>()
        {
            ["BELGIUM"]     = "Brussels",
            ["ENGLAND"]     = "London",
            ["FRANCE"]      = "Paris",
            ["NETHERLANDS"] = "Amsterdam",
            ["SCOTLAND"]    = "Edinburgh",
            ["WALES"]       = "Cardiff",
        };
    }

    private static IEnumerable<Range> ParseCommaSeparatedRanges(string s)
    {
        return SplitByCommasNotInBrackets(s).Where(x => x.Length != 0).Select(ParseRange);
    }

    private static Range ParseRange(string s)
    {
        s = s[1..^1];
        var parts  = s.Split(',', 2, StringSplitOptions.TrimEntries);
        var bottom = ParseIndex(parts[0]);
        var top    = ParseIndex(parts[1]);
        return new Range(bottom, top);
    }

    private static Index ParseIndex(string s)
    {
        var isFromEnd = s.StartsWith('^');

        if(isFromEnd)
            s = s[1..].TrimStart();

        var num = int.Parse(s);
        return new Index(num, isFromEnd);
    }

    private static IEnumerable<string> SplitByCommasNotInBrackets(string s)
    {
        var currentStart = 0;
        
        for(int i = 0; i < s.Length; i++)
        {
            switch(s[i])
            {
                case '(':
                {
                    i = GetPositionOfMatchingCloseBracket(s, i);
                    break;
                }

                case ',':
                {
                    yield return s[currentStart..i].Trim();
                    currentStart = i + 1;
                    break;
                }
            }
        }

        yield return s[currentStart..].Trim();
    }

    private static int GetPositionOfMatchingCloseBracket(string s, int openBracketPosition)
    {
        int bracketDepth = 1;

        for(int i = openBracketPosition + 1; i < s.Length; i++)
        {
            switch(s[i])
            {
                case '(':
                {
                    bracketDepth++;
                    break;
                }

                case ')':
                {
                    if(--bracketDepth == 0)
                        return i;

                    break;
                }
            }
        }
        
        throw new Exception("There is no matching close bracket for that opening bracket.\n"
                          + $"s: {s}\nopenBracketPosition: {openBracketPosition}");
    }
    
    
    [Theory]
    [InlineData("",              "",              Label = "Empty string")]
    [InlineData("Nothing doing", "Nothing doing", Label = "Nothing to escape")]

    [InlineData("a\\sb",  "a b",  Label = "Space")]
    [InlineData("a\\tb",  "a\tb", Label = "Tab")]
    [InlineData("a\\rb",  "a\rb", Label = "Carriage return")]
    [InlineData("a\\nb",  "a\nb", Label = "Linefeed")]
    [InlineData("a\\\\b", "a\\b", Label = "Literal backslash")]

    [InlineData("\\s",     " ",     Label = "Nothing but an escape")]
    [InlineData("\\s a",   "  a",   Label = "Escape at start")]
    [InlineData("a \\s",   "a  ",   Label = "Escape at end")]
    [InlineData("a \\s b", "a   b", Label = "Escape in middle")]

    [InlineData(@"a\\\\b", @"a\\b", Label = "Consecutive escaped backslashes")]
    [InlineData(@"\\a",    @"\a",   Label = "Escaped backslash at start")]
    [InlineData(@"a\\",    @"a\",   Label = "Escaped backslash at end")]
    [InlineData(@"a\",     @"a",    Label = "Lone backslash at end")]
    public void EscapeText(string input, string expectedResult)
    {
        KdeConfigTextUtils.DeEscapeText(input).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("a\\xb", Label = "x")]
    [InlineData("a\\ b", Label = "space")]
    [InlineData("a\\$b", Label = "dollar sign")]
    public void EscapeText_InvalidEscapeSequence(string input)
    {
        Action call = () => KdeConfigTextUtils.DeEscapeText(input);
        call.Should().Throw<FormatException>();
    }


    [Theory]
    [InlineData("",     "", Label = "Empty string")]
    [InlineData("Text", "", Label = "No expansion ranges")]

    [InlineData("$VAR",                   "(0, 4)",          Label = "Unbracketed expansion")]
    [InlineData("$VAR text",              "(0, 4)",          Label = "Unbracketed expansion at start")]
    [InlineData("this is $VAR some text", "(8, 12)",         Label = "Unbracketed expansion in middle")]
    [InlineData("text $VAR",              "(5, 9)",          Label = "Unbracketed expansion at end")]
    [InlineData("$VAR $ANOTHER",          "(0, 4), (5, 13)", Label = "Multiple unbracketed expansions")]
    [InlineData("$VAR$ANOTHER",           "(0, 4), (4, 12)", Label = "Multiple unbracketed expansions consecutive")]

    [InlineData("${VAR}",                   "(0, 6)",          Label = "Bracketed expansion")]
    [InlineData("${VAR} text",              "(0, 6)",          Label = "Bracketed expansion at start")]
    [InlineData("this is ${VAR} some text", "(8, 14)",         Label = "Bracketed expansion in middle")]
    [InlineData("text ${VAR}",              "(5, 11)",         Label = "Bracketed expansion at end")]
    [InlineData("text ${VAR",               "(5, 10)",         Label = "Bracketed expansion at end with no closer")]
    [InlineData("${VAR} ${ANOTHER}",        "(0, 6), (7, 17)", Label = "Multiple bracketed expansions")]
    [InlineData("${VAR}${ANOTHER}",         "(0, 6), (6, 16)", Label = "Multiple bracketed expansions consecutive")]
    [InlineData("${VAR}text",               "(0, 6)",          Label = "Text immediately after br. expansion")]
    public void GetShellExpansionRanges(string input, string resultRangesString)
    {
        var expectedRanges = ParseCommaSeparatedRanges(resultRangesString);

        var result = KdeConfigTextUtils.GetShellExpansionRanges(input);

        result.Should().BeEquivalentTo(expectedRanges, cfg => cfg.WithStrictOrdering());
    }

    [Theory]
    [InlineData("",     "",     Label = "Empty string")]
    [InlineData("Text", "Text", Label = "No expansions")]

    [InlineData("$FRANCE",              "Paris",              Label = "Unbracketed expansion")]
    [InlineData("$FRANCE text",         "Paris text",         Label = "Unbracketed expansion at start")]
    [InlineData("This is $FRANCE text", "This is Paris text", Label = "Unbracketed expansion in middle")]
    [InlineData("text $FRANCE",         "text Paris",         Label = "Unbracketed expansion at end")]
    [InlineData("$FRANCE $BELGIUM",     "Paris Brussels",     Label = "Multiple unbracketed expansions")]
    [InlineData("$FRANCE$BELGIUM",      "ParisBrussels",      Label = "Multiple unbracketed expansion consecutive")]

    [InlineData("${FRANCE}",              "Paris",              Label = "Bracketed expansion")]
    [InlineData("${FRANCE} text",         "Paris text",         Label = "Bracketed expansion at start")]
    [InlineData("This is ${FRANCE} text", "This is Paris text", Label = "Bracketed expansion in middle")]
    [InlineData("text ${FRANCE}",         "text Paris",         Label = "Bracketed expansion at end")]
    [InlineData("${FRANCE} ${BELGIUM}",   "Paris Brussels",     Label = "Multiple br. expansions")]
    [InlineData("${FRANCE}${BELGIUM}",    "ParisBrussels",      Label = "Multiple br. expansions consecutive")]
    [InlineData("${FRANCE}${BELGIUM",     "ParisBrussels",      Label = "Multiple br. expansions cons. no closer")]
    [InlineData("${FRANCE}text",          "Paristext",          Label = "Text immediately after br. expansion")]
    public void ShellExpand(string input, string expectedResult)
    {
        KdeConfigTextUtils.ShellExpand(input, sv => fixture.DummyShellVars[sv]).Should().Be(expectedResult);
    }
    
    [Theory]
    [InlineData("key=value",   "key", false, false, "", "value", Label = "Normal case")]
    [InlineData(" key =value", "key", false, false, "", "value", Label = "Whitespace around key")]
    [InlineData("key= value ", "key", false, false, "", "value", Label = "Whitespace around value")]

    [InlineData("key[$i]=value",      "key", true,  false, "", "value", Label = "Locked-in tag")]
    [InlineData("key[$e]=value",      "key", false, true,  "", "value", Label = "Expansions tag")]
    [InlineData("key[$i][$e]=value",  "key", true,  true,  "", "value", Label = "Locked-in then expansions tags")]
    [InlineData("key[$e][$i]=value",  "key", true,  true,  "", "value", Label = "Expansions then locked-in tag")]
    [InlineData("key[$ei]=value",     "key", true,  true,  "", "value", Label = "Combo expansions/locked-in tag")]
    [InlineData("key[$ie]=value",     "key", true,  true,  "", "value", Label = "Combo locked-in/expansions tag")]
    [InlineData("key[$i][$i]=value",  "key", true,  false, "", "value", Label = "Repeated locked-in tag")]
    [InlineData("key[$e][$e]=value",  "key", false, true,  "", "value", Label = "Repeated expansions tag")]
    [InlineData("key[$ie][$e]=value", "key", true,  true,  "", "value", Label = "Expansions tag repeated after combo")]
    [InlineData("key[$ie][$i]=value", "key", true,  true,  "", "value", Label = "Locked-in tag repeated after combo")]

    [InlineData("key[fr]=value",     "key", false, false, "fr", "value", Label = "Locale tag")]
    [InlineData("key[fr][en]=value", "key", false, false, "fr, en", "value", Label = "Multiple locale tags")]
    [InlineData("key[en][en]=value", "key", false, false, "en", "value", Label = "Repeated locale tag")]

    [InlineData("key[en][$i]=value", "key", true, false, "en", "value", Label = "Locale then locked-in tags")]
    [InlineData("key[$i][en]=value", "key", true, false, "en", "value", Label = "Locked-in then locale tags")]

    [InlineData("key[en]ignored=value", "key", false, false, "en", "value", Label = "Ignored text after locale tag")]
    [InlineData("key[$i]ignored=value", "key", true,  false, "", "value", Label = "Ignored text after locked-in tag")]
    
    [InlineData("\\s \\s=value", "   ", false, false, "", "value", Label = "Whitespace-only key.")]
    public void ParsePartsOfEntry(string input,
                          string key,
                          bool   isLockedDown,
                          bool   expansionsAreEnabled,
                          string localeTagsListString,
                          string value)
    {
        var localeTags = localeTagsListString.Length == 0
                             ? []
                             : localeTagsListString.Split(',', StringSplitOptions.TrimEntries);

        var (resultKey, resultLocales, resultLocalisation) = KdeConfigTextUtils.ParsePartsOfEntry(input, _ => true);

        resultKey.Should().Be(key);
        resultLocalisation.RawValue.Should().Be(value);
        resultLocalisation.IsLockedDown.Should().Be(isLockedDown);
        resultLocalisation.ShellExpansionIsEnabled.Should().Be(expansionsAreEnabled);
        resultLocales.Should().BeEquivalentTo(localeTags);
    }
    
    [Theory]
    [InlineData("key[$x]=value", Label = "Invalid behaviour tag")]
    [InlineData("key\\x=value",  Label = "Invalid escape sequence")]

    [InlineData("=value",         Label = "Empty key")]
    [InlineData("  =value",       Label = "Empty key with whitespace")]
    [InlineData("[en]=value",     Label = "Empty key with tags")]
    [InlineData("[en]text=value", Label = "Empty key with tags and ignored text")]
    // Note: kreadconfig6 itself doesn't complain about empty keys if they have tags. I believe this is a bug.

    public void ParsePartsOfEntry_Invalid(string input)
    {
        var call = () =>
        {
            var _ = KdeConfigTextUtils.ParsePartsOfEntry(input, _ => true);
        };

        call.Should().Throw<FormatException>();
    }
}
