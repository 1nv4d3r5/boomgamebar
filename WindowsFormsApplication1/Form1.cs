using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
//using Microsoft.Office.Interop.Excel;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private SqlConnection con;
        Alarm a;
        Options opt;
        int clicked_column_index;
        private int clientCounter;

        public Form1()
        {
            InitializeComponent();
            clicked_column_index = -1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateData();
            //MessageBox.Show(dataGridView1.Rows[0].Cells[2].Value.ToString());
            //MessageBox.Show((Convert.ToDateTime(dataGridView1.Rows[0].Cells[2].Value.ToString())).ToString());
            this.MinimumSize = this.MaximumSize;
            clientCounter = dataGridView1.Rows.Count;
            //MessageBox.Show(clientCounter.ToString());
            
            dataGridView1.Columns[0].HeaderText = "Client #";
            dataGridView1.Columns[1].HeaderText = "Table #";
            dataGridView1.Columns[2].HeaderText = "Time start";
            dataGridView1.Columns[3].HeaderText = "Time out";
            dataGridView1.Columns[4].HeaderText = "Discount ID";
            dataGridView1.Columns[5].HeaderText = "Discount sum";
            dataGridView1.Columns[6].HeaderText = "Total sum";
            dataGridView1.Columns[7].HeaderText = "Table state";
        }
        private void UpdateData()
        {
            con = new SqlConnection(@"Server= (local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM new_client", con);
            SqlCommandBuilder sb = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds, "new_client");
            dataGridView1.DataSource = ds.Tables[0];
            for (int i = 0; i < dataGridView1.Columns.Count-1; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value == null)
                {
                    continue;
                }
                else if (row.Cells[2].Value.ToString() == row.Cells[3].Value.ToString())
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
            clientCounter = dataGridView1.Rows.Count;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            new_client nc = new new_client(clientCounter);
            nc.ShowDialog();
            UpdateData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Table_info ti = new Table_info(con);
            ti.ShowDialog();
            UpdateData();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            opt = new Options();
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please choose any field!");
            }
            else if (clicked_column_index != -1 && dataGridView1.CurrentRow.Cells[2].Value.ToString() == dataGridView1.CurrentRow.Cells[3].Value.ToString())
            {
                if (MessageBox.Show("Are ou sure that you want to close table#" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + " ?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SqlCommand update_client_info;
                    update_client_info = new SqlCommand("UPDATE new_client SET time_out = @cur_time, client_state = 'closed' WHERE client_num = @client_num;", con);
                    update_client_info.Parameters.Add(new SqlParameter("@client_num", SqlDbType.Int));
                    update_client_info.Parameters["@client_num"].Value = clicked_column_index;

                    update_client_info.Parameters.Add(new SqlParameter("@cur_time", SqlDbType.DateTime));
                    update_client_info.Parameters["@cur_time"].Value = System.DateTime.Now;

                    update_client_info.ExecuteNonQuery();
                    DoesClientHasDiscount dchd = new DoesClientHasDiscount(clicked_column_index, dataGridView1.CurrentRow.Cells[4].Value.ToString());
                    dchd.ShowDialog();

                    UpdateData();

                    double price = (opt.getPrice(int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString())));

                    SqlCommand command2;
                    command2 = new SqlCommand("(SELECT discount FROM customer WHERE customer_id=(SELECT customer_id FROM new_client WHERE client_num = @client_num))", con);
                    command2.Parameters.Add(new SqlParameter("@client_num", SqlDbType.VarChar));
                    command2.Parameters["@client_num"].Value = clicked_column_index;

                    double dr = double.Parse(command2.ExecuteScalar().ToString()) / 100;//discount
                    //MessageBox.Show("discount in % = " + (dr).ToString());

                    SqlCommand difference_between_two_dates = new SqlCommand("(SELECT CAST(DATEDIFF(minute,(SELECT time_come from new_client WHERE client_num = @client_num), (SELECT time_out from new_client WHERE client_num = @client_num )) AS FLOAT(56)));", con);
                    difference_between_two_dates.Parameters.Add(new SqlParameter("@client_num", SqlDbType.Int));
                    difference_between_two_dates.Parameters["@client_num"].Value = clicked_column_index;

                    double dr2 = double.Parse(difference_between_two_dates.ExecuteScalar().ToString());//difference between two times

                    //MessageBox.Show("difference between two dates= " + dr2);
                    double discount = dr * (dr2 * price);

                    // MessageBox.Show("Discount in money= " + discount.ToString());
                    SqlCommand free_closed_table;
                    free_closed_table = new SqlCommand("UPDATE tables_info SET table_state='free' WHERE table_id=(SELECT table_id from new_client WHERE client_num = @client_num)" +
                                             "UPDATE new_client SET  discount = @discount, total_cost = @total_sum WHERE client_num = @client_num;", con);
                    free_closed_table.Parameters.Add(new SqlParameter("@client_num", SqlDbType.Int));
                    //MessageBox.Show(clicked_column_index.ToString());
                    free_closed_table.Parameters["@client_num"].Value = clicked_column_index;

                    free_closed_table.Parameters.Add(new SqlParameter("@discount", SqlDbType.Int));
                    free_closed_table.Parameters["@discount"].Value = (int)discount;

                    free_closed_table.Parameters.Add(new SqlParameter("@total_sum", SqlDbType.Int));
                    free_closed_table.Parameters["@total_sum"].Value = (int)((int)(dr2 * price) - (int)discount);

                    free_closed_table.ExecuteNonQuery();
                    UpdateData();
                    //if (dataGridView1.Rows[clicked_column_index].Cells[4].Value.ToString() != "0")
                    //{
                    //    SqlCommand updateHistory = new SqlCommand("INSERT INTO customer_game_history VALUES(@customer_id, @date_time, @discount_price)",con);
                    //    updateHistory.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.VarChar));
                    //    updateHistory.Parameters["@customer_id"].Value = dataGridView1.Rows[clicked_column_index].Cells[4].Value.ToString();

                    //    updateHistory.Parameters.Add(new SqlParameter("@date_time", SqlDbType.DateTime));
                    //    updateHistory.Parameters["@date_time"].Value = dataGridView1.Rows[clicked_column_index].Cells[2].Value.ToString();

                    //}
                    clicked_column_index = -1;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //clicked_column_index = int.Parse(e.RowIndex.ToString());
           //MessageBox.Show(dataGridView1.CurrentRow.Cells[0].Value.ToString());
            clicked_column_index = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clients c = new Clients(con);
            c.ShowDialog();
            UpdateData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Password p = new Password();
            //p.ShowDialog();
            //if (p.getResult() == false)
            //{
            //    MessageBox.Show("Incorrect password!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else
            //{
                List<List<string>> data = new List<List<string>>();
                string lines = "";
                clientCounter = 0;
                for (int i = 0; i < dataGridView1.Rows.Count - 1; ++i)
                {
                    //MessageBox.Show(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    
                    TimeSpan timespan = Convert.ToDateTime(dataGridView1.Rows[i].Cells[2].Value.ToString()) - System.DateTime.Now;
                    //MessageBox.Show(timespan.TotalMilliseconds.ToString());

                    if (timespan.TotalMilliseconds < 0)
                    {
                        data.Add(new List<string>());
                        for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                        {
                            data[i].Add(dataGridView1.Rows[i].Cells[j].Value.ToString());
                        }
                    }
                    else
                        continue;
                }
                for (int i = 0; i < data.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                    {

                        lines += data[i][j] + "   |    ";
                    }
                    WriteReport(lines);
                    lines = "";
                }
                SqlCommand clear = new SqlCommand("DELETE FROM new_client WHERE (cast(time_come - GETDATE() AS FLOAT(56))<0) AND client_state='closed'", con);

                clear.ExecuteNonQuery();
                SqlCommand updateNumbers;
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    clientCounter++;
                    updateNumbers = new SqlCommand("UPDATE new_client SET client_num = @c_n WHERE client_num = @client_num2", con);//,"+
                                                    //"time_come = @time_c, time_out = @time_o, table_id = @table_id, customer_id = @cust_id, "+
                                                    //"client_state = @client_state;",con);

                    updateNumbers.Parameters.Add(new SqlParameter("@client_num2", SqlDbType.Int));
                    updateNumbers.Parameters["@client_num2"].Value = int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());

                    updateNumbers.Parameters.Add(new SqlParameter("@c_n", SqlDbType.Int));
                    updateNumbers.Parameters["@c_n"].Value = clientCounter;
                    //updateNumbers.Parameters.Add(new SqlParameter("@time_c", SqlDbType.DateTime));
                    //updateNumbers.Parameters["@time_c"].Value = dataGridView1.Rows[i].Cells[2].Value.ToString();

                    //updateNumbers.Parameters.Add(new SqlParameter("@time_o", SqlDbType.DateTime));
                    //updateNumbers.Parameters["@time_o"].Value = dataGridView1.Rows[i].Cells[3].Value.ToString();

                    //updateNumbers.Parameters.Add(new SqlParameter("@table_id", SqlDbType.Int));
                    //updateNumbers.Parameters["@table_id"].Value = int.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());

                    //updateNumbers.Parameters.Add(new SqlParameter("@cust_id", SqlDbType.VarChar));
                    //updateNumbers.Parameters["@cust_id"].Value = dataGridView1.Rows[i].Cells[4].Value.ToString();

                    //updateNumbers.Parameters.Add(new SqlParameter("@disc", SqlDbType.Int));
                    //updateNumbers.Parameters["@disc"].Value = (int)(dataGridView1.Rows[i].Cells[5].Value);

                    //updateNumbers.Parameters.Add(new SqlParameter("@total_c", SqlDbType.Int));
                    //updateNumbers.Parameters["@total_c"].Value = int.Parse(dataGridView1.Rows[i].Cells[6].Value.ToString());

                    //updateNumbers.Parameters.Add(new SqlParameter("@client_state", SqlDbType.VarChar));
                    //updateNumbers.Parameters["@client_state"].Value = dataGridView1.Rows[i].Cells[7].Value.ToString();

                    updateNumbers.ExecuteNonQuery();
                }
                
                UpdateData();
            }
        //}
        private void WriteReport(string lines)
        {
            string nameOfFile = "./REPORT.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(nameOfFile))
            {
                file.WriteLine(lines);
            }
        }

        private void newClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void closeClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void tablesInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        private void customersInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NotificationWithAlarm nwa = new NotificationWithAlarm();
           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            a = new Alarm(con);
            a.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }
    }
}
