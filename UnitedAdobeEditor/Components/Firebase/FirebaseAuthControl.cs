using Firebase.Auth;
using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Classes;

namespace UnitedAdobeEditor.Components.Firebase
{
    public class FirebaseAuthControl
    {
        public const string ApiKey = "<Api Key>";
        public const string AuthDomain = "<Auth Domain>";

        private FirebaseAuthClient _client;

        private static FirebaseAuthControl? p_instance;
        public static FirebaseAuthControl Instance => p_instance ?? new FirebaseAuthControl();

        public FirebaseAuthClient Client { get { return _client; } }
        public FirebaseAuthControl()
        {
            p_instance = this;
            var config = new FirebaseAuthConfig
            {
                ApiKey = FirebaseAuthControl.ApiKey,
                AuthDomain = FirebaseAuthControl.AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                },
                UserRepository = new FirebaseData()
            };

            _client = new FirebaseAuthClient(config);
        }
    }
}
