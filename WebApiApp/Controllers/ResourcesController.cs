using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        [HttpGet]
        [Route("resource-one")]
        [Authorize(Policy = "apiOne")]
        public IActionResult GetResourceOne()
        {
            return new JsonResult(new { resource = "One" });
        }

        [HttpGet]
        [Route("resource-two")]
        [Authorize(Policy = "apiTwo")]
        public IActionResult GetResourceTwo()
        {
            return new JsonResult(new { resource = "Two" });
        }

        [HttpGet]
        [Route("resource-three")]
        [Authorize(Policy = "apiThree")]
        public IActionResult GetResourceThree()
        {
            return new JsonResult(new { resource = "Three" });
        }

        [HttpGet]
        [Route("resource-four")]
        [Authorize(Policy = "apiFour")]
        public IActionResult GetResourceFour()
        {
            return new JsonResult(new { resource = "Four" });
        }
    }
}
