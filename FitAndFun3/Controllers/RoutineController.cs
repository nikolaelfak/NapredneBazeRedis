using FitAndFun.Models;
using FitAndFun.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FitAndFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineService _routineService;

        public RoutineController(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        [HttpGet("{id}")]
        public IActionResult GetRoutine(int id)
        {
            var routine = _routineService.GetRoutineById(id);

            if (routine == null)
            {
                return NotFound();
            }

            return Ok(routine);
        }

        [HttpPost]
        public IActionResult AddRoutine(Routine routine)
        {
            _routineService.AddRoutine(routine);

            return CreatedAtAction(nameof(GetRoutine), new { id = routine.RoutineId }, routine);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetRoutinesByUserId(int userId)
        {
            var routines = _routineService.GetRoutinesByUserId(userId);

            return Ok(routines);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoutine(int id, Routine routine)
        {
            try
            {
                var existingRoutine = await _routineService.GetRoutineById(id);

                if (existingRoutine == null)
                {
                    return NotFound();
                }

                existingRoutine.Name = routine.Name;
                existingRoutine.UserId = routine.UserId;
                existingRoutine.Duration = routine.Duration;
                existingRoutine.Date = routine.Date;
                existingRoutine.Location = routine.Location;
                existingRoutine.ActivityType = routine.ActivityType;
                existingRoutine.AdditionalDescription = routine.AdditionalDescription;

                await _routineService.UpdateRoutine(existingRoutine);

                return Ok(existingRoutine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRoutine(int id)
        {
            try
            {
                _routineService.DeleteRoutine(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
