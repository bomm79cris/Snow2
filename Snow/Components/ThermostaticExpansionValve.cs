using Snow.Refrigerant.Models;
using Snow.Refrigerant.Table;
using System.ComponentModel;

public class ExpansionValve : IComponent
{
    private readonly RefrigeranteTable props;

    public ExpansionValve(RefrigeranteTable refrigerantProps)
    {
        props = refrigerantProps;
    }

    public string Name => "Válvula de Expansión";

    public ThermoState Inlet { get; private set; }    // H3
    public ThermoState Outlet { get; private set; }   // H4

    // 🔹 Método principal: expansión isentálpica
    public ThermoState Process(ThermoState inletState)
    {
        Inlet = inletState;
        Outlet = props.H4; // Estado después de la válvula (saturado líquido a baja presión)
        return Outlet;
    }
}
