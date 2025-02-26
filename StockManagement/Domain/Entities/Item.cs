namespace StockManagement.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedDate { get; set; }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0 && StockQuantity + quantity < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            StockQuantity += quantity;
        }
    }

}
