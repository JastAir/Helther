using Helther.Shared.Db;
using Helther.Shared.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppContext = Helther.Shared.Db.AppContext;

namespace Helther.Server.Services;

public class PingService : IPingService
{
    private readonly AppContext _appContext;

    public PingService(AppContext appContext)
    {
        _appContext = appContext;
    }
    public async Task<bool> Ping(int serviceId)
    {
        var findedService = _appContext.Services.FirstOrDefault(s => s.Id == serviceId);
        if (findedService != null)
        {
            using HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(findedService.Url),
                Method = HttpMethod.Head
            };

            try
            {
                var resp = await httpClient.SendAsync(request);

                findedService.Status = (int)resp.StatusCode;
                findedService.LastUpdateDateTime = DateTime.Now;

                _appContext.Services.Update(findedService);
                await _appContext.SaveChangesAsync();

                Debug.WriteLine($"Service: {findedService.Id} was update");
                return true;
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == null)
                {
                    findedService.Status = 404;
                }
                else
                {
                    findedService.Status = (int)e.StatusCode;
                }

                findedService.LastUpdateDateTime = DateTime.Now;

                _appContext.Services.Update(findedService);
                await _appContext.SaveChangesAsync();
                return false;
            }
        }

        return false;
    }
}