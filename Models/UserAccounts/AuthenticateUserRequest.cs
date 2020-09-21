using System.ComponentModel.DataAnnotations;
using AspRest.API.Entities;
using System;

namespace AspRest.API.Models.UserAccounts
{
    public class AuthenticateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email{get;set;}

        [Required]
        public string Password{get;set;}
    }
}