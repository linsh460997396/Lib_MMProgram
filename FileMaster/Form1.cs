using MetalMaxSystem;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace FileMaster
{
    public partial class Form1 : Form
    {
        #region 字段及其属性方法
        private static bool simRunState = false;
        private static bool _workState = false;
        private static bool _workStop = false;
        private static int _workErrCount = 0;
        private static int _startIndex;
        private static int _strCount;
        private static int _dirCount = 0;
        private static int _proCount;
        private static int _proCountMax;
        private static Thread _workThread;
        private static bool _userOpEnable = true;

        /// <summary>
        /// 工作状态
        /// </summary>
        public static bool WorkStatus { get => _workState; set => _workState = value; }
        /// <summary>
        /// 打断工作用的状态变量
        /// </summary>
        public static bool WorkStop { get => _workStop; set => _workStop = value; }
        /// <summary>
        /// 工作中无法处理的文件数量
        /// </summary>
        public static int WorkErrCount { get => _workErrCount; set => _workErrCount = value; }
        /// <summary>
        /// 文件名字符起始位置
        /// </summary>
        public static int StartIndex { get => _startIndex; set => _startIndex = value; }
        /// <summary>
        /// 从文件名起始位置开始计算的字符数量
        /// </summary>
        public static int StrCount { get => _strCount; set => _strCount = value; }
        /// <summary>
        /// 决定首次目录统计时是否换行的状态变量
        /// </summary>
        public static int DirCount { get => _dirCount; set => _dirCount = value; }
        /// <summary>
        /// 工作进度计数
        /// </summary>
        public static int ProCount { get => _proCount; set => _proCount = value; }
        /// <summary>
        /// 工作进度最大数
        /// </summary>
        public static int ProCountMax { get => _proCountMax; set => _proCountMax = value; }
        /// <summary>
        /// 工作专用后台子线程
        /// </summary>
        public static Thread WorkThread { get => _workThread; set => _workThread = value; }
        /// <summary>
        /// 用户操作许可
        /// </summary>
        public static bool UserOpEnable { get => _userOpEnable; set => _userOpEnable = value; }

        #endregion

        #region Functions

        /// <summary>
        /// 递归方式强制修改目录（进最里层修改文件后修改这些文件的父级目录，层层递出时重复动作），修改前会去掉文件（夹）的Archive、ReadOnly、Hidden属性
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="workFilePath">工作文本全名路径</param>
        /// <param name="torf">最外层目录是否参与修改，默认true</param>
        /// <param name="isOuter">是否最外层目录，默认true</param>
        public void DFRecursively(DirectoryInfo dirInfo, string workFilePath, bool torf = true, bool isOuter = true)
        {
            bool isOuterDir = isOuter;
            foreach (DirectoryInfo newInfo in dirInfo.GetDirectories())
            {
                DFRecursively(newInfo, workFilePath, true, false);//递归遍历子目录，对子目录来说最外层即它自己，是需要修改的（不填则默认torf = true）
            }
            foreach (FileInfo newInfo in dirInfo.GetFiles())
            {
                //处理每个目录内部的文件（从里层文件开始修改）
                //newInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                DAction(workFilePath, newInfo.FullName);
            }
            if (!isOuterDir || (isOuterDir && torf))
            {//如果不是最外层目录
                //对每个目录处理（从里层目录开始修改）
                //dirInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                DAction(workFilePath, dirInfo.FullName);
            }
        }

        /// <summary>
        /// 递归方式强制修改目录（进最里层修改文件后修改这些文件的父级目录，层层递出时重复动作），修改前会去掉文件（夹）的Archive、ReadOnly、Hidden属性
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="workFilePath">工作文本全名路径</param>
        /// <param name="torf">最外层目录是否参与修改，默认true</param>
        /// <param name="isOuter">是否最外层目录，默认true</param>
        public void DFRecursively(string dirPath, string workFilePath, bool torf = true, bool isOuter = true)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            DFRecursively(dirInfo, workFilePath, torf, isOuter);
        }

        /// <summary>
        /// 参数1和2是数字
        /// </summary>
        /// <returns></returns>
        private bool Param1And2IsNumeric()
        {
            bool torf = true;
            if (!MMCore.IsNumeric(textBox_param2.Text))
            {
                textBox_param2.Text = "请重填！";
                torf = false;
            }
            else
            {
                StrCount = Convert.ToInt32(textBox_param2.Text);
                if (StrCount < 0)
                {
                    textBox_param2.Text = "请重填！";
                    torf = false;
                }
            }
            if (!MMCore.IsNumeric(textBox_param1.Text))
            {
                textBox_param1.Text = "请重填！";
                torf = false;
            }
            else
            {
                StartIndex = Convert.ToInt32(textBox_param1.Text);
                if (StartIndex < 0)
                {
                    textBox_param1.Text = "请重填！";
                    torf = false;
                }
            }
            return torf;
        }

        /// <summary>
        /// 验证检索文件大小是否在用户定义范围（递归耗时）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool ParamIsInRange(string path)
        {
            bool torf = false;
            long x;
            long a;
            long b;
            for (int i = 0; i < 1; i++)
            {
                if (MMCore.IsNumeric(textBox_rangeMin.Text) && MMCore.IsNumeric(textBox_rangeMax.Text))
                {
                    a = long.Parse(textBox_rangeMin.Text);
                    b = long.Parse(textBox_rangeMax.Text);
                    if (MMCore.IsDir(path))
                    {
                        x = MMCore.GetDirectoryLength(path);
                    }
                    else
                    {
                        x = MMCore.GetFileLength(path);
                    }
                    if (x >= a && x < b)
                    {
                        torf = true;
                    }
                }
            }
            return torf;
        }

        /// <summary>
        /// 首次写入文件时标记卷名卷号
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="diskSymbol">工作目录盘符</param>
        private void FirstWrite(string workFilePath, string diskSymbol)
        {
            DriveInfo drive = new DriveInfo(diskSymbol);
            if (drive.Name == $"{diskSymbol}" + @":\")
            {
                if (checkBox_printDiskName.Checked)
                {
                    MMCore.WriteLine(
                        workFilePath,
                        " Volume in drive " + diskSymbol + " is " + drive.VolumeLabel,
                        true
                    );
                }

                if (checkBox_printDiskSymbol.Checked)
                {
                    MMCore.WriteLine(
                        workFilePath,
                        " Volume Serial Number is " + MMCore.GetHardDiskID(diskSymbol),
                        true
                    );
                }
                MMCore.WriteLine(workFilePath, "", true);
            }

        }

        private void UserOpEnableChange(bool torf)
        {
            UserOpEnable = torf;
            foreach (Control a in Controls)
            {
                if (a is Panel)
                {
                    Panel p = a as Panel;  //取出Panel
                    if (!torf)
                    {
                        //改变Panel的颜色，执行时是灰色
                        p.BackColor = Color.Gray;
                    }
                    else
                    {
                        //不执行时是白色
                        p.BackColor = Color.Transparent;
                    }
                    foreach (Control c in p.Controls) //遍历面板中的每一个控件
                    {
                        if (c.GetType().Name.Equals("TextBox"))
                        {
                            c.Enabled = torf;//禁用文本框
                        }
                        if (c.GetType().Name.Equals("CheckBox"))
                        {
                            c.Enabled = torf;//禁用复选框
                        }
                        if (c.GetType().Name.Equals("ComboBox"))
                        {
                            c.Enabled = torf;//禁用下拉框
                        }
                        if (c.GetType().Name.Equals("Button"))
                        {
                            if (!c.Name.Equals("button_run"))
                            {
                                c.Enabled = torf;//禁用除运行按钮外的其他按钮
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 点击执行按钮
        /// </summary>
        private void ButtonRun()
        {
            string workPath;
            string workFilePath;
            string diskSymbol;

            for (int i = 0; i < 1; i++)
            {
                workPath = textBox_workPath.Text;
                workFilePath = textBox_workFilePath.Text;

                switch (comboBox_selectFunc.SelectedIndex)
                {
                    case 0:
                        label_headTip.Text = "批量将文件（夹）名称打印到工作文件";
                        break;
                    case 1:
                        label_headTip.Text = "批量将文件（夹）名称去除固定位数字符";
                        break;
                    case 2:
                        label_headTip.Text = "批量将文件（夹）名称保留固定位数字符";
                        break;
                    case 3:
                        label_headTip.Text = "批量插入字符到文件（夹）名称固定位数字符前";
                        break;
                    case 4:
                        label_headTip.Text = "批量插入字符到文件（夹）名称固定位数字符后";
                        break;
                    case 5:
                        label_headTip.Text = "批量插入字符到文件（夹）名称最前";
                        break;
                    case 6:
                        label_headTip.Text = "批量插入字符到文件（夹）名称最后";
                        break;
                    case 7:
                        label_headTip.Text = "批量替换文件（夹）名称固定位数字符";
                        break;
                    case 8:
                        label_headTip.Text = "批量替换文件（夹）名称的指定字符";
                        break;
                    case 9:
                        label_headTip.Text = "批量替换文件（夹）名称的后缀字符";
                        break;
                    case 10:
                        label_headTip.Text = "批量删除（移动/复制）指定名称的文件（夹）";
                        break;
                    default:
                        label_headTip.Text = "功能未选择！";
                        break;
                }
                if ((label_headTip.Text == "功能未选择！") || (comboBox_selectFunc.SelectedIndex == -1))
                {
                    break;
                }
                if (!Directory.Exists(workPath))
                {
                    label_headTip.Text = "工作目录无效！";
                    break; //调用本函数的动作有验证工作目录路径字符，通过后执行到此处再次验证有无此目录，无则打断
                }
                else
                {
                    diskSymbol = workPath.Substring(0, 1);
                }
                //生成指定文本到程序目录的情况：1）工作文本为空；2）工作文本路径已存在但未允许指定工作文本；3）勾选仅输出.txt但工作文本格式错误，虽不为空但后缀非.txt；4）文件路径非法。
                if (
                    workFilePath == ""
                    || (File.Exists(workFilePath) && !checkBox_coverWorkFile.Checked)
                    || (Regex.IsMatch(workFilePath, @"^(.*)(\.txt)$") && !checkBox_printTXTOnly.Checked)
                    || !MMCore.IsDFPath(workFilePath)
                )
                {
                    //工作文本路径错误，重置为系统默认
                    workFilePath = AppDomain.CurrentDomain.BaseDirectory + @"temp.txt";
                    textBox_workFilePath.Text = workFilePath;
                    label_headTip.Text = "工作文本路径错误，按系统默认输出！";
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                DFRun(workPath, workFilePath, diskSymbol);
                stopwatch.Stop();
                //MMCore.WriteLine(workFilePath, stopwatch.Elapsed.ToString(), true);
                if (checkBox40.Checked) { label_dirStatistics.Text += " 时耗 => " + stopwatch.Elapsed.ToString(); }
            }
            //放弃了线程注销做法，程序将始终运行至此，可以知道是用户中断还是正常运行结束
            DirCount = 0;//影响打印的目录是否换行
            WorkStatus = false;//重置工作状态
            if (WorkStop) { label_dirStatistics.Text = "用户取消！"; }
            WorkStop = false;//重置_workStop状态，如果是用户取消的，打印告知
            WorkErrCount = 0;//重置错误计数
            UserOpEnableChange(true);//重置用户操作状态
            button_run.Text = "执行";
            //WorkThread.Abort();
        }

        /// <summary>
        /// 按照功能编号去选择执行检索文件后的动作
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction(string workFilePath, string item)
        {
            switch (comboBox_selectFunc.SelectedIndex)
            {
                case 1:
                    DAction_f1(workFilePath, item);
                    break;
                case 2:
                    DAction_f2(workFilePath, item);
                    break;
                case 3:
                    DAction_f3456(workFilePath, item);
                    break;
                case 4:
                    DAction_f3456(workFilePath, item);
                    break;
                case 5:
                    DAction_f3456(workFilePath, item);
                    break;
                case 6:
                    DAction_f3456(workFilePath, item);
                    break;
                case 7:
                    DAction_f7(workFilePath, item);
                    break;
                case 8:
                    DAction_f8(workFilePath, item);
                    break;
                case 9:
                    DAction_f9(workFilePath, item);
                    break;
                case 10:
                    DAction_f10(workFilePath, item);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 功能编号01，将文件（夹）名称去除固定位数字符
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f1(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            string itemFrontName;
            string itemExtension;
            string newFileName;
            string newFilePath = "";
            string reportPath;
            bool isDir = false;
            int counter;
            for (int i = 0; i < 1; i++)
            {
                dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1);
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item);//返回的是上一级目录名称
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);
                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }
                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";
                if (Param1And2IsNumeric() && textBox_param2.Text != "" && textBox_param1.Text != "" && StrCount > 0)
                {
                    if (itemName.Length > (StrCount + StartIndex)) //文件名长度足以支撑修改，8>(0+7)或(1+6)，至少留一个字符
                    {
                        if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                        {
                            if (textBox_param4.Text == "" || (string.Compare(Path.GetExtension(item), textBox_param4.Text, checkBox_param4.Checked) == 0))
                            {
                                //勾选保护后缀时的处理
                                if (itemFrontName.Length >= (StrCount + StartIndex)) //前缀允许全部删除
                                {
                                    newFileName =
                                        itemFrontName.Remove(StartIndex, StrCount) + itemExtension;
                                    newFilePath = Path.Combine(dirName + @"\", newFileName);
                                }
                                else
                                {
                                    //预判出错文件并汇报，忽略这些文件的重命名，此时newFilePath=""
                                    if (WorkErrCount == 0)
                                    {
                                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                    }
                                    WorkErrCount += 1;
                                    MMCore.WriteLine(reportPath, "前缀字符数不足以修改:" + item, true);
                                }
                            }
                        }
                        else
                        {
                            //不保护后缀（或参数4默认填了*）
                            newFileName = itemName.Remove(StartIndex, StrCount);
                            newFilePath = Path.Combine(dirName + @"\", newFileName);
                        }
                        if (
                            newFilePath != ""
                            && !File.Exists(newFilePath)
                            && !Directory.Exists(newFilePath)
                        )
                        {
                            //对非空newFilePath且不会发生覆盖的文件，执行重命名
                            if (!isDir)
                            {
                                FileInfo fileInfo = new FileInfo(item);
                                fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在为路径添加随机值
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath), $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}");
                                            }
                                            if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //这个@"\"加不加都无所谓
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath) + @"\", $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}");
                                                counter++;
                                            }
                                            if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }
                            else if (checkBox_dirModification.Checked)
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                                directoryInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在，为路径添加一个随机值
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath), $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}");
                                            }
                                            if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //这个@"\"加不加都无所谓
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath) + @"\", $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}");
                                                counter++;
                                            }
                                            if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }

                            MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                        }
                        else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                        {
                            //newFilePath=""，说明出现预判出错文件且汇报过，现在仅对预判重叠文件进行汇报
                            if (WorkErrCount == 0)
                            {
                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                            }
                            WorkErrCount += 1;
                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                        }
                    }
                    else
                    {
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "文件名长度不足以扣减:" + item, true);
                    }

                    if (WorkErrCount > 0)
                    {
                        label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                    }
                    //测试↓
                    //MMCore.WriteLine( reportPath, "item = " + item, true);
                    //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                    //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                    //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                    //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                }
            }
        }

        /// <summary>
        /// 功能编号02，将文件（夹）名称保留固定位数字符
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f2(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            string itemFrontName;
            string itemExtension;
            string newFileName;
            string newFilePath = "";
            string reportPath;
            bool isDir = false;
            int counter;
            for (int i = 0; i < 1; i++)
            {
                dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1);
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item);
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);
                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }
                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";
                if (Param1And2IsNumeric() && StrCount > 0)
                {
                    if (itemName.Length > (StrCount + StartIndex)) //文件名（带后缀）长度足以支撑修改
                    {
                        if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                        {
                            if (
                                textBox_param4.Text == ""
                                || (
                                    string.Compare(
                                        Path.GetExtension(item),
                                        textBox_param4.Text,
                                        checkBox_param4.Checked
                                    ) == 0
                                )
                            )
                            {
                                //勾选保护后缀时的处理
                                if (itemFrontName.Length >= (StrCount + StartIndex)) //假设ABCD.txt, 8>=(0+8)或8>=(1+7)
                                {
                                    if (StartIndex == 0)
                                    {
                                        newFileName =
                                            itemFrontName.Substring(StartIndex, StrCount)
                                            + itemExtension;
                                    }
                                    else
                                    {
                                        newFileName =
                                            itemFrontName.Substring(StartIndex, StrCount)
                                            + itemExtension;
                                    }
                                    newFilePath = Path.Combine(dirName + @"\", newFileName);
                                }
                                else
                                {
                                    //预判出错文件并汇报，忽略这些文件的重命名，此时newFilePath=""
                                    if (WorkErrCount == 0)
                                    {
                                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                    }
                                    WorkErrCount += 1;
                                    MMCore.WriteLine(reportPath, "前缀字符数不足以修改:" + item, true);
                                }
                            }
                        }
                        else
                        {
                            //不保护后缀（或参数4默认填了*）
                            if (StartIndex == 0)
                            {
                                newFileName = itemName.Remove(StrCount);
                            }
                            else
                            {
                                newFileName = itemName.Remove(0, StartIndex);
                                newFileName = newFileName.Remove(StrCount - StartIndex);
                            }
                            newFilePath = Path.Combine(dirName + @"\", newFileName);
                        }
                        if (
                            newFilePath != ""
                            && !File.Exists(newFilePath)
                            && !Directory.Exists(newFilePath)
                        )
                        {
                            //对非空newFilePath且不会发生覆盖的文件，执行重命名
                            if (!isDir)
                            {
                                FileInfo fileInfo = new FileInfo(item);
                                fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在，为路径添加一个随机值
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath), $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}");
                                            }
                                            if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //这个@"\"加不加都无所谓
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath) + @"\", $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}");
                                                counter++;
                                            }
                                            if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }
                            else if(checkBox_dirModification.Checked)
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                                directoryInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在，为路径添加一个随机值
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath), $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}");
                                            }
                                            if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //这个@"\"加不加都无所谓
                                                newFilePath = Path.Combine(Path.GetDirectoryName(newFilePath) + @"\", $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}");
                                                counter++;
                                            }
                                            if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }

                            MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                        }
                        else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                        {
                            //newFilePath=""，说明出现预判出错文件且汇报过，现在仅对预判重叠文件进行汇报
                            if (WorkErrCount == 0)
                            {
                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                            }
                            WorkErrCount += 1;
                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                        }
                    }
                    else
                    {
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "文件名长度不足以扣减:" + item, true);
                    }

                    if (WorkErrCount > 0)
                    {
                        label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                    }
                    //测试↓
                    //MMCore.WriteLine( reportPath, "item = "+ item, true);
                    //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                    //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                    //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                    //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                }
            }
        }

        /// <summary>
        /// 功能编号03~06，插入字符到文件（夹）名称固定位数字符前后或名称最前最后
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f3456(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            string itemFrontName;
            string itemExtension;
            string newFileName = "";
            string newFilePath = "";
            string reportPath;
            string tempFrontName;
            string tempMidName;
            string tempEndName;
            bool torf = false;
            bool isDir = false;
            int counter;
            //固定功能5和6的参数
            switch (comboBox_selectFunc.SelectedIndex)
            {
                case 5:
                    StartIndex = 0;
                    StrCount = 1;
                    torf = true;
                    break;
                case 6:
                    StartIndex = 0;
                    StrCount = 1;
                    torf = true;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < 1; i++)
            {
                dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1);
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item); //返回的是上一级目录名称
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);
                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }
                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";
                if ((torf || Param1And2IsNumeric()) && StrCount > 0)
                {
                    if (itemName.Length >= (StrCount + StartIndex)) //文件名（带后缀）长度足以支撑修改
                    {
                        if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                        {
                            if (
                                textBox_param4.Text == ""
                                || (
                                    string.Compare(
                                        Path.GetExtension(item),
                                        textBox_param4.Text,
                                        checkBox_param4.Checked
                                    ) == 0
                                )
                            )
                            {
                                //勾选保护后缀时的处理
                                if (itemFrontName.Length >= (StrCount + StartIndex)) //假设ABCD.txt, 8>=(0+8)或8>=(1+7)
                                {
                                    if (StartIndex == 0)
                                    {
                                        tempFrontName = "";
                                        tempMidName = itemFrontName.Substring(0, StrCount);
                                        if (itemFrontName.Length == StrCount)
                                        {
                                            tempEndName = "";
                                        }
                                        else
                                        {
                                            tempEndName = itemFrontName.Substring(StrCount);
                                        }
                                    }
                                    else
                                    {
                                        tempFrontName = itemFrontName.Substring(0, StartIndex);
                                        tempMidName = itemFrontName.Substring(StartIndex, StrCount);
                                        if (itemFrontName.Length > (StrCount + StartIndex))
                                        {
                                            //字符数有余，如8>(1+6)的情况
                                            tempEndName = itemFrontName.Substring(StrCount + 1);
                                        }
                                        else
                                        {
                                            //字符数正好，如8=(1+7)的情况
                                            tempEndName = "";
                                        }
                                    }
                                    switch (comboBox_selectFunc.SelectedIndex)
                                    {
                                        case 3:
                                            newFileName =
                                                tempFrontName
                                                + textBox_param5.Text
                                                + tempMidName
                                                + tempEndName
                                                + itemExtension;
                                            break;
                                        case 4:
                                            newFileName =
                                                tempFrontName
                                                + tempMidName
                                                + textBox_param5.Text
                                                + tempEndName
                                                + itemExtension;
                                            break;
                                        case 5:
                                            newFileName =
                                                textBox_param5.Text
                                                + tempFrontName
                                                + tempMidName
                                                + tempEndName
                                                + itemExtension;
                                            break;
                                        case 6:
                                            newFileName =
                                                tempFrontName
                                                + tempMidName
                                                + tempEndName
                                                + textBox_param5.Text
                                                + itemExtension;
                                            break;
                                        default:
                                            break;
                                    }
                                    newFilePath = Path.Combine(dirName + @"\", newFileName);
                                }
                                else
                                {
                                    //预判出错文件并汇报，忽略这些文件的重命名，此时newFilePath=""
                                    if (WorkErrCount == 0)
                                    {
                                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                    }
                                    WorkErrCount += 1;
                                    MMCore.WriteLine(reportPath, "固定字符数超过前缀字符数:" + item, true);
                                }
                            }
                        }
                        else
                        {
                            //不保护后缀（或参数4默认填了*）
                            if (StartIndex == 0)
                            {
                                tempFrontName = "";
                                tempMidName = itemName.Substring(0, StrCount);
                                if (itemName.Length == StrCount)
                                {
                                    tempEndName = "";
                                }
                                else
                                {
                                    tempEndName = itemName.Substring(StrCount);
                                }
                            }
                            else
                            {
                                tempFrontName = itemName.Substring(0, StartIndex);
                                tempMidName = itemName.Substring(StartIndex, StrCount);
                                if (itemName.Length > (StrCount + StartIndex))
                                {
                                    //字符数有余，如8>(1+6)的情况
                                    tempEndName = itemName.Substring(StrCount + 1);
                                }
                                else
                                {
                                    //字符数正好，如8=(1+7)的情况
                                    tempEndName = "";
                                }
                            }
                            switch (comboBox_selectFunc.SelectedIndex)
                            {
                                case 3:
                                    newFileName =
                                        tempFrontName + textBox_param5.Text + tempMidName + tempEndName;
                                    break;
                                case 4:
                                    newFileName =
                                        tempFrontName + tempMidName + textBox_param5.Text + tempEndName;
                                    break;
                                case 5:
                                    newFileName =
                                        textBox_param5.Text + tempFrontName + tempMidName + tempEndName;
                                    break;
                                case 6:
                                    newFileName =
                                        tempFrontName + tempMidName + tempEndName + textBox_param5.Text;
                                    break;
                                default:
                                    break;
                            }
                            newFilePath = Path.Combine(dirName + @"\", newFileName);
                        }
                        if (
                            newFilePath != ""
                            && !File.Exists(newFilePath)
                            && !Directory.Exists(newFilePath)
                        )
                        {
                            //对非空newFilePath且不会发生覆盖的文件，执行重命名
                            if (!MMCore.IsDir(item))
                            {
                                FileInfo fileInfo = new FileInfo(item);
                                fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在为路径添加随机值
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath),
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                                );
                                            }
                                            if (simRunState == false)
                                                fileInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath) + @"\",
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                                );
                                                counter++;
                                            }
                                            if (simRunState == false)
                                                fileInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }
                            else if (checkBox_dirModification.Checked)
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                                directoryInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在为路径添加随机值
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath),
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                                );
                                            }
                                            if (simRunState == false)
                                                directoryInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath) + @"\",
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                                );
                                                counter++;
                                            }
                                            if (simRunState == false)
                                                directoryInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }

                            MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                        }
                        else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                        {
                            //newFilePath=""，说明出现预判出错文件且汇报过，现在仅对预判重叠文件进行汇报
                            if (WorkErrCount == 0)
                            {
                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                            }
                            WorkErrCount += 1;
                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                        }
                    }
                    else
                    {
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "固定字符数超过对象字符数:" + item, true);
                    }

                    if (WorkErrCount > 0)
                    {
                        label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                    }
                    //测试↓
                    //MMCore.WriteLine( reportPath, "item = " + item, true);
                    //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                    //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                    //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                    //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                    //MMCore.WriteLine( reportPath, "tempFrontName = " + tempFrontName, true);
                    //MMCore.WriteLine( reportPath, "tempMidName = " + tempMidName, true);
                    //MMCore.WriteLine( reportPath, "tempEndName = " + tempEndName, true);
                }
            }
        }

        /// <summary>
        /// 功能编号07，批量替换文件（夹）名称固定位数字符
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f7(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            string itemFrontName;
            string itemExtension;
            string newFileName;
            string newFilePath = "";
            string reportPath;
            string tempFrontName;
            //string tempMidName;
            string tempEndName;
            bool torf = false;
            bool isDir = false;
            int counter;
            for (int i = 0; i < 1; i++)
            {
                dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1);
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item); //返回的是上一级目录名称
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);
                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }
                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";
                if (Param1And2IsNumeric() && StrCount > 0)
                {
                    if (textBox_param5.Text == "" && (itemName.Length > (StrCount + StartIndex)))
                    {
                        torf = true;
                    }
                    else if (textBox_param5.Text != "" && (itemName.Length >= (StrCount + StartIndex)))
                    {
                        torf = true;
                    }
                    if (torf) //文件名（带后缀）长度足以支撑修改
                    {
                        if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                        {
                            if (
                                textBox_param4.Text == ""
                                || (
                                    string.Compare(
                                        Path.GetExtension(item),
                                        textBox_param4.Text,
                                        checkBox_param4.Checked
                                    ) == 0
                                )
                            )
                            {
                                //勾选保护后缀时的处理
                                if (itemFrontName.Length >= (StrCount + StartIndex)) //假设ABCD.txt, 8>=(0+8)或8>=(1+7)
                                {
                                    if (StartIndex == 0)
                                    {
                                        tempFrontName = "";
                                        //tempMidName = itemFrontName.Substring(0, _strCount);
                                        if (itemFrontName.Length == StrCount)
                                        {
                                            tempEndName = "";
                                        }
                                        else
                                        {
                                            tempEndName = itemFrontName.Substring(StrCount);
                                        }
                                    }
                                    else
                                    {
                                        tempFrontName = itemFrontName.Substring(0, StartIndex);
                                        //tempMidName = itemFrontName.Substring(_startIndex, _strCount);
                                        if (itemFrontName.Length > (StrCount + StartIndex))
                                        {
                                            //字符数有余，如8>(1+6)的情况
                                            tempEndName = itemFrontName.Substring(StrCount + 1);
                                        }
                                        else
                                        {
                                            //字符数正好，如8=(1+7)的情况
                                            tempEndName = "";
                                        }
                                    }
                                    newFileName =
                                        tempFrontName + textBox_param5.Text + tempEndName + itemExtension;
                                    newFilePath = Path.Combine(dirName + @"\", newFileName);
                                }
                                else
                                {
                                    //预判出错文件并汇报，忽略这些文件的重命名，此时newFilePath=""
                                    if (WorkErrCount == 0)
                                    {
                                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                    }
                                    WorkErrCount += 1;
                                    MMCore.WriteLine(reportPath, "固定字符数超过前缀字符数:" + item, true);
                                }
                            }
                        }
                        else
                        {
                            //不保护后缀（或参数4默认填了*）
                            if (StartIndex == 0)
                            {
                                tempFrontName = "";
                                //tempMidName = itemName.Substring(0, _strCount);
                                if (itemName.Length == StrCount)
                                {
                                    tempEndName = "";
                                }
                                else
                                {
                                    tempEndName = itemName.Substring(StrCount);
                                }
                            }
                            else
                            {
                                tempFrontName = itemName.Substring(0, StartIndex);
                                //tempMidName = itemName.Substring(_startIndex, _strCount);
                                if (itemName.Length > (StrCount + StartIndex))
                                {
                                    //字符数有余，如8>(1+6)的情况
                                    tempEndName = itemName.Substring(StrCount + 1);
                                }
                                else
                                {
                                    //字符数正好，如8=(1+7)的情况
                                    tempEndName = "";
                                }
                            }
                            newFileName = tempFrontName + textBox_param5.Text + tempEndName;
                            newFilePath = Path.Combine(dirName + @"\", newFileName);
                        }
                        if (
                            newFilePath != ""
                            && !File.Exists(newFilePath)
                            && !Directory.Exists(newFilePath)
                        )
                        {
                            //对非空newFilePath且不会发生覆盖的文件，执行重命名
                            if (!MMCore.IsDir(item))
                            {
                                FileInfo fileInfo = new FileInfo(item);
                                fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在为路径添加随机值
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath),
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                                );
                                            }
                                            if (simRunState == false)
                                                fileInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath) + @"\",
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                                );
                                                counter++;
                                            }
                                            if (simRunState == false)
                                                fileInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }
                            else if (checkBox_dirModification.Checked)
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                                directoryInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                try
                                {
                                    if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                    {
                                        if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (comboBox_SamePathHandle.SelectedIndex == 1)
                                        {
                                            //检查目标路径是否存在
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                //如果存在为路径添加随机值
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath),
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                                );
                                            }
                                            if (simRunState == false)
                                                directoryInfo.MoveTo(newFilePath);
                                        }
                                        else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                        {
                                            counter = 2;
                                            while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                            {
                                                newFilePath = Path.Combine(
                                                    Path.GetDirectoryName(newFilePath) + @"\",
                                                    $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                                );
                                                counter++;
                                            }
                                            if (simRunState == false)
                                                directoryInfo.MoveTo(newFilePath);
                                        }
                                        else
                                        {
                                            if (WorkErrCount == 0)
                                            {
                                                MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                            }
                                            WorkErrCount += 1;
                                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                        }
                                    }
                                }
                                catch (IOException ioEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                    MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                    MessageBox.Show($"无法移动文件：{ioEx.Message}");
                                }
                                catch (UnauthorizedAccessException uaEx)
                                {
                                    MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                    MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                    MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                }
                                catch (Exception ex)
                                {
                                    MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                    MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                    MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                                }
                            }

                            MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                        }
                        else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                        {
                            //newFilePath=""，说明出现预判出错文件且汇报过，现在仅对预判重叠文件进行汇报
                            if (WorkErrCount == 0)
                            {
                                MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                            }
                            WorkErrCount += 1;
                            MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                        }
                    }
                    else
                    {
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "固定字符数超过对象字符数:" + item, true);
                    }

                    if (WorkErrCount > 0)
                    {
                        label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                    }
                    //测试↓
                    //MMCore.WriteLine( reportPath, "item = " + item, true);
                    //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                    //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                    //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                    //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                    //MMCore.WriteLine( reportPath, "tempFrontName = " + tempFrontName, true);
                    //MMCore.WriteLine( reportPath, "tempMidName = " + tempMidName, true);
                    //MMCore.WriteLine( reportPath, "tempEndName = " + tempEndName, true);
                }
            }
        }

        /// <summary>
        /// 功能编号08，批量替换文件（夹）名称的指定字符
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f8(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            string itemFrontName;
            string itemExtension;
            string newFileName;
            string newFilePath = "";
            string reportPath;
            //string tempFrontName;
            //string tempMidName;
            //string tempEndName;
            bool torf = false;
            bool isDir = false;
            int counter;
            //固定功能5和6的参数
            StartIndex = 0;
            StrCount = 1;

            for (int i = 0; i < 1; i++)
            {
                dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1);
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item); //返回的是上一级目录名称
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);

                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }

                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";

                if (textBox_param5.Text == "" && (itemName.Length > (StrCount + StartIndex)))
                {
                    torf = true;
                }
                else if (textBox_param5.Text != "" && (itemName.Length >= (StrCount + StartIndex)))
                {
                    torf = true;
                }
                if (torf) //文件名（带后缀）长度足以支撑修改
                {
                    if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                    {
                        //如填了空格全保护或指定.txt后遇到.txt情况则不操作
                        //若指定.txt后遇到.PNG情况，进行操作
                        if (
                            string.Compare(
                                Path.GetExtension(item),
                                textBox_param4.Text,
                                checkBox_param4.Checked
                            ) != 0
                        )
                        {
                            //处理
                            if (itemFrontName.Length >= (StrCount + StartIndex)) //假设ABCD.txt, 8>=(0+8)或8>=(1+7)
                            {
                                //if (_startIndex == 0)
                                //{
                                //   tempFrontName = "";
                                //   tempMidName = itemFrontName.Substring(0, _strCount);
                                //   if (itemFrontName.Length == _strCount)
                                //   {
                                //       tempEndName = "";
                                //   }
                                //   else
                                //   {
                                //       tempEndName = itemFrontName.Substring(_strCount);
                                //   }
                                //}
                                //else
                                //{
                                //   tempFrontName = itemFrontName.Substring(0, _startIndex);
                                //   tempMidName = itemFrontName.Substring(_startIndex, _strCount);
                                //   if (itemFrontName.Length > (_strCount + _startIndex))
                                //   {
                                //       //字符数有余，如8>(1+6)的情况
                                //       tempEndName = itemFrontName.Substring(_strCount + 1);
                                //   }
                                //   else
                                //   {
                                //       //字符数正好，如8=(1+7)的情况
                                //       tempEndName = "";
                                //   }
                                //}
                                newFileName = itemFrontName.Replace(textBox_param5.Text, textBox_param6.Text) + itemExtension;
                                newFilePath = Path.Combine(dirName + @"\", newFileName);
                            }
                            else
                            {
                                //预判出错文件并汇报，忽略这些文件的重命名，此时newFilePath=""
                                if (WorkErrCount == 0)
                                {
                                    MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                }
                                WorkErrCount += 1;
                                MMCore.WriteLine(reportPath, "固定字符数超过前缀字符数:" + item, true);
                            }
                        }
                    }
                    else
                    {
                        //不保护后缀（或参数4默认填了*）
                        newFileName = itemName.Replace(textBox_param5.Text, textBox_param6.Text);
                        newFilePath = Path.Combine(dirName + @"\", newFileName);
                    }
                    if (
                        newFilePath != ""
                        && !File.Exists(newFilePath)
                        && !Directory.Exists(newFilePath)
                    )
                    {
                        //对非空newFilePath且不会发生覆盖的文件，执行重命名
                        if (!MMCore.IsDir(item))
                        {
                            FileInfo fileInfo = new FileInfo(item);
                            fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                            try
                            {
                                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                {
                                    if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                }
                                else
                                {
                                    if (comboBox_SamePathHandle.SelectedIndex == 1)
                                    {
                                        //检查目标路径是否存在
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //如果存在为路径添加随机值
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath),
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                            );
                                        }
                                        if (simRunState == false)
                                            fileInfo.MoveTo(newFilePath);
                                    }
                                    else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                    {
                                        counter = 2;
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath) + @"\",
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                            );
                                            counter++;
                                        }
                                        if (simRunState == false)
                                            fileInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (WorkErrCount == 0)
                                        {
                                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                        }
                                        WorkErrCount += 1;
                                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                    }
                                }
                            }
                            catch (IOException ioEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                MessageBox.Show($"无法移动文件：{ioEx.Message}");
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                            }
                        }
                        else if (checkBox_dirModification.Checked)
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(item);
                            directoryInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                            try
                            {
                                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                {
                                    if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                }
                                else
                                {
                                    if (comboBox_SamePathHandle.SelectedIndex == 1)
                                    {
                                        //检查目标路径是否存在
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //如果存在为路径添加随机值
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath),
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                            );
                                        }
                                        if (simRunState == false)
                                            directoryInfo.MoveTo(newFilePath);
                                    }
                                    else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                    {
                                        counter = 2;
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath) + @"\",
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                            );
                                            counter++;
                                        }
                                        if (simRunState == false)
                                            directoryInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (WorkErrCount == 0)
                                        {
                                            MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                        }
                                        WorkErrCount += 1;
                                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                    }
                                }
                            }
                            catch (IOException ioEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                MessageBox.Show($"无法移动文件：{ioEx.Message}");
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                            }
                        }

                        MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                    }
                    else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                    {
                        //newFilePath=""，说明出现预判出错文件且汇报过，现在仅对预判重叠文件进行汇报
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                    }
                }
                else
                {
                    if (WorkErrCount == 0)
                    {
                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                    }
                    WorkErrCount += 1;
                    MMCore.WriteLine(reportPath, "固定字符数超过对象字符数:" + item, true);
                }

                if (WorkErrCount > 0)
                {
                    label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                }
                //测试↓
                //MMCore.WriteLine( reportPath, "item = " + item, true);
                //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                //MMCore.WriteLine( reportPath, "tempFrontName = " + tempFrontName, true);
                //MMCore.WriteLine( reportPath, "tempMidName = " + tempMidName, true);
                //MMCore.WriteLine( reportPath, "tempEndName = " + tempEndName, true);
            }
        }

        /// <summary>
        /// 功能编号09，批量替换文件（夹）名称的后缀字符
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f9(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            string itemFrontName;
            string itemExtension;
            string newFileName;
            string newFilePath = "";
            string reportPath;
            string tempFrontName;
            string tempMidName;
            string tempEndName;
            bool torf = false;
            bool isDir = false;
            int counter;
            //固定功能5和6的参数
            StartIndex = 0;
            StrCount = 1;

            for (int i = 0; i < 1; i++)
            {
                dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1);
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item); //返回的是上一级目录名称
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);

                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }

                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";

                if (textBox_param5.Text == "" && (itemName.Length > (StrCount + StartIndex)))
                {
                    torf = true;
                }
                else if (textBox_param5.Text != "" && (itemName.Length >= (StrCount + StartIndex)))
                {
                    torf = true;
                }
                if (torf) //文件名（带后缀）长度足以支撑修改
                {
                    if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                    {
                        //如填了空格全保护或指定.txt后遇到.txt情况则不操作
                        //若指定.txt后遇到.PNG情况，进行操作
                        if (
                            string.Compare(
                                Path.GetExtension(item),
                                textBox_param4.Text,
                                checkBox_param4.Checked
                            ) != 0
                        )
                        {
                            //处理
                            if (itemFrontName.Length >= (StrCount + StartIndex)) //假设ABCD.txt, 8>=(0+8)或8>=(1+7)
                            {
                                if (StartIndex == 0)
                                {
                                    tempFrontName = "";
                                    tempMidName = itemFrontName.Substring(0, StrCount);
                                    if (itemFrontName.Length == StrCount)
                                    {
                                        tempEndName = "";
                                    }
                                    else
                                    {
                                        tempEndName = itemFrontName.Substring(StrCount);
                                    }
                                }
                                else
                                {
                                    tempFrontName = itemFrontName.Substring(0, StartIndex);
                                    tempMidName = itemFrontName.Substring(StartIndex, StrCount);
                                    if (itemFrontName.Length > (StrCount + StartIndex))
                                    {
                                        //字符数有余，如8>(1+6)的情况
                                        tempEndName = itemFrontName.Substring(StrCount + 1);
                                    }
                                    else
                                    {
                                        //字符数正好，如8=(1+7)的情况
                                        tempEndName = "";
                                    }
                                }
                                newFileName =
                                    tempFrontName + tempMidName + tempEndName + textBox_param5.Text;
                                newFilePath = Path.Combine(dirName + @"\", newFileName);
                            }
                            else
                            {
                                //预判出错文件并汇报，忽略这些文件的重命名，此时newFilePath=""
                                if (WorkErrCount == 0)
                                {
                                    MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                }
                                WorkErrCount += 1;
                                MMCore.WriteLine(reportPath, "固定字符数超过前缀字符数:" + item, true);
                            }
                        }
                    }
                    else
                    {
                        //不保护后缀（或参数4默认填了*）
                        newFileName = itemFrontName + textBox_param5.Text;
                        newFilePath = Path.Combine(dirName + @"\", newFileName);
                    }
                    if (
                        newFilePath != ""
                        && !File.Exists(newFilePath)
                        && !Directory.Exists(newFilePath)
                    )
                    {
                        //对非空newFilePath且不会发生覆盖的文件，执行重命名
                        if (!MMCore.IsDir(item))
                        {
                            FileInfo fileInfo = new FileInfo(item);
                            fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                            try
                            {
                                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                {
                                    if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                }
                                else
                                {
                                    if (comboBox_SamePathHandle.SelectedIndex == 1)
                                    {
                                        //检查目标路径是否存在
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //如果存在为路径添加随机值
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath),
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                            );
                                        }
                                        if (simRunState == false)
                                            fileInfo.MoveTo(newFilePath);
                                    }
                                    else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                    {
                                        counter = 2;
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath) + @"\",
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                            );
                                            counter++;
                                        }
                                        if (simRunState == false)
                                            fileInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (WorkErrCount == 0)
                                        {
                                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                        }
                                        WorkErrCount += 1;
                                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                    }
                                }
                            }
                            catch (IOException ioEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                MessageBox.Show($"无法移动文件：{ioEx.Message}");
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                            }
                        }
                        else if (checkBox_dirModification.Checked)
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(item);
                            directoryInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                            try
                            {
                                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                {
                                    if (simRunState == false) directoryInfo.MoveTo(newFilePath);
                                }
                                else
                                {
                                    if (comboBox_SamePathHandle.SelectedIndex == 1)
                                    {
                                        //检查目标路径是否存在
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //如果存在为路径添加随机值
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath),
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                            );
                                        }
                                        if (simRunState == false)
                                            directoryInfo.MoveTo(newFilePath);
                                    }
                                    else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                    {
                                        counter = 2;
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath) + @"\",
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                            );
                                            counter++;
                                        }
                                        if (simRunState == false)
                                            directoryInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (WorkErrCount == 0)
                                        {
                                            MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                        }
                                        WorkErrCount += 1;
                                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                    }
                                }
                            }
                            catch (IOException ioEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                MessageBox.Show($"无法移动文件：{ioEx.Message}");
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                            }
                        }

                        MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                    }
                    else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                    {
                        //newFilePath=""，说明出现预判出错文件且汇报过，现在仅对预判重叠文件进行汇报
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                    }
                }
                else
                {
                    if (WorkErrCount == 0)
                    {
                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                    }
                    WorkErrCount += 1;
                    MMCore.WriteLine(reportPath, "固定字符数超过对象字符数:" + item, true);
                }

                if (WorkErrCount > 0)
                {
                    label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                }
                //测试↓
                //MMCore.WriteLine( reportPath, "item = " + item, true);
                //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                //MMCore.WriteLine( reportPath, "tempFrontName = " + tempFrontName, true);
                //MMCore.WriteLine( reportPath, "tempMidName = " + tempMidName, true);
                //MMCore.WriteLine( reportPath, "tempEndName = " + tempEndName, true);
            }
        }

        /// <summary>
        /// 功能编号10，批量删除（移动/复制）指定名称的文件（夹）
        /// </summary>
        /// <param name="workFilePath"></param>
        /// <param name="item"></param>
        private void DAction_f10(string workFilePath, string item)
        {
            string dirName;
            string itemName;
            //string itemFrontName;
            string itemExtension;
            string newFileName;
            string newFilePath = "";
            string reportPath;
            bool isDir = false;
            FileInfo fileInfo;
            DirectoryInfo dirInfo;
            string baseName; string extension; string tempFileName; string tempdirName; int counter; string randomValue;


            for (int i = 0; i < 1; i++)
            {
                //dirName = item.Substring(0, item.LastIndexOf('\\'));
                itemName = item.Substring(item.LastIndexOf('\\') + 1); //最后一个\之后的全部字符
                if (MMCore.IsDir(item))
                {
                    //itemFrontName = Path.GetDirectoryName(item); //返回的是上一级目录名称
                    //itemFrontName = Path.GetFileNameWithoutExtension(item);
                    isDir = true;
                }
                else
                {
                    //itemFrontName = Path.GetFileNameWithoutExtension(item);
                }
                itemExtension = Path.GetExtension(item);

                if (itemExtension == "")
                {
                    if (checkBox_emptySuffixFileIgnore.Checked && !isDir) { break; }//忽略空后缀文件
                    if (checkBox_emptySuffixDirIgnore.Checked && isDir) { break; }//忽略空后缀目录
                }
                else
                {
                    if (checkBox_nonEmptySuffixFileIgnore.Checked && !isDir) { break; }//忽略非空后缀文件
                    if (checkBox_nonEmptySuffixDirIgnore.Checked && isDir) { break; }//忽略非空后缀目录
                }

                reportPath =
                    workFilePath.Substring(0, workFilePath.LastIndexOf('\\'))
                    + @"\"
                    + Path.GetFileNameWithoutExtension(workFilePath)
                    + @"_Report.txt";



                if (checkBox_protectSuffix.Checked && textBox_param4.Text != "*") //指定后缀且勾选后缀保护
                {
                    //如填了空格全保护或指定.txt后遇到.txt情况则不操作
                    //若指定.txt后遇到.PNG情况，进行操作
                    if (
                        string.Compare(
                            Path.GetExtension(item),
                            textBox_param4.Text,
                            checkBox_param4.Checked
                        ) != 0
                    )
                    {
                        newFileName = itemName;
                    }
                    else
                    {
                        //如指定.txt后遇到.txt情况，输出空，然后报告过滤情况，此时newFilePath=""
                        if (WorkErrCount == 0)
                        {
                            MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                        }
                        WorkErrCount += 1;
                        MMCore.WriteLine(reportPath, "用户保护:" + item, true);
                        break;
                    }
                }
                else
                {
                    //不保护后缀（或参数4默认填了*）
                    newFileName = itemName;
                }
                //用户自定目录的验证和替换
                if (comboBox_param8.SelectedIndex == 0)
                {
                    if (MMCore.IsDFPath(textBox_param8.Text) && MMCore.IsDir(textBox_param8.Text))
                    {
                        dirName = textBox_param8.Text;
                    }
                    else
                    {
                        //无效时，按默认输出
                        dirName = workFilePath.Substring(0, workFilePath.LastIndexOf('\\')) + @"\" + Path.GetFileNameWithoutExtension(workFilePath) + @"_Delete";
                        textBox_param8.Text = dirName;
                    }
                    newFilePath = Path.Combine(dirName + @"\", newFileName);
                    if (!Directory.Exists(dirName)) { Directory.CreateDirectory(dirName); }
                }

                if (!isDir)
                {//处理对象是文件
                    switch (comboBox_param8.SelectedIndex)
                    {
                        case 0:
                            if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                            {
                                //新路径必须不存在才可以移入
                                fileInfo = new FileInfo(item);
                                fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                if (simRunState == false) fileInfo.CopyTo(newFilePath);
                                MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                            }
                            else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                            {//已有同名文件或目录则进行处理
                                if (comboBox_SamePathHandle.SelectedIndex == 1)
                                {
                                    //为路径添加一个随机值
                                    baseName = Path.GetFileNameWithoutExtension(newFilePath);
                                    extension = Path.GetExtension(newFilePath);
                                    tempdirName = Path.GetDirectoryName(newFilePath);
                                    while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                    {
                                        tempFileName = $"{baseName}_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                                        newFilePath = Path.Combine(tempdirName + @"\", tempFileName);
                                    }
                                    fileInfo = new FileInfo(item);
                                    fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                    if (simRunState == false) fileInfo.CopyTo(newFilePath);
                                    MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                                }
                                else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                {
                                    baseName = Path.GetFileNameWithoutExtension(newFilePath);
                                    extension = Path.GetExtension(newFilePath);
                                    tempdirName = Path.GetDirectoryName(newFilePath);
                                    counter = 2;
                                    while (File.Exists(newFilePath))
                                    {
                                        tempFileName = $"{baseName}({counter}){extension}";
                                        newFilePath = Path.Combine(tempdirName + @"\", tempFileName);
                                        counter++;
                                    }
                                    fileInfo = new FileInfo(item);
                                    fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                    if (simRunState == false) fileInfo.CopyTo(newFilePath);
                                    MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                                }
                                else
                                {
                                    if (WorkErrCount == 0)
                                    {
                                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                    }
                                    WorkErrCount += 1;
                                    MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                }
                            }
                            break;
                        case 1:
                            if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                            {
                                //新路径必须不存在才可以移入
                                fileInfo = new FileInfo(item);
                                fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                            }
                            else if (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                            {//已有同名文件或目录则进行处理
                                if (comboBox_SamePathHandle.SelectedIndex == 1)
                                {
                                    //为路径添加一个随机值
                                    baseName = Path.GetFileNameWithoutExtension(newFilePath);
                                    extension = Path.GetExtension(newFilePath);
                                    tempdirName = Path.GetDirectoryName(newFilePath);
                                    while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                    {
                                        tempFileName = $"{baseName}_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                                        newFilePath = Path.Combine(tempdirName + @"\", tempFileName);
                                    }
                                    fileInfo = new FileInfo(item);
                                    fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                    if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                    MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                                }
                                else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                {
                                    baseName = Path.GetFileNameWithoutExtension(newFilePath);
                                    extension = Path.GetExtension(newFilePath);
                                    tempdirName = Path.GetDirectoryName(newFilePath);
                                    counter = 2;
                                    while (File.Exists(newFilePath))
                                    {
                                        tempFileName = $"{baseName}({counter}){extension}";
                                        newFilePath = Path.Combine(tempdirName + @"\", tempFileName);
                                        counter++;
                                    }
                                    fileInfo = new FileInfo(item);
                                    fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                                    if (simRunState == false) fileInfo.MoveTo(newFilePath);
                                    MMCore.WriteLine(workFilePath, "【处理完成】" + item + " => " + newFilePath, true);
                                }
                                else
                                {
                                    if (WorkErrCount == 0)
                                    {
                                        MMCore.WriteLine(reportPath, "↓未处理文件如下↓", false);
                                    }
                                    WorkErrCount += 1;
                                    MMCore.WriteLine(reportPath, "预判到路径会重叠文件:" + item, true);
                                }
                            }
                            break;
                        case 2:
                            if (simRunState == false) MMCore.DelFileToRecycleBin(item, checkBox_param8.Checked);
                            break;
                        case 3:
                            fileInfo = new FileInfo(item);
                            fileInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);//解除占用
                            if (simRunState == false) fileInfo.Delete();
                            break;
                        default:
                            break;
                    }
                }
                else if (checkBox_dirModification.Checked)
                {//处理对象是目录
                    switch (comboBox_param8.SelectedIndex)
                    {
                        case 0:
                            dirInfo = new DirectoryInfo(item);
                            dirInfo.Attributes &= ~(
                                FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden
                            ); //解除占用
                            try
                            {
                                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                {
                                    if (simRunState == false)
                                        MMCore.CopyDirectory(item, newFilePath);
                                }
                                else
                                {
                                    if (comboBox_SamePathHandle.SelectedIndex == 1)
                                    {
                                        //检查目标路径是否存在
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //如果存在，为路径添加一个随机值
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath),
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                            );
                                        }
                                        if (simRunState == false)
                                            MMCore.CopyDirectory(item, newFilePath);
                                    }
                                    else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                    {
                                        counter = 2;
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //这个@"\"加不加都无所谓
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath) + @"\",
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                            );
                                            counter++;
                                        }
                                        if (simRunState == false)
                                            MMCore.CopyDirectory(item, newFilePath);
                                    }
                                    else
                                    {
                                        if (WorkErrCount == 0)
                                        {
                                            MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                        }
                                        WorkErrCount += 1;
                                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                    }
                                }
                            }
                            catch (IOException ioEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法复制文件：{ioEx.Message}", false);
                                MMCore.Tell($"无法复制文件：{ioEx.Message}");
                                MessageBox.Show($"无法复制文件：{ioEx.Message}");
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                MMCore.WriteLine(reportPath, $"复制文件时发生错误：{ex.Message}", false);
                                MMCore.Tell($"复制文件时发生错误：{ex.Message}");
                                MessageBox.Show($"复制文件时发生错误：{ex.Message}");
                            }
                            break;
                        case 1:
                            dirInfo = new DirectoryInfo(item);
                            dirInfo.Attributes &= ~(
                                FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden
                            ); //解除占用
                            try
                            {
                                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                                {
                                    if (simRunState == false)
                                        dirInfo.MoveTo(newFilePath);
                                }
                                else
                                {
                                    if (comboBox_SamePathHandle.SelectedIndex == 1)
                                    {
                                        //检查目标路径是否存在
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //如果存在，为路径添加一个随机值
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath),
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(newFilePath)}"
                                            );
                                        }
                                        if (simRunState == false)
                                            dirInfo.MoveTo(newFilePath);
                                    }
                                    else if (comboBox_SamePathHandle.SelectedIndex == 2)
                                    {
                                        counter = 2;
                                        while (File.Exists(newFilePath) || Directory.Exists(newFilePath))
                                        {
                                            //这个@"\"加不加都无所谓
                                            newFilePath = Path.Combine(
                                                Path.GetDirectoryName(newFilePath) + @"\",
                                                $"{Path.GetFileNameWithoutExtension(newFilePath)}({counter}){Path.GetExtension(newFilePath)}"
                                            );
                                            counter++;
                                        }
                                        if (simRunState == false)
                                            dirInfo.MoveTo(newFilePath);
                                    }
                                    else
                                    {
                                        if (WorkErrCount == 0)
                                        {
                                            MMCore.WriteLine(reportPath, "↓未处理文件夹如下↓", false);
                                        }
                                        WorkErrCount += 1;
                                        MMCore.WriteLine(reportPath, "预判到路径会重叠文件夹:" + item, true);
                                    }
                                }
                            }
                            catch (IOException ioEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法移动文件：{ioEx.Message}", false);
                                MMCore.Tell($"无法移动文件：{ioEx.Message}");
                                MessageBox.Show($"无法移动文件：{ioEx.Message}");
                            }
                            catch (UnauthorizedAccessException uaEx)
                            {
                                MMCore.WriteLine(reportPath, $"无法访问文件，因为没有足够的权限：{uaEx.Message}", false);
                                MMCore.Tell($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                                MessageBox.Show($"无法访问文件，因为没有足够的权限：{uaEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                MMCore.WriteLine(reportPath, $"移动文件时发生错误：{ex.Message}", false);
                                MMCore.Tell($"移动文件时发生错误：{ex.Message}");
                                MessageBox.Show($"移动文件时发生错误：{ex.Message}");
                            }
                            break;
                        case 2:
                            if (simRunState == false) MMCore.DelDirectoryToRecycleBin(item, checkBox_param8.Checked);
                            break;
                        case 3:
                            dirInfo = new DirectoryInfo(item);
                            if (simRunState == false) MMCore.DelDirectoryRecursively(dirInfo);
                            break;
                        default:
                            break;
                    }
                }
                if (WorkErrCount > 0)
                {
                    label_headTip.Text = $"{WorkErrCount}个文件处理失败！请查看Report文件";
                }
                //测试↓
                //MMCore.WriteLine( reportPath, "item = " + item, true);
                //MMCore.WriteLine( reportPath, "dirName = " + dirName, true);
                //MMCore.WriteLine( reportPath, "itemName = " + itemName, true);
                //MMCore.WriteLine( reportPath, "itemFrontName = " + itemFrontName, true);
                //MMCore.WriteLine( reportPath, "itemExtension = " + itemExtension, true);
                //MMCore.WriteLine( reportPath, "tempFrontName = " + tempFrontName, true);
                //MMCore.WriteLine( reportPath, "tempMidName = " + tempMidName, true);
                //MMCore.WriteLine( reportPath, "tempEndName = " + tempEndName, true);
            }
        }

        /// <summary>
        /// 检索文件和目录并打印或修改
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="workFilePath"></param>
        /// <param name="diskSymbol"></param>
        private void DFRun(string workPath, string workFilePath, string diskSymbol)
        {
            string[] directories;
            for (int i = 0; i < 1; i++)
            {
                label_dirStatistics.Text = "任务执行中...点击此处可查看进度百分比";
                ProCount = 0;
                //首次向工作文本写入信息
                FirstWrite(workFilePath, diskSymbol);
                if (!(checkBox_traversalRecursive.Checked && checkBox_recursion.Checked))
                {
                    if (checkBox_DFDividually.Checked)
                    {//勾选目录与文件分列（常用于打印输出时严格分开）
                        if (checkBox_dirFirst.Checked)
                        {//勾选优先检索（处理）目录
                            DirRun(workPath, workFilePath);
                            MMCore.WriteLine(workFilePath, "", true);
                            FileRun(workPath, workFilePath);
                        }
                        else
                        {//否则优先检索（处理）文件
                            FileRun(workPath, workFilePath);
                            MMCore.WriteLine(workFilePath, "", true);
                            DirRun(workPath, workFilePath);
                        }
                    }
                    else
                    {//不严格分列情况下
                        DFPrint(workPath, workFilePath, true);//处理最外层目录（同时允许修改一层直接子目录）
                        if (checkBox_recursion.Checked)
                        {//允许遍历子目录（禁用目录修改）
                            directories = Directory.GetDirectories(workPath, "*", SearchOption.AllDirectories);//获取了所有子目录路径字符
                            ProCountMax = directories.Length;
                            foreach (var item in directories)
                            {//处理每个遍历到的子目录
                                if (WorkStop) { break; }
                                ProCount++;
                                DFPrint(item, workFilePath);
                                if (WorkStatus)
                                {
                                    label_dirStatistics.Text = ProCount.ToString() + @"/" + ProCountMax.ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    //递归遍历子目录并进行功能处理
                    DFRecursively(workPath, workFilePath, false);

                }
                bool torf;
                if (checkBox_keepHistory.Checked)
                {
                    torf = true;
                }
                else
                {
                    torf = false;
                }
                MMCore.WriteLine(workFilePath, "████████████████████████████████████████████" + "\r\n" + "", true, true, torf);//尾行留空

                #region 测试
                //string randomValue = Guid.NewGuid().ToString().Substring(0, 8); //生成一个8位的随机字符串
                //string directoryName = Path.GetFileName(workPath);
                //string parentDirectoryPath = Path.GetDirectoryName(workPath);
                //string newWorkPath = Path.Combine(parentDirectoryPath, $"{directoryName}_{randomValue}");
                //MMCore.Tell(string.Format("randomValue={0} directoryName={1} parentDirectoryPath={2} newWorkPath={3}", randomValue, directoryName, parentDirectoryPath, newWorkPath));
                ////randomValue = cfcff240 directoryName = Dir parentDirectoryPath = C:\Users\linsh\Desktop newWorkPath = C:\Users\linsh\Desktop\Dir_cfcff240

                //randomValue = Guid.NewGuid().ToString().Substring(0, 8); //生成一个8位随机字符串
                //string fileName = Path.GetFileName(workFilePath);
                //string directoryPath = Path.GetDirectoryName(workFilePath);
                //string newWorkFilePath = Path.Combine(directoryPath, $"{Path.GetFileNameWithoutExtension(fileName)}_{randomValue}{Path.GetExtension(fileName)}");
                //MMCore.Tell(string.Format("randomValue={0} fileName={1} directoryPath={2} newWorkFilePath={3}", randomValue, fileName, directoryPath, newWorkFilePath));
                ////randomValue = 5fc90e46 fileName = test.txt directoryPath = C:\Users\linsh\Desktop newWorkFilePath = C:\Users\linsh\Desktop\test_5fc90e46.txt
                #endregion
            }
            if (!WorkStop) { label_dirStatistics.Text = "任务已完成！"; }
        }

        /// <summary>
        /// 根据是否勾选“优先检索目录”进行目录内一层搜索处理，早期仅用于打印功能，升级后支持按用户指定功能动作。
        /// 允许修改目录时函数外部子目录遍历需手动处理，可能存在非递归进行，外层目录被先改名会致内层路径出错，故不可手动调用于非递归的子目录遍历（打印功能不受限）。
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="workFilePath"></param>
        /// <param name="torf">是否允许目录下直接子目录的修改（当函数外部非遍历或递归遍历目录先改最内目录再层层退出情况下方可允许）</param>
        private void DFPrint(string path, string workFilePath, bool torf = false)
        {
            if (comboBox_selectFunc.SelectedIndex == 0 && checkBox_dirFirst.Checked)
            {//打印功能且优先检索（处理）目录时
                //先打印（修改）目录
                DPrint(path, workFilePath, torf);
                FPrint(path, workFilePath);
            }
            else
            {//否则始终优先处理文件再处理目录
                //先打印（修改）文件
                FPrint(path, workFilePath);
                //打印功能不受限，不可调用于非递归目录遍历修改（会导致目录改后路径出错，该函数内部也会纠错）。
                DPrint(path, workFilePath, torf);
            }
        }

        /// <summary>
        /// 检索目录并按用户功能动作
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="workFilePath"></param>
        /// <param name="torf">是否允许目录下直接子目录的修改（当函数外部非遍历或递归遍历目录先改最内目录再层层退出情况下方可允许）</param>
        private void DPrint(string path, string workFilePath, bool torf = false)
        {
            string[] directories;
            string tempStr = "";
            string dirName;
            string dirSize = "";
            string dirTime = "";
            string a = "";
            string b = "";
            int j = 0;
            int k = 0;
            DirectoryInfo directoryInfo;
            if (checkBox_dirSearch.Checked) //允许检索目录
            {
                for (int i = 0; i < 1; i++)
                {//仅执行一次，方便写break;
                    if (checkBox_emptyDirIgnore.Checked && MMCore.IsDirectoryEmptyUserDef(path))
                    {//勾选忽略空目录且验证到路径是用户定义的空目录类型
                        //MMCore.WriteLine(workFilePath, "忽略空目录 = " + path, true);
                        break;//勾选了忽略空目录，遇空目录时跳出
                    }
                    directories = Directory.GetDirectories(path); //获取直接子目录（不进行子目录的递归遍历）路径名称以字符形式存入字符组
                    if (directories.Length == 0 && comboBox_selectFunc.SelectedIndex == 0)
                    {//没有子目录且功能是打印
                        if (DirCount == 0)
                        {
                            //因循环体仅执行一次，只会运行此动作
                            DirCount = 1;
                        }
                        else
                        {
                            //无效动作
                            MMCore.WriteLine(workFilePath, "", true);
                        }
                        MMCore.WriteLine(workFilePath, " Directory of " + path, true); //首次输出path，表示正在对其检索
                        if (checkBox_fileStatistics.Checked || checkBox_DirCount.Checked)
                        {
                            if (checkBox_fileStatistics.Checked)
                            {
                                j = Directory.GetFiles(path).Length;
                                a = "               " + j.ToString() + " File(s)";
                            }
                            if (checkBox_DirCount.Checked)
                            {
                                k = Directory.GetDirectories(path).Length;
                                b = "             " + k.ToString() + " DIR(s)";
                            }
                            MMCore.WriteLine(workFilePath, a + b, true);
                        }
                        if (j + k > 0) { MMCore.WriteLine(workFilePath, "", true); }//目录有内容时换行以隔开内容
                    }
                    foreach (var item in directories) //遍历path内的直接子目录
                    {
                        if (WorkStop) { break; }
                        //开始过滤
                        if (checkBox_emptyDirIgnore.Checked && MMCore.IsDirectoryEmpty(item))
                        {//勾选忽略空目录且验证到路径是用户定义的空目录类型
                            //MMCore.WriteLine(workFilePath, "子目录为空：" + item , true);
                            continue; //忽略空目录时，子目录为空亦跳过
                        }
                        if (checkBox_nonEmptyDirIgnore.Checked && !MMCore.IsDirectoryEmptyUserDef(item))
                        {
                            continue; //忽略非空目录（只输出空目录），那么非空目录将被跳过
                        }
                        //通配符错误时重置
                        if (textBox_param3.Text == "*") { }
                        else if (string.Compare(Path.GetExtension(item), textBox_param3.Text, checkBox_param3.Checked) != 0) { continue; }
                        if (checkBox_specialStr.Checked && textBox_specialStr.Text != "")
                        {//启用了关键字并且关键字不为空时
                            if (checkBox_regular.Checked)
                            {//启用了正则表达式
                                //从文件路径提取的文件名是否匹配文本框中的正则表达式（textBox_specialStr.Text），如果不匹配则使用 continue 跳过当前循环迭代
                                if (!Regex.IsMatch(item.Substring(item.LastIndexOf('\\') + 1), textBox_specialStr.Text)) { continue; }
                            }
                            else
                            {
                                //未启用正则情况下
                                if (checkBox_specialStrIgnoreCase.Checked)
                                {
                                    //忽略大小写
                                    //IndexOf函数对大小写不敏感，适用于不区分大小写的判断，返回值为int型（在sring中的索引值）
                                    if (item.Substring(item.LastIndexOf('\\') + 1).IndexOf(textBox_specialStr.Text, StringComparison.OrdinalIgnoreCase) == -1)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    //String.Contains对大小写敏感，适用于区分大小写的判断
                                    if (!item.Substring(item.LastIndexOf('\\') + 1).Contains(textBox_specialStr.Text))
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        if (checkBox_range.Checked)
                        {//启用范围大小功能时
                         //若未反选则在范围外的正常被过滤，若反选则范围内的被过滤
                            if ((checkBox_RangeReversal.Checked && ParamIsInRange(item)) ||
                                (!checkBox_RangeReversal.Checked && !ParamIsInRange(item)))
                            {
                                continue;
                            }
                        }
                        //经过过滤后的子目录开始处理
                        switch (comboBox_selectFunc.SelectedIndex)
                        {
                            case 0:
                                //如果是打印功能
                                //MMCore.WriteLine(workFilePath, "目录：" + item.Substring(0, item.LastIndexOf('\\'))+" =>上回："+ tempStr, true);
                                if (item.Substring(0, item.LastIndexOf('\\')) != tempStr) //内部循环若不是遍历到同一个子目录（目前必然通过）
                                {
                                    tempStr = item.Substring(0, item.LastIndexOf('\\'));
                                    if (DirCount == 0)
                                    {
                                        //无效动作
                                        DirCount = 1;
                                    }
                                    else
                                    {
                                        //DirCount已经为1，所有的子目录信息必然运行此动作，进行换行输出
                                        MMCore.WriteLine(workFilePath, "", true);
                                    }
                                    MMCore.WriteLine(workFilePath, " Directory of " + tempStr, true); //输出子目录，表示正在对其检索
                                    if (checkBox_fileStatistics.Checked || checkBox_DirCount.Checked)
                                    {
                                        if (checkBox_fileStatistics.Checked)
                                        {
                                            j = Directory.GetFiles(path).Length;
                                            a = "               " + j.ToString() + " File(s)";
                                        }
                                        if (checkBox_DirCount.Checked)
                                        {
                                            k = Directory.GetDirectories(path).Length;
                                            b = "             " + k.ToString() + " DIR(s)";
                                        }
                                        MMCore.WriteLine(workFilePath, a + b, true);
                                    }
                                    if (j + k > 0) { MMCore.WriteLine(workFilePath, "", true); }//目录有内容时换行以隔开内容
                                }

                                if (checkBox_printDirPath.Checked)
                                {//勾选输出目录全路径情况
                                    dirName = item;
                                }
                                else
                                {//仅输出名称
                                    dirName = item.Substring(item.LastIndexOf('\\') + 1);
                                }
                                directoryInfo = new DirectoryInfo(item); //根据子目录字符元素建立其目录信息实例
                                if (checkBox_printDirSize.Checked)
                                {//打印目录大小
                                    dirSize = MMCore.CountSize(MMCore.GetDirectoryLength(item), checkBox_byteCount.Checked);
                                }
                                if (checkBox_printDirTime.Checked)
                                {//打印目录最后写入时间（修改日期）
                                    dirTime = directoryInfo.LastWriteTime.ToString();
                                }
                                dirName = dirTime + " <DIR> " + dirSize + " " + dirName;
                                //打印上述信息
                                MMCore.WriteLine(workFilePath, dirName, true);
                                break;
                            default:
                                //修改动作（函数升级后虽可用，但外部非递归情况下不允许处理目录的修改）
                                if (checkBox_dirModification.Checked && torf)
                                {
                                    DAction(workFilePath, item);
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检索文件并按用户功能动作（不含子目录遍历）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="workFilePath"></param>
        private void FPrint(string path, string workFilePath)
        {
            string[] files;
            string fileName;
            string newFileName;
            string newFilePath;
            string fileSize = "";
            string fileTime = "";
            //string tempStr = "";
            //string a = "";
            //string b = "";
            DirectoryInfo directoryInfo;
            FileInfo[] fileInfos;
            if (checkBox_fileSearch.Checked) //允许检索文件
            {
                for (int i = 0; i < 1; i++)
                {
                    if (checkBox_suffix.Checked) //输出文件后缀
                    {
                        files = Directory.GetFiles(path);
                        foreach (var item in files) //处理每个遍历到的文件名字符
                        {
                            if (WorkStop) { break; }
                            //MMCore.WriteLine(workFilePath, "文件大小" + MMCore.CountSize(MMCore.GetFileLength(item),checkBox39.Checked), true);//测试专用
                            if (checkBox_emptyFileIgnore.Checked && MMCore.CountSize(MMCore.GetFileLength(item), checkBox_byteCount.Checked) == "0 Byte")
                            {
                                continue; //勾选了忽略空文件，遇空文件时跳过
                            }
                            if (checkBox_nonEmptyFileIgnore.Checked && !(MMCore.CountSize(MMCore.GetFileLength(item), checkBox_byteCount.Checked) == "0 Byte"))
                            {
                                continue; //勾选了忽略非空文件，遇非空文件时跳过
                            }
                            //通配符错误时重置
                            if (textBox_param3.Text == "*") { }
                            else if (string.Compare(Path.GetExtension(item), textBox_param3.Text, checkBox_param3.Checked) != 0) { continue; }
                            if (checkBox_specialStr.Checked && textBox_specialStr.Text != "")
                            {
                                if (checkBox_regular.Checked)
                                {
                                    if (!Regex.IsMatch(item.Substring(item.LastIndexOf('\\') + 1), textBox_specialStr.Text)) { continue; }
                                }
                                else
                                {
                                    //未启用正则情况下
                                    if (checkBox_specialStrIgnoreCase.Checked)
                                    {
                                        //忽略大小写
                                        //IndexOf函数对大小写不敏感，适用于不区分大小写的判断，返回值为int型（在sring中的索引值）
                                        if (item.Substring(item.LastIndexOf('\\') + 1).IndexOf(textBox_specialStr.Text, StringComparison.OrdinalIgnoreCase) == -1)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //String.Contains对大小写敏感，适用于区分大小写的判断
                                        if (!item.Substring(item.LastIndexOf('\\') + 1).Contains(textBox_specialStr.Text))
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (checkBox_range.Checked)
                            {//启用范围大小功能时
                             //若未反选则在范围外的正常被过滤，若反选则范围内的被过滤
                                if ((checkBox_RangeReversal.Checked && ParamIsInRange(item)) ||
                                    (!checkBox_RangeReversal.Checked && !ParamIsInRange(item)))
                                {
                                    continue;
                                }
                            }
                            switch (comboBox_selectFunc.SelectedIndex)
                            {
                                case 0:
                                    if (checkBox_printFilePath.Checked) //勾选文件全路径
                                    {
                                        fileName = item;
                                    }
                                    else
                                    {
                                        fileName = item.Substring(item.LastIndexOf('\\') + 1);
                                    }
                                    FileInfo fileInfo = new FileInfo(item);
                                    if (checkBox_printFileSize.Checked)
                                    {
                                        fileSize = MMCore.CountSize(MMCore.GetFileLength(item), checkBox_byteCount.Checked);
                                    }
                                    if (checkBox_printFileTime.Checked)
                                    {
                                        fileTime = fileInfo.LastWriteTime.ToString();
                                    }
                                    fileName = fileTime + " " + fileSize + " " + fileName;
                                    MMCore.WriteLine(workFilePath, fileName, true);
                                    break;
                                default:
                                    if (checkBox_fileModification.Checked)
                                    {
                                        DAction(workFilePath, item);
                                    }
                                    break;
                            }
                        }
                    }
                    else //不输出文件后缀
                    {
                        directoryInfo = new DirectoryInfo(path);
                        fileInfos = directoryInfo.GetFiles();
                        foreach (FileInfo fileInfo in fileInfos)
                        {
                            if (WorkStop) { break; }
                            if (
                                checkBox_emptyFileIgnore.Checked
                                && MMCore.CountSize(MMCore.GetFileLength(fileInfo.Name), checkBox_byteCount.Checked) == "0 Byte"
                            )
                            {
                                continue; //勾选了忽略空文件，遇空文件时跳过
                            }
                            if (
                                checkBox_nonEmptyFileIgnore.Checked
                                && !(MMCore.CountSize(MMCore.GetFileLength(fileInfo.Name), checkBox_byteCount.Checked) == "0 Byte")
                            )
                            {
                                continue; //勾选了忽略非空文件，遇非空文件时跳过
                            }
                            //通配符错误时重置
                            if (textBox_param3.Text == "*") { }
                            else if (
                                string.Compare(
                                    Path.GetExtension(fileInfo.Name),
                                    textBox_param3.Text,
                                    checkBox_param3.Checked
                                ) != 0
                            )
                            {
                                continue;
                            }
                            if (checkBox_specialStr.Checked && textBox_specialStr.Text != "")
                            {
                                if (checkBox_regular.Checked)
                                {
                                    if (!Regex.IsMatch(fileInfo.Name.Substring(fileInfo.Name.LastIndexOf('\\') + 1), textBox_specialStr.Text)) { continue; }
                                }
                                else
                                {
                                    //未启用正则情况下
                                    if (checkBox_specialStrIgnoreCase.Checked)
                                    {
                                        //忽略大小写
                                        //IndexOf函数对大小写不敏感，适用于不区分大小写的判断，返回值为int型（在sring中的索引值）
                                        if (fileInfo.Name.Substring(fileInfo.Name.LastIndexOf('\\') + 1).IndexOf(textBox_specialStr.Text, StringComparison.OrdinalIgnoreCase) == -1)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //String.Contains对大小写敏感，适用于区分大小写的判断
                                        if (!fileInfo.Name.Substring(fileInfo.Name.LastIndexOf('\\') + 1).Contains(textBox_specialStr.Text))
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (checkBox_range.Checked && !ParamIsInRange(fileInfo.Name))
                            {
                                continue;
                            }
                            switch (comboBox_selectFunc.SelectedIndex)
                            {
                                case 0:
                                    if (checkBox_printFilePath.Checked) //勾选允许文件全路径
                                    {
                                        newFilePath = Path.Combine(
                                            fileInfo.DirectoryName,
                                            Path.GetFileNameWithoutExtension(fileInfo.Name)
                                        );
                                        if (checkBox_printFileSize.Checked)
                                        {
                                            fileSize = MMCore.CountSize(MMCore.GetFileLength(fileInfo.FullName), checkBox_byteCount.Checked);
                                        }
                                        if (checkBox_printFileTime.Checked)
                                        {
                                            fileTime = fileInfo.LastWriteTime.ToString();
                                        }
                                        newFilePath = fileTime + " " + fileSize + " " + newFilePath;
                                        MMCore.WriteLine(workFilePath, newFilePath, true);
                                    }
                                    else
                                    {
                                        newFileName = Path.GetFileNameWithoutExtension(
                                            fileInfo.Name
                                        );
                                        if (checkBox_printFileSize.Checked)
                                        {
                                            fileSize = MMCore.CountSize(MMCore.GetFileLength(fileInfo.FullName), checkBox_byteCount.Checked);
                                        }
                                        if (checkBox_printFileTime.Checked)
                                        {
                                            fileTime = fileInfo.LastWriteTime.ToString();
                                        }
                                        newFileName = fileTime + " " + fileSize + " " + newFileName;
                                        MMCore.WriteLine(workFilePath, newFileName, true);
                                    }
                                    break;
                                default:
                                    if (checkBox_fileModification.Checked)
                                    {
                                        DAction(workFilePath, fileInfo.FullName);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 勾选目录与文件分列时的目录处理动作
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="workFilePath"></param>
        private void DirRun(string workPath, string workFilePath)
        {
            string dirName;
            string[] directories;
            string dirSize = "";
            string dirTime = "";
            DirectoryInfo directoryInfo;
            if (checkBox_dirSearch.Checked) //允许检索目录
            {
                for (int i = 0; i < 1; i++)
                {
                    if (!checkBox_recursion.Checked)
                    {
                        //（允许目录修改）不允许遍历子目录，仅允许修改一层直接目录
                        directories = Directory.GetDirectories(workPath);
                    }
                    else
                    {
                        //允许遍历子目录（同时不允许目录修改）
                        directories = Directory.GetDirectories(workPath, "*", SearchOption.AllDirectories);
                    }
                    foreach (var item in directories) //处理每个遍历到的目录
                    {
                        if (WorkStop) { break; }
                        if (checkBox_emptyDirIgnore.Checked && MMCore.IsDirectoryEmpty(item))
                        {
                            continue; //勾选了忽略空目录，但部分子目录为空亦跳过
                        }

                        if (checkBox_nonEmptyDirIgnore.Checked && !MMCore.IsDirectoryEmptyUserDef(item))
                        {
                            continue; //勾选了忽略非空目录（只输出空目录），那么非空目录将被跳过
                        }
                        //通配符错误时重置
                        if (textBox_param3.Text == "*") { }
                        else if (string.Compare(Path.GetExtension(item), textBox_param3.Text, checkBox_param3.Checked) != 0) { continue; }
                        if (checkBox_specialStr.Checked && textBox_specialStr.Text != "")
                        {
                            if (checkBox_regular.Checked)
                            {
                                if (!Regex.IsMatch(item.Substring(item.LastIndexOf('\\') + 1), textBox_specialStr.Text)) { continue; }
                            }
                            else
                            {
                                //未启用正则情况下
                                if (checkBox_specialStrIgnoreCase.Checked)
                                {
                                    //忽略大小写
                                    //IndexOf函数对大小写不敏感，适用于不区分大小写的判断，返回值为int型（在sring中的索引值）
                                    if (item.Substring(item.LastIndexOf('\\') + 1).IndexOf(textBox_specialStr.Text, StringComparison.OrdinalIgnoreCase) == -1)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    //String.Contains对大小写敏感，适用于区分大小写的判断
                                    if (!item.Substring(item.LastIndexOf('\\') + 1).Contains(textBox_specialStr.Text))
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        if (checkBox_range.Checked)
                        {//启用范围大小功能时
                         //若未反选则在范围外的正常被过滤，若反选则范围内的被过滤
                            if ((checkBox_RangeReversal.Checked && ParamIsInRange(item)) ||
                                (!checkBox_RangeReversal.Checked && !ParamIsInRange(item)))
                            {
                                continue;
                            }
                        }
                        switch (comboBox_selectFunc.SelectedIndex)
                        {
                            case 0:
                                //如果是打印功能
                                if (checkBox_printDirPath.Checked)
                                {//勾选输出目录全路径
                                    dirName = item;
                                }
                                else
                                {
                                    dirName = item.Substring(item.LastIndexOf('\\') + 1);
                                }
                                if (checkBox_printDirSize.Checked)
                                {
                                    dirSize = MMCore.CountSize(MMCore.GetDirectoryLength(item), checkBox_byteCount.Checked);
                                }
                                if (checkBox_printDirTime.Checked)
                                {
                                    directoryInfo = new DirectoryInfo(item);
                                    dirTime = directoryInfo.LastWriteTime.ToString();
                                }
                                dirName = dirTime + " <DIR> " + dirSize + " " + dirName;
                                MMCore.WriteLine(workFilePath, dirName, true);
                                break;
                            default:
                                //修改目录动作（允许遍历子目录时，文件夹修改是被禁止的，所以不会执行）
                                if (checkBox_dirModification.Checked)
                                {
                                    DAction(workFilePath, item);
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 勾选目录与文件分列时的文件处理动作
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="workFilePath"></param>
        private void FileRun(string workPath, string workFilePath)
        {
            string fileName;
            string[] files;
            string fileSize = "";
            string fileTime = "";
            string newFileName;
            string newFilePath;
            DirectoryInfo directoryInfo;
            FileInfo[] fileInfos;
            if (checkBox_fileSearch.Checked) //允许检索文件
            {
                for (int i = 0; i < 1; i++)
                {
                    if (!WorkStatus) { break; }
                    if (!checkBox_recursion.Checked)
                    {
                        //（允许目录修改操作下）不允许遍历子目录，仅遍历当前目录文件
                        files = Directory.GetFiles(workPath);
                    }
                    else
                    {
                        //允许遍历子目录下所有文件
                        files = Directory.GetFiles(workPath, "*", SearchOption.AllDirectories);
                    }
                    if (checkBox_suffix.Checked) //未勾选输出文件后缀
                    {
                        foreach (var item in files) //处理每个遍历到的文件
                        {
                            if (WorkStop) { break; }
                            if (checkBox_emptyFileIgnore.Checked && MMCore.CountSize(MMCore.GetFileLength(item), checkBox_byteCount.Checked) == "0 Byte")
                            {
                                continue; //勾选了忽略空文件，遇空文件时跳过
                            }
                            if (checkBox_nonEmptyFileIgnore.Checked && !(MMCore.CountSize(MMCore.GetFileLength(item), checkBox_byteCount.Checked) == "0 Byte"))
                            {
                                continue; //勾选了忽略非空文件，遇非空文件时跳过
                            }
                            //通配符错误时重置
                            if (textBox_param3.Text == "*") { }
                            else if (
                                string.Compare(
                                    Path.GetExtension(item),
                                    textBox_param3.Text,
                                    checkBox_param3.Checked
                                ) != 0
                            )
                            {
                                continue;
                            }
                            if (checkBox_specialStr.Checked && textBox_specialStr.Text != "")
                            {
                                if (checkBox_regular.Checked)
                                {
                                    if (!Regex.IsMatch(item.Substring(item.LastIndexOf('\\') + 1), textBox_specialStr.Text)) { continue; }
                                }
                                else
                                {
                                    //未启用正则情况下
                                    if (checkBox_specialStrIgnoreCase.Checked)
                                    {
                                        //忽略大小写
                                        //IndexOf函数对大小写不敏感，适用于不区分大小写的判断，返回值为int型（在sring中的索引值）
                                        if (item.Substring(item.LastIndexOf('\\') + 1).IndexOf(textBox_specialStr.Text, StringComparison.OrdinalIgnoreCase) == -1)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //String.Contains对大小写敏感，适用于区分大小写的判断
                                        if (!item.Substring(item.LastIndexOf('\\') + 1).Contains(textBox_specialStr.Text))
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (checkBox_range.Checked)
                            {//启用范围大小功能时
                             //若未反选则在范围外的正常被过滤，若反选则范围内的被过滤
                                if ((checkBox_RangeReversal.Checked && ParamIsInRange(item)) ||
                                    (!checkBox_RangeReversal.Checked && !ParamIsInRange(item)))
                                {
                                    continue;
                                }
                            }
                            switch (comboBox_selectFunc.SelectedIndex)
                            {
                                case 0:
                                    if (checkBox_printFilePath.Checked)
                                    {
                                        //勾选允许文件全路径
                                        fileName = item;
                                    }
                                    else
                                    {
                                        fileName = item.Substring(item.LastIndexOf('\\') + 1);
                                    }

                                    if (checkBox_printFileSize.Checked)
                                    {
                                        fileSize = MMCore.CountSize(MMCore.GetFileLength(item), checkBox_byteCount.Checked);
                                    }
                                    if (checkBox_printFileTime.Checked)
                                    {
                                        FileInfo fileInfo = new FileInfo(item);
                                        fileTime = fileInfo.LastWriteTime.ToString();
                                    }
                                    fileName = fileTime + " " + fileSize + " " + fileName;
                                    MMCore.WriteLine(workFilePath, fileName, true);
                                    break;
                                default:
                                    //修改目录动作
                                    if (checkBox_fileModification.Checked)
                                    {
                                        DAction(workFilePath, item);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        directoryInfo = new DirectoryInfo(workPath);
                        fileInfos = directoryInfo.GetFiles();
                        foreach (FileInfo fileInfo in fileInfos)
                        {
                            if (WorkStop) { break; }
                            if (
                                checkBox_emptyFileIgnore.Checked
                                && MMCore.CountSize(MMCore.GetFileLength(fileInfo.Name), checkBox_byteCount.Checked) == "0 Byte"
                            )
                            {
                                continue; //勾选了忽略空文件，遇空文件时跳过
                            }
                            if (
                                checkBox_nonEmptyFileIgnore.Checked
                                && !(MMCore.CountSize(MMCore.GetFileLength(fileInfo.Name), checkBox_byteCount.Checked) == "0 Byte")
                            )
                            {
                                continue; //勾选了忽略非空文件，遇非空文件时跳过
                            }
                            //通配符错误时重置
                            if (textBox_param3.Text == "*") { }
                            else if (
                                string.Compare(
                                    Path.GetExtension(fileInfo.Name),
                                    textBox_param3.Text,
                                    checkBox_param3.Checked
                                ) != 0
                            )
                            {
                                continue;
                            }
                            if (checkBox_specialStr.Checked && textBox_specialStr.Text != "")
                            {
                                if (checkBox_regular.Checked)
                                {
                                    if (!Regex.IsMatch(fileInfo.Name.Substring(fileInfo.Name.LastIndexOf('\\') + 1), textBox_specialStr.Text)) { continue; }
                                    //↓可用AsSpan替代Substring来提升性能
                                    //if (!Regex.IsMatch(fileInfo.Name.AsSpan().Slice(fileInfo.Name.LastIndexOf('\\') + 1).ToString(), textBox_specialStr.Text))
                                    //{
                                    //    continue;
                                    //}
                                }
                                else
                                {
                                    //未启用正则情况下
                                    if (checkBox_specialStrIgnoreCase.Checked)
                                    {
                                        //忽略大小写
                                        //IndexOf函数对大小写不敏感，适用于不区分大小写的判断，返回值为int型（在sring中的索引值）
                                        if (fileInfo.Name.Substring(fileInfo.Name.LastIndexOf('\\') + 1).IndexOf(textBox_specialStr.Text, StringComparison.OrdinalIgnoreCase) == -1)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //String.Contains对大小写敏感，适用于区分大小写的判断
                                        if (!fileInfo.Name.Substring(fileInfo.Name.LastIndexOf('\\') + 1).Contains(textBox_specialStr.Text))
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (checkBox_range.Checked && !ParamIsInRange(fileInfo.Name))
                            {
                                continue;
                            }
                            switch (comboBox_selectFunc.SelectedIndex)
                            {
                                case 0:
                                    if (checkBox_printFilePath.Checked)
                                    {
                                        //勾选允许文件全路径
                                        newFilePath = Path.Combine(
                                            fileInfo.DirectoryName,
                                            Path.GetFileNameWithoutExtension(fileInfo.Name)
                                        );
                                        if (checkBox_printFileSize.Checked)
                                        {
                                            fileSize = MMCore.CountSize(MMCore.GetFileLength(fileInfo.FullName), checkBox_byteCount.Checked);
                                        }
                                        if (checkBox_printFileTime.Checked)
                                        {
                                            fileTime = fileInfo.LastWriteTime.ToString();
                                        }
                                        newFilePath = fileTime + " " + fileSize + " " + newFilePath;
                                        MMCore.WriteLine(workFilePath, newFilePath, true);
                                    }
                                    else
                                    {
                                        newFileName = Path.GetFileNameWithoutExtension(
                                            fileInfo.Name
                                        );
                                        if (checkBox_printFileSize.Checked)
                                        {
                                            fileSize = MMCore.CountSize(MMCore.GetFileLength(fileInfo.FullName), checkBox_byteCount.Checked);
                                        }
                                        if (checkBox_printFileTime.Checked)
                                        {
                                            fileTime = fileInfo.LastWriteTime.ToString();
                                        }
                                        newFileName = fileTime + " " + fileSize + " " + newFileName;
                                        MMCore.WriteLine(workFilePath, newFileName, true);
                                    }
                                    break;
                                default:
                                    if (checkBox_fileModification.Checked)
                                    {
                                        DAction(workFilePath, fileInfo.FullName);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Work
        /// <summary>
        /// 窗口初始化
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            label_headTip.ForeColor = Color.Red;
            label_dirStatistics.ForeColor = Color.Red;
            label_controlTips.ForeColor = Color.Green;
            label_paramDescription3.Text = "检索后缀（如.txt）";
            if (comboBox_selectFunc.SelectedIndex == -1)
            {
                comboBox_selectFunc.SelectedIndex = 0;
            }
            if (comboBox_DirEmptyMode.SelectedIndex == -1)
            {
                comboBox_DirEmptyMode.SelectedIndex = 0;
            }
            if (comboBox_param8.SelectedIndex == -1)
            {
                comboBox_param8.SelectedIndex = 2;
            }
            if (comboBox_SelectWorkFilePath.SelectedIndex == -1)
            {
                comboBox_SelectWorkFilePath.SelectedIndex = 0;
            }
            if (comboBox_SamePathHandle.SelectedIndex == -1)
            {
                comboBox_SamePathHandle.SelectedIndex = 2;
            }
            label_controlTips.Text = string.Empty;
            if (!checkBox_range.Checked) { panel8.Visible = false; }
            else { panel8.Visible = true; }
            checkBox_batMethod.Visible = false;
            checkBox_traversalRecursive.Visible = false;
            button_simRun.Text = "模拟=OFF";
            button_simRun.ForeColor = Color.Red;
            button_simRun.Visible = false;
            //↓功能未完成暂先隐藏
            button_restore.Visible = false;
            checkBox_runValidation.Visible = false;

        }

        /// <summary>
        /// 选择工作目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_selectWorkPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowDialog();
                string path = fbd.SelectedPath;
                textBox_workPath.Text = path;
            }
        }

        /// <summary>
        /// 选择工作文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_selectWorkFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (checkBox_printTXTOnly.Checked) { ofd.Filter = "Text files (*.txt)|*.txt"; }
                else { ofd.Filter = "All Files (*.*)|*.*"; }

                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    textBox_workFilePath.Text = path;
                }
                else if (result == DialogResult.Cancel)
                {
                    //MessageBox.Show("用户取消了文件选择！");
                }
            }
        }

        /// <summary>
        /// 点击执行所选功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_run_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 1; i++)
            {
                if (MMCore.IsDFPath(textBox_workPath.Text))
                {
                    UserOpEnableChange(false);
                }
                else
                {
                    label_dirStatistics.Text = "工作目录错误！";
                    break;
                }
                if (button_run.Text == "执行" && WorkStatus == false)
                {
                    WorkStatus = true;
                    button_run.Text = "取消";
                    WorkThread = new Thread(ButtonRun) { IsBackground = true };
                    WorkThread.Start();
                }
                else if (button_run.Text == "取消" && WorkStatus == true)
                {
                    WorkStop = true;
                }
            }
        }

        /// <summary>
        /// 点击模拟执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_simRun_Click(object sender, EventArgs e)
        {
            if (simRunState)
            {
                simRunState = false;
                button_simRun.Text = "模拟=OFF";
                button_simRun.ForeColor = Color.Red;
            }
            else
            {
                simRunState = true;
                button_simRun.Text = "模拟=ON";
                button_simRun.ForeColor = Color.Green;
            }
        }

        /// <summary>
        /// 下拉选择功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox_protectSuffix.Checked)
            {
                textBox_param4.Text = "";
            } //勾选时，设置参数4为空（即保护任意后缀）
            else
            {
                textBox_param4.Text = "*";
            }
            switch (comboBox_selectFunc.SelectedIndex)
            {
                case 0:
                    label_headTip.Text = "批量将文件（夹）名称打印到工作文件";
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel5.Visible = false;
                    panel9.Visible = false;
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = false;
                    checkBox_fileModification.Visible = false;
                    CheckBoxVisible(true);
                    break;
                case 1:
                    label_headTip.Text = "批量将文件（夹）名称去除固定位数字符（填写要移除的起始位和字符数，字符起始位从0起算）";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "移除字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel9.Visible = true;
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 2:
                    label_headTip.Text = "批量将文件（夹）名称保留固定位数字符（填写要保留的起始位和字符数，字符起始位从0起算）";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "保留字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel9.Visible = true;
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 3:
                    label_headTip.Text = "批量插入字符到文件（夹）名称固定位数字符前（填写固定字符起始位和字符数，字符起始位从0起算）";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "固定字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel9.Visible = true;
                    panel5.Visible = true; //参数5面板
                    label16.Text = "插入字符"; //参数5说明
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 4:
                    label_headTip.Text = "批量插入字符到文件（夹）名称固定位数字符后（填写固定字符起始位和字符数，字符起始位从0起算）";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "固定字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel9.Visible = true;
                    panel5.Visible = true; //参数5面板
                    label16.Text = "插入字符"; //参数5说明
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 5:
                    label_headTip.Text = "批量插入字符到文件（夹）名称最前（勾选保留后缀则只修改前缀名称）";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "固定字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel9.Visible = true;
                    panel5.Visible = true; //参数5面板
                    label16.Text = "插入字符"; //参数5说明
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 6:
                    label_headTip.Text = "批量插入字符到文件（夹）名称最后（注：保护后缀要将参数4留空）";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "保留字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel9.Visible = true;
                    panel5.Visible = true; //参数5面板
                    label16.Text = "插入字符"; //参数5说明
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 7:
                    label_headTip.Text = "批量替换文件（夹）名称固定位数字符";
                    label__paramDescription1.Text = "起始位（数字）";
                    label_paramDescription2.Text = "替换字符数";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel9.Visible = true;
                    panel5.Visible = true; //参数5面板
                    label16.Text = "替换后缀"; //参数5说明
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 8:
                    label_headTip.Text = "批量替换文件（夹）名称的指定字符";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel9.Visible = true;
                    panel5.Visible = true; //参数5面板
                    label16.Text = "原字符"; //参数5说明
                    panel6.Visible = true; //参数6面板
                    label18.Text = "替换字符"; //参数6说明
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                case 9:
                    label_headTip.Text = "批量替换文件（夹）名称的后缀字符";
                    label1_paramDescription4.Text = "保护后缀";
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel9.Visible = true;//保护后缀
                    panel5.Visible = true; //参数5面板
                    label16.Text = "替换后缀"; //参数5说明
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    if (checkBox_protectSuffix.Checked)
                    {
                        checkBox_protectSuffix.Checked = false;
                    }
                    break;
                case 10:
                    label_headTip.Text = "批量删除（移动/复制）指定名称的文件（夹），选此功能时，参数5支持正则表达式";
                    label1_paramDescription4.Text = "保护该后缀文件不被删或修改";
                    checkBox_protectSuffix.Checked = false;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel9.Visible = true;//保护后缀
                    panel5.Visible = false; //参数5面板
                    panel6.Visible = false; //参数6面板
                    label_param8.Text = "移至"; //参数8说明
                    panel10.Visible = true; //参数8面板
                    checkBox_specialStr.Checked = true;//选择文件移动删除功能时默认开启正则
                    checkBox_dirModification.Visible = true;
                    checkBox_fileModification.Visible = true;
                    CheckBoxVisible(false);
                    break;
                default:
                    label_headTip.Text = "功能未选择！";
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel5.Visible = false;
                    panel9.Visible = false;
                    panel6.Visible = false; //参数6面板
                    panel10.Visible = false; //参数8面板
                    checkBox_dirModification.Visible = false;
                    checkBox_fileModification.Visible = false;
                    button_simRun.Visible = false;
                    checkBox_suffix.Visible = false;
                    checkBox_printDiskSymbol.Visible = false;
                    checkBox_printDirPath.Visible = false;
                    checkBox_printFileSize.Visible = false;
                    checkBox_printFilePath.Visible = false;
                    checkBox_batMethod.Visible = false;
                    checkBox_printDiskName.Visible = false;
                    checkBox_fileStatistics.Visible = false;
                    checkBox_DirCount.Visible = false;
                    checkBox_printDirSize.Visible = false;
                    checkBox_printFileTime.Visible = false;
                    checkBox_printDirTime.Visible = false;
                    checkBox_byteCount.Visible = false;
                    break;
            }
        }

        private void CheckBoxVisible(bool torf)
        {
            button_simRun.Visible = !torf;
            checkBox_suffix.Visible = torf;
            checkBox_printDiskSymbol.Visible = torf;
            checkBox_printDirPath.Visible = torf;
            checkBox_printFileSize.Visible = torf;
            checkBox_printFilePath.Visible = torf;
            checkBox_batMethod.Visible = torf;
            checkBox_printDiskName.Visible = torf;
            checkBox_fileStatistics.Visible = torf;
            checkBox_DirCount.Visible = torf;
            checkBox_printDirSize.Visible = torf;
            checkBox_printFileTime.Visible = torf;
            checkBox_printDirTime.Visible = torf;
            checkBox_byteCount.Visible = torf;
        }

        /// <summary>
        /// 工作文本变化时检查并修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox_workFilePath.Text == "")
            {
                textBox_workFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "temp.txt";
            }
            else if (
                checkBox_printTXTOnly.Checked
                && (
                    !textBox_workFilePath.Text.Contains(@".txt")
                    || !Regex.IsMatch(textBox_workFilePath.Text, @"^(.*)(\.txt)$")
                )
            )
            {
                textBox_workFilePath.Text += @".txt";
            }
        }

        /// <summary>
        /// 功能1_对删除字符起始位置进行检查并提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (!MMCore.IsNumeric(textBox_param1.Text))
            {
                textBox_param1.Text = "请重填！";
            }
            else
            {
                StartIndex = Convert.ToInt32(textBox_param1.Text);
                if (StartIndex < 0)
                {
                    textBox_param1.Text = "请重填！";
                }
            }
        }

        /// <summary>
        /// 功能1_对删除字符数检查并提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!MMCore.IsNumeric(textBox_param2.Text))
            {
                textBox_param2.Text = "请重填！";
            }
            else
            {
                StrCount = Convert.ToInt32(textBox_param2.Text);
                if (StrCount < 0)
                {
                    textBox_param2.Text = "请重填！";
                }
            }
        }

        /// <summary>
        /// 对检索后缀功能检查并重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox_param3.Text == "")
            {
                textBox_param3.Text = "*";
            }
        }

        /// <summary>
        /// 用户快捷选择工作文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string temp;
            switch (comboBox_SelectWorkFilePath.SelectedIndex)
            {
                case 0: //文本>用户自定
                    break;
                case 1: //文本>程序目录
                    textBox_workFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + @"temp.txt";
                    label_controlTips.Text = "工作文本：" + textBox_workFilePath.Text;
                    break;
                case 2: //文本>工作目录（外）
                    textBox_workFilePath.Text = textBox_workPath.Text;
                    if (textBox_workFilePath.Text == "")
                    {
                        textBox_workFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "temp.txt";
                    }
                    else if (checkBox_printTXTOnly.Checked && (!textBox_workFilePath.Text.Contains(@".txt") || !Regex.IsMatch(textBox_workFilePath.Text, @"^(.*)(\.txt)$")))
                    {
                        textBox_workFilePath.Text += @".txt";
                    }
                    label_controlTips.Text = "工作文本：" + textBox_workFilePath.Text;
                    break;
                case 3: //文本>工作目录（内）
                    textBox_workFilePath.Text = textBox_workPath.Text;
                    if (textBox_workFilePath.Text == "")
                    {
                        textBox_workFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "temp.txt";
                    }
                    else if (checkBox_printTXTOnly.Checked && (!textBox_workFilePath.Text.Contains(@".txt") || !Regex.IsMatch(textBox_workFilePath.Text, @"^(.*)(\.txt)$")))
                    {
                        temp = textBox_workFilePath.Text.Substring(textBox_workFilePath.Text.LastIndexOf('\\') + 1);
                        textBox_workFilePath.Text += @"\" + temp + @".txt";
                    }
                    label_controlTips.Text = "工作文本：" + textBox_workFilePath.Text;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 可点击的标签框提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label11_Click(object sender, EventArgs e)
        {
            if (WorkStatus)
            {
                label_dirStatistics.Text = ProCount.ToString() + @"/" + ProCountMax.ToString();
                if (label_dirStatistics.Text == @"0/0")
                {
                    label_dirStatistics.Text = "计算中...";
                }
            }
        }

        /// <summary>
        /// 取消线程间的安全访问
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //取消线程间的安全访问（最简单省事，但最不负责）
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 文件（夹）检索大小（Min）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!MMCore.IsNumeric(textBox_rangeMin.Text))
            {
                textBox_rangeMin.Text = "文件（夹）检索大小（Min）";
            }
        }

        /// <summary>
        /// 文件（夹）检索大小（Max）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (!MMCore.IsNumeric(textBox_rangeMax.Text))
            {
                textBox_rangeMax.Text = "文件（夹）检索大小（Max）";
            }
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_protectSuffix.Checked)
            {
                textBox_param4.Text = "";
            } //勾选时，设置参数4为空（即保护任意后缀）
            else
            {
                textBox_param4.Text = "*";
            }
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_printDirSize.Checked)
            {
                label_dirStatistics.Text = "你选择了目录递归统计大小，目前非常耗时哦！";
            }
            else
            {
                label_dirStatistics.Text = "";
            }
        }

        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_dirModification.Checked)
            {//允许目录修改
                if (!checkBox_traversalRecursive.Checked)
                {//没有勾选递归遍历时执行安全检查
                    checkBox_recursion.Checked = false;
                    label_dirStatistics.Text = "[安全操作]未勾选递归遍历，修改文件夹时不支持目录遍历！";
                }
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_recursion.Checked)
            {
                if (!checkBox_traversalRecursive.Visible) { checkBox_traversalRecursive.Visible = true; }
                else
                {
                    //分列与文件夹优先无效了
                    checkBox_DFDividually.Checked = false;
                    checkBox_dirFirst.Checked = false;
                    checkBox_DFDividually.Visible = false;
                    checkBox_dirFirst.Visible = false;
                }
                if (checkBox_dirModification.Checked)
                {
                    if (!checkBox_traversalRecursive.Checked)
                    {
                        checkBox_dirModification.Checked = false;
                        label_dirStatistics.Text = "[安全操作]未勾选递归遍历，遍历子目录时不支持目录修改！";
                    }
                }
                else
                {

                    if (comboBox_selectFunc.SelectedIndex == 0) { checkBox_batMethod.Visible = true; label_dirStatistics.Text = "遍历子目录很耗时！若只想打印文件（夹）名称，可勾选BAT方式！"; }
                    else
                    {
                        checkBox_batMethod.Checked = false;
                        checkBox_batMethod.Visible = false; label_dirStatistics.Text = "遍历子目录会增加耗时！";
                    }
                }
            }
            else
            {
                if (!checkBox_DFDividually.Visible) { checkBox_DFDividually.Visible = true; }
                if (!checkBox_dirFirst.Visible) { checkBox_dirFirst.Visible = true; }
                if (checkBox_traversalRecursive.Checked) { checkBox_traversalRecursive.Checked = false; }
                if (checkBox_traversalRecursive.Visible) { checkBox_traversalRecursive.Visible = false; }
                label_dirStatistics.Text = "";
                checkBox_batMethod.Checked = false;
                checkBox_batMethod.Visible = false;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e) { }

        private void textBox10_TextChanged(object sender, EventArgs e) { }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (textBox_param4.Text == "*")
            {
                checkBox_protectSuffix.Checked = false;
            }
            else if (textBox_param4.Text == "")
            {
                checkBox_protectSuffix.Checked = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            string path = fbd.SelectedPath;
            textBox_param8.Text = path;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_param8.SelectedIndex)
            {
                case 0:
                    textBox_param8.Enabled = true;
                    button_param8.Enabled = true;
                    break;
                case 1:
                    textBox_param8.Enabled = true;
                    button_param8.Enabled = true;
                    break;
                case 2:
                    textBox_param8.Enabled = false;
                    button_param8.Enabled = false;
                    break;
                case 3:
                    textBox_param8.Enabled = false;
                    button_param8.Enabled = false;
                    break;
                default:
                    break;

            }
        }
        private void checkBox36_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_regular.Checked) { checkBox_specialStrIgnoreCase.Visible = false; } else { checkBox_specialStrIgnoreCase.Visible = true; }
        }

        private void checkBox38_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_specialStr.Checked) { panel11.Visible = true; } else { panel11.Visible = false; }
        }

        private void checkBox_range_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox_range.Checked) { panel8.Visible = false; }
            else { panel8.Visible = true; label_controlTips.Text = "启用文件大小范围过滤来辅助筛选"; }
        }

        #endregion

        private void checkBox_traversalRecursive_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox_traversalRecursive.Checked)
            {
                if (checkBox_recursion.Checked)
                {
                    checkBox_recursion.Checked = false;
                }
                //if (checkBox_dirModification.Checked)
                //{
                //   checkBox_dirModification.Checked = false;
                //}
                if (!checkBox_DFDividually.Visible) { checkBox_DFDividually.Visible = true; }
                if (!checkBox_dirFirst.Visible) { checkBox_dirFirst.Visible = true; }
            }
            else
            {
                if (checkBox_traversalRecursive.Checked && checkBox_recursion.Checked)
                {
                    //分列与文件夹优先无效了
                    checkBox_DFDividually.Checked = false;
                    checkBox_dirFirst.Checked = false;
                    checkBox_DFDividually.Visible = false;
                    checkBox_dirFirst.Visible = false;
                }
            }
        }
    }
}
