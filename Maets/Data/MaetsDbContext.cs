using Maets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Maets.Data;

public class MaetsDbContext : DbContext
{
    public MaetsDbContext()
    {
    }

    public MaetsDbContext(DbContextOptions<MaetsDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<App> Apps { get; set; } = null!;

    public DbSet<AppScreenshot> AppScreenshots { get; set; } = null!;

    public DbSet<AppsDeveloper> AppsDevelopers { get; set; } = null!;

    public DbSet<AppsLabel> AppsLabels { get; set; } = null!;

    public DbSet<Company> Companies { get; set; } = null!;

    public DbSet<Label> Labels { get; set; } = null!;

    public DbSet<MediaFile> MediaFiles { get; set; } = null!;

    public DbSet<Review> Reviews { get; set; } = null!;

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
        });

        modelBuilder.Entity<AppScreenshot>(entity => {
            entity.ToTable("App_Screenshots");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasValueGenerator<GuidValueGenerator>();

            entity.HasOne(x => x.File)
                .WithOne()
                .HasForeignKey<AppScreenshot>(x => x.FileId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("app_screenshots_fileid_foreign");

            entity.HasOne(x => x.App)
                .WithMany(x => x.Screenshots)
                .HasForeignKey(x => x.AppId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("app_screenshots_appid_foreign");
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

            entity.HasMany(c => c.DevelopedApps)
                .WithMany(a => a.Developers)
                .UsingEntity<AppsDeveloper>(
                    configureRight => configureRight
                        .HasOne(d => d.App)
                        .WithMany()
                        .HasForeignKey(d => d.AppId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("apps_developers_appid_foreign"),
                    configureLeft => configureLeft
                        .HasOne(d => d.Company)
                        .WithMany()
                        .HasForeignKey(d => d.CompanyId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("apps_developers_companyid_foreign"),
                    builder => builder
                        .ToTable("Apps_Developers")
                        .Property(x => x.Id)
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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("users_avatarid_foreign");
        });

        modelBuilder.Entity<CompanyEmployee>(entity =>
        {
            entity.ToTable("CompanyEmployees");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<CompanyEmployee>(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("employees_userid_foreign");

            entity.HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employees_companyid_foreign");
        });
    }
}
