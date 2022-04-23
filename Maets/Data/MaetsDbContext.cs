using Maets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Maets.Data;

public class MaetsDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<App> Apps { get; set; } = null!;

    public DbSet<AppScreenshot> AppScreenshots { get; set; } = null!;

    public DbSet<AppsDeveloper> AppsDevelopers { get; set; } = null!;

    public DbSet<AppsLabel> AppsLabels { get; set; } = null!;

    public DbSet<Company> Companies { get; set; } = null!;

    public DbSet<Label> Labels { get; set; } = null!;

    public DbSet<MediaFile> MediaFiles { get; set; } = null!;

    public DbSet<Review> Reviews { get; set; } = null!;

    public DbSet<CompanyEmployee> CompanyEmployees { get; set; } = null!;

    public MaetsDbContext() {}

    public MaetsDbContext(DbContextOptions<MaetsDbContext> options)
        : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>(entity =>
        {
            entity.ToTable("Apps");

            entity.HasIndex(e => e.Title, "apps_title_unique")
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Description).HasDefaultValueSql("(N'')");

            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Publisher)
                .WithMany(p => p.PublishedApps)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("apps_publisherid_foreign");

            entity.HasMany(a => a.Developers)
                .WithMany(c => c.DevelopedApps)
                .UsingEntity<AppsDeveloper>(
                configureRight => configureRight
                    .HasOne(d => d.Company)
                    .WithMany()
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("apps_developers_companyid_foreign"),
                configureLeft => configureLeft
                    .HasOne(d => d.App)
                    .WithMany()
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("apps_developers_appid_foreign"),
                builder => builder
                    .ToTable("Apps_Developers")
                    .Property(x => x.Id)
                    .ValueGeneratedOnAdd()
                    .HasValueGenerator<GuidValueGenerator>()
                );
            
            entity.HasOne(d => d.MainImage)
                .WithOne()
                .HasForeignKey<App>(d => d.MainImageId)
                .HasConstraintName("apps_mainimageid_foreign");

            entity.HasMany(d => d.Screenshots)
                .WithMany("Apps")
                .UsingEntity<AppScreenshot>(
                    configureLeft => configureLeft
                        .HasOne(x => x.File)
                        .WithMany()
                        .HasForeignKey(x => x.FileId)
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("app_screenshots_fileid_foreign"),
                    configureRight => configureRight
                        .HasOne(x => x.App)
                        .WithMany()
                        .HasForeignKey(x => x.AppId)
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("app_screenshots_appid_foreign"),
                    builder => builder
                        .ToTable("App_Screenshots")
                        .Property(x => x.Id)
                        .ValueGeneratedOnAdd()
                        .HasValueGenerator<GuidValueGenerator>()
                );
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Companies");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Description).HasDefaultValueSql("(N'')");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Photo)
                .WithMany()
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("companies_photoid_foreign");

            entity.HasMany(x => x.Employees)
                .WithMany(x => x.Companies)
                .UsingEntity<CompanyEmployee>(
                    configureRight => configureRight
                        .HasOne(x => x.User)
                        .WithMany()
                        .HasForeignKey(u => u.UserId)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("employees_userid_foreign"),
                    configureLeft => configureLeft
                        .HasOne(x => x.Company)
                        .WithMany()
                        .HasForeignKey(x => x.CompanyId)
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("employees_companyid_foreign"),
                    builder => builder
                        .ToTable("CompanyEmployees")
                        .Property(e => e.Id)
                        .ValueGeneratedOnAdd()
                        .HasValueGenerator<GuidValueGenerator>()
                );
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.ToTable("Labels");

            entity.HasIndex(e => e.Name, "labels_name_unique")
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(48);

            entity.HasMany(l => l.Apps)
                .WithMany(a => a.Labels)
                .UsingEntity<AppsLabel>(
                    configureRight => configureRight
                        .HasOne(x => x.App)
                        .WithMany()
                        .HasForeignKey(d => d.AppId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("apps_labels_appid_foreign"),
                    configureRight => configureRight
                        .HasOne(x => x.Label)
                        .WithMany()
                        .HasForeignKey(d => d.LabelId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("apps_labels_labelid_foreign"),
                    builder => builder
                        .ToTable("Apps_Labels")
                        .Property(e => e.Id)
                        .ValueGeneratedOnAdd()
                        .HasValueGenerator<GuidValueGenerator>()
                );
        });

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.ToTable("MediaFiles");

            entity.HasIndex(e => e.Key, "mediafiles_key_unique")
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Key).HasMaxLength(255);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Description).HasDefaultValueSql("(N'')");

            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.App)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.AppId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reviews_appid_foreign");

            entity.HasOne(d => d.Author)
                .WithMany()
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reviews_authorid_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasIndex(e => e.UserName).IsUnique();

            entity.HasOne(d => d.Avatar)
                .WithMany()
                .HasForeignKey(d => d.AvatarId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_avatarid_foreign");
        });
    }
}
