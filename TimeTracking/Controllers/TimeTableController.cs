using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html.table;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TimeTracking.Library;
using TimeTracking.Models;


namespace TimeTracking.Controllers
{
    public class TimeTableController : Controller
    {
        private AspNetUser user;
        private IStringLocalizer<Resource> localizer;
        public SignInManager<IdentityUser> signInManager;

        public TimeTableController(IStringLocalizer<Resource> localizer, SignInManager<IdentityUser> signInManager)
        {
            this.localizer = localizer;
            this.signInManager = signInManager;
        }

        public IActionResult Index(int ProjectId)
        {
            if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            user = Library.FactoryModels.CreateAspNetUserByEmail(User.Identity.Name);

            Project project = new Project();
            project.Load(ProjectId);

            List<TimeTable> timeTables = TimeTable.GetAll(user, project);

            ViewBag.Project = project;
            ViewBag.TimeTables = timeTables;
            ViewBag.User = user;

            return View();
        }

        [HttpPost]
        public IActionResult Index(int ProjectId, DateTime? WorkDate, int? WorkTime)
        {
            if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            if (WorkTime == 0 || WorkTime == null)
                ModelState.AddModelError("WorkTime", localizer["WorkTimeError"]);

            if (WorkDate == null)
                ModelState.AddModelError("WorkDate", localizer["WorkDateError"]);

            if (ModelState.IsValid)
            {
                AspNetUser user = new AspNetUser();
                user.LoadByEmail(User.Identity.Name);

                TimeTable timeTable = new TimeTable();
                timeTable.UserId = user.Id;
                timeTable.ProjectId = ProjectId;
                timeTable.WorkDate = (DateTime) WorkDate;
                timeTable.WorkTime = Convert.ToInt32(WorkTime);
                timeTable.save();

                TempData["Message"] = localizer["TimeTableAdded"].ToString();
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

            return Redirect("/projekti/" + ProjectId.ToString());
        }

        [HttpGet]
        public IActionResult DeleteTimeTable(int id)
        {
            if (!signInManager.IsSignedIn(User)) return Redirect(AppConsts.ROUTE_LOGIN);

            TimeTable timeTable = new TimeTable();
            timeTable.Load(id);

            string redirectUrl = "/projekti/" + timeTable.ProjectId.ToString();

            timeTable.Delete();

            TempData["Message"] = localizer["TimeTableDeleted"].ToString();

            return Redirect(redirectUrl);
        }

        [HttpGet]
        public FileResult DownloadPDF(int ProjectId)
        {
            user = Library.FactoryModels.CreateAspNetUserByEmail(User.Identity.Name);

            Project project = new Project();
            project.Load(ProjectId);

            List<TimeTable> timeTables = TimeTable.GetAll(user, project);

            PdfDocument doc = new PdfDocument();
            //Add a page.
            PdfPage page = doc.Pages.Add();
            //Create a PdfGrid.
            PdfGrid pdfGrid = new PdfGrid();
            //Add values to list
            List<object> data = new List<object>();

            int counter = 1;
            int timeSum = 0;
            string userName = "";
            foreach (TimeTable timeTable in timeTables)
            {
                if (!string.IsNullOrEmpty(userName) && userName != timeTable.UserName)
                {
                    Object totalRow = new { User = "", WorkDate = localizer["Total"], WorkTime = timeSum.ToString() + " | " + TimeTableHelper.GetTimeInHours(timeSum) };
                    data.Add(totalRow);

                    Object row = new
                    {
                        User = timeTable.UserName,
                        WorkDate = timeTable.WorkDate.ToShortDateString(),
                        WorkTime = timeTable.WorkTime.ToString()
                    };
                    data.Add(row);

                    timeSum = timeTable.WorkTime;
                    userName = timeTable.UserName;
                }
                else
                {
                    Object row = new
                    {
                        User = timeTable.UserName,
                        WorkDate = timeTable.WorkDate.ToShortDateString(),
                        WorkTime = timeTable.WorkTime.ToString()
                    };
                    data.Add(row);

                    timeSum += timeTable.WorkTime;
                    userName = timeTable.UserName;

                    if (counter == timeTables.Count())
                    {
                        Object totalRow = new { User = "", WorkDate = localizer["Total"], WorkTime = timeSum.ToString() + " | " + TimeTableHelper.GetTimeInHours(timeSum) };
                        data.Add(totalRow);
                    }
                }
                counter++;
            }

            //Object row1 = new { User = "Clay" };
            //data.Add(row1);
  
            //Add list to IEnumerable
            IEnumerable<object> dataTable = data;
            //Assign data source.
            pdfGrid.DataSource = dataTable;
            //Draw grid to the page of PDF document.
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));
            //Save the PDF document to stream
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            //If the position is not set to '0' then the PDF will be empty.
            stream.Position = 0;
            //Close the document.
            doc.Close(true);
            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";
            //Define the file name.
            string fileName = "Output.pdf";
            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);
        }
    }
}
