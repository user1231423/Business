using Business.Files.Extensions;
using Common.ExceptionHandler.Exceptions;
using Data.Files;
using Data.Files.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files.Services
{
    public class FileService
    {
        /// <summary>
        /// Files db context
        /// </summary>
        private FilesDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public FileService(FilesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<int> Save(IFormFile file)
        {
            try
            {
                var fileInfo = new File()
                {
                    ContentType = file.ContentType,
                    Extension = file.FileName.Split(".").LastOrDefault(),
                    OriginalName = file.FileName,
                    Path = ConfigurationManager.AppSettings["DefaultFilesUploadLocation"],
                    CreateDate = DateTime.Now
                };

                fileInfo.Path += fileInfo.Key;

                //Upload to disk
                file.UploadToDisk(fileInfo.Path);

                //Save file information to db
                _context.Files.Add(fileInfo);

                await _context.SaveChangesAsync();

                return fileInfo.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load file by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public File Load(int id)
        {
            try
            {
                return _context.Files.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load file by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public File Load(string key)
        {
            try
            {
                return _context.Files.SingleOrDefault(x => x.Key == key);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<int> Update(int id, File file)
        {
            try
            {
                var oldFile = Load(id);

                if (oldFile == null)
                    throw new NotFoundException("File not found");

                //Update old message fields
                oldFile.UpdateModifiedFields(file, ref _context);

                oldFile.UpdateDate = DateTime.Now;

                _context.Update(oldFile);

                await _context.SaveChangesAsync();

                return oldFile.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<int> Update(int id, IFormFile formFile)
        {
            try
            {
                var oldFile = Load(id);

                if (oldFile == null)
                    throw new NotFoundException("File not found");

                oldFile.UpdateDate = DateTime.Now;

                oldFile.ContentType = formFile.ContentType;
                oldFile.OriginalName = formFile.FileName;
                oldFile.Extension = formFile.FileName.Split(".").LastOrDefault();

                _context.Update(oldFile);

                await _context.SaveChangesAsync();

                if (formFile != null)
                    formFile.UpdateToDisk(oldFile.Path);

                return oldFile.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            try
            {
                var file = Load(id);

                if (file == null)
                    throw new NotFoundException("File not found");

                _context.Remove(file);

                await _context.SaveChangesAsync();

                //Remove file from disk
                FileExtensions.DeleteFromDisk(file.Path);

                return file.Id;
            }
            catch
            {
                throw;
            }
        }
    }
}
