using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace AscentProfiler
{
        internal static class Serializer
        {
                

                internal static string SerializeToString(object Obj)
                {
                        string encodeB64;

                        using (MemoryStream stream = new MemoryStream())
                        {
                                BinaryFormatter f = new BinaryFormatter();

                                using (DeflateStream gz = new DeflateStream(stream, CompressionMode.Compress))
                                {
                                        f.Serialize(gz, Obj);
                                }

                        encodeB64 = Convert.ToBase64String(stream.ToArray()).Replace('/', '_');

                        }


                        return encodeB64;
                }

                internal static object DeserializeFromString(string decodeB64)
                {
                        
                        decodeB64 = decodeB64.Replace('_', '/');
                        byte[] data = Convert.FromBase64String(decodeB64);
                        using (MemoryStream stream = new MemoryStream(data))
                        {
                                BinaryFormatter f = new BinaryFormatter();

                                using (DeflateStream gz = new DeflateStream(stream, CompressionMode.Decompress))
                                {
                                        return f.Deserialize(gz);
                                }

                        }

                }


        }
}
