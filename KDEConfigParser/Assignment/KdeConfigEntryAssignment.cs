using static Scot.Massie.KDEConfigParser.Utils.KdeConfigTextUtils;

namespace Scot.Massie.KDEConfigParser.Assignment;

/// <inheritdoc cref="IKdeConfigEntryAssignment"/>
public class KdeConfigEntryAssignment
    : IKdeConfigEntryAssignment
{
    /// <inheritdoc />
    public bool IsLockedDown { get; }

    /// <inheritdoc />
    public bool ShellExpansionIsEnabled { get; }

    /// <inheritdoc />
    public string RawValue { get; } // Note: Neither escaped, not with shell expansions having taken effect.

    /// <inheritdoc />
    public string UnexpandedValue { get; } // Note: Escaped, but without shell expansions having taken effect.

    /// <inheritdoc />
    public string Value { get; } // Note: After having been escaped *and* shell expansions having taken effect.
    
    private KdeConfigEntryAssignment(string rawValue,
                                     string unexpandedValue,
                                     string value,
                                     bool   shellExpansionIsEnabled,
                                     bool   isLockedDown)
    {
        RawValue                = rawValue;
        UnexpandedValue         = unexpandedValue;
        Value                   = value;
        ShellExpansionIsEnabled = shellExpansionIsEnabled;
        IsLockedDown            = isLockedDown;
    }
    
    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="value">
    /// The value of the assignment. This should be the <see cref="UnexpandedValue">unexpanded value</see>, and the
    /// other versions of the value will be derived from that.
    /// </param>
    /// <param name="shellExpansionIsEnabled">
    /// Whether shell expansion is enabled. If it is, the shell will be queried for any referenced shell variables in
    /// order to produce the <see cref="Value">end value.</see> See the format documentation for more information.
    /// </param>
    /// <param name="isLockedDown">
    /// Whether the value is locked down. See the format documentation for more information.
    /// </param>
    public KdeConfigEntryAssignment(string value,
                                    bool shellExpansionIsEnabled,
                                    bool isLockedDown)
    {
        RawValue                = EscapeText(value);
        UnexpandedValue         = value;
        Value                   = shellExpansionIsEnabled ? ShellExpand(value) : value;
        ShellExpansionIsEnabled = shellExpansionIsEnabled;
        IsLockedDown            = isLockedDown;
    }

    /// <summary>
    /// Creates a new assignment, given a string representation of a value as it appears in the configuration format.
    /// (i.e. already escaped)
    /// </summary>
    /// <param name="escapedValue">
    /// The value of the assignment. This should be the <see cref="RawValue">raw value</see>, with shell expansions not
    /// yet expanded and special characters having been escaped. The other versions of the value will be derived from
    /// that.
    /// </param>
    /// <param name="shellExpansionIsEnabled">
    /// Whether shell expansion is enabled. If it is, the shell will be queried for any referenced shell variables in
    /// order to produce the <see cref="Value">end value.</see> (after being de-escaped) See the format documentation
    /// for more information.
    /// </param>
    /// <param name="isLockedDown">
    /// Whether the value is locked down. See the format documentation for more information.
    /// </param>
    /// <returns>The new assignment object.</returns>
    public static KdeConfigEntryAssignment NewFromEscapedValue(string escapedValue,
                                                               bool   shellExpansionIsEnabled, 
                                                               bool   isLockedDown)
    {
        var deEscaped = DeEscapeText(escapedValue);
        var expanded  = shellExpansionIsEnabled ? ShellExpand(deEscaped) : deEscaped;
        return new KdeConfigEntryAssignment(escapedValue, deEscaped, expanded, shellExpansionIsEnabled, isLockedDown);
    }
}
