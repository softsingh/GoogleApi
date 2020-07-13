using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdrive
{
    class AlreadyExistException : Exception
    {
        public Google.Apis.Drive.v3.Data.File file { get; set; }

        public AlreadyExistException(string message, Google.Apis.Drive.v3.Data.File DriveFile) : base(message)
        {
            file = DriveFile;
        }
    }
}
