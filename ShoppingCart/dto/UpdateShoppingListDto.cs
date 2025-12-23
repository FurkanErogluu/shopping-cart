namespace ShoppingCart.dto
{
    public class UpdateShoppingListDto
    {
        public int Id { get; set; }     // Hangi listeyi güncelliyoruz?
        public string Name { get; set; } // Yeni ismi ne olacak?
        public bool IsCompleted { get; set; } // Alışveriş listesi tamamlandı mı?
    }
}