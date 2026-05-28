using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockSrv.Application.DTOs;
using MockSrv.Application.Interfaces.DbContextes;
using MockSrv.Application.Interfaces.Services;
using MockSrv.Domain.Entities;

namespace MockSrv.Application.Services;

public class MockRequestResponseService : IMockRequestResponseService
{
    protected IApplicationDbContext _applicationDbContexte;

    protected ILogger<MockRequestResponseService> _logger;

    protected IMapper _mapper;

    public MockRequestResponseService(
        ILogger<MockRequestResponseService> logger,
        IMapper mapper,
        IApplicationDbContext applicationDbContexte)
    {
        _logger = logger;
        _mapper = mapper;
        _applicationDbContexte = applicationDbContexte;
    }

    public async Task<MockRequestResponseDto> AddAsync(MockRequestResponseDto modele)
    {
        var entity = _mapper.Map<MockEntity>(modele);

        await _applicationDbContexte.MockRequests.AddAsync(entity);
        await (_applicationDbContexte as DbContext)!.SaveChangesAsync();

        return _mapper.Map<MockRequestResponseDto>(entity);
    }

    public async Task<MockRequestResponseDto> UpdateAsync(MockRequestResponseDto modele)
    {
        var entity = _mapper.Map<MockEntity>(modele);

        MockEntity? existing = await _applicationDbContexte.MockRequests.FindAsync(entity.Id);

        if (existing != null)
        {
            (_applicationDbContexte as DbContext)!.Entry(existing).State = EntityState.Modified;
            (_applicationDbContexte as DbContext)!.Entry(existing).CurrentValues.SetValues(entity);

            await (_applicationDbContexte as DbContext)!.SaveChangesAsync();

            return _mapper.Map<MockRequestResponseDto>(existing);
        }

        return modele;
    }

    public async Task<IEnumerable<MockRequestResponseDto>> GetAsync()
    {
        var lst = await _applicationDbContexte.MockRequests.ToListAsync();

        return _mapper.Map<IEnumerable<MockRequestResponseDto>>(lst);
    }

    public async Task<MockRequestResponseDto> GetAsync(int id)
    {
        var mock = await _applicationDbContexte.MockRequests.FirstOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<MockRequestResponseDto>(mock);
    }

    public async Task DeleteAsync(int id)
    {
        var mock = await _applicationDbContexte.MockRequests.FindAsync(id);
        if (mock != null)
        {
            (_applicationDbContexte as DbContext)!.Remove(mock);
            await (_applicationDbContexte as DbContext)!.SaveChangesAsync();
        }
    }
}
