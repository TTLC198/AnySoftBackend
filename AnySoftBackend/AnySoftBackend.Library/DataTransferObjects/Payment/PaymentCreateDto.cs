using System;

namespace AnySoftBackend.Library.DataTransferObjects.Payment;

/// <summary>
/// Payment Data Transfer Object
/// </summary>
public class PaymentCreateDto
{
    /// <summary>
    /// Bank Card Number
    /// </summary>
    public string? Number { get; set; }
    /// <summary>
    /// Bank Card Name on Card 
    /// </summary>
    public string? CardName { get; set; }
    /// <summary>
    /// Bank Card Expiration Date
    /// </summary>
    public DateTime ExpirationDate { get; set; }
    /// <summary>
    /// Bank Card Security Code
    /// </summary>
    public string? Cvc { get; set; }
}