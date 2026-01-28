using System.Security.Claims;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using E_Commerce_Platform_Ass1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass1.Web.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IMomoService _momoService;

        public WalletController(IWalletService walletService, IMomoService momoService)
        {
            _walletService = walletService;
            _momoService = momoService;
        }

        /// <summary>
        /// GET: /Wallet - Xem số dư ví
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            var walletDto = await _walletService.GetOrCreateAsync(userId);
            var transactions = await _walletService.GetTransactionsAsync(userId, 10);

            var vm = new WalletViewModel
            {
                Balance = walletDto.Balance,
                UpdatedAt = walletDto.UpdatedAt,
                LastChangeAmount = walletDto.LastChangeAmount,
                LastChangeType = walletDto.LastChangeType,
                Transactions = transactions
            };

            return View(vm);
        }
        
        /// <summary>
        /// GET: /Wallet/TopUp - Form nạp tiền
        /// </summary>
        [HttpGet]
        public IActionResult TopUp()
        {
            return View(new TopUpViewModel());
        }
        
        /// <summary>
        /// POST: /Wallet/TopUp - Xử lý nạp tiền qua Momo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopUp(TopUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Validate amount
            if (model.Amount < 10000)
            {
                ModelState.AddModelError("Amount", "Số tiền nạp tối thiểu là 10,000 VNĐ.");
                return View(model);
            }
            
            if (model.Amount > 10000000)
            {
                ModelState.AddModelError("Amount", "Số tiền nạp tối đa là 10,000,000 VNĐ.");
                return View(model);
            }
            
            try
            {
                // Lưu số tiền vào session
                HttpContext.Session.SetString("TopUpAmount", model.Amount.ToString());
                
                // Tạo payment URL qua Momo
                var payUrl = await _momoService.CreateTopUpPaymentAsync(
                    (long)model.Amount,
                    $"Nạp tiền ví - {model.Amount:N0} VNĐ"
                );
                
                if (string.IsNullOrEmpty(payUrl))
                {
                    TempData["Error"] = "Không thể tạo yêu cầu thanh toán Momo.";
                    return View(model);
                }
                
                return Redirect(payUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return View(model);
            }
        }
        
        /// <summary>
        /// GET: /Wallet/TopUpCallback - Callback từ Momo sau khi thanh toán
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TopUpCallback(int resultCode, string? orderId, string? transId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }
            
            var vm = new TopUpCallbackViewModel();
            
            if (resultCode == 0)
            {
                // Thanh toán thành công
                var amountStr = HttpContext.Session.GetString("TopUpAmount");
                
                if (string.IsNullOrEmpty(amountStr) || !decimal.TryParse(amountStr, out decimal amount))
                {
                    vm.IsSuccess = false;
                    vm.Message = "Không tìm thấy thông tin nạp tiền.";
                    return View(vm);
                }
                
                try
                {
                    var wallet = await _walletService.TopUpAsync(userId, amount, transId);
                    
                    HttpContext.Session.Remove("TopUpAmount");
                    
                    vm.IsSuccess = true;
                    vm.Amount = amount;
                    vm.NewBalance = wallet.Balance;
                    vm.Message = $"Nạp tiền thành công {amount:N0} VNĐ!";
                }
                catch (Exception ex)
                {
                    vm.IsSuccess = false;
                    vm.Message = $"Lỗi khi nạp tiền: {ex.Message}";
                }
            }
            else
            {
                vm.IsSuccess = false;
                vm.Message = resultCode switch
                {
                    1006 => "Bạn đã hủy giao dịch.",
                    1005 => "Tài khoản Momo không đủ số dư.",
                    1003 => "Giao dịch đã hết hạn.",
                    _ => "Thanh toán không thành công. Vui lòng thử lại."
                };
            }
            
            return View(vm);
        }
        
        /// <summary>
        /// GET: /Wallet/Transactions - Lịch sử giao dịch
        /// </summary>
        public async Task<IActionResult> Transactions()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }
            
            var transactions = await _walletService.GetTransactionsAsync(userId, 50);
            
            return View(transactions);
        }
    }
}
