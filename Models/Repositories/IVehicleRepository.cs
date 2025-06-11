namespace VolvoFleetProgram.Models.Repositories
{
    public interface IVehicleRepository
    {
        void Add(Vehicle vehicle);
        Vehicle GetByChassisId(ChassisIdentifier chassisId);
        void Update(Vehicle vehicle);
        IEnumerable<Vehicle> GetAll();
        bool Exist(ChassisIdentifier chassisId);
    }
}
