using AMPM_CentralHubAPI.Utilities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMPM_CentralHubAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogoController : ControllerBase {
        private readonly AmPmCentralHubContext _dbContext;
        public CatalogoController(AmPmCentralHubContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("getCatalogo")]
        public async Task<IActionResult> getCatalogo([FromBody] string tipo)
        {
            try
            {
                if (_dbContext.Database.CanConnect() == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
                var catalogos = _dbContext.Catalogos.Where(c=> c.Tipo.Equals(tipo)).ToList();
                if (catalogos != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { value = catalogos });

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
