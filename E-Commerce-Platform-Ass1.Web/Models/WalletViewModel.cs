using E_Commerce_Platform_Ass1.Service.DTOs;

namespace E_Commerce_Platform_Ass1.Web.Models
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public decimal? LastChangeAmount { get; set; }
        public string? LastChangeType { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Lịch sử giao dịch gần đây
        /// </summary>
        public List<WalletTransactionDto> Transactions { get; set; } = new();
    }
}
