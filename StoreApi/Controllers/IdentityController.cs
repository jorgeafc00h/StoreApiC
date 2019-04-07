using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class IdentityController : ControllerBase
    {

        /// <summary>
        /// Returns claims for any kind of user authenticated
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Get()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToArray();
            return Ok(new { message = "Hello API", claims });
        }

        /// <summary>
        /// Role based authentication should Just Admin Role
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("Adminclaims",Name = "AdminClaims")]
        [Authorize(AuthenticationSchemes = "Bearer" ,Roles ="Admin")]
        public IActionResult GetAdminClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToArray();
            return Ok(new { message = "Claims For Administrator", claims });
        }

        /// <summary>
        /// Role based authentication should Just User Role
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("UserClaims", Name = "UserClaims")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public IActionResult GetUserClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToArray();
            return Ok(new { message = "Claims for User API", claims });
        }
    }
}
