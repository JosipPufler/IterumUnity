namespace Iterum.DTOs
{
    public class RegisterForm
    {
        public string username;
        public string password;

        public RegisterForm(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
