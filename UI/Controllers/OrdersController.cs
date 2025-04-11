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
         
            var businessId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(businessId))
            {
                return Unauthorized(); 
            }

            var stockItems = await _stockService.GetAllItemsAsync();

            var viewModel = new CreateOrderViewModel
            {
                BusinessId = businessId, 
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
              
                if (string.IsNullOrEmpty(viewModel.BusinessId))
                {
                    ModelState.AddModelError("", "Business ID is required.");
                    return View(viewModel);
                }

              
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

            return View(viewModel);  
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string stripeToken)
        {
            try
            {
                
                var order = await _orderService.GetOrderWithItemsAsync(orderId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

               
                var options = new ChargeCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), // Amount in cents
                    Currency = "usd",
                    Description = $"Order #{order.Id}",
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                
                if (charge.Status == "succeeded")
                {
                   
                    order.Status = "Active";
                    await _orderService.UpdateOrderAsync(order);

                   
                    foreach (var orderItem in order.OrderItems)
                    {
                      
                        var item = await _stockService.GetItemByIdAsync(orderItem.ItemId);
                        if (item != null)
                        {
                           
                            item.StockQuantity -= orderItem.Quantity;

                          
                            if (item.StockQuantity < 0)
                            {
                                item.StockQuantity = 0;
                            }
 
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
