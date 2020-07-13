using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace gdrive
{
    class GoogleDrive
    {
        static string[] Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        static string ApplicationName = "Quickstart";
        private static DriveService _service = GetService();

        public static DriveService GetService()
        {
            //if (Utils.IsConnectedToInternet() == false)
            //    throw new Exception("Internet Connection not available");

            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.

                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        public static Google.Apis.Drive.v3.Data.File GetItem(string ItemPath, bool GetTrashed = false)
        {
            Google.Apis.Drive.v3.Data.File file = null;

            try
            {
                file = _service.Files.Get(ItemPath).Execute();
                return file;
            }
            catch (Exception)
            {
                ItemPath = ItemPath.Trim();
                ItemPath = ItemPath.Replace("\\", "/");

                if (ItemPath[0] == '/')
                    ItemPath = ItemPath.Substring(1, ItemPath.Length - 1);

                if (ItemPath[ItemPath.Length - 1] == '/')
                    ItemPath = ItemPath.Substring(0, ItemPath.Length - 1);

                int PathArrayIndex = 0;
                var PathArray = ItemPath.Split('/');
                string ParentID = "root";
                bool found;

                FilesResource.ListRequest request = _service.Files.List();
                request.Fields = "nextPageToken, files(id, name, size, version, trashed, mimeType, createdTime)";

                IList<Google.Apis.Drive.v3.Data.File> items;

                for (int i = 0; i < PathArray.Length; i++)
                {
                    // Code similar to Dir()
                    request.Q = $"parents = '{ParentID}'";

                    try
                    {
                        items = request.Execute().Files;
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    if (items != null && items.Count > 0)
                    {
                        found = false;

                        foreach (var item in items)
                        {
                            if (item.Name == PathArray[PathArrayIndex])
                            {
                                if (GetTrashed == false && item.Trashed == true)
                                {
                                    continue;
                                }

                                ParentID = item.Id;
                                file = item;
                                found = true;
                                PathArrayIndex++;
                                break;
                            }
                        }

                        if (found == false)
                        {
                            return null; ;
                        }
                    }
                    else
                    {
                        return null; ;
                    }
                }

                return file;
            }
        }

        public static string GetItemPath(string ItemID)
        {
            FilesResource.GetRequest request;
            Google.Apis.Drive.v3.Data.File file;
            string ParentID = ItemID, ItemPath = "";

            do
            {
                try
                {
                    request = _service.Files.Get(ParentID);
                    request.Fields = "name, parents";
                    file = request.Execute();

                    if (file.Parents is null)
                        return ItemPath;

                    ParentID = file.Parents[0];

                    if (ItemPath == "")
                        ItemPath = file.Name;
                    else
                        ItemPath = file.Name + "/" + ItemPath;
                }
                catch (Exception)
                {
                    return null;
                }

            } while (true);
        }

        public static List<Google.Apis.Drive.v3.Data.File> Dir(string ParentID = "root", bool GetTrashed = false)
        {
            if (ParentID != "root")
            {
                var file = GetItem(ParentID, GetTrashed);

                if (file == null)
                {
                    throw new Exception($"Invalid Parent ID/Path '{ParentID}'");
                }

                ParentID = file.Id;
            }

            FilesResource.ListRequest request = _service.Files.List();
            request.Fields = "nextPageToken, files(id, name, size, version, trashed, mimeType, createdTime)";
            request.Q = $"parents = '{ParentID}'";

            //request.Q = "mimeType = 'application/vnd.google-apps.folder' and name = 'Temp'";
            //request.PageSize = 10;
            //request.PageToken = 10;

            IList<Google.Apis.Drive.v3.Data.File> items;

            try
            {
                items = request.Execute().Files;
            }
            catch (Exception)
            {
                throw new Exception($"Error in getting folder items for '{ParentID}'");
            }

            List<Google.Apis.Drive.v3.Data.File> ItemList = new List<Google.Apis.Drive.v3.Data.File>();

            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (item.Trashed == true)
                    {
                        if (GetTrashed == false)
                            continue;
                    }

                    ItemList.Add(item);
                }
            }
            else
            {
                return null;
            }

            return ItemList;
        }

        public static Google.Apis.Drive.v3.Data.File CreateFolder(string FolderName, string Parent = "root")
        {
            Google.Apis.Drive.v3.Data.File file;
            string ParentID = Parent;

            if (ParentID != "root")
            {
                file = GetItem(ParentID);

                if (file == null)
                {
                    throw new Exception($"Invalid Parent ID '{ParentID}'");
                }

                ParentID = file.Id;
            }

            List<Google.Apis.Drive.v3.Data.File> items;

            try
            {
                items = Dir(ParentID);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

            if(items != null && items.Count > 0)
            {
                foreach(var item in items)
                {
                    if (item.Name == FolderName)
                    {
                        throw new AlreadyExistException($"Folder '{FolderName}' Already Exists in '{Parent}', ID = '{item.Id}'", item);
                    }
                }
            }

            var FileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = FolderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { ParentID }
            };

            try
            {
                var request = _service.Files.Create(FileMetadata);
                request.Fields = "id";
                file = request.Execute();
                return file;
            }
            catch (Exception Ex)
            {
                throw new Exception($"Error in Creating Folder '{FolderName}' in '{ParentID}' : {Ex.Message}");
            }
        }

        public static Google.Apis.Drive.v3.Data.File CreateFolderStructure(string FolderPath, bool Trashed = false)
        {
            FolderPath = FolderPath.Trim();
            FolderPath = FolderPath.Replace("\\", "/");

            if (FolderPath[0] == '/')
                FolderPath = FolderPath.Substring(1, FolderPath.Length - 1);

            if (FolderPath[FolderPath.Length - 1] == '/')
                FolderPath = FolderPath.Substring(0, FolderPath.Length - 1);

            string[] PathArray = FolderPath.Split('/');

            int i, j;
            string ParentFolder, FolderID = "root";
            Google.Apis.Drive.v3.Data.File file = null;

            for (i = PathArray.Length - 1; i >= 0; i--)
            {
                ParentFolder = PathArray[0];

                for (j = 1; j <= i; j++)
                    ParentFolder = ParentFolder + "/" + PathArray[j];

                file = GetItem(ParentFolder);

                if (file != null)
                {
                    if(i == PathArray.Length - 1)
                    {
                        throw new AlreadyExistException($"Folder '{FolderPath}' Already Exists, ID = '{file.Id}'", file);
                    }

                    FolderID = file.Id;
                    break;
                }
                //else
                //{
                //    ParentFolder = Directory.GetParent(ParentFolder).FullName;
                //}
            }

            if (i == -1) FolderID = "root";

            for (j = i + 1; j <= PathArray.Length - 1; j++)
            {
                try
                {
                    file = CreateFolder(PathArray[j], FolderID);
                    FolderID = file.Id;
                }
                catch (Exception)
                {
                    throw new Exception($"Error in Creating Folder '{PathArray[j]}' in '{FolderID}'");
                }
            }

            return file;
        }

        public static Google.Apis.Drive.v3.Data.File UploadFile(string FilePath, string Parent = "root")
        {
            string ParentID = Parent;

            if (!string.IsNullOrEmpty(FilePath) && System.IO.File.Exists(FilePath))
            {
                if (ParentID != "root")
                {
                    var file = GetItem(ParentID);

                    if (file == null)
                    {
                        throw new Exception($"Invalid Parent ID/Path '{ParentID}'");
                    }

                    ParentID = file.Id;
                }

                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(FilePath),
                    MimeType = MimeTypeMap.GetMimeType(Path.GetExtension(FilePath)),
                    Parents = new List<string>() { ParentID }
                };

                FilesResource.CreateMediaUpload request;

                using (var stream = new FileStream(FilePath, FileMode.Open))
                {
                    try
                    {
                        request = _service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                        request.Fields = "id, name, size, version, trashed, mimeType, createdTime";
                        request.ProgressChanged += Request_ProgressChanged;
                        request.ResponseReceived += Request_ResponseReceived;
                        var response = request.Upload();
                    }

                    catch (Exception Ex)
                    {
                        throw new Exception($"Error in Uploading File '{FilePath}' : {Ex.Message}");
                    }
                }

                return request.ResponseBody;
            }
            else
            {
                throw new Exception($"Invalid Path '{FilePath}'");
            }
        }

        private static void Request_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            if (obj != null)
            {
                if (obj.Exception != null)
                    throw new Exception(obj.Exception.Message);
            }
        }

        private static void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj == null)
                throw new Exception("Error in Uploading File");
        }

        public static string DownloadFile(string FileID, string DestFolder)
        {
            if(!string.IsNullOrWhiteSpace(DestFolder) && Directory.Exists(DestFolder))
            {
                var file = GetItem(FileID);

                if (file == null)
                    throw new Exception($"Invalid File ID/Path '{FileID}'");

                if (file.MimeType == "application/vnd.google-apps.folder")
                    throw new Exception($"Source '{FileID}' is a Folder");

                bool failed = false;
                string FileName = file.Name;
                string FilePath = Path.Combine(DestFolder, FileName);

                FilesResource.GetRequest request = _service.Files.Get(file.Id);
                MemoryStream MemStream = new MemoryStream();

                request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:

                            //Console.WriteLine(progress.BytesDownloaded);
                            break;

                        case DownloadStatus.Completed:

                            //Console.WriteLine("Download complete.");
                            SaveMemoryStream(MemStream, FilePath);
                            break;

                        case DownloadStatus.Failed:

                            //Console.WriteLine("Download failed.");
                            failed = true;
                            break;
                    }
                };

                try
                {
                    request.Download(MemStream);

                    if (failed == true)
                    {
                        return null;
                    }
                    else
                    {
                        return FilePath;
                    }
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
            else
            {
                throw new Exception($"Invalid Destination Folder '{DestFolder}'");
            }
        }

        private static void SaveMemoryStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

        public static void Delete(string ItemID)
        {
            Google.Apis.Drive.v3.Data.File file;

            if (ItemID == "root")
            {
                throw new Exception("Root folder can not be deleted");
            }
            else
            {
                file = GetItem(ItemID);

                if (file == null)
                {
                    throw new Exception($"Invalid Item ID/Path '{ItemID}'");
                }
            }

            try
            {
                var result = _service.Files.Delete(file.Id).Execute();
            }

            catch (Exception Ex)
            {
                throw new Exception("Error in Deleting Item : ", Ex);
            }
        }

        public static IList<Permission> GetPermission(String ItemID)
        {
            var file = GetItem(ItemID);

            if (file == null)
            {
                throw new Exception($"Invalid Item ID/Path '{ItemID}'");
            }

            try
            {
                PermissionList list = _service.Permissions.List(file.Id).Execute();
                return list.Permissions;
            }
            catch (Exception Ex)
            {
                throw new Exception("Error in Creating Permission : " + Ex.Message);
            }
        }

        /// <summary>
        /// Create Permission on Item.
        /// </summary>
        /// <param name="ItemID">ID of the item to create permission for.</param>
        /// <param name="EmailAddress">
        /// Email Address of the user or group to which this permission refers.
        /// </param>
        /// <param name="type">
        /// The values are "user", "group", "domain" or "anyone".
        /// </param>
        /// <param name="role">
        /// The values are "owner", "organizer", "fileOrganizer", "writer", "commenter" or "reader".
        /// </param>
        public static IList<Permission> CreatePermission(String ItemID, String EmailAddress, String PermissionID, String Role, String Type)
        {
            var file = GetItem(ItemID);

            if (file == null)
            {
                throw new Exception($"Invalid Item ID/Path '{ItemID}'");
            }

            Permission permission = new Permission
            {
                EmailAddress = EmailAddress,
                Id = PermissionID,
                Role = Role,
                Type = Type,
            };

            try
            {
                _service.Permissions.Create(permission, file.Id).Execute();
                
                PermissionList list = _service.Permissions.List(file.Id).Execute();
                return list.Permissions;
            }
            catch (Exception Ex)
            {
                throw new Exception("Error in Creating Permission : " + Ex.Message);
            }
        }
    }
}
