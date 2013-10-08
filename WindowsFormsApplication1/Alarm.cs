using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Data.SqlClient;

namespace WindowsFormsApplication1
{
    public partial class Alarm : Form
    {
        newAlarm tempAlarm;
        int alarm_index;
        NotificationWithAlarm nwa = new NotificationWithAlarm();
        SqlConnection con;
        public Alarm(SqlConnection c)
        {
            tempAlarm = new newAlarm();
            InitializeComponent();
            con = c;
            updateListBox();
            if (dataGridView1.Rows.Count == 0)
            {
                alarm_index = 1;
                MessageBox.Show("1");
            }
            
            
        }
        private void Alarm_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "0";
           
            dateTimePicker2.Value = System.DateTime.Now;

            dateTimePicker4.Value = System.DateTime.Now.AddMinutes(5);

          

            currentTimeLabel.Text = System.DateTime.Now.ToString();
            updateListBox();
        }
        
        private void updateListBox()
        {
            dataGridView1.Columns[0].HeaderText = "Begin Time";
            dataGridView1.Columns[1].HeaderText = "End Time";
            dataGridView1.Columns[2].HeaderText = "Text Message";
            dataGridView1.Columns[3].HeaderText = "Table Number";
            dataGridView1.Columns[4].HeaderText = "Minutes Left";
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM alarm", con);
            SqlCommandBuilder sb = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds, "Alarm");
            dataGridView1.DataSource = ds.Tables[0];
        }


        public void createNewAlarm(DateTime dateOut, int tableNum, string TextMessage)
        {
            tempAlarm = new newAlarm();
            tempAlarm.Alarm(alarm_index, dateOut, tableNum, TextMessage, con);
            alarm_index++;
            tempAlarm.insertToDB();
            updateListBox();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tempAlarm.secondsLeft++;
            if (tempAlarm.secondsLeft >= 60)
            {
                tempAlarm.minutesLeft--;
                tempAlarm.secondsLeft = 0;
                if (tempAlarm.minutesLeft <= 5)
                {
                    string tempResult = nwa.callNotification(tempAlarm.message, tempAlarm.tableNum);
                    if (tempResult == "snooze")
                    {
                        tempAlarm.Snooze();
                    }
                    else if (tempResult == "done")
                    {
                        tempAlarm.StopAlarm();
                    }
                    else if (nwa.callNotification(tempAlarm.message, tempAlarm.tableNum) == "do nothing")
                    {
                        MessageBox.Show("Smth goes wrong check buttons event handler");
                    }
                    tempAlarm.updateDB();
                    updateListBox();
                }
            }
            tempAlarm.updateDB();
        }
        
        private void StopAndDeleteAlarm(newAlarm alarmNum)
        {
            SqlCommand deleteAlarmFromDB;
            deleteAlarmFromDB = new SqlCommand("DELETE FROM alarm WHERE table_num=@table_num and alarm_num = @alarm_num; ", con);

            deleteAlarmFromDB.Parameters.Add(new SqlParameter("@table_num", SqlDbType.Int));
            deleteAlarmFromDB.Parameters["@table_num"].Value = alarmNum.tableNum;

            deleteAlarmFromDB.Parameters.Add(new SqlParameter("@alarm_num", SqlDbType.Int));
            deleteAlarmFromDB.Parameters["@alarm_num"].Value = alarmNum.alarm_num;

            deleteAlarmFromDB.ExecuteNonQuery();
            alarmNum = null;
            updateListBox();
        }


        private void timeUpdate_Tick(object sender, EventArgs e)
        {
            currentTimeLabel.Text = System.DateTime.Now.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string to = dateTimePicker2.Value.ToString(@"dd/MM/yyyy") + " " + dateTimePicker4.Value.ToString("hh:mm");

            TimeSpan span = DateTime.Parse(to) - System.DateTime.Now;
            if (span.TotalMinutes < 0)
            {
                MessageBox.Show("Wrong time!!! Check the date!!!\nDifference between dates is equal to " + span.ToString());
            }
            else
            {
                createNewAlarm(DateTime.Parse(to), int.Parse(comboBox1.Text), textBox5.Text);
            }
            updateListBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string to = dateTimePicker2.Value.ToString(@"dd/MM/yyyy") + " " + dateTimePicker4.Value.ToString("hh:mm");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
