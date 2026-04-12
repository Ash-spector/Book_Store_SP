using Book_Store_SP.DataAccess.Repository.IRepository;
using Book_Store_SP.Models;
using Book_Store_SP.Models.ViewModel;
using Book_Store_SP.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Book_Store_SP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.SP_CALL.List<Category>(SD.Proc_Category_GetAll)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                CoverTypeList = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_CoverType_GetAll)
                    .Select(ct => new SelectListItem
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    })
            };

            if (id == null || id == 0)
                return View(productVM);

            // Edit mode
            var param = new DynamicParameters();
            param.Add("@id", id);
            productVM.Product = _unitOfWork.SP_CALL.OneRecord<Product>(SD.Proc_Product_GetOne, param);

            if (productVM.Product == null)
                return NotFound();

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                obj.CategoryList = _unitOfWork.SP_CALL.List<Category>(SD.Proc_Category_GetAll)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });
                obj.CoverTypeList = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_CoverType_GetAll)
                    .Select(ct => new SelectListItem
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    });
                return View(obj);
            }

            // Handle image upload
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);

                // Delete old image if updating
                if (obj.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
            }

            var param = new DynamicParameters();

            if (obj.Product.Id == 0)
            {
                param.Add("@title", obj.Product.Title);
                param.Add("@description", obj.Product.Description);
                param.Add("@isbn", obj.Product.ISBN);
                param.Add("@author", obj.Product.Author);
                param.Add("@listPrice", obj.Product.ListPrice);
                param.Add("@price", obj.Product.Price);
                param.Add("@price50", obj.Product.Price50);
                param.Add("@price100", obj.Product.Price100);
                param.Add("@imageUrl", obj.Product.ImageUrl);
                param.Add("@categoryId", obj.Product.CategoryId);
                param.Add("@coverTypeId", obj.Product.CoverTypeId);
                _unitOfWork.SP_CALL.Execute(SD.Proc_Product_Create, param);
                TempData["success"] = "Product created successfully";
            }
            else
            {
                param.Add("@id", obj.Product.Id);
                param.Add("@title", obj.Product.Title);
                param.Add("@description", obj.Product.Description);
                param.Add("@isbn", obj.Product.ISBN);
                param.Add("@author", obj.Product.Author);
                param.Add("@listPrice", obj.Product.ListPrice);
                param.Add("@price", obj.Product.Price);
                param.Add("@price50", obj.Product.Price50);
                param.Add("@price100", obj.Product.Price100);
                param.Add("@imageUrl", obj.Product.ImageUrl);
                param.Add("@categoryId", obj.Product.CategoryId);
                param.Add("@coverTypeId", obj.Product.CoverTypeId);
                _unitOfWork.SP_CALL.Execute(SD.Proc_Product_Update, param);
                TempData["success"] = "Product updated successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.SP_CALL.List<Product>(SD.Proc_Product_GetAll);
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@id", id);

            var product = _unitOfWork.SP_CALL.OneRecord<Product>(SD.Proc_Product_GetOne, param);

            if (product == null)
                return Json(new { success = false, message = "Unable to delete!" });

            // Delete image if exists
            var wwwRootPath = _hostEnvironment.WebRootPath;
            if (product.ImageUrl != null)
            {
                var imagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _unitOfWork.SP_CALL.Execute(SD.Proc_Product_Delete, param);
            return Json(new { success = true, message = "Product deleted successfully" });
        }

        #endregion
    }
}