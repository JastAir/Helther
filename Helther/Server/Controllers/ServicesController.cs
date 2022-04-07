using System.Diagnostics;
using Helther.Client.Models;
using Helther.Server.Services;
using Helther.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using AppContext = Helther.Shared.Db.AppContext;

namespace Helther.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppContext _appContext;
    private readonly IPingService _pingService;

    public ServicesController(AppContext appContext, IPingService pingService)
    {
        _appContext = appContext;
        _pingService = pingService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var allServices = _appContext.Services.ToList();
            return Ok(allServices);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpGet("RefreshStatus/{id}")]
    public async Task<IActionResult> RefreshStatus(int id)
    {
        var result = await _pingService.Ping(id);
        if (result)
        {
            return Ok(new List<Service>());
        }

        return BadRequest();
    }

    [HttpPost("Create")]
    public IActionResult Create(CreatingServiceModel service)
    {
        var createdService = _appContext.Services.Add(new Service
        {
            Name = service.Name,
            Url = service.Url,
            RateInSec = service.Rate
        }).Entity;
        _appContext.SaveChanges();

        RefreshStatus(createdService.Id);
        
        return Ok(createdService);
    }

    [HttpPost("Edit")]
    public IActionResult Edit(Service service)
    {
        _appContext.Services.Update(service);
        _appContext.SaveChanges();
        return Ok(service);
    }

    [HttpDelete("Remove/{id}")]
    public IActionResult Remove(int id)
    {
        var findedService = _appContext.Services.ToList().First(s => s.Id == id);
        _appContext.Services.Remove(findedService);
        _appContext.SaveChanges();
        return Ok();
    }
}