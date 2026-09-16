using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.IsCancelled).IsRequired();

        builder.OwnsOne(i => i.Product, product =>
        {
            product.Property(p => p.Id).HasColumnName("ProductId").IsRequired();
            product.Property(p => p.Name).HasColumnName("ProductName").IsRequired().HasMaxLength(100);
        });

        builder.OwnsOne(i => i.Quantity, quantity =>
        {
            quantity.Property(q => q.Value).HasColumnName("Quantity").IsRequired();
        });

        builder.OwnsOne(i => i.UnitPrice, unitPrice =>
        {
            unitPrice.Property(u => u.Value).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)").IsRequired();
        });

        builder.OwnsOne(i => i.Discount, discount =>
        {
            discount.Property(d => d.Percentage).HasColumnName("DiscountPercentage").HasColumnType("decimal(5,2)").IsRequired();
        });
    }
}
