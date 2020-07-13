using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdrive
{
    public class GoogleDriveItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public bool? Trashed { get; set; }
        public string MimeType { get; set; }
        public DateTime? CreatedTime { get; set; }
        
    }
}
