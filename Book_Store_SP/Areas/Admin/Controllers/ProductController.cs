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

        public IActionResult Index()
        {
            return View();
        }

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

            if (id == null || id == 0)
                return View(productVM);

            var param = new DynamicParameters();
            param.Add("@id", id);
            productVM.Product = _spcall.OneRecord<Product>(SD.Proc_Product_GetOne, param);

            if (productVM.Product == null)
                return NotFound();

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            ModelState.Remove("Product.ImageUrl");
            ModelState.Remove("Product.Name");
            ModelState.Remove("Product.Category");
            ModelState.Remove("Product.CoverType");

            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, "images", "products");

                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    if (productVM.Product.Id != 0)
                    {
                        var param2 = new DynamicParameters();
                        param2.Add("@id", productVM.Product.Id);
                        var existing = _spcall.OneRecord<Product>(
                            SD.Proc_Product_GetOne, param2);

                        if (existing?.ImageUrl != null)
                        {
                            var oldPath = Path.Combine(webRootPath,
                                existing.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }
                    }

                    using (var fileStream = new FileStream(
                        Path.Combine(uploads, fileName + extension),
                        FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\products\"
                        + fileName + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var param2 = new DynamicParameters();
                        param2.Add("@id", productVM.Product.Id);
                        var existing = _spcall.OneRecord<Product>(
                            SD.Proc_Product_GetOne, param2);
                        productVM.Product.ImageUrl = existing?.ImageUrl;
                    }
                }

                productVM.Product.Name = productVM.Product.Title;

                var param = new DynamicParameters();

                if (productVM.Product.Id == 0)
                {
                    param.Add("title", productVM.Product.Title);
                    param.Add("description", productVM.Product.Description);
                    param.Add("author", productVM.Product.Author);      // author BEFORE isbn
                    param.Add("isbn", productVM.Product.ISBN);          // isbn AFTER author
                    param.Add("listPrice", productVM.Product.ListPrice);
                    param.Add("price", productVM.Product.Price);
                    param.Add("price50", productVM.Product.Price50);
                    param.Add("price100", productVM.Product.Price100);
                    param.Add("imageUrl", productVM.Product.ImageUrl);
                    param.Add("categoryId", productVM.Product.CategoryId);
                    param.Add("coverTypeId", productVM.Product.CoverTypeId);

                    try
                    {
                        _spcall.Execute(SD.Proc_Product_Create, param);
                        TempData["success"] = "Product created successfully";
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = ex.Message;
                        System.Diagnostics.Debug.WriteLine("SP ERROR: " + ex.Message);
                        // Reload dropdowns
                        productVM.CategoryList = _spcall.List<Category>(SD.Proc_Category_GetAll)
                            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                        productVM.CoverTypeList = _spcall.List<CoverType>(SD.Proc_CoverType_GetAll)
                            .Select(ct => new SelectListItem { Text = ct.Name, Value = ct.Id.ToString() });
                        return View(productVM);
                    }
                }
                else
                {
                    param.Add("title", productVM.Product.Title);
                    param.Add("description", productVM.Product.Description);
                    param.Add("author", productVM.Product.Author);      // author BEFORE isbn
                    param.Add("isbn", productVM.Product.ISBN);          // isbn AFTER author
                    param.Add("listPrice", productVM.Product.ListPrice);
                    param.Add("price", productVM.Product.Price);
                    param.Add("price50", productVM.Product.Price50);
                    param.Add("price100", productVM.Product.Price100);
                    param.Add("imageUrl", productVM.Product.ImageUrl);
                    param.Add("categoryId", productVM.Product.CategoryId);
                    param.Add("coverTypeId", productVM.Product.CoverTypeId);

                    try
                    {
                        _spcall.Execute(SD.Proc_Product_Update, param);
                        TempData["success"] = "Product updated successfully";
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = ex.Message;
                        System.Diagnostics.Debug.WriteLine("SP ERROR: " + ex.Message);
                        productVM.CategoryList = _spcall.List<Category>(SD.Proc_Category_GetAll)
                            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                        productVM.CoverTypeList = _spcall.List<CoverType>(SD.Proc_CoverType_GetAll)
                            .Select(ct => new SelectListItem { Text = ct.Name, Value = ct.Id.ToString() });
                        return View(productVM);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            productVM.CategoryList = _spcall.List<Category>(SD.Proc_Category_GetAll)
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            productVM.CoverTypeList = _spcall.List<CoverType>(SD.Proc_CoverType_GetAll)
                .Select(ct => new SelectListItem { Text = ct.Name, Value = ct.Id.ToString() });

            return View(productVM);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _spcall.List<Product>(SD.Proc_Product_GetAll);
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@id", id);

            var product = _spcall.OneRecord<Product>(SD.Proc_Product_GetOne, param);

            if (product == null)
                return Json(new { success = false, message = "Unable to delete!" });

            if (product.ImageUrl != null)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath,
                    product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _spcall.Execute(SD.Proc_Product_Delete, param);
            return Json(new { success = true, message = "Product deleted successfully" });
        }

        #endregion
    }
}