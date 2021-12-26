﻿using Halcyon.Web.Data;
using Halcyon.Web.Services.Hash;
using Halcyon.Web.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Seed
{
    public class SeedService : ISeedService
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        private readonly SeedSettings _seedSettings;

        public SeedService(
            HalcyonDbContext context,
            IHashService hashService,
            IOptions<SeedSettings> seedSettings)
        {
            _context = context;
            _hashService = hashService;
            _seedSettings = seedSettings.Value;
        }

        public void SeedData()
            => SeedDataAsync()
            .GetAwaiter()
            .GetResult();

        public async Task SeedDataAsync()
        {
            await _context.Database.MigrateAsync();

            var roleIds = new List<int>();

            foreach (Roles role in Enum.GetValues(typeof(Roles)))
            {
                var roleId = await AddRoleAsync(role.ToString());
                roleIds.Add(roleId);
            }

            await AddSystemUserAsync(roleIds);
        }

        private async Task<int> AddRoleAsync(string name)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(u => u.Name == name);

            if (role == null)
            {
                role = new Role
                {
                    Name = name
                };

                _context.Roles.Add(role);

                await _context.SaveChangesAsync();
            }

            return role.Id;
        }

        private async Task<int> AddSystemUserAsync(List<int> roleIds)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.EmailAddress == _seedSettings.EmailAddress);

            if (user == null)
            {
                user = new User();
                _context.Users.Add(user);
            }

            user.EmailAddress = _seedSettings.EmailAddress;
            user.Password = _hashService.GenerateHash(_seedSettings.Password);
            user.FirstName = "System";
            user.LastName = "Administrator";
            user.DateOfBirth = new DateTime(1970, 1, 1).ToUniversalTime();

            user.UserRoles.Clear();

            foreach (var roleId in roleIds)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }

            await _context.SaveChangesAsync();

            return user.Id;
        }
    }
}