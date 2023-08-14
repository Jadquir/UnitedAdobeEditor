using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Scripts;

namespace UnitedAdobeEditor.Components.Classes
{
    public abstract class EncryptedFile<T> where T : class
    {
        protected abstract string GetEncrptionKey();
        protected abstract string GetSaveFilePath();
        protected abstract T? GetSaveData();

        public T? Load(T? defaultValue = null)
        {
            var path = GetSaveFilePath();
            if (File.Exists(path))
            {
                try
                {

                    var encryptedData = File.ReadAllBytes(path);
                    var decryptedData = Encryption.DecryptData(encryptedData, GetEncrptionKey());
                    var jsonString = Encoding.UTF8.GetString(decryptedData);
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch (Exception)
                {

                }
            }
            return defaultValue;
        }
        public void Save(T value)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(value);
                var dataToEncrypt = Encoding.UTF8.GetBytes(jsonString);
                var encryptedData = Encryption.EncryptData(dataToEncrypt, GetEncrptionKey());
                File.WriteAllBytes(GetSaveFilePath(), encryptedData);
            }
            catch (Exception)
            {

            }
        }
        public void Save()
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(GetSaveData());
                var dataToEncrypt = Encoding.UTF8.GetBytes(jsonString);
                var encryptedData = Encryption.EncryptData(dataToEncrypt, GetEncrptionKey());
                File.WriteAllBytes(GetSaveFilePath(), encryptedData);
            }
            catch (Exception)
            {

            }
        }
    }

}
