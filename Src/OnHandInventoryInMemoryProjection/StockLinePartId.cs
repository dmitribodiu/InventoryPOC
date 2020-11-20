﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace OnHandInventoryInMemoryProjection
{
    public class StockLinePartId 
    {
        public static Guid NewId(Guid skuId, Guid locationId, Guid? reservationId = null)
        {
            var id = $"{skuId}-{locationId}-{reservationId}";
            return id.ToGuid();
        }
    }

    public static class StringExtensions
    {
        public static Guid ToGuid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("value can not be empty");
            }

            Guid guid;

            using (var md5 = MD5.Create())
            {
                guid = new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(value)));
            }

            return guid;
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}
