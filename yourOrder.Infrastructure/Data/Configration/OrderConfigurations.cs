using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.OrderAggregate;

namespace yourOrder.Infrastructure.Data.Configration
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Status)
                .HasConversion(dbstatus => dbstatus.ToString(),
                               systatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), systatus));

            builder.Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.OwnsOne(o => o.ShipToAddress, orderAddress => orderAddress.WithOwner());

            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(o => o.DeliveryMethod).WithMany().OnDelete(DeleteBehavior.NoAction);

        }
    }
}
