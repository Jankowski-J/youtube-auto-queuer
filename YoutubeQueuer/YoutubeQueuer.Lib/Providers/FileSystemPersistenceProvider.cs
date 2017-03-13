using System;
using System.IO;
using Newtonsoft.Json;
using YoutubeQueuer.Lib.Providers.Abstract;

namespace YoutubeQueuer.Lib.Providers
{
    internal class FileSystemPersistenceProvider : IFileSystemPersistenceProvider
    {
        public void PersistData<T>(T data, string fileName)
        {
            var serialized = JsonConvert.SerializeObject(data);
            var fullPath = GetFullPath(fileName);

            File.WriteAllText(fullPath, serialized);
        }

        public T GetDataOrDefault<T>(string fileName)
        {
            try
            {
                var fullPath = GetFullPath(fileName);

                var serialized = File.ReadAllText(fullPath);

                return JsonConvert.DeserializeObject<T>(serialized);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        private static string GetFullPath(string fileName)
        {
            var basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            var combinedPath = Path.Combine(basePath, fileName);
            return combinedPath;
        }
    }
}
