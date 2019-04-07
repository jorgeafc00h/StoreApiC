using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApi.Context.Data
{

    class ProductAuditLogEntityTypeConfiguration : IEntityTypeConfiguration<ProductAuditLog>
    {
        public void Configure(EntityTypeBuilder<ProductAuditLog> builder)
        {
            builder.HasOne(pl => pl.Product).WithMany().HasForeignKey(pl => pl.ProductId);

        }
    }

    class WharehouseActivityEntityTypeConfiguration : IEntityTypeConfiguration<WharehouseActivity>
    {
        public void Configure(EntityTypeBuilder<WharehouseActivity> builder)
        {
            builder.HasOne(wa => wa.Product).WithMany().HasForeignKey(wa => wa.ProductId);
        }
    }
}
