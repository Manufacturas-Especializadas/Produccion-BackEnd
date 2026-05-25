using Application.Interfaces;
using Infrastructure.FileStorage;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandController : ControllerBase
    {
        private readonly IExcelParserService _excelParseService;

        private readonly IDemandCalculationService _demandCalculationService;

        public DemandController(
                IExcelParserService excelParserService,
                IDemandCalculationService demandCalculationService)
        {
            _excelParseService = excelParserService;
            _demandCalculationService = demandCalculationService;
        }

        [HttpGet("lines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableLines([FromQuery] DateTime date)
        {
            try
            {
                if (date == default) date = DateTime.Today;

                var lines = await _demandCalculationService.GetAvailableLinesAsync(date);

                return Ok(lines);
            }
            catch(Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Error: {ex.Message}"
                );
            }
        }

        [HttpGet("historical")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistoricalDemand([FromQuery] DateTime date, [FromQuery] string lineName)
        {
            try
            {
                if (date == default || string.IsNullOrWhiteSpace(lineName))
                {
                    return BadRequest("La fecha y la línea son requeridas");
                }

                var results = await _demandCalculationService.GetHistoricalCalculationAsync(date, lineName);

                if (!results.Any())
                {
                    return NotFound("No se encontró programa de producción para esta fecha y línea");
                }

                return Ok(results);
            }
            catch(Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Error: {ex.Message}"
                );
            }
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDemandExcel(IFormFile file, [FromForm] string lineName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha subido ningún archivo o el archivo está vacío.");
            }

            try
            {
                using var stream = file.OpenReadStream();

                var parsedDemand = _excelParseService.ParseDemandExcel(stream);

                if (!parsedDemand.Any())
                {
                    return BadRequest("El archivo se leyó, pero el sistema no detectó datos de producción válidos.");
                }

                var calculationResults = await _demandCalculationService.ProcessDemandAndCalculateOperatorsAsync(parsedDemand, lineName);

                return Ok(calculationResults);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | SQL Error: " + ex.InnerException.Message;
                }

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno: {errorMessage}");
            }
        }
    }
}