using System.Collections.Generic;
using BizLayer;
using BizLayer.ManagerClasses;
using BizLayer.HelperClasses;

// created by Charles Drews
namespace CMS_App.Models
{
    public class UserViewModel
    {
        public User User { get; set; }

        public MajorType Major { get; set; }

        public CourseHelper Course { get; set; }

        public List<MajorType> Majors { get; set; }

        public List<RoleType> RoleOptions { get; set; }

        public List<MajorType> MajorOptions { get; set; }

        public string PhotoUrl { get; set; }

        public UserViewModel()
        {
            User = new User();
            Major = new MajorType();
            Majors = new List<MajorType>();
            RoleOptions = UserManager.GetRoleTypeOptions();
            MajorOptions = UserManager.GetMajorTypeOptions();
        }

        public UserViewModel(User user)
        {
            User = user;
            Major = new MajorType();
            Majors = new List<MajorType>();
            var userMajors = UserManager.GetMajorsByStudentId(user.Id);
            foreach (var major in userMajors)
            {
                Majors.Add(major.Name);
            }
            RoleOptions = UserManager.GetRoleTypeOptions();
            MajorOptions = UserManager.GetMajorTypeOptions();
        }
    }
}