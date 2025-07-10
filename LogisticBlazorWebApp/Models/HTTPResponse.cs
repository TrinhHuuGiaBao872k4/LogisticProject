using System; 
public class HTTPResponse<T>
{
    public int statusCode { get; set; }
    public string messsage { get; set; }
    public T data { get; set; }
    public DateTime dateTime { get; set; }
}