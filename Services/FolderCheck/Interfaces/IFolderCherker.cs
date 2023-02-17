namespace DataProcessing.Services.FolderCheck.Interfaces
{
    public interface IFolderCherker
    {
        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> GetFiles(string path, IEnumerable<string> extentions);
    }
}
