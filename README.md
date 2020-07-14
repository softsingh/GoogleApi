# Google API

C# Google Drive API based application

## Usage

### Get File Object from ID/Path

gdrive --getitem **item_id_or_path**

### Get Item Path from ID

gdrive --getitempath **item_id**

### List Directory Contents

gdrive --dir **item_id_or_path**

### Create Folder

gdrive --createfolder **folder_name** **[parent_id_or_path]**

### Create Directories and Sub-Directories

gdrive --createfolderstructure **folder_path**

### Upload File

gdrive --uploadfile **local_file_path** **[parent_id_or_path]**

### Download File

gdrive --downloadfile **file_id_or_path** **local_folder_path**

### Delete File/Folder

gdrive --delete **item_id_or_path**

### Get Permission Information

gdrive --getpermission **item_id_or_path**
