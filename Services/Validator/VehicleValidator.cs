using VolvoFleetProgram.Enums;
using VolvoFleetProgram.Models;

namespace VolvoFleetProgram.Services.Validator
{
    public class VehicleValidator
    {
        public void Validate(Vehicle vehicle)
        {
            if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));

            if (vehicle.ChassisId == null) throw new ArgumentException("Chassi é obrigatorio");

            if (vehicle.ChassisId.ChassisNumber < 0) throw new ArgumentException("O numero de chassi nao pode ser negativo");

            if (string.IsNullOrEmpty(vehicle.Color)) throw new ArgumentException("A cor do veiculo nao pode ser vazia");

            if (!Enum.IsDefined(typeof(vehicleType), vehicle.Type)) throw new ArgumentException("Tipo de veiculo nao encontrado");
        }
    }
}
