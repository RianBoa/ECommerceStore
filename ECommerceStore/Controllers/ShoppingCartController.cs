using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;
using ECommerceStore.Services;

namespace ECommerceStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartController(ApplicationDbContext context, ShoppingCartService shoppingCartService)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;

        }

        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound(); // Return 404
            }

            _shoppingCartService.AddToCart(product); 

            return RedirectToAction("Index", "ShoppingCart"); 
        }
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.ShoppingCartItems.SingleOrDefault(item => item.Id == cartItemId);

            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult RemoveOneFromCart(int cartItemId)
        {
            var cartItem = _context.ShoppingCartItems.SingleOrDefault(item => item.Id == cartItemId);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _context.ShoppingCartItems.Remove(cartItem);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: ShoppingCartItems
        public async Task<IActionResult> Index()
        {
            var cartItems = await _context.ShoppingCartItems.Include(s => s.Product).ToListAsync();

            var totalSum = cartItems.Sum(item => item.Product.Price * item.Quantity);

            ViewBag.TotalSum = totalSum;

            return View(cartItems);
        }

        // GET: ShoppingCartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }

            return View(shoppingCartItem);
        }

        // GET: ShoppingCartItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: ShoppingCartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity")] ShoppingCartItem shoppingCartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", shoppingCartItem.ProductId);
            return View(shoppingCartItem);
        }

        // GET: ShoppingCartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", shoppingCartItem.ProductId);
            return View(shoppingCartItem);
        }

        // POST: ShoppingCartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity")] ShoppingCartItem shoppingCartItem)
        {
            if (id != shoppingCartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartItemExists(shoppingCartItem.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", shoppingCartItem.ProductId);
            return View(shoppingCartItem);
        }

        // GET: ShoppingCartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }

            return View(shoppingCartItem);
        }

        // POST: ShoppingCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (shoppingCartItem != null)
            {
                _context.ShoppingCartItems.Remove(shoppingCartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartItemExists(int id)
        {
            return _context.ShoppingCartItems.Any(e => e.Id == id);
        }
    }
}
