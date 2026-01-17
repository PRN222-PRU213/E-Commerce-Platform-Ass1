namespace E_Commerce_Platform_Ass1.Service.DTO.Response
{
    public class CartViewModel
    {
        public Guid Id { get; set; }

        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        public decimal TotalPrice { get; set; }
    }
}
