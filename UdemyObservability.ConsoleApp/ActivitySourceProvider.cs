using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyObservability.ConsoleApp;

internal static class ActivitySourceProvider
{
    public static ActivitySource Source = new ActivitySource(OpenTelemetryConstants.ActivitySourceName);

    // public static ActivitySource SourceFile = new ActivitySource(OpenTelemetryConstants.ActivitySourceName);
}
