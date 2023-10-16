﻿using Halcyon.Api.Data;
using Halcyon.Api.Models.Token;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        private readonly IJwtService _jwtService;

        public TokenController(
            HalcyonDbContext context,
            IHashService hashService,
            IJwtService jwtService)
        {
            _context = context;
            _hashService = hashService;
            _jwtService = jwtService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(JwtToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateToken(CreateTokenRequest request)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (user is null || user.Password is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "The credentials provided were invalid."
                );
            }

            var verified = _hashService.VerifyHash(request.Password, user.Password);

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

            var result = _jwtService.CreateToken(user);

            return Ok(result);
        }
    }
}