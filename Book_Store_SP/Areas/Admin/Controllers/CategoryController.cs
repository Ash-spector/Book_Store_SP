using Book_Store_SP.DataAccess.Repository.IRepository;
using Book_Store_SP.Models;
using Book_Store_SP.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Book_Store_SP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly ISP_CALL _spcall;

        public CoverTypeController(ISP_CALL spcall)
        {
            _spcall = spcall;
        }

        // ✅ SAME AS CATEGORY (no data passed)
        public IActionResult Index()
        {
            return View();
        }

        // ✅ SAME STRUCTURE AS CATEGORY
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if (id == null)
                return View(coverType);

            DynamicParameters param = new DynamicParameters();
            param.Add("id", id.GetValueOrDefault());

            coverType = _spcall.OneRecord<CoverType>(SD.Proc_CoverType_GetOne, param);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        [HttpPost]
        public IActionResult Upsert(CoverType obj)
        {
            if (obj == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(obj);

            DynamicParameters param = new DynamicParameters();
            param.Add("name", obj.Name);

            if (obj.Id == 0)
            {
                _spcall.Execute(SD.Proc_CoverType_Create, param);
            }
            else
            {
                param.Add("id", obj.Id);
                _spcall.Execute(SD.Proc_CoverType_Update, param);
            }

            return RedirectToAction("Index");
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new
            {
                data = _spcall.List<CoverType>(SD.Proc_CoverType_GetAll)
            });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id);

            var coverTypeInDb = _spcall.OneRecord<CoverType>(SD.Proc_CoverType_GetOne, param);

            if (coverTypeInDb == null)
            {
                return Json(new { success = false, message = "Delete failed" });
            }

            _spcall.Execute(SD.Proc_CoverType_Delete, param);

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}