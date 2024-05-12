namespace redditpoller.api.Models
{
    public class ResponseContainer<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }

        public ResponseContainer(T data, bool success)
        {
            Data = data;
            Success = success;
        }
    }
}
