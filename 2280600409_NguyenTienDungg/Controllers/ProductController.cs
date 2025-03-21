using Microsoft.AspNetCore.Mvc;
using _2280600409_NguyenTienDungg.Models;
using _2280600409_NguyenTienDungg.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace _2280600409_NguyenTienDungg.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // GET: Add Product
        public async Task<IActionResult> Add()
        {
            await LoadCategories();
            return View(new Product());
        }

        // POST: Add Product
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            await LoadCategories();

            if (product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category.");
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (imageUrl != null && imageUrl.Length > 0)
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }

            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Update Product
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            await LoadCategories(product.CategoryId);
            return View(product);
        }

        // POST: Update Product
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl");

            if (id != product.Id) return NotFound();

            await LoadCategories(product.CategoryId);

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (imageUrl != null && imageUrl.Length > 0)
            {
                existingProduct.ImageUrl = await SaveImage(imageUrl);
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);
            return RedirectToAction(nameof(Index));
        }

        // GET: Display Product
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Delete Product
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Confirm Delete
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Save Image to wwwroot/images
        private async Task<string> SaveImage(IFormFile image)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileName = Path.GetFileName(image.FileName);
            var savePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }

        // Load Categories for Dropdown
        private async Task LoadCategories(int? selectedCategoryId = null)
        {
            var categories = await _categoryRepository.GetAllAsync();

            if (categories == null || !categories.Any())
            {
                ViewBag.CategoryError = "No categories found. Please add a category first.";
                ViewBag.Categories = new SelectList(new List<Category>());
            }
            else
            {
                ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
            }
        }
    }
}
