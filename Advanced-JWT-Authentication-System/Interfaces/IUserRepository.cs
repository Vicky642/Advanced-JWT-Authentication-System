using Advanced_JWT_Authentication_System.Models;
using Advanced_JWT_Authentication_System.Models.Authentication;
using Advanced_JWT_Authentication_System.Models.Db;
using Advanced_JWT_Authentication_System.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);       
        Task<ExecutionResult<User>> RegisterAsync(RegistrationModel model);
    }
}
