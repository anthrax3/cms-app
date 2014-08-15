using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

// created by Charles Drews
namespace CMS_App.Models
{
    public static class IdentityModelsHelper
    {
        /// <summary>
        /// Create a role in the Identity system if it is not already present
        /// </summary>
        /// <param name="roleName"></param>
        public static void AddIdentityRoleIfNotExists(string roleName)
        {
            using (var db = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                if (!roleManager.RoleExists(roleName))
                {
                    roleManager.Create(new IdentityRole(roleName));
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Add user to role in the Identity system
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="roleName"></param>
        public static void UpdateIdentityUserRole(string userEmail, string roleName)
        {
            using (var db = new ApplicationDbContext())
            {
                AddIdentityRoleIfNotExists(roleName);
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = userManager.FindByName(userEmail); // note that the user name is their email address
                if (user != null)
                {
                    // only want one role per user, not multiple
                    // so remove previous roles before adding new one
                    foreach (var role in userManager.GetRoles(user.Id))
                    {
                        userManager.RemoveFromRole(user.Id, role);
                    }
                    userManager.AddToRole(user.Id, roleName);
                }
                db.SaveChanges();
            }
        }

        /* Note that there are zero references to DeleteIdentityUser(). This is purposeful. The original idea
         * was to delete an Identity entry when the corresponding User entry was deleted from our database.
         * Instead though, we will simply set the IsActive property for a User in our database to false (and
         * not actually delete them) so there is no need to delete the corresponding Identity entry.
         */
        /// <summary>
        /// Delete a user from the Identity system
        /// </summary>
        /// <param name="userEmail"></param>
        public static void DeleteIdentityUser(string userEmail)
        {
            using (var db = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var user = userManager.FindByName(userEmail); // note that the user name is their email address
                if (user != null)
                {
                    userManager.Delete(user);
                }
                db.SaveChanges();
            }
        }
    }
}