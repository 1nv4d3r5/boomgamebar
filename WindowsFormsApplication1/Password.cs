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
    public partial class Password : Form
    {
        bool result;
        ErrorProvider errorProvider1;
        public Password()
        {
            InitializeComponent();

        }
        public bool getResult()
        {
            if (result == true)
                return true;
            else
                return false;
        }

        private void Password_Load(object sender, EventArgs e)
        {
            result = false;
            textBox1.UseSystemPasswordChar = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //textBox1.BackColor = Color.White;
            //label1.Text = "ENTER PASSWORD TO COMPLETE ACTION";
            //label1.ForeColor = Color.Black;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            enter();
        }

        private void enter()
        {
            if (textBox1.Text == "123456")
            {
                result = true;
                Close();
            }
            else
            {
                result = false;
                Close();
            }
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                enter();
            }
        }
    }
}
