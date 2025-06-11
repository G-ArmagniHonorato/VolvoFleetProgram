using VolvoFleetProgram.Enums;

namespace VolvoFleetProgram.Models
{
    public class Vehicle
    {
        public ChassisIdentifier ChassisId { get; set; }
        public vehicleType Type { get; set; }
        public int NumberPassengers { get; private set; }
        public string Color { get; set; }

        public Vehicle()
        {
        }

        public Vehicle(vehicleType type, ChassisIdentifier chassisId, string color)
        {
            ChassisId = chassisId;
            Type = type;
            Color = color;
            SetNumberOfPassengersBasedOnType();
        }


        public void SetNumberOfPassengersBasedOnType()
        {
            NumberPassengers = Type switch
            {
                vehicleType.Bus => 42,
                vehicleType.Truck => 1,
                vehicleType.Car => 4,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    }
}
