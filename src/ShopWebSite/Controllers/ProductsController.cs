using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopWebSite.Data;
using ShopWebSite.Models;
using ShopWebSite.Services;

namespace ShopWebSite.Controllers
{
    public class ProductsController : Controller
    {
        private IApiService<Product> _service;
        private IApiService<Supplier> _serviceSupplier;


        public ProductsController(IApiService<Product> service,IApiService<Supplier> serviceSupplier)
        {
            _service = service;
            _serviceSupplier = serviceSupplier;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _service.GetByIdAsync(id.Value);
            product.Supplier = await _serviceSupplier.GetByIdAsync(product.SupplierId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> CreateAsync()
        {
            
            var suppliers = await _serviceSupplier.GetAllAsync();
            ViewBag.Suppliers = suppliers;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,SupplierId,Title,Description,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                var item = await _service.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var suppliers = await _serviceSupplier.GetAllAsync();
            ViewBag.Suppliers = suppliers;

            var product = await _service.GetByIdAsync(id.Value);
            
            product.Supplier = await _serviceSupplier.GetByIdAsync(product.SupplierId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,SupplierId,Title,Description,ImageUrl")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _service.UpdateAsync(id, product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _service.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            var item = _service.GetByIdAsync(id).Result;
            return (item.ProductId == id);
        }
    }
}
