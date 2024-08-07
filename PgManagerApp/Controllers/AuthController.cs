﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace PgManagerApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        public AuthController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(MasterUser model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.MasterUser.Any(u => u.Email == model.Email))
                {
                    TempData["Error"] = $"{model.Email} already in use";
                    return RedirectToAction("Register");
                }

                model.PasswordHash = HashPassword(model.PasswordHash);
                _context.MasterUser.Add(model);
                _context.SaveChanges();
                TempData["Message"] = $"User account has been created, you can log in now.";
                return RedirectToAction("Login");
            }
            return View("Register", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.MasterUser
                    .SingleOrDefault(u => u.Email == model.Email && u.PasswordHash == HashPassword(model.Password));

                if (user != null)
                {
                    HttpContext.Session.SetInt32("MasterUserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Name);
                    _cache.Set("Username", user.Name);
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    TempData["Error"] = "Invalid email or password";
                }
                
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
