using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;//you need this to use [Owned] and [Key]
//this is how you define value object data that for the purposes of
//relational database has to live outside of the entity table.
//if we were using a NoSQL db, this would not be necessary, it'd live
//within the same Document as the parent
namespace AspRest.API.Entities{
    [Owned]
    public class RefreshToken
    {
      [Key]
      public long Id{get;set;}
      //the owner
      public UserAccount UserAccount {get;set;}
      public string Token{get;set;}
      public DateTime ExpiresOn {get;set;}
      public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
      public DateTime CreatedOn{get;set;}
      public string CreatedByIp {get;set;}
      public DateTime? RevokedOn {get;set;}
      public string RevokedByIp{get;set;}
      public string ReplacedByToken{get;set;}
      public bool IsActive => RevokedOn ==null && !IsExpired;
    }
}