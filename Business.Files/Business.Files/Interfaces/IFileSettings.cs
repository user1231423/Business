using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files.Interfaces
{
    public interface IFileSettings
    {
        string DefaultFilesUploadLocation { get; }
    }
}
