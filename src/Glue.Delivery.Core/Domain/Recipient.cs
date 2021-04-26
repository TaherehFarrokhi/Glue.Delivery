namespace Glue.Delivery.Core.Domain
{
    public sealed class Recipient
    {
        public string Name { get; init; }
        public string Address { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
    }
}