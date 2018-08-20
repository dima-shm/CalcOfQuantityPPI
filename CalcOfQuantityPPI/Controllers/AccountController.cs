using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Register()
        {
            ViewBag.Roles = new SelectList(RoleManager.Roles, "Name", "Description");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.Login, Email = model.Login, Name = model.Name };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindAsync(model.Login, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties{ IsPersistent = true }, claim);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(string id)
        {
            User user = UserManager.FindById(id);
            if (user != null)
            {
                var userRoles = UserManager.GetRoles(id);
                var allRoles = new List<Role>(RoleManager.Roles);
                EditViewModel model = new EditViewModel
                {
                    Id = id,
                    Name = user.Name,
                    Login = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(EditViewModel model, List<string> roles)
        {
            User user = UserManager.FindById(model.Id);
            if (user != null)
            {
                user.Name = model.Name;
                user.Email = model.Login;
                user.UserName = model.Login;
                EditUserRoles(model.Id, roles);
                IdentityResult result = UserManager.Update(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }
            return View(model);
        }

        private void EditUserRoles(string userId, List<string> roles)
        {
            User user = UserManager.FindById(userId);
            if (user != null)
            {
                var userRoles = UserManager.GetRoles(userId);
                var allRoles = new List<Role>(RoleManager.Roles);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                UserManager.AddToRoles(userId, addedRoles.ToArray());
                UserManager.RemoveFromRoles(userId, removedRoles.ToArray());
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(string id)
        {
            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = UserManager.FindById(id);
            if (user != null)
            {
                IdentityResult result = UserManager.Delete(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}