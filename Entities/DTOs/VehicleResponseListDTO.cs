using VolvoFleetProgram.Models;

namespace VolvoFleetProgram.Entities.DTOs
{
    public class VehicleResponseListDTO
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public int Total { get; set; }
    }
}
