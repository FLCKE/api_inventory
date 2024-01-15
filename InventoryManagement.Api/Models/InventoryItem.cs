 // InventoryManagement.Api/Models/InventoryItem.cs
namespace InventoryManagement.Api.Models
{
    public class InventoryItem
    {
        public int? product_id { get; set; }
        public string? product_name { get; set; }
        public string? description { get; set; }
        public double? price { get; set; }
        public string? category { get; set; }
        public int? stock_quantity { get; set; }

        // Constructeur par défaut avec initialisation des propriétés
        public InventoryItem()
        {
            product_id = null;
            product_name = null;
            description = null;
            price = null;
            category = null;
            stock_quantity = null;
        }
    }
}
