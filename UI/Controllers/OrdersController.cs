using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Domain.Models;
using OrderManagementSystem.UI.Models;
using SharedComponents.Domain.Interfaces;
using Stripe;

namespace OrderManagementSystem.UI.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IStockService _stockService;
        private readonly IPaymentService _paymentService;

        public OrdersController(IOrderService orderService, IStockService stockService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _stockService = stockService;
            _paymentService = paymentService;
        }           

         [HttpGet]
         public async Task<IActionResult> Index()
         {
           var orders = await _orderService.GetAllOrdersAsync();
           return View(orders);
         }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Merr ID-në e përdoruesit aktual
            var businessId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(businessId))
            {
                return Unauthorized(); // Nëse nuk ka përdorues të autentikuar, kthe përgjigje 401
            }

            var stockItems = await _stockService.GetAllItemsAsync();

            var viewModel = new CreateOrderViewModel
            {
                BusinessId = businessId, // Vendos ID-në e biznesit
                Items = stockItems.Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    StockQuantity = i.StockQuantity
                }).ToList()
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var stockItems = await _stockService.GetAllItemsAsync();
                viewModel.Items = stockItems.Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    StockQuantity = i.StockQuantity
                }).ToList();
                return View(viewModel);
            }

            try
            {
                // Kontrolloni nëse BusinessId është null
                if (string.IsNullOrEmpty(viewModel.BusinessId))
                {
                    ModelState.AddModelError("", "Business ID is required.");
                    return View(viewModel);
                }

                // Krijo një porosi të re
                var order = new Order
                {
                    BusinessId = viewModel.BusinessId,
                    CreatedDate = DateTime.Now,
                    Status = "Pending",
                };

                var orderItems = viewModel.OrderItems
                    .Where(o => o.Quantity > 0)
                    .Select(o => new OrderItem
                    {
                        ItemId = o.ItemId,
                        Quantity = o.Quantity,
                        Price = o.Price
                    }).ToList();

                await _orderService.CreateOrderAsync(order, orderItems);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var stockItems = await _stockService.GetAllItemsAsync();
                viewModel.Items = stockItems.Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    StockQuantity = i.StockQuantity
                }).ToList();
                return View(viewModel);
            }
        }

        [HttpGet]
        public IActionResult Payment(int orderId, decimal totalAmount)
        {
            var viewModel = new PaymentViewModel
            {
                OrderId = orderId,
                TotalAmount = totalAmount
            };

            return View(viewModel); // Return the Payment view with the required data
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string stripeToken)
        {
            try
            {
                // Retrieve the order
                var order = await _orderService.GetOrderWithItemsAsync(orderId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                // Use Stripe API to process payment
                var options = new ChargeCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), // Amount in cents
                    Currency = "usd",
                    Description = $"Order #{order.Id}",
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                // Check if the payment was successful
                if (charge.Status == "succeeded")
                {
                    // Update order status to "Active"
                    order.Status = "Active";
                    await _orderService.UpdateOrderAsync(order);

                    // Reduce stock in the Items table
                    foreach (var orderItem in order.OrderItems)
                    {
                        // Fetch the corresponding item from the Items table
                        var item = await _stockService.GetItemByIdAsync(orderItem.ItemId);
                        if (item != null)
                        {
                            // Update stock quantity
                            item.StockQuantity -= orderItem.Quantity;

                            // Ensure stock quantity is not negative
                            if (item.StockQuantity < 0)
                            {
                                item.StockQuantity = 0;
                            }

                            // Update the item in the Items table
                            await _stockService.UpdateItemAsync(item);
                        }
                        else
                        {
                            throw new Exception($"Item with ID {orderItem.ItemId} not found.");
                        }
                    }

                    return RedirectToAction("Index", new { message = "Payment successful!" });
                }
                else
                {
                    ModelState.AddModelError("", "Payment failed. Please try again.");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Payment failed: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderWithItemsAsync(id);  
            if (order == null)
            {
                return NotFound("Porosia nuk u gjet.");
            }

            return View(order);
        }


    }
}
