﻿using Halcyon.Web.Data;
using Halcyon.Web.Services.Hash;
using Halcyon.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class SeedController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        private readonly SeedSettings _seedSettings;

        public SeedController(
            HalcyonDbContext context,
            IHashService hashService,
            IOptions<SeedSettings> seedSettings)
        {
            _context = context;
            _hashService = hashService;
            _seedSettings = seedSettings.Value;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Index()
        {
            await _context.Database.MigrateAsync();

            if (_seedSettings.Users != null)
            {
                var emailAddresses = _seedSettings.Users
                    .Select(u => u.EmailAddress);

                var users = await _context.Users
                    .Where(u => emailAddresses.Contains(u.EmailAddress))
                    .ToListAsync();

                foreach (var seedUser in _seedSettings.Users)
                {
                    var user = users.FirstOrDefault(u => u.EmailAddress == seedUser.EmailAddress);

                    if (user is null)
                    {
                        user = new();

                        _context.Users.Add(user);
                    }

                    user.EmailAddress = seedUser.EmailAddress;
                    user.Password = _hashService.GenerateHash(seedUser.Password);
                    user.FirstName = seedUser.FirstName;
                    user.LastName = seedUser.LastName;
                    user.DateOfBirth = seedUser.DateOfBirth.ToUniversalTime();
                    user.IsLockedOut = false;
                    user.Roles = seedUser.Roles;
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
