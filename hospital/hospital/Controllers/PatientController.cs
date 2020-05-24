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
    public class PatientController : Controller
    {
        private NpgsqlConnection npgSqlConnection;
        public PatientController()
        {
            npgSqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; User ID = postgres; Password = 1234; Database = Hospital; ");
            npgSqlConnection.Open();
        }

        [HttpGet("[action]")]
        public IEnumerable<PatientDiagnosisView> Diagnosis([FromQuery] string PatientID)
        {
            var result = new List<PatientDiagnosisView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT code, name, treatment FROM diseases LEFT JOIN diagnosis ON code = diseasecode WHERE patientid = " +
                PatientID + ";", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new PatientDiagnosisView()
                            {
                                Name = dbDataRecord["Name"].ToString(),
                                Treatment = dbDataRecord["Treatment"].ToString(),
                                Code = dbDataRecord["Code"].ToString()
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
        public IEnumerable<PatientRoomView> Room([FromQuery] string PatientID)
        {
            var result = new List<PatientRoomView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT number, places, doctors.name AS doctor, workstart, workend, positions.name AS position FROM patients LEFT JOIN Rooms ON patients.room = rooms.number LEFT JOIN doctors ON rooms.fixeddoctor = doctors.id LEFT JOIN Positions ON positioncode = key WHERE patients.ID = " +
                PatientID + ";", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new PatientRoomView()
                            {
                                Number = Convert.ToInt32(dbDataRecord["Number"]),
                                Places = Convert.ToInt32(dbDataRecord["Places"]),
                                Doctor = dbDataRecord["Doctor"].ToString(),
                                WorkStart = dbDataRecord["WorkStart"].ToString(),
                                WorkEnd = dbDataRecord["WorkEnd"].ToString(),
                                Position = dbDataRecord["Position"].ToString()
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
        public IEnumerable<PatientAppointmentView> Appointment([FromQuery] string PatientID)
        {
            var result = new List<PatientAppointmentView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT key, apptime, name, cabinet, id FROM appointment LEFT JOIN Doctors ON appointment.doctor = doctors.ID WHERE patient = " +
                PatientID + ";", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new PatientAppointmentView()
                            {
                                Key = Convert.ToInt32(dbDataRecord["Key"]),
                                AppTime = DateTime.Parse(dbDataRecord["AppTime"].ToString()),
                                Name = dbDataRecord["Name"].ToString(),
                                Cabinet = Convert.ToInt32(dbDataRecord["Cabinet"]),
                                DoctorID = Convert.ToInt32(dbDataRecord["ID"])
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
        public IEnumerable<PatientDoctorView> Doctor()
        {
            var result = new List<PatientDoctorView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT positions.name AS position, doctors.name, workstart, workend, hired FROM doctors LEFT JOIN positions on doctors.positionCode = positions.key;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new PatientDoctorView()
                            {
                                Position = dbDataRecord["Position"].ToString(),
                                Name = dbDataRecord["Name"].ToString(),
                                WorkStart = dbDataRecord["WorkStart"].ToString(),
                                WorkEnd = dbDataRecord["WorkEnd"].ToString(),
                                Hired = DateTime.Parse(dbDataRecord["Hired"].ToString()),
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

        [HttpDelete("[action]")]
        public void DelAppointment([FromQuery] string key)
        {
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("DELETE FROM appointment WHERE key = " +
                key + ";", npgSqlConnection))
            {
                npgSqlCommand.ExecuteNonQuery();
                npgSqlCommand.Dispose();
            }
            npgSqlConnection.Close();
        }


        public class PatientDiagnosisView
        {
            public string Name { get; set; }
            public string Treatment { get; set; }
            public string Code { get; set; }
        }
        public class PatientRoomView
        {
            public int Number { get; set; }
            public int Places { get; set; }
            public string Doctor { get; set; }
            public string WorkStart { get; set; }
            public string WorkEnd { get; set; }
            public string Position { get; set; }
        }
        public class PatientAppointmentView
        {
            public int Key { get; set; }
            public DateTime AppTime { get; set; }
            public string Name { get; set; }
            public int Cabinet { get; set; }
            public int DoctorID { get; set; }
        }
        public class PatientDoctorView
        {
            public string Position { get; set; }
            public string Name { get; set; }
            public string WorkStart { get; set; }
            public string WorkEnd { get; set; }
            public DateTime Hired { get; set; }
        }
    }
}
