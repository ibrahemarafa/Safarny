using APIs_Graduation.Models;
using APIs_Graduation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.RegisterAsync(model);

            if (!result.IsAuthenticated)
            {

                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.GetTokenAsync(model);

            if (!result.IsAuthenticated)
            {

                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost("AddRole")]
        [AllowAnonymous]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.AddRoleAsync(model);

            if (string.IsNullOrEmpty(result))
            {

                return BadRequest(result);
            }
            return Ok(model);
        }

    }
}
