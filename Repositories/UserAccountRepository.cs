using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using AspRest.API.Entities;
using AspRest.API.Repositories;
using System.Threading.Tasks;



namespace AspRest.API.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly ApplicationDbContext _context;
        
        public UserAccountRepository(ApplicationDbContext context)
        {
            _context=context;
        }


       public void Create(UserAccount newAccount)
       {
             newAccount.Id=0;
            _context.UserAccounts.Add(newAccount);
            _context.SaveChanges();

       }

       public IEnumerable<UserAccount> GetAll()
       {
           return _context.UserAccounts;
       }


       public UserAccount GetByEmail(string email)
       {
           return _context.UserAccounts.SingleOrDefault(x=>x.Email == email);
       }
       
       public UserAccount GetById(long id)
       {
           return _context.UserAccounts.Find(id);
       }

       public UserAccount GetByToken(string token)
       {
           var account=_context.UserAccounts.SingleOrDefault(u=> u.RefreshTokens.Any(t => t.Token ==token));
           return account;
       }
       public UserAccount GetByResetToken(string resetToken)
       {
           var account=_context.UserAccounts.SingleOrDefault(x=> x.ResetToken == resetToken 
                            && x.ResetTokenExpiresOn > DateTime.UtcNow);
           return account;
       }
       public void Update(UserAccount updatedAccount)
       {
           _context.UserAccounts.Update(updatedAccount);
           _context.SaveChanges();
       }

       public void Delete(UserAccount account)
       {
           _context.UserAccounts.Remove(account);
           _context.SaveChanges();
       }
       
       public bool Exists(string email)
       {
           var account= _context.UserAccounts.FirstOrDefault(x=>x.Email== email);
           if(account == null) return false;
           
           return true;   
       }

       public int Count()
       {
           return _context.UserAccounts.Count();
       }

       
    }
}