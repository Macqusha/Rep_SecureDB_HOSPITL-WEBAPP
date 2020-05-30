using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace hospital.Controllers
{
    [Route("api/[controller]")]
    public class DoctorController : Controller
    {
        private NpgsqlConnection npgSqlConnection;
        public DoctorController()
        {
            npgSqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; User ID = postgres; Password = 1234; Database = Hospital; ");
            npgSqlConnection.Open();
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("[action]")]
        public IEnumerable<DoctorAppointmentView> Appointment([FromQuery] string DoctorID)
        {
            var result = new List<DoctorAppointmentView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT key, apptime, cabinet, name, phone, bd, passportserial, passportnumber, room, id AS patientid FROM appointment LEFT JOIN patients ON appointment.patient = patients.ID WHERE doctor = " +
                DoctorID + ";", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new DoctorAppointmentView()
                            {
                                key = Convert.ToInt32(dbDataRecord["key"]),
                                apptime = DateTime.Parse(dbDataRecord["apptime"].ToString()),
                                cabinet = Convert.ToInt32(dbDataRecord["cabinet"]),
                                name = dbDataRecord["name"].ToString(),
                                phone = Convert.ToInt32(dbDataRecord["phone"]),
                                bd = DateTime.Parse(dbDataRecord["bd"].ToString()),
                                passportserial = Convert.ToInt32(Crypto.Decrypt(dbDataRecord["passportserial"].ToString())),
                                passportnumber = Convert.ToInt32(Crypto.Decrypt(dbDataRecord["passportnumber"].ToString())),
                                room = doformatINT(dbDataRecord["room"].ToString()),
                                patientid = Convert.ToInt32(dbDataRecord["patientid"]),
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

        private int? doformatINT(string param)
        {
            int? rr;
            if (param == "")
            {
                rr = null;
            }
            else
            {
                rr = int.Parse(param);
            }
            return rr;
        }
        private DateTime? doformatDT(string param)
        {
            DateTime? rr;
            if (param == "")
            {
                rr = null;
            }
            else
            {
                rr = DateTime.Parse(param);
            }
            return rr;
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("[action]")]
        public IEnumerable<DoctorPatientView> Patient([FromQuery] string DoctorID)
        {
            var result = new List<DoctorPatientView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT room, places, name, phone, bd, passportserial, passportnumber, arrival, " +
                "departure, id AS patientid FROM rooms LEFT JOIN patients ON rooms.number = room WHERE name is not null AND fixeddoctor = " +
                DoctorID + ";", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new DoctorPatientView()
                            {
                                room = Convert.ToInt32(dbDataRecord["room"]),
                                places = Convert.ToInt32(dbDataRecord["places"]),
                                name = dbDataRecord["name"].ToString(),
                                phone = Convert.ToInt32(dbDataRecord["phone"]),
                                bd = DateTime.Parse(dbDataRecord["bd"].ToString()),
                                passportserial = Convert.ToInt32(Crypto.Decrypt(dbDataRecord["passportserial"].ToString())),
                                passportnumber = Convert.ToInt32(Crypto.Decrypt(dbDataRecord["passportnumber"].ToString())),
                                arrival = doformatDT(dbDataRecord["arrival"].ToString()),
                                departure = doformatDT(dbDataRecord["departure"].ToString()),
                                patientid = Convert.ToInt32(dbDataRecord["patientid"]),
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

        [Authorize(Roles = "Doctor")]
        [HttpPost("[action]")]
        public void AddDisease([FromQuery] string PatientID, string Code)
        {
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO diagnosis VALUES (" + PatientID + ",'" + Code + "');", npgSqlConnection))
            {
                npgSqlCommand.ExecuteNonQuery();
                npgSqlCommand.Dispose();
            }
            npgSqlConnection.Close();
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("[action]")]
        public IEnumerable<DoctorRoomView> GetFreeRooms([FromQuery] string DoctorID)
        {
            var result = new List<DoctorRoomView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT number, places, places - count(room) AS free FROM rooms " +
                "LEFT JOIN patients ON number = room LEFT JOIN doctors on fixeddoctor = doctors.id WHERE doctors.id = " + 
                DoctorID + " GROUP BY number,doctors.id;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            if (Convert.ToInt32(dbDataRecord["free"]) > 0)
                                result.Add(new DoctorRoomView()
                                {
                                    number = Convert.ToInt32(dbDataRecord["number"]),
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

        [Authorize(Roles = "Doctor")]
        [HttpPost("[action]")]
        public void AddRoom([FromQuery] string PatientID, string Room)
        {
            if (Room != " " && Room != null)
                using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("UPDATE patients SET arrival = now(), departure = null, room = " + Room +
                    " WHERE id = " + PatientID + ";", npgSqlConnection))
                {
                    npgSqlCommand.ExecuteNonQuery();
                    npgSqlCommand.Dispose();
                }
            npgSqlConnection.Close();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("[action]")]
        public void RemoveRoom([FromQuery] string PatientID)
        {
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("UPDATE patients SET departure = now(), room = null WHERE id = " + 
                PatientID + ";", npgSqlConnection))
            {
                npgSqlCommand.ExecuteNonQuery();
                npgSqlCommand.Dispose();
            }
            npgSqlConnection.Close();
        }

        public class DoctorAppointmentView
        {
            public int key { get; set; }
            public DateTime apptime { get; set; }
            public int cabinet { get; set; }
            public string name { get; set; }
            public int phone { get; set; }
            public DateTime bd { get; set; }
            public int passportserial { get; set; }
            public int passportnumber { get; set; }
            public int? room { get; set; }
            public int patientid { get; set; }
        }
        public class DoctorPatientView
        {
            public int room { get; set; }
            public int places { get; set; }
            public string name { get; set; }
            public int phone { get; set; }
            public DateTime bd { get; set; }
            public int passportserial { get; set; }
            public int passportnumber { get; set; }
            public DateTime? arrival { get; set; }
            public DateTime? departure { get; set; }
            public int patientid { get; set; }
        }
        public class DoctorRoomView
        {
            public int number { get; set; }
            public int places { get; set; }
            public int free { get; set; }
        }
    }
}
