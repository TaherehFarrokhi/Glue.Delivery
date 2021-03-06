using Glue.Delivery.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Glue.Delivery.Core.Stores
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext()
        {
        }

        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<OrderDelivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDelivery>()
                .HasKey(m => m.DeliveryId);

            modelBuilder.Entity<OrderDelivery>().OwnsOne(m => m.Order);
            modelBuilder.Entity<OrderDelivery>().OwnsOne(m => m.AccessWindow);
            modelBuilder.Entity<OrderDelivery>().OwnsOne(m => m.Recipient);
        }
    }
}