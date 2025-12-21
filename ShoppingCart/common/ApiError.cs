public class ApiError(string Code, string Message)
{
    public string code {get; set;} = Code;
    
    public string message {get; set;} = Message;
    
}