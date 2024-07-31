using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment3_Backend.Models;
using Assignment3_Backend.ViewModels;


namespace Assignment3_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IRepository repository, AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Select(p => new ProductViewModel
                {
                    price = p.Price,
                    producttype = p.ProductType.Name,
                    brand = p.Brand.Name,
                    description = p.Description,
                    name = p.Name,
                    Image = p.Image // Include the image field
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            var brands = await _context.Brands.ToListAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
        {
            var types = await _context.ProductTypes.ToListAsync();
            return Ok(types);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductViewModel
                {
                    price = p.Price,
                    producttype = p.ProductType.Name,
                    brand = p.Brand.Name,
                    description = p.Description,
                    name = p.Name,
                    Image = p.Image // Include the image field
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // GET: api/products/filter?filterText={filterText}
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> FilterProducts(string filterText)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Where(p =>
                    p.Name.Contains(filterText) ||
                    p.Description.Contains(filterText) ||
                    p.Brand.Name.Contains(filterText) ||
                    p.ProductType.Name.Contains(filterText))
                .Select(p => new ProductViewModel
                {
                    price = p.Price,
                    producttype = p.ProductType.Name,
                    brand = p.Brand.Name,
                    description = p.Description,
                    name = p.Name,
                    Image = p.Image // Include the image field
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/products/sort?sortBy={sortBy}&orderBy={orderBy}
        [HttpGet("sort")]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> SortProducts(string sortBy, string orderBy)
        {
            var productsQuery = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Select(p => new ProductViewModel
                {
                    price = p.Price,
                    producttype = p.ProductType.Name,
                    brand = p.Brand.Name,
                    description = p.Description,
                    name = p.Name,
                    Image = p.Image // Include the image field
                });

            switch (sortBy.ToLower())
            {
                case "name":
                    productsQuery = orderBy.ToLower() == "asc" ? productsQuery.OrderBy(p => p.name) : productsQuery.OrderByDescending(p => p.name);
                    break;
                case "price":
                    productsQuery = orderBy.ToLower() == "asc" ? productsQuery.OrderBy(p => p.price) : productsQuery.OrderByDescending(p => p.price);
                    break;
                case "brand":
                    productsQuery = orderBy.ToLower() == "asc" ? productsQuery.OrderBy(p => p.brand) : productsQuery.OrderByDescending(p => p.brand);
                    break;
                case "producttype":
                    productsQuery = orderBy.ToLower() == "asc" ? productsQuery.OrderBy(p => p.producttype) : productsQuery.OrderByDescending(p => p.producttype);
                    break;
                case "description":
                    productsQuery = orderBy.ToLower() == "asc" ? productsQuery.OrderBy(p => p.description) : productsQuery.OrderByDescending(p => p.description);
                    break;
                default:
                    return BadRequest("Invalid sortBy parameter.");
            }

            var products = await productsQuery.ToListAsync();
            return Ok(products);
        }

        // GET: api/products/page?pageSize={pageSize}&pageNumber={pageNumber}
        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProductsPage(int pageSize, int pageNumber)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Select(p => new ProductViewModel
                {
                    price = p.Price,
                    producttype = p.ProductType.Name,
                    brand = p.Brand.Name,
                    description = p.Description,
                    name = p.Name,
                    Image = p.Image 
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(products);
        }

        // POST: api/products
        [HttpPost("create")]
        public async Task<IActionResult> AddProduct([FromForm] ProductPostVM productVm)
        {
            if (ModelState.IsValid)
            {
                string base64String = null;
                if (productVm.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await productVm.Image.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                }

                var product = new Product
                {
                    Name = productVm.Name,
                    Price = productVm.Price,
                    Description = productVm.Description,
                    BrandId = productVm.BrandId,
                    ProductTypeId = productVm.ProductTypeId,
                    Image = base64String // Save the base64 string
                };

                _repository.Add(product);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok(new { product.Name });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving product");
            }

            return BadRequest(ModelState);
        }
    }
}
