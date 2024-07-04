using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyObservability.ConsoleApp;

internal class ServiceTwo
{
    internal async Task<int> WriteToFile(string text)
    {

        Activity.Current?.SetTag("Guncel Activity", Activity.Current.DisplayName);

        using (var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Server, name: "CustomWriteToFile"))
        {
            await File.WriteAllTextAsync("myFile.txt", text);

            var a= (await File.ReadAllTextAsync("myFile.txt")).Length;
        }

        using (var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Server, name: "CustomWriteToFile2"))
        {
            await File.WriteAllTextAsync("myFile2.txt", text);

            var a2 = (await File.ReadAllTextAsync("myFile.txt")).Length;
        }

        return 0;


    }
}
