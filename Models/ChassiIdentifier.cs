namespace VolvoFleetProgram.Models
{
    public class ChassisIdentifier
    {
        public string ChassisSeries { get; set; }
        public int ChassisNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not ChassisIdentifier other)
                return false;

            return ChassisSeries == other.ChassisSeries &&
                   ChassisNumber == other.ChassisNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ChassisSeries, ChassisNumber);
        }
    }

}
