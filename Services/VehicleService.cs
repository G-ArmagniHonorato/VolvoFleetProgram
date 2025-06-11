

using VolvoFleetProgram.Models;
using VolvoFleetProgram.Models.Repositories;
using VolvoFleetProgram.Services.Validator;

namespace VolvoFleetProgram.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repository;
        private readonly VehicleValidator _validator = new VehicleValidator();

        public VehicleService(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public void InsertVehicle(Vehicle vehicle)
        {
            _validator.Validate(vehicle);

            if (_repository.Exist(vehicle.ChassisId)) throw new InvalidOperationException("Já existe um veiculo com esse chassi");

            vehicle.SetNumberOfPassengersBasedOnType();


            _repository.Add(vehicle);
        }

        public void UpdateVehicle(ChassisIdentifier chassisId, string newColor)
        {
            if (chassisId == null) throw new ArgumentNullException(nameof(chassisId), "Chassi é obrigatorio");

            if (string.IsNullOrEmpty(newColor)) throw new ArgumentException("É necessario adicionar a nova cor");

            var vehicle = _repository.GetByChassisId(chassisId);
            if (vehicle == null)
                throw new InvalidOperationException("Veiculo não Encontrado");

            vehicle.Color = newColor;
            _repository.Update(vehicle);
        }

        public Vehicle GetVehicleByChassi(ChassisIdentifier chassisId)
        {
            if (chassisId == null) throw new ArgumentNullException(nameof(chassisId), "Chassi é obrigatorio");

            return _repository.GetByChassisId(chassisId);
        }

        public IEnumerable<Vehicle> GetAllVehicles()
        {
            return _repository.GetAll();
        }
    }
}
