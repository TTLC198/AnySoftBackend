namespace AnySoftBackend.Library.DataTransferObjects.Order;

/// <summary>
/// Order DTO to secure a purchase
/// </summary>
public class OrderPurchaseDto
{
    /// <summary>
    /// Order identifier
    /// </summary>
    public int OrderId { get; set; }
    /// <summary>
    /// Payment identifier
    /// </summary>
    public int PaymentId { get; set; }
}