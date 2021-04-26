namespace Glue.Delivery.Core.Domain
{
    public enum DeliveryState
    {
        Created = 0, 
        Approved = 1, 
        Completed = 2, 
        Cancelled = 3,
        Expired = 4
    }
}