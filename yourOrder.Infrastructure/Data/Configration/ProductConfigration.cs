using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.ProductAggregate;

namespace yourOrder.Infrastructure.Data.Configration
{
    internal class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(p => p.productBrand)
                    .WithMany()
                    .HasForeignKey(p => p.ProductBrandId);

            builder.HasOne(p => p.productType)
                    .WithMany()
                    .HasForeignKey(p => p.ProductTypeId);
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Description)
                .IsRequired();
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

        }
    }
}
