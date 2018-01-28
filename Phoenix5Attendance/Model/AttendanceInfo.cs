using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix5Attendance.Model
{
    public enum DaCardStatus
    {
        None = -1,
        Arrived = 1,
        Leave = 2
    }

    public class AttendanceInfo : Staff
    {
        public int ID { get; set; }
        public string StrDaCardDT { get; set; }
        
        public string StrDaCardDate { get; set; }

        public string StrDaCardDay { get; set; }
        public DaCardStatus Status { get; set; }

        public string StrStatus
        {
            get
            {
                string _strStatus = string.Empty;
                switch (Status)
                {
                    case DaCardStatus.Arrived:
                        _strStatus = "簽到";
                        break;
                    case DaCardStatus.Leave:
                        _strStatus = "簽退";
                        break;
                    default:
                        _strStatus = "無定義";
                        break;
                }
                return _strStatus;
            }
        }

        public string Remark { get; set; }

        public float OverTimeHour { get; set; }
        
        public DateTime DaCardDT
        { get { return new DateTime(
                            int.Parse(StrDaCardDate.Substring(0, 4)),
                            int.Parse(StrDaCardDate.Substring(4, 2)),
                            int.Parse(StrDaCardDate.Substring(6, 2)),
                            int.Parse(StrDaCardDay.Substring(0, 2)),
                            int.Parse(StrDaCardDay.Substring(3, 2)),
                            int.Parse(StrDaCardDay.Substring(6, 2))); ;
            } }
        public AttendanceInfo()
        {
            StrDaCardDT = string.Empty;
            StrDaCardDate = string.Empty;
            StrDaCardDay = string.Empty;
            OverTimeHour = 0;
            Remark = string.Empty;
            Status = DaCardStatus.None;
        }
    }
}
