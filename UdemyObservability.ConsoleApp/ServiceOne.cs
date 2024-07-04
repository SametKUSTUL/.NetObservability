using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyObservability.ConsoleApp
{
    internal class ServiceOne
    {

        static HttpClient httpClient=new HttpClient();
        internal async Task<int> MakeRequestToGoogle()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity(kind:System.Diagnostics.ActivityKind.Producer,name:"CustomMakeRequestToGoogle");

            try
            {
                var eventTags = new ActivityTagsCollection();
                eventTags.Add("userId", 30);

                activity?.AddEvent(new("GoogleIstekBasladi", tags: eventTags));

                var result = await httpClient.GetAsync("https://www.google.com");
                var responseContent = await result.Content.ReadAsStringAsync();
                eventTags.Add("google body lenght", responseContent.Length);
                activity?.AddEvent(new ActivityEvent("Google istek tamamlandi", tags: eventTags));

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
