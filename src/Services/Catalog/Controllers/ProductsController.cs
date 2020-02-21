using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.Data;
using Catalog.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogContext _context;
        private IConfiguration _configuration;
        private ILogger _logger;


        public ProductsController(CatalogContext context, IConfiguration configuration, ILogger<ProductsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }


        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            return await _context.Product.ToListAsync();
        }

        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetProductCount()
        {
            return await _context.Product.CountAsync();
        }
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();
            try
            {
                AddUrlToQueue(product.ProductId, product.ImageUrl);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

        private void AddUrlToQueue(int id,string url) {
            // Retrieve storage account from connection string.
            
            var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString") ?? _configuration.GetConnectionString("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference(Environment.GetEnvironmentVariable("ImageWorkerQueueName") ?? "imagesworker");

            // Create the queue if it doesn't already exist.
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            ImageWorkerMessage workerMessage = new ImageWorkerMessage() { Id = id, Url = url };
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(workerMessage));
            queue.AddMessage(message);
        }
    }

    public class ImageWorkerMessage {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
