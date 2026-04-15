using Book_Store_SP.DataAccess.Repository.IRepository;
using Book_Store_SP.Models;
using Book_Store_SP.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Book_Store_SP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ISP_CALL _spcall;

        public CategoryController(ISP_CALL spcall)
        {
            _spcall = spcall;
        }

        // ✅ SERVER-SIDE DATA LOADING
        public IActionResult Index()
        {
            var categories = _spcall.List<Category>(SD.Proc_Category_GetAll);
            return View(categories);
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null || id == 0)
                return View(category);

            var param = new DynamicParameters();
            param.Add("id", id);

            category = _spcall.OneRecord<Category>(SD.Proc_Category_GetOne, param);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            var param = new DynamicParameters();

            if (obj.Id == 0)
            {
                param.Add("name", obj.Name);
                _spcall.Execute(SD.Proc_Category_Create, param);
                TempData["success"] = "Category created successfully";
            }
            else
            {
                param.Add("id", obj.Id);
                param.Add("name", obj.Name);
                _spcall.Execute(SD.Proc_Category_Update, param);
                TempData["success"] = "Category updated successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _spcall.List<Category>(SD.Proc_Category_GetAll);
            return Json(new { data = categories });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("id", id);

            var category = _spcall.OneRecord<Category>(SD.Proc_Category_GetOne, param);

            if (category == null)
                return Json(new { success = false, message = "Unable to delete!" });

            _spcall.Execute(SD.Proc_Category_Delete, param);

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}