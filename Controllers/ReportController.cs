using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment3_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment3_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reports/productcountbybrand
        [HttpGet("productcountbybrand")]
        public async Task<ActionResult<IEnumerable<ChartData>>> GetProductCountByBrand()
        {
            var productsByBrand = await _context.Products
                .GroupBy(p => p.Brand.Name)
                .Select(g => new ChartData
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .ToListAsync();

            return Ok(productsByBrand);
        }

        // GET: api/reports/productcountbyproducttype
        [HttpGet("productcountbyproducttype")]
        public async Task<ActionResult<IEnumerable<ChartData>>> GetProductCountByProductType()
        {
            var productsByProductType = await _context.Products
                .GroupBy(p => p.ProductType.Name)
                .Select(g => new ChartData
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .ToListAsync();

            return Ok(productsByProductType);
        }

        // GET: api/reports/activeproductsreport
        [HttpGet("activeproductsreport")]
        public async Task<ActionResult<IEnumerable<ActiveProductsReport>>> GetActiveProductsReport()
        {
            var activeProductsReport = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductType.Name)
                .ThenBy(p => p.Brand.Name)
                .ToListAsync();

            return Ok(activeProductsReport);
        }
    }

    public class ChartData
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

    public class ActiveProductsReport
    {
        public string ProductType { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}

