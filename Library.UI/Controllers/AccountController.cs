using Library.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Library.UI.Controllers
{
    public class AccountController : Controller
    {
        readonly private Library.Service.ILibraryService service;

        public AccountController(Library.Service.ILibraryService service)
        {
            this.service = service;
        }
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (service.RegistrationNewUser(user))
                    {
                        Session["email"] = user.Email;
                        FormsAuthentication.SetAuthCookie(user.Email, false);

                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Library", ex.Message);
            }
         
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            // Lets first check if the Model is valid or not
            if (ModelState.IsValid)
            {

                string userMail = model.Email;

                // Now if our password was enctypted or hashed we would have done the
                // same operation on the user entered password here, But for now
                // since the password is in plain text lets just authenticate directly

                bool userValid = service.Login(model);

                // User found in the database
                if (userValid)
                {

                    FormsAuthentication.SetAuthCookie(userMail, false);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        Session["email"] = userMail;
                        return RedirectToAction("Index", "Library");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user email is incorrect.");
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session["email"] = null;
            return RedirectToAction("Index", "Library");
        }
    }
}