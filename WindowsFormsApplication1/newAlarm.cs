using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data.SqlClient;
using System.Data;
namespace WindowsFormsApplication1
{
    class newAlarm
    {
        SqlConnection con;
        public DateTime ringTime;   
        TimeSpan timespan;
        public int secondsLeft;
        public int minutesLeft;
        public int tableNum;       //only for checking with client
        public string message;
        public int alarm_num;
        public void Alarm(int An, DateTime dO, int tN, string mess, SqlConnection c)
        {
            alarm_num = An;
            con = c;
            ringTime = dO;
            message = mess;
            tableNum = tN;
            minutesLeft = getMinutes();
        }
        private int getMinutes()
        {
            timespan = ringTime - System.DateTime.Now;
            return int.Parse(timespan.TotalMinutes.ToString());
        }
        public void insertToDB()
        {
            SqlCommand insertToAlarmTableCommand;
            insertToAlarmTableCommand = new SqlCommand("INSERT INTO alarm(alarm_num, table_num, time_out, text_message, minute_left)" +
                                              "VALUES (@alarm_num, @table_num, @time_out, @text_message, @minute_left)", con);

            insertToAlarmTableCommand.Parameters.Add(new SqlParameter("@alarm_num", SqlDbType.Int));
            insertToAlarmTableCommand.Parameters["@alarm_num"].Value = alarm_num;

            insertToAlarmTableCommand.Parameters.Add(new SqlParameter("@table_num", SqlDbType.Int));
            insertToAlarmTableCommand.Parameters["@table_num"].Value = tableNum;

            insertToAlarmTableCommand.Parameters.Add(new SqlParameter("@time_out", SqlDbType.DateTime));
            insertToAlarmTableCommand.Parameters["@time_out"].Value = ringTime.ToString();

            insertToAlarmTableCommand.Parameters.Add(new SqlParameter("@text_message", SqlDbType.VarChar));
            insertToAlarmTableCommand.Parameters["@text_message"].Value = message;

            insertToAlarmTableCommand.Parameters.Add(new SqlParameter("@minute_left", SqlDbType.Int));
            insertToAlarmTableCommand.Parameters["@minute_left"].Value = minutesLeft;

            insertToAlarmTableCommand.ExecuteNonQuery();
        }
        public void updateDB()
        {
            SqlCommand updateAlarmTableCommand;
            updateAlarmTableCommand = new SqlCommand("UPDATE alarm SET minute_left=@minute_left WHERE" +
                        "alarm_num = @alarm_num and time_out=@time_out and  text_message=@text_message and table_num=@table_num", con);

            updateAlarmTableCommand.Parameters.Add(new SqlParameter("@alarm_num", SqlDbType.Int));
            updateAlarmTableCommand.Parameters["@alarm_num"].Value = alarm_num;

            updateAlarmTableCommand.Parameters.Add(new SqlParameter("@table_num", SqlDbType.Int));
            updateAlarmTableCommand.Parameters["@table_num"].Value = tableNum;

            updateAlarmTableCommand.Parameters.Add(new SqlParameter("@time_out", SqlDbType.DateTime));
            updateAlarmTableCommand.Parameters["@time_out"].Value = ringTime.ToString();

            updateAlarmTableCommand.Parameters.Add(new SqlParameter("@text_message", SqlDbType.VarChar));
            updateAlarmTableCommand.Parameters["@text_message"].Value = message;

            updateAlarmTableCommand.Parameters.Add(new SqlParameter("@minute_left", SqlDbType.Int));
            updateAlarmTableCommand.Parameters["@minute_left"].Value = minutesLeft;

            updateAlarmTableCommand.ExecuteNonQuery();
        }
        public void takeDataFromDB()
        {
            //SqlCommand take;
            //take = new SqlCommand("SELECT date_come FROM alarm WHERE ", con);
            //take.Parameters.Add(new SqlParameter("@client_num", SqlDbType.VarChar));
            //take.Parameters["@client_num"].Value = clicked_column_index;

            //double dr = double.Parse(take.ExecuteScalar().ToString()) / 100;//discount
        }
        public void StopAlarm()
        {
            minutesLeft = 0;
            secondsLeft = 0;
        }
        public void Snooze()
        {
            minutesLeft += 5;
        }
    }
}
