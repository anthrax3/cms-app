using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BizLayer.HelperClasses;
using System.Threading.Tasks;

// created by Charles Drews, code in "Events" region added by Shikuan Huang
namespace BizLayer.ManagerClasses
{
    public static class UserManager
    {
        #region Create
        
        /// <summary>
        /// Creates a new instance of User and save it in the database
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="isActive"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static ReturnValue CreateUser(String firstName, String lastName, String emailAddress,
                                             bool isActive, RoleType role)
        {
            var retVal = new ReturnValue();
            using (var db = new CmsModelContainer())
            {
                try
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        DateAdded = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        IsActive = isActive,
                        Role = role,
                        FirstName = String.IsNullOrWhiteSpace(firstName)
                            ? String.Empty
                            : HttpUtility.HtmlEncode(firstName),
                        LastName = String.IsNullOrWhiteSpace(lastName)
                            ? String.Empty
                            : HttpUtility.HtmlEncode(lastName),
                        Email = String.IsNullOrWhiteSpace(emailAddress)
                            ? String.Empty
                            : HttpUtility.HtmlEncode(emailAddress)
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    retVal.Success = true;
                }
                catch (Exception ex)
                {
                    retVal.Success = false;
                    retVal.ErrorMessages.Add(ex.ToString());
                }
            }

            return retVal;
        }
        
        #endregion Create

        #region Read

        /// <summary>
        /// Returns a list of all users present in the database, limited to users w/ Role == roleType if roleType is provided
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public static IEnumerable<User> GetAllUsers(RoleType? roleType = null)
        {
            var users = new List<User>();
            using (var db = new CmsModelContainer())
            {
                if (roleType != null)
                {
                    users.AddRange(db.Users.Where(u => u.IsActive && u.Role == roleType));
                }
                else
                {
                    users.AddRange(db.Users.Where(u => u.IsActive));
                }
            }

            return users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.DateAdded);
        }

        /// <summary>
        /// Returns a list of all inactive users present in the database, limited to users w/ Role == roleType if roleType is provided
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public static IEnumerable<User> GetInactiveUsers(RoleType? roleType = null)
        {
            var users = new List<User>();
            using (var db = new CmsModelContainer())
            {
                if (roleType != null)
                {
                    users.AddRange(db.Users.Where(u => !u.IsActive && u.Role == roleType));
                }
                else
                {
                    users.AddRange(db.Users.Where(u => !u.IsActive));
                }
            }

            return users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.DateAdded);
        }
        
        /// <summary>
        /// Returns the user specified by the userId argument
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static User GetUserById(Guid userId)
        {
            var user = new User();
            if (userId != null && userId != Guid.Empty)
            {
                using (var db = new CmsModelContainer())
                {
                    user = db.Users.FirstOrDefault(u => u.Id == userId);
                }
            }

            return user;
        }

        /// <summary>
        /// Returns the user specified by the emailAddress argument
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ReturnValue GetUserByEmailAddress(string emailAddress, out User user)
        {
            var retVal = new ReturnValue();
            user = new User();
            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                using (var db = new CmsModelContainer())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == emailAddress);
                    if (user != null)
                    {
                        retVal.Success = true;
                        return retVal;
                    }
                }
            }
            retVal.Success = false;
            retVal.ErrorMessages.Add("Invalid User");
            return retVal;
        }

        /// <summary>
        /// Returns a list of Major instances associated with the student specified by the studentId argument
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public static IEnumerable<Major> GetMajorsByStudentId(Guid studentId)
        {
            var majors = new List<Major>();
            using (var db = new CmsModelContainer())
            {
                majors.AddRange(db.Majors.Where(m => m.Student.Id == studentId));
            }

            return majors;
        }

        /// <summary>
        /// Returns the boolean IsActive property value for the specified user or false if invalid user
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static bool GetUserIsActiveStatusByEmail(string emailAddress)
        {
            var user = new User();
            var retVal = new ReturnValue();
            retVal = GetUserByEmailAddress(emailAddress, out user);
            
            if (retVal.Success && user != null && user.Id != Guid.Empty)
            {
                return user.IsActive;
            }
            else
            {
                return false;
            }
        }

        #endregion Read

        #region Update

        /// <summary>
        /// Updates the database entry corresponding to the user argument with the properties of the user argument
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ReturnValue UpdateUser(User user, IEnumerable<Major> majors)
        {
            var returnValue = new ReturnValue();
            if (user == null)
            {
                returnValue.ErrorMessages.Add("Invalid User");
                return returnValue;
            }

            using (var db = new CmsModelContainer())
            {
                try
                {
                    var originalUser = db.Users.SingleOrDefault(u => u.Id == user.Id);
                    
                    if (originalUser == null)
                    {
                        returnValue.ErrorMessages.Add("Invalid User");
                        return returnValue;
                    }

                    originalUser.DateUpdated = DateTime.Now;
                    originalUser.IsActive = user.IsActive;
                    originalUser.Role = user.Role;
                    originalUser.FirstName = HttpUtility.HtmlEncode(user.FirstName);
                    originalUser.LastName = HttpUtility.HtmlEncode(user.LastName);
                    originalUser.Email = HttpUtility.HtmlEncode(user.Email);
                    originalUser.Majors.Clear();
                    foreach (var major in majors)
                    {
                        originalUser.Majors.Add(major);
                    }
                    db.SaveChanges();

                    returnValue.Success = true;
                }
                catch (Exception ex)
                {
                    returnValue.Success = false;
                    returnValue.ErrorMessages.Add(ex.ToString());
                }
            }
            return returnValue;
        }

        #endregion Update

        #region Deactivate/Activate

        /// <summary>
        /// Sets the IsActive property to false for the specified user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ReturnValue DeactivateUser(User user)
        {
            var retVal = new ReturnValue();

            if (user == null)
            {
                retVal.ErrorMessages.Add("Invalid User");
                return retVal;
            }

            using (var db = new CmsModelContainer())
            {
                db.Users.Attach(user);
                //db.Users.Remove(user); //don't delete, only deactivate!
                user.IsActive = false;
                db.SaveChanges();
                retVal.Success = true;
            }

            if (ToggleEnrollmentHandleAdded == false)
            {
                ToggleEnrollmentHandleAdded = true;
                UserDeleted += new UserHandler(ToggleAssociatedEnrollmentRecord);
            }
            if (UserDeleted != null)
            {
                UserDeleted(user.Id, false);
            }

            return retVal;
        }

        /// <summary>
        /// Sets the IsActive property to true for the specified user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ReturnValue ActivateUser(User user)
        {
            var retVal = new ReturnValue();

            if (user == null)
            {
                retVal.ErrorMessages.Add("Invalid User");
                return retVal;
            }

            using (var db = new CmsModelContainer())
            {
                db.Users.Attach(user);
                user.IsActive = true;
                db.SaveChanges();
                retVal.Success = true;
            }

            if (ToggleEnrollmentHandleAdded == false)
            {
                ToggleEnrollmentHandleAdded = true;
                UserDeleted += new UserHandler(ToggleAssociatedEnrollmentRecord);
            }
            if (UserDeleted != null)
            {
                UserDeleted(user.Id, true);
            }
            
            return retVal;
        }

        #endregion Deactivate/Activate

        #region GetEnumValues

        /// <summary>
        /// Returns a list of all possible values of the enum MajorType
        /// </summary>
        /// <returns></returns>
        public static List<MajorType> GetMajorTypeOptions()
        {
            List<MajorType> majorOptions = new List<MajorType>();
            
            foreach (var value in Enum.GetValues(typeof(MajorType)))
            {
                majorOptions.Add((MajorType)value);
            }

            return majorOptions;
        }

        /// <summary>
        /// Returns a list of all possible values of the enum RoleType
        /// </summary>
        /// <returns></returns>
        public static List<RoleType> GetRoleTypeOptions()
        {
            List<RoleType> roleOptions = new List<RoleType>();

            foreach (var value in Enum.GetValues(typeof(RoleType)))
            {
                roleOptions.Add((RoleType)value);
            }

            return roleOptions;
        }

        #endregion GetEnumValues

        #region Events

        public delegate ReturnValue UserHandler(Guid id, bool value);
        public static event UserHandler UserDeleted;

        static bool ToggleEnrollmentHandleAdded { get; set; }
        public static ReturnValue ToggleAssociatedEnrollmentRecord(Guid id, bool value)
        {
            var retVal = new ReturnValue();

            using (var db = new CmsModelContainer())
            {
                try
                {
                    var query = from record in db.Enrollments
                                where record.Students.Id == id
                                select record;

                    foreach (var record in query)
                    {
                        record.IsActive = value;
                    }

                    db.SaveChanges();
                    retVal.Success = true;
                }
                catch(Exception ex)
                {
                    retVal.Success = false;
                    retVal.ErrorMessages.Add(ex.ToString());
                }

                return retVal;
            }
        }

        #endregion Events

        #region Delete

        /// <summary>
        /// Deletes specified user from database; use only to remove fake users added during unit testing.
        /// </summary>
        public static void DeleteUser(User user)
        {
            if (user == null)
            {
                return;
            }

            using (var db = new CmsModelContainer())
            {
                db.Users.Attach(user);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        #endregion Delete

    }
}
