using System.Text;
using System.Text.Json;

namespace Jobify.Application.Helpers;

public static class ObjectByteConverter
{
    public static byte[] SerializeToBytes<T>(T obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        string json = JsonSerializer.Serialize(obj);
        return Encoding.UTF8.GetBytes(json);
    }

    public static T DeserializeFromBytes<T>(byte[] data)
    {
        if (data is null || data.Length == 0)
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));

        string json = Encoding.UTF8.GetString(data);

        T? obj = JsonSerializer.Deserialize<T>(json)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize data into type {typeof(T).FullName}"
            );

        return obj;
    }
}

