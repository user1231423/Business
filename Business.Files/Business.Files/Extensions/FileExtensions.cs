namespace Business.Files.Extensions
{
    using Common.ExceptionHandler.Exceptions;
    using Data.Files;
    using Microsoft.AspNetCore.Http;

    public static class FileExtensions
    {
        /// <summary>
        /// Update modified fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pEntity"></param>
        /// <param name="pEntity2"></param>
        /// <param name="pContext"></param>
        public static void UpdateModifiedFields<T>(this T pEntity, T pEntity2, ref FilesDbContext pContext)
        {
            var entry = pContext.Entry(pEntity);
            foreach (var prop in entry.Properties)
            {
                var value = pEntity2.GetType().GetProperty(prop.Metadata.Name).GetValue(pEntity2);
                if (value != null)
                {
                    prop.CurrentValue = value;
                }
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path">Path to upload</param>
        public static void UploadToDisk(this IFormFile file, string path)
        {
            try
            {
                using var stream = System.IO.File.Create(path);
                file.CopyTo(stream);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes file
        /// </summary>
        /// <param name="path">Path to file</param>
        public static void DeleteFromDisk(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    throw new InternalException("Path to delete is null or empty");

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates file
        /// </summary>
        /// <param name="path">Path to file</param>
        public static void UpdateToDisk(this IFormFile file, string path)
        {
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        // If file found, delete it    
                        DeleteFromDisk(path);

                        //Upload new file
                        file.UpdateToDisk(path);
                    }
                    //If file does not exist upload it
                    else
                        file.UploadToDisk(path);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}

