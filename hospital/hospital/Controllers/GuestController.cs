using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        private async Task Authenticate(UserData user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.role)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // Authentification cookies setup
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost("[action]")]
        public async Task<IEnumerable<UserData>> GetAuth([FromQuery] string Login, string Password)
        {
            var result = new List<UserData>();
            Login = Regex.Replace(Login.ToLower(), "[^a-z0-9]", "");
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
                            result.Add(new UserData()
                            {
                                id = Convert.ToInt32(dbDataRecord["id"]),
                                name = choosename(dbDataRecord),
                                role = chooserole(Convert.ToInt32(dbDataRecord["id"]))
                            });
                        }
                    }
                    
                    if (result.Count != 1 || result[0].name == "error" || result[0].id < 1)
                    {
                        if (result.Count == 0)
                        {
                            result.Add(new UserData()
                            {
                                id = 0,
                                name = "error",
                                role = "error"
                            });
                        }
                    }
                    else
                    {
                        await Authenticate(result[0]);
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
        string chooserole(int id)
        {
            if (id >= 1 && id < 100) return "Admin";
            if (id >= 100 && id < 1000) return "Doctor";
            if (id >= 1000) return "Patient";
            return "Guest";
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

        public class UserData
        {
            public int id;
            public string name;
            public string role;
        }
        public class stringres
        {
            public string login { get; set; }
        }
    }
}
