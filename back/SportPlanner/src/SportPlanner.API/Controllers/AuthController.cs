using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportPlanner.Application.DTOs;
using SportPlanner.Application.UseCases;

namespace SportPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var command = new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password);

            var result = await _mediator.Send(command);

            if (result is not null && !string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                // Set HttpOnly Secure cookie for refresh token
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                };

                Response.Cookies.Append("refresh_token", result.RefreshToken, cookieOptions);
            }

            // Return response without exposing refresh token in body
            return Ok(new
            {
                result.UserId,
                result.FirstName,
                result.LastName,
                result.Email,
                result.Role,
                accessToken = result.AccessToken
            });
        }
        catch (InvalidOperationException ex)
        {
            // Return Supabase error details
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Registration error: {ex.Message}" });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var query = new LoginUserQuery(request.Email, request.Password);
            var result = await _mediator.Send(query);

            if (result is null)
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }

            if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                };

                Response.Cookies.Append("refresh_token", result.RefreshToken, cookieOptions);
            }

            return Ok(new
            {
                result.UserId,
                result.FirstName,
                result.LastName,
                result.Email,
                result.Role,
                accessToken = result.AccessToken
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Return Supabase auth error details
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Login error: {ex.Message}" });
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult> Refresh()
    {
        try
        {
            // Read refresh token from cookie
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new { error = "No refresh token" });
            }

            // Ask application layer to exchange refresh token for a new access token
            var query = new RefreshTokenQuery(refreshToken);
            var response = await _mediator.Send(query);

            if (response is null)
            {
                return Unauthorized(new { error = "Invalid refresh token" });
            }

            // Rotate cookie if new refresh token present
            if (!string.IsNullOrWhiteSpace(response.RefreshToken))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                };

                Response.Cookies.Append("refresh_token", response.RefreshToken, cookieOptions);
            }

            return Ok(new { accessToken = response.AccessToken });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> Logout()
    {
        // Clear refresh token cookie
        Response.Cookies.Delete("refresh_token");

        // Optionally notify auth service to sign out (revoke tokens in Supabase)
        try
        {
            // Call application sign out if needed
            var command = new LogoutCommand();
            await _mediator.Send(command);
        }
        catch { /* ignore errors on signout attempt */ }

        return Ok();
    }

    /// <summary>
    /// DEBUG endpoint to verify authentication and user lookup.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public ActionResult GetCurrentUser()
    {
        Console.WriteLine("[AuthController/me] Endpoint called");
        Console.WriteLine($"[AuthController/me] User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
        Console.WriteLine($"[AuthController/me] User.Identity.Name: {User.Identity?.Name}");
        Console.WriteLine($"[AuthController/me] Claims count: {User.Claims.Count()}");

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"[AuthController/me] Claim: {claim.Type} = {claim.Value}");
        }

        return Ok(new
        {
            isAuthenticated = User.Identity?.IsAuthenticated,
            name = User.Identity?.Name,
            claims = User.Claims.Select(c => new { type = c.Type, value = c.Value })
        });
    }
}
