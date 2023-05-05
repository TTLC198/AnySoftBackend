namespace RPM_Project_Backend.Models;

public class ErrorModel
{
    public string Message { get; set; }

    public ErrorModel(string message)
    {
        Message = message;
    }
}