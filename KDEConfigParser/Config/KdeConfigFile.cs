using System.Text;

namespace Scot.Massie.KDEConfigParser.Config;

/// <inheritdoc cref="IKdeConfigFile"/>
public class KdeConfigFile : KdeConfig, IKdeConfigFile
{
    /// <summary>
    /// The path of the file this configuration saves to and loads from.
    /// </summary>
    public string FilePath { get; }
    
    /// <summary>
    /// Creates a new configuration that saves to and loads from a file.
    /// </summary>
    /// <param name="filePath">The path of the file to save to and load from.</param>
    /// <exception cref="PathTooLongException">
    /// The path parameter is longer than the system-defined maximum length. Note: In .NET for Windows Store apps or the
    /// Portable Class Library, catch the base class exception, <see cref="IOException"/>, instead.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The filepath does not contain directory information.
    /// </exception>
    public KdeConfigFile(string filePath)
    {
        var dirname = Path.GetDirectoryName(filePath);

        if(dirname is null)
            throw new ArgumentException("The filepath must contain directory information.", nameof(filePath));
            
        FilePath = filePath;
    }

    /// <inheritdoc />
    public void Load()
    {
        using var fs     = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(fs);
        Clear();
        ReadFromTextReader(reader);
    }

    /// <inheritdoc />
    public void LoadOrCreateDefault(string defaultConfiguration)
    {
        try
        {
            using var fs     = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fs);
            Clear();
            ReadFromTextReader(reader);
        }
        catch(FileNotFoundException)
        {
            File.WriteAllText(FilePath, defaultConfiguration);
            LoadFromString(defaultConfiguration);
        }
    }

    /// <inheritdoc />
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        using var fs     = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(fs, Encoding.UTF8);
        WriteToTextWriter(writer);
    }
}
