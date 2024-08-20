using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Shared
{
    public class OpenTelemetryTraceIdMiddleware
    {
        private readonly RequestDelegate _next;
    }
}
