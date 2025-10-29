using Snow.Components.Interfaces;
using Snow.Refrigerant.Models;
using Snow.Refrigerant.Table;

public class Compressor : IComponent
{
    private readonly RefrigeranteTable props;

    public Compressor(RefrigeranteTable refrigerantProps)
    {
        props = refrigerantProps;
    }

    public string Name => "Compresor";

    public ThermoState Inlet { get; private set; }
    public ThermoState Outlet { get; private set; }

    public double LastHeat => throw new NotImplementedException();

    public double LastWork => throw new NotImplementedException();
    public ThermoState Process(ThermoState inletState)
    {
        Inlet = inletState;
        Outlet = props.H2;
        return Outlet;
    }
    public double GetWork()
    {
        if (Inlet == null || Outlet == null)
            throw new InvalidOperationException("Los estados de entrada y salida deben estar definidos.");

        return Outlet.Enthalpy - Inlet.Enthalpy;
    }
}
