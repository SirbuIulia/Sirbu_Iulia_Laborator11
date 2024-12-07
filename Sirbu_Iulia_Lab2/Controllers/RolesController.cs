using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sirbu_Iulia_Lab2.Models;

namespace Sirbu_Iulia_Lab2.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleMgr, UserManager<IdentityUser> userMrg)
        {
            roleManager = roleMgr;
            userManager = userMrg;
        }

        public ViewResult Index() => View(roleManager.Roles);

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Role name cannot be empty.");
                return View(name);
            }

            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("", "Role ID cannot be null or empty.");
                return View("Index", roleManager.Roles);
            }

            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ModelState.AddModelError("", "Role not found.");
                return View("Index", roleManager.Roles);
            }

            IdentityResult result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return RedirectToAction("Index");

            Errors(result);
            return View("Index", roleManager.Roles);
        }

        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("", "Role ID cannot be null or empty.");
                return RedirectToAction("Index");
            }

            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ModelState.AddModelError("", "Role not found.");
                return RedirectToAction("Index");
            }

            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();

            foreach (IdentityUser user in userManager.Users)
            {
                bool isMember = await userManager.IsInRoleAsync(user, role.Name);
                if (isMember)
                {
                    members.Add(user);
                }
                else
                {
                    nonMembers.Add(user);
                }
            }

            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.RoleId))
            {
                ModelState.AddModelError("", "Invalid role modification data.");
                return RedirectToAction("Index");
            }

            IdentityResult result;


            foreach (string userId in model.AddIds ?? Array.Empty<string>())
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Errors(result);
                }
            }


            foreach (string userId in model.DeleteIds ?? Array.Empty<string>())
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Errors(result);
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            return await Update(model.RoleId);
        }

    }
}