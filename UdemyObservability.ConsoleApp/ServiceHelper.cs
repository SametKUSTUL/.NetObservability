namespace UdemyObservability.ConsoleApp;

internal class ServiceHelper
{
    internal async Task Work1()
    {
        using var activity = ActivitySourceProvider.Source.StartActivity(); // SPAN 

        var serviceOne = new ServiceOne();

        Console.WriteLine($"Google dan gelen cevap:{ await serviceOne.MakeRequestToGoogle()}");
        Console.WriteLine("Work1 tamamlandi.");
    }

    internal async Task Work2()
    {
        using var activity = ActivitySourceProvider.SourceFile.StartActivity(); // SPAN 
        activity?.SetTag("work 2 tag", "work 2 tag value");
        activity?.AddEvent(new System.Diagnostics.ActivityEvent("work 2 event"));
    }
}
