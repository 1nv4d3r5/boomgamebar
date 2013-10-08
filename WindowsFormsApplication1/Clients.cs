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
    public partial class Clients : Form
    {
        SqlConnection con;
        int clicked_row = -1;
        public List<List<string>> clients_array;
        public Clients(SqlConnection conection)
        {
            con = conection;
            InitializeComponent();
           
        }
        public void updateData()
        {
            //con = new SqlConnection(@"Server=(local)\SQLEXPRESS; Initial Catalog=Boom; Integrated Security=True;");
            //con.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM customer ORDER BY customer_id ASC;", con);
            SqlCommandBuilder sb = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds, "customer");
            dataGridView1.DataSource = ds.Tables[0];

            clients_array = new List<List<string>>();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; ++i)
            {
                clients_array.Add(new List<string>());
                for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                {
                    clients_array[i].Add(dataGridView1.Rows[i].Cells[j].Value.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddNewClient anc = new AddNewClient(con);
            anc.ShowDialog();
            updateData();
        }

        private void Clients_Load(object sender, EventArgs e)
        {
            updateData();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Password p = new Password();
            p.ShowDialog();
            if (p.getResult() == true)
            {
                MessageBox.Show("Correct");

                string dr = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                if (dr == "G 001" || dr  == "0")
                {
                    MessageBox.Show("Unable to delete user! You are trying to delete 'usual user' or 'G 001'","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    SqlCommand deleteUser = new SqlCommand("DELETE FROM customer WHERE customer_id=@customer_id;", con);
                    deleteUser.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.VarChar));
                    deleteUser.Parameters["@customer_id"].Value = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    deleteUser.ExecuteNonQuery();
                    updateData();
                }
            }
            else
                MessageBox.Show("Wrong password");
            //MessageBox.Show(dataGridView1.Rows[clicked_row].Cells[0].Value.ToString());
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            clicked_row = int.Parse(dataGridView1.CurrentRow.Index.ToString());
            //MessageBox.Show(clicked_row.ToString());
        }
    }
}
