using Microsoft.EntityFrameworkCore;
using AspRest.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;

namespace AspRest.API.Repositories
{
    public class ApplicationDbContext: DbContext
    {
       public DbSet<UserAccount> UserAccounts {get;set;}
      
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	     : base(options){}
    }
}