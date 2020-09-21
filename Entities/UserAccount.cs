using System;
using System.Linq;
using System.Collections.Generic;

namespace AspRest.API.Entities{
    public class UserAccount
    {
        public long Id{get;set;}
        //other fields here
        public Title Title{get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string Email{get;set;}
        public string PasswordHash{get;set;}
        public bool AcceptTerms {get;set;}
        public Role Role{get;set;}
        public string VerificationToken{get;set;}
        public DateTime? VerifiedOn {get;set;}
        public bool IsVerified=> true;
       //set to true until I implement the email service
       // public bool IsVerified=> VerifiedOn.HasValue || PasswordResetLastOn.HasValue;
        public string ResetToken {get;set;}
        public DateTime? ResetTokenExpiresOn {get;set;}
        public DateTime? PasswordResetLastOn{get;set;}
        public DateTime CreatedOn{get;set;}
        public DateTime? UpdatedOn{get;set;}
        public DateTime? LastLoginOn{get;set;}
        //maintain an audit trail of refresh tokens
        public List<RefreshToken> RefreshTokens{get;set;}

        public bool OwnsToken(string token)
        {
            var check=RefreshTokens.FirstOrDefault(x=> x.Token==token);
            if(check == null ) return false;
            return true;
        }

    }
}