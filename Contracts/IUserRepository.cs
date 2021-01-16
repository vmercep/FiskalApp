using Fiskal.Model;
using FiskalApp.Helpers;
using FiskalApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Contracts
{
    public interface IUserRepository:IRepositoryBase<Users>
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, AppSettings _appsettings);
        IEnumerable<Users> GetAll();
        Users GetById(int id);

        Users InsertUser(Users user);

        Task<Users> UpdateUser(Users user);

        Task<Users> UpdateUserWithPassword(Users user);
    }
}
