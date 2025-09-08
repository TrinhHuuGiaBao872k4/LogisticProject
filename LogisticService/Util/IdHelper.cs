using System;

namespace LogisticService.Helpers
{
    public static class IdHelper
    {
        /// <summary>
        /// Sinh mã ID duy nhất theo prefix + ticks
        /// </summary>
        /// <param name="prefix">Tiền tố (ví dụ: DH, CTHD, LS...)</param>
        /// <returns>ID duy nhất dạng PREFIX + ticks</returns>
        public static string GenerateId(string prefix, int maxLength = 20)
        {
            var rawId = $"{prefix}{DateTime.UtcNow:yyyyMMddHHmmssfff}{new Random().Next(100, 999)}";

            // Nếu quá dài thì cắt bớt
            return rawId.Length > maxLength ? rawId.Substring(0, maxLength) : rawId;
        }
    }
}
