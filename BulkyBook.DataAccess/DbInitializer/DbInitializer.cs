﻿using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            //apply migrations if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count()>0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex)
            {

            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    Name = "Admin",
                    PhoneNumber = "0606060606",
                    StreetAdress = "1 rue test",
                    State = "Test",
                    PostalCode = "31000",
                    City = "Toulouse"
                },"Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@admin.com");

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}