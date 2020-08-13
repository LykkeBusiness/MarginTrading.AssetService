namespace MarginTrading.AssetService.Contracts.Common
{
    /// <summary>
    /// Response which holds error code
    /// </summary>
    public class ErrorCodeResponse<T>
    {
        /// <summary>
        /// Error code
        /// </summary>
        public T ErrorCode { get; set; }
    }
}