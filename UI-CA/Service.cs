using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC.BL.Domain;
using System.Net.Http;
using Newtonsoft.Json;


namespace SC.UI.CA
{
    class Service
    {
        private string baseUri = "http://localhost:52506/api/";

        public IEnumerable<TicketResponse> GetTicketResponses(int ticketNumber)
        {
            IEnumerable<TicketResponse> responses = null;

            using (HttpClient http = new HttpClient())
            {
                string uri = baseUri + "TicketResponse/" + ticketNumber;
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                httpRequest.Headers.Add("Accept", "application/json");
                HttpResponseMessage httpResponse = http.SendAsync(httpRequest).Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseContentAsString = httpResponse.Content.ReadAsStringAsync().Result;
                    responses = JsonConvert.DeserializeObject<List<TicketResponse>>(responseContentAsString);
                }
                else
                    throw new Exception(httpResponse.StatusCode + " " + httpResponse.ReasonPhrase);
            }
            return responses;
        }

        
        public TicketResponse AddTicketResponse(int ticketNumber, string response, bool isClientResponse)
        {
            TicketResponse tr = null;
            using (HttpClient http = new HttpClient())
            {
                string uri = baseUri + "TicketResponse";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                object data = new { Ticketnumber = ticketNumber, ResponseText = response, IsClientResponse = isClientResponse };
                string dataAsJson = JsonConvert.SerializeObject(data);
                httpRequest.Content = new StringContent(dataAsJson, Encoding.UTF8, "application/json");
                httpRequest.Headers.Add("Accept", "application/json");
                HttpResponseMessage httpResponse = http.SendAsync(httpRequest).Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseContentAsString = httpResponse.Content.ReadAsStringAsync().Result;
                    tr = JsonConvert.DeserializeObject<TicketResponse>(responseContentAsString);
                }
                else
                    throw new Exception(httpResponse.StatusCode + " " + httpResponse.ReasonPhrase);
            }

            return tr;
        }

        public void ChangeStateToClosed(int ticketNumber)
        {
            using (HttpClient http = new HttpClient())
            {
                string uri = baseUri + "/Ticket/" + ticketNumber + "/State/Closed";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, uri);
                HttpResponseMessage httpResponse = http.SendAsync(httpRequest).Result;
                if(!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception(httpResponse.StatusCode + " " + httpResponse.ReasonPhrase);
                }
            }
        }
    }
}
