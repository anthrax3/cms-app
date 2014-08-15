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
    public class ActivateDeactivateUserTests
    {
        [TestMethod]
        public void TestDeactivateUser()
        {
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);
            
            User user1;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user1);
            Assert.IsTrue(user1.IsActive); // yes, is active

            ReturnValue retValDeactivate = BizLayer.ManagerClasses.UserManager.DeactivateUser(user1);
            Assert.IsTrue(retValDeactivate.Success);

            User user2;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user2);
            Assert.IsFalse(user2.IsActive); // no, is not active

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }

        [TestMethod]
        public void TestActivateUser()
        {
            ReturnValue retValCreate = BizLayer.ManagerClasses.UserManager.CreateUser(
                "unit_test_first_name",
                "unit_test_last_name",
                "unit_test_email@test.com",
                true,
                RoleType.student);

            User user1;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user1);
            Assert.IsTrue(user1.IsActive); // yes, is active

            ReturnValue retValDeactivate = BizLayer.ManagerClasses.UserManager.DeactivateUser(user1);
            Assert.IsTrue(retValDeactivate.Success);

            User user2;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user2);
            Assert.IsFalse(user2.IsActive); // no, is not active

            ReturnValue retValActivate = BizLayer.ManagerClasses.UserManager.ActivateUser(user2);
            Assert.IsTrue(retValActivate.Success);

            User user3;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out user3);
            Assert.IsTrue(user3.IsActive); // yes, is active

            // remove user after test
            User userToDelete;
            BizLayer.ManagerClasses.UserManager.GetUserByEmailAddress("unit_test_email@test.com", out userToDelete);
            BizLayer.ManagerClasses.UserManager.DeleteUser(userToDelete);
        }
    }
}
