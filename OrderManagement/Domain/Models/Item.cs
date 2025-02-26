using System;
using System.Collections.Generic;

namespace OrderManagement.Domain.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
