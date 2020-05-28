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
        public IEnumerable<AuthDataView> GetAuth([FromQuery] string Login, string Password)
        {
            var result = new List<AuthDataView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT authentication.id, doctors.name as doctor, " +
                "patients.name AS patient, admins.name AS admin FROM authentication " +
                "LEFT JOIN admins ON authentication.id = admins.id LEFT JOIN doctors ON authentication.id = doctors.id " +
                "LEFT JOIN patients ON authentication.id = patients.id " +
                "WHERE login = '" + Login + "' AND passwordhash = '" + Password + "';", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AuthDataView()
                            {
                                id = Convert.ToInt32(dbDataRecord["id"]),
                                name = choosename(dbDataRecord)
                            });
                        }
                    }
                    
                    if (result.Count != 1 || result[0].name == "error" || result[0].id < 1)
                    {
                        if (result.Count == 0)
                        {
                            result.Add(new AuthDataView()
                            {
                                id = 0,
                                name = "error"
                            });
                        }
                    }
                    npgSqlDataReader.Close();
                }
                npgSqlCommand.Dispose();
            }
            npgSqlConnection.Close();
            return result;
        }

        string choosename(DbDataRecord db)
        {
            int id = Convert.ToInt32(db["id"]);
            if (id >= 1 && id < 100) return db["admin"].ToString();
            if (id >= 100 && id < 1000) return db["doctor"].ToString();
            if (id >= 1000) return db["patient"].ToString();
            return "error";
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
            public int id;
            public string name;
        }
        public class stringres
        {
            public string login { get; set; }
        }
    }
}
