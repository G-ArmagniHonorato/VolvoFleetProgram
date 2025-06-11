using VolvoFleetProgram.Models;

namespace VolvoFleetProgram.Services
{
    public interface IVehicleService
    {
        void InsertVehicle(Vehicle vehicle);
        void UpdateVehicle(ChassisIdentifier chassiId, string newColor); 
        Vehicle GetVehicleByChassi(ChassisIdentifier chassiId);
        IEnumerable<Vehicle> GetAllVehicles();
    }
}
