using System.ComponentModel.DataAnnotations;
using AspRest.API.Entities;
using System;

namespace AspRest.API.Models.UserAccounts
{
    public class RegisterUserAccountRequest
    {
        [Required]
        public string Title{get;set;}

        [Required]
        public string FirstName{get;set;}

        [Required]
        public string LastName{get;set;}

        [Required]
        [EmailAddress]
        public string Email{get;set;}

        [Required]
        [MinLength(6)]
        public string Password{get;set;}

        [Compare("Password")]
        public string ConfirmPassword{get;set;}

        [Range(typeof(bool),"true","true")]
        public bool AcceptTerms{get;set;}
    }
}