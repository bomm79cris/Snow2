using Snow.Components.Interfaces;
using Snow.Refrigerant.Models;
using Snow.Refrigerant.Table;

public class BasicEvaporator : IComponent
{
    private readonly RefrigeranteTable props;

    public BasicEvaporator(RefrigeranteTable refrigerantProps)
    {
        props = refrigerantProps;
    }

    public string Name => "Evaporador";

    public ThermoState Inlet { get; private set; }  
    public ThermoState Outlet { get; private set; } 

    public ThermoState Process(ThermoState inletState)
    {
        Inlet = inletState;
        Outlet = props.H1; 
        return Outlet;
    }

    public double LastHeat => throw new NotImplementedException();

    public double LastWork => throw new NotImplementedException();
}
