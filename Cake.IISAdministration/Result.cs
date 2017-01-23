namespace Cake.IISAdministration
{

    public class Result<T>
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public T Data { set; get; }
    }
}