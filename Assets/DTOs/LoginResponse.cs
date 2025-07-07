using System;

namespace Iterum.DTOs
{
    [Serializable]
    public class LoginResponse
    {
        public string Username { get; set; }
        public string JwtToken { get; set; }

        public LoginResponse(string username, string jwtToken)
        {
            Username = username;
            JwtToken = jwtToken;
        }
    }
}
