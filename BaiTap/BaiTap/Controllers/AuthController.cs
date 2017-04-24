using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTap.Models;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity.Validation;

namespace BaiTap.Controllers
{
    public class AuthController : Controller
    {
        OnlineStoreEntities db = new OnlineStoreEntities();
        public const string SALT = @"zrRu^Wq>NI7?=]e1Y`@PjX/]+Kjl\)POEgIIl(B5%J:%ow&;<87e]2;Ske3>&+7[";
        // GET: Auth return login page
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["auth"] == null)
                return View();
            return RedirectToAction("Index", "Home");
        }

        //POST 
        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Password = ToHashCodeByMD5(SALT, viewModel.Password);
                User account = db.Users.FirstOrDefault(m => m.Email == viewModel.Email && m.Password == viewModel.Password);
                if (account != null)
                {
                    Session["auth"] = account;
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Notification = "username or password is incorrect";
                return View();
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["auth"] = null;
            return RedirectToAction("Index", "Home");
        }

        //get register page
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        /// To hash code by MD5
        /// </summary>
        /// <param name="salt">salt string</param>
        /// <param name="code">code string</param>
        /// <returns>string</returns>
        public static string ToHashCodeByMD5(string salt, string code)
        {
            try
            {
                // Convert saft string to byte
                var saltByte = Encoding.UTF8.GetBytes(salt);

                // Convert data string to byte
                var codeByte = Encoding.UTF8.GetBytes(code);

                // Hash password with MD5
                var hmacMD5 = new HMACMD5(saltByte);
                var saltedHash = hmacMD5.ComputeHash(codeByte);
                var hashLoginKey = Encoding.UTF8.GetString(saltedHash);

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < saltedHash.Length; i++)
                {
                    sBuilder.Append(saltedHash[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isExist = db.Users.Any(m => m.Email == viewModel.Email);
                    if (isExist) //email is neutral
                    {
                        ViewBag.Notice = "email is neutral";
                        return View();
                    }

                    var user = new User();
                    user.UserName = viewModel.UserName;
                    user.Email = viewModel.Email;
                    user.Phone = viewModel.Phone;
                    user.Address = viewModel.Address;
                    user.Password = ToHashCodeByMD5(SALT, viewModel.Password);
                    user.CreatedAt = DateTime.Now;
                    db.Users.Add(user);
                    db.SaveChanges();
                    //ViewBag.Notice = "Registration Successful";
                    return RedirectToAction("Login");
                }
                return View();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
    }
}