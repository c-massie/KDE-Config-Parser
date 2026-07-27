# KDE Config Parser for C#

This is a library for handling, saving, and loading KDE configuration files in C#.

## How to install

### From Nuget

Include the package `Scot.Massie.KDEConfigParser`

### From Github

1. Go to https://github.com/c-massie/KDE-Config-Parser/releases
2. Download the latest `.dll` version of the library and the accompanying `.xml` file, containing documentation for the
library. Place them together in the same desired directory.
3. Right-click the project you want to add this to, go to `Add Reference...` in VS, `Add... > Add Reference...` in
Rider.
4. Click the "Add from..."/"Browse" button in the window that opens, and navigate to the .dll file downloaded.

## How to use

`KDEConfigFile` (and its interface, `IKDEConfigFile`) is the type for managing/accessing a configuration backed by a
file. Its constructor takes the path of the file the configuration should be saved to and loaded from.

The configuration object will be empty and won't reflect the contents of its backing file until `.Load()` is called.

Calling `.Save()` will write the contents of the object to the filepath it was created with. If a file already exists
there, it'll be overwritten. If it doesn't, it will be created.

There's also a base type of `KDEConfig` (and its interface, `IKDEConfig`) which includes all of the configuration
management without being backed by a file. It can still read from/write to strings, though.

In reading or writing, the category name can be replaced with null to refer to the default category. (i.e. the entries
that appear before any category header in the config file.)

### Writing

Where `cfg` is an instance of `KDEConfigFile`:

`cfg["Category", "Key"] = "Value"` or `cfg.Set("Category", "Key", "Value")` sets the value of that key in that category.

`cfg["Category", "Key", "en"] = "Value"` or `cfg.Set("Category", "Key", "en", "Value")` sets the value of that key in
that category, but specifically for where the user's UI culture is English, via the English two-letter ISO code "en".

`cfg.Set(...)` also supports specifying whether shell expansion is enabled and whether the value is "locked-down".

`cfg.Clear()` removes everything, `cfg.Clear("Category")` removes everything in that category,
`cfg.Clear("Category", "Key")` removes the value assigned to that key including any localised values, and
`cfg.Clear("Category", "Key", "en")` removes specifically the value assigned to that key for the "en" locale. The
default locale can be cleared without clearing anything else with `cfg.Clear("Category", "Key", "")`.

`cfg["Category", "Key"] = null` does the same thing as `cfg.Clear("Category", "Key", "")`, and
`cfg["Category", "Key", "en"] = null` does the same thing as `cfg.Clear("Category", "Key", "en")`.

### Reading

`cfg["Category", "Key"]` or `cfg.Get("Category", "Key")` gets the value of that key in that category, appropriate for
the user's locale. e.g. if the user's UI culture is English, a value with the locale "en" will be looked for first, and
only if that's not found, will the value assigned with no locale be provided.

`cfg["Category", "Key", "en"]` or `cfg.Get("Category", "Key", "en")` gets the value of that key in that category,
intended specifically for the specified locale, regardless of the user's current locale. If a value doesn't exist for
the specified locale specifically, the default value will still be produced if it exists.

Where no appropriate value exists for a key, null is returned.

## Notes

- Information about the KDE configuration file format can be read
  [here.](https://userbase.kde.org/KDE_System_Administration/Configuration_Files)
- The behaviour of `kreadconfig6` and `kwriteconfig6` were used as a reference in establishing expected behaviour.
Additional notes on the configuration file format not specified in the above link are available
[here.](KDEConfigParser/ParsingNotes.md)
- There are no dependencies on anything platform specific.

## Why

I'm working on a tool meant for Linux and wanted to have it be configurable. I wanted to use a standard configuration
file format and chose KDE's config format, but a nuget package didn't exist to easily read from/write to it.
