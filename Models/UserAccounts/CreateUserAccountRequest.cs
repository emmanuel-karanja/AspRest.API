//the dto for creating the user
using System.ComponentModel.DataAnnotations;//for the validation attributes
using AspRest.API.Entities;
using System;

namespace AspRest.API.Models.UserAccounts
{
    public class CreateUserAccountRequest
    {
        [Required]
        [EnumDataType(typeof(Title))]
        public string Title{get;set;}

        [Required]
        public string FirstName{get;set;}

        [Required]
        public string LastName{get;set;}

        [Required]
        [EnumDataType(typeof(Role))]
        public string Role{get;set;}

        [Required]
        [EmailAddress]
        public string Email{get;set;}

        [Required]
        [MinLength(6)]
        public string Password{get;set;}

        [Required]
        [Compare("Password")]
        public string ConfirmPassword{get;set;}
    }
}