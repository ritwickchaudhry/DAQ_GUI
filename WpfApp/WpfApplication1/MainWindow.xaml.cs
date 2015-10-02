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
        TextBlock block  ;
        
        public SerialPort _serialport;
        public Thread _serialThread;

        static bool _continue = true;
        int i = 0;
        public MainWindow()
        {
            block = textBlock;
            InitializeComponent();
            _serialport = new SerialPort();
            _serialport.PortName = "COM3";
           _serialThread = new Thread(ReadAndUpdate);
        }

        private void startbutton_Click(object sender, RoutedEventArgs e)
        {
            _continue = true;
            Thread.Sleep(1);
            _serialport.Open();
            _serialThread.Start();
        }
        private void stopbutton_click(object sender, RoutedEventArgs e)
        {
            _continue = false;
            _serialThread.Abort();
            _serialThread = new Thread(ReadAndUpdate);
            _serialport.Close();

        }

        public void ReadAndUpdate()
        {
            
            while (_continue)
            {
                try
                {
                    string message = _serialport.ReadLine();
                    Console.WriteLine(message);
                    
                    Dispatcher.Invoke(() => {
                        textBlock.Text = message;
                    });
                }
                catch (TimeoutException) { }
            }
            _serialport.Close();
        }

        
    }
   
    
}



   
