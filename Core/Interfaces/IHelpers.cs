using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHelpers
    {
            string HashPassword(string password);
            bool VerifyPassword(string password, string hashPassword);


    }
}
