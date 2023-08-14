using Firebase.Auth.Repository;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnitedAdobeEditor.Components.Classes.FirebaseData;
using Newtonsoft.Json;
using System.IO;
using UnitedAdobeEditor.Components.Helpers;

namespace UnitedAdobeEditor.Components.Classes
{
    public class FirebaseData : EncryptedFile<UserDal>, IUserRepository
    {
        public class UserDal
        {
            public UserInfo UserInfo { get; set; }

            public FirebaseCredential Credential { get; set; }
            [JsonConstructor]
            public UserDal()
            {
            }

            public UserDal(UserInfo userInfo, FirebaseCredential credential)
            {
                UserInfo = userInfo;
                Credential = credential;
            }
        }

        protected override string GetEncrptionKey() => "<Encryption Key>";

        protected override string GetSaveFilePath() => Files.FirebaseUserPath;

        protected override UserDal? GetSaveData() => null;
        public void DeleteUser()
        {
            var path = GetSaveFilePath();
            if (File.Exists(path))
                File.Delete(path);
        }

        public (UserInfo userInfo, FirebaseCredential credential) ReadUser()
        {
            UserDal? user = Load();
            user ??= new UserDal();
            return (user.UserInfo, user.Credential);
        }

        public bool UserExists()
        {
            return File.Exists(GetSaveFilePath());
        }


        public void SaveUser(User user)
        {
            Save(new UserDal(user.Info, user.Credential));
        }
    }

}
