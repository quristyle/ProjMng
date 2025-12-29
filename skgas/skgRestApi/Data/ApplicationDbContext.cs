using Microsoft.EntityFrameworkCore;
using skgRestApi.Models;

namespace skgRestApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<BirthdayEntry> BirthdayEntries { get; set; } = null!;
    public DbSet<MealMenu> MealMenus { get; set; } = null!;
    public DbSet<MealMenuItem> MealMenuItems { get; set; } = null!;
    public DbSet<MealFeedback> MealFeedbacks { get; set; } = null!;
    public DbSet<WeatherInfo> WeatherInfos { get; set; } = null!;
    public DbSet<ConnectHubItem> ConnectHubItems { get; set; } = null!;
    public DbSet<SfmReport> SfmReports { get; set; } = null!;
    public DbSet<SafetyStandard> SafetyStandards { get; set; } = null!;
    public DbSet<SafetySticker> SafetyStickers { get; set; } = null!;
    public DbSet<SafetyViolation> SafetyViolations { get; set; } = null!;
    public DbSet<AppSetting> AppSettings { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<ReferenceStandard> ReferenceStandards { get; set; } = null!;

    
    public DbSet<UserToken> UserTokens { get; set; } = null!;
    
    public DbSet<LoginHistory> LoginHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ghub");

        

        // Unique Index 설정
        modelBuilder.Entity<SafetyStandard>().HasIndex(s => s.Code).IsUnique();
        modelBuilder.Entity<AppSetting>().HasIndex(a => a.Key).IsUnique();
        modelBuilder.Entity<ReferenceStandard>().HasIndex(r => r.Key).IsUnique();

        // MealMenu -> MealMenuItem 
        modelBuilder.Entity<MealMenu>()
            .HasMany(m => m.Items)
            .WithOne()
            .HasForeignKey("MealMenuId")
            .OnDelete(DeleteBehavior.Cascade);

        // DateOnly  (Postgres date)
        modelBuilder.Entity<BirthdayEntry>().Property(b => b.BirthDate).HasColumnType("date");
        modelBuilder.Entity<MealMenu>().Property(m => m.Date).HasColumnType("date");
        modelBuilder.Entity<SafetyStandard>().Property(s => s.EffectiveDate).HasColumnType("date");

        // UserProfile UserId 길이 제한
        modelBuilder.Entity<UserProfile>().Property(u => u.UserId).HasMaxLength(100);
    }
}