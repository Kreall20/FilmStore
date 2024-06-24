using ShopCinema.Data.Cart;
using ShopCinema.Data.Services;
using ShopCinema.Data.Static;
using ShopCinema.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ShopCinema.Data;
using Microsoft.EntityFrameworkCore;
using ShopCinema.Models;
using Microsoft.AspNetCore.Identity;
using Azure;

namespace ShopCinema.Controllers
{
    [Authorize] 
    public class OrdersController : Controller
    {
        private readonly IMoviesService _moviesService;
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrdersService _ordersService;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMoviesService _service;

        public OrdersController(IMoviesService moviesService, ShoppingCart shoppingCart, IOrdersService ordersService, IMoviesService service, UserManager<ApplicationUser> userManager)
        {
            _moviesService = moviesService;
            _shoppingCart = shoppingCart;
            _ordersService = ordersService;
            _service = service;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            return View(orders);
        }

        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems(_userManager.GetUserAsync(User).Result.UserName);
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal(_userManager.GetUserAsync(User).Result.UserName)
            };

            return View(response);
        }
        public async Task<IActionResult> FilterOrder(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string userRole = User.FindFirstValue(ClaimTypes.Role);
                var list = new List<OrderItem>();
                var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
                for(int i = 0;i < orders.Count; i++)
                {
                    var listorders = orders[i].OrderItems.Where(n => n.Movie.Name == searchString);
                    list.AddRange(listorders);
                }
                return View(list);
            }
            return View("NotFound");
        }
        public async Task<IActionResult> Filter(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString)) {
                var items = _shoppingCart.GetShoppingCartItems(_userManager.GetUserAsync(User).Result.UserName).Where(n => n.Movie.Name == searchString).ToList();
                _shoppingCart.ShoppingCartItems = items;
                var response = new ShoppingCartVM()
                {
                    ShoppingCart = _shoppingCart,
                };
                return View(response);
            }
            return View("NotFound");
        }
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _moviesService.GetMovieByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.AddItemToCart(item, _userManager.GetUserAsync(User).Result.UserName);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _moviesService.GetMovieByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item, _userManager.GetUserAsync(User).Result.UserName);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> CompleteOrder()
        {
            var items = _shoppingCart.GetShoppingCartItems(_userManager.GetUserAsync(User).Result.UserName);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            await _ordersService.StoreOrderAsync(items, userId, userEmailAddress);
            await _shoppingCart.ClearShoppingCartAsync(_userManager.GetUserAsync(User).Result.UserName);
            foreach (var item in items)
            {
                var dbMovie = await _service.GetMovieByIdAsync(item.Movie.Id);
                var response = new NewMovieVM()
                {
                    Id = dbMovie.Id,
                    Name = dbMovie.Name,
                    Description = dbMovie.Description,
                    Price = dbMovie.Price,
                    Image = dbMovie.Image,
                    countofMovie = dbMovie.countofMovie - item.Amount,
                    MovieCategory = dbMovie.Category,
                    ProducerId = dbMovie.ProducerId,
                    ActorIds = dbMovie.Actors_Movies.Select(n => n.ActorId).ToList(),
                };
                await _service.UpdateMovieAsync(response);
            }
            return View("OrderCompleted");
        }
    }
}
