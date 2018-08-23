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

        private DatabaseHelper db;

        public AccountController()
        {
            db = new DatabaseHelper();
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
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
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
        public ActionResult Register()
        {
            InitRolesAndDepartmentOnViewBag();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.Login, Email = model.Login, Name = model.Name };
                initUserDepartmentIdByRole(user, model);
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
            InitRolesAndDepartmentOnViewBag();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(string id)
        {
            User user = UserManager.FindById(id);
            if (user != null)
            {
                EditViewModel model = new EditViewModel
                {
                    Id = id,
                    Name = user.Name,
                    Login = user.Email,
                    Role = RoleManager.FindByName(UserManager.GetRoles(id).First()),
                    Department = db.GetDepartment(user.DepartmentId),
                    DatabaseHelper = db
                };
                return View(model);
            }
            return View("Error");
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
                user.DepartmentId = model.Department.Id;
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

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult ChangePassword(string userId)
        {
            User user = UserManager.FindById(userId);
            if (user != null)
            {
                ChangePasswordViewModel model = new ChangePasswordViewModel
                {
                    UserId = user.Id,
                    Login = user.Email
                };
                return View(model);
            }
            return View("Error");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.Login);
                if (user == null)
                {
                    return View("Error");
                }
                var validPass = await UserManager.PasswordValidator.ValidateAsync(model.NewPassword);
                if (validPass.Succeeded)
                {
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                    var res = UserManager.Update(user);
                    if (res.Succeeded)
                    {
                        AuthenticationManager.SignOut();
                        return RedirectToAction("Index", "Admin");
                    }
                }
                return View("Error");
            }
            return View(model);
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
            return View("Error");
        }

        #region PartialViews

        [HttpGet]
        public PartialViewResult DepartmentList(int id)
        {
            return PartialView(db.GetDepartments(id));
        }

        #endregion

        #region Helpers

        private void InitRolesAndDepartmentOnViewBag()
        {
            ViewBag.Roles = new SelectList(RoleManager.Roles, "Name", "Description");
            ViewBag.StructuralDepartments = new SelectList(db.GetDepartments(), "Id", "Name");
            ViewBag.Departments = new SelectList(db.GetDepartments(db.GetDepartmentByParentId(null).Id), "Id", "Name");
        }

        private void initUserDepartmentIdByRole(User user, RegisterViewModel model)
        {
            if (model.Role == "structural-department-head")
            {
                user.DepartmentId = model.StructuralDepartmentId;
            }
            else if (model.Role == "department-head")
            {
                user.DepartmentId = model.DepartmentId;
            }
            else
            {
                user.DepartmentId = null;
            }
        }

        #endregion
    }
}