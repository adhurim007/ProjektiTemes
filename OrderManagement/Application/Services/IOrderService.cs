using OrderManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task CreateOrderAsync(Order order, List<OrderItem> orderItems);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id); 
        Task<Order> GetOrderWithItemsAsync(int id);
    }

}
