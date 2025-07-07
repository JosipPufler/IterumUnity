// Assets/Scripts/Utils/SecurityUtils.cs
using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Iterum.Scripts.Utils
{
    public static class SecurityUtils
    {
        [Serializable] public class JwtPayload { public long exp; }

        static byte[] FromBase64Url(string value)
        {
            value = value.Replace('-', '+').Replace('_', '/');
            switch (value.Length % 4)
            {
                case 2: value += "=="; break;
                case 3: value += "="; break;
            }
            return Convert.FromBase64String(value);
        }

        public static DateTime? GetExpiryFromJwt(string jwt)
        {
            try
            {
                if (string.IsNullOrEmpty(jwt)) return null;

                string token = jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                             ? jwt.Substring(7)
                             : jwt;

                string[] parts = token.Split('.');
                if (parts.Length < 2)
                {
                    Debug.LogError("Invalid JWT token format.");
                    return null;
                }

                byte[] payloadBytes = FromBase64Url(parts[1]);
                string json = Encoding.UTF8.GetString(payloadBytes);

                var data = JsonUtility.FromJson<JwtPayload>(json);
                return DateTimeOffset.FromUnixTimeSeconds(data.exp).UtcDateTime;
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to decode JWT: " + ex.Message);
                return null;
            }
        }

        public static bool IsTokenValid(string jwt)
        {
            DateTime? exp = GetExpiryFromJwt(jwt);
            return exp != null && exp.Value > DateTime.UtcNow;
        }
    }
}
