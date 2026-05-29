using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MockSrv.Web.Controllers;

public class AccountController(IDataProtectionProvider dataProtectionProvider, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : Controller
{
    [HttpGet("account/signinactual")]
    public async Task<IActionResult> SignInActual(string t)
    {
        var data = dataProtectionProvider
            .CreateProtector("SignIn")
            .Unprotect(t);

        var parts = data.Split('|');

        var identityUser = await userManager.FindByIdAsync(parts[0]);

        var isTokenValid = await userManager.VerifyUserTokenAsync(identityUser!, TokenOptions.DefaultProvider, "SignIn", parts[1]);

        if (isTokenValid)
        {
            await signInManager.SignInAsync(identityUser!, true);
            if (parts.Length == 3 && Url.IsLocalUrl(parts[2]))
            {
                return Redirect(parts[2]);
            }
            return Redirect("/");
        }
        else
        {
            return Unauthorized("STOP!");
        }
    }

    [HttpGet("account/signout")]
    public new async Task<IActionResult> SignOut()
    {
        await signInManager.SignOutAsync();

        return Redirect("/");
    }
}