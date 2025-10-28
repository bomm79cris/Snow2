using Snow.Refrigerant.Models;
using Snow.Refrigerant.Table;
using System.ComponentModel;

public class BasicCondenser : IComponent
{
    private readonly RefrigeranteTable props;

    public BasicCondenser(RefrigeranteTable refrigerantProps)
    {
        props = refrigerantProps;
    }

    public string Name => "Condensador";

    public ThermoState Inlet { get; private set; }  
    public ThermoState Outlet { get; private set; }

    public ThermoState Process(ThermoState inletState)
    {
        Inlet = inletState;
        Outlet = props.H3; 
        return Outlet;
    }
}
