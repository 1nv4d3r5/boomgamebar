using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class NotificationWithAlarm : Form
    {
        string result = "Do nothing";
        System.Media.SoundPlayer player;
        public NotificationWithAlarm()
        {
            InitializeComponent();
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            player = new System.Media.SoundPlayer("beep.wav");
            player.PlayLooping();
        }
        public string callNotification(string message, int table_num)
        {
            if (table_num == 0)
            {
                textBox1.Text = message;
            }
            else
            {
                textBox1.Text = message + " " + table_num;
            }
            textBox1.TextAlign = HorizontalAlignment.Center;
            ShowDialog();
            return result;
        }

        private void SnoozeButton(object sender, EventArgs e)
        {
            result = "snooze";
            player.Stop();
            Close();
        }

        private void done_button(object sender, EventArgs e)
        {
            result = "done";
            player.Stop();
            Close();
        }
    }
}
