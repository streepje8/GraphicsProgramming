namespace Striped.Engine.Serialization;

public class AssetImporter
{
    public static void ExportAsset(string folder, string filename, SerializeableObject asset)
    {
        if(!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        File.WriteAllText(folder + "/" + filename + "." + asset.GetType().Name.ToLower(), asset.Serialize());
    }
    
    public static T ImportAsset<T>(string path) where T : SerializeableObject
    {
        if(File.Exists(path))
        {
            return Deserializer.Deserialize<T>(File.ReadAllText(path));
        }
        return null;
    }
}