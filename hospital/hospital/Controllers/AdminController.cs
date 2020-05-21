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
    public class AdminController : Controller
    {
        private NpgsqlConnection npgSqlConnection;
        public AdminController()
        {
            npgSqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; User ID = postgres; Password = 1234; Database = Hospital; ");
            npgSqlConnection.Open();
        }

        [HttpGet("[action]")]
        public IEnumerable<AdminDoctorView> Doctor([FromQuery] string AdminID)
        {
            var result = new List<AdminDoctorView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT id AS doctorid, doctors.name, phone, address, bd, workstart, workend, positions.name AS position FROM doctors LEFT JOIN positions ON positioncode = key;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminDoctorView()
                            {
                                doctorid = Convert.ToInt32(dbDataRecord["doctorid"]),
                                name = dbDataRecord["name"].ToString(),
                                phone = Convert.ToInt32(dbDataRecord["phone"]),
                                address = dbDataRecord["address"].ToString(),
                                bd = DateTime.Parse(dbDataRecord["bd"].ToString()),
                                workstart = dbDataRecord["workstart"].ToString(),
                                workend = dbDataRecord["workend"].ToString(),
                                position = dbDataRecord["position"].ToString(),
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

        [HttpGet("[action]")]
        public IEnumerable<AdminPatientView> Patient([FromQuery] string AdminID)
        {
            var result = new List<AdminPatientView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT id AS doctorid, name, passportserial, passportnumber FROM patients;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminPatientView()
                            {
                                doctorid = Convert.ToInt32(dbDataRecord["doctorid"]),
                                name = dbDataRecord["name"].ToString(),
                                passportserial = Convert.ToInt32(dbDataRecord["passportserial"]),
                                passportnumber = Convert.ToInt32(dbDataRecord["passportnumber"]),
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

        [HttpGet("[action]")]
        public IEnumerable<AdminCabinetView> Cabinet([FromQuery] string AdminID)
        {
            var result = new List<AdminCabinetView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT number FROM cabinets;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminCabinetView()
                            {
                                number = Convert.ToInt32(dbDataRecord["number"]),
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

        [HttpGet("[action]")]
        public IEnumerable<AdminRoomView> Room([FromQuery] string AdminID)
        {
            var result = new List<AdminRoomView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT number, fixeddoctor, doctors.name, places, places - count(room) AS free FROM rooms LEFT JOIN patients ON number = room LEFT JOIN doctors on fixeddoctor = doctors.id GROUP BY room, number, places, doctors.name;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminRoomView()
                            {
                                number = Convert.ToInt32(dbDataRecord["number"]),
                                fixeddoctor = Convert.ToInt32(dbDataRecord["fixeddoctor"]),
                                name = dbDataRecord["name"].ToString(),
                                places = Convert.ToInt32(dbDataRecord["places"]),
                                free = Convert.ToInt32(dbDataRecord["free"]),
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

        [HttpGet("[action]")]
        public IEnumerable<AdminDiseaseView> Disease([FromQuery] string AdminID)
        {
            var result = new List<AdminDiseaseView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT * FROM diseases;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminDiseaseView()
                            {
                                code = dbDataRecord["code"].ToString(),
                                name = dbDataRecord["name"].ToString(),
                                treatment = dbDataRecord["treatment"].ToString(),
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

        [HttpGet("[action]")]
        public IEnumerable<AdminPositionView> Position([FromQuery] string AdminID)
        {
            var result = new List<AdminPositionView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT * FROM positions;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminPositionView()
                            {
                                key = Convert.ToInt32(dbDataRecord["key"]),
                                name = dbDataRecord["name"].ToString(),
                                salary = Convert.ToInt32(dbDataRecord["salary"])
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

        [HttpGet("[action]")]
        public IEnumerable<AdminAuthView> Auth([FromQuery] string AdminID)
        {
            var result = new List<AdminAuthView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT * FROM authentication;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminAuthView()
                            {
                                login = dbDataRecord["login"].ToString(),
                                passwordhash = dbDataRecord["passwordhash"].ToString(),
                                token = dbDataRecord["token"].ToString(),
                                id = Convert.ToInt32(dbDataRecord["id"])
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

        [HttpGet("[action]")]
        public IEnumerable<AdminAuditView> Audit([FromQuery] string AdminID)
        {
            var result = new List<AdminAuditView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT * FROM audit;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new AdminAuditView()
                            {
                                id = Convert.ToInt32(dbDataRecord["id"]),
                                login = dbDataRecord["login"].ToString(),
                                action = dbDataRecord["action"].ToString(),
                                acttime = DateTime.Parse(dbDataRecord["acttime"].ToString())
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

        public class AdminDoctorView
        {
            public int doctorid { get; set; }
            public string name { get; set; }
            public int phone { get; set; }
            public string address { get; set; }
            public DateTime bd { get; set; }
            public string workstart { get; set; }
            public string workend { get; set; }
            public string position { get; set; }
        }
        public class AdminPatientView
        {
            public int doctorid { get; set; }
            public string name { get; set; }
            public int passportserial { get; set; }
            public int passportnumber { get; set; }
        }
        public class AdminCabinetView
        {
            public int number { get; set; }
        }
        public class AdminRoomView
        {
            public int number { get; set; }
            public int fixeddoctor { get; set; }
            public string name { get; set; }
            public int places { get; set; }
            public int free { get; set; }
        }
        public class AdminDiseaseView
        {
            public string code { get; set; }
            public string name { get; set; }
            public string treatment { get; set; }
        }
        public class AdminPositionView
        {
            public int key { get; set; }
            public string name { get; set; }
            public int salary { get; set; }
        }
        public class AdminAuthView
        {
            public string login { get; set; }
            public string passwordhash { get; set; }
            public string token { get; set; }
            public int id { get; set; }
        }
        public class AdminAuditView
        {
            public int id { get; set; }
            public string login { get; set; }
            public string action { get; set; }
            public DateTime acttime { get; set; }
        }
    }
}
