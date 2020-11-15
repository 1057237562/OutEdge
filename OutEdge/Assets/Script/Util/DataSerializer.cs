using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataSerializer
{
    public static byte[] ObjectToBytes(object obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
        }
    }

    public static object BytesToObject(byte[] Bytes)
    {
        using (MemoryStream ms = new MemoryStream(Bytes))
        {
            IFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(ms);
        }
    }
}
