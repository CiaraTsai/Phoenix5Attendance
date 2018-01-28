using Newtonsoft.Json;
using Phoenix5Attendance.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix5Attendance.Crl
{
    public class GetData
    {
        public static string From_FileAddress { get { return ConfigurationManager.AppSettings["OriginalAddress"]; }}

        public static string To_FileAddress { get {return ConfigurationManager.AppSettings["SourceAddress"]; } }

        public static string HolidayAPIAddress { get { return "http://data.ntpc.gov.tw/api/v1/rest/datastore/382000000A-000077-002"; } }

        public static bool IsDone { get; set; }


        public GetData()
        {
            IsDone = false;
        }

        public static bool Init()
        {
            bool IsOk = GetFile();

            return IsOk;
        }

        /// <summary>
        /// 將出勤檔案複製到本地資料夾
        /// </summary>
        /// <returns></returns>
        private static bool GetFile()
        {
            try
            {
                string[] txtList = Directory.GetFiles(From_FileAddress, "*.txt");

                // Copy text files.
                foreach (string f in txtList)
                {

                    // Remove path from the file name.
                    string fName = f.Substring(From_FileAddress.Length);

                    try
                    {
                        if (!File.Exists(To_FileAddress+fName))
                        {
                            // Will not overwrite if the destination file already exists.
                            File.Copy(Path.Combine(From_FileAddress, fName), Path.Combine(To_FileAddress, fName), true);
                          
                        }
                            IsDone = true;
                     
                    }

                    // Catch exception if the file was already copied.
                    catch (IOException copyError)
                    {
                        Console.WriteLine(copyError.Message);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return IsDone;
        }

        /// <summary>
        /// 取得員工清單
        /// </summary>
        /// <param name="StaffList"></param>
        /// <returns></returns>
        public static bool GetStaffName(ref List<Staff> StaffList)
        {
            string[] files = Directory.GetFiles(To_FileAddress, "*.txt");

            StringBuilder strFile = new StringBuilder();

            foreach (string txtName in files)
            {
                int counter = 0;
                string line;

                // Read the file and display it line by line.  
                System.IO.StreamReader file = new System.IO.StreamReader(txtName);
                while ((line = file.ReadLine()) != null)
                {
                    char[] str1 = { ' ', '\t', '\n', '\r', '\n', '\r', '\n' };
                    string[] str = line.Split(str1);

                    if (str.Length > 0)
                    {
                        if (str[0] != string.Empty)
                        {
                            int Count = StaffList.Where(i => i.Name == str[0].ToString()).Count();
                            if (Count == 0)
                            {
                                StaffList.Add(new Staff() { Name = str[0] });
                            }
                        }

                    }
                    counter++;
                }

                file.Close();
                System.Console.WriteLine("There were {0} lines.", counter);
                // Suspend the screen.  
                System.Console.ReadLine();
                

            }
            

            return true;
        }


        /// <summary>
        /// 取得國定假日
        /// </summary>
        /// <param name="HolidayInfo"></param>
        public static void GetHoliday(ref Holiday HolidayInfo)
        {
            try
            {
                if (HolidayInfo == null)
                    HolidayInfo = new Holiday();

                var request = WebRequest.Create(HolidayAPIAddress);

                var response = request.GetResponse() as HttpWebResponse;
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                var srcString = reader.ReadToEnd();
                HolidayInfo = JsonConvert.DeserializeObject<Holiday>(srcString);
                
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 執行條件查詢
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="AttendanceList"></param>
        /// <returns></returns>
        public static bool GetQuery(string Name, string Year, string Month, ref List<AttendanceInfo> AttendanceList)
        {
            try
            {
                if (AttendanceList == null)
                    AttendanceList = new List<AttendanceInfo>();

                string[] files = Directory.GetFiles(To_FileAddress, "*.txt");

                StringBuilder strFile = new StringBuilder();
                int ID = 0;
                foreach (string txtName in files)
                {
                    int counter = 0;
                    string line;

                    if (txtName.Substring(txtName.Length - 12, 4) == Year.ToString() && txtName.Substring(txtName.Length - 8, 2) == Month.ToString())
                    {
                        
                        // Read the file and display it line by line.  
                        System.IO.StreamReader file = new System.IO.StreamReader(txtName);
                        while ((line = file.ReadLine()) != null)
                        {
                            char[] str1 = { ' ', '\t', '\n', '\r', '\n', '\r', '\n' };
                            string[] str = line.Split(str1);
                            //0, 4, 8
                            if (str.Length > 0)
                            {
                                if (str[0] != string.Empty && str[0] == Name)
                                {
                                    DaCardStatus dcs;
                                    string[] str2 = null;

                                    if (str[8] != string.Empty)
                                    {
                                        str2 = str[8].Split(':');
                                    }
                                    else
                                    {
                                        str2 = str[9].Split(':');
                                    }



                                    //Status
                                    if (AttendanceList.Where(i => i.Name == Name && i.StrDaCardDate == (str[4] == string.Empty ? str[5] : str[4])).Count() == 0)
                                    {

                                        if (int.Parse(str2[0]) >= 18)
                                        {
                                            dcs = DaCardStatus.None;
                                        }
                                        else
                                        {
                                            dcs = DaCardStatus.Arrived;
                                        }
                                    }
                                    else
                                    {
                                        dcs = DaCardStatus.Leave;
                                    }

                                    AttendanceList.Add(new AttendanceInfo() { ID = ID++ , Name = str[0], StrDaCardDate = str[4] == string.Empty ? str[5] : str[4], StrDaCardDay = str2[0] + ":" + str2[1] + ":00", Status = dcs, Remark = dcs == DaCardStatus.None ? "未打上班卡" : "" });
                                }

                            }
                            counter++;
                        }

                        file.Close();
                        System.Console.WriteLine("There were {0} lines.", counter);
                        // Suspend the screen.  
                        System.Console.ReadLine();

                    }
                }

                var _obj1 = AttendanceList.GroupBy(info => info.StrDaCardDate)
                        .Select(group => new
                        {
                            Date = group.Key,
                            Count = group.Count()
                        });

                var _obj2 = _obj1.Where(item => item.Count > 2);

                foreach (var item in _obj2)
                {
                    List<AttendanceInfo> _obj3 = AttendanceList.Where(j => j.StrDaCardDate == item.Date).OrderBy(j => j.DaCardDT).ToList();
                    int i = 0;
                    foreach (AttendanceInfo item2 in _obj3)
                    {
                        if (i !=0 && _obj3.Count()-1 != i)
                        {
                            int index = AttendanceList.FindIndex(r => r.StrDaCardDay.Equals(item2.StrDaCardDay));
                            AttendanceList.RemoveAt(index);
                        }
                        i++;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 計算加班時數
        /// </summary>
        /// <param name="AttdInfoList"></param>
        /// <param name="Hour2"></param>
        /// <returns></returns>
        public static bool GetOverTime(ref List<AttendanceInfo> AttdInfoList, out float Hour2)
        {
            bool IsOk = false;

            float Hour = 0;
            try
            {

                foreach(var item in AttdInfoList)
                {
                    string[] str = item.StrDaCardDay.Split(':');

                    //計算是不是周末


                    //OverTime
                    if (int.Parse(str[0]) >= 19)
                    {
                        //七點半的加班
                        if (int.Parse(str[0]) == 19 && int.Parse(str[1]) >= 30)
                        {
                            Hour = Hour + 0.5f;
                        }

                        if (int.Parse(str[0]) > 19)
                        {
                            if (int.Parse(str[0]) - 19 > 0 && int.Parse(str[1]) >= 0 && int.Parse(str[1]) <=30)
                            {
                                Hour = Hour + int.Parse(str[0]) - 19;
                            }
                            else
                            {
                                Hour = Hour + int.Parse(str[0]) - 19 + 0.5f;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Hour2 = Hour;
            return IsOk;
        }


        public static bool GetOverTime(ref List<AttendanceInfo> AttdInfoList, ref Holiday HolidayInfo, out float Hour2)
        {
            bool IsOk = false;

            float Hour = 0;
            try
            {
                Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>();
                foreach (var item in AttdInfoList)
                {
                    string _strDaCardDate = item.StrDaCardDate.Substring(0, 4) + "/" + item.StrDaCardDate.Substring(4, 2) + "/" + item.StrDaCardDate.Substring(6, 2);
                    string[] str = item.StrDaCardDay.Split(':');
                    Holidayrecords holidayobj = new Holidayrecords();
                    int _Count = 0;
                    //計算是不是周末
                    if (HolidayInfo != null)
                    {
                        _Count = HolidayInfo.result.records.Where(i => i.date == Convert.ToDateTime(_strDaCardDate)).Count();
                        holidayobj = HolidayInfo.result.records.Where(i => i.date == Convert.ToDateTime(_strDaCardDate)).FirstOrDefault();
                    }

                    //假日與補班日
                    if (_Count > 0)
                    {
                        //補班日
                        if (holidayobj.IsHoliday == false)
                        {
                            //OverTime
                            if (int.Parse(str[0]) >= 19)
                            {
                                //七點半的加班
                                if (int.Parse(str[0]) == 19 && int.Parse(str[1]) >= 30)
                                {
                                    Hour = Hour + 0.5f;
                                }

                                if (int.Parse(str[0]) > 19)
                                {
                                    if (int.Parse(str[0]) - 19 > 0 && int.Parse(str[1]) >= 0 && int.Parse(str[1]) <= 30)
                                    {
                                        Hour = Hour + int.Parse(str[0]) - 19;
                                    }
                                    else
                                    {
                                        Hour = Hour + int.Parse(str[0]) - 19 + 0.5f;
                                    }
                                }
                            }
                        }
                        //假日
                        else
                        {
                            if (!dictionary.ContainsKey(_strDaCardDate))
                            {
                                //helf
                                if (int.Parse(str[1])> 0 && int.Parse(str[1]) <= 30)
                                {
                                    dictionary.Add(_strDaCardDate, Convert.ToDateTime(str[0] + ":" + 30 + ":00"));
                                }
                                else
                                {
                                    dictionary.Add(_strDaCardDate, Convert.ToDateTime((int.Parse(str[0]) + 1).ToString() + ":" + 00 + ":00"));
                                }                                
                            }
                            else
                            {
                                DateTime _afterdt = new DateTime();

                                //helf
                                if (int.Parse(str[1]) > 0 && int.Parse(str[1]) <= 30)
                                {
                                    _afterdt = Convert.ToDateTime(str[0] + ":" + 00 + ":00");
                                }
                                else
                                {
                                    _afterdt = Convert.ToDateTime(str[0] + ":" + 30 + ":00");
                                }

                                double _dhours = Math.Round(new TimeSpan(_afterdt.Ticks - dictionary[_strDaCardDate].Ticks).TotalHours, 1);
                                dictionary.Remove(_strDaCardDate);
                                Hour = Hour + (float)_dhours;
                            }
                            item.Remark = holidayobj.holidayCategory;
                        }

                    }
                    //一班上班日
                    else
                    {
                        //OverTime
                        if (int.Parse(str[0]) >= 19)
                        {
                            //七點半的加班
                            if (int.Parse(str[0]) == 19 && int.Parse(str[1]) >= 30)
                            {
                                Hour = Hour + 0.5f;
                            }

                            if (int.Parse(str[0]) > 19)
                            {
                                if (int.Parse(str[0]) - 19 > 0 && int.Parse(str[1]) >= 0 && int.Parse(str[1]) <= 30)
                                {
                                    Hour = Hour + int.Parse(str[0]) - 19;
                                }
                                else
                                {
                                    Hour = Hour + int.Parse(str[0]) - 19 + 0.5f;
                                }
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Hour2 = Hour;
            return IsOk;
        }
    }
}
