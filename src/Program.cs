using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

if (Environment.GetCommandLineArgs().Length < 2)
{
    Console.WriteLine("Path to metrics must be specified in arguments");
    Environment.Exit(1);
}

var pathToMetrics = Environment.GetCommandLineArgs()[1];

Console.WriteLine("Path to metrics: " + pathToMetrics);

var metrics = new List<StorageMetric>();

foreach (var file in Directory.GetFiles(pathToMetrics))
{
    RootObject rootObject = null;

    try
    {
        rootObject = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(file));
    }
    catch (System.Exception ex)
    {
        Console.WriteLine("ERROR: " + file + " -> " + ex.Message);
    }

    if (rootObject != null)
    {
        if (rootObject.Value[0].ErrorCode != "Success")
        {
            Console.WriteLine("ERROR: " + rootObject.Value[0].ErrorCode);
        }
        else
        {
            var validMeasures = rootObject.Value[0].TimeSeries[0].Data.Where(x => x.Maximum != null).ToList();

            if (validMeasures.Count == 0)
            {
                Console.WriteLine("ERROR: no measure for " + file);
            }
            else
            {
                var firstMeasure = validMeasures.OrderBy(x => x.TimeStamp).Take(Math.Min(4, validMeasures.Count)).OrderBy(x => x.Maximum).First();
                var lastMeasure = validMeasures.OrderByDescending(x => x.TimeStamp).Take(Math.Min(4, validMeasures.Count)).OrderBy(x => x.Maximum).First();

                var metric = new StorageMetric()
                {
                    DatabaseName = rootObject.Value[0].Id.Split('/')[10],
                    ServerName = rootObject.Value[0].Id.Split('/')[8],
                    Size0 = (int)(firstMeasure.Maximum.GetValueOrDefault() / 1024 / 1024),
                    Size1 = (int)(lastMeasure.Maximum.GetValueOrDefault() / 1024 / 1024),
                    DaysBetweenMeasures = (int)(lastMeasure.TimeStamp - firstMeasure.TimeStamp).TotalDays
                };

                metrics.Add(metric);
            }
        }
    }
}

var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = ";"
};

using (var writer = new StreamWriter(pathToMetrics + "\\..\\out.csv"))
using (var csv = new CsvWriter(writer, config))
{
    csv.WriteRecords(metrics);
}