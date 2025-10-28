using System.Globalization;

namespace Snow.Refrigerant.Table
{
    public class SuperHeatedTable
    {
        public double Pressure { get; set; }
        public string Magnitude { get; set; }
        public Dictionary<double, double> Values { get; set; }

        public SuperHeatedTable()
        {
            Values = new Dictionary<double, double>();
        }

        public static List<SuperHeatedTable> LoadSuperheatedTable(string csvPath)
        {
            var list = new List<SuperHeatedTable>();
            using (var reader = new StreamReader(csvPath))
            {
                string header = reader.ReadLine();
                string[] temps = header.Split(',');
                var temperatureColumns = new List<double>();
                for (int i = 2; i < temps.Length; i++)
                    temperatureColumns.Add(double.Parse(temps[i], CultureInfo.InvariantCulture));

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] fields = line.Split(',');
                    var entry = new SuperHeatedTable
                    {
                        Pressure = double.Parse(fields[0], CultureInfo.InvariantCulture),
                        Magnitude = fields[1]
                    };

                    for (int i = 0; i < temperatureColumns.Count && i + 2 < fields.Length; i++)
                    {
                        double value = double.Parse(fields[i + 2], CultureInfo.InvariantCulture);
                        entry.Values[temperatureColumns[i]] = value;
                    }

                    list.Add(entry);
                }
            }

            return list;
        }
    }
}
