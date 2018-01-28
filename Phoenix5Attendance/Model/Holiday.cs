using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix5Attendance.Model
{
    public class Holiday
    {
        public bool Success { get; set; }

        public HolidayResult result { get; set; }
        public Holiday()
        {
            Success = false;
            result = new HolidayResult();
        }
    }

    public class HolidayResult
    {
        public string resource_id { get; set; }

        public int limit { get; set; }

        public int total { get; set; }

        public List<HolidayFields> fields { get; set; }

        public List<Holidayrecords> records { get; set; }
        public HolidayResult()
        {
            resource_id = string.Empty;
            limit = 0;
            total = 0;
            fields = new List<HolidayFields>();
            records = new List<Holidayrecords>();
        }
    }

    public class HolidayFields
    {
        public string type { get; set; }
        public string id { get; set; }

        public HolidayFields()
        {
            type = string.Empty;
            id = string.Empty;
        }
    }

    public class Holidayrecords
    {
        public DateTime date { get; set; }
        public string name { get; set; }
        public string isHoliday { get; set; }
        public bool IsHoliday
        {
            get
            {
                bool _result = false;
                switch(isHoliday)
                {
                    case "是":
                        _result = true;
                        break;
                    case "否":
                        _result = false;
                        break;
                    default:
                        _result = true;
                        break;
                }
                return _result;
            }
        }
        public string holidayCategory { get; set; }
        public string description { get; set; }

        public Holidayrecords()
        {
            name = string.Empty;
            isHoliday = string.Empty;
            holidayCategory = string.Empty;
            description = string.Empty;
        }
    }
}
