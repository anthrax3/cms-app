using System;
using System.Web.Mvc;
using BizLayer;
using BizLayer.HelperClasses;
using BizLayer.ManagerClasses;

// created by Charles Drews
namespace CMS_App.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        // GET: Survey/Take/5
        [Authorize(Roles="student")]
        public ActionResult Take(Guid? id)
        {
            if (!UserManager.GetUserIsActiveStatusByEmail(User.Identity.Name))
            {
                TempData["failure-message"] = "Your user account has been deactivated. Please contact the application administrator (admin@admin.com) to reactivate your account.";
                return RedirectToAction("Index", "Home");
            }

            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Course");
            }

            var course = CourseManager.GetCourseById(id.Value);

            if (course != null && course.Id != Guid.Empty)
            {
                var surveyViewModel = new Models.SurveyViewModel(course);
                return View(surveyViewModel);
            }

            return RedirectToAction("Index", "Course");
        }

        // POST: Survey/Take/5
        [HttpPost]
        [Authorize(Roles = "student")]
        public ActionResult Take(Guid id, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                int countOfQuestions;
                if (!int.TryParse(collection["CountOfQuestions"], out countOfQuestions))
                    countOfQuestions = 0;

                Guid enrollmentId;
                if (!Guid.TryParse(collection["EnrollmentId"], out enrollmentId))
                    enrollmentId = Guid.Empty;

                var retVal = new ReturnValue();
                Guid responseOptionId;
                for (int i = 0; i < countOfQuestions; i++ )
                {
                    if (!Guid.TryParse(collection["SelectedResponseOptions[" + i + "].ResponseOptionId"],
                        out responseOptionId))
                        break;
                    retVal = SurveyManager.CreateResponseEvent(enrollmentId, responseOptionId);
                    // if response event cannot be created (because course doesn't exist, or because
                    // the current user is not enrolled in that course, etc.) then redirect to course details
                    if (!retVal.Success)
                        return RedirectToAction("Details", "Course", new { id = id });
                }

                return RedirectToAction("Details", "Course", new { id = id });
            }
            else
            {
                var course = CourseManager.GetCourseById(id);
                var surveyViewModel = new Models.SurveyViewModel(course);
                return View(surveyViewModel);
            }
        }

        // GET: Survey/Results/5
        [Authorize(Roles = "admin, instructor")]
        public ActionResult Results(Guid? id)
        {
            if (!UserManager.GetUserIsActiveStatusByEmail(User.Identity.Name))
            {
                TempData["failure-message"] = "Your user account has been deactivated. Please contact the application administrator (admin@admin.com) to reactivate your account.";
                return RedirectToAction("Index", "Home");
            }

            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Course");
            }

            var resultsViewModel = new Models.ResultsViewModel(id.Value);

            if (resultsViewModel != null && resultsViewModel.Course.Id != Guid.Empty)
            {
                return View(resultsViewModel);
            }

            return RedirectToAction("Index", "Course");
        }

        
    }
}
