namespace YoutubeQueuer.Lib.Providers.Abstract
{
    public interface IFileSystemPersistenceProvider
    {
        void PersistData<T>(T data, string fileName);
        T GetData<T>(string fileName);
    }
}
