using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session.TimeSeries;
using Raven.TestDriver;
using Xunit;

namespace TimeSeriesRepro;

public class TimeSeriesTests : RavenTestDriver
{
    [Fact]
    public async Task Can_Read_NameTimeSeries_Back()
    {
        var documentStore = GetDocumentStore(database: Guid.NewGuid().ToString());
        using var session = documentStore.OpenAsyncSession();

        var document = new MyDocument("TEST");
        await session.StoreAsync(document);
        var ts = session.TimeSeriesFor<StoreMetrics>(document);
        ts.Append(DateTime.Now, new StoreMetrics());
        await session.SaveChangesAsync();
        
        var readBack = await ts.GetAsync();
        Assert.NotNull(readBack);
    }

    [Fact]
    public async Task Can_Read_TimeSeries_Back()
    {
        var documentStore = GetDocumentStore(database: Guid.NewGuid().ToString());
        using var session = documentStore.OpenAsyncSession();

        var document = new MyDocument("TEST");
        await session.StoreAsync(document);
        var ts = session.TimeSeriesFor<StoreMetrics>(document);
        ts.Append(DateTime.Now, new StoreMetrics());
        await session.SaveChangesAsync();

        var readBack = await session.TimeSeriesFor(document, "StoreMetrics").GetAsync();
        Assert.NotNull(readBack);
    }
}

public record MyDocument(string Id);

public struct StoreMetrics
{
    [TimeSeriesValue(0)]
    public double Downloads { get; set; }

    [TimeSeriesValue(1)]
    public double ReDownloads { get; set; }

    [TimeSeriesValue(2)]
    public double Uninstalls { get; set; }

    [TimeSeriesValue(3)]
    public double Updates { get; set; }

    [TimeSeriesValue(4)]
    public double Returns { get; set; }
}