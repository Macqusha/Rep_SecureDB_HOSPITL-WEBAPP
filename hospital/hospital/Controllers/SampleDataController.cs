using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace hospital.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }


    [Route("api/[controller]")]
    public class MyGuestController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<GuestView> GuestViews()
        {
            String connectionParams = "Server=localhost;Port=5432;User ID=postgres;Password=1234;Database=Hospital;";
            NpgsqlConnection npgSqlConnection1 = new NpgsqlConnection(connectionParams);
            npgSqlConnection1.Open();
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("select * from doctors;", npgSqlConnection1);
            var result = new List<GuestView>();
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            if (npgSqlDataReader.HasRows)
            {
                //Заполнение таблицы в приложении записями из таблицы в базе данных
                foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                {
                    result.Add(new GuestView()
                    {
                        col1 = dbDataRecord["Name"].ToString(),
                        col2 = Convert.ToInt32(dbDataRecord["Phone"])
                    });
                }
            }
            npgSqlDataReader.Close();
            npgSqlCommand.Dispose();
            npgSqlConnection1.Close();

            return result;
        }

        public class GuestView
        {
            public string col1 { get; set; }
            public int col2 { get; set; }
        }        
    }
}
