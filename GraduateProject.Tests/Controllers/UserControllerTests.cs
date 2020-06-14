using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraduateProject.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduateProject.Models;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Web.Security;
using System.Data.Entity.Migrations;

namespace GraduateProject.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        MyDatabaseEntities db = new MyDatabaseEntities();

        [TestMethod()]
        public void UserController()
        {

            Assert.IsNotNull(db);
        }

        [TestMethod()]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailfor = "VerifyAccount")
        {
            //string emailID = "wesamhihi3@gmail.com";
            //string activationCode = Guid.NewGuid().ToString();
            //string emailfor = "VerifyAccount";
            string subject = "";
            string body = "";

            var verifyUrl = "/User/" + emailfor + "/" + activationCode;
            string link = " https://localhost:44309/User/VerifyAccount/f4a5a670-d2e7-4f5c-a2b0-d804b36512fa";
            ///  var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
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
            Assert.IsNotNull(body);
        }

        [TestMethod()]
        public bool IsEmailExist(string email)
        {

            var v = db.Users.Where(a => a.EmailID == "email").FirstOrDefault();
            string state;
            if (v == null)
                state = "exist";
            else
                state = "new";
            Assert.IsNotNull(state);
            return v != null;
        }

        [TestMethod()]
        public void Login()
        {
            UserLogin login = new UserLogin();
            login.EmailID = "wesamhihi3@gmail.com";
            login.Password = "wesam1998@";
            string message = "";
            var v = db.Users.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
            if (v != null)
            {
                if (!v.IsEmailVerfied)
                {
                    message = "Please verify your email first";
                    Assert.IsNotNull(message);
                }
                if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                {
                    int timeout = login.RememberMe ? 525600 : 20;//525600 min = 1year
                    var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;

                    message = "login successfully";

                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            else
            {
                message = "Email Not found";
            }


            Console.WriteLine(message);
            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void Sign()
        {
            User user = new User();
            user.FirstName = "r";
            user.LastName = "rr";
            user.EmailID = "wesamh@yahoo.com";
            user.Password = "wesam98@";
            user.ConfirmPassword = "wesam98@";

            bool Status = false;
            string message = "";
            string succ;
            //
            // Model Validation 


            #region //Email is already Exist 
            var isExist = IsEmailExist(user.EmailID);
            if (isExist)
            {
                succ = "Email already exist";
                Assert.IsNotNull(succ);
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
            db.Users.Add(user);
            db.SaveChanges();

            //Send Email to User
            SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
            message = "Registration successfully done. Account activation link " +
                " has been sent to your email id:" + user.EmailID;
            Status = true;
            #endregion


            Assert.IsNotNull(user);
        }

        [TestMethod()]
        public void ForPassword()
        {
            //Verfiy Email ID
            //Generate Reset Password
            //Send Email
            string message = "";
            //bool Status = false;

            var account = db.Users.Where(a => a.EmailID == "wesamhihi3@gmail.com").FirstOrDefault();
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

            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void Resetpass()
        {
            ResetPasswordModel message;
            string s;
            string id = "f895e05b-4352-42a5-b2c3-3a6a0ef8e6a2";
            var user = db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                message = model;
                s = "ss";
            }
            else
            { s = "db"; }

            Console.WriteLine(s);
            Assert.IsNotNull(s);
        }

        [TestMethod()]
        public void VerifyAccount()
        {
            string message;
            bool Status = false;

            db.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                            // Confirm password does not match issue on save changes
            var v = db.Users.Where(a => a.ActivationCode == new Guid()).FirstOrDefault();
            if (v != null)
            {
                v.IsEmailVerfied = true;
                db.SaveChanges();
                Status = true;
                message = "sucissful Verify Account";
            }
            else
            {
                message = "Invalid Request";
            }

            Assert.Fail();
        }

        [TestMethod()]
        public void UserInfo()
        {
            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == "wesamhihi3@gmail.com");

            Console.WriteLine(u.FirstOrDefault().FirstName);
            Assert.IsNotNull(u);
        }

        [TestMethod()]
        public void ChangeEmail()
        {
            string message = "change done";
            string email = "wesamhihi3@gmail.com";
            string password = "wesam1998@";
            string p = Crypto.Hash(password);
            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == "wesamhihi@yahoo.com");
            foreach (var item in u)
            {
                if (item.Password != p)
                {
                    message = "Wrong password";
                }
                else
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
                        var user111 = dc.Users.Where(b => b.EmailID == "wesamhihi@yahoo.com");
                        foreach (var ii in user111)
                        {
                            dc.Users.Remove(ii);
                        }
                        dc.SaveChanges();
                    }


                }

            }
            Console.WriteLine(message);
            Assert.IsNotNull(message);
        }

        [TestMethod()]
        public void UpdatePassword()
        {
            string message = "changed";
           string currontpassword = Crypto.Hash("wesam1998@");
           string Newpassword = Crypto.Hash("wesam98@");
           string  confirmPassword = Crypto.Hash("wesam98@");

            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == "wesamhihi3@gmail.com");
            foreach (var item in u)
            {
                if (item.Password != currontpassword)
                {
                    message = "Wrong password";
                }
                else
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
                            var user111 = dc.Users.Where(b => b.Password == currontpassword && b.EmailID == "wesamhihi3@gmail.com");
                            foreach (var ii in user111)
                            {
                                dc.Users.Remove(ii);
                            }
                            dc.SaveChanges();
                        }
                    }


                }

            Console.WriteLine(message);
            Assert.IsNotNull(message);
        }
    }
}