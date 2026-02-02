namespace E_Commerce_Platform_Ass1.Service.Services.IServices
{
    public interface IGeminiService
    {
        /// <summary>
        /// Generate text content từ prompt
        /// </summary>
        Task<string?> GenerateContentAsync(string prompt);
        
        /// <summary>
        /// Generate và parse JSON response
        /// </summary>
        Task<T?> GenerateJsonContentAsync<T>(string prompt) where T : class;
    }
}
