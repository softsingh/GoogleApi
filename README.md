# Google API

This is a Google API based C# command line application.

## How to use

### Get File Object from ID/Path

```
gdrive --getitem item_id_or_path
```

### Get Item Path from ID

```
gdrive --getitempath item_id
```

### List Directory Contents

```
gdrive --dir item_id_or_path
```

### Create Folder

```
gdrive --createfolder folder_name [parent_id_or_path]
```

### Create Directories and Sub-Directories

```
gdrive --createfolderstructure folder_path
```

### Upload File
```
gdrive --uploadfile local_file_path [parent_id_or_path]
```
### Download File
```
gdrive --downloadfile file_id_or_path local_folder_path
```
### Delete File/Folder
```
gdrive --delete item_id_or_path
```
### Get Permission Information
```
gdrive --getpermission item_id_or_path
```
## Download and Run
- Download the [Portable Version](https://github.com/softsingh/GoogleApi/releases/download/Portable/gdrive.zip)
- Extract the ZIP File
- Download **credentials.json** from google api console. Visit [this link](https://developers.google.com/drive/api/v3/quickstart/dotnet) for more information.
- Put **credentials.json** inside the extracted folder
- run **gdrive.exe** through command line

## How to Compile
- Download **credentials.json** from google api console. Visit [this link](https://developers.google.com/drive/api/v3/quickstart/dotnet) for more information.
- Copy **credentials.json** to **GoogleApi\gdrive** folder.
- Open **GoogleApi.sln** in Visual Studio.
- Restore NuGet package (Tools -> NuGet Package Manager -> Manage Nuget Packages for Solution).
- Save changes, close solution and reload it again.
- Hit **Start** to compile.
