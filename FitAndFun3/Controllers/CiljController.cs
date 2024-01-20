using FitAndFun.Models;
using FitAndFun.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAndFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CiljController : ControllerBase
    {
        private readonly ICiljService _ciljService;

        public CiljController(ICiljService ciljService)
        {
            _ciljService = ciljService ?? throw new ArgumentNullException(nameof(ciljService));
        }

        [HttpGet("{id}")]
        public IActionResult GetCilj(int id)
        {
            try
            {
                var cilj = _ciljService.GetCiljById(id);

                if (cilj == null)
                {
                    return NotFound();
                }

                return Ok(cilj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddCilj(Cilj cilj)
        {
            try
            {
                _ciljService.AddCilj(cilj);

                return CreatedAtAction(nameof(GetCilj), new { id = cilj.CiljId }, cilj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCilj(int id, Cilj cilj)
        {
            try
            {
                var existingCilj = await _ciljService.GetCiljById(id);
                if (existingCilj == null)
                {
                    return NotFound($"Cilj sa identifikatorom {id} nije pronađen.");
                }

                if (cilj.DatumZavrsetka < existingCilj.DatumPocetka)
                {
                    return BadRequest("Datum završetka ne može biti pre datuma početka.");
                }

                existingCilj.Naziv = cilj.Naziv;
                existingCilj.Opis = cilj.Opis;
                existingCilj.DatumPocetka = cilj.DatumPocetka;
                existingCilj.DatumZavrsetka = cilj.DatumZavrsetka;

                await _ciljService.UpdateCilj(existingCilj);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška prilikom ažuriranja cilja: {ex.Message}");
            }
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteCilj(int id)
        {
            try
            {
                _ciljService.DeleteCilj(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetCiljeviByUser(int userId)
        {
            try
            {
                var ciljevi = _ciljService.GetCiljeviByUserId(userId);

                return Ok(ciljevi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
