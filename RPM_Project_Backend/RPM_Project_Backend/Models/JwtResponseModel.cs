namespace RPM_Project_Backend.Models;

/// <summary>
/// JWT Response object Model
/// </summary>
public class JwtResponseModel
{
    /// <summary>
    /// JWT Bearer Token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// DateTime until which the token is valid for entry
    /// </summary>
    public DateTime Expiration { get; set; }
}