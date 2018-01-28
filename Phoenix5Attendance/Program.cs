using Phoenix5Attendance.Crl;
using Phoenix5Attendance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phoenix5Attendance
{
    public partial class Form1 : Form
    {
        private Holiday HolidayInfo = null;
        public Form1()
        {
            InitializeComponent();
            HolidayInfo = new Holiday();
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            label4.Text = "查詢中…";
            if (CboStaff.Items.Count > 0)
            {
                if (CboStaff.SelectedIndex > 0)
                {

                    Clear();
                    List<AttendanceInfo> AttdInfoList = new List<AttendanceInfo>();
                    float Hour = 0;
                    GetData.GetQuery(CboStaff.SelectedItem.ToString(), CboYear.SelectedItem.ToString(), CboMonth.SelectedItem.ToString().Length == 1 ? "0" + CboMonth.SelectedItem.ToString() : CboMonth.SelectedItem.ToString(), ref AttdInfoList);
                    //GetData.GetOverTime(ref AttdInfoList, out Hour);
                    GetData.GetOverTime(ref AttdInfoList, ref HolidayInfo, out Hour);
                    label6.Text = Hour.ToString();

                    foreach (var item in AttdInfoList)
                    {
                        string[] row = { item.StrDaCardDate, item.StrDaCardDay, item.Name, item.StrStatus, item.Remark };

                        var listViewItem = new ListViewItem(row);

                        ListViewShow.Items.Add(listViewItem);
                        label4.Text = "查完了…";
                    }
                }
                else
                {
                    label4.Text = "沒選員工誰知道你要查誰啦!";
                    MessageBox.Show("請選擇員工！");
                }
            }
            else
            {
                label4.Text = "沒人怎麼查詢啦!";
                MessageBox.Show("請先抓資料!");
            }
        }


        private void BtnGetData_Click(object sender, EventArgs e)
        {
            CboStaff.Items.Clear();
            CboStaff.Items.Add("請選擇…");
            CboStaff.SelectedIndex = 0;

            Clear();
            bool IsOk = GetData.Init();
            if (IsOk)
            {
                GetData.GetHoliday(ref HolidayInfo);

                label4.Text = "抓完了!";

                List<Staff> staffList = new List<Staff>();
                GetData.GetStaffName(ref staffList);
                staffList.Sort((x,y) => { return x.Name.CompareTo(y.Name); });
                foreach(var stf in staffList)
                {
                    CboStaff.Items.Add(stf.Name);
                }

            }
            else
            {
                label4.Text = "抓取失敗!";
            }
        }

        private void Clear()
        {
            label4.Text = string.Empty;
            ListViewShow.Items.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            object[] objarray = new object[6];
            objarray[0] = DateTime.Now.Date.Year - 3;

            for (int i= 1; i<= 5; i++)
            {
                objarray[i] = int.Parse(objarray[0].ToString()) + i;
            }

            CboYear.Items.AddRange(objarray);
            CboYear.SelectedIndex = 3;

            CboMonth.SelectedIndex = DateTime.Now.Date.Month - 1;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("========^______^==========\nCiara Tsai \n版本：2018年後 \n========^______^==========", "關於作者");
        }
    }
}
