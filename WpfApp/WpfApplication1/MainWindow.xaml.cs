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
        SolidColorBrush black = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffffff"));
        static bool _continue = true;
        int i = 0;
        public String filename = "";
        dynamic_graph graph1 = new dynamic_graph();
        //senso  values declaration
        bool mc_error = false;
        bool bms_error = false;
        bool bspd_error = false;
        bool imd_error = false;
        int speed = 0;
        int _break = 0;
        int throttle = 0;
        int battery = 0;
        //main window
        public MainWindow()
        {
            InitializeComponent();
            Load_comboBox();
            Load_battery_break_throttle_indicator();
            //settting serial port
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
                _serialThread.Start(); //starts thread to take serial input and update gui
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //Console.WriteLine(ex);
            }


        }


        private void stopbutton_click(object sender, RoutedEventArgs e)
        {
            _continue = false;
            _serialThread.Abort();  //aborts previous thread
            _serialThread = new Thread(ReadAndUpdate); //reinitialising thread
            _serialport.Close();

        }

        //function to decode string
        


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBoxItem port = (ComboBoxItem)comboBox.SelectedItem;
            _serialport.PortName = port.Content.ToString();
            MessageBox.Show("Port changed");
        }

        private void baudrate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem rate = (ComboBoxItem)baudrate.SelectedItem;
            _serialport.BaudRate = Int32.Parse(rate.Content.ToString());
            MessageBox.Show("Baudrate changed");
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
        private void Load_battery_break_throttle_indicator()
        {
            //for battery
            batteryindicator.Minimum = 0;
            batteryindicator.Maximum = 100;
            batteryindicator.Value =0;
            batteryindicator.Foreground = green;
            //for break
            Break.Minimum = 0;
            Break.Maximum = 100;
            Break.Value = 0;
            Break.Foreground = green;
            //for throttle
            Throttle.Minimum = 0;
            Throttle.Maximum = 100;
            Throttle.Value = 0;
            Throttle.Foreground = green;
        }

        private void change_progressbar_level(int n)
        {
            batteryindicator.Value = n;
            Throttle.Value = n;
            Break.Value = n;
            battery_percentage.Text = n.ToString()+"%";
            if (n <= 20)
            {
                batteryindicator.Foreground = red;

            }
            if (n >80)
            {
                Break.Foreground = red;
                Throttle.Foreground = red;

            }
            else
            {
                Break.Foreground = green;
                Throttle.Foreground = green;
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
        


        //function 
        private void Decode(string s)
        {
            try
            {


                if (s[0] == '#' && s[8] == '#')
                {
                    speed = (int)s[1];
                    throttle = (int)s[2];
                    _break = (int)s[3];
                    mc_error = Compare(s[4]);
                    bms_error = Compare(s[5]);
                    bspd_error = Compare(s[6]);
                    imd_error = Compare(s[7]);
                }


            }
            catch { }
        }
        //function to read serial data and update
        public void ReadAndUpdate()
        {

            while (_continue)
            {
                Thread.Sleep(1);
                try
                {
                    string message = _serialport.ReadLine();
                    //dispature used when a variable is updated by two threads
                    Dispatcher.Invoke(() => {
                        textBlock.Text = message;
                        //decode function
                        Decode(message);
                        //update speed txt
                        string speed_txt = speed.ToString() + "(KMPH)";
                        Speed.Text = speed_txt;
                        //update speedometer
                        update_pointer(speed);
                        //update error state
                        update_errors(mc_error, bms_error, bspd_error, imd_error);
                        //update battery break throttle
                        update_battery_break_throttle(_break, throttle, battery);

                        try
                        {

                            int num = Int32.Parse(message);
                            //Decode("#û#");
                            //set_pointer(speed);
                            if (num > 190)
                            {
                                //digital_signal.Fill = green;
                            }
                            //change_progressbar_level(num * 100 / 255);
                            //graph1.plot_data(i, num);
                        }
                        catch (Exception e)
                        {

                        }
                        message = speed.ToString() + " " + _break.ToString() + " " + throttle.ToString();
                        write_in_file(message);
                        i++;
                    });
                }
                catch (TimeoutException) { }
            }
            _serialport.Close();
        }

        //function to update errors
        private void update_errors(bool mc_error, bool bms_error, bool bspd_error, bool imd_error)
        {
            if (mc_error)
            {
                mc_error_led.Fill = red;
            }
            else mc_error_led.Fill = green;
            if (bms_error)
            {
                bms_error_led.Fill = red;
            }
            else bms_error_led.Fill = green;
            if (bspd_error)
            {
                bspd_error_led.Fill = red;
            }
            else bspd_error_led.Fill = green;
            if (imd_error)
            {
                imd_error_led.Fill = red;
            }
            else imd_error_led.Fill = green;
        }
        private void update_battery_break_throttle(int _break, int throttle, int battery)
        {
            batteryindicator.Value = battery;
            Throttle.Value = throttle;
            Break.Value = _break;
            battery_percentage.Text = battery.ToString() + "%";
            if (battery <= 20)
            {
                batteryindicator.Foreground = red;

            }
            else batteryindicator.Foreground = green;
            if (_break > 80)
            {
                Break.Foreground = red;
            }
            else Break.Foreground = green;
            if (throttle > 80)
            {
                Throttle.Foreground = red;
            }
            else Throttle.Foreground = green;

        }

        private void update_pointer(int speed)
        {
            double ref_x = Canvas.GetLeft(Ellipse_1) + Ellipse_1.Width / 2;
            double ref_y = Canvas.GetTop(Ellipse_1) + Ellipse_1.Height / 2;
            double R = Ellipse_1.Height / 4 + Ellipse_1.Width / 4;
            double angle;
            double new_x = 0;
            double new_y = 0;
            //speed = 0;
            angle = Math.PI * (30 + speed * 270 / 255) / 180;
            new_x = ref_x - R * Math.Sin(angle) - pointer.Width / 2;
            new_y = ref_y + R * Math.Cos(angle) - pointer.Height / 2;
            Canvas.SetTop(pointer, new_y);
            Canvas.SetLeft(pointer, new_x);
        }



        private bool Compare(char c)
        {
            if (c == '0') return false;
            return true;
        } 






    }
   
    
}



   
