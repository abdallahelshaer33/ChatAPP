using ChatAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly Appdbcontext _db;

        public usersController(Appdbcontext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> GetUSers()
        {
            var us = await _db.users.ToListAsync();
            return Ok(us);
        }
    }
}
