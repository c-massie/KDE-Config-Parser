namespace Scot.Massie.KDEConfigParser.Assignment;

/// <summary>
/// An assigned value to an entry. This includes behaviour tag information and different levels of having been
/// processed.
/// </summary>
public interface IKdeConfigEntryAssignment
{
    /// <summary>
    /// Whether escaping is enabled. If it is, <c>\n</c>, <c>\r</c>, <c>\t</c>, and <c>\s</c> in the
    /// <see cref="RawValue">raw value</see> will be translated into a newline, carriage return, tab, and space
    /// respectively in the <see cref="UnexpandedValue">value before shell expansion</see>, and vice versa. If it's not
    /// enabled, there will be no attempt to 
    /// </summary>
    /// <remarks>
    /// This flag is not represented in the KDE config file format, it exists purely to aid in interaction with the
    /// library.
    /// 
    /// This allows for values to be processed by other handlers that may, themselves, handle escape codes, such that
    /// text doesn't need to be escaped multiple times.
    /// </remarks>
    bool EscapingIsEnabled { get; }

    /// <summary>
    /// Whether shell expansion is enabled. If it is, references to shell variables will be replaced with what can be
    /// read from bash when querying those variables. See the format documentation for more information.
    /// </summary>
    bool ShellExpansionIsEnabled { get; }
    
    /// <summary>
    /// Whether the value is locked-down. See the format documentation for more information.
    /// </summary>
    bool IsLockedDown { get; }

    /// <summary>
    /// The raw, unprocessed form of this value. This is as it appears in the text of the configuration file.
    /// </summary>
    string RawValue { get; }
    
    /// <summary>
    /// The form of this value where escape sequences have been converted back into their literal characters, but before
    /// shell expansion has taken place if enabled.
    /// </summary>
    string UnexpandedValue { get; }

    /// <summary>
    /// The final intended value of this assignment. This is after escape sequences have been converted back into their
    /// literal characters, and shell expansions have had their values read from the shell if shell expansion is
    /// enabled.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets a version of this assignment where the <see cref="Value">end value</see> has been re-evaluated.
    /// </summary>
    /// <returns>
    /// If shell expansions are enabled and re-evaluating produces a different result, a copy of this with the shell
    /// expansions re-evaluated. Otherwise, this.
    /// </returns>
    IKdeConfigEntryAssignment ReExpanded();
}
