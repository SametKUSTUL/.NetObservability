using System.Diagnostics;

namespace UdemyObservability.ConsoleApp
{
    internal class ServiceOne
    {

        static HttpClient httpClient = new HttpClient();
        internal async Task<int> MakeRequestToGoogle()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Producer, name: "CustomMakeRequestToGoogle");

            try
            {
                var eventTags = new ActivityTagsCollection();
                eventTags.Add("userId", 30);

                activity?.AddEvent(new("GoogleIstekBasladi", tags: eventTags));
                activity?.AddTag("request.schema", "https"); // Activity tag ekleme
                activity?.AddTag("request.method", "get");


                var result = await httpClient.GetAsync("https://www.google.com");
                var responseContent = await result.Content.ReadAsStringAsync();
                eventTags.Add("google body lenght", responseContent.Length);
                activity?.AddEvent(new ActivityEvent("Google istek tamamlandi", tags: eventTags)); // activity nin içindeki event a tag ekleme

                activity?.AddTag("response.lenght", responseContent.Length);


                var serviceTwo = new ServiceTwo();
                var fileLenght = await serviceTwo.WriteToFile("Merhaba Dunya");

                return responseContent.Length;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return -1;
            }



        }
    }
}
