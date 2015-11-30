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
    public partial class dynamic_graph : Form
    {
        public dynamic_graph()
        {
            InitializeComponent();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        public void plot_data(int i, int y)
        {
              
            chart1.Series["test1"].Points.AddXY(i, y);

                  
            chart1.Series["test1"].Color = Color.Red;

        }
    }
}

