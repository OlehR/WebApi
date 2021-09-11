using SharedLib;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class DataPrice
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Article { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal QuantityOpt { get; set; }
        public decimal Rest { get; set; }
        public int ActionType { get; set; }
        public decimal PriceBase { get; set; }
        public decimal MinPercent { get; set; }
        public decimal PriceMin { get; set; }
        public decimal PriceIndicative { get; set; }
        public string PromotionName { get; set; }
        public decimal PriceMain { get; set; }
        public decimal Sum { get; set; }
        public string BarCodes { get; set; }
    }

class Program
    {
        static void Main(string[] args)
        {
            var con = new MsDbConnection();
            var con2 = new MsDbConnection();
            //var cc=con.CreateConnection();
            var res=con.CreateReader("SELECT am.code_wares FROM dbo.dw_am am WHERE am.code_warehouse=118 order by code_wares");
            int i = 0;
            while (res.Read())
            {
                var CodeWares = res.GetInt32(0);
                try
                {
                    if (i % 100 == 0)
                        Console.WriteLine(i.ToString());
                    i++;
                    
                    //Console.Write(CodeWares.ToString() + " ");
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    var client = new HttpClient();
                    string jsonInString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                        "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\n" +
                        "<soap:Body><GetInfoForTheProduct xmlns=\"vopak\">" + "\n" +
                        "<CodeOfShop>000000118</CodeOfShop>" + "\n" +
                        "<Scancode></Scancode>" + "\n" +
                        "<CodeOfProduct>" + CodeWares.ToString() + "</CodeOfProduct>" + "\n" +
                        "</GetInfoForTheProduct>" + "\n" +
                        "</soap:Body>" + "\n" +
                        "</soap:Envelope>";
                    string uri = "http://1CSRV/utppsu/ws/ws1.1cws";
                    var response = client.PostAsync(uri, new StringContent(jsonInString, Encoding.UTF8, "application/json"));
                    var responseString = response.Result.Content.ReadAsStringAsync();
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    //Console.Write(' '+elapsedMs.ToString()+' ');
                    var resS = responseString.Result;
                    resS = resS.Substring(resS.IndexOf(@"-instance"">") + 11);
                    resS = resS.Substring(0, resS.IndexOf("</m:return>"));
                    resS = resS.Replace("&amp;", "&");
                    var varRes = resS.Split(new char[] { ';' });
                    watch = System.Diagnostics.Stopwatch.StartNew();
                    var res2 = con2.CreateReader("SELECT dbo.GetPrice(118, " + CodeWares.ToString() + ", NULL, NULL,1)");
                    res2.Read();
                    watch.Stop();
                    elapsedMs = watch.ElapsedMilliseconds;
                    //Console.WriteLine(' ' + elapsedMs.ToString() + ' ');
                    var res2Str = res2.GetString(0);
                    res2.Close();

                    var objP = JsonConvert.DeserializeObject<DataPrice>(res2Str);

                    decimal Price = varRes[2] == "" ? 0 : Decimal.Parse(varRes[2]);
                    if (Price != objP.Price)
                    {
                        Console.WriteLine(CodeWares.ToString());
                        Console.WriteLine(resS);
                        Console.WriteLine(res2Str);
                        Console.WriteLine();
                    }
                    //Console.WriteLine(responseString.Result);
                }
                catch( Exception e)
                {
                    Console.WriteLine(i.ToString() + " " + CodeWares.ToString()+ " " +e.Message) ;
                }
                
        }
        }
    }
}
