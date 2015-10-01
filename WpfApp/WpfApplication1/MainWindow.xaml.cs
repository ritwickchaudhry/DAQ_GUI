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
        SerialPort _serialport;
        Thread serialThread;
        static bool _continue=true;
        int i = 0;
        public MainWindow()
        {
            InitializeComponent();
            _serialport = new SerialPort();
            _serialport.PortName = "COM3";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.inputText.Text);
            /*myTextBlock newtextblock = new myTextBlock(textBlock);
            Thread thread = new Thread(newtextblock.execute);
            thread.Start();*/

            //serialThread = new Thread(new ThreadStart(ReadAndUpdate(textBlock, _serialport));
            Thread.Sleep(1);
                //textBlock.Text = DateTime.Now.ToString("h:mm:ss tt");
                ReadAndUpdate(textBlock, _serialport);
           



        }

        private void inputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            

        }
        public static void ReadAndUpdate(TextBlock textBlock ,SerialPort _serialport)
        {
            _serialport.Open();
           while (_continue)
            {
                try
                {
                    string message = _serialport.ReadLine();
                    Console.WriteLine(message);
                    textBlock.Text = message;
                }
                catch (TimeoutException) { }
            }
            _serialport.Close();
        }
        



    }
}
