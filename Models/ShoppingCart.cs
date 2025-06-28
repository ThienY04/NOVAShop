namespace lab03.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i =>
                i.ProductId == item.ProductId &&
                i.Size == item.Size &&
                i.Color == item.Color);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void UpdateItem(int productId, string size, string color, int quantity)
        {
            var item = Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                i.Size == size &&
                i.Color == color);

            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    RemoveItem(productId, size, color);
                }
            }
        }

        public void RemoveItem(int productId, string size, string color)
        {
            Items.RemoveAll(i =>
                i.ProductId == productId &&
                i.Size == size &&
                i.Color == color);
        }
    }
}
