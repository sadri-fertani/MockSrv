using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MockSrv.Application.DTOs;
using MockSrv.Application.Interfaces.Services;
using MockSrv.Domain.Globals;

namespace MockSrv.Api.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class AdminController(ILogger<AdminController> logger, IMockRequestResponseService mockRequestResponseService) : ControllerBase
{
    /// <summary>
    /// Retourne un MockRequest par son id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ActionName(nameof(GetOneAsync))]
    public async Task<ActionResult<MockRequestResponseDto>> GetOneAsync(int id)
    {
        try
        {
            var mock = await mockRequestResponseService.GetAsync(id);

            if (mock == null) return NotFound($"Le MockRequest ayant l'id: {id} est introuvable.");

            return mock;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error {GetOneAsync}", nameof(GetOneAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Retourne la liste des MockRequest
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [ActionName(nameof(GetAllAsync))]
    public async Task<ActionResult<List<MockRequestResponseDto>>> GetAllAsync()
    {
        try
        {
            var mocks = await mockRequestResponseService.GetAsync();
            if (mocks == null) return NotFound("Aucun MockRequest trouvé.");

            return mocks.ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error {GetAllAsync}", nameof(GetAllAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Créer un nouveau MockRequest
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost()]
    [ActionName(nameof(PostAsync))]
    public async Task<ActionResult<MockRequestResponseDto>> PostAsync(MockRequestResponseDto model)
    {
        try
        {
            var newModel = await mockRequestResponseService.AddAsync(model);

            return CreatedAtAction(nameof(GetOneAsync), new { id = newModel.Id }, newModel);
        }
        catch (DbUpdateException ex)
        when
        (
            (ex.InnerException is SqliteException eSqlLite && eSqlLite.SqliteExtendedErrorCode == ErrorCodes.CODE_DUPLICATE_KEY_SQLLITE)
            ||
            (ex.InnerException is SqlException eSqlServer && eSqlServer.Number == ErrorCodes.CODE_DUPLICATE_KEY_SQLSERVER)
        )
        {
            logger.LogError(ex, "Error {PostAsync}", nameof(PostAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, ErrorCodes.DUPLICATE_REQUEST);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error {PostAsync}", nameof(PostAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Modifie un MockRequest existant en remplaçant toutes ses valeurs
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut()]
    [ActionName(nameof(PutAsync))]
    public async Task<ActionResult<MockRequestResponseDto>> PutAsync(MockRequestResponseDto model)
    {
        try
        {
            var mock = await mockRequestResponseService.GetAsync(model!.Id);
            
            if (mock == null) 
                return NotFound($"Le MockRequest ayant l'id: {model!.Id} est introuvable.");
            
            await mockRequestResponseService.UpdateAsync(model);

            return Ok(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error {PutAsync}", nameof(PutAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Supprime un mock existant
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [ActionName(nameof(DeleteAsync))]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var mock = await mockRequestResponseService.GetAsync(id);

            if (mock != null)
            {
                await mockRequestResponseService.DeleteAsync(id);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error {DeleteAsync}", nameof(DeleteAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
