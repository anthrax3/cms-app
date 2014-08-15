using System.Collections.Generic;
using BizLayer;
using BizLayer.ManagerClasses;
using BizLayer.HelperClasses;

// created by Charles Drews
namespace CMS_App.Models
{
    public class TeacherDetailViewModel
    {
        public UserViewModel UserViewModel { get; set; }

        public IEnumerable<CourseHelper> CourseHelpers { get; set; }

        public TeacherDetailViewModel()
        {
            UserViewModel = new UserViewModel();
            CourseHelpers = new List<CourseHelper>();
        }

        public TeacherDetailViewModel(User user)
        {
            UserViewModel = new UserViewModel(user);
            CourseHelpers = new List<CourseHelper>();
            CourseHelpers = CourseManager.GetCoursesForTeacher(user);
        }
    }
}