using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MetalMaxSystem;

namespace RecordKeyBoardAndMouse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RecordService recordService;
        private Label[] labels = new Label[10];

        public MainWindow()
        {
            InitializeComponent();
            //初始化
            recordService = new RecordService();
            //初始化Grid
            for (int i = 0; i < 10; i++)
            {
                int row = i != 0 ? i - 1 : 9;
                labels[i] = new Label();
                labels[i].Content = "x:0,y:0";
                labels[i].SetValue(Grid.ColumnProperty, 1);
                labels[i].SetValue(Grid.RowProperty, row);
                MyGrid.Children.Add(labels[i]);
            }

            //循环任务
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler((source, e1) =>
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    //记录成员变量
                    this.label.Content = "wParam:" + recordService.WParam + ",x:" + recordService.X + ",y:" + recordService.Y;
                    this.label2.Content = "keyStatus:" + recordService.KeyStatus + ",keyValue:" + recordService.KeyValue;
                    //查询是否按下按钮
                    int result = recordService.IsPressTarget();
                    if (result != -1)
                    {
                        int index = result != 0 ? 9 + result : 19;
                        //MyGrid.Children.Remove(labels[result]);
                        labels[result].Content = "x:" + recordService.X + ",y:" + recordService.Y;
                        //MyGrid.Children.Add(labels[result]);
                    }
                });
            });  //到达时间的时候执行事件；
            aTimer.Interval = 50;// 设置引发时间的时间间隔　此处设置为１秒（500毫秒）
            aTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            aTimer.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            recordService.StartMouseHook();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            recordService.StartKeyboardHook();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            recordService.StopMouseHook();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            recordService.StopKeyboardHook();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            recordService.StartKeyboardHook();
            recordService.StartMouseHook();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            recordService.StopMouseHook();
            recordService.StopKeyboardHook();
        }
    }
}
