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
    public partial class AddNewClient : Form
    {
        private SqlConnection connection;
        public AddNewClient(SqlConnection con)
        {
            InitializeComponent();
            //connection = con;
            comboBox1.Text = "S";
            textBox1.Text = "";
            textBox2.Text = "10";
            textBox3.Text = "";
            textBox4.Text = "";
        }
        private void UpdateData()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Please fill up all fields!");
            }
            else
            {
                connection = new SqlConnection(@"Server= (local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");
                connection.Open();
                SqlCommand command;
                command = new SqlCommand("INSERT INTO customer(customer_id, discount, customer_name, customer_phone) VALUES (@customer_id, @discount, @customer_name, @customer_phone); ", connection);
                command.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.VarChar));
                command.Parameters["@customer_id"].Value = comboBox1.Text + " " + textBox1.Text;

                command.Parameters.Add(new SqlParameter("@discount", SqlDbType.Float));
                command.Parameters["@discount"].Value = double.Parse(textBox2.Text);

                command.Parameters.Add(new SqlParameter("@customer_name", SqlDbType.VarChar));
                command.Parameters["@customer_name"].Value = textBox3.Text;

                command.Parameters.Add(new SqlParameter("@customer_phone", SqlDbType.VarChar));
                command.Parameters["@customer_phone"].Value = textBox4.Text;

                command.ExecuteNonQuery();
                Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "S")
            {
                textBox2.Text = "10";
            }
            else if (comboBox1.Text == "G")
            {
                textBox2.Text = "20";
            }
            else 
            {
                textBox2.Text = "0";
            }
        }
    }
}
