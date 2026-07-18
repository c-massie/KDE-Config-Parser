using System.Globalization;
using System.Security;

namespace Scot.Massie.KDEConfigParser.Config;

/// <summary>
/// Handler for KDE configuration files.
/// </summary>
/// <remarks>
/// As localisation codes supported by C# is an exact subset of those supported by KDE (except `iv`, the invariant
/// culture), I directly pass the two-letter ISO code from the <see cref="CultureInfo"/> object, if used instead of an
/// explicit two-letter code.
/// </remarks>
/// <seealso cref="IKdeConfig"/>
/// <seealso cref="FileStream"/>
/// <seealso href="https://userbase.kde.org/KDE_System_Administration/Configuration_Files"/>
/// <seealso href="https://community.kde.org/KDE_Core/ISO_Codes"/>
public interface IKdeConfigFile : IKdeConfig
{
    /// <summary>
    /// The path of the file this configuration saves to and loads from.
    /// </summary>
    public string FilePath { get; }
    
    /// <summary>
    /// Replaces the contents of this configuration with what could be read from its filepath.
    /// </summary>
    /// <exception cref="FileNotFoundException">If the file could not be found.</exception>
    /// <exception cref="NotSupportedException">
    /// Path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
    /// </exception>
    /// <exception cref="IOException">An IO error.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The filepath is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Reading is not permitted by the operating system for the configuration's filepath.
    /// </exception>
    /// <exception cref="PathTooLongException">The file path exceeds the system-defined maximum length.</exception>
    /// <seealso cref="FileStream"/>
    public void Load();

    /// <summary>
    /// If the file backing this exists, replaces the contents of this configuration with what could be read from its
    /// file. If it doesn't exist, creates a given default configuration file in its place and reads instead.
    /// </summary>
    /// <param name="defaultConfiguration">
    /// The formatted text of the default configuration. This should be as it appears in the file.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
    /// </exception>
    /// <exception cref="IOException">An IO error.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The filepath is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Reading is not permitted by the operating system for the configuration's filepath.
    /// </exception>
    /// <exception cref="PathTooLongException">The file path exceeds the system-defined maximum length.</exception>
    /// <seealso cref="FileStream"/>
    public void LoadOrCreateDefault(string defaultConfiguration);

    /// <summary>
    /// Writes the contents of this configuration to its backing file. If the backing file doesn't exist, this creates
    /// it. If the backing file does exist, replaces it.
    /// </summary>
    /// <remarks>
    /// This _explicitly_ saves the contents in the UTF8 encoding.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
    /// </exception>
    /// <exception cref="IOException">An IO error.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The filepath is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Reading is not permitted by the operating system for the configuration's filepath.
    /// </exception>
    /// <exception cref="PathTooLongException">The file path exceeds the system-defined maximum length.</exception>
    /// <seealso cref="FileStream"/>
    public void Save();
}
