using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SimpleWebApiConsoleTest
{

class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Wait for Web.Api-Start");
            Console.ReadLine();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync("https://localhost:7001/api/AcceptedCreditCards");
            var paymentMethods = await response.Content.ReadFromJsonAsync<List<string>>();

            paymentMethods?.ForEach(Console.WriteLine);

            Console.ReadKey();
        }
    }
}
