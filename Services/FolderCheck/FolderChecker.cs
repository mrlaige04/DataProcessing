using DataProcessing.Services.FolderCheck.Interfaces;

namespace DataProcessing.Services.FolderCheck
{
    public class FolderChecker : IFolderCherker, IDisposable
    {
        private FileSystemWatcher _watcher;
        private IEnumerable<string>? extFilter;
        public FolderChecker(string path)
        {
            _watcher = new FileSystemWatcher(path);
        }

        public FolderChecker(string path, IEnumerable<string> extentionsFilder) : this(path)
        {
            if (extentionsFilder == null) return;

            extFilter = extentionsFilder;

            foreach (var extFilter in extentionsFilder)
            {
                _watcher.Filters.Add(extFilter);
            }
        }

        public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path);

        public IEnumerable<string> GetFiles(string path, IEnumerable<string> extentions)
        {
            List<string> files = Directory.GetFiles(path, "*.*")
                .Where(file => extentions
                    .Contains(Path.GetExtension(file)))
                .ToList();
            return files;
        }

        

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        

        public void EnableWatcher(FileSystemEventHandler handler)
        {
            _watcher.Created += handler;
            _watcher.EnableRaisingEvents = true;
        }
        public void DisableWatcher()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public void Dispose()
        {
            _watcher.Dispose();
            extFilter = null;
        }
    }
}
