using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdrive
{
    public class GoogleDriveCmdOptions
    {
        public bool Json { get; set; } = false;
        public bool GetItem { get; set; }
        public bool GetItemPath { get; set; }
        public bool Dir { get; set; }
        public bool CreateFolder { get; set; }
        public bool CreateFolderStructure { get; set; }
        public bool UploadFile { get; set; }
        public bool DownloadFile { get; set; }
        public bool Delete { get; set; }
        public bool GetPermission { get; set; }
        public bool CreatePermission { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string EmailAddress { get; set; }
        public string PermissionID { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }
        public string ErrorMessage { get; set; } = "";

    }
}
