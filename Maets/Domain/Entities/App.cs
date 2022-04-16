﻿namespace Maets.Domain.Entities;

public sealed class App : Entity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset? ReleaseDate { get; set; }
    public decimal Price { get; set; }
    public Guid PublisherId { get; set; }


    public Company? Publisher { get; set; }

    public ICollection<AppScreenshot> Screenshots { get; set; } = new HashSet<AppScreenshot>();

    public ICollection<Company> Developers { get; set; } = new HashSet<Company>();

    public ICollection<Label> Labels { get; set; } = new HashSet<Label>();

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}