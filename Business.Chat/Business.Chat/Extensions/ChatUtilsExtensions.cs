using Data.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Extensions
{
    public static class ChatUtilsExtensions
    {
        /// <summary>
        /// Update modified fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pEntity"></param>
        /// <param name="pEntity2"></param>
        /// <param name="pContext"></param>
        public static void UpdateModifiedFields<T>(this T pEntity, T pEntity2, ref ChatDbContext pContext)
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
