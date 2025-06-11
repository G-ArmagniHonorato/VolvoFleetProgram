using Microsoft.AspNetCore.Mvc;
using VolvoFleetProgram.Entities.DTOs;
using VolvoFleetProgram.Entities.Request;
using VolvoFleetProgram.Enums;
using VolvoFleetProgram.Models;
using VolvoFleetProgram.Services;

namespace VolvoFleetProgram.Controllers
{
    [ApiController]
    [Route("api/Vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllVehicles()
        {
            try
            {
                var vehicles = _vehicleService.GetAllVehicles().ToList();
                var response = new VehicleResponseListDTO
                {
                    Vehicles = vehicles,
                    Total = vehicles.Count()
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno: " + ex.Message);
            }
        }

        [HttpGet("Details/{series}/{number}")]
        public IActionResult GetVehicleDetails(string series, int number)
        {
            try
            {
                var chassisId = new ChassisIdentifier
                {
                    ChassisSeries = series,
                    ChassisNumber = number
                };
                var vehicle = _vehicleService.GetVehicleByChassi(chassisId);
                if (vehicle == null) return NotFound();
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno: " + ex.Message);
            }
        }

        [HttpPost("Create")]
        public IActionResult CreateVehicle([FromBody] Vehicle vehicle)
        {
            try
            {
                _vehicleService.InsertVehicle(vehicle);

                return CreatedAtAction(nameof(GetVehicleDetails),
                    new { series = vehicle.ChassisId.ChassisSeries, number = vehicle.ChassisId.ChassisNumber },
                    vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno: " + ex.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdateVehicleColor([FromBody] UpdateVehicleColorRequest request)
        {
            try
            {
                var chassisId = new ChassisIdentifier
                {
                    ChassisSeries = request.ChassisSeries,
                    ChassisNumber = request.ChassisNumber
                };
                var vehicle = _vehicleService.GetVehicleByChassi(chassisId);
                if (vehicle == null) return NotFound();

                _vehicleService.UpdateVehicle(chassisId, request.NewColor);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno: " + ex.Message);
            }
        }
    }

}
