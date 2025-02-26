using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Domain.Models;
using SharedComponents.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagement.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderManagementDbContext _context; 
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderRepository2;
        private readonly IStockService _stockService;

        public OrderService(IRepository<Order> orderRepository, IStockService stockService, IRepository<OrderItem> orderRepository2, OrderManagementDbContext context)
        {
            _orderRepository = orderRepository;
            _stockService = stockService;
            _orderRepository2 = orderRepository2;
            _context = context; 
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }
         
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order> GetOrderWithItemsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems) 
                .ThenInclude(o=> o.Item)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

         

        public async Task CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "Order nuk mund të jetë null.");

            if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("OrderItems nuk mund të jetë bosh.", nameof(orderItems));

            // Përllogarite TotalAmount
            order.TotalAmount = orderItems.Sum(i => i.Quantity * i.Price);

            // Shto porosinë
            await _orderRepository.AddAsync(order);

            // Lidh çdo OrderItem me porosinë
            foreach (var orderItem in orderItems)
            {
                orderItem.OrderId = order.Id;
                await _orderRepository2.AddAsync(orderItem); // Shto çdo OrderItem
            }
        }


        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(order.Id);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {order.Id} not found.");
            }

            // Optional: Adjust stock if the order items have changed
            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            // Rikthimi i stokut për çdo artikull të porosisë
            foreach (var orderItem in order.OrderItems)
            {
                var stockItem = await _stockService.GetItemByIdAsync(orderItem.ItemId);
                stockItem.StockQuantity += orderItem.Quantity;
                await _stockService.UpdateItemAsync(stockItem);
            }

            await _orderRepository.DeleteAsync(id);
        }

        public decimal CalculateTotalCost(Order order)
        {
            return order.OrderItems.Sum(item => item.Price * item.Quantity);
        }

       
    }
}
