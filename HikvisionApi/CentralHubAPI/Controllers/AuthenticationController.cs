using AMPM_CentralHubAPI.Request.User;
using AMPM_CentralHubAPI.Utilities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMPM_CentralHubAPI.Controllers {
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly JwtCripto _jwtCripto;
        private readonly AmPmCentralHubContext _dbContext;
        public AuthenticationController(JwtCripto jwtCripto, AmPmCentralHubContext dbContext) {
            _jwtCripto = jwtCripto;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(GetUserRequest getUser) {
            if(_dbContext.Database.CanConnect() == false)
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message ="Can´t Connect to Database"});
            var usuario= _dbContext.Usuarios.Where(u => u.userName == getUser.name 
                && u.password == _jwtCripto.criptSHA384(getUser.password)).FirstOrDefault();
            if (usuario != null) {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "OK" });
                
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, message = "Invalid User Password" });
            }
        }
        [HttpPost]
        [Route("getToken")]
        public async Task<IActionResult> getToken(GetUserRequest getUser)
        {
            if (_dbContext.Database.CanConnect() == false)
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
            var usuario = _dbContext.Usuarios.Where(u => u.userName == getUser.name
                && u.password == _jwtCripto.criptSHA384(getUser.password)).FirstOrDefault();
            if (usuario != null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _jwtCripto.GenerateJwtToken(getUser.id, getUser.name) });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, token = "" });
            }
        }

        //[HttpGet]
        //[Route("getUsuario")]
        //public async Task<IActionResult> getUsuarios()
        //{
        //    if (_dbContext.Database.CanConnect() == false)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Can´t Connect to Database" });
        //    var usuarios = _dbContext.Usuarios.ToList();
        //    if (usuarios != null)
        //    {
        //        return StatusCode(StatusCodes.Status200OK, new { value= usuarios });

        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, message = "Invalid User Password" });
        //    }
        //}
    }
}
