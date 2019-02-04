using System;
using System.Web;
using CellGame.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;

namespace CellGame
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesAndUsers();
            DatabaseUtils.getUserRoles();
        }

        private void createRolesAndUsers()
        {

            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.Email = "admin@test.com";
                user.UserName = "admin@test.com";
                user.LockoutEnabled = true;

                string userPWD = "Test_123";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            if (!roleManager.RoleExists("Student"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.Email = "student@test.com";
                user.UserName = "student@test.com";
                user.LockoutEnabled = true;

                string userPWD = "Test_123";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Student");

                }
            }
            if (!roleManager.RoleExists("Professor"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Professor";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.Email = "professor@test.com";
                user.UserName = "professor@test.com";
                user.LockoutEnabled = true;

                string userPWD = "Test_123";

                var chkUser = UserManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Professor");

                }
            }
            if (!roleManager.RoleExists("TA"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "TA";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.Email = "ta@test.com";
                user.UserName = "ta@test.com";
                user.LockoutEnabled = true;

                string userPWD = "Test_123";

                var chkUser = UserManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "TA");

                }
            }

        }
    }

}
