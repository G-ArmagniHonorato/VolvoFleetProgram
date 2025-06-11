namespace VolvoFleetProgram.Models.Repositories
{
    public class CacheVehicleRepository : IVehicleRepository
    {
        private readonly Dictionary<ChassisIdentifier, Vehicle> _vehicles = new();

        public void Add(Vehicle vehicle)
        {
            if (Exist(vehicle.ChassisId))
                throw new InvalidOperationException("Já existe um Veiculo com esse Chassi");

            _vehicles[vehicle.ChassisId] = vehicle;
        }

        public void Update(Vehicle vehicle)
        {
            if (!Exist(vehicle.ChassisId))
                throw new InvalidOperationException("Veiculo não Existe");

            _vehicles[vehicle.ChassisId] = vehicle;
        }

        public Vehicle GetByChassisId(ChassisIdentifier chassisId)
        {
            _vehicles.TryGetValue(chassisId, out var vehicle);
            return vehicle;
        }

        public IEnumerable<Vehicle> GetAll()
        {
            return _vehicles.Values;
        }

        public bool Exist(ChassisIdentifier chassisId)
        {
            return _vehicles.ContainsKey(chassisId);
        }
    }
}
