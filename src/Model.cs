public class RootObject
{
    public ValueObject[] Value { get; set; }
}

public class ValueObject
{
    public string ErrorCode { get; set; }
    public string Id { get; set; }
    public TimeSerie[] TimeSeries { get; set; }
}

public class TimeSerie
{
    public TimeSerieData[] Data { get; set; }
}

public class TimeSerieData
{
    public float? Maximum { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class StorageMetric
{
    public string DatabaseName { get; set; }
    public string ServerName { get; set; }
    public int Size0 { get; set; }
    public int Size1 { get; set; }
    public int DaysBetweenMeasures { get; set; }
}