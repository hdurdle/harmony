using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace HarmonyHub.Utils
{
    /// <summary>
    /// Allow serialization into JSON.
    /// Most useful to be able to save complex settings for instance.
    /// Usage:
    /// ...
    /// [TypeConverter(typeof(TypeConverterJson<PersistantObject>))]
    /// [DataContract]
    /// public class PersistantObject
    /// ...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Serializer<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aData"></param>
        /// <returns></returns>
        public T Internalize(string aData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(aData);
            MemoryStream stream = new MemoryStream(byteArray);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });
            return (T)ser.ReadObject(stream);
        }

    }
}