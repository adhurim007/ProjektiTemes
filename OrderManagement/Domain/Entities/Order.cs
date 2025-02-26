using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public int Id { get;  set; }
        public string BusinessId { get;  set; }
        public decimal TotalAmount { get;  set; }
        public OrderStatus Status { get;  set; }
        public List<OrderItem> Items { get;  set; }

        public Order(string businessId, List<OrderItem> items)
        {
            BusinessId = businessId;
            Items = items;
            TotalAmount = items.Sum(i => i.TotalPrice);
            Status = OrderStatus.Pending;
        }

        public void MarkAsPaid()
        {
            if (Status == OrderStatus.Paid)
                throw new InvalidOperationException("Order is already paid.");
            Status = OrderStatus.Paid;
        }
    }

    public class OrderItem
    {
        private string v1;
        private int v2;
        private int v3;

        public int Id { get;  set; }
        public int ItemId { get;  set; }
        public int Quantity { get;  set; }
        public decimal Price { get;  set; }
        public decimal TotalPrice => Quantity * Price;

        public OrderItem(int itemId, int quantity, decimal price)
        {
            ItemId = itemId;
            Quantity = quantity;
            Price = price;
        }

        public OrderItem(string v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    public enum OrderStatus
    {
        Pending,
        Paid
    }

}
