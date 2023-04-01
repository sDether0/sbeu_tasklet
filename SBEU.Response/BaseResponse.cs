namespace SBEU.Response
{
    public class BaseResponse<T>
    {
        public bool Status { get; set; }
        public int Code { get; set; }
        public BaseError? Error { get; set; }
        public T? Result { get; set; }
    }
}