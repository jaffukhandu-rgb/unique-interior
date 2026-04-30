public class CustomerViewModel
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = "";
    public string Address { get; set; } = "";
    public string PhoneNumber { get; set; } = "";

    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal GrandTotal { get; set; }

    public List<WorkVM> Works { get; set; } = new();
}

public class WorkVM
{
    public int CategoryId { get; set; }
    public decimal Unit { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}