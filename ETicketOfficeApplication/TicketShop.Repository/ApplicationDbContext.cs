using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketShop.Domain.Domain;
using TicketShop.Domain.Identity;

namespace TicketShop.Repository
{
    public class ApplicationDbContext : IdentityDbContext<EShopUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketsInShoppingCarts { get; set; }
        public virtual DbSet<TicketInOrder> TicketsInOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // IDENTIFIERS

            builder.Entity<Ticket>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInOrder>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInShoppingCart>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            // RELATIONS

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(t => t.ShoppingCarts)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Cart)
                .WithMany(t => t.Tickets)
                .HasForeignKey(z => z.CartId);

            builder.Entity<ShoppingCart>()
                .HasOne<EShopUser>(z => z.Owner)
                .WithOne(z => z.Cart)
                .HasForeignKey<ShoppingCart>(t => t.OwnerId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.Ticket)
                .WithMany(t => t.Orders)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.Order)
                .WithMany(t => t.Tickets)
                .HasForeignKey(z => z.OrderId);
        }
    }
}
