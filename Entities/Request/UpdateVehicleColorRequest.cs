namespace VolvoFleetProgram.Entities.Request
{
    public class UpdateVehicleColorRequest
    {
        public string ChassisSeries { get; set; }
        public int ChassisNumber { get; set; }
        public string NewColor { get; set; }
    }
}
