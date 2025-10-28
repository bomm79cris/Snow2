using Snow.Refrigerant.Models;
namespace Snow.Refrigerant.Table
{
    public class RefrigeranteTable
    {
        public string Refrigerant { get; private set; }
        public double EvaporationTemperature { get; set; }
        public double CondensationTemperature { get; set; }

        public ThermoState H1 { get; private set; }
        public ThermoState H2 { get; private set; }
        public ThermoState H3 { get; private set; }
        public ThermoState H4 { get; private set; }

        private List<SaturatedTable> saturatedData;
        private List<SuperHeatedTable> superheatedData;

        public RefrigeranteTable(string refrigerant, double tempEvap, double tempCond)
        {
            Refrigerant = refrigerant;
            EvaporationTemperature = tempEvap;
            CondensationTemperature = tempCond;

            saturatedData = SaturatedTable.LoadSaturatedTable("C:\\Users\\TheHackerMan\\source\\repos\\Snow\\Snow\\shared\\NH3Sat.csv");
            superheatedData = SuperHeatedTable.LoadSuperheatedTable("C:\\Users\\TheHackerMan\\source\\repos\\Snow\\Snow\\shared\\NH3REC.csv");

            H1 = BuildH1();
            H3 = BuildH3();
            H4 = BuildH4();
            H2 = BuildH2();

        }
        private ThermoState BuildH1()
        {
            var sat = InterpolateSaturated(EvaporationTemperature);
            return new ThermoState
            {
                Temperature = EvaporationTemperature,
                Pressure = sat.Pressure,
                Enthalpy = sat.Hg,  
                Entropy = sat.Sg,
                Phase = "SaturatedVapor"
            };
        }

        public ThermoState BuildH2()
        {
            if (H1 == null)
                throw new InvalidOperationException("H1 debe estar calculado antes de calcular H2.");
            if (H4 == null)
                throw new InvalidOperationException("H4 debe estar calculado antes de calcular H2.");
            if (superheatedData == null || superheatedData.Count == 0)
                throw new InvalidOperationException("No se han cargado datos de la tabla de vapor recalentado.");

            double s1 = H1.Entropy; // Entropía del punto 1
            double pCond = H3.Pressure;

            // Presiones disponibles en la tabla
            var pressures = superheatedData.Select(x => x.Pressure).Distinct().OrderBy(p => p).ToList();

            if (pCond < pressures.First() || pCond > pressures.Last())
                throw new InvalidOperationException($"Presión {pCond} fuera del rango [{pressures.First()} - {pressures.Last()}].");

            double h2, t2;

            // Buscar presiones adyacentes para interpolar
            double lowerP = pressures.Where(p => p <= pCond).DefaultIfEmpty(pressures.First()).Max();
            double upperP = pressures.Where(p => p >= pCond).DefaultIfEmpty(pressures.Last()).Min();

            (double hLow, double tLow) = GetSafeHAndTAtPressureForEntropy(lowerP, s1);
            (double hHigh, double tHigh) = GetSafeHAndTAtPressureForEntropy(upperP, s1);

            // Interpolación lineal por presión
            h2 = LinearInterpolation(lowerP, hLow, upperP, hHigh, pCond);
            t2 = LinearInterpolation(lowerP, tLow, upperP, tHigh, pCond);

            H2 = new ThermoState
            {
                Pressure = pCond,
                Enthalpy = h2,
                Entropy = s1,
                Temperature = t2,
                Phase = "SuperheatedVapor"
            };

            return H2;
        }



        private ThermoState BuildH3()
        {
            var satCond = InterpolateSaturated(CondensationTemperature);

            return new ThermoState
            {
                Temperature = CondensationTemperature,
                Pressure = satCond.Pressure,
                Enthalpy = satCond.Hf,   
                Entropy = satCond.Sf,
                Phase = "SaturatedLiquid"
            };
        }

        private ThermoState BuildH4()
        {
            var satEvap = InterpolateSaturated(EvaporationTemperature);
            double h4 = H3.Enthalpy;

            double x4 = (h4 - satEvap.Hf) / (satEvap.Hfg);

            return new ThermoState
            {
                Temperature = EvaporationTemperature,
                Pressure = satEvap.Pressure,
                Enthalpy = h4,
                Entropy = satEvap.Sf + x4 * satEvap.Sfg,
                Phase = "TwoPhaseMixture"
            };
        }

        private SaturatedTable InterpolateSaturated(double temperature)
        {
            var exact = saturatedData.FirstOrDefault(x => Math.Abs(x.Temperature - temperature) < 1e-6);
            if (exact != null) return exact;

            var lower = saturatedData.Where(x => x.Temperature < temperature).OrderByDescending(x => x.Temperature).FirstOrDefault();
            var upper = saturatedData.Where(x => x.Temperature > temperature).OrderBy(x => x.Temperature).FirstOrDefault();

            if (lower == null || upper == null)
                throw new Exception("Temperatura fuera del rango de datos de saturación.");

            return new SaturatedTable
            {
                Temperature = temperature,
                Pressure = LinearInterpolation(lower.Temperature, lower.Pressure, upper.Temperature, upper.Pressure, temperature),
                Vf = LinearInterpolation(lower.Temperature, lower.Vf, upper.Temperature, upper.Vf, temperature),
                Vfg = LinearInterpolation(lower.Temperature, lower.Vfg, upper.Temperature, upper.Vfg, temperature),
                Vg = LinearInterpolation(lower.Temperature, lower.Vg, upper.Temperature, upper.Vg, temperature),
                Hf = LinearInterpolation(lower.Temperature, lower.Hf, upper.Temperature, upper.Hf, temperature),
                Hfg = LinearInterpolation(lower.Temperature, lower.Hfg, upper.Temperature, upper.Hfg, temperature),
                Hg = LinearInterpolation(lower.Temperature, lower.Hg, upper.Temperature, upper.Hg, temperature),
                Sf = LinearInterpolation(lower.Temperature, lower.Sf, upper.Temperature, upper.Sf, temperature),
                Sfg = LinearInterpolation(lower.Temperature, lower.Sfg, upper.Temperature, upper.Sfg, temperature),
                Sg = LinearInterpolation(lower.Temperature, lower.Sg, upper.Temperature, upper.Sg, temperature)
            };
        }

        private (double h, double T) GetSafeHAndTAtPressureForEntropy(double pressure, double sTarget)
        {
            var rowsAtP = superheatedData.Where(x => Math.Abs(x.Pressure - pressure) < 1e-6).ToList();
            if (rowsAtP.Count == 0)
                throw new InvalidOperationException($"No hay datos para P={pressure} en la tabla recalentada.");

            var sRow = rowsAtP.FirstOrDefault(r => r.Magnitude.Equals("s", StringComparison.InvariantCultureIgnoreCase));
            var hRow = rowsAtP.FirstOrDefault(r => r.Magnitude.Equals("h", StringComparison.InvariantCultureIgnoreCase));

            if (sRow == null || hRow == null)
                throw new InvalidOperationException($"Faltan filas 's' o 'h' para P={pressure}.");

            var sPoints = sRow.Values.OrderBy(kvp => kvp.Key).ToList();
            var hPoints = hRow.Values.OrderBy(kvp => kvp.Key).ToList();

            double sMin = sPoints.Min(kv => kv.Value);
            double sMax = sPoints.Max(kv => kv.Value);

            // 🔹 Ajustar si la entropía buscada está fuera del rango de la tabla
            if (sTarget < sMin)
            {
                Console.WriteLine($"⚠ Advertencia: s={sTarget:F4} < s_min({sMin:F4}) para P={pressure}. Usando s_min.");
                sTarget = sMin;
            }
            else if (sTarget > sMax)
            {
                Console.WriteLine($"⚠ Advertencia: s={sTarget:F4} > s_max({sMax:F4}) para P={pressure}. Usando s_max.");
                sTarget = sMax;
            }

            // Buscar los dos puntos de entropía que encierran sTarget
            var lower = sPoints.Where(kv => kv.Value <= sTarget).OrderByDescending(kv => kv.Value).First();
            var upper = sPoints.Where(kv => kv.Value >= sTarget).OrderBy(kv => kv.Value).First();

            double frac = (sTarget - lower.Value) / (upper.Value - lower.Value);

            // Interpolar temperatura y entalpía
            double tLow = lower.Key;
            double tHigh = upper.Key;

            double hLow = hRow.Values[tLow];
            double hHigh = hRow.Values[tHigh];

            double tInterp = tLow + frac * (tHigh - tLow);
            double hInterp = hLow + frac * (hHigh - hLow);

            return (hInterp, tInterp);
        }


        private double LinearInterpolation(double x0, double y0, double x1, double y1, double x)
        {
            if (x1 == x0) return y0;
            return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
        }

        public void PrintCycleStates()
        {
            Console.WriteLine("=== Ciclo Ideal - Estados Termodinámicos ===");
            Console.WriteLine($"H1 - Evaporador: {H1}");
            Console.WriteLine($"H2 - Compresor:  {H2}");
            Console.WriteLine($"H3 - Condensador:{H3}");
            Console.WriteLine($"H4 - Expansión:  {H4}");
        }
    }
}
