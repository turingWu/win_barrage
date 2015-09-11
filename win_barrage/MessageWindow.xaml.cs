using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Configuration;

namespace win_barrage
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(string message)
        {
            InitializeComponent();
            double fontSize = Double.Parse(ConfigurationManager.AppSettings["font_size"]);
            if (message.Trim().Length > 0)
            {
                label.Content = message;
            }
            Random rand = new Random();
            byte[] colorBytes = new byte[3];
            rand.NextBytes(colorBytes);
            label.Foreground = new SolidColorBrush(Color.FromRgb(colorBytes[0], colorBytes[1], colorBytes[2]));
            label.FontSize = fontSize * (1.5 * rand.NextDouble() + 1);
        }
        private void messageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(SystemParameters.PrimaryScreenWidth, 0 - this.ActualWidth, new TimeSpan(0, 0, 10));
            animation.Completed += new EventHandler(animation_Completed);
            this.BeginAnimation(Window.LeftProperty, animation);
        }
        void animation_Completed(object sender, EventArgs e)
        {
            Console.WriteLine(sender);
            this.Close();
        }
    }
}
