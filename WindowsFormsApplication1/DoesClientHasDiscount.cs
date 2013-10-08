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
    public partial class DoesClientHasDiscount : Form
    {
        private SqlConnection connection;// = new SqlConnection(@"Server= (local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");
        
        private int clicked_row;
        private string discount_id;
        private int op_clients = 0;
        List<string> clients_info;
        public DoesClientHasDiscount(int client_number, string discount)
        {
            discount_id = discount;
            clicked_row = client_number;
            InitializeComponent();
            clients_info = new List<string>();
            comboBox1.Text = discount;
        }
        private void GetClients()
        {
            connection = new SqlConnection(@"Server= (local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");

            connection.Open();
            using (connection)
            {
                SqlCommand command = new SqlCommand("SELECT customer_id FROM customer", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    clients_info.Add(reader.GetString(0).ToString());
                }
            }
        }

        private void DoesClientHasDiscount_Load(object sender, EventArgs e)
        {
            
            GetClients();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("For request you must fill in field!!!");
            }

            else
            {
                connection = new SqlConnection(@"Server= (local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");
                connection.Open();
                SqlCommand command;
                command = new SqlCommand("UPDATE new_client SET customer_ID=@customer_id WHERE client_num=@client_num; ", connection);
                command.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.VarChar));
                command.Parameters["@customer_id"].Value = comboBox1.Text;

                command.Parameters.Add(new SqlParameter("@client_num", SqlDbType.Int));
                command.Parameters["@client_num"].Value = clicked_row;

                command.ExecuteNonQuery();
                
                comboBox1.Items.Clear();

                Close();
            }
        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (op_clients == 0)
            {
                op_clients = 1;
                for (int i = 0; i < clients_info.Count; i++)
                {
                    comboBox1.Items.Add(clients_info[i].ToString());
                }
            }
        }
    }
}
