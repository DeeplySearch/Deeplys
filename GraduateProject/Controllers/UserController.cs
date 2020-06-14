using GraduateProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Web.Security;
using System.Data.Entity.Migrations;


namespace GraduateProject.Controllers
{
    public class UserController : Controller
    {
        MyDatabaseEntities db;

        public UserController()
        {
            db = new MyDatabaseEntities();
        }

        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailfor = "VerifyAccount")
        {
            string subject = "", body = "";
            var verifyUrl = "/User/" + emailfor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("deeplyproject282@gmail.com", "Deeply");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "ghazi1997@";
            subject = "Your account is successfully created!";
            body = "<br/><br/>We are excited to tell you that your Deeply account is" +
           " successfully created. Please click on the below link to verify your account" +
           "<br/><br/><a href=" + link + ">" + link + "</a>";
            if (emailfor.Equals("ResetPass"))
            {
                subject = "Change Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                "<br/><br/><a href=" + link + ">" + link
                + "</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
             using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        public bool IsEmailExist(string emailID)
        {
            using (MyDatabaseEntities db = new MyDatabaseEntities())
            {
                var v = db.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }

        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            using (MyDatabaseEntities db = new MyDatabaseEntities())
            {
                var v = db.Users.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                if (v != null)
                {
                    if (!v.IsEmailVerfied)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20;//525600 min = 1year
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();

        }
        [HttpGet]
        public ActionResult Sign()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                #endregion
                user.IsEmailVerfied = false;

                #region Save to Database
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    //Send Email to User
                    SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + user.EmailID;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        public ActionResult ForPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForPassword(string EmailID)
        {
            //Verfiy Email ID
            //Generate Reset Password
            //Send Email
            string message = "";
            //bool Status = false;
            using (MyDatabaseEntities db = new MyDatabaseEntities())
            {
                var account = db.Users.Where(a => a.EmailID == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetcode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EmailID, resetcode, "ResetPass");
                    account.ResetPasswordCode = resetcode;
                    //The line below to avoid confirm password not match issue
                    db.Configuration.ValidateOnSaveEnabled = false;

                    db.SaveChanges();
                    message = "Reset password link has been sent to your Email Id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        public ActionResult Resetpass(string Id)
        {
            using (MyDatabaseEntities db = new MyDatabaseEntities())
            {
                var user = db.Users.Where(a => a.ResetPasswordCode == Id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = Id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resetpass(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (MyDatabaseEntities db = new MyDatabaseEntities())
                {
                    var user = db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "New Password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerfied = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public ActionResult UserInfo()
        {
            MyDatabaseEntities db = new MyDatabaseEntities();
            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == User.Identity.Name);

            return View(u);
        }

        public ActionResult ChangeEmail()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ChangeEmail(string email, string password)
        {

            string p = Crypto.Hash(password);
            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == User.Identity.Name);
            foreach (var item in u)
            {
                if (item.Password != p)
                {
                    return View("Wrong password");
                }
                else
                {
                    if (ModelState.IsValid)
                    {

                        User user = new User();

                        foreach (var l in u)
                        {

                            user = l;
                        }
                        user.EmailID = email;
                        user.Password = p;
                        user.ConfirmPassword = p;
                        using (MyDatabaseEntities dc = new MyDatabaseEntities())
                        {
                            dc.Users.Add(user);
                            dc.SaveChanges();
                            var user111 = dc.Users.Where(b => b.EmailID == User.Identity.Name);
                            foreach (var ii in user111)
                            {
                                dc.Users.Remove(ii);
                            }
                            dc.SaveChanges();
                        }
                    }


                }

            }
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");

        }


        public ActionResult UpdatePassword()
        {

            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassword(string currontpassword, string Newpassword, string confirmPassword)
        {

            currontpassword = Crypto.Hash(currontpassword);
            Newpassword = Crypto.Hash(Newpassword);
            confirmPassword = Crypto.Hash(confirmPassword);

            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == User.Identity.Name);
            foreach (var item in u)
            {
                if (item.Password != currontpassword)
                {
                    return View("Wrong password");
                }
                else
                {
                    if (ModelState.IsValid)
                    {

                        User user = new User();

                        foreach (var l in u)
                        {

                            user = l;
                        }
                        user.Password = Newpassword;
                        user.ConfirmPassword = confirmPassword;
                        using (MyDatabaseEntities dc = new MyDatabaseEntities())
                        {
                            dc.Users.Add(user);
                            dc.SaveChanges();
                            var user111 = dc.Users.Where(b => b.Password == currontpassword && b.EmailID == User.Identity.Name);
                            foreach (var ii in user111)
                            {
                                dc.Users.Remove(ii);
                            }
                            dc.SaveChanges();
                        }
                    }


                }

            }
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");

        }





    }
}