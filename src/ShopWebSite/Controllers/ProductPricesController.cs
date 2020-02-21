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
    public class ProductPricesController : Controller
    {
        private IApiService<ProductPrice> _service;

        public ProductPricesController(IApiService<ProductPrice> service)
        {
            _service = service;
        }

       
        // GET: ProductPrices
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        // GET: ProductPrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPrice = await _service.GetByIdAsync(id.Value);
            if (productPrice == null)
            {
                return NotFound();
            }

            return View(productPrice);
        }

        // GET: ProductPrices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductPriceId,ProductId,PeriodFromUtc,PeriodToUtc,Price")] ProductPrice productPrice)
        {
            if (ModelState.IsValid)
            {
                var item= await _service.CreateAsync(productPrice);
                return RedirectToAction(nameof(Index));
            }
            return View(productPrice);
        }

        // GET: ProductPrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPrice = await _service.GetByIdAsync(id.Value);
            if (productPrice == null)
            {
                return NotFound();
            }
            return View(productPrice);
        }

        // POST: ProductPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductPriceId,ProductId,PeriodFromUtc,PeriodToUtc,Price")] ProductPrice productPrice)
        {
            if (id != productPrice.ProductPriceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _service.UpdateAsync(id,productPrice);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductPriceExists(productPrice.ProductPriceId))
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
            return View(productPrice);
        }

        // GET: ProductPrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productPrice = await _service.GetByIdAsync(id.Value);
            if (productPrice == null)
            {
                return NotFound();
            }

            return View(productPrice);
        }

        // POST: ProductPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productPrice = await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductPriceExists(int id)
        {
            var item = _service.GetByIdAsync(id).Result;
            return (item.ProductPriceId== id);

        }
    }
}
