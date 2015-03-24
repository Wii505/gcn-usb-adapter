using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace AutoUpdater
{
    /// <summary>
    /// Autoupdater checks a provided remote JSON file and generates an update link from that file.
    /// </summary>
    public class AutoUpdater
    {
        public AutoUpdater(string _updateUrl = "", int _currentVersion = 0)
        {
            updateUrl          = _updateUrl;
            currentVersion     = _currentVersion;
            updateAvailable    = false;
        }
        /// <summary>
        /// Checks the JSON file for a version difference, and sets the download URL for generate redirect.
        /// Returns true if there is a newer version, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool CheckForUpdates()
        {
            try
            {
                var client = new WebClient();
                var ResponseData = Encoding.UTF8.GetString(client.DownloadData(updateUrl));
                var sr = new JavaScriptSerializer();
                versionResponse databack = sr.Deserialize<versionResponse>(ResponseData);
                if (databack.applicationVersion > currentVersion)
                {
                    updateAvailable = true;
                    response = databack;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Generates a url and starts the default process to handle it, assuming an update is available.
        /// </summary>
        public void GenerateRedirect()
        {
            if (updateAvailable)
            {
                var startInfo = new ProcessStartInfo("explorer.exe", response.applicationUrl);
                Process.Start(startInfo);
            }
        }
        public string   updateUrl           { get; set; }
        public int      currentVersion      { get; set; }
        public bool     updateAvailable     { get; set; }
        public versionResponse response     { get; set; }
    }

    /// <summary>
    /// The JSON Response object for the AutoUpdater.
    /// </summary>
    public class versionResponse
    {
        public versionResponse()
        {
            applicationVersion = 0;
            applicationUrl = "";
            updateDescription = "";
        }
        public int applicationVersion { get; set; }
        public string applicationUrl { get; set; }
        public string updateDescription { get; set; }
    }
}
