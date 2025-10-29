using Snow.Components.Interfaces;
using Snow.Refrigerant.Models;
using Snow.Refrigerant.Table;

public class ExpansionValve : IComponent
{
    private readonly RefrigeranteTable props;

    public ExpansionValve(RefrigeranteTable refrigerantProps)
    {
        props = refrigerantProps;
    }

    public string Name => "Válvula de Expansión";

    public ThermoState Inlet { get; private set; }   
    public ThermoState Outlet { get; private set; }

    public double LastHeat => throw new NotImplementedException();

    public double LastWork => throw new NotImplementedException();

    public ThermoState Process(ThermoState inletState)
    {
        Inlet = inletState;
        Outlet = props.H4; 
        return Outlet;
    }
}
