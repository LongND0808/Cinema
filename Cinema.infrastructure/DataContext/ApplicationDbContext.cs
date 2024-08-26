using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Infrastructure.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>,
        UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillFood> BillFoods { get; set; }
        public virtual DbSet<BillStatus> BillStatuses { get; set; }
        public virtual DbSet<BillTicket> BillTickets { get; set; }
        public virtual DbSet<Domain.Entities.Cinema> Cinemas { get; set; }
        public virtual DbSet<ConfirmEmail> ConfirmEmails { get; set; }
        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MovieType> MovieTypes { get; set; }
        public virtual DbSet<Bonus> Promotions { get; set; }
        public virtual DbSet<RankCustomer> RankCustomers { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<SeatStatus> SeatStatuses { get; set; }
        public virtual DbSet<SeatType> SeatTypes { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<UserStatus> UserStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình DeleteBehavior cho các khóa ngoại
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var foreignKeys = entity.GetForeignKeys();
                foreach (var foreignKey in foreignKeys)
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }

        }

        public async Task<int> CommitChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
