using Snow.Refrigerant.Models;

namespace Snow.Components.Interfaces
{
    public interface IComponent
    {
        /// <summary>Procesa el estado de entrada y devuelve el estado de salida.</summary>
        ThermoState Process(ThermoState inlet);

        /// <summary>Último flujo de calor positivo si aplica (kJ/kg o BTU/lbm según unidad usada).</summary>
        double LastHeat { get; }

        /// <summary>Último trabajo (>0 para trabajo realizado por el compresor).</summary>
        double LastWork { get; }
    }

}
