using FitAndFun.Models;
using FitAndFun.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FitAndFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NagradeController : ControllerBase
    {
        private readonly INagradeService _nagradeService;

        public NagradeController(INagradeService nagradeService)
        {
            _nagradeService = nagradeService;
        }

        [HttpGet("{id}")]
        public IActionResult GetNagrade(int id)
        {
            var nagrade = _nagradeService.GetNagradeById(id);

            if (nagrade == null)
            {
                return NotFound();
            }

            return Ok(nagrade);
        }

        [HttpPost]
        public IActionResult AddNagrade(Nagrade nagrade)
        {
            _nagradeService.AddNagrade(nagrade);

            return CreatedAtAction(nameof(GetNagrade), new { id = nagrade.NagradeId }, nagrade);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateNagrade(int id, Nagrade nagrade)
        {
            if (id != nagrade.NagradeId)
            {
                return BadRequest();
            }

            _nagradeService.UpdateNagrade(nagrade);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNagrade(int id)
        {
            _nagradeService.DeleteNagrade(id);

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetNagradeByUserId(int userId)
        {
            var nagrade = _nagradeService.GetNagradeByUserId(userId);

            return Ok(nagrade);
        }
    }
}
