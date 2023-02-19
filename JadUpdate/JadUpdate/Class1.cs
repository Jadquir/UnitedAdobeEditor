using System.ComponentModel;
using System;
using System.Diagnostics;
using JadUpdate.Class;
using Newtonsoft.Json;
using Version = JadUpdate.Class.Version;
using JadWebRequest;

namespace JadUpdate
{
    public class JadUpdate
    {
        public static string UpdateUrl { get; set; }
        public static Version LatestVersion { get; set; }
        public static Version CurrentVersion { get; set; }

        public static UpdateEventArgs CurrentUpdate;
        public static void Check(EventHandler<UpdateEventArgs> OnUpdateCheckCompleted = null)
        {
            if(CurrentVersion == null)
            {
                Debug.WriteLine("Version is not assigned! ( JadUpdate.Version )");
                return;
            }
            if (String.IsNullOrEmpty(UpdateUrl))
            {
                Debug.WriteLine("UpdateUrl is not assigned! ( JadUpdate.UpdateUrl )");
                return;
            }
            CheckUpdate(UpdateUrl, OnUpdateCheckCompleted);
        }
        public static bool IsCheckingUpdates { get; private set; }
        private static void CheckUpdate(string url = null, EventHandler<UpdateEventArgs> OnUpdateCheckCompleted = null)
        {
            if (url == null)
            {
                url = UpdateUrl;
            }
            IsCheckingUpdates = true;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                if (url == null)
                {
                    e.Cancel = true;
                    return;
                }
                UpdateData data = null;
                try
                {
                    string response = JadWebRequest.JadWebRequest.Get(url);
                    data = JsonConvert.DeserializeObject<UpdateData>(response);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error while checking update : " + ex.Message);
                    data = null;
                }

                if (data == null)
                {
                    e.Cancel = true;
                    return;
                }
                if (data.UpdateUrl != url)
                {
                    CheckUpdate(data.UpdateUrl,OnUpdateCheckCompleted);
                    e.Cancel = true;
                    return;
                }
                var uae = new UpdateEventArgs() { UpdateData = data };
                if (data.UpdateChangeLog.GetLatest().IsNewer(CurrentVersion))
                {
                    uae.UpdateAvailable = true;
                }
                else
                {
                    uae.UpdateAvailable = false;
                }
                LatestVersion = data.UpdateChangeLog.GetLatest();
                e.Result = uae;
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                bool Cancelled = e.Cancelled;
                worker.Dispose();
                if (Cancelled)
                {
                    return;
                }

                CurrentUpdate = (UpdateEventArgs)e.Result;
                IsCheckingUpdates = false;
                OnUpdateCheckCompleted?.Invoke(null, (UpdateEventArgs)e.Result);
                Debug.WriteLine("Update Check completed");
            };
            worker.RunWorkerAsync();

        }
    }
}