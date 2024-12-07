using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sirbu_Iulia_Lab2.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public ClaimsController(UserManager<IdentityUser> userMgr)
        {
            userManager = userMgr;
        }

        public ViewResult Index() => View(User?.Claims ?? Enumerable.Empty<Claim>());

        public ViewResult Create() => View();

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> Create_Post(string claimType, string claimValue)
        {
            if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
            {
                ModelState.AddModelError("", "Claim Type and Value are required.");
                return View();
            }

            IdentityUser? user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);
            IdentityResult result = await userManager.AddClaimAsync(user, claim);

            if (result.Succeeded)
                return RedirectToAction("Index");

            Errors(result);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string claimValues)
        {
            if (string.IsNullOrWhiteSpace(claimValues))
            {
                ModelState.AddModelError("", "Invalid claim values.");
                return View("Index", User?.Claims ?? Enumerable.Empty<Claim>());
            }

            IdentityUser? user = await userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View("Index", User?.Claims ?? Enumerable.Empty<Claim>());
            }

            string[] claimValuesArray = claimValues.Split(";");
            if (claimValuesArray.Length < 3)
            {
                ModelState.AddModelError("", "Invalid claim values.");
                return View("Index", User?.Claims ?? Enumerable.Empty<Claim>());
            }

            string claimType = claimValuesArray[0];
            string claimValue = claimValuesArray[1];
            string claimIssuer = claimValuesArray[2];

            Claim? claim = User.Claims.FirstOrDefault(x =>
                x.Type == claimType && x.Value == claimValue && x.Issuer == claimIssuer);

            if (claim == null)
            {
                ModelState.AddModelError("", "Claim not found.");
                return View("Index", User?.Claims ?? Enumerable.Empty<Claim>());
            }

            IdentityResult result = await userManager.RemoveClaimAsync(user, claim);

            if (result.Succeeded)
                return RedirectToAction("Index");

            Errors(result);
            return View("Index", User?.Claims ?? Enumerable.Empty<Claim>());
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}