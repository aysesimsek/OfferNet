using System;
using System.Linq;
using System.Web.Mvc;
using TeklifNet.Models;
namespace TeklifNet.Controllers
{
    public class StockController : Controller
    {
        #region Show stock details
        [HttpGet]
        [Authorize]
        public ActionResult Show_Stock()
        {
            int UserID = Convert.ToInt32(User.Identity.Name);
            using (eteklifn_netEntities dc = new eteklifn_netEntities())
            {
                var user = dc.TBLKULLANP.Where(a => a.ID == UserID).FirstOrDefault();
                var stock = dc.TBLSTSABIT.Where(a => a.SIRKET_ID == user.SIRKET_ID).ToList();
                return Json(new { data = stock }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_StockView()
        {

            return View();

        }
        #endregion

        #region Delete stock details
        [Authorize]
        [HttpPost]
        public ActionResult Delete_Stock(int id)
        {
            using (eteklifn_netEntities db = new eteklifn_netEntities())
            {
                try
                {
                    TBLSTSABIT stok = db.TBLSTSABIT.Where(x => x.STOK_ID == id).FirstOrDefault<TBLSTSABIT>();
                    db.TBLSTSABIT.Remove(stok);
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

        #region Add or Edit stock details
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_Stock(int id = 0)
        {
            if (id == 0)
                return View(new TBLSTSABIT());
            else
            {
                using (eteklifn_netEntities db = new eteklifn_netEntities())
                {
                    return View(db.TBLSTSABIT.Where(x => x.STOK_ID == id).FirstOrDefault<TBLSTSABIT>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_Stock(TBLSTSABIT stock)
        {
            int UserID = Convert.ToInt32(User.Identity.Name);
            using (eteklifn_netEntities db = new eteklifn_netEntities())
            {
                var user = db.TBLKULLANP.Where(a => a.ID == UserID).FirstOrDefault();
                stock.SIRKET_ID = user.SIRKET_ID;
                stock.KAYITTARIHI = DateTime.Now.ToString();
                if (stock.STOK_ID == 0)
                {
                    db.TBLSTSABIT.Add(stock);
                    stock.KAYITYAPANKULL = user.KULL_KODU;
                    stock.KAYITTARIHI = DateTime.Now.ToString();
                    db.SaveChanges();
                    return Json(new { success = true, message = "Kayıt başarıyla kaydedildi." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                    stock.DUZELTMEYAPANKULL = user.KULL_KODU;
                    stock.DUZELTMETARIHI = DateTime.Now.ToString();
                    db.SaveChanges();
                    return Json(new { success = true, message = "Kayıt başarıyla güncellendi." }, JsonRequestBehavior.AllowGet);
                }
            }


        }
        #endregion
    }
}