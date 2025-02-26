using OrderManagement.Domain.Models;

namespace OrderManagementSystem.UI.Models
{
    public class CreateOrderViewModel
    {
        public string? BusinessId { get; set; } // Static value for now
        public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
