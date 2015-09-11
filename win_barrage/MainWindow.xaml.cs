using System.Windows;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Configuration;

namespace win_barrage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private double SLEEPTIME = 0.8;
        private double FETCH_FREQUENCE = 10;
        private double MIN_HEIGHT = 0.3;
        private double MAX_HEIGHT = 0.8;
        private int currentActivityId = 0;
        private Thread barrageThread = null;
        public MainWindow()
        {
            InitializeComponent();
            string isDebug = ConfigurationManager.AppSettings["debug"];
            if (isDebug.Equals("true"))
            {
                webBrowser.Source = new Uri("http://ty.weixiao.qq.com/");
            }
            else
            {
                webBrowser.Source = new Uri("http://weixiao.qq.com/");
            }
            this.SLEEPTIME = Double.Parse(ConfigurationManager.AppSettings["sleep_time"]);
            this.FETCH_FREQUENCE = Double.Parse(ConfigurationManager.AppSettings["fetch_frequency"]);
            this.MIN_HEIGHT = Double.Parse(ConfigurationManager.AppSettings["min_height"]);
            this.MAX_HEIGHT = Double.Parse(ConfigurationManager.AppSettings["max_height"]);
        }

        private void initSomeBarrages()
        {
            BarrageTools barrageTools = new BarrageTools(currentActivityId);
            List<Barrage> barrages = barrageTools.initBarrage();
            Console.WriteLine("init barrage");
            foreach (Barrage barrage in barrages)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Random rand = new Random();
                    MessageWindow messageWindow = new MessageWindow(barrage.Nickname + "：" + barrage.Content);
                    messageWindow.Top = SystemParameters.PrimaryScreenHeight * this.MIN_HEIGHT + SystemParameters.PrimaryScreenHeight * (this.MAX_HEIGHT - this.MIN_HEIGHT) * rand.NextDouble() - messageWindow.ActualHeight;
                    messageWindow.Left = SystemParameters.PrimaryScreenWidth;
                    messageWindow.Show();
                }));
                Thread.Sleep(TimeSpan.FromSeconds(SLEEPTIME));
            }
            while (true)
            {
                Console.WriteLine("fetch barrage");
                try
                {
                    barrages = barrageTools.fetchNewBarrage();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Catch exception in fetchNewBarrage():" + exception.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(FETCH_FREQUENCE));
                    continue;
                }
                foreach (Barrage barrage in barrages)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Random rand = new Random();
                        MessageWindow messageWindow = new MessageWindow(barrage.Nickname + "：" + barrage.Content);
                        messageWindow.Top = SystemParameters.PrimaryScreenHeight * this.MIN_HEIGHT + SystemParameters.PrimaryScreenHeight * (this.MAX_HEIGHT - this.MIN_HEIGHT) * rand.NextDouble() - messageWindow.ActualHeight;
                        messageWindow.Left = SystemParameters.PrimaryScreenWidth;
                        messageWindow.Show();
                    }));
                    Thread.Sleep(TimeSpan.FromSeconds(SLEEPTIME));
                }
                Thread.Sleep(TimeSpan.FromSeconds(FETCH_FREQUENCE));
            }
            // 使用System.Threading.Timer定期拉取新弹幕
            // Timer构造函数参数说明：
            // Callback：一个 TimerCallback 委托，表示要执行的方法。
            // State：一个包含回调方法要使用的信息的对象，或者为空引用（Visual Basic 中为 Nothing）。
            // dueTime：调用 callback 之前延迟的时间量（以毫秒为单位）。指定 Timeout.Infinite 以防止计时器开始计时。指定零 (0) 以立即启动计时器。
            // Period：调用 callback 的时间间隔（以毫秒为单位）。指定 Timeout.Infinite 可以禁用定期终止。
            // System.Threading.Timer threadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(fetchNewBarrage), null, 0, 100);
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentActivityId == 0)
            {
                return;
            }
            barrageThread = new Thread(initSomeBarrages);
            barrageThread.IsBackground = true;
            barrageThread.Start();
            stopBtn.Visibility = System.Windows.Visibility.Visible;
            this.WindowState = System.Windows.WindowState.Minimized;
            
        }

        private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string pattern = @".*weixiao.qq.com/home/\d*/activity/wall/(\d*)";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(e.Uri.ToString()))
            {
                currentActivityId = Int32.Parse(regex.Match(e.Uri.ToString()).Groups[1].Value);
                startBtn.Content = "OK！点击开始为当前活动播放弹幕~~~";
                startBtn.Foreground = Brushes.Green;
                startBtn.IsEnabled = true;
            }
            else
            {
                startBtn.Content = "登陆后请进入需要上墙的活动页面";
                startBtn.Foreground = Brushes.Red;
                startBtn.IsEnabled = false;
            }
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (barrageThread != null)
            {
                barrageThread.Abort();
                barrageThread = null;
                stopBtn.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}
