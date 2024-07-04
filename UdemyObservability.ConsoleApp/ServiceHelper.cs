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
}
