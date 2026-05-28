using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockSrv.Domain.Entities;

namespace MockSrv.Application.Interfaces.DbContextes;

public interface IApplicationDbContext
{
    public DatabaseFacade Database { get; }

    public DbSet<MockEntity> MockRequests { get; set; }

    public int SaveChanges();
}
