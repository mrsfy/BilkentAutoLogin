using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using RestSharp;

namespace BilnetAutoLogin
{
    public enum LoginStatus
    {
        INVALID_AUTH_INFO,
        SUCCESS,
        NO_INTERNET,
        OUTSIDE_OF_CAMPUS_NETWORK
    }
    public class Logic
    {


        private RegistryKey registryKey;

        public String Username
        {
            get
            {
                return Properties.Settings.Default.Username;
            }
            set
            {
                Properties.Settings.Default.Username = value;
                SaveSettings();
            }
        }

        public String Password
        {
            get
            {
                return Properties.Settings.Default.Password;
            }
            set
            {
                Properties.Settings.Default.Password = value;
                SaveSettings();
            }
        }

        public bool IsStartup
        {
            get
            {
                return Properties.Settings.Default.Startup;
            }
            set
            {
                Console.WriteLine(System.Environment.GetCommandLineArgs()[0]);
                Properties.Settings.Default.Startup = value;
                if (value)
                {
                    registryKey.SetValue("BilnetAutoLogin", System.Environment.GetCommandLineArgs()[0]);
                }
                else
                {
                    registryKey.DeleteValue("BilnetAutoLogin", false);
                }
                SaveSettings();
            }
        }

        public Logic()
        {

            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(OnNetworkStatusChanged);
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(OnNetworkStatusChanged);
            OnNetworkStatusChanged(null, null);
            registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        }

        public void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }

        public void OnNetworkStatusChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Network Status Changed!");
            //Console.WriteLine("Google ping: " + IsGoogleAvailable());
            Console.WriteLine("html result: " + TryLogin());
        }

        
        private LoginStatus TryLogin()
        {


            var client = new RestClient("https://auth2.bilkent.edu.tr") ;
            var request = new RestRequest("auth/login", Method.POST);
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36");
            request.AddParameter("UserName", Username);
            request.AddParameter("Password", Password);
            request.AddParameter("agree", true);
            request.ReadWriteTimeout = 1000;
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);
            LoginStatus result;
            if (response.Content.Contains("Redirect to Bilnet login page...."))
            {
                Console.WriteLine("Redirect to Bilnet login page....");
                Task.Delay(3000);
                result = TryLogin();
            }
            else if (response.Content.Contains("Invalid ID or Password"))
            {
                Console.WriteLine("Invalid ID or Password");
                result = LoginStatus.INVALID_AUTH_INFO;
            }
            else if (response.Content.Contains("This service is available only on-campus."))
            {
                Console.WriteLine("This service is available only on-campus.");
                result = LoginStatus.OUTSIDE_OF_CAMPUS_NETWORK;
            }
            else if (response.StatusCode == 0)
            {
                Console.WriteLine("No Internet!");
                result = LoginStatus.NO_INTERNET;
            }
            else
            {
                result = LoginStatus.SUCCESS;
            }

            return result;
        }


        private bool IsGoogleAvailable()
        {
            bool result = false;
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply pingReply = ping.Send("www.google.com");

                    if (pingReply.Status == IPStatus.Success)
                    {
                        result = true;
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            return result;
        }
        
    }
}
