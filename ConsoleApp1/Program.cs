using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            string jsonInString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\n" +
                "<soap:Body><GetInfoForTheProduct xmlns=\"vopak\">" + "\n" +
                "<CodeOfShop>000000009</CodeOfShop>" + "\n" +
                "<Scancode>9000100866484</Scancode>" + "\n" +
                "<CodeOfProduct></CodeOfProduct>" + "\n" +
                "</GetInfoForTheProduct>" + "\n" +
                "</soap:Body>" + "\n" +
                "</soap:Envelope>";
            string uri = "http://1CSRV/utppsu/ws/ws1.1cws";
            var response = client.PostAsync(uri, new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            var responseString = response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(responseString.Result);
            Console.ReadKey();
        }
    }
}
