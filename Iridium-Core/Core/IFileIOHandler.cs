using System.IO;

namespace Iridium.Core
{
    public interface IFileIOHandler
    {
        string ReadAllText(string path);
        string[] ReadAllLines(string path);
        void WriteAllText(string path, string s);
        bool FileExists(string path);
        void Delete(string path);
        Stream OpenReadStream(string path, bool exclusive);
        Stream OpenWriteStream(string path, bool exclusive, bool create);
        void AppendAllText(string path, string s);
    }
}