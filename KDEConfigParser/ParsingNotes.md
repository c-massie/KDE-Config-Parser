# Parsing notes

## Escape sequences

- Invalid escape sequences produce a FormatException.
- An unescaped trailing backslash at the end of the string resolves to an empty string, with no error.

## Values (i.e. entry assignments)

- Escape sequences are supported.
- Escape sequences are resolved before shell expansions.

### Shell expansions

- The only shell expansions supported are `$VAR` and `${VAR}` where `VAR` is the name of a bash variable. If it doesn't follow that _exact_ format, it doesn't work.

    - The curly brackets having anything other than a variable name (even if it's a variable name followed by a space, for instance) results in them producing an empty string.
        
    - Any other shell expansions are treated as plain text and not a shell expansion. e.g. `$(echo doot)` produces the literal string `(echo doot)` rather than the text `doot`. (See rule below about `$` producing empty strings.)
        
- I'm assuming that bash variable names can only contain alphanumeric characters or '_'.

- A `$` at the end of the line produces a literal `$` character, whether shell expansion is enabled or not.

- With shell expansion enabled, a `$` followed by something that isn't a bash variable name results in an empty string. Obviously, with it disabled, a `$` just produces a literal `$` in any case.
    
- With shell expansions enabled, a `${` with no closing `}` treats the rest of the line as though it were a bash variable name. This is an empty string if the rest of the line doesn't constitute a valid existing bash variable.

## Entries

- One line having multiple locales specified isn't strictly correct and should result in an error, but I'll be forgiving and interpret it as multiple separate lines with the same content. Saving will produce literally separate lines, though.

- The key is only read up to the first open square bracket. If there's a tag in the middle of the key (e.g. in `first[en]second`), everything after the tag is ignored.

- I'm assuming that keys, behaviour tags, and localisation tags cannot contain square brackets or equals characters.

- Where the same localisation code is given for the same key multiple times, it's only the last instance that counts.

- C#'s invariant culture has the two-letter code `iv`. This is the only one not supported by the KDE config format. It conceptually maps to the default localisation (i.e. the one with no locale tags), so I'll translate it to that.

## Categories

### Names/headers

- Category names *can* contain opening square brackets.

- Category names cannot contain closing square brackets - The category name is only read up to the first closing square bracket.

- The same escape sequences are available as for values and keys.

## Files

- KDE config files should be saved explicitly with the UTF8 encoding.

## See also

- Specifications for the KDE config file format: https://userbase.kde.org/KDE_System_Administration/Configuration_Files