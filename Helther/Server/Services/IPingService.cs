using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helther.Server.Services;

public interface IPingService
{
    public Task<bool> Ping(int serviceId);
}