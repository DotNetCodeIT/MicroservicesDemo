using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Marketing.Data;
using Marketing.Models;

namespace Marketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPriceController : ControllerBase
    {
        private readonly MarketingContext _context;

        public ProductPriceController(MarketingContext context)
        {
            _context = context;
        }

        // GET: api/ProductPrice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductPrice>>> GetProductPrice()
        {
            return await _context.ProductPrice.ToListAsync();
        }

        // GET: api/ProductPrice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPrice>> GetProductPrice(int id)
        {
            var productPrice = await _context.ProductPrice.FindAsync(id);

            if (productPrice == null)
            {
                return NotFound();
            }

            return productPrice;
        }

        // PUT: api/ProductPrice/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductPrice(int id, ProductPrice productPrice)
        {
            if (id != productPrice.ProductPriceId)
            {
                return BadRequest();
            }

            _context.Entry(productPrice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductPriceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductPrice
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ProductPrice>> PostProductPrice(ProductPrice productPrice)
        {
            _context.ProductPrice.Add(productPrice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductPrice", new { id = productPrice.ProductPriceId }, productPrice);
        }

        // DELETE: api/ProductPrice/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductPrice>> DeleteProductPrice(int id)
        {
            var productPrice = await _context.ProductPrice.FindAsync(id);
            if (productPrice == null)
            {
                return NotFound();
            }

            _context.ProductPrice.Remove(productPrice);
            await _context.SaveChangesAsync();

            return productPrice;
        }

        private bool ProductPriceExists(int id)
        {
            return _context.ProductPrice.Any(e => e.ProductPriceId == id);
        }
    }
}
