using Business.Files.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files.Settings
{
    public class FileSettings : IFileSettings
    {
        public string DefaultFilesUploadLocation { get; set; }
    }
}
