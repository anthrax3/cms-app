using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BizLayer;
using BizLayer.HelperClasses;
using BizLayer.ManagerClasses;

// created by Charles Drews
namespace CMS_App.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        // GET: Student
        [OutputCache(Duration = 15, VaryByParam = "none")]
        public ActionResult Index()
        {
            if (!UserManager.GetUserIsActiveStatusByEmail(User.Identity.Name))
            {
                TempData["failure-message"] = "Your user account has been deactivated. Please contact the application administrator (admin@admin.com) to reactivate your account.";
                return RedirectToAction("Index", "Home");
            }

            var allStudents = UserManager.GetAllUsers(RoleType.student);
            var allStudentViewModels = new List<Models.UserViewModel>();
            foreach (var student in allStudents)
            {
                allStudentViewModels.Add(new Models.UserViewModel(student));
            }
            return View(allStudentViewModels);
        }

        // GET: Student/Details/5
        [OutputCache(Duration = 15, VaryByParam = "id")]
        public ActionResult Details(Guid? id)
        {
            if (!UserManager.GetUserIsActiveStatusByEmail(User.Identity.Name))
            {
                TempData["failure-message"] = "Your user account has been deactivated. Please contact the application administrator (admin@admin.com) to reactivate your account.";
                return RedirectToAction("Index", "Home");
            }

            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Student");
            }

            var student = UserManager.GetUserById(id.Value);

            if (student != null 
                && student.Id != Guid.Empty)
            {
                var studentDetailViewModel = new Models.StudentDetailViewModel(student);
                studentDetailViewModel.UserViewModel.PhotoUrl = string.Format("http://www.gravatar.com/avatar/{0}?size=200&d=http://media.tumblr.com/tumblr_lak5phfeXz1qzqijq.png", Hash.HashEmailForGravatar(studentDetailViewModel.UserViewModel.User.Email));
                return View(studentDetailViewModel);
            }

            return RedirectToAction("Index");
        }

        // GET: Student/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index", "Student");
            }

            var student = UserManager.GetUserById(id.Value);

            if (student != null 
                && student.Id != Guid.Empty)
            {
                var studentViewModel = new Models.UserViewModel(student);
                return View(studentViewModel);
            }

            return RedirectToAction("Index");
        }

        // POST: Student/Edit/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid id, FormCollection collection)
        {
            var studentViewModel = new Models.UserViewModel();
            UpdateModel(studentViewModel, collection);
            // add selected Majors to User instance
            foreach (var major in studentViewModel.Majors)
            {
                studentViewModel.User.Majors.Add(new Major
                {
                    Id = Guid.NewGuid(),
                    Name = major
                });
            }
            var retVal = UserManager.UpdateUser(studentViewModel.User, studentViewModel.User.Majors);

            // also update IdentityUser
            Models.IdentityModelsHelper.UpdateIdentityUserRole(studentViewModel.User.Email,
                                                              studentViewModel.User.Role.ToString());

            if (retVal.Success)
            {
                TempData["success-message"] = String.Format("{0} {1} has been updated",
                                                    studentViewModel.User.FirstName,
                                                    studentViewModel.User.LastName);
                return RedirectToAction("Index");
            }

            TempData["failure-message"] = String.Format("Error updating record for {0} {1}",
                                                studentViewModel.User.FirstName,
                                                studentViewModel.User.LastName);
            return View(studentViewModel);
        }

        // GET: Student/Deactivate/5
        [Authorize(Roles = "admin")]
        public ActionResult Deactivate(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var student = UserManager.GetUserById(id.Value);

            if (student != null 
                && student.Id != Guid.Empty)
            {
                var studentViewModel = new Models.UserViewModel(student);
                return View(studentViewModel);
            }

            return RedirectToAction("Index");
        }


        // POST: Student/Deactivate/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Deactivate(Guid id, FormCollection collection)
        {
            var student = UserManager.GetUserById(id);
            var deactivateRetVal = UserManager.DeactivateUser(student);
            
            // don't need this call anymore; not actually deleting anyone
            //Models.IdentityModelsHelper.DeleteIdentityUser(student.Email);

            if (deactivateRetVal.Success)
            {
                TempData["success-message"] = String.Format("{0} {1} has been deactivated",
                                                    student.FirstName, student.LastName);
                return RedirectToAction("Index", "Student");
            }

            TempData["failure-message"] = String.Format("Error deactivating '{0} {1}'",
                                                student.FirstName, student.LastName);
            var studentViewModel = new Models.UserViewModel(student);
            return View(studentViewModel);
        }

        // GET: Student/Activate/5
        [Authorize(Roles = "admin")]
        public ActionResult Activate(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var student = UserManager.GetUserById(id.Value);

            if (student != null 
                && student.Id != Guid.Empty)
        {
            var studentViewModel = new Models.UserViewModel(student);
            return View(studentViewModel);
        }

            return RedirectToAction("Index");
        }

        // POST: Student/Activate/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Activate(Guid id, FormCollection collection)
        {
            var student = UserManager.GetUserById(id);
            var activateRetVal = UserManager.ActivateUser(student);

            if (activateRetVal.Success)
            {
                TempData["success-message"] = String.Format("{0} {1} has been activated",
                                                    student.FirstName, student.LastName);
                return RedirectToAction("Index", "User");
            }

            TempData["failure-message"] = String.Format("Error activating '{0} {1}'",
                                                student.FirstName, student.LastName);
            var studentViewModel = new Models.UserViewModel(student);
            return View(studentViewModel);
        }
    }
}
