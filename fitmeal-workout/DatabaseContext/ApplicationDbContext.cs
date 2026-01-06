using fitmeal_workout.Models;
using Microsoft.EntityFrameworkCore;

namespace fitmeal_workout.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<OrderModel> ordersDetails { get; set; }
        public DbSet<createOrderRequest> userPlanDetails { get; set; }

        public DbSet<ExceptionDetails> ExceptionDetails { get; set; }
    }
}
