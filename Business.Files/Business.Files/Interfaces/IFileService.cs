using Data.Files.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files.Interfaces
{
    public interface IFileService
    {
        Task<int> Save(IFormFile file);

        Task<File> Load(int id);

        Task<File> Load(string key);

        Task<int> Update(int id, File file);

        Task<int> Update(int id, IFormFile formFile);

        Task<int> Delete(int id);
    }
}
