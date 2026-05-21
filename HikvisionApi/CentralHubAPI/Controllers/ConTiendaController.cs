using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMPM_CentralHubAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ContiendaController : ControllerBase {
        private readonly AmPmCentralHubContext _dbContext;
        public ContiendaController(AmPmCentralHubContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("getContienda")]
        public async Task<IActionResult> getContienda()
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                var stores = _dbContext.ConTiendas.Where(c => c.Enabled == 1).Select(x => new
                {
                    Id = x.Id,
                    IdStoreHQ = x.IdStoreHQ,
                    StoreName = x.StoreName
                }).ToList();
                if (stores != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { value = stores });

                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "Can´t get data" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = $"Error in controller: \n {ex.Message}" });
            }

        }
    }

}
