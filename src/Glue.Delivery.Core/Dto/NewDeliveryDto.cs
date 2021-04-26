namespace Glue.Delivery.Core.Dto
{
    public sealed class NewDeliveryDto
    {
        public AccessWindowDto AccessWindow { get; init; }
        public RecipientDto Recipient { get; init; }
        public OrderDto Order { get; init; }
    }
}