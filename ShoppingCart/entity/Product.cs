using ShoppingCart.Enums;

namespace ShoppingCart.Entities 
{
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; } = null!;
        
        public decimal Price { get; set; }
        
        public UnitType DefaultUnit { get; set; }
        public string DefaultUnitName { get; set; } = null!;         
    }
}