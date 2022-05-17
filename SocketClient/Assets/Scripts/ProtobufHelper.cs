using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

public class ProtobufHelper 
{

    public static string Serialize2String<T>(T t)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms,t);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    public static byte[] Serialize2Bytes<T>(T t)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms,t);
            return ms.ToArray();
        }
    }

    public static T DeSerialize<T>(byte[] content)
    {
        using (MemoryStream ms = new MemoryStream(content))
        {
            T t = Serializer.Deserialize<T>(ms);
            return t;
        }
    }

    public static T DeSerialize<T>(string content)
    {
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
        {
            T t = Serializer.Deserialize<T>(ms);
            return t;
        }
    }
}
