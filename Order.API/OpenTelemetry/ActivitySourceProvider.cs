using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.API.OpenTelemetry;

internal static class ActivitySourceProvider
{
    public static ActivitySource Source=null!;
}
