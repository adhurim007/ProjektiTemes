using System;
using System.Collections.Generic;

namespace OrderManagement.Domain.Models;

public partial class Order
{
    public int Id { get; set; }

    public string BusinessId { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual AspNetUser Business { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


}
