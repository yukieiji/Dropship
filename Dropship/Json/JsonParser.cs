using System.IO;
using System.Text;
using System.Reflection;

using Newtonsoft.Json.Linq;

namespace Dropship.Json;

public static class JsonParser
{
    public static JObject GetJObjectFromAssembly(string path)
    {
        return StreamToJObject(
            Assembly.GetCallingAssembly().GetManifestResourceStream(path));
    }

    public static JObject GetJObjectFromDisk(string diskPath)
    {
        using Stream stream = File.OpenRead(diskPath);
        return StreamToJObject(stream);
    }

    public static JObject StreamToJObject(Stream stream)
    {
        byte[] byteArray = new byte[stream.Length];
        stream.Read(byteArray, 0, (int)stream.Length);

        return JObject.Parse(Encoding.UTF8.GetString(byteArray));
    }
}
