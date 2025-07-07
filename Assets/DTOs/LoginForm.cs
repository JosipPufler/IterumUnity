using System;

namespace Iterum.DTOs
{
    [Serializable]
    public class LoginForm
    {
        public string username;
        public string password;

        public LoginForm(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
