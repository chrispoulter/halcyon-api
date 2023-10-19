﻿using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.Register;

public class RegisterController : BaseController
{
    private readonly HalcyonDbContext _context;

    private readonly IPasswordHasher _passwordHasher;

    public RegisterController(
        HalcyonDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("/account/register")]
    [Tags("Account")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index(RegisterRequest request)
    {
        var existing = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (existing is not null)
        {
            return Problem(
              statusCode: StatusCodes.Status400BadRequest,
              title: "User name is already taken."
          );
        }

        var user = request.Adapt<User>();
        user.Password = _passwordHasher.GenerateHash(request.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(new UpdateResponse { Id = user.Id });
    }
}