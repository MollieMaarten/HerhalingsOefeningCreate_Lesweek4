using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FunctionApp1
{
    public static class BezoekersService
    {
        private static string CONNECTIONSTRING = Environment.GetEnvironmentVariable("ConnectionString");

        [FunctionName("BezoekersService")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "HttpTriggerCSharp/BezoekersService/{name}")]HttpRequestMessage req, string name, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }

        [FunctionName("GetDagen")]
        public static async Task<HttpResponseMessage> GetDagen([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days/")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                List<string> days = new List<string>();

                using (SqlConnection connection = new SqlConnection(CONNECTIONSTRING))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT DISTINCT DagVanDeWeek FROM Bezoekers";
                        command.CommandText = sql;

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            days.Add(reader["DagVanDeWeek"].ToString());
                        }
                    }
                }
                return req.CreateResponse(HttpStatusCode.OK, days);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
