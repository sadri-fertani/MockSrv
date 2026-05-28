using MockSrv.Application.DTOs;
using Refit;

namespace MockSrv.Web.Services;

public interface IMockServerApi
{
    [Get("/")]
    Task<List<MockRequestResponseDto>> GetAllAsync(CancellationToken cancellationToken);

    [Delete("/{id}")]
    Task<List<MockRequestResponseDto>> DeleteAsync(int id, CancellationToken cancellationToken);

    [Put("/")]
    Task<MockRequestResponseDto> UpdateAsync(MockRequestResponseDto dto, CancellationToken cancellationToken);

    [Post("/")]
    Task<MockRequestResponseDto> AddAsync(MockRequestResponseDto dto, CancellationToken cancellationToken);
}
