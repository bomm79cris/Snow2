using System.Globalization;

namespace Snow.Refrigerant.Table
{
    public class SaturatedTable
    {
        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public double Vf { get; set; }
        public double Vfg { get; set; }
        public double Vg { get; set; }
        public double Hf { get; set; }
        public double Hfg { get; set; }
        public double Hg { get; set; }
        public double Sf { get; set; }
        public double Sfg { get; set; }
        public double Sg { get; set; }

        public static List<SaturatedTable> LoadSaturatedTable(string csvPath)
        {
            var table = new List<SaturatedTable>();
            using (var reader = new StreamReader(csvPath))
            {
                string header = reader.ReadLine(); // Skip header
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(',');

                    var row = new SaturatedTable
                    {
                        Pressure = double.Parse(fields[0], CultureInfo.InvariantCulture),
                        Temperature = double.Parse(fields[1], CultureInfo.InvariantCulture),
                        Vf = double.Parse(fields[2], CultureInfo.InvariantCulture),
                        Vfg = double.Parse(fields[3], CultureInfo.InvariantCulture),
                        Vg = double.Parse(fields[4], CultureInfo.InvariantCulture),
                        Hf = double.Parse(fields[5], CultureInfo.InvariantCulture),
                        Hfg = double.Parse(fields[6], CultureInfo.InvariantCulture),
                        Hg = double.Parse(fields[7], CultureInfo.InvariantCulture),
                        Sf = double.Parse(fields[8], CultureInfo.InvariantCulture),
                        Sfg = double.Parse(fields[9], CultureInfo.InvariantCulture),
                        Sg = double.Parse(fields[10], CultureInfo.InvariantCulture)
                    };
                    table.Add(row);
                }
            }
            return table;
        }
    }
}
