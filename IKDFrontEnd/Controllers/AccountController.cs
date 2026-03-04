using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IKDFrontEnd.Models;

[Route("account")]
public class AccountController : Controller
{
    private readonly DbikdContext _context;
    public AccountController(DbikdContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // ---------------- GOOGLE LOGIN ----------------
    [Route("google-login")]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [Route("google-response")]
    public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
    {
        try
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
                return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var picture = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(email))
                throw new Exception("Google did not return email.");

            // --- DB check ---
            var user = _context.TblDefMemberInfoikds.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                var names = (name ?? "").Split(' ');
                user = new TblDefMemberInfoikd
                {
                    MemberFirstName = names.FirstOrDefault() ?? "",
                    MemberLastName = string.Join(" ", names.Skip(1)),
                    Email = email,
                    MemberSource = "Google",
                    MemberName = name ?? email,
                    Verified = true,
                    IsActive = true,
                    LastLogin = DateTime.Now
                };
                _context.TblDefMemberInfoikds.Add(user);
                await _context.SaveChangesAsync();

                // Get the generated MemberId
                await _context.Entry(user).ReloadAsync();
            }
            else
            {
                user.LastLogin = DateTime.Now;
                user.MemberSource = "Google";
                _context.TblDefMemberInfoikds.Update(user);
                await _context.SaveChangesAsync();
            }

            // --- IMPORTANT: Clear the external cookie first ---
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // --- Create YOUR app's claims identity ---
            var claimsIdentity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.MemberId.ToString()),
            new Claim(ClaimTypes.Name, user.MemberName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("UserId", user.MemberId.ToString()), // Your custom claim
            new Claim("picture", picture ?? "")
        }, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                AllowRefresh = true
            };

            // --- Sign in with YOUR app's identity ---
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // --- Return JS script for popup ---
            var script = $@"
        <script>
            if (window.opener && !window.opener.closed) {{
                window.opener.postMessage({{
                    success: true,
                    message: 'Google login successful',
                    user: {{
                        id: {user.MemberId},
                        name: '{user.MemberName?.Replace("'", "\\'")}',
                        email: '{user.Email?.Replace("'", "\\'")}',
                        picture: '{picture?.Replace("'", "\\'")}'
                    }}
                }}, '*');
                window.close();
            }} else {{
                window.location.href = '{returnUrl ?? "/"}';
            }}
        </script>";
            return Content(script, "text/html");
        }
        catch (Exception ex)
        {
            return Content($"<h1>Google Login Error</h1><pre>{ex.Message}</pre>", "text/html");
        }
    }


    // ---------------- FACEBOOK LOGIN ----------------
    [Route("facebook-login")]
    public IActionResult FacebookLogin(string? returnUrl = null)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("ExternalLoginCallback", "Account", new { returnUrl })
        };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }

    [Route("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
            return RedirectToAction("Login");

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var providerId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var picture = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

        var names = (name ?? "").Split(' ');
        var firstName = names.FirstOrDefault() ?? "";
        var lastName = string.Join(" ", names.Skip(1));

        // check user in DB
        var user = _context.TblDefMemberInfoikds.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            user = new TblDefMemberInfoikd
            {
                MemberFirstName = firstName,
                MemberLastName = lastName,
                Email = email,
                MemberSource = "Facebook",
                MemberName = name ?? email ?? "FacebookUser",
                Fburl = "https://facebook.com/" + providerId,
                Verified = true,
                IsActive = true,
                LastLogin = DateTime.Now
            };
            _context.TblDefMemberInfoikds.Add(user);
        }
        else
        {
            user.LastLogin = DateTime.Now;
            user.MemberSource = "Facebook";
            _context.TblDefMemberInfoikds.Update(user);
        }

        await _context.SaveChangesAsync();

        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.MemberName),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("UserId", user.MemberId.ToString()),
            new Claim("picture", picture ?? "")
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
    }

    // ---------------- LOGOUT ----------------
    [HttpPost("/Account/Logout")]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Json(new { success = true });
    }

    [HttpGet("LoadLoginPartial")] 
    public IActionResult LoadLoginPartial(string? returnUrl = null) 
    { ViewBag.ReturnUrl = returnUrl ?? HttpContext.Request.Path + HttpContext.Request.QueryString; 
        return PartialView("_LoginPartial"); 
    }
}
