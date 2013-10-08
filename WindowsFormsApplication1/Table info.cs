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
    public partial class Table_info : Form
    {
        SqlConnection con;
        public List<List<string>> data;
        public Table_info(SqlConnection connection)
        {
            con = connection;
            InitializeComponent();
        }
        public void updateData()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM tables_info ORDER BY table_id, table_state;", con);
            SqlCommandBuilder sb = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds, "new_client");

            dataGridView1.DataSource = ds.Tables[0];
            
            //dataGridView1.RowsDefaultCellStyle.BackColor = Color.GreenYellow;
            
            data = new List<List<string>>();
            
            for (int i = 0; i < dataGridView1.Rows.Count - 1; ++i)
            {
                data.Add(new List<string>());
                for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                {
                    data[i].Add(dataGridView1.Rows[i].Cells[j].Value.ToString());
                }
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //MessageBox.Show("COLOR");
                if (row.Cells[0].Value == null)
                    continue;
                if (row.Cells[1].Value.ToString() == "free")
                {
                    row.DefaultCellStyle.BackColor = Color.GreenYellow;
                }
                else if (row.Cells[1].Value.ToString() == "order")
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                }
                else if (row.Cells[1].Value.ToString() == "busy")
                {
                    row.DefaultCellStyle.BackColor = Color.OrangeRed;

                }
            }
           
        }

        private void Table_info_Load(object sender, EventArgs e)
        {
            updateData();
        }
    }
}
