﻿ using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Shift_Tech.DbModels
{
    public class ShopDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }
        public ShopDbContext() : base() { }
        public virtual DbSet<ImageFile> Images { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
    }
}
