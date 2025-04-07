namespace FileService.Domain;
public class FileStorageSettings
{
    public string FolderPath
    {
        get
        {
            if (UseBaseDir)
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _folderPath);
            return _folderPath;
        }
        set
        {
            _folderPath = value;
        }
    }
    public bool UseBaseDir { get; set; } 

    private string _folderPath = "files";
}
