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
    public class CreateAndUpdateUserTests
    {
        [TestMethod]
        public void TestUserCreate()
        {
            // create new user & check if successful
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            Assert.AreEqual(true, retValCreate.Success);
            
            // remove user after test
            User user;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user);
            BizLayer.ManagerClasses.UserManager.DeleteUser(user);
        }

        [TestMethod]
        public void TestUserEdit()
        {
            // create new user, edit user, & check if successful
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            Assert.AreEqual(true, retValCreate.Success);

            User user1;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user1);
            Assert.IsNotNull(user1);
            Assert.AreNotEqual(Guid.Empty, user1.Id);

            var majors = new List<Major>();
            majors.AddRange(BizLayer.ManagerClasses.UserManager.GetMajorsByStudentId(user1.Id));

            user1.FirstName = "unit_test_first_name_edited";
            user1.LastName = "unit_test_last_name_edited";
            user1.Role = RoleType.instructor;
            ReturnValue retValUpdate = BizLayer.ManagerClasses.UserManager.UpdateUser(user1, majors);
            Assert.AreEqual(true, retValUpdate.Success);

            User user2;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user2);
            Assert.AreEqual("unit_test_first_name_edited", user2.FirstName);
            Assert.AreEqual("unit_test_last_name_edited", user2.LastName);
            Assert.AreEqual(RoleType.instructor, user2.Role);

            // remove user after test
            User user3;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user3);
            BizLayer.ManagerClasses.UserManager.DeleteUser(user3);
        }
    }
}
