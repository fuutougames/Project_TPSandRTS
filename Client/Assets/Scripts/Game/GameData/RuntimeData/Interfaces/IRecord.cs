
namespace GameData
{
    public interface IRecord
    {
        object EncodeData();
        int DecodeData(object data);
        int Save(string path, object data);
        object Load(string path);
    }
}
