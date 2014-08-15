using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BizLayer;
using BizLayer.ManagerClasses;

// created by Charles Drews
using BizLayer.HelperClasses;

namespace CMS_App.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        // GET: User
        [OutputCache(Duration = 15, VaryByParam = "none")]
        public ActionResult Index()
        {
            var allUsers = UserManager.GetAllUsers();
            var allUserViewModels = new List<Models.UserViewModel>();
            foreach (var user in allUsers)
            {
                allUserViewModels.Add(new Models.UserViewModel(user));
            }
            return View(allUserViewModels);
        }

        // GET: User/Details/5
        [OutputCache(Duration = 15, VaryByParam = "id")]
        public ActionResult Details(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var user = UserManager.GetUserById(id.Value);

            if (user != null 
                && user.Id != Guid.Empty)
            {
            var userViewModel = new Models.UserViewModel(user);
            userViewModel.PhotoUrl = string.Format("http://www.gravatar.com/avatar/{0}?size=200&d=http://media.tumblr.com/tumblr_lak5phfeXz1qzqijq.png", Hash.HashEmailForGravatar(userViewModel.User.Email));
            return View(userViewModel);
        }

            return RedirectToAction("Index");
        }

        // GET: User/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var user = UserManager.GetUserById(id.Value);

            if (user != null 
                && user.Id != Guid.Empty)
            {
            var userViewModel = new Models.UserViewModel(user);
            return View(userViewModel);
        }

            return RedirectToAction("Index");
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, FormCollection collection)
        {
            var userViewModel = new Models.UserViewModel();
            UpdateModel(userViewModel, collection);
            // add selected Majors to User instance if student
            if (userViewModel.User.Role == RoleType.student)
            { 
                foreach (var major in userViewModel.Majors)
                {
                    userViewModel.User.Majors.Add(new Major
                    {
                        Id = Guid.NewGuid(),
                        Name = major
                    });
                }
            }
            var retVal = UserManager.UpdateUser(userViewModel.User, userViewModel.User.Majors);

                // also update IdentityUser
            Models.IdentityModelsHelper.UpdateIdentityUserRole(userViewModel.User.Email,
                                                                  userViewModel.User.Role.ToString());

            if (retVal.Success)
            {
                TempData["success-message"] = String.Format("{0} {1} has been updated",
                                                    userViewModel.User.FirstName,
                                                    userViewModel.User.LastName); 
                return RedirectToAction("Index");
            }

            TempData["failure-message"] = String.Format("Error updating record for {0} {1}",
                                                userViewModel.User.FirstName,
                                                userViewModel.User.LastName);
            return View(userViewModel);
            }

        // GET: User/Deactivate/5
        public ActionResult Deactivate(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var user = UserManager.GetUserById(id.Value);
            
            if (user != null 
                && user.Id != Guid.Empty)
            {
                var userViewModel = new Models.UserViewModel(user);
                return View(userViewModel);
        }

            return RedirectToAction("Index");
        }

        // POST: User/Deactivate/5
        [HttpPost]
        public ActionResult Deactivate(Guid id, FormCollection collection)
        {
                var user = UserManager.GetUserById(id);
            var deactivateRetVal = UserManager.DeactivateUser(user);
                
            // don't need this call anymore; not actually deleting anyone
            //Models.IdentityModelsHelper.DeleteIdentityUser(user.Email);

            if (deactivateRetVal.Success)
                {
                TempData["success-message"] = String.Format("{0} {1} has been deactivated",
                                                    user.FirstName, user.LastName);
                    return RedirectToAction("Index", "User");
                }

            TempData["failure-message"] = String.Format("Error deactivating '{0} {1}'",
                                                user.FirstName, user.LastName);
            var userViewModel = new Models.UserViewModel(user);
            return View(userViewModel);
        }

        // GET: User/Inactive/5
        [Authorize(Roles = "admin")]
        [OutputCache(Duration = 15, VaryByParam = "none")]
        public ActionResult Inactive()
        {
            var inactiveUsers = UserManager.GetInactiveUsers();
            var inactiveUserViewModels = new List<Models.UserViewModel>();
            foreach (var user in inactiveUsers)
            {
                inactiveUserViewModels.Add(new Models.UserViewModel(user));
            }
            return View(inactiveUserViewModels);
        }

        // GET: User/Activate/5
        public ActionResult Activate(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var user = UserManager.GetUserById(id.Value);

            if (user != null 
                && user.Id != Guid.Empty)
        {
            var userViewModel = new Models.UserViewModel(user);
            return View(userViewModel);
        }

            return RedirectToAction("Index");
        }

        // POST: User/Activate/5
        [HttpPost]
        public ActionResult Activate(Guid id, FormCollection collection)
        {
            var user = UserManager.GetUserById(id);
            var activateRetVal = UserManager.ActivateUser(user);

            if (activateRetVal.Success)
            {
                TempData["success-message"] = String.Format("{0} {1} has been activated",
                                                    user.FirstName, user.LastName);
                return RedirectToAction("Index", "User");
            }

            TempData["failure-message"] = String.Format("Error activating '{0} {1}'",
                                                user.FirstName, user.LastName);
            var userViewModel = new Models.UserViewModel(user);
            return View(userViewModel);
        }
    }
}
