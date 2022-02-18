using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Text;
using TimeTracking.Library;
using TimeTracking.Models;

namespace TimeTracking.Controllers
{
    public class ProjectsController : Controller
    {
        public IStringLocalizer<Resource> localizer;
        public SignInManager<IdentityUser> signInManager;

        public ProjectsController(IStringLocalizer<Resource> localizer, SignInManager<IdentityUser> signInManager)
        {   
            this.localizer = localizer;
            this.signInManager = signInManager;

        }
        /* Projects list */
        public IActionResult Index()
        {
           if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            AspNetUser user = FactoryModels.CreateAspNetUserByEmail(User.Identity.Name);
            List<Project> projects = Project.GetAll();

            ViewBag.Title = localizer["ProjectIndexPageTitle"];
            ViewBag.Projects = projects;
            ViewBag.User = user;
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(string ProjectName, string ProjectDescription)
        {
            if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            if (string.IsNullOrEmpty(ProjectName))
                ModelState.AddModelError("ProjectName", localizer["ProjectNameEmpty"]);

            if (string.IsNullOrEmpty(ProjectDescription)) 
                ModelState.AddModelError("ProjectDescription", localizer["ProjectDescriptionEmpty"]);

            if(ModelState.IsValid)
            {
                Project project = new Project();
                project.ProjectName = ProjectName;
                project.ProjectDescription = ProjectDescription;
                project.AddProject();

                TempData["Message"] = localizer["ProjectAdded"].ToString();
            } else
            {
                StringBuilder result = new StringBuilder();

                foreach (var item in ModelState)
                {
                    var errors = item.Value.Errors;

                    foreach (var error in errors)
                    {
                        result.Append(error.ErrorMessage + "<br>");
                    }
                }

                TempData["Errors"] = result.ToString();
            }

            return Redirect("/projekti");
        }

        [HttpPost]
        public ActionResult UpdateProject(int ProjectId, string ProjectName, string ProjectDescription)
        {
            if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            if (string.IsNullOrEmpty(ProjectName))
                ModelState.AddModelError("ProjectName", localizer["ProjectNameEmpty"]);

            if (string.IsNullOrEmpty(ProjectDescription))
                ModelState.AddModelError("ProjectDescription", localizer["ProjectDescriptionEmpty"]);

            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Load(ProjectId);
                project.ProjectName = ProjectName;
                project.ProjectDescription = ProjectDescription;
                project.UpdateProject();

                TempData["Message"] = localizer["ProjectUpdated"].ToString();
            } else
            {
                StringBuilder result = new StringBuilder();

                foreach (var item in ModelState)
                {
                    var errors = item.Value.Errors;

                    foreach (var error in errors)
                    {
                        result.Append(error.ErrorMessage + "<br>");
                    }
                }

                TempData["Errors"] = result.ToString();
            }

            return Redirect("/projekti");
        }

        [HttpGet]
        public IActionResult DeleteProject(int id)
        {
            if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            Project project = new Project();
            project.Load(id);
            project.Delete();            
            
            TempData["Message"] = localizer["ProjectDeleted"].ToString();

            return Redirect("/projekti");
        }

    }
}
