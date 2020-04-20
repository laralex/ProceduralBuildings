using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfDummyController
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public struct SessionInfo
    {
        public byte[] SessionKey { get; set; }
        public DateTime LastInteraction { get; set; }
    }
    public class VisualizationController : IVisualizationController
    {
        public Stream DownloadModel()
        {
            //model = null;

            //var addressValue = address.ToString();
            //bool isSessionConfirmed = Sessions[addressValue].SessionKey.SequenceEqual(sessionKey);
            //if (!isSessionConfirmed) return false;

            var dataPath = Path.Combine(Environment.CurrentDirectory, "data");
            if (!Directory.Exists(dataPath)) return null;

            var modelFiles = Directory.GetFiles(dataPath);
            if (modelFiles.Length == 0) return null;

            var randomModelPath = modelFiles[new Random().Next(modelFiles.Length)];
            return File.OpenRead(randomModelPath);
        }

        private bool IsActive { get; set; }
    }
}

/*
             public bool TryMakeSession(EndpointAddress address, out byte[] sessionKey)
        {
            const int SESSION_KEY_BYTES = 16;
            var addressValue = address.ToString();
            if (!Sessions.ContainsKey(addressValue))
            {
                sessionKey = new byte[SESSION_KEY_BYTES];
                var rng = new Random();
                rng.NextBytes(sessionKey);
                Sessions[addressValue] = new SessionInfo
                {
                    SessionKey = sessionKey,
                    LastInteraction = DateTime.Now
                };
                return true;
            }
            sessionKey = null;
            return false;
        }

            private void CleanDeadSessions()
        {
            const int ALIVE_SESSION_THRESHOLD_SEC = 120;
            var now = DateTime.Now;
            lock (Sessions)
            {
                Sessions = Sessions
                .Where(kv => (now - kv.Value.LastInteraction).Seconds <= ALIVE_SESSION_THRESHOLD_SEC)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        private Dictionary<string, SessionInfo> Sessions { get; set; }

     */
