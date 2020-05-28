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
    public class GuestController : Controller
    {
        private NpgsqlConnection npgSqlConnection;
        public GuestController()
        {
            npgSqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; User ID = postgres; Password = 1234; Database = Hospital; ");
            npgSqlConnection.Open();
        }

        [HttpPost("[action]")]
        public int GetAuth([FromQuery] string Login, string Password)
        {

            npgSqlConnection.Close();
            return 1;
        }

        [HttpPost("[action]")]
        public Object GetReg([FromQuery] string Name, [FromQuery] string Bd, [FromQuery]  int PasSerial, [FromQuery]  int PasNum, [FromQuery]  int Phone,
             [FromQuery] string Password)
        {
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO patients (Name, BD, PassportSerial, PassportNumber, Phone) VALUES ('"
                + Name + "', '" + Bd + "', " + PasSerial + ", " + PasNum + ", " + Phone + ");", npgSqlConnection))
            {
                npgSqlCommand.ExecuteNonQuery();
                npgSqlCommand.Dispose();
            }

            int ID = 0;
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT id FROM patients WHERE name = '" + Name + "' AND passportserial = " +
                PasSerial + " AND passportnumber = " + PasNum + ";", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            ID = Convert.ToInt32(dbDataRecord["id"]);
                        }
                    }
                    npgSqlDataReader.Close();
                }
                npgSqlCommand.Dispose();
            }

            if (ID >= 1000)
            {
                string Login = AdminController.Translit((Name.Split(' ')[0]).ToLower()) + ID.ToString();
                using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO authentication (id, login, passwordhash) VALUES (" +
                    +ID + ",'" + Login + "','" + Password + "');", npgSqlConnection))
                {
                    npgSqlCommand.ExecuteNonQuery();
                    npgSqlCommand.Dispose();
                }
                npgSqlConnection.Close();

                return new stringres() { login = Login };
            }
            else
            {
                npgSqlConnection.Close();
                return "error";
            }
        }

        public class AuthDataView
        {
            public string login;
            public string password;
            public int id;
        }

        public class stringres
        {
            public string login { get; set; }
        }
    }
}
