using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlnOpener.Properties {
    
    internal sealed partial class Settings {
        
        public Settings()
        {
            this.PropertyChanged += (s, e) => Save();
        }
                
        internal List<string> FileCache
        {
            get
            {
                return (FileCacheString == "") ? new List<string>() : FileCacheString.Split('\n').ToList(); }
            set
            {
                FileCacheString = String.Join("\n",value);
            }
        }

        // yeah, this should really be in its own class...
        public void RefreshFileCache(string rootPath, string extension = "*.sln")
        {
            FileCache = FileCache
                // remove those that start with rootPath
                .Where(f => !f.StartsWith(rootPath))
                // query those which do
                .Union(Directory.GetFiles(rootPath, extension, SearchOption.AllDirectories))
                .ToList();
        }

        public IEnumerable<string> GetFiles(string rootPath, string extension)
        {
            // if there are no files starting with rootPath, force refresh
            if (!FileCache.Any(f => f.StartsWith(rootPath)))
                RefreshFileCache(rootPath, extension);
            
            return FileCache.Where(f => f.StartsWith(rootPath));
        }

    }
}
