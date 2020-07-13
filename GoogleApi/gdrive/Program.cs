using Google.Apis.Drive.v3.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gdrive
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new GoogleDriveCmdOptions();

            try
            {
                options = Utils.ParseCommandLineArgs(args);

                //string j = JsonConvert.SerializeObject(options, Formatting.Indented);
                //Console.Write(j);
                //Console.Read();
                //return;

                if (options.ErrorMessage != "")
                    throw new Exception(options.ErrorMessage);

                if(options.GetItem == true)
                {
                    var file = GoogleDrive.GetItem(options.Source);

                    if (file == null)
                    {
                        throw new Exception($"Error in getting Google Drive Item '{options.Source}'");
                    }

                    GoogleDriveItem gdriveItem = Utils.FileToGoogleDriveItem(file);

                    string msg = $"Information of Google Drive Item '{options.Source}'";

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, gdriveItem);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                        Console.WriteLine();
                        string json = JsonConvert.SerializeObject(gdriveItem, Formatting.Indented);
                        Console.WriteLine(json);
                    }
                        
                }
                else if (options.GetItemPath == true)
                {
                    string path = GoogleDrive.GetItemPath(options.Source);

                    if (path == null)
                    {
                        throw new Exception($"Error in getting Item Path '{options.Source}'");
                    }

                    string msg = $"Path of '{options.Source}' is '{path}'";

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, path);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                        Console.WriteLine();
                    }
                }
                else if (options.Dir == true)
                {
                    var ItemList = GoogleDrive.Dir(options.Source);
                    string msg;

                    if (ItemList != null && ItemList.Count > 0)
                    {
                        msg = $"Directory of '{options.Source}'";
                    }
                    else
                    {
                        msg = $"Directory '{options.Source}' is empty";
                    }

                    if (options.Json == true)
                    {
                        List<GoogleDriveItem> gdriveItemList = Utils.FileListToGoogleDriveItemList(ItemList);

                        JsonReturn jr = new JsonReturn("OK", msg, gdriveItemList);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                        Console.WriteLine();

                        if (ItemList != null && ItemList.Count > 0)
                        {
                            foreach (var item in ItemList)
                            {
                                if(item.Trashed == false)
                                {
                                    if (item.MimeType == "application/vnd.google-apps.folder")
                                        Console.Write("<DIR>  ");
                                    else
                                        Console.Write("       ");

                                    Console.WriteLine(item.Name);
                                }
                            }
                        }
                    }
                }

                else if (options.CreateFolder == true)
                {
                    var file = GoogleDrive.CreateFolder(options.Source, options.Destination);

                    if (file == null)
                    {
                        throw new Exception($"Error in Creating Folder '{options.Source}' in '{options.Destination}'");
                    }

                    string msg = $"Folder '{options.Source}' created in '{options.Destination}', ID = {file.Id}";

                    if (options.Json == true)
                    {
                        GoogleDriveItem gdriveItem = Utils.FileToGoogleDriveItem(file);
                        JsonReturn jr = new JsonReturn("OK", msg, gdriveItem);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                    }
                }

                else if (options.CreateFolderStructure == true)
                {
                    var file = GoogleDrive.CreateFolderStructure(options.Source);

                    if (file == null)
                    {
                        throw new Exception($"Error in Creating Folder Structure '{options.Source}'");
                    }

                    string msg = $"Folder Structure '{options.Source}' created successfully, ID = {file.Id}";

                    GoogleDriveItem gdriveItem = Utils.FileToGoogleDriveItem(file);

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, gdriveItem);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                    }
                }

                else if (options.UploadFile == true)
                {
                    var file = GoogleDrive.UploadFile(options.Source, options.Destination);
                    string msg = $"File '{options.Source}' uploaded successfully, ID = {file.Id}";

                    if (options.Json == true)
                    {
                        GoogleDriveItem item = Utils.FileToGoogleDriveItem(file);
                        JsonReturn jr = new JsonReturn("OK", msg, item);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                    }
                }
                else if (options.DownloadFile == true)
                {
                    string FilePath = GoogleDrive.DownloadFile(options.Source, options.Destination);

                    if (string.IsNullOrWhiteSpace(FilePath))
                        throw new Exception("Error in Downloading File");

                    string msg = $"File '{options.Source}' downloaded successfully, Path = {FilePath}";

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, FilePath);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                    }
                }
                else if (options.Delete == true)
                {
                    GoogleDrive.Delete(options.Source);
                    string msg = $"Item '{options.Source}' deleted successfully";

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, null);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                    }
                }
                else if (options.GetPermission == true)
                {
                    IList<Permission> permissions = GoogleDrive.GetPermission(options.Source);
                    string msg = $"Permissions for '{options.Source}'";

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, permissions);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                        string json = JsonConvert.SerializeObject(permissions, Formatting.Indented);
                        Console.WriteLine(json);
                    }
                }
                else if (options.CreatePermission == true)
                {
                    IList<Permission> permissions = GoogleDrive.CreatePermission(options.Source, options.EmailAddress, options.PermissionID, options.Role, options.Type);
                    
                    string msg = $"Permissions for '{options.Source}' Created Successfully";

                    if (options.Json == true)
                    {
                        JsonReturn jr = new JsonReturn("OK", msg, permissions);
                        string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                        Console.Write(json);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(msg);
                        string json = JsonConvert.SerializeObject(permissions, Formatting.Indented);
                        Console.WriteLine(json);
                    }
                }
                else
                {
                    throw new Exception($"Invalid Command Line Arguments '{args}'");
                }
            }
            catch (AlreadyExistException Ex)
            {
                if (options.Json == true)
                {
                    GoogleDriveItem item = Utils.FileToGoogleDriveItem(Ex.file);
                    JsonReturn jr = new JsonReturn("Warning", Ex.Message, item);
                    string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                    Console.Write(json);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(Ex.Message);
                }
            }
            catch (Exception Ex)
            {
                if (options.Json == true)
                {
                    JsonReturn jr = new JsonReturn("ERROR", Ex.Message, null);
                    string json = JsonConvert.SerializeObject(jr, Formatting.Indented);
                    Console.Write(json);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(Ex.Message);
                }
            }
            Console.Read();
        }
    }
}
