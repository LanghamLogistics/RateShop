using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RateShop
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var signInResult = await TokenAuthenticate();
            var rateRequest = CreateRequest();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + signInResult.AccessToken);
                    request.Method = HttpMethod.Post;
                    string json = JsonConvert.SerializeObject(rateRequest);
                    HttpResponseMessage result = default(HttpResponseMessage);
                    request.RequestUri = new Uri("https://api.elangham.com/api/v1/RateShop/GetRates");

                    using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = content;
                        result = await client.SendAsync(request);
                        var rateResult = await result.Content.ReadAsStringAsync();
                        Console.WriteLine(rateResult);
                    }
                }
            }
        }

        static RateRequest CreateRequest()
        {
            var request = new RateRequest();

            request.ShipperZip = "46268";
            request.ConsigneeZip = "30336";
            request.PickupDate = new System.DateTime(2021, 9, 1);
            request.RatingLevel = ""; // <--Your account number to be provided by Langham Project Management Team
            request.ServiceFlags = new string[] { "000600" };  // <-- Delivery Appointment Required

            var item = new RateRequestItem();
            item.DimensionUOM = "IN";
            item.FreightClass = "50";
            item.Height = 10;
            item.ItemizeType = "Total";
            item.Length = 10;
            item.Quantity = 1;
            item.QuantityUOM = "CAS";
            item.Weight = 25;
            item.Width = 10;
            request.Items.Add(item);

            return request;
        }

        static async Task<SignInResult> TokenAuthenticate()
        {
            var authData = new List<KeyValuePair<string, string>>();
            authData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            authData.Add(new KeyValuePair<string, string>("userName", "<Your Username>")); // <-- Register for an account on https://warehouse.elangham.com and supply that username/password
            authData.Add(new KeyValuePair<string, string>("password", "<Your Password>"));

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(authData);
                var result = await client.PostAsync("https://api.elangham.com/token", content);
                string json = await result.Content.ReadAsStringAsync();

                var signInResult = JsonConvert.DeserializeObject<SignInResult>(json);
                return signInResult;
            }
        }
    }

    public class SignInResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public uint ExpiresIn { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty(".issued")]
        public DateTimeOffset Issued { get; set; }
        [JsonProperty(".expires")]
        public DateTimeOffset Expires { get; set; }
    }

    public class RateRequest
    {
        public string RatingLevel { get; set; }
        public DateTime PickupDate { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public string ShipperCity { get; set; }
        public string ShipperState { get; set; }
        public string ShipperZip { get; set; }
        public string ShipperCountry { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeState { get; set; }
        public string ConsigneeZip { get; set; }
        public string ConsigneeCountry { get; set; }
        public string[] ServiceFlags { get; set; }
        public List<RateRequestItem> Items { get; set; } = new List<RateRequestItem>();
    }

    public class RateRequestItem
    {
        public string ItemCode { get; set; }
        public string FreightClass { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public int Quantity { get; set; }
        public string QuantityUOM { get; set; }
        public string DimensionUOM { get; set; } = "";
        public string ItemizeType { get; set; } = "";
    }
}