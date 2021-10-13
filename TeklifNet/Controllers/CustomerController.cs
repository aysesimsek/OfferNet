using System;
using System.Linq;
using System.Web.Mvc;
using TeklifNet.Models;
using System.Data;
using System.Web;
using System.IO;

namespace Deneme.Controllers
{
    public class CustomerController : Controller
    {
        #region Show Cari Detayları
        [HttpGet]
        [Authorize]
        public ActionResult Show_Customer()
        {
            int UserID = Convert.ToInt32(User.Identity.Name);
            using (eteklifn_netEntities dc = new eteklifn_netEntities())
            {
                var user = dc.TBLKULLANP.Where(a => a.ID == UserID).FirstOrDefault();
                var customers = dc.TBLCASABIT.Where(a => a.SIRKET_ID == user.SIRKET_ID).ToList();
                return Json(new { data = customers }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_CustomerView()
        {
          
            return View();

        }
        #endregion

        #region Delete Cari ve Cari Detayları
        [Authorize]
        [HttpPost]
        public ActionResult Delete_Customer(int id)
        {
            using (eteklifn_netEntities db = new eteklifn_netEntities())
            {
                try
                {
                    TBLCASABIT cari = db.TBLCASABIT.Where(x => x.CARI_ID == id).FirstOrDefault<TBLCASABIT>();
                    db.TBLCASABIT.Remove(cari);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Kayıt başarıyla silindi." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "İşlem, başka bir işlem tarafından kullanılıyor." }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region Add or Edit Customer
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_Customer(int id = 0)
        {
            if (id == 0)
                return View(new TBLCASABIT());
            else
            {
                using (eteklifn_netEntities db = new eteklifn_netEntities())
                {
                    return View(db.TBLCASABIT.Where(x => x.CARI_ID == id).FirstOrDefault<TBLCASABIT>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_Customer(HttpPostedFileBase image, TBLCASABIT customer)
        {
            int UserID = Convert.ToInt32(User.Identity.Name);
            using ( eteklifn_netEntities db = new eteklifn_netEntities())
            {
                var user = db.TBLKULLANP.Where(a => a.ID == UserID).FirstOrDefault();
                customer.SIRKET_ID = user.SIRKET_ID;             
                if (customer.CARI_ID == 0)
                {
                    db.TBLCASABIT.Add(customer);
                    customer.KAYITYAPANKULL = user.KULL_KODU;
                    customer.KAYITTARIHI = DateTime.Now.ToString();
                    db.SaveChanges();
                    return Json(new { success = true, message = "Kayıt başarıyla kaydedildi." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    customer.DUZELTMEYAPANKULL = user.KULL_KODU;
                    customer.DUZELTMETARIHI = DateTime.Now.ToString();
                    db.SaveChanges();
                    return Json(new { success = true, message = "Kayıt başarıyla güncellendi." }, JsonRequestBehavior.AllowGet);
                }
            }


        }
        #endregion

    }
}