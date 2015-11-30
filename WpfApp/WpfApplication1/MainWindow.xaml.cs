using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;


namespace WpfApplication1
{

    public partial class MainWindow : Window
    {
       //variables definations
        public SerialPort _serialport;
        public Thread _serialThread;
        SolidColorBrush green = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff27d116"));
        SolidColorBrush red = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF12E0F"));
        static bool _continue = true;
        int i = 0;
        public String filename="";
        dynamic_graph graph1 = new dynamic_graph();

        //main window
        public MainWindow()
        {
            InitializeComponent();
            Load_comboBox();
            Load_batteryindicator();
            _serialport = new SerialPort();
            _serialport.PortName = "COM1";
           _serialThread = new Thread(ReadAndUpdate);
            _serialport.BaudRate = 9600;
        }

        //button to start reading data

        private void startbutton_Click(object sender, RoutedEventArgs e)
        {
            DateTime current = DateTime.Now;
            String current_str = current.Date.ToString().Replace(" ", "");
            current_str = current.ToString().Replace(":", "-");
            filename = current_str + ".txt";
            _continue = true;
            //graph1.Show();
            
            try
            {
                _serialport.Open();
                _serialThread.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex);
            }
            
            
        }


        private void stopbutton_click(object sender, RoutedEventArgs e)
        {
            digital_signal.Fill = red;
            _continue = false;
            _serialThread.Abort();
            _serialThread = new Thread(ReadAndUpdate);
            _serialport.Close();
            _serialport.BaudRate = 9600;

        }


        public void ReadAndUpdate()
        {
            
            while (_continue)
            {
                Thread.Sleep(1);
                try
                {
                    string message = _serialport.ReadLine();
                    Dispatcher.Invoke(() => {
                        textBlock.Text = message;
                        Speed.Text = message+"KMPH";
                        //Console.WriteLine(message);
                        try
                        {
                             
                            int num = Int32.Parse(message);
                            Canvas.SetTop(pointer, -40-117.5253*Math.Sin(Math.PI*(45+num*270/255)*180));
                            Canvas.SetLeft(pointer, 40+117.5253*Math.Cos(Math.PI*(45+num*270/255)*180));
                            if (num > 190)
                            {
                                digital_signal.Fill = green;
                            }
                            change_battery_level(num * 100 / 255);
                            //graph1.plot_data(i, num);
                        }
                        catch(Exception e)
                        {

                        }                   
                        write_in_file(message);
                        i++;
                    });
                }
                catch (TimeoutException) { }
            }
            _serialport.Close();
        }
        public void setPortName(SerialPort _serialport, TextBox box)
        {
            _serialport.PortName = box.Text;
        }

        private void Com_Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBoxItem port = (ComboBoxItem)comboBox.SelectedItem;
            _serialport.PortName = port.Content.ToString();
            MessageBox.Show("Port changed");
        }
        private void Load_comboBox()
        {
            comboBox.Items.Clear();
            ComboBoxItem item1;
            comboBox.Text = "select COM port:";
            item1 = new ComboBoxItem();
            foreach (string s in SerialPort.GetPortNames())
            {
                item1.Content = s;
                comboBox.Items.Add(item1);
            }
        }
        private void Load_batteryindicator()
        {
            batteryindicator.Minimum = 0;
            batteryindicator.Maximum = 100;
            batteryindicator.Value = 50;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
        private void change_battery_level(int n)
        {
            batteryindicator.Value = n;
            battery_percentage.Text = n.ToString()+"%";
            if (n <= 20)
            {
                batteryindicator.Foreground = red;

            }
        }
        private void write_in_file(string s)
        {
            DateTime current = DateTime.Now;
            
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename, true))
            {
                file.WriteLine(s);
            }
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg= new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name 
            dlg.DefaultExt = ".txt"; // Default file extension 
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                MessageBox.Show(filename);
            }
            graph_window my_window = new graph_window(filename);
            my_window.Show();
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            Load_comboBox();
        }

        private void baudrate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem rate = (ComboBoxItem)baudrate.SelectedItem;
            _serialport.BaudRate = Int32.Parse(rate.Content.ToString());
            MessageBox.Show("Baudrate changed");
        }
    }
   
    
}



   
