using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace WindowsFormsApplication1
{
    class Options
    {
        public double getPrice(int table_num)
        {
            if (table_num > 0 && table_num < 8)
            {
                return (double)80 / (double)60;
            }
            else if (table_num == 8)
            {
                return (double)100 / (double)60;
            }
            else if (table_num == 9 || table_num == 11)
            {
                return (double)150 / (double)60;
            }
            else if (table_num == 12)
            {
                return (double)250 / (double)60;
            }
            else
            {
                string temp = "Error there is no table #" + table_num.ToString();
                MessageBox.Show(temp);
                return 0;
            }
        }
        
        
    }
}
