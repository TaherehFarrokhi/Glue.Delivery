using System;

namespace Glue.Delivery.Core.Domain
{
    public sealed class AccessWindow
    {
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
    }
}