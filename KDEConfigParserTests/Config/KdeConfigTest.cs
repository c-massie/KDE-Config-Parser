namespace Scot.Massie.KDEConfigParser.Config;

[TestSubject(typeof(KdeConfig))]
public class KdeConfigTest
{
    public class Loading
    {
        public class Parsing
        {
            [Fact]
            public void SimpleCase()
            {
                var source = """
                             [first]
                             key=value
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key"]);
                cfg.Get("first", "key").Should().Be("value");
            }

            [Fact]
            public void MultipleEntriesInCategory()
            {
                var source = """
                             [first]
                             key1=value1
                             key2=value2
                             key3=value3
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key1", "key2", "key3"]);
                cfg.Get("first", "key1").Should().Be("value1");
                cfg.Get("first", "key2").Should().Be("value2");
                cfg.Get("first", "key3").Should().Be("value3");
            }

            [Fact]
            public void EmptyCategory()
            {
                var source = """
                             [first]

                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.IsEmpty.Should().BeTrue();
            }

            [Fact]
            public void MultipleCategories()
            {
                var source = """
                             [first]
                             key1=value1

                             [second]
                             key2=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first", "second"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key1"]);
                cfg.GetKeysInCategory("second").Should().BeEquivalentTo(["key2"]);
                cfg.Get("first",  "key1").Should().Be("value1");
                cfg.Get("second", "key2").Should().Be("value2");
            }

            [Fact]
            public void RepeatedCategory()
            {
                var source = """
                             [first]
                             key1=value1

                             [first]
                             key2=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key1", "key2"]);
                cfg.Get("first", "key1").Should().Be("value1");
                cfg.Get("first", "key2").Should().Be("value2");
            }

            [Fact]
            public void SameKeyInDifferentCategories()
            {
                var source = """
                             [first]
                             key=value1

                             [second]
                             key=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first", "second"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key"]);
                cfg.GetKeysInCategory("second").Should().BeEquivalentTo(["key"]);
                cfg.Get("first",  "key").Should().Be("value1");
                cfg.Get("second", "key").Should().Be("value2");
            }

            [Fact]
            public void KeyOverwritten()
            {
                var source = """
                             [first]
                             key=value1
                             key=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key"]);
                cfg.Get("first", "key").Should().Be("value2");
            }

            [Fact]
            public void DefaultFollowedByLocalised()
            {
                var source = """
                             [first]
                             key=value1
                             key[xx]=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key"]);
                cfg.Get("first", "key", "").Should().Be("value1");
                cfg.Get("first", "key", "xx").Should().Be("value2");
            }

            [Fact]
            public void LocalisedFollowedByDefault()
            {
                var source = """
                             [first]
                             key[xx]=value1
                             key=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key"]);
                cfg.Get("first", "key", "xx").Should().Be("value1");
                cfg.Get("first", "key", "").Should().Be("value2");
            }

            [Fact]
            public void EntriesInDefaultCategory()
            {
                var source = """
                             key1=value1
                             key2=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEmpty();
                cfg.IsEmpty.Should().BeFalse();
                cfg.GetKeysInCategory(null).Should().BeEquivalentTo(["key1", "key2"]);
                cfg.Get(null, "key1").Should().Be("value1");
                cfg.Get(null, "key2").Should().Be("value2");
            }

            [Fact]
            public void EntriesInDefaultAndNamedCategories()
            {
                var source = """
                             key1=value1

                             [second]
                             key2=value2
                             """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["second"]);
                cfg.GetKeysInCategory(null).Should().BeEquivalentTo(["key1"]);
                cfg.GetKeysInCategory("second").Should().BeEquivalentTo(["key2"]);
                cfg.Get(null,     "key1").Should().Be("value1");
                cfg.Get("second", "key2").Should().Be("value2");
            }

            [Fact]
            public void EmptyCategoriesArePreserved()
            {
                var source = """
                             [first]
                             [second]
                             key=value
                             
                             [third]
                             """;

                var cfg = new KdeConfig();
                cfg.PreservesAllCategories = true;
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["first", "second", "third"]);
                cfg.GetKeysInCategory("first").Should().BeEmpty();
                cfg.GetKeysInCategory("second").Should().BeEquivalentTo(["key"]);
                cfg.Get("second", "key").Should().Be("value");
                cfg.GetKeysInCategory("third").Should().BeEmpty();
            }

            [Fact]
            public void SpecificEmptyCategoriesArePreserved()
            {
                var source = """
                             [first]
                             [second]
                             [third]
                             key=value
                             [fourth*]
                             [fifth]
                             [sixth~]
                             [seventh*]
                             """;
                
                var cfg = new KdeConfig();
                cfg.PreserveCategoriesWhere(c => c.EndsWith("*"));
                cfg.PreserveCategoriesWhere(c => c.EndsWith("~"));
                cfg.LoadFromString(source);

                cfg.GetCategories().Should().BeEquivalentTo(["third", "fourth*", "sixth~", "seventh*"]);
                cfg.GetKeysInCategory("third").Should().BeEquivalentTo(["key"]);
                cfg.Get("third", "key").Should().Be("value");
                cfg.GetKeysInCategory("fourth*").Should().BeEmpty();
                cfg.GetKeysInCategory("sixth~").Should().BeEmpty();
                cfg.GetKeysInCategory("seventh*").Should().BeEmpty();
                
            }

            [Theory]
            [InlineData("",         Label = "Completely empty")]
            [InlineData("       ",  Label = "Spaces")]
            [InlineData("\n\n\n\n", Label = "Blank lines")]
            public void Empty(string source)
            {
                var cfg = new KdeConfig();
                cfg.LoadFromString(source);
                cfg.IsEmpty.Should().BeTrue();
            }

            [Theory]
            [InlineData("[first]",           "first",           Label = "Simple case")]
            [InlineData("[category name]",   "category name",   Label = "Multiple words")]
            [InlineData("[category   name]", "category   name", Label = "More than one space")]
            [InlineData("[first ]",          "first ",          Label = "Trailing space in name")]
            [InlineData("[ first]",          " first",          Label = "Leading space in name")]
            [InlineData("[]",                "",                Label = "Empty")]
            [InlineData("[   ]",             "   ",             Label = "Only spaces")]
            [InlineData("[[]",               "[",               Label = "Only")]

            [InlineData("[]]",         "",      Label = "")]
            [InlineData("[first]]",    "first", Label = "")]
            [InlineData("[first]name", "first", Label = "")]
            [InlineData("   [first]",  "first", Label = "")]
            [InlineData("[first]   ",  "first", Label = "")]

            [InlineData("[\\s]",  " ",  Label = "")]
            [InlineData("[\\n]",  "\n", Label = "")]
            [InlineData("[\\r]",  "\r", Label = "")]
            [InlineData("[\\t]",  "\t", Label = "")]
            [InlineData("[\\\\]", "\\", Label = "")]
            public void CategoryName(string categoryHeader, string expectedResult)
            {
                var source = $"""
                              {categoryHeader}
                              key=value
                              """;

                var cfg = new KdeConfig();
                cfg.LoadFromString(source);
                var cats = cfg.GetCategories();
                cats.Should().BeEquivalentTo([expectedResult]);
            }


            [Theory]

            [InlineData("""


                        [first]
                        key=value
                        """,
                        Label = "Leading blank lines"
            )]

            [InlineData("""
                        [first]
                        key=value


                        """,
                        Label = "Trailing blank lines"
            )]

            [InlineData("""
                        [first]


                        key=value
                        """,
                        Label = "Blank lines between header and entries"
            )]
            public void BlankLines(string source)
            {
                var cfg = new KdeConfig();
                cfg.LoadFromString(source);
                cfg.GetCategories().Should().BeEquivalentTo(["first"]);
                cfg.GetKeysInCategory("first").Should().BeEquivalentTo(["key"]);
                cfg.Get("first", "key").Should().Be("value");
            }
        }

        [Fact]
        public void LoadingClearsExisting()
        {
            var source1 = """
                          key1=value1
                          
                          [first]
                          key2=value2
                          """;

            var source2 = """
                          key3=value3
                          
                          [second]
                          key4=value4
                          """;

            var cfg = new KdeConfig();
            cfg.LoadFromString(source1);
            cfg.LoadFromString(source2);

            cfg.GetCategories().Should().BeEquivalentTo(["second"]);
            cfg.GetKeysInCategory(null).Should().BeEquivalentTo(["key3"]);
            cfg.GetKeysInCategory("second").Should().BeEquivalentTo(["key4"]);
            cfg.Get(null,     "key3").Should().Be("value3");
            cfg.Get("second", "key4").Should().Be("value4");
        }
    }

    public class Saving
    {
        public class Format
        {
            private ITestOutputHelper output;

            public Format(ITestOutputHelper output)
            {
                this.output = output;
            }

            [Fact]
            public void Nothing()
            {
                var expected = """
                               
                               
                               """;

                var cfg    = new KdeConfig();
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();
                result.Should().Be(expected);
            }

            [Fact]
            public void DefaultCategoryEntry()
            {
                var expected = """
                               key=value
                               
                               """;

                var cfg = new KdeConfig();
                cfg.Set(null, "key", "value");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }

            [Fact]
            public void DefaultCategoryEntries()
            {
                var expected = """
                               key1=value1
                               key2=value2
                               key3=value3
                               
                               """;

                var cfg = new KdeConfig();
                cfg.Set(null, "key1", "value1");
                cfg.Set(null, "key2", "value2");
                cfg.Set(null, "key3", "value3");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }

            [Fact]
            public void NamedCategoryEntry()
            {
                var expected = """
                               [first]
                               key=value
                               
                               """;

                var cfg = new KdeConfig();
                cfg.Set("first", "key", "value");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();
                
                result.Should().Be(expected);
            }

            [Fact]
            public void NamedCategoryEntries()
            {
                var expected = """
                               [first]
                               key1=value1
                               key2=value2
                               key3=value3

                               """;

                var cfg = new KdeConfig();
                cfg.Set("first", "key1", "value1");
                cfg.Set("first", "key2", "value2");
                cfg.Set("first", "key3", "value3");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();
                
                result.Should().Be(expected);
            }

            [Fact]
            public void MultipleNamedCategories()
            {
                var expected = """
                               [first]
                               key=value1
                               
                               [second]
                               key=value2

                               """;

                var cfg = new KdeConfig();
                cfg.Set("first", "key", "value1");
                cfg.Set("second", "key", "value2");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();
                
                result.Should().Be(expected);
            }

            [Fact]
            public void DefaultAndNamedCategories()
            {
                var expected = """
                               key=value1
                               
                               [first]
                               key=value2
                               
                               [second]
                               key=value3

                               """;

                var cfg = new KdeConfig();
                cfg.Set(null, "key", "value1");
                cfg.Set("first", "key", "value2");
                cfg.Set("second", "key", "value3");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();
                
                result.Should().Be(expected);
            }

            [Fact]
            public void EmptyEnforcedCategories()
            {
                var expected = """
                               [first]
                               
                               [second]
                               
                               [third]
                               
                               """;

                var cfg = new KdeConfig();
                cfg.EnforceCategories(["first", "second", "third"]);
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }

            [Fact]
            public void EmptyCategoriesRetained()
            {
                var expected = """
                               [first]

                               [second]

                               [third]
                               
                               """;
                
                var cfg = new KdeConfig();
                cfg.PreservesAllCategories = true;
                
                cfg["first", "key"]  = "value";
                cfg["second", "key"] = "value";
                cfg["third", "key"]  = "value";
                cfg.Clear("first",  "key");
                cfg.Clear("second", "key");
                cfg.Clear("third",  "key");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                string result = writer.ToString();

                result.Should().Be(expected);
            }

            [Fact]
            public void SpecificEmptyCategoriesRetained()
            {
                var expected = """
                               [first*]

                               [third~]
                               
                               """;

                var cfg = new KdeConfig();
                cfg.PreserveCategoriesWhere(cat => cat.EndsWith("*"));
                cfg.PreserveCategoriesWhere(cat => cat.EndsWith("~"));
                
                cfg["first*", "key"]  = "value";
                cfg["second", "key"] = "value";
                cfg["third~", "key"]  = "value";
                cfg["umpteenth", "key"]  = "value";
                cfg.Clear("first*",  "key");
                cfg.Clear("second", "key");
                cfg.Clear("third~",  "key");
                cfg.Clear("umpteenth",  "key");
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }
            
            
            
            [Theory]
            [InlineData("   name",   "[   name]",   Label = "Leading whitespace")]
            [InlineData("name   ",   "[name   ]",   Label = "Trailing whitespace")]
            [InlineData("name\n",    "[name\\n]",   Label = "Contains linefeed")]
            [InlineData("name\r",    "[name\\r]",   Label = "Contains carriage return")]
            [InlineData("name\t",    "[name\\t]",   Label = "Contains tab")]
            [InlineData("name\\",    "[name\\\\]",  Label = "Contains backslash")]
            [InlineData("name name", "[name name]", Label = "Contains space")]
            public void CategoryName(string name, string expectedHeader)
            {
                var expected = $"""
                                {expectedHeader}
                                key=value

                                """;

                var cfg = new KdeConfig();
                cfg.Set(name, "key", "value");

                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }

            [Theory]
            [InlineData("   key",  "\\s  key", Label = "Leading whitespace")]
            [InlineData("key   ",  "key  \\s", Label = "Trailing whitespace")]
            [InlineData("key\n",   "key\\n",   Label = "Contains linefeed")]
            [InlineData("key\r",   "key\\r",   Label = "Contains carriage return")]
            [InlineData("key\t",   "key\\t",   Label = "Contains tab")]
            [InlineData("key\\",   "key\\\\",  Label = "Contains backslash")]
            [InlineData("key key", "key key",  Label = "Contains space")]
            public void Key(string key, string expectedKeyString)
            {
                var expected = $"""
                                [first]
                                {expectedKeyString}=value

                                """;

                var cfg = new KdeConfig();
                cfg.Set("first", key, "value");

                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }

            [Theory]
            [InlineData("   value",    "\\s  value",  Label = "Leading whitespace")]
            [InlineData("value   ",    "value  \\s",  Label = "Trailing whitespace")]
            [InlineData("value\n",     "value\\n",    Label = "Contains linefeed")]
            [InlineData("value\r",     "value\\r",    Label = "Contains carriage return")]
            [InlineData("value\t",     "value\\t",    Label = "Contains tab")]
            [InlineData("value\\",     "value\\\\",   Label = "Contains backslash")]
            [InlineData("value value", "value value", Label = "Contains space")]
            public void Value(string value, string expectedValueString)
            {
                 var expected = $"""
                                 [first]
                                 key={expectedValueString}

                                 """;

                 var cfg = new KdeConfig();
                 cfg.Set("first", "key", value);

                 var writer = new StringWriter();
                 cfg.WriteToTextWriter(writer);
                 var result = writer.ToString();

                 result.Should().Be(expected);
            }

            [Theory]
            [InlineData(true,  false, false, Label = "Shell expansion")]
            [InlineData(false, true,  false, Label = "Locked down")]
            [InlineData(true,  true,  false, Label = "Shell expansion and locked down")]
            [InlineData(false, false, true,  Label = "Locale")]
            [InlineData(true,  false, true,  Label = "Locale, supports expansion")]
            [InlineData(false, true,  true,  Label = "Locale, locked down")]
            [InlineData(true,  true,  true,  Label = "Locale, supports expansion, and locked down")]
            public void EntryWithTags(bool supportsShellExpansion,
                                      bool isLockedDown,
                                      bool hasLocale)
            {
                var keyName   = "key";
                var locale    = "en";
                var localeTag = hasLocale ? $"[{locale}]" : "";

                var behaviourTag = (supportsShellExpansion, isLockedDown) switch
                {
                    (true, true)   => "[$ei]",
                    (true, false)  => "[$e]",
                    (false, true)  => "[$i]",
                    (false, false) => "",
                };

                var expected = $"""
                                [first]
                                {keyName}{localeTag}{behaviourTag}=value
                                
                                """;

                var cfg = new KdeConfig();

                if(hasLocale)
                    cfg.Set("first", keyName, locale, "value", supportsShellExpansion, isLockedDown);
                else
                    cfg.Set("first", keyName, "value", supportsShellExpansion, isLockedDown);


                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }

            [Fact]
            public void DifferentLocalisationsHavingDifferentBehaviourTags()
            {
                var expected = """
                               [first]
                               key[en][$e]=value1
                               key[fr][$i]=value2
                               
                               """;

                var cfg = new KdeConfig();
                cfg.Set("first", "key", "en", "value1", true,  false);
                cfg.Set("first", "key", "fr", "value2", false, true);
                
                var writer = new StringWriter();
                cfg.WriteToTextWriter(writer);
                var result = writer.ToString();

                result.Should().Be(expected);
            }
        }
    }
}
