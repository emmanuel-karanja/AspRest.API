using System.ComponentModel.DataAnnotations;
using AspRest.API.Entities;
using System;

namespace AspRest.API.Models.UserAccounts
{
    public class UpdateUserAccountRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _email;
        private string _title;

       
        public string FirstName{get;set;}
        public string LastName{get;set;}

        [EnumDataType(typeof(Role))]
        public string Role
        {
            get=> _role;
            set=> _role=replaceEmptyWithNull(value);
        }

        [EnumDataType(typeof(Title))]
        public string Title
        {
            get=> _title;
            set=> _title=replaceEmptyWithNull(value);
        }

        [EmailAddress]
        public string EmailAddress{
            get=>_email;
            set=> _email=replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
           get => _password;
           set=>_password=replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword
        {
            get=> _confirmPassword;
            set=> _confirmPassword=replaceEmptyWithNull(value);
        }


        private string replaceEmptyWithNull(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}