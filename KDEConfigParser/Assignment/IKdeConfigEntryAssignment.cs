namespace Scot.Massie.KDEConfigParser.Assignment;

/// <summary>
/// An assigned value to an entry. This includes behaviour tag information and different levels of having been
/// processed.
/// </summary>
public interface IKdeConfigEntryAssignment
{
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
}
