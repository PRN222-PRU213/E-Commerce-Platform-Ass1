using E_Commerce_Platform_Ass1.Service.DTOs;
using E_Commerce_Platform_Ass1.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass1.Service.Services
{
    public class VnptEKycService : IVnptEKycService
    {
        private readonly VnptEKycConfig _config;

        public VnptEKycService(IOptions<VnptEKycConfig> config)
        {
            _config = config.Value;
        }

        public async Task<EKycResult> VerifyAsync(IFormFile frontCccd, IFormFile backCccd, IFormFile selfie)
        {
            try
            {
                var ocr = await CallOcrAsync(frontCccd, backCccd);
                if (ocr == null || !ocr.Success)
                    return EKycResult.Fail("OCR CCCD thất bại hoặc không nhận diện được thông tin");

                var faceMatch = await CallFaceMatchAsync(frontCccd, selfie);
                if (faceMatch == null || faceMatch.similarity < 0.8)
                    return EKycResult.Fail("Khuôn mặt không khớp với ảnh trên CCCD (Tỉ lệ khớp thấp)");

                return new EKycResult 
                { 
                    IsSuccess = true, 
                    CCCDNumber = ocr.IdNumber, 
                    FullName = ocr.FullName,
                    FaceMatchScore = faceMatch.similarity
                };
            }
            catch (Exception ex)
            {
                return EKycResult.Fail($"Lỗi hệ thống trong quá trình xác thực: {ex.Message}");
            }
        }

        private async Task<OrcResultDto?> CallOcrAsync(IFormFile front, IFormFile back)
        {
            // 1. Upload ảnh để lấy mã hash Minio (theo tài liệu OCR yêu cầu img_front/img_back là mã hash)
            string frontHash = await UploadFileAsync(front);
            string backHash = await UploadFileAsync(back);

            using var client = CreateHttpClient();

            var requestBody = new
            {
                img_front = frontHash,
                img_back = backHash,
                client_session = $"WEB_DESKTOP_WINDOWS_DEVICE_1.0.0_EShopper_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                type = -1, // CMT/CCCD
                validate_postcode = true,
                token = Guid.NewGuid().ToString(),
                crop_param = "0,0"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ai/v1/ocr/id", jsonContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var methodWarning = response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed 
                    ? " (Lưu ý: API báo lỗi 405. Vui lòng kiểm tra lại 'BaseUrl' trong appsettings.json, thường phải là https://api-ekyc.vnpt.vn)" 
                    : "";
                throw new Exception($"VNPT API Error (OCR): {response.StatusCode}{methodWarning} - {content}");
            }

            if (content.Trim().StartsWith("<"))
                throw new Exception("VNPT API đã trả về nội dung HTML thay vì dữ liệu. Có thể service đang bảo trì hoặc sai cấu hình URL.");

            return JsonConvert.DeserializeObject<OrcResultDto>(content);
        }

        private async Task<dynamic?> CallFaceMatchAsync(IFormFile front, IFormFile selfie)
        {
            // 1. Upload ảnh selfie để lấy mã hash
            string frontHash = await UploadFileAsync(front);
            string faceHash = await UploadFileAsync(selfie);

            using var client = CreateHttpClient();

            var requestBody = new
            {
                img_front = frontHash,
                img_face = faceHash,
                client_session = $"WEB_DESKTOP_WINDOWS_DEVICE_1.0.0_EShopper_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                token = Guid.NewGuid().ToString()
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ai/v1/face/compare", jsonContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"VNPT API Error (FaceMatch): {response.StatusCode} - {content}");

            var jo = Newtonsoft.Json.Linq.JObject.Parse(content);
            if (jo["message"]?.ToString() != "IDG-00000000")
                throw new Exception($"Face Match thất bại: {jo["message"]}");

            var obj = jo["object"];
            return new
            {
                result = obj?["result"]?.ToString(),
                msg = obj?["msg"]?.ToString(),
                similarity = (double)(obj?["prob"] ?? 0) / 100.0 // Chuyển từ % sang scale 0-1
            };
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            using var client = CreateHttpClient();
            var form = new MultipartFormDataContent();
            
            // Theo chuẩn VNPT mới cung cấp:
            // Endpoint: file-service/v1/addFile
            // Fields: file, title, description
            
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");
            form.Add(streamContent, "file", file.FileName);
            form.Add(new StringContent("KYC_ID_Card"), "title");
            form.Add(new StringContent("Identity Verification Document"), "description");

            var response = await client.PostAsync("file-service/v1/addFile", form);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"VNPT API Error (Upload): {response.StatusCode} - {content}");

            // Sử dụng JObject để parse an toàn hơn dynamic
            var jo = Newtonsoft.Json.Linq.JObject.Parse(content);
            var message = jo["message"]?.ToString();
            
            if (message != "IDG-00000000")
                throw new Exception($"Upload ảnh thất bại: {message} - {content}");

            var obj = jo["object"];
            if (obj == null)
                throw new Exception("Trường 'object' không tồn tại trong phản hồi upload.");

            // Nếu obj là một object phức tạp, hãy lấy giá trị token/hash từ nó
            // Nếu obj là chuỗi, lấy trực tiếp
            string hash = obj.Type == Newtonsoft.Json.Linq.JTokenType.Object 
                ? obj["hash"]?.ToString() ?? obj["id"]?.ToString() ?? obj.ToString()
                : obj.ToString();

            if (string.IsNullOrEmpty(hash))
                throw new Exception("Không lấy được mã hash từ phản hồi của server.");

            return hash;
        }

        private HttpClient CreateHttpClient()
        {
            var baseUrl = _config.BaseUrl;
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(60)
            };

            var token = _config.AccessToken ?? "";
            if (token.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(7);
            }

            client.DefaultRequestHeaders.Add("Token-id", _config.TokenId);
            client.DefaultRequestHeaders.Add("Token-key", _config.TokenKey);
            client.DefaultRequestHeaders.Add("mac-address", "TEST1");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}
