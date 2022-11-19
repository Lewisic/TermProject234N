using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TermProject.AdditionalModels;
using TermProject.Models;
using TermProject.UnitConversion;

namespace TermProjectRESTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly TermProjectContext _context;

        public ShoppingListController(TermProjectContext context)
        {
            _context = context;
        }

        // GET: api/Ingredients
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }*/

        [HttpGet]
        public async Task<ActionResult<ShoppingList>> GetShoppingList()
        {
            var shoppingList = new ShoppingList(_context);

            if (shoppingList.Items.Count == 0)
            {
                return NotFound();
            }

            return shoppingList;
        }

        // GET: api/shoppinglist/2020-09-01/2020-10-01
        [HttpGet("{startDate}/{endDate}")]
        public async Task<ActionResult<ShoppingList>> GetShoppingList(DateTime startDate, DateTime endDate)
        {
            var shoppingList = new ShoppingList(_context, startDate, endDate);

            if (shoppingList.Items.Count == 0)
            {
                return NotFound();
            }

            return shoppingList;
        }
    }
}
