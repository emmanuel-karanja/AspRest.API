
using System;
using System.Collections.Generic;
using AspRest.API.Entities;
using System.Threading.Tasks;

namespace AspRest.API.Repositories
{
    public interface IUserAccountRepository
    {
       void Create(UserAccount newAccount);
       UserAccount GetByEmail(string email);
       UserAccount GetByToken(string token);
       UserAccount GetByResetToken(string resetToken);
       IEnumerable<UserAccount> GetAll();
       UserAccount GetById(long id);
       void Delete(UserAccount account);
       void Update(UserAccount updatedAccount);
       bool Exists(string email);
       int Count();
    }
}