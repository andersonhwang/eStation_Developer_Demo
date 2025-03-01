using Serilog;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Demo_Common.Helper
{
    /// <summary>
    /// File Helper
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Save file
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Data object</param>
        /// <param name="path">Save path, default with object type</param>
        public static void Save<T>(T obj, string path = "") where T : class
        {
            if (string.IsNullOrEmpty(path)) path = obj.GetType().Name + ".json";
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
            File.WriteAllBytes(path, bytes);
        }

        /// <summary>
        /// Try to get object from json file
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="path">File path</param>
        /// <returns></returns>
        public static T TryGet<T>(string path) where T : class, new()
        {
            var json = string.Empty;
            try
            {
                if (File.Exists(path))
                {
                    json = File.ReadAllText(path);
                    var obj = JsonSerializer.Deserialize<T>(json);
                    if (obj != null) return obj;
                }
                else
                {
                    Log.Warning("File_Not_Exist:" + path);
                    json = JsonSerializer.Serialize(new T());
                    var bytes = Encoding.UTF8.GetBytes(json);
                    File.WriteAllBytes(path, bytes);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"File_Read_Error:{path}:{json}", ex);
            }
            return new T();
        }

        /// <summary>
        /// Get certificate
        /// </summary>
        /// <param name="path">Certificate path</param>
        /// <param name="key">Certificate key</param>
        /// <returns>X509 certificate</returns>
        public static byte[] GetCertificate(string path, string key)
        {
            var x509 = new X509Certificate2(path, key, X509KeyStorageFlags.Exportable);
            return x509.Export(X509ContentType.Pfx);
        }

        /// <summary>
        /// Get bytes MD5
        /// </summary>
        /// <param name="bytes">Bytes array</param>
        /// <returns>MD5 value</returns>
        public static string GetBytesMd5(byte[] bytes)
        {
            try
            {
                using var md5Provider = MD5.Create();
                var buffer = md5Provider.ComputeHash(bytes);
                var result = Convert.ToHexString(buffer);
                md5Provider.Clear();
                return result;
            }
            catch (Exception e)
            {
                Log.Error("Get_Bytes_MD5_Error", e);
                return string.Empty;
            }
        }
    }
}
