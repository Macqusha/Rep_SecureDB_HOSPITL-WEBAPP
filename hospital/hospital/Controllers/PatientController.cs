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
    public class PatientController : Controller
    {
        private NpgsqlConnection npgSqlConnection;
        public PatientController()
        {
            npgSqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; User ID = postgres; Password = 1234; Database = Hospital; ");
            npgSqlConnection.Open();
        }

        [Authorize(Roles = "Patient, Doctor")]
        [HttpGet("[action]")]
        public IEnumerable<PatientDiagnosisView> Diagnosis([FromQuery] string PatientID)
        {
            var result = new List<PatientDiagnosisView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT code, name, treatment FROM diseases " +
                "LEFT JOIN diagnosis ON code = diseasecode WHERE patientid = " + PatientID + ";", npgSqlConnection))
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

        [Authorize(Roles = "Patient")]
        [HttpGet("[action]")]
        public IEnumerable<PatientRoomView> Room([FromQuery] string PatientID)
        {
            var result = new List<PatientRoomView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT number, places, doctors.name AS doctor, " +
                "workstart, workend, positions.name AS position FROM patients LEFT JOIN Rooms ON patients.room = rooms.number " +
                "LEFT JOIN doctors ON rooms.fixeddoctor = doctors.id LEFT JOIN Positions ON positioncode = key WHERE number > 0 AND patients.ID = " +
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

        [Authorize(Roles = "Patient")]
        [HttpGet("[action]")]
        public IEnumerable<PatientAppointmentView> Appointment([FromQuery] string PatientID)
        {
            var result = new List<PatientAppointmentView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT key, apptime, name, cabinet, id FROM appointment " +
                "LEFT JOIN Doctors ON appointment.doctor = doctors.ID WHERE patient = " + PatientID + ";", npgSqlConnection))
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

        [Authorize(Roles = "Patient")]
        [HttpGet("[action]")]
        public IEnumerable<PatientDoctorView> Doctor()
        {
            var result = new List<PatientDoctorView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT id, positions.name AS position, doctors.name, " +
                "workstart, workend, hired FROM doctors LEFT JOIN positions on doctors.positionCode = positions.key;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {

                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            result.Add(new PatientDoctorView()
                            {
                                ID = Convert.ToInt32(dbDataRecord["ID"]),
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

        [Authorize(Roles = "Patient, Doctor")]
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

        [Authorize(Roles = "Patient")]
        [HttpGet("[action]")]
        public IEnumerable<FreeDatesView> GetFreeDates([FromQuery] string DoctorID, string Start, string End)
        {
            var result = new List<FreeDatesView>();

            var cabinets = new List<int>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT * FROM cabinets;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            cabinets.Add(Convert.ToInt32(dbDataRecord["number"]));
                        }
                    }
                    npgSqlDataReader.Close();
                }
                npgSqlCommand.Dispose();
            }

            var workstart = Start.Split(':');
            var workend = End.Split(':');
            if (Convert.ToInt32(workend[1]) < 30)
            {
                if (Convert.ToInt32(workend[1]) != 0)
                {
                    workend[0] = (Convert.ToInt32(workend[0]) - 1).ToString();
                    workend[1] = (Convert.ToInt32(workend[1]) + 30).ToString();
                }
                else
                {
                    workend[0] = (Convert.ToInt32(workend[0]) + 23).ToString();
                    workend[1] = (Convert.ToInt32(workend[1]) + 30).ToString();
                }
            }
            else
            {
                workend[1] = (Convert.ToInt32(workstart[1]) - 30).ToString();
            }

            DateTime firstDate = DateTime.Today;
            firstDate = firstDate.AddDays(1);
            firstDate = firstDate.AddHours(Convert.ToInt32(workstart[0]));
            firstDate = firstDate.AddMinutes(Convert.ToInt32(workstart[1]));
            firstDate = firstDate.AddSeconds(Convert.ToInt32(workstart[2]));

            DateTime LastDate = DateTime.Today;
            LastDate = LastDate.AddDays(3);
            LastDate = LastDate.AddHours(Convert.ToInt32(workend[0]));
            LastDate = LastDate.AddMinutes(Convert.ToInt32(workend[1]));
            LastDate = LastDate.AddSeconds(Convert.ToInt32(workend[2]));

            for (DateTime i = firstDate; i < LastDate; i = i.AddMinutes(30))
            {
                if (i.TimeOfDay >= firstDate.TimeOfDay && i.TimeOfDay <= LastDate.TimeOfDay)
                    for (int j = 0; j < cabinets.Count(); j++)
                        result.Add(new FreeDatesView()
                        {
                            date = i.ToString(),
                            cabinet = cabinets[j]
                        });
            }

            var apps = new List<PatientAppointmentView>();
            using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT apptime, cabinet, doctor FROM appointment;", npgSqlConnection))
            {
                using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader())
                {
                    if (npgSqlDataReader.HasRows)
                    {
                        foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                        {
                            apps.Add(new PatientAppointmentView()
                            {
                                AppTime = DateTime.Parse(dbDataRecord["AppTime"].ToString()),
                                Cabinet = Convert.ToInt32(dbDataRecord["Cabinet"]),
                                DoctorID = Convert.ToInt32(dbDataRecord["Doctor"])
                            });
                        }
                    }
                    npgSqlDataReader.Close();
                }
                npgSqlCommand.Dispose();
            }

            for (int i = 0; i < apps.Count; i++)
                for (int j = 0; j < result.Count; j++)
                {
                    if (DateTime.Parse(result[j].date) == apps[i].AppTime)
                    {
                        if (apps[i].DoctorID == Convert.ToInt32(DoctorID) || result[j].cabinet == apps[i].Cabinet)
                        {
                            result.Remove(result[j]);
                            j--;
                        }

                    }
                }

            npgSqlConnection.Close();
            return result;
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("[action]")]
        public void AddAppointment([FromQuery] string DoctorID, string PatientID, string Date, string Cabinet)
        {
            if (Date != "" && Date != null)
            {
                //Проверка
                bool allCorrect = true;
                using (NpgsqlCommand npgSqlCommand1 = new NpgsqlCommand("SELECT apptime, cabinet, doctor FROM appointment;", npgSqlConnection))
                {
                    using (NpgsqlDataReader npgSqlDataReader = npgSqlCommand1.ExecuteReader())
                    {
                        if (npgSqlDataReader.HasRows)
                        {
                            foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                            {
                                if (DateTime.Parse(dbDataRecord["AppTime"].ToString()) == DateTime.Parse(Date))
                                {
                                    if ((DoctorID == dbDataRecord["Doctor"].ToString()) || (Cabinet == dbDataRecord["Cabinet"].ToString()))
                                    {
                                        allCorrect = false;
                                    }
                                }
                            }
                        }
                        npgSqlDataReader.Close();
                    }
                    npgSqlCommand1.Dispose();
                }

                if (allCorrect)
                {
                    using (NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO appointment (cabinet,doctor,patient,apptime) " +
                        "VALUES (" + Cabinet + "," + DoctorID + "," + PatientID + ",'" + Date + "');", npgSqlConnection))
                    {
                        npgSqlCommand.ExecuteNonQuery();
                        npgSqlCommand.Dispose();
                    }
                }
                npgSqlConnection.Close();
            }
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
            public int ID { get; set; }
            public string Position { get; set; }
            public string Name { get; set; }
            public string WorkStart { get; set; }
            public string WorkEnd { get; set; }
            public DateTime Hired { get; set; }
        }
        public class FreeDatesView
        {
            public string date;
            public int cabinet;
        }
    }
}
