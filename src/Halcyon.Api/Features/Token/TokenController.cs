﻿using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Token;

public class TokenController : BaseController
{
    private readonly HalcyonDbContext _dbContext;

    private readonly IPasswordHasher _passwordHasher;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public TokenController(
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpPost("/token")]
    [Tags("Token")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index(TokenRequest request)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (user is null || user.Password is null)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The credentials provided were invalid."
            );
        }

        var verified = _passwordHasher.VerifyPassword(request.Password, user.Password);

        if (!verified)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The credentials provided were invalid."
            );
        }

        if (user.IsLockedOut)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This account has been locked out, please try again later."
            );
        }

        var result = _jwtTokenGenerator.GenerateJwtToken(user);

        return Content(result);
    }
}
