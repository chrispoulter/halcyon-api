﻿using Halcyon.Web.Data;
using Halcyon.Web.Filters.Validation;
using Mapster;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.User
{
    public class CreateUserRequest
    {
        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        [DisplayName("Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }

        [DisplayName("First Name")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [DisplayName("Date Of Birth")]
        [Required]
        [Past]
        public DateTime? DateOfBirth { get; set; }

        public List<Role> Roles { get; set; }
    }

    public class CreateUserRequestMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(CreateUserRequest, string), Data.User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}