using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QFE.WPF.Tools
{
    class Internet {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable() {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
        public static async Task<bool> CheckConnection(String URL)
        {
            try
            {
                return IsInternetAvailable();
                /*
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.OK) return true;
                else return false;*/
            }
            catch
            {
                return false;
            }
        }
    }
}
