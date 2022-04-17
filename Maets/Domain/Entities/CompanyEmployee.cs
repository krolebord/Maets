namespace Maets.Domain.Entities;

public class CompanyEmployee : Entity
{
    public Guid UserId { get; set; }

    public Guid CompanyId { get; set; }


    public User? User { get; set; }

    public Company? Company { get; set; }
}
