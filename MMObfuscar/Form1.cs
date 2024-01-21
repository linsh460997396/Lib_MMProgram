using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MetalMaxSystem;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
//using System.Windows.Controls;

namespace MMObfuscar
{
    public partial class Form1 : Form
    {
        private static bool _userOpEnable = true;
        /// <summary>
        /// 用户操作许可
        /// </summary>
        public static bool UserOpEnable { get => _userOpEnable; set => _userOpEnable = value; }

        private static bool _workStatus = false;
        /// <summary>
        /// 工作状态
        /// </summary>
        public static bool WorkStatus { get => _workStatus; set => _workStatus = value; }

        private static bool _workStop = false;
        /// <summary>
        /// 打断工作用的状态变量
        /// </summary>
        public static bool WorkStop { get => _workStop; set => _workStop = value; }

        private static Thread _workThread;
        /// <summary>
        /// 工作专用后台子线程，防止工作时UI主线程界面卡住无法点击等问题
        /// </summary>
        public static Thread WorkThread { get => _workThread; set => _workThread = value; }

        public Form1()
        {
            InitializeComponent();
            //label_Tips.ForeColor = Color.Red;
            label_Statistics.ForeColor = Color.Red;
        }

        private string GetCodeFromMainThread()
        {
            if (richTextBox_Code.InvokeRequired)
            {
                return richTextBox_Code.Invoke(GetCodeFromMainThread);
            }
            else
            {
                return richTextBox_Code.Text;
            }
        }

        private void SetCodeToMainThread(string code)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                richTextBox_Code.Text = code;
            });
        }

        private int GetSelectedIndexFromMainThread()
        {
            if (comboBox_SelectFunc.InvokeRequired)
            {
                return comboBox_SelectFunc.Invoke(GetSelectedIndexFromMainThread);
            }
            else
            {
                return comboBox_SelectFunc.SelectedIndex;
            }
        }

        private string GetTipsFromMainThread()
        {
            if (label_Tips.InvokeRequired)
            {
                return label_Tips.Invoke(GetTipsFromMainThread);
            }
            else
            {
                return label_Tips.Text;
            }
        }

        private void SetTipsToMainThread(string tips)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                label_Tips.Text = tips;
            });
        }

        private string GetStatisticsFromMainThread()
        {
            if (label_Statistics.InvokeRequired)
            {
                return label_Statistics.Invoke(GetStatisticsFromMainThread);
            }
            else
            {
                return label_Statistics.Text;
            }
        }

        private string GetExclusionRulesPathFromMainThread()
        {
            if (textBox_ExclusionRulesPath.InvokeRequired)
            {
                return textBox_ExclusionRulesPath.Invoke(GetExclusionRulesPathFromMainThread);
            }
            else
            {
                return textBox_ExclusionRulesPath.Text;
            }
        }

        private void SetExclusionRulesPathToMainThread(string path)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                textBox_ExclusionRulesPath.Text = path;
            });
        }

        private void SetStatisticsToMainThread(string text)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                label_Statistics.Text = text;
            });
        }

        private void SetBtnRunTextToMainThread(string text)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                button_Run.Text = text;
            });
        }

        private void SetPanelBackColorToMainThread(Panel p, Color c)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                p.BackColor = c;
            });
        }

        private void SetControlEnableToMainThread(Control c, bool torf)
        {
            // 调用 Invoke 方法将操作发送到主线程
            Invoke((MethodInvoker)delegate ()
            {
                c.Enabled = torf;
            });
        }

        private Type GetControlTypeFromMainThread(Control control)
        {
            Type type = null;
            if (control.InvokeRequired) // 判断当前线程是否为UI主线程
            {
                control.BeginInvoke((MethodInvoker)(() => GetControlTypeFromMainThread(control))); // 将操作放入UI主线程的消息队列中
            }
            else
            {
                type = control.GetType(); // 获取控件的类型信息
            }
            return type;
        }

        /// <summary>
        /// 按钮点击后处理主要内容，本函数交由后台线程运行
        /// </summary>
        private void ButtonRun()
        {
            for (int i = 0; i < 1; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                switch (GetSelectedIndexFromMainThread())
                {
                    case -1:
                        SetTipsToMainThread("功能未选择！");
                        break;
                    case 0:
                        //SetTipsToMainThread("对Galaxy代码进行混肴");
                        SelectedFunc_0(GetExclusionRulesPathFromMainThread());
                        break;
                    default:
                        SetTipsToMainThread("功能无效！");
                        break;
                }
                stopwatch.Stop();
                Debug.WriteLine(stopwatch.Elapsed.ToString());
                SetStatisticsToMainThread(" 时耗：" + stopwatch.Elapsed.ToString());
            }
            //放弃了线程注销做法，程序将始终运行至此，可以知道是用户中断还是正常运行结束
            WorkStatus = false;//重置工作状态
            if (WorkStop) { SetStatisticsToMainThread("用户取消！"); }
            WorkStop = false;//重置_workStop状态，如果是用户取消的，打印告知
            UserOpEnableChange(true);//重置用户操作状态
            SetBtnRunTextToMainThread("执行");
            //Debug.WriteLine("子线程已经完成！");
            //线程清除（如果不放心，Abort方法能在目标线程中抛出一个ThreadAbortException异常从而导致目标线程的终止）
            //WorkThread.Abort();
        }

        /// <summary>
        /// The Button of Run.
        /// 点击执行按钮后应创建一个后台线程进行复杂任务处理，防止对UI所在的主线程造成卡顿。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Run_Click(object sender, EventArgs e)
        {
            //正则匹配抓取全部函数名（包含重复）
            if (richTextBox_Code.Text != "")
            {
                for (int i = 0; i < 1; i++)
                {
                    //开始工作，大部分界面置灰（用户不可操作）
                    UserOpEnableChange(false);

                    if (button_Run.Text == "执行" && WorkStatus == false)
                    {
                        WorkStatus = true;
                        button_Run.Text = "取消";
                        //创建后台线程实例来运行复杂任务
                        WorkThread = new Thread(ButtonRun) { IsBackground = true };
                        WorkThread.Start();
                        // 等待子线程完成↓
                        //WorkThread.Join();
                    }
                    else if (button_Run.Text == "取消" && WorkStatus == true)
                    {
                        WorkStop = true;
                    }
                }
            }

        }

        /// <summary>
        /// 第1个功能索引的执行方法：对Galaxy代码进行混肴
        /// </summary>
        /// <param name="exclusionRulesPath"></param>
        private void SelectedFunc_0(string exclusionRulesPath)
        {
            //排除规则文件使用系统默认的情况：1）文件路径为空；2）文件路径虽不为空但后缀非.txt；3）文件路径非法。
            if (
                exclusionRulesPath == ""
                || !(Regex.IsMatch(exclusionRulesPath, @"^(.*)(\.txt)$"))
                || !MMCore.IsDFPath(exclusionRulesPath)
            )
            {
                //文本路径错误，重置为系统默认
                exclusionRulesPath = AppDomain.CurrentDomain.BaseDirectory + @"exclusion_rules.txt";
                SetExclusionRulesPathToMainThread(exclusionRulesPath);
                SetTipsToMainThread("排除规则文件路径错误，重置为系统默认！");
            }

            // 定义正则表达式模式，匹配函数名
            string pattern = @"(?<=^|[^a-zA-Z_])[a-zA-Z_][\w]*(?=\s*\([^\)]*\)(\s+|\n|$))";
            MatchCollection matches = Regex.Matches(GetCodeFromMainThread(), pattern);
            CodeObfuscator obfuscator = new CodeObfuscator();
            //读取排除规则文本，添加混肴规则时会防止它们参与混肴（格式：每行一个函数名）
            obfuscator.LoadExclusionRules(exclusionRulesPath);
            //遍历全部函数名
            foreach (Match match in matches)
            {
                //Debug.WriteLine("Function Name: " + match.Value);
                //构建正则表达式，让Lib、GAx3开头的函数名也避开混肴
                if (!Regex.IsMatch(match.Value, "^(Lib|GAx3).*")) 
                {
                    //添加函数名及混肴后名称到混肴规则（这过程会自动去重，也不会生成相同混肴名称）
                    obfuscator.AddReplacement(match.Value);
                }
                
            }
            string obfuscatedCode = obfuscator.ObfuscateCode(GetCodeFromMainThread());
            SetCodeToMainThread(obfuscatedCode);
        }

        private void UserOpEnableChange(bool torf)
        {
            UserOpEnable = torf;
            //遍历全控件并获取类型可跨线程不用回调，但控件其他属性读写操作需要回调
            foreach (Control a in Controls)
            {
                if (a is Panel)
                {
                    Panel p = a as Panel;  //取出Panel
                    if (!torf)
                    {
                        // 改变Panel的颜色，执行时是灰色
                        SetPanelBackColorToMainThread(p, Color.Gray);
                    }
                    else
                    {
                        //不执行时是白色
                        SetPanelBackColorToMainThread(p, Color.Transparent);
                    }
                    foreach (Control c in p.Controls) //遍历面板中的每一个控件
                    {
                        if (c.GetType().Name.Equals("TextBox"))
                        {
                            //禁用文本框
                            SetControlEnableToMainThread(c, torf);
                        }
                        if (c.GetType().Name.Equals("CheckBox"))
                        {
                            //禁用复选框
                            SetControlEnableToMainThread(c, torf);
                        }
                        if (c.GetType().Name.Equals("ComboBox"))
                        {
                            //禁用下拉框
                            SetControlEnableToMainThread(c, torf);
                        }
                        if (c.GetType().Name.Equals("Button"))
                        {
                            if (!c.Name.Equals("button_Run"))
                            {
                                //禁用除运行按钮外的其他按钮
                                SetControlEnableToMainThread(c, torf);
                            }
                        }

                    }
                }
            }
        }

        private void button_SelectExclusionRulesFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string path = ofd.FileName;
            textBox_ExclusionRulesPath.Text = path;
        }

        private void richTextBox_Code_TextChanged(object sender, EventArgs e)
        {
            //文本改变时不需要写任何动作
        }

        private void comboBox_SelectFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (GetSelectedIndexFromMainThread())
            {
                case -1:
                    SetTipsToMainThread("功能未选择！");
                    break;
                case 0:
                    //SetTipsToMainThread("对Galaxy代码进行混肴");
                    break;
                default:
                    SetTipsToMainThread("功能无效！");
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_SelectFunc.Items.Add("对Galaxy代码进行混肴");
            comboBox_SelectFunc.SelectedIndex = 0;
        }
    }
}
