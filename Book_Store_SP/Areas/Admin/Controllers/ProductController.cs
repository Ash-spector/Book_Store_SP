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
        private readonly ISP_CALL _spcall;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ISP_CALL spcall, IWebHostEnvironment webHostEnvironment)
        {
            _spcall = spcall;
            _webHostEnvironment = webHostEnvironment;
        }

        // ✅ CATEGORY STYLE (AJAX)
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _spcall.List<Category>(SD.Proc_Category_GetAll)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                CoverTypeList = _spcall.List<CoverType>(SD.Proc_CoverType_GetAll)
                    .Select(ct => new SelectListItem
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    })
            };

            if (id == null)
                return View(productVM);

            var param = new DynamicParameters();
            param.Add("id", id.GetValueOrDefault());

            productVM.Product = _spcall.OneRecord<Product>(SD.Proc_Product_GetOne, param);

            if (productVM.Product == null)
                return NotFound();

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            // ✅ FIXED VALIDATION ISSUES
            ModelState.Remove("Product.ImageUrl");
            ModelState.Remove("Product.Category");
            ModelState.Remove("Product.CoverType");

            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                string imageUrl = productVM.Product.ImageUrl;

                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"images/products");

                    // ✅ ENSURE FOLDER EXISTS
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    // ✅ DELETE OLD IMAGE
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var oldImagePath = Path.Combine(webRootPath, imageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStream = new FileStream(
                        Path.Combine(uploads, fileName + extension),
                        FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // ✅ KEEP OLD IMAGE (ONLY IF EDIT)
                    if (productVM.Product.Id != 0)
                    {
                        var param2 = new DynamicParameters();
                        param2.Add("id", productVM.Product.Id);

                        var existing = _spcall.OneRecord<Product>(
                            SD.Proc_Product_GetOne, param2);

                        productVM.Product.ImageUrl = existing?.ImageUrl;
                    }
                }

                // ✅ PARAMS FOR SP
                var param = new DynamicParameters();
                param.Add("title", productVM.Product.Title);
                param.Add("description", productVM.Product.Description);
                param.Add("author", productVM.Product.Author);
                param.Add("isbn", productVM.Product.ISBN);
                param.Add("listPrice", productVM.Product.ListPrice);
                param.Add("price", productVM.Product.Price);
                param.Add("price50", productVM.Product.Price50);
                param.Add("price100", productVM.Product.Price100);
                param.Add("imageUrl", productVM.Product.ImageUrl);
                param.Add("categoryId", productVM.Product.CategoryId);
                param.Add("coverTypeId", productVM.Product.CoverTypeId);

                if (productVM.Product.Id == 0)
                {
                    _spcall.Execute(SD.Proc_Product_Create, param);
                }
                else
                {
                    param.Add("id", productVM.Product.Id);
                    _spcall.Execute(SD.Proc_Product_Update, param);
                }

                return RedirectToAction("Index");
            }

            // ✅ RELOAD DROPDOWNS (IMPORTANT)
            productVM.CategoryList = _spcall.List<Category>(SD.Proc_Category_GetAll)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            productVM.CoverTypeList = _spcall.List<CoverType>(SD.Proc_CoverType_GetAll)
                .Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                });

            return View(productVM);
        }

        #region APIs

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new
            {
                data = _spcall.List<Product>(SD.Proc_Product_GetAll)
            });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("id", id);

            var product = _spcall.OneRecord<Product>(SD.Proc_Product_GetOne, param);

            if (product == null)
                return Json(new { success = false, message = "Delete failed" });

            // ✅ DELETE IMAGE
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, product.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _spcall.Execute(SD.Proc_Product_Delete, param);

            return Json(new { success = true, message = "Deleted successfully" });
        }

        #endregion
    }
}