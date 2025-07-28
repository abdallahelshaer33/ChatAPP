namespace ChatAPI.Common
{
    public class Response<T>
    {
        public bool ISSuccess { get;  } 

        public T Data { get; }

        public string? Error { get; }

        public string? Message { get; set; }

        public Response(bool iSSuccess , T data ,string? error , string? message)
        {
            ISSuccess = iSSuccess;
            Data = data;
            Error = error;
            Message = message;
            
        }

        public static Response<T> success(T data, string? message = "") => new(true, data, null, message);
        public static Response<T> failure(string? error) => new(false, default!, error ,null);
    }
}
