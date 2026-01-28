using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass1.Web.Models
{
    /// <summary>
    /// ViewModel cho form nạp tiền
    /// </summary>
    public class TopUpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập số tiền nạp.")]
        [Range(10000, 10000000, ErrorMessage = "Số tiền nạp từ 10,000 đến 10,000,000 VNĐ.")]
        [Display(Name = "Số tiền nạp")]
        public decimal Amount { get; set; }
    }
    
    /// <summary>
    /// ViewModel cho kết quả nạp tiền
    /// </summary>
    public class TopUpCallbackViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
    }
}
