using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace gdrive
{
    public class Utils
    {
        public static GoogleDriveCmdOptions ParseCommandLineArgs(string[] args)
        {
            var options = new GoogleDriveCmdOptions();
            int index = 0;
            int length = args.Length;

            foreach (string arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                {
                    index++;
                    continue;
                }

                switch (arg.ToLower())
                {
                    case "--json": // Json Output Flag
                        
                        options.Json = true;
                        break;

                    case "--getitem": // Get File Object from ID/Path
                        
                        options.GetItem = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                                options.Source = args[index + 1];
                            else
                                options.ErrorMessage = $"Invalid Path : '{args[index + 1]}'";
                        }
                        else
                        {
                            options.ErrorMessage = $"Invalid Path : ''";
                        }
                        break;

                    case "--getitempath": // Get Item Path from ID

                        options.GetItemPath = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                                options.Source = args[index + 1];
                            else
                                options.ErrorMessage = $"Invalid ID '{args[index + 1]}'";
                        }
                        else
                        {
                            options.ErrorMessage = $"Invalid ID ''";
                        }
                        break;

                    case "--dir": // List Directory Contents
                    case "--ls":
                        
                        options.Dir = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                                options.Source = args[index + 1];
                            else
                                options.Source = "root";
                        }
                        else
                        {
                            options.Source = "root";
                        }
                        break;

                    case "--createfolder": // Create Directory
                    case "--cf":

                        options.CreateFolder = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[++index];

                                if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                                {
                                    if (!IsKeyword(args[index + 1]))
                                    {
                                        options.Destination = args[index + 1];
                                    }
                                    else
                                    {
                                        options.Destination = "root";
                                    }
                                }
                                else
                                {
                                    options.Destination = "root";
                                }
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Folder Name ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Folder Name ''";
                        }

                        break;

                    case "--createfolderstructure": // Create Directories and Sub-Directories
                    case "--cfs":

                        options.CreateFolderStructure = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[index + 1];
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Folder Path ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Folder Path ''";
                        }

                        break;

                    case "--uploadfile": // Upload File

                        options.UploadFile = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[++index];

                                if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                                {
                                    if (!IsKeyword(args[index + 1]))
                                    {
                                        options.Destination = args[index + 1];
                                    }
                                    else
                                    {
                                        options.Destination = "root";
                                    }
                                }
                                else
                                {
                                    options.Destination = "root";
                                }
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Source File Path ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Source File Path ''";
                        }

                        break;

                    case "--downloadfile": // Download File

                        options.DownloadFile = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[++index];

                                if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                                {
                                    if (!IsKeyword(args[index + 1]))
                                    {
                                        options.Destination = args[index + 1];
                                    }
                                    else
                                    {
                                        options.ErrorMessage = "Invalid Destination Folder ''";
                                    }
                                }
                                else
                                {
                                    options.ErrorMessage = "Invalid Destination Folder ''";
                                }
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Source File ID/Path ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Source File ID/Path ''";
                        }

                        break;

                    case "--deletefile": // Delete File/Folder
                    case "--delete":
                    case "--del":

                        options.Delete = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[index + 1];
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Item ID/Path ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Item ID/Path ''";
                        }

                        break;

                    case "--getpermission": // Get Permission

                        options.GetPermission = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[index + 1];
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Item ID/Path ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Item ID/Path ''";
                        }

                        break;

                    case "--createpermission": // Create (Set) Permission

                        options.CreatePermission = true;

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Source = args[index + 1];
                            }
                            else
                            {
                                options.ErrorMessage = "Invalid Item ID/Path ''";
                            }
                        }
                        else
                        {
                            options.ErrorMessage = "Invalid Item ID/Path ''";
                        }

                        break;

                    case "--emailaddress": // Email Address for CreatePermission
                    case "--email":

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.EmailAddress = args[index + 1];
                            }
                            else
                            {
                                options.EmailAddress = "";
                            }
                        }
                        else
                        {
                            options.EmailAddress = "";
                        }

                        break;

                    case "--permissionid": // Permission ID for CreatePermission

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.PermissionID = args[index + 1];
                            }
                            else
                            {
                                options.PermissionID = "";
                            }
                        }
                        else
                        {
                            options.PermissionID = "";
                        }

                        break;

                    case "--role": // Role for CreatePermission

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Role = args[index + 1];
                            }
                            else
                            {
                                options.Role = "";
                            }
                        }
                        else
                        {
                            options.Role = "";
                        }

                        break;

                    case "--type": // Role for CreatePermission

                        if (length - 1 > index && !string.IsNullOrWhiteSpace(args[index + 1]))
                        {
                            if (!IsKeyword(args[index + 1]))
                            {
                                options.Type = args[index + 1];
                            }
                            else
                            {
                                options.Type = "";
                            }
                        }
                        else
                        {
                            options.Type = "";
                        }

                        break;
                }

                index++;
            }

            return options;
        }

        public static bool IsKeyword(string MyText)
        {
            switch (MyText.ToLower())
            {
                case "--json":

                case "--getitem":

                case "--getitempath":

                case "--dir":
                case "--ls":
                
                case "--createfolder":                    
                case "--cf":
                
                case "--createfolderstructure":
                case "--cfs":
                
                case "--uploadfile":

                case "--downloadfile":

                case "--deletefile":
                case "--delete":
                case "--del":

                case "--getpermission":

                case "--createpermission":

                case "--emailaddress":
                case "--email":

                case "--permissionid":

                case "--role":

                case "--type":
                    return true;

                default:
                    return false;
            }
        }

        public static GoogleDriveItem FileToGoogleDriveItem(Google.Apis.Drive.v3.Data.File file)
        {
            return new GoogleDriveItem()
            {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                Version = file.Version,
                Trashed = file.Trashed,
                MimeType = file.MimeType,
                CreatedTime = file.CreatedTime
            };
        }

        public static List<GoogleDriveItem> FileListToGoogleDriveItemList(List<Google.Apis.Drive.v3.Data.File> files)
        {
            List<GoogleDriveItem> itemlist = new List<GoogleDriveItem>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    itemlist.Add(new GoogleDriveItem()
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        Trashed = file.Trashed,
                        MimeType = file.MimeType,
                        CreatedTime = file.CreatedTime
                    });
                }

                return itemlist;
            }
            else
            {
                return null;
            }
        }

        public static bool IsConnectedToInternet()
        {
            string host = "https://drive.google.com";
            bool result = false;
            Ping p = new Ping();

            try
            {
                PingReply reply = p.Send(host, 3000);

                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }

            return result;
        }
    }
}
