using Business.Files.Extensions;
using Business.Files.Interfaces;
using Common.ExceptionHandler.Exceptions;
using Data.Files;
using Data.Files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files.Services
{
    public class FileService : IFileService
    {
        /// <summary>
        /// Files db context
        /// </summary>
        private FilesDbContext _context;

        /// <summary>
        /// File settings
        /// </summary>
        private readonly IFileSettings _fileSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public FileService(FilesDbContext context, IFileSettings fileSettings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fileSettings = fileSettings ?? throw new ArgumentNullException(nameof(fileSettings));
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
                    Path = _fileSettings.DefaultFilesUploadLocation,
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
        public async Task<File> Load(int id)
        {
            try
            {
                return await _context.Files.SingleOrDefaultAsync(x => x.Id == id);
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
        public async Task<File> Load(string key)
        {
            try
            {
                return await _context.Files.SingleOrDefaultAsync(x => x.Key == key);
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
                var oldFile = await Load(id);

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
                var oldFile = await Load(id);

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
                var file = await Load(id);

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
