using System.Diagnostics;
using Helther.Shared.Entity;
using AppContext = Helther.Shared.Db.AppContext;

namespace Helther.Server.Services;

public class PingService : IHostedService, IDisposable
{
    // DI
    private IServiceScopeFactory  _scopeFactory;

    // Private
    private IDictionary<Service, Timer> _timers = new Dictionary<Service, Timer>();

    public PingService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        new Task(() => CheckNewServices(cancellationToken)).Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var (_, timer) in _timers)
        {
            timer.Change(Timeout.Infinite, 0);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach (var (_, timer) in _timers)
        {
            timer.Dispose();
        }
    }

    private async Task RefreshService(Service service)
    {
        using var scope = _scopeFactory.CreateScope();
        var appContext = scope.ServiceProvider.GetRequiredService<AppContext>();

        var findedService = appContext.Services.FirstOrDefault(s => s.Id == service.Id);
        if (findedService == null)
        {
            await _timers[service].DisposeAsync();
            _timers.Remove(service);
            Debug.WriteLine($"Service: {service.Id} was removed from service list");
        }
        else
        {
            using HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(service.Url),
                Method = HttpMethod.Head
            };
            var resp = await httpClient.SendAsync(request);

            findedService.Status = (int) resp.StatusCode;
            findedService.LastUpdateDateTime = DateTime.Now;
            //findedService.History.Add(new Check
            //{
            //    Status = (int) resp.StatusCode,
            //    Service = service,
            //    CheckDateTime = DateTime.Now
            //});

            appContext.Services.Update(findedService);
            await appContext.SaveChangesAsync();
            
            Debug.WriteLine($"Service: {service.Id} was update");
        }
    }

    private async Task CheckNewServices(CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var appContext = scope.ServiceProvider.GetRequiredService<AppContext>();

        while (!token.IsCancellationRequested)
        {
            appContext.Services.ToList().ForEach(service =>
            {
                if (!_timers.ContainsKey(service))
                {
                    Timer timer = new Timer(state =>
                        RefreshService(service), null, TimeSpan.Zero, TimeSpan.FromSeconds(service.RateInSec));
                    _timers.Add(service, timer);
                }
            });
            await Task.Delay(5000, token);
        }
    }
}