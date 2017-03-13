namespace YoutubeQueuer.Lib.Providers.Abstract
{
    public interface IFileSystemPersistenceProvider
    {
        void PersistData<T>(T data, string fileName);
        T GetDataOrDefault<T>(string fileName);
    }
}
