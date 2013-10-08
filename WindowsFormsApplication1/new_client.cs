using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication1
{
    public partial class new_client : Form
    {
        Options opt = new Options();
        private SqlConnection connection;
        private double getPrice;
        private int op_tables = 0;
        private int op_clients = 0;
        private int clientCounter;
        List<string> free_tables;
        List<string> clients_info;
        public new_client(int counter)
        {
            InitializeComponent();
            groupBox2.Enabled = false;
            clientCounter = counter;
            comboBox1.Text = "0";
            comboBox3.Text = "";
            client_date_come.CustomFormat = "dd/MMM/yy";
            client_date_out.CustomFormat = "dd/MMM/yy";
            dateTimePicker1.Value = System.DateTime.Now;
            dateTimePicker2.Value = System.DateTime.Now;
            client_date_come.Value = System.DateTime.Now;
            client_date_out.Value = System.DateTime.Now;
            //client_date_come.Value = System.DateTime.Now;
            //client_date_out.Value = System.DateTime.Now;
            //dateTimePicker1.Value = System.DateTime.Now;
            //dateTimePicker2.Value = System.DateTime.Now;
           
           //MessageBox.Show(client_date_come.Value.ToString("dd/mm/yyyy") + " " + dateTimePicker1.Value.ToString("HH:mm"));
          
            clients_info = new List<string>();
            free_tables = new List<string>();
            GetDataFromDB();
        }
        private void GetDataFromDB()
        {
            using (connection)
            {
                connection = new SqlConnection(@"Server= (local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT table_id FROM tables_info WHERE table_state='free'", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    free_tables.Add(reader.GetInt32(0).ToString());
                }
                reader.Close();
                SqlCommand command2 = new SqlCommand("SELECT customer_id FROM customer", connection);
                SqlDataReader reader2 = command2.ExecuteReader();
                while (reader2.Read())
                {
                    clients_info.Add(reader2.GetString(0).ToString());
                }
                reader2.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Options opt = new Options();
            getPrice = opt.getPrice(int.Parse(comboBox1.Text));
        }

        private void comboBox3_MouseClick(object sender, MouseEventArgs e)
        {
            if (op_tables == 0)
            {
                op_tables = 1;
                for (int i = 0; i < free_tables.Count; i++)
                {
                    comboBox3.Items.Add(free_tables[i]);
                }
            }
        }

        private void comboBox1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (op_clients == 0)
            {
                op_clients = 1;
                for (int i = 0; i < clients_info.Count; i++)
                {
                    comboBox1.Items.Add(clients_info[i]);
                }
            }
             
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
            {
                MessageBox.Show("For request you must fill all fields!!!");
            }
            else
            {
                if (radioButton1.Checked)
                {
                    if (client_date_come.Value.Date > System.DateTime.Now.Date)
                    {
                        MessageBox.Show("Check the date and time!\n" +
                                        " The date you put is is too far from current time!\n" +
                                        " You've entered date: " + dateTimePicker1.Value.ToString("dd.MMM.yyyy"));
                    }
                    else
                    {
                        if (client_date_come.Value.Date != System.DateTime.Now.Date)
                        {
                            DialogResult dr = new System.Windows.Forms.DialogResult();
                            MessageBox.Show("Date is different from current date!\nAre you sure that you put this date?\n" +
                                "Your date is equal to " + client_date_come.Value.ToString("dd.MMM.yyyy") + "\nCurrent date is " + System.DateTime.Now.ToString("dd.MMM.yyyy") +
                                "\nPress 'Yes' if you put correct date\n" +
                                "Press 'No' if you want to put correct date", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (dr == DialogResult.Yes)
                            {
                                clientCounter++;
                                SqlCommand open_table;
                                open_table = new SqlCommand("INSERT INTO new_client(client_num, table_id, time_come, time_out, customer_id, discount, total_cost, client_state)" +
                                                            " VALUES (@client_num, @table_id, @client_time, @client_out, @cust_id, '0', '0', 'opened'); ", connection);
                                open_table.Parameters.Add(new SqlParameter("@client_num", SqlDbType.Int));
                                open_table.Parameters["@client_num"].Value = clientCounter;

                                open_table.Parameters.Add(new SqlParameter("@table_id", SqlDbType.Int));
                                open_table.Parameters["@table_id"].Value = int.Parse(comboBox3.Text);

                                open_table.Parameters.Add(new SqlParameter("@client_time", SqlDbType.DateTime));
                                open_table.Parameters["@client_time"].Value = client_date_come.Value.ToString("dd/MMM/yyyy") + " " + dateTimePicker1.Value.ToString("HH:mm");//textBox1.Text.ToString() + ":" + textBox2.Text.ToString());

                                open_table.Parameters.Add(new SqlParameter("@client_out", SqlDbType.DateTime));
                                open_table.Parameters["@client_out"].Value = client_date_come.Value.ToString("dd/MMM/yyyy") + " " + dateTimePicker1.Value.ToString("HH:mm");//textBox3.Text.ToString() + ":" + textBox4.Text.ToString());

                                open_table.Parameters.Add(new SqlParameter("@cust_id", SqlDbType.VarChar));
                                open_table.Parameters["@cust_id"].Value = comboBox1.Text.ToString();

                                open_table.ExecuteNonQuery();

                                SqlCommand update_table_info;
                                update_table_info = new SqlCommand("UPDATE tables_info SET table_state='busy' WHERE table_id=@table_id", connection);
                                update_table_info.Parameters.Add(new SqlParameter("@table_id", SqlDbType.Int));
                                update_table_info.Parameters["@table_id"].Value = int.Parse(comboBox1.Text);

                                update_table_info.ExecuteNonQuery();
                                comboBox1.Items.Clear();
                                Close();
                            }
                        }
                    }
                }
                else if (radioButton2.Checked)
                {
                    if (System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) < System.DateTime.Now.Date)
                    {
                        MessageBox.Show("Check the date and time!\n" +
                                        " The date you put is is too far from current time!\n" +
                                        " You've entered date: " + dateTimePicker1.Value.ToString("dd.MMM.yyyy"));
                    }
                    else if (int.Parse(textBox5.Text) == 0 ||
                        client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm") == System.DateTime.Now.ToString("dd.MMM.yyyy HH:mm"))
                    {
                        MessageBox.Show("ERROR!\nCheck all fields!!!\nPossible troubles:\n -You didn't change date!\n-Paid sum is equal to zero!");
                    }
                    else
                    {
                        clientCounter++;
                        SqlCommand deposit_table;
                        deposit_table = new SqlCommand("INSERT INTO new_client(client_num, table_id, time_come, time_out, customer_id, discount, total_cost, client_state)" +
                                                    " VALUES (@client_num, @table_id, @client_time, @client_out, @cust_id, '0', '0', 'opened'); ", connection);
                        deposit_table.Parameters.Add(new SqlParameter("@client_num", SqlDbType.Int));
                        deposit_table.Parameters["@client_num"].Value = clientCounter;

                        deposit_table.Parameters.Add(new SqlParameter("@table_id", SqlDbType.Int));
                        deposit_table.Parameters["@table_id"].Value = int.Parse(comboBox3.Text);

                        deposit_table.Parameters.Add(new SqlParameter("@client_time", SqlDbType.DateTime));
                        deposit_table.Parameters["@client_time"].Value = client_date_come.Value.ToString("dd/MMM/yyyy") + " " + dateTimePicker1.Value.ToString("HH:mm");//textBox1.Text.ToString() + ":" + textBox2.Text.ToString());

                        deposit_table.Parameters.Add(new SqlParameter("@client_out", SqlDbType.DateTime));
                        deposit_table.Parameters["@client_out"].Value = client_date_come.Value.ToString("dd/MMM/yyyy") + " " + dateTimePicker1.Value.ToString("HH:mm");//textBox3.Text.ToString() + ":" + textBox4.Text.ToString());

                        deposit_table.Parameters.Add(new SqlParameter("@cust_id", SqlDbType.VarChar));
                        deposit_table.Parameters["@cust_id"].Value = comboBox1.Text.ToString();

                        deposit_table.ExecuteNonQuery();

                        SqlCommand update_table_info;
                        update_table_info = new SqlCommand("UPDATE tables_info SET table_state='busy' WHERE table_id=@table_id", connection);
                        update_table_info.Parameters.Add(new SqlParameter("@table_id", SqlDbType.Int));
                        update_table_info.Parameters["@table_id"].Value = int.Parse(comboBox1.Text);

                        update_table_info.ExecuteNonQuery();
                        comboBox1.Items.Clear();
                        Close();
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                groupBox1.Enabled = true;
                groupBox2.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                groupBox2.Enabled = true;
                groupBox1.Enabled = false;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Options opt = new Options();
            int sum;
            double paidTime = 0;
            if (e.KeyChar != 8 && (e.KeyChar < 48 || e.KeyChar > 57))
            {
                e.Handled = true;
                try
                {
                    sum = int.Parse(textBox5.Text);
                    paidTime = sum / opt.getPrice(int.Parse(comboBox3.Text));
                    dateTimePicker2.Value = System.DateTime.Now.AddMinutes(Math.Round(paidTime));
                    dateTimePicker2.Update();
                    Invalidate();
                }
                catch (FormatException)
                {

                }
            }
            
            
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan ts;
            double sum = 0;
            //MessageBox.Show(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm"));
            if (System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) <
                                                                System.DateTime.Parse(System.DateTime.Now.ToString("dd.MM.yyyy HH:mm")))
            {
                client_date_out.Value = System.DateTime.Parse(System.DateTime.Now.ToString("dd.MMM.yyyy"));
                dateTimePicker2.Value = System.DateTime.Parse(System.DateTime.Now.ToString("HH:mm"));
            }
            else if (System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) !=
                                                                System.DateTime.Parse(System.DateTime.Now.ToString("dd.MM.yyyy HH:mm")))
            {
                try
                {
                    ts = System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) - System.DateTime.Now;//.ToString("dd.MM.yyyy HH:mm");

                    sum = ts.TotalMinutes * opt.getPrice(int.Parse(comboBox3.Text));
                    //MessageBox.Show("timespan = " + ts.TotalMinutes.ToString() + " \nsum = " + sum.ToString());
                    textBox5.Text = Math.Round(sum).ToString();
                }
                catch (Exception)
                {

                }
            }
        }

        private void client_date_out_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan ts;
            double sum = 0;
            //ts = System.DateTime.Parse("9.08.2013 02:39:00").Subtract(System.DateTime.Now);
            //MessageBox.Show("9.08.2013 01:00:00 - "+ System.DateTime.Now.ToString("dd.MM.yyyy HH:mm") +" = " + ts.TotalMinutes.ToString());
            if (System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) <
                                                                System.DateTime.Parse(System.DateTime.Now.ToString("dd.MM.yyyy HH:mm")))
            {
                client_date_out.Value = System.DateTime.Parse(System.DateTime.Now.ToString("dd.MMM.yyyy"));
                dateTimePicker2.Value = System.DateTime.Parse(System.DateTime.Now.ToString("HH:mm"));
            }
            else if (System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) !=
                                                                System.DateTime.Parse(System.DateTime.Now.ToString("dd.MM.yyyy HH:mm")))
            {
                try
                {
                    ts = System.DateTime.Parse(client_date_out.Value.ToString("dd.MMM.yyyy") + " " + dateTimePicker2.Value.ToString("HH:mm")) - System.DateTime.Now;//.ToString("dd.MM.yyyy HH:mm");

                    sum = ts.TotalMinutes * opt.getPrice(int.Parse(comboBox3.Text));
                    //MessageBox.Show("timespan = " + ts.TotalMinutes.ToString() + " \nsum = " + sum.ToString());
                    textBox5.Text = Math.Round(sum).ToString();
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
