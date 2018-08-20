using CalcOfQuantityPPI.Data;
using CalcOfQuantityPPI.Models;
using CalcOfQuantityPPI.ViewModels.Roles;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CalcOfQuantityPPI.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new Role
                {
                    Name = model.Name,
                    Description = model.Description
                });
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            Role role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                return View(new EditRoleViewModel { Id = role.Id, Name = role.Name, Description = role.Description });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Role role = await RoleManager.FindByIdAsync(model.Id);
                if (role != null)
                {
                    role.Description = model.Description;
                    role.Name = model.Name;
                    IdentityResult result = await RoleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Что-то пошло не так");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            Role role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
    }
}