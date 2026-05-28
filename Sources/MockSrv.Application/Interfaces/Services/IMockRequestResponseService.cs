using MockSrv.Application.DTOs;

namespace MockSrv.Application.Interfaces.Services;

public interface IMockRequestResponseService
{
    Task<IEnumerable<MockRequestResponseDto>> GetAsync();
    Task<MockRequestResponseDto> GetAsync(int id);
    Task<MockRequestResponseDto> AddAsync(MockRequestResponseDto modele);
    Task<MockRequestResponseDto> UpdateAsync(MockRequestResponseDto modele);
    Task DeleteAsync(int id);
}
