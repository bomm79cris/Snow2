namespace Snow.Refrigerant.Models
{
    public class ThermoState
    {
        public string Name { get; set; }                  
        public double Pressure { get; set; }             
        public double Temperature { get; set; }          
        public double Enthalpy { get; set; }             
        public double Entropy { get; set; }              
        public double SpecificVolume { get; set; }       
        public string Phase { get; set; }                 

        public override string ToString()
        {
            return $"{Name,-3} | P={Pressure,8:F2} kPa | T={Temperature,6:F2} °C | h={Enthalpy,8:F2} kJ/kg | s={Entropy,6:F4} kJ/kg·K | Phase={Phase}";
        }
    }
}
