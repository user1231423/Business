using Data.Authentication;
using Data.Authentication.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Update modified fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pEntity"></param>
        /// <param name="pEntity2"></param>
        /// <param name="pContext"></param>
        public static void UpdateModifiedFields<T>(this T pEntity, T pEntity2, ref AuthenticationDbContext pContext)
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
    }
}
