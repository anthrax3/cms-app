using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizLayer;
using BizLayer.HelperClasses;
using BizLayer.ManagerClasses;

// created by Charles Drews
namespace CMS_App.Tests.UserManager
{
    [TestClass]
    public class ReadUsersTests
    {
        [TestMethod]
        public void TestUsersGetAll()
        {
            var users = new List<User>();
            users.AddRange(BizLayer.ManagerClasses.UserManager.GetAllUsers());
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
            bool activeOnly = true;
            foreach (var user in users)
            {
                if (!user.IsActive)
                {
                    activeOnly = false;
                }
            }
            Assert.IsTrue(activeOnly);
        }

        [TestMethod]
        public void TestUsersGetAllStudents()
        {
            var users = new List<User>();
            users.AddRange(BizLayer.ManagerClasses.UserManager.GetAllUsers(RoleType.student));
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
            bool studentsOnly = true;
            bool activeOnly = true; 
            foreach (var user in users)
            {
                if (user.Role != RoleType.student)
                {
                    studentsOnly = false;
                }
                if (!user.IsActive)
                {
                    activeOnly = false;
                }
            }
            Assert.IsTrue(studentsOnly);
            Assert.IsTrue(activeOnly);
        }

        [TestMethod]
        public void TestUsersGetAllTeachers()
        {
            var users = new List<User>();
            users.AddRange(BizLayer.ManagerClasses.UserManager.GetAllUsers(RoleType.instructor));
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
            bool teachersOnly = true;
            bool activeOnly = true; 
            foreach (var user in users)
            {
                if (user.Role != RoleType.instructor)
                {
                    teachersOnly = false;
                }
                if (!user.IsActive)
                {
                    activeOnly = false;
                }
            }
            Assert.IsTrue(teachersOnly);
            Assert.IsTrue(activeOnly);
        }

        [TestMethod]
        public void TestUsersGetAllInactive()
        {
            // add at least one inactive user
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                false,
                RoleType.student);

            var users = new List<User>();
            users.AddRange(BizLayer.ManagerClasses.UserManager.GetInactiveUsers());
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
            bool inactiveOnly = true;
            foreach (var user in users)
            {
                if (user.IsActive)
                {
                    inactiveOnly = false;
                }
            }
            Assert.IsTrue(inactiveOnly);

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestUsersGetAllInactiveStudents()
        {
            // add at least one inactive user
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                false,
                RoleType.student);

            var users = new List<User>();
            users.AddRange(BizLayer.ManagerClasses.UserManager.GetInactiveUsers(RoleType.student));
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
            bool studentsOnly = true;
            bool inactiveOnly = true;
            foreach (var user in users)
            {
                if (user.Role != RoleType.student)
                {
                    studentsOnly = false;
                }
                if (user.IsActive)
                {
                    inactiveOnly = false;
                }
            }
            Assert.IsTrue(studentsOnly);
            Assert.IsTrue(inactiveOnly);

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestUsersGetAllInactiveTeachers()
        {
            // add at least one inactive user
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                false,
                RoleType.instructor);

            var users = new List<User>();
            users.AddRange(BizLayer.ManagerClasses.UserManager.GetInactiveUsers(RoleType.instructor));
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
            bool teachersOnly = true;
            bool inactiveOnly = true;
            foreach (var user in users)
            {
                if (user.Role != RoleType.instructor)
                {
                    teachersOnly = false;
                }
                if (user.IsActive)
                {
                    inactiveOnly = false;
                }
            }
            Assert.IsTrue(teachersOnly);
            Assert.IsTrue(inactiveOnly);

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestUserGetById()
        {
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            User user;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user);
            Guid userId = user.Id;
            User user2 = BizLayer.ManagerClasses.UserManager.GetUserById(userId);
            Assert.AreEqual(user.Id, user2.Id);

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestUserGetByEmail()
        {
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            User user;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user);
            Assert.AreEqual("unit_test_email@test.com", user.Email);

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestUserGetMajors()
        {
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            User user;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user);

            var majors = new List<Major>();
            majors.Add(new Major { Id = Guid.NewGuid(), Name = MajorType.Chemistry});
            majors.Add(new Major { Id = Guid.NewGuid(), Name = MajorType.History });
            ReturnValue retValUpdate = BizLayer.ManagerClasses.UserManager.UpdateUser(user, majors);
            Assert.AreEqual(true, retValUpdate.Success);

            var majors2 = new List<Major>();
            majors2.AddRange(BizLayer.ManagerClasses.UserManager.GetMajorsByStudentId(user.Id));
            foreach (var major in majors2)
            {
                Assert.IsTrue(major.Name == MajorType.Chemistry || major.Name == MajorType.History);
            }

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            using (var db = new CmsModelContainer())
            {
                db.Users.Attach(userToDelete);
                foreach (var major in majors)
                {
                    userToDelete.Majors.Remove(major);
                }
                db.SaveChanges();
            }
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestUserGetIsActiveStatus()
        {
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            User user;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user);
            Assert.IsTrue(user.IsActive);

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }
    }
}
