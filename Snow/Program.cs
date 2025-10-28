using System;
using Snow.Refrigerant.Table;
using Snow.Refrigerant.Models;

namespace Snow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== SIMULADOR CICLO DE REFRIGERACIÓN ===\n");

            Console.Write("Ingrese el nombre del refrigerante (ej: R134a): ");
            string refrigerant = Console.ReadLine()?.Trim() ?? "R134a";

            Console.Write("Ingrese la temperatura de evaporación [°F]: ");
            double tEvap = double.Parse(Console.ReadLine() ?? "5");

            Console.Write("Ingrese la temperatura de condensación [°F]: ");
            double tCond = double.Parse(Console.ReadLine() ?? "40");

            try
            {
                var table = new RefrigeranteTable(refrigerant, tEvap, tCond);

                Console.WriteLine("\n=== RESULTADOS DEL CICLO ===\n");
                PrintState("H1 - Salida del Evaporador / Entrada al Compresor", table.H1);
                PrintState("H2 - Salida del Compresor", table.H2);
                PrintState("H3 - Salida del Condensador", table.H3);
                PrintState("H4 - Salida de la Válvula de Expansión", table.H4);

                double qEvaporador = table.H1.Enthalpy - table.H4.Enthalpy;
                double qCondensador = table.H2.Enthalpy - table.H3.Enthalpy;
                double trabajoCompresor = table.H2.Enthalpy - table.H1.Enthalpy;
                double cop = qEvaporador / trabajoCompresor;

                Console.WriteLine("\n=== BALANCE ENERGÉTICO ===");
                Console.WriteLine($"Trabajo del compresor (BTU/lbm): {trabajoCompresor:F2}");
                Console.WriteLine($"Calor absorbido en evaporador (BTU/lbm): {qEvaporador:F2}");
                Console.WriteLine($"Calor rechazado en condensador (BTU/lbm): {qCondensador:F2}");
                Console.WriteLine($"COP del ciclo: {cop:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error en la simulación: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }

        private static void PrintState(string title, ThermoState state)
        {
            Console.WriteLine($"\n{title}:");
            Console.WriteLine($"  Fase:        {state.Phase}");
            Console.WriteLine($"  T = {state.Temperature,8:F2} °F");
            Console.WriteLine($"  P = {state.Pressure,8:F2} psi");
            Console.WriteLine($"  h = {state.Enthalpy,8:F2} BTU/lbm");
            Console.WriteLine($"  s = {state.Entropy,8:F4} BTU/lbm·R");
        }
    }
}
