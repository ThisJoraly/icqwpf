using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace icqwpf
{
    public class AuthenticationManager
    {
        public User Authenticate(string username, string password)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return null;
            }


            return user;
        }

        public void Register(string username)
        {
            var newUser = new User
            {
                Username = username
            };
        }

    }
}
