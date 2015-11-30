using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApplication1
{
    public partial class graph_window : Form
    {
        int i = 0;
        int y = 0;
        string filename;
        public graph_window(string s)
        {
            InitializeComponent();            
            filename = s;
            plot_data();
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            
        }
        private void plot_data()
        {
            //string[] lines = System.IO.File.ReadAllLines(@"F:\iitb racing\GUI\DAQ_GUI\WpfApp\WpfApplication1\WriteLines16-Oct-15 12-00-00 AM.txt");
            string[] lines = System.IO.File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                try
                {
                    i++;
                    bool result = Int32.TryParse(line, out y);
                    if (y != 0)
                    {
                        chart1.Series["test1"].Points.AddXY(i, y);

                    }                 
                                                            
                }
                catch (Exception e)
                {

                }
                    
                          
            }
            chart1.Series["test1"].Color = Color.Red;

        }
    }
}
