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
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var coverTypes = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_CoverType_GetAll);
            return View(coverTypes);
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if (id == null || id == 0)
                return View(coverType);

            var param = new DynamicParameters();
            param.Add("@id", id);
            coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_CoverType_GetOne, param);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            var param = new DynamicParameters();

            if (obj.Id == 0)
            {
                param.Add("@name", obj.Name);
                _unitOfWork.SP_CALL.Execute(SD.Proc_CoverType_Create, param);
                TempData["success"] = "CoverType created successfully";
            }
            else
            {
                param.Add("@id", obj.Id);
                param.Add("@name", obj.Name);
                _unitOfWork.SP_CALL.Execute(SD.Proc_CoverType_Update, param);
                TempData["success"] = "CoverType updated successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypes = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_CoverType_GetAll);
            return Json(new { data = coverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@id", id);

            var coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_CoverType_GetOne, param);

            if (coverType == null)
                return Json(new { success = false, message = "Unable to delete!" });

            _unitOfWork.SP_CALL.Execute(SD.Proc_CoverType_Delete, param);
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}