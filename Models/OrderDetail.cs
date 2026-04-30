using System;
using System.Collections.Generic;

namespace Unique_1.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public int? Quantity { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Amount { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Order? Order { get; set; }
}
