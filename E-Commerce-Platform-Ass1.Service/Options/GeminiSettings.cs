namespace E_Commerce_Platform_Ass1.Service.Options
{
    public class GeminiSettings
    {
        public const string SectionName = "GeminiSettings";
        
        /// <summary>
        /// API Key từ Google AI Studio
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Model name (default: gemini-1.5-flash)
        /// </summary>
        public string Model { get; set; } = "gemini-1.5-flash";
        
        /// <summary>
        /// Base URL cho Gemini API
        /// </summary>
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";
    }
}
