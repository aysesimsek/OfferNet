using mailinblue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using TeklifNet.Models;

namespace TeklifNet.Controllers
{
    public class UserController : Controller
    {
        #region Register
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsValid,ActivationLink")] Registration reg)
        {
            TBLSIRKETLER s = new TBLSIRKETLER();
            TBLKULLANP user = new TBLKULLANP();


            bool Status = false;
            string message = "";

            // Model Validation 
            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = IsEmailExist(reg.MAIL);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    message = "Bu email adresi zaten kullanılmaktadır.";
                    ViewBag.Message = message;
                    ViewBag.Status = Status;
                    Status = false;
                    return View(reg);
                }
                #endregion



                user.KULL_ADI = reg.KULL_ADI;
                user.MAIL = reg.MAIL;

                #region Generate Activation Code and User Code
                user.AKTIVASYON_LINK = Guid.NewGuid();

           
                byte[] bytes = Encoding.ASCII.GetBytes(user.KULL_ADI);
                int result = BitConverter.ToInt32(bytes, 0);
                user.KULL_KODU = user.ID + Convert.ToString(result);
                #endregion

                #region  Password Hashing 
                user.SIFRE = Crypto.Hash(reg.SIFRE);
                reg.SIFREONAY = Crypto.Hash(reg.SIFREONAY);
                #endregion

                user.ISVALID = false;
                user.YETKI = "1";
                #region Save to Database
                using (eteklifn_netEntities dc = new eteklifn_netEntities())
                {

                   

                    #region Company

                    //company.SIRKET_YETKILI = user.KULL_ADI;
                    s.SIRKET_VERGINUMARASI = reg.SIRKET_VERGINUMARASI;
                    s.SIRKET_UNVANI = reg.SIRKET_UNVANI;
                    s.SIRKET_ADRES = reg.SIRKET_ADRES;
                    s.SIRKET_IL = reg.SIRKET_IL;
                   // company.Country = reg.Country;
                    s.SIRKET_FAX = reg.SIRKET_FAX;
                    //s.SIRKET_TELEFON = reg.SIRKET_TELEFON;
                    s.SIRKET_ILCE = reg.SIRKET_ILCE;
                    s.SIRKET_VERGIDAIRESI = reg.SIRKET_VERGIDAIRESI;
                    //company.IsDefault = true;

                    dc.TBLSIRKETLER.Add(s);
                    dc.SaveChanges();
                    #endregion


                    user.SIRKET_ID = s.SIRKET_ID;
                    dc.TBLKULLANP.Add(user);
                    dc.SaveChanges();

                    //Send Email to User
                    SendinBlue(user.ID, user.AKTIVASYON_LINK.ToString());
                    message = "Kayıt işlemi başarıyla tamamlandı. Hesap aktifleştirme linki " + user.MAIL + " adresine gönderildi.";
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Geçersiz İşlem";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (eteklifn_netEntities dc = new eteklifn_netEntities())
            {
                //var sub = dc.SubUser.Where(a => a.Email == emailID).FirstOrDefault();
                var v = dc.TBLKULLANP.Where(a => a.MAIL == emailID).FirstOrDefault();


                return v != null;
            }
        }
        #endregion

        #region Send Mail
        [NonAction]
        public void SendinBlue(int regID, string activationlink, string emailFor = "VerifyAccount")
        {
            using (eteklifn_netEntities dc = new eteklifn_netEntities())
            {
                string body = null;
                var regInfo = dc.TBLKULLANP.Where(x => x.ID == regID).FirstOrDefault();
                var VerifyUrl = "/User/" + emailFor + "/" + activationlink;
                var Link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, VerifyUrl);
                // var url = activationlink + "Register/Confirm?regId=" + regID;


                string subject = "";

                if (emailFor == "VerifyAccount")
                {
                    body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Activation" + ".cshtml");
                    subject = "Hesap Doğrulaması";

                    body = body.Replace("@ViewBag.ConfirmationLink", Link);
                    body = body.ToString();

                }
                else if (emailFor == "ResetPassword")
                {
                    body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Password" + ".cshtml");
                    subject = "Şifre Sıfırlama";
                    body = body.Replace("@ViewBag.ResetLink", Link);
                    body = body.Replace("@ViewBag.UserMail", regInfo.MAIL);
                    body = body.ToString();
                }



                API sendinBlue = new mailinblue.API("MxcTA0LGPkWmHI2g");
                Dictionary<string, Object> data = new Dictionary<string, Object>();
                Dictionary<string, string> to = new Dictionary<string, string>();


                to.Add(regInfo.MAIL, regInfo.MAIL);
                List<string> from_name = new List<string>();
                from_name.Add("bilgi@mutabakatci.com");
                from_name.Add("Yirmibeş Yazılım ve Danışmanlık");

                data.Add("to", to);
                data.Add("from", from_name);
                data.Add("subject", subject);
                data.Add("html", body);

                Object sendEmail = sendinBlue.send_email(data);
            }




        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (eteklifn_netEntities dc = new eteklifn_netEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                var v = dc.TBLKULLANP.Where(a => a.AKTIVASYON_LINK == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.ISVALID = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Geçersiz İşlem";
                }
            }
            ViewBag.Status = Status;
            return RedirectToAction("Login", "User");
        }
        #endregion

        #region Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login, string ReturnUrl = "", string message = "")
        {
            message = " ";
            using (eteklifn_netEntities dc = new eteklifn_netEntities())
            {
                var v = dc.TBLKULLANP.Where(a => a.MAIL == login.MAIL).FirstOrDefault();
                //var sub = dc.SubUser.Where(a => a.Email == login.Email).FirstOrDefault();


                if (v != null)
                {
                    //var company = dc.TBLSIRKETLER.Where(a => a.KullanıcıID == v.ID && a.IsDefault == true).FirstOrDefault();

                    if (v.ISVALID == false)
                    {
                        ViewBag.Message = "Lütfen e-posta adresinizi doğrulayınız.";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(login.SIFRE), v.SIFRE) == 0)
                    {

                        string Keys = v.ID.ToString(); /*+ "," + company.CompanyId.ToString() + "," + "1"*/
                        //Session["Yetki"] = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(Keys, false, Session.Timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        HttpContext.Response.AppendCookie(cookie);


                        //HttpCookie Scookie = new HttpCookie("UserOp");
                        //Scookie.Values["Yetki"] = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
                        //Scookie.Values["CompanyId"] = v.SIRKET_ID.ToString();
                        //Scookie.Values["isAdmin"] = "1";
                        //Scookie.Expires = DateTime.Now.AddMinutes(30);
                        //Scookie.HttpOnly = true;
                        //Response.Cookies.Add(Scookie);
                        //HttpContext.Response.AppendCookie(Scookie);

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
                        message = "Geçersiz Kimlik Bilgisi";
                    }
                }
            //    else if (sub != null)
            //    {
            //        //var company = dc.Company.Where(a => a.KullanıcıID == sub.Yetkili && a.IsDefault == true).FirstOrDefault();
            //        var company = dc.Yetki.Where(a => a.UserID == sub.ID && a.isDefault == true).FirstOrDefault();
            //        v = dc.SiteUser.Where(a => a.ID == sub.Yetkili).FirstOrDefault();
            //        var yetki = dc.Yetki.Where(a => a.UserID == sub.ID && a.CompanyId == company.CompanyId).FirstOrDefault();
            //        if (string.Compare(Crypto.Hash(login.Password), sub.Password) == 0)
            //        {

            //            string Keys = v.ID.ToString(); /*+ "," + company.CompanyId.ToString() + "," + "0" + "," + sub.ID.ToString()*/
            //            //Session["Yetki"] = yetki.Yetkiler;

            //            int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
            //            var ticket = new FormsAuthenticationTicket(Keys, login.RememberMe, timeout);
            //            string encrypted = FormsAuthentication.Encrypt(ticket);
            //            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
            //            cookie.Expires = DateTime.Now.AddMinutes(timeout);
            //            HttpContext.Response.Cookies.Add(cookie);
            //            cookie.HttpOnly = true;
            //            Response.Cookies.Add(cookie);


            //            HttpCookie Scookie = new HttpCookie("UserOp");
            //            Scookie.Values["Yetki"] = yetki.Yetkiler;
            //            Scookie.Values["CompanyId"] = company.CompanyId.ToString();
            //            Scookie.Values["isAdmin"] = "0";
            //            Scookie.Values["SubId"] = sub.ID.ToString();
            //            Scookie.Expires = DateTime.Now.AddMinutes(30);
            //            Scookie.HttpOnly = true;
            //            Response.Cookies.Add(Scookie);
            //            HttpContext.Response.AppendCookie(Scookie);


            //            if (Url.IsLocalUrl(ReturnUrl))
            //            {
            //                return Redirect(ReturnUrl);
            //            }
            //            else
            //            {

            //                return RedirectToAction("Index", "Home");
            //            }
            //        }
            //        else
            //        {
            //            message = @Deneme.Resource.InvalidCredential;
            //        }
            //    }
            //    else
            //    {
            //        message = @Deneme.Resource.InvalidCredential;
            //    }
            }
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            //Session.Abandon();
            return RedirectToAction("Login", "User");
        }
        #endregion
    }
}