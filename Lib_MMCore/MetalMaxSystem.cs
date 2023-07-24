#region 序言

//--------------------------------------------------------------------------------------------------
//MetalMaxSystem.FuncLib（MM_函数库）
//Code By Prinny（蔚蓝星海）
//Github：
//--------------------------------------------------------------------------------------------------
//功能及使用说明：
//
//函数参数中int player范围是0~16，其中1=用户本人，16=触发器（程序自身调用），其余均为玩家
//
//关于静态（Static）
//声明为静态的，内存数据副本只从模板创建1份，放在静态内存区（区别于实例数据动态增删的活动内存区，它是为了效率从逻辑上分类的而非物理分类），且创建后只在程序结束才会清理
//静态数据包括静态局部变量，创建唯一副本时只赋初值一次，后每次调用函数不再重新赋初值只保留上次调用结束时的值
//静态数据内存地址不变化（并非声明静态就一定给全局通用要考虑作用域声明，全局用命名空间类成员时需加前缀globol::）
//
//其余另详README.md
//--------------------------------------------------------------------------------------------------

#endregion

#region 引用空间

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Threading.Timer;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

#endregion

/// <summary>
/// MetalMax系统引用空间
/// </summary>
namespace MetalMaxSystem
{
    #region 枚举存放区

    /// <summary>
    /// 主副循环入口索引
    /// </summary>
    public enum Entry
    {
        MainAwake,
        MainStart,
        MainUpdate,
        MainEnd,
        MainDestroy,
        SubAwake,
        SubStart,
        SubUpdate,
        SubEnd,
        SubDestroy,
    }

    #endregion

    #region 结构存放区



    #endregion

    #region 委托类型存放区

    /// <summary>
    /// 声明键鼠事件函数引用（委托类型）
    /// </summary>
    /// <param name="ifKeyDown"></param>
    /// <param name="player"></param>
    public delegate void KeyMouseEventFuncref(bool ifKeyDown, int player);

    /// <summary>
    /// 声明主副循环入口事件函数引用（委托类型）
    /// </summary>
    public delegate void EntryEventFuncref();

    /// <summary>
    /// 声明计时器事件函数引用（委托类型）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TimerEventHandler(object sender, EventArgs e);

    #endregion

    #region 类存放区

    //公开静态类，数据在内存中唯一（从模板只产生一个可修改副本），其他程序集可调用。类的访问修饰符只有public和internal，internal修饰后只能在自身程序集（dll或exe）使用。前缀加partial（分部类型）则是定义要拆分到多个文件中的类
    //提供给程序集外或用户使用的函数全部标public，不让直接操作或需隐藏在内部使用则不标
    //若需额外让派生类（子类）可使用则标protected，会限制在基类派生类（父子类）中，但要注意其结合privite（修饰成员）、internal（修饰类）时反而是扩大使用范围
    //静态类只有1个活动副本，所有静态数据的副本创建后只有在程序结束才会清理

    /// <summary>
    /// 【MM_函数库】核心类
    /// </summary>
    public static class MMCore
    {
        #region 常量存放区

        public const int c_keyNone = -1;
        public const int c_keyShift = 0;
        public const int c_keyControl = 1;
        public const int c_keyAlt = 2;
        public const int c_key0 = 3;
        public const int c_key1 = 4;
        public const int c_key2 = 5;
        public const int c_key3 = 6;
        public const int c_key4 = 7;
        public const int c_key5 = 8;
        public const int c_key6 = 9;
        public const int c_key7 = 10;
        public const int c_key8 = 11;
        public const int c_key9 = 12;
        public const int c_keyA = 13;
        public const int c_keyB = 14;
        public const int c_keyC = 15;
        public const int c_keyD = 16;
        public const int c_keyE = 17;
        public const int c_keyF = 18;
        public const int c_keyG = 19;
        public const int c_keyH = 20;
        public const int c_keyI = 21;
        public const int c_keyJ = 22;
        public const int c_keyK = 23;
        public const int c_keyL = 24;
        public const int c_keyM = 25;
        public const int c_keyN = 26;
        public const int c_keyO = 27;
        public const int c_keyP = 28;
        public const int c_keyQ = 29;
        public const int c_keyR = 30;
        public const int c_keyS = 31;
        public const int c_keyT = 32;
        public const int c_keyU = 33;
        public const int c_keyV = 34;
        public const int c_keyW = 35;
        public const int c_keyX = 36;
        public const int c_keyY = 37;
        public const int c_keyZ = 38;
        public const int c_keySpace = 39;
        public const int c_keyGrave = 40;
        public const int c_keyNumPad0 = 41;
        public const int c_keyNumPad1 = 42;
        public const int c_keyNumPad2 = 43;
        public const int c_keyNumPad3 = 44;
        public const int c_keyNumPad4 = 45;
        public const int c_keyNumPad5 = 46;
        public const int c_keyNumPad6 = 47;
        public const int c_keyNumPad7 = 48;
        public const int c_keyNumPad8 = 49;
        public const int c_keyNumPad9 = 50;
        public const int c_keyNumPadPlus = 51;
        public const int c_keyNumPadMinus = 52;
        public const int c_keyNumPadMultiply = 53;
        public const int c_keyNumPadDivide = 54;
        public const int c_keyNumPadDecimal = 55;
        public const int c_keyEquals = 56;
        public const int c_keyMinus = 57;
        public const int c_keyBracketOpen = 58;
        public const int c_keyBracketClose = 59;
        public const int c_keyBackSlash = 60;
        public const int c_keySemiColon = 61;
        public const int c_keyApostrophe = 62;
        public const int c_keyComma = 63;
        public const int c_keyPeriod = 64;
        public const int c_keySlash = 65;
        public const int c_keyEscape = 66;
        public const int c_keyEnter = 67;
        public const int c_keyBackSpace = 68;
        public const int c_keyTab = 69;
        public const int c_keyLeft = 70;
        public const int c_keyUp = 71;
        public const int c_keyRight = 72;
        public const int c_keyDown = 73;
        public const int c_keyInsert = 74;
        public const int c_keyDelete = 75;
        public const int c_keyHome = 76;
        public const int c_keyEnd = 77;
        public const int c_keyPageUp = 78;
        public const int c_keyPageDown = 79;
        public const int c_keyCapsLock = 80;
        public const int c_keyNumLock = 81;
        public const int c_keyScrollLock = 82;
        public const int c_keyPause = 83;
        public const int c_keyPrintScreen = 84;
        public const int c_keyNextTrack = 85;
        public const int c_keyPrevTrack = 86;
        public const int c_keyF1 = 87;
        public const int c_keyF2 = 88;
        public const int c_keyF3 = 89;
        public const int c_keyF4 = 90;
        public const int c_keyF5 = 91;
        public const int c_keyF6 = 92;
        public const int c_keyF7 = 93;
        public const int c_keyF8 = 94;
        public const int c_keyF9 = 95;
        public const int c_keyF10 = 96;
        public const int c_keyF11 = 97;
        public const int c_keyF12 = 98;

        public const int c_mouseButtonNone = 0;
        public const int c_mouseButtonLeft = 1;
        public const int c_mouseButtonMiddle = 2;
        public const int c_mouseButtonRight = 3;
        public const int c_mouseButtonXButton1 = 4;
        public const int c_mouseButtonXButton2 = 5;

        public const int c_keyMax = 98;//键盘按键句柄数组下标上限（按键0~98，无按键-1）
        public const int c_regKeyMax = 8;//每个键盘按键可注册函数的数组下标上限
        public const int c_mouseMax = 5;//鼠标按键句柄数组下标上限（按键1~5，无按键0）
        public const int c_regMouseMax = 24;//每个鼠标按键可注册函数的数组下标上限

        private const int c_entryMax = 9;//主副循环入口句柄数组下标上限（0~9）
        private const int c_regEntryMax = 1;//每个主副循环入口可注册函数数组下标上限

        public const int c_playerAny = 16;//0默认中立玩家，1用户本人，2-14玩家（电脑或其他用户），15默认敌对玩家，16由系统触发，活动玩家=用户+电脑（不含中立）
        public const int c_maxPlayers = 16;//限制最大玩家数，0-15共16个

        #endregion

        #region 变量

        //虽然类只有字段没全局变量，但理论上公用静态变量才是该程序在内存中唯一的全局变量，无论类实例化多次或多线程从模板调用，它只生成一次副本。而非静态（实例）类每次实例化都复制一份模板去形成多个副本
        //私有实例变量相当于类/函数的局部变量，不标Static则类/函数结束时垃圾回收，标Static则副本唯一且只有程序集结束才从内存消失，所以静态局部变量在函数结束不参与垃圾回收，以便相同函数重复访问
        //静态数据是从模板形成的内存中唯一的可修改副本（不同类起相同名称其实是不一样的，要考虑命名空间和类名路径，无需担心重复）

        private static int[] keyEventFuncrefGroupNum = new int[c_keyMax + 1];//键盘按键已注册数量（每下标算1个，即使+=多个委托函数）
        private static int[] mouseEventFuncrefGroupNum = new int[c_mouseMax + 1];//鼠标按键的已注册数量（每下标算1个，即使+=多个委托函数）
        public static bool[] stopKeyMouseEvent = new bool[c_maxPlayers + 1];//停用用户的按键事件

        private static int[] entryEventFuncrefGroupNum = new int[c_entryMax + 1];//主副循环每个入口的已注册数量（每下标算1个，即使+=多个委托函数）
        public static bool[] stopEntryEvent = new bool[c_maxPlayers + 1];//停用用户主副循环事件

        private static Thread mainUpdateThread;//主循环线程
        private static Thread subUpdateThread;//副循环线程

        private static Hashtable systemDataTable = new Hashtable();//系统全局数据表（不排泄）
        private static Hashtable tempDataTable = new Hashtable();//临时局部数据表（设计过程中，函数结束时应手动排泄）

        //注：C#自带委托列表类型可用于存储这些委托类型变量↓

        //声明用于存放键盘、鼠标"按键事件引用类型"委托变量二维数组集合（单个元素也是集合能添加多个委托函数）
        private static KeyMouseEventFuncref[,] keyEventFuncrefGroup = new KeyMouseEventFuncref[c_keyMax + 1, c_regKeyMax + 1];
        private static KeyMouseEventFuncref[,] mouseEventFuncrefGroup = new KeyMouseEventFuncref[c_mouseMax + 1, c_regMouseMax + 1];

        //声明用于存放"主副循环入口事件引用类型"委托变量二维数组集合
        private static EntryEventFuncref[,] entryEventFuncrefGroup = new EntryEventFuncref[c_entryMax + 1, c_regEntryMax + 1];

        #endregion

        #region 字段及其属性方法

        //字段及其属性方法（private保护和隐藏字段防止用户直接修改引发错误，设计成只允许通过public修饰的属性方法间接去安全修改）
        //静态全局字段会形成内存中唯一的可修改副本，静态属性是提供用户给字段赋值并保护字段不出错的方法
        //在使用中，所有字段的读写通过属性来安全操作

        private static int _directoryEmptyUserDefIndex = 0;
        /// <summary>
        /// 用户定义空文件夹形式，0是子文件（夹）数量为0，1是文件夹大小为0，2是前两者必须都符合
        /// </summary>
        public static int DirectoryEmptyUserDefIndex { get => _directoryEmptyUserDefIndex; set => _directoryEmptyUserDefIndex = value; }

        private static AutoResetEvent _autoResetEvent_MainUpdate;//声明一个主循环自动复位事件对象（用来控制主循环线程信号）
        public static AutoResetEvent AutoResetEvent_MainUpdate { get => _autoResetEvent_MainUpdate; }

        private static AutoResetEvent _autoResetEvent_SubUpdate;//声明一个副循环自动复位事件对象（用来控制副循环线程信号）
        public static AutoResetEvent AutoResetEvent_SubUpdate { get => _autoResetEvent_SubUpdate; }

        private static Timer _mainUpdateTimer, _subUpdateTimer;
        public static Timer MainUpdateTimer { get => _mainUpdateTimer; }
        public static Timer SubUpdateTimer { get => _subUpdateTimer; }

        private static int _mainUpdateDuetime, _mainUpdatePeriod, _subUpdateDuetime, _subUpdatePeriod;
        public static int MainUpdateDuetime { get => _mainUpdateDuetime; set => _mainUpdateDuetime = value; }
        public static int MainUpdatePeriod { get => _mainUpdatePeriod; set => _mainUpdatePeriod = value; }
        public static int SubUpdateDuetime { get => _subUpdateDuetime; set => _subUpdateDuetime = value; }
        public static int SubUpdatePeriod { get => _subUpdatePeriod; set => _subUpdatePeriod = value; }

        //地图相关↓

        private static double _mapHeight;
        private static double[,] _terrainHeight = new double[2560 + 1, 2560 + 1];
        private static double[,,] _terrainThickness;

        /// <summary>
        /// 地图首个纹理图层顶面高度，默认值=8（m），称地面高度或地图高度均可
        /// </summary>
        public static double MapHeight { get => _mapHeight; set => _mapHeight = value; }

        /// <summary>
        /// 地面上附加的悬崖、地形物件的高度，二维坐标数组元素[2560+1,2560+1]（设计精度0.1m，按256m计）
        /// </summary>
        public static double[,] TerrainHeight { get => _terrainHeight; set => _terrainHeight = value; }

        /// <summary>
        /// 地层厚度，数组元素[2560+1,2560+1,16+1]，前2个数组纬度表示平面坐标（设计精度0.1m，按256m计），最后1个数组纬度表示地面高度往下的地层数（0为地图面层）
        /// </summary>
        public static double[,,] TerrainThickness { get => _terrainThickness; set => _terrainThickness = value; }

        #endregion

        #region Functions 数学公式区

        public static Vector ToVector(Vector3D vector)
        {
            return new Vector(vector.X, vector.Y);
        }

        public static double AngleBetween(double x, double y, double a, double b)
        {
            return Vector.AngleBetween(new Vector(x, y), new Vector(a, b));
        }

        public static double AngleBetween(double x, double y, double z, double a, double b, double c)
        {
            return Vector3D.AngleBetween(new Vector3D(x, y, z), new Vector3D(a, b, z));
        }

        public static double AngleBetween(PointF point1, PointF point2)
        {

            return Vector.AngleBetween(new Vector(point1.X, point1.Y), new Vector(point2.X, point2.Y));
        }

        public static double AngleBetween(Point3D point1, Point3D point2)
        {
            return Vector3D.AngleBetween(new Vector3D(point1.X, point1.Y, point1.Z), new Vector3D(point2.X, point2.Y, point2.Z));
        }

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            return Vector.AngleBetween(vector1, vector2);
        }

        public static double AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            return Vector3D.AngleBetween(vector1, vector2);
        }

        public static double Distance(double x, double y, double a, double b)
        {
            double x1 = x;
            double y1 = y;

            double x2 = a;
            double y2 = b;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
            return result;
        }

        public static double Distance(PointF point1, PointF point2)
        {
            double x1 = point1.X;
            double y1 = point1.Y;

            double x2 = point2.X;
            double y2 = point2.Y;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
            return result;
        }

        public static double Distance(Vector vector1, Vector vector2)
        {
            double x1 = vector1.X;
            double y1 = vector1.Y;

            double x2 = vector2.X;
            double y2 = vector2.Y;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
            return result;
        }

        public static double Distance(double x, double y, double z, double a, double b, double c)
        {
            double x1 = x;
            double y1 = y;
            double z1 = z;

            double x2 = a;
            double y2 = b;
            double z2 = c;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2) + Math.Pow((z1 - z2), 2));
            return result;
        }

        public static double Distance(Point3D point1, Point3D point2)
        {
            double x1 = point1.X;
            double y1 = point1.Y;
            double z1 = point1.Z;

            double x2 = point2.X;
            double y2 = point2.Y;
            double z2 = point2.Z;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2) + Math.Pow((z1 - z2), 2));
            return result;
        }

        public static double Distance(Vector3D vector1, Vector3D vector2)
        {
            double x1 = vector1.X;
            double y1 = vector1.Y;
            double z1 = vector1.Z;

            double x2 = vector2.X;
            double y2 = vector2.Y;
            double z2 = vector2.Z;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2) + Math.Pow((z1 - z2), 2));
            return result;
        }

        #endregion

        #region Functions 通用功能区

        /// <summary>
        /// 递归强制删除文件夹（进最里层删除文件使文件夹为空后删除这个空文件夹，层层递出时重复动作），删除前会去掉文件（夹）的Archive、ReadOnly、Hidden属性以确保删除
        /// </summary>
        /// <param name="dirInfo"></param>
        public static void DelDirectoryRecursively(DirectoryInfo dirInfo)
        {
            foreach (DirectoryInfo newInfo in dirInfo.GetDirectories())
            {
                DelDirectoryRecursively(newInfo);//递归遍历子文件夹
            }
            foreach (FileInfo newInfo in dirInfo.GetFiles())
            {
                //处理每个文件夹内部的文件（从里层开始删除）
                newInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                newInfo.Delete();
            }
            //对每个文件夹处理（从里层开始删除）
            dirInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
            dirInfo.Delete(true);
        }

        /// <summary>
        /// 递归强制删除文件夹（进最里层删除文件使文件夹为空后删除这个空文件夹，层层递出时重复动作），删除前会去掉文件（夹）的Archive、ReadOnly、Hidden属性以确保删除
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DelDirectoryRecursively(string dirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            foreach (DirectoryInfo newInfo in dirInfo.GetDirectories())
            {
                DelDirectoryRecursively(newInfo);
            }
            foreach (FileInfo newInfo in dirInfo.GetFiles())
            {
                newInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                newInfo.Delete();
            }
            dirInfo.Attributes &= ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
            dirInfo.Delete(true);

        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <returns>删除成功返回真，否则返回假</returns>
        public static bool DelDirectory(DirectoryInfo dirInfo)
        {
            bool torf = false;
            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
                if (!dirInfo.Exists) { torf = true; }
            }
            return torf;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns>删除返回真，否则返回假</returns>
        public static bool DelDirectory(string dirPath)
        {
            bool torf = false;
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                dirInfo.Delete(true);
                if (!dirInfo.Exists) { torf = true; }
            }
            return torf;
        }

        /// <summary>
        /// 添加Shell API特性[DllImport("shell32.dll", CharSet = CharSet.Unicode)]到SHFileOperation静态函数，删除文件到回收站功能专用
        /// </summary>
        /// <param name="lpFileOp"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        /// <summary>
        /// 删除文件到回收站功能专用结构体，已添加特性[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public uint wFunc;
            public string pFrom;
            public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        /// <summary>
        /// 删除文件到回收站功能专用枚举，已添加特性[Flags]
        /// </summary>
        [Flags]
        public enum SHFileOperationFlags : ushort
        {
            /// <summary>
            /// 不出现错误确认或询问用户的对话框
            /// </summary>
            FOF_SILENT = 0x0004,
            /// <summary>
            /// 不出现任何对话框
            /// </summary>
            FOF_NOCONFIRMATION = 0x0010,
            /// <summary>
            /// 文件删除后可以放到回收站
            /// </summary>
            FOF_ALLOWUNDO = 0x0040,
            /// <summary>
            /// 不出现错误对话框
            /// </summary>
            FOF_NOERRORUI = 0x0400,
        }

        /// <summary>
        /// 删除文件到回收站
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="torf"></param>
        public static void DelFileToRecycleBin(string filePath, bool torf)
        {
            if (!File.Exists(filePath))
            {
                return;
            }
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT
            {
                wFunc = 0x003,//删除文件到回收站
                pFrom = filePath + '\0'//多个文件以 \0 分隔
            };
            if (!torf)
            {
                //不确认直接删除（通过或运算符集成准许撤销+不出现任何对话框）
                fileop.fFlags = (ushort)(SHFileOperationFlags.FOF_ALLOWUNDO | SHFileOperationFlags.FOF_NOCONFIRMATION);
            }
            else
            {
                //需要用户确认删除，文件操作属性清空
                fileop.fFlags = 0;
            }
            SHFileOperation(ref fileop);
        }

        /// <summary>
        /// 删除文件夹到回收站
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="torf">回收站删除提示</param>
        public static void DelDirectoryToRecycleBin(string dirPath, bool torf)
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT
            {
                wFunc = 0x003,//删除文件到回收站
                pFrom = dirPath + '\0'//多个文件以 \0 分隔
            };
            if (!torf)
            {
                //不确认直接删除（通过或运算符集成准许撤销+不出现任何对话框）
                fileop.fFlags = (ushort)(SHFileOperationFlags.FOF_ALLOWUNDO | SHFileOperationFlags.FOF_NOCONFIRMATION);
            }
            else
            {
                //需要用户确认删除，文件操作属性清空
                fileop.fFlags = 0;
            }
            SHFileOperation(ref fileop);
        }

        /// <summary>
        /// 输入long型Size，将字节大小转字符串Byte、KB、MB、GB、TB、PB、EB、ZB、YB、NB形式
        /// </summary>
        /// <param name="Size">字节大小</param>
        /// <param name="Byte">true强制输出字节单位</param>
        /// <returns></returns>
        public static string CountSize(long Size, bool Byte)
        {
            string strSize = "";
            if (Byte)
            {
                strSize = StrAddSymbol(Size.ToString(), 3, ",") + " Byte";
            }
            else
            {
                if (Size < 1024.00)
                    strSize = Size.ToString() + " Byte";
                else if (Size >= 1024.00 && Size < Math.Pow(1024, 2))
                    strSize = (Size / 1024.00).ToString("F2") + " KB";
                else if (Size >= Math.Pow(1024, 2) && Size < Math.Pow(1024, 3))
                    strSize = (Size / Math.Pow(1024, 2)).ToString("F2") + " MB";
                else if (Size >= Math.Pow(1024, 3) && Size < Math.Pow(1024, 4))
                    strSize = (Size / Math.Pow(1024, 3)).ToString("F2") + " GB";
                else if (Size >= Math.Pow(1024, 4) && Size < Math.Pow(1024, 5))
                    strSize = (Size / Math.Pow(1024, 4)).ToString("F2") + " TB";
                else if (Size >= Math.Pow(1024, 5) && Size < Math.Pow(1024, 6))
                    strSize = (Size / Math.Pow(1024, 5)).ToString("F2") + " PB";
                else if (Size >= Math.Pow(1024, 6) && Size < Math.Pow(1024, 7))
                    strSize = (Size / Math.Pow(1024, 6)).ToString("F2") + " EB";
                else if (Size >= Math.Pow(1024, 7) && Size < Math.Pow(1024, 8))
                    strSize = (Size / Math.Pow(1024, 7)).ToString("F2") + " ZB";
                else if (Size >= Math.Pow(1024, 8) && Size < Math.Pow(1024, 9))
                    strSize = (Size / Math.Pow(1024, 8)).ToString("F2") + " YB";
                else if (Size >= Math.Pow(1024, 9) && Size < Math.Pow(1024, 10))
                    strSize = (Size / Math.Pow(1024, 9)).ToString("F2") + " NB";
                else if (Size >= Math.Pow(1024, 10))
                    strSize = (Size / Math.Pow(1024, 10)).ToString("F2") + " DB";
            }
            return strSize;
        }

        /// <summary>
        /// 输入double型Size，将字节大小转字符串Byte、KB、MB、GB、TB、PB、EB、ZB、YB、NB形式
        /// </summary>
        /// <param name="Size">字节大小</param>
        /// <param name="Byte">true强制输出字节单位</param>
        /// <returns></returns>
        public static string CountSize(double Size, bool Byte)
        {
            string strSize = "";
            if (Byte)
            {
                strSize = StrAddSymbol(Size.ToString(), 3, ",") + " Byte";
            }
            else
            {
                if (Size < 1024.00)
                    strSize = Size.ToString() + " Byte";
                else if (Size >= 1024.00 && Size < Math.Pow(1024, 2))
                    strSize = (Size / 1024.00).ToString("F2") + " KB";
                else if (Size >= Math.Pow(1024, 2) && Size < Math.Pow(1024, 3))
                    strSize = (Size / Math.Pow(1024, 2)).ToString("F2") + " MB";
                else if (Size >= Math.Pow(1024, 3) && Size < Math.Pow(1024, 4))
                    strSize = (Size / Math.Pow(1024, 3)).ToString("F2") + " GB";
                else if (Size >= Math.Pow(1024, 4) && Size < Math.Pow(1024, 5))
                    strSize = (Size / Math.Pow(1024, 4)).ToString("F2") + " TB";
                else if (Size >= Math.Pow(1024, 5) && Size < Math.Pow(1024, 6))
                    strSize = (Size / Math.Pow(1024, 5)).ToString("F2") + " PB";
                else if (Size >= Math.Pow(1024, 6) && Size < Math.Pow(1024, 7))
                    strSize = (Size / Math.Pow(1024, 6)).ToString("F2") + " EB";
                else if (Size >= Math.Pow(1024, 7) && Size < Math.Pow(1024, 8))
                    strSize = (Size / Math.Pow(1024, 7)).ToString("F2") + " ZB";
                else if (Size >= Math.Pow(1024, 8) && Size < Math.Pow(1024, 9))
                    strSize = (Size / Math.Pow(1024, 8)).ToString("F2") + " YB";
                else if (Size >= Math.Pow(1024, 9) && Size < Math.Pow(1024, 10))
                    strSize = (Size / Math.Pow(1024, 9)).ToString("F2") + " NB";
                else if (Size >= Math.Pow(1024, 10))
                    strSize = (Size / Math.Pow(1024, 10)).ToString("F2") + " DB";
            }
            return strSize;
        }

        /// <summary>
        /// 为字符串str每隔every位添加symbol
        /// </summary>
        /// <param name="str"></param>
        /// <param name="every"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string StrAddSymbol(string str, int every, string symbol)
        {
            string n = "";
            for (int i = str.Length - 1, j = 1; i >= 0; i--)
            {
                n = str[i].ToString() + n;
                if (j > 0 && i > 0 && (j % every == 0))
                {
                    n = symbol + n;
                    j = 0;
                }
                j++;
            }
            return n;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static long GetFileLength(FileInfo fileInfo)
        {
            long len = 0;
            if (fileInfo.Exists)
            {
                len = fileInfo.Length;
            }
            return len;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filePath">文件名完整路径</param>
        /// <returns></returns>
        public static long GetFileLength(string filePath)
        {
            long len = 0;
            if (File.Exists(filePath))
            {
                len = new FileInfo(filePath).Length;
            }
            return len;
        }

        /// <summary>
        /// 获取文件夹大小，递归方法较耗时
        /// </summary>
        /// <param name="dirPath">文件夹完整路径</param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
            {
                return 0;
            }
            long len = 0;
            //定义一个DirectoryInfo对象
            DirectoryInfo di = new DirectoryInfo(dirPath);
            //通过GetFiles方法,获取di目录中的所有文件的大小，量越大越慢
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }
            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }

        /// <summary>
        /// 取得设备硬盘的卷序列号
        /// </summary>
        /// <param name="diskSymbol">盘符</param>
        /// <returns>成功返回卷序列号，失败返回"uHnIk"</returns>
        public static string GetHardDiskID(string diskSymbol)
        {
            try
            {
                string hdInfo = "";
                ManagementObject disk = new ManagementObject(
                    "win32_logicaldisk.deviceid=\"" + diskSymbol + ":\""
                );
                hdInfo = disk.Properties["VolumeSerialNumber"].Value.ToString();
                disk = null;
                return hdInfo.Trim();
            }
            catch
            {
                return "uHnIk";
            }
        }

        /// <summary>
        /// 验证字符串是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            Regex reg1 = new Regex(@"^[0-9]\d*$");
            return reg1.IsMatch(str);
        }

        /// <summary>
        /// 验证字符串是否为合法文件（夹）名称，可以是虚拟路径（本函数不验证其真实存在）
        /// </summary>
        /// <param name="path">文件（夹）路径全名</param>
        /// <returns></returns>
        public static bool IsDFPath(string path)
        {
            Regex regex = new Regex(
                @"^([a-zA-Z]:\\)([-\u4e00-\u9fa5\w\s.()~!@#$%^&()\[\]{}+=]+\\?)*$"
            );
            Match result = regex.Match(path);
            return result.Success;
        }

        /// <summary>
        /// 验证字符串路径的文件（夹）是否真实存在
        /// </summary>
        /// <param name="path">文件（夹）路径全名</param>
        /// <returns></returns>
        public static bool IsDF(string path)
        {
            bool torf = false;
            if (Directory.Exists(path) || File.Exists(path))
            {
                torf = true;
            }
            return torf;
        }

        /// <summary>
        /// 判断目标属性是否为真实文件夹
        /// </summary>
        /// <param name="path">文件夹路径全名</param>
        /// <returns></returns>
        public static bool IsDirAttributes(string path)
        {
            FileInfo fi = new FileInfo(path);
            if ((fi.Attributes & FileAttributes.Directory) != 0)
            {
                //ReadOnly = 1,
                //Hidden = 2,
                //System = 4,
                //Directory = 16,
                //Archive = 32,
                //Device = 64,
                //如果设置了ReadOnly和Directory，则FileAttributes等于16+1=17，二进制为00001001
                //如果没有设置目录位，则会得到零：
                //File.GetAttributes(source) = 00000001
                //  FileAttributes.Directory = 00001000 &
                //-------------------------------------
                //                             00000000
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证字符串路径的文件夹是否真实存在
        /// </summary>
        /// <param name="path">文件夹路径全名</param>
        /// <returns></returns>
        public static bool IsDir(string path)
        {
            bool torf = false;
            if (Directory.Exists(path))
            {
                torf = true;
            }
            return torf;
        }

        /// <summary>
        /// 验证字符串路径的文件是否真实存在
        /// </summary>
        /// <param name="path">文件路径全名</param>
        /// <returns></returns>
        public static bool IsFile(string path)
        {
            bool torf = false;
            if (File.Exists(path))
            {
                torf = true;
            }
            return torf;
        }

        /// <summary>
        /// 验证目录是否为空
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryEmpty(string path)
        {
            bool torf = false;
            DirectoryInfo dir = new DirectoryInfo(path);
            //为了效率，只要验证当前层就可以了
            if (dir.GetFiles().Length + dir.GetDirectories().Length == 0)
            {
                torf = true;
            }
            return torf;
        }

        /// <summary>
        /// 验证路径是否为用户定义的空文件夹，通过MMCore.DirectoryEmptyUserDefIndex属性可定义空文件夹形式
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryEmptyUserDef(string path)
        {
            bool torf = false;
            switch (DirectoryEmptyUserDefIndex) //定义空文件夹形式
            {
                case 0:
                    if (IsDirectoryEmpty(path))
                    {
                        torf = true;
                    } //里面的子文件夹和文件数量均为0
                    break;
                case 1:
                    if (GetDirectoryLength(path) == 0)
                    {
                        torf = true;
                    } //文件夹大小为0
                    break;
                case 2:
                    if (IsDirectoryEmpty(path) && GetDirectoryLength(path) == 0)
                    {
                        torf = true;
                    } //以上两者
                    break;
                default:
                    if (IsDirectoryEmpty(path))
                    {
                        torf = true;
                    } //里面的子文件夹和文件数量均为0
                    break;
            }
            return torf;
        }

        /// <summary>
        /// 写文本每行，文件若不存在则自动新建
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="torf">false是覆盖，true是追加文本</param>
        public static void WriteLine(string path, string value, bool torf)
        {
            using (StreamWriter sw = new StreamWriter(path, torf, Encoding.Unicode))
            {
                sw.Write(value);
            }

        }

        /// <summary>
        /// 验证文件大小是否在用户定义的[a,b]范围
        /// </summary>
        /// <param name="path"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsDFSizeInRange(string path, long a, long b)
        {
            bool torf = false;
            long x;
            for (int i = 0; i < 1; i++)
            {
                if (IsDir(path))
                {
                    x = GetDirectoryLength(path);
                }
                else if (IsFile(path))
                {
                    x = GetFileLength(path);
                }
                else { break; }
                if (x >= a && x <= b)
                {
                    torf = true;
                }
            }
            return torf;
        }

        /// <summary>
        /// 创建GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CreateGetHttpResponse(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.50";
            try
            {
                HttpWebResponse webresponse = request.GetResponse() as HttpWebResponse;
                using (Stream s = webresponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(s, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "requestFalse";
            }

        }

        /// <summary>
        /// 创建POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static string CreatePostHttpResponse(string url, IDictionary<string, string> parameters, string ContentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.50";
            request.ContentType = ContentType;
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                HttpWebResponse webresponse = request.GetResponse() as HttpWebResponse;
                using (Stream s = webresponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(s, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "requestFalse";
            }
        }

        /// <summary>
        /// 下载指定网站的指定节点内容到指定文件夹并保存为自定义文件名
        /// 使用范例：
        /// HtmlDocument doc = new();
        /// doc.LoadHtml(MMCore.CreateGetHttpResponse("https://ac.qq.com/Comic/ComicInfo/id/542330"));
        /// HtmlNode img = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[3]/div[1]/div[1]/div[1]/a/img");
        /// string imgUal = img.Attributes["src"].Value;
        /// MMCore.Download(imgUal, "123.jpg", @"C:\Users\Admin\Desktop\Download\", true);
        /// Console.WriteLine("下载完成！");
        /// </summary>
        /// <param name="url">浏览器网址</param>
        /// <param name="filename">自定义文件名</param>
        /// <param name="path">下载路径，如 @"C:\Users\Admin\Desktop\Download\"</param>
        /// <param name="cover">发生文件重复时覆盖</param>
        /// <returns></returns>
        public static bool Download(string url, string filename, string path, bool cover)
        {
            string tempPath = Path.Combine(Path.GetDirectoryName(path), "temp");//确定临时文件夹全名路径
            string filepath = Path.Combine(path, filename);//确定最终下载文件全名路径
            Directory.CreateDirectory(tempPath);  //创建临时文件夹
            string tempFile = tempPath + "\\" + filename + ".temp"; //确定临时下载文件全名路径
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);    //临时下载文件存在则删除
            }
            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                responseStream.Close();
                responseStream.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误:{0}", ex.Message);
                return false;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
            try
            {
                File.Move(tempFile, filepath);
            }
            catch
            {
                if (cover) { File.Delete(filepath); File.Move(tempFile, filepath); }
            }
            try
            {
                DelDirectory(tempPath);
            }
            catch
            {
                DelDirectoryRecursively(tempPath);
            }
            return true;
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false, string custom = null)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 创建文件夹，若已存在则什么也不干
        /// </summary>
        /// <param name="path"></param>
        public static void CreatDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                directory.Create();
            }
        }

        /// <summary>
        /// 创建文件，若已存在则什么也不干
        /// </summary>
        /// <param name="filepath"></param>
        public static void CreatFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                File.Create(filepath);
            }
        }

        /// <summary>
        /// 用WinRAR解压带密码的压缩包
        /// </summary>
        /// <param name="zipFilePath">压缩包路径</param>
        /// <param name="unZipPath">解压后文件夹的路径</param>
        /// <param name="password">压缩包密码</param>
        /// <returns></returns>
        public static bool UnZip(string zipFilePath, string unZipPath, string password)
        {
            if (!IsOwnWinRAR())
            {
                MessageBox.Show("本机并未安装WinRAR,请安装该压缩软件!", "温馨提示");
                return false;
            }

            Process Process1 = new Process();
            Process1.StartInfo.FileName = "WinRAR.exe";
            Process1.StartInfo.CreateNoWindow = true;
            Process1.StartInfo.Arguments = " x -p" + password + " " + zipFilePath + " " + unZipPath;
            Process1.Start();
            if (Process1.HasExited)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断系统上是否安装winrar
        /// </summary>
        /// <returns></returns>
        public static bool IsOwnWinRAR()
        {
            RegistryKey the_Reg =
                Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
            return !string.IsNullOrEmpty(the_Reg.GetValue("").ToString());

        }
        #endregion

        #region Functions 数据表功能区

        /// <summary>
        /// 内部函数，添加数据表键值对的原始动作（重复添加则覆盖）
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        private static void DataTableSet(bool place, string key, object val)
        {
            if (place)
            {
                if (systemDataTable.Contains(key)) { systemDataTable.Remove(key); }
                systemDataTable.Add(key, val);
            }
            else
            {
                if (tempDataTable.Contains(key)) { tempDataTable.Remove(key); }
                tempDataTable.Add(key, val);
            }
        }

        /// <summary>
        /// 判断数据表键是否存在
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DataTableKeyExists(bool place, string key)
        {
            if (place) { return systemDataTable.ContainsKey(key); }
            else { return tempDataTable.ContainsKey(key); }
        }

        /// <summary>
        /// 获取数据表键对应的值
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object DataTableGetValue(bool place, string key)
        {
            if (place) { return systemDataTable[key]; }
            else { return tempDataTable[key]; }
        }

        /// <summary>
        /// 从数据表中清除Key
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        public static void DataTableClear0(bool place, string key)
        {
            DataTableRemove(place, key);
        }

        /// <summary>
        /// 从数据表中清除Key[]，模拟1维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        public static void DataTableClear1(bool place, string key, int lp_1)
        {
            DataTableRemove(place, (key + "_" + lp_1.ToString()));
        }

        /// <summary>
        /// 从数据表中清除Key[,]，模拟2维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        public static void DataTableClear2(bool place, string key, int lp_1, int lp_2)
        {
            DataTableRemove(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString()));
        }

        /// <summary>
        /// 从数据表中清除Key[,,]，模拟3维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="lp_3"></param>
        public static void DataTableClear3(bool place, string key, int lp_1, int lp_2, int lp_3)
        {
            DataTableRemove(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString()));
        }

        /// <summary>
        /// 从数据表中清除Key[,,,]，模拟4维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="lp_3"></param>
        /// <param name="lp_4"></param>
        public static void DataTableClear4(bool place, string key, int lp_1, int lp_2, int lp_3, int lp_4)
        {
            DataTableRemove(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString() + "_" + lp_4.ToString()));
        }

        /// <summary>
        /// 移除数据表键值对
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        private static void DataTableRemove(bool place, string key)
        {
            if (place) { systemDataTable.Remove(key); }
            else { tempDataTable.Remove(key); }
        }

        /// <summary>
        /// 保存数据表键值对
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void DataTableSave0(bool place, string key, object val)
        {
            DataTableSet(place, key, val);
        }

        /// <summary>
        /// 保存数据表键值对，模拟1维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="val"></param>
        public static void DataTableSave1(bool place, string key, int lp_1, object val)
        {
            DataTableSet(place, (key + "_" + lp_1.ToString()), val);
        }

        /// <summary>
        /// 保存数据表键值对，模拟2维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="val"></param>
        public static void DataTableSave2(bool place, string key, int lp_1, int lp_2, object val)
        {
            DataTableSet(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString()), val);
        }

        /// <summary>
        /// 保存数据表键值对，模拟3维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="lp_3"></param>
        /// <param name="val"></param>
        public static void DataTableSave3(bool place, string key, int lp_1, int lp_2, int lp_3, object val)
        {
            DataTableSet(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString()), val);
        }

        /// <summary>
        /// 保存数据表键值对，模拟4维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="lp_3"></param>
        /// <param name="lp_4"></param>
        /// <param name="val"></param>
        public static void DataTableSave4(bool place, string key, int lp_1, int lp_2, int lp_3, int lp_4, object val)
        {
            DataTableSet(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString() + "_" + lp_4.ToString()), val);
        }

        /// <summary>
        /// 读取数据表键值对
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object DataTableLoad0(bool place, string key)
        {
            if ((DataTableKeyExists(place, key) == false))
            {
                return null;
            }
            return DataTableGetValue(place, key);
        }

        /// <summary>
        /// 读取数据表键值对，模拟1维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <returns></returns>
        public static object DataTableLoad1(bool place, string key, int lp_1)
        {
            if ((DataTableKeyExists(place, (key + "_" + lp_1.ToString())) == false))
            {
                return null;
            }
            return DataTableGetValue(place, (key + "_" + lp_1.ToString()));
        }

        /// <summary>
        /// 读取数据表键值对，模拟2维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <returns></returns>
        public static object DataTableLoad2(bool place, string key, int lp_1, int lp_2)
        {
            if ((DataTableKeyExists(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString())) == false))
            {
                return null;
            }
            return DataTableGetValue(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString()));
        }

        /// <summary>
        /// 读取数据表键值对，模拟3维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="lp_3"></param>
        /// <returns></returns>
        public static object DataTableLoad3(bool place, string key, int lp_1, int lp_2, int lp_3)
        {
            if ((DataTableKeyExists(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString())) == false))
            {
                return null;
            }
            return DataTableGetValue(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString()));
        }

        /// <summary>
        /// 读取数据表键值对，模拟4维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        /// <param name="lp_2"></param>
        /// <param name="lp_3"></param>
        /// <param name="lp_4"></param>
        /// <returns></returns>
        public static object DataTableLoad4(bool place, string key, int lp_1, int lp_2, int lp_3, int lp_4)
        {
            if ((DataTableKeyExists(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString() + "_" + lp_4.ToString())) == false))
            {
                return null;
            }
            return DataTableGetValue(place, (key + "_" + lp_1.ToString() + "_" + lp_2.ToString() + "_" + lp_3.ToString() + "_" + lp_4.ToString()));
        }

        /// <summary>
        /// 存储区容错处理函数，当数据表键值存在时执行线程等待。常用于多线程触发器频繁写值，如大量注册注销动作使存储区数据重排序的，因数据表正在使用需排队等待完成才给执行下一个。执行原理：将调用该函数的当前线程反复挂起50毫秒，直到动作要写入的存储区闲置。
        /// </summary>
        /// <param name="key"></param>
        public static void ThreadWait(string key)
        {
            while (DataTableLoad0(true, "MMCore_ThreadWait_" + key) != null)
            {
                if (DataTableLoad0(true, "MMCore_ThreadWait_" + key).ToString() == "1")
                {
                    Thread.Sleep(50); //将调用该函数的当前线程挂起
                }
            }
        }

        /// <summary>
        /// 存储区容错处理函数，当数据表键值存在时执行线程等待。常用于多线程触发器频繁写值，如大量注册注销动作使存储区数据重排序的，因数据表正在使用需排队等待完成才给执行下一个。执行原理：将调用该函数的当前线程反复挂起period毫秒，直到动作要写入的存储区闲置。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="period"></param>
        public static void ThreadWait(string key, int period)
        {
            while (DataTableLoad0(true, "MMCore_ThreadWait_" + key) != null)
            {
                if (DataTableLoad0(true, "MMCore_ThreadWait_" + key).ToString() == "1")
                {
                    Thread.Sleep(period); //将调用该函数的当前线程挂起
                }
            }
        }

        /// <summary>
        /// 存储区容错处理函数，引发注册注销等存储区频繁重排序的动作，在函数开始/完成写入存储区时，设置线程等待（val=1）/闲置（val=0）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">函数动作完成，所写入存储区闲置时填"0"，反之填"1"</param>
        private static void ThreadWaitSet(string key, string val)
        {
            DataTableSet(true, "MMCore_ThreadWait_" + key, val);
        }

        #endregion

        #region Functions 按键事件功能区

        //------------------------------------↓KeyDownEventStart↓-----------------------------------------

        /// <summary>
        /// 注册指定键盘按键的委托函数，每个键盘按键最大注册数量限制（8），超过则什么也不做
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        public static void RegistKeyEventFuncref(int key, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MMCore_KeyEventFuncref_");//注册注销时进行等待
            ThreadWaitSet("MMCore_KeyEventFuncref_", "1");
            if (keyEventFuncrefGroupNum[key] >= c_regKeyMax)
            {
                return;
            }
            keyEventFuncrefGroupNum[key] += 1;
            keyEventFuncrefGroup[key, keyEventFuncrefGroupNum[key]] = funcref;
            ThreadWaitSet("MMCore_KeyEventFuncref_", "0");
        }
        /// <summary>
        /// 注册指定键盘按键的委托函数（登录在指定注册序号num位置）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="num">不能超过最大注册数量限制（8）</param>
        /// <param name="funcref"></param>
        public static void RegistKeyEventFuncref(int key, int num, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MMCore_KeyEventFuncref_");//注册注销时进行等待
            ThreadWaitSet("MMCore_KeyEventFuncref_", "1");
            keyEventFuncrefGroup[key, num] = funcref;
            ThreadWaitSet("MMCore_KeyEventFuncref_", "0");
        }

        /// <summary>
        /// 注销指定键盘按键的委托函数（发生序号重排）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        public static void RemoveKeyEventFuncref(int key, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MMCore_KeyEventFuncref_");
            ThreadWaitSet("MMCore_KeyEventFuncref_", "1");
            for (int a = 1; a <= keyEventFuncrefGroupNum[key]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (keyEventFuncrefGroup[key, a] == funcref)
                {
                    //该键位注册总数减一
                    keyEventFuncrefGroupNum[key] -= 1;
                    for (int b = a; b <= keyEventFuncrefGroupNum[key]; b += 1)
                    {
                        //将后序有效函数（如有）按序重排
                        keyEventFuncrefGroup[key, b] = keyEventFuncrefGroup[key, b];
                    }
                    //新的序号下从可疑序号重新开始检查，确保该函数在键位中彻底消失
                    a -= 1;
                }
            }
            ThreadWaitSet("MMCore_KeyEventFuncref_", "0");
        }

        /// <summary>
        /// 返回指定键盘按键注册函数的序号
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        /// <returns>错误时返回-1</returns>
        public static int GetKeyEventFuncrefNearestNum(int key, KeyMouseEventFuncref funcref)
        {
            int num = -1;
            for (int a = 1; a <= keyEventFuncrefGroupNum[key]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (keyEventFuncrefGroup[key, a] == funcref)
                {
                    //返回最近的函数序号
                    num = a;
                    break;
                }
            }
            return num;
        }

        /// <summary>
        /// 返回指定键盘按键指定函数的注册数量（>1则注册了多个同样的函数）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        public static int GetKeyEventFuncrefCount(int key, KeyMouseEventFuncref funcref)
        {
            int count = 0;
            for (int a = 1; a <= keyEventFuncrefGroupNum[key]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (keyEventFuncrefGroup[key, a] == funcref)
                {
                    count += 1;
                }
            }

            return count;
        }

        /// <summary>
        /// 归并键盘按键指定函数（如存在则移除该函数注册并序号重排，之后重新注册1次）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        public static bool RedoKeyEventFuncref(int key, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MMCore_KeyEventFuncref_");//注册注销时进行等待
            ThreadWaitSet("MMCore_KeyEventFuncref_", "1");
            bool result = false;
            int num = GetKeyEventFuncrefCount(key, funcref);
            if (num > 1)
            {
                result = true;
                //发现重复函数，移除后重新注册
                RemoveKeyEventFuncref(key, funcref);
                RegistKeyEventFuncref(key, funcref);
            }
            ThreadWaitSet("MMCore_KeyEventFuncref_", "0");
            return result;
        }

        /// <summary>
        /// 全局键盘按键事件，对指定键盘按键执行委托函数动作集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keydown"></param>
        /// <param name="player"></param>
        public static void KeyDownGlobalEvent(int key, bool keydown, int player)
        {
            for (int a = 1; a <= keyEventFuncrefGroupNum[key]; a += 1)
            {
                //这里不开新线程，是否另开线程运行宜由委托函数去写
                keyEventFuncrefGroup[key, a](keydown, player);//执行键盘按键委托
            }
        }

        //--------------------------------------↑KeyDownEventEnd↑-----------------------------------------

        //------------------------------------↓MouseDownEventStart↓---------------------------------------

        /// <summary>
        /// 注册指定鼠标键位的委托函数，每个鼠标按键最大注册数量限制（24），超过则什么也不做
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        public static void RegistMouseEventFuncref(int key, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MouseEventFuncref");//注册注销时进行等待
            ThreadWaitSet("MouseEventFuncref", "1");
            if (mouseEventFuncrefGroupNum[key] >= c_regMouseMax)
            {
                return;
            }
            mouseEventFuncrefGroupNum[key] += 1;
            mouseEventFuncrefGroup[key, mouseEventFuncrefGroupNum[key]] = funcref;
            ThreadWaitSet("MouseEventFuncref", "0");
        }

        /// <summary>
        /// 注册指定鼠标键位的委托函数（登录在指定注册序号num位置）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="num">不能超过最大注册数量限制（24）</param>
        /// <param name="funcref"></param>
        public static void RegistMouseEventFuncref(int key, int num, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MouseEventFuncref");//注册注销时进行等待
            ThreadWaitSet("MouseEventFuncref", "1");
            mouseEventFuncrefGroup[key, num] = funcref;
            ThreadWaitSet("MouseEventFuncref", "0");
        }

        /// <summary>
        /// 注销指定鼠标键位的委托函数（发生序号重排）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        public static void RemoveMouseEventFuncref(int key, KeyMouseEventFuncref funcref)
        {
            ThreadWait("MouseEventFuncref");
            ThreadWaitSet("MouseEventFuncref", "1");
            for (int a = 1; a <= mouseEventFuncrefGroupNum[key]; a += 1)
            {
                if (mouseEventFuncrefGroup[key, a] == funcref)
                {
                    mouseEventFuncrefGroupNum[key] -= 1;
                    for (int b = a; b <= mouseEventFuncrefGroupNum[key]; b += 1)
                    {
                        mouseEventFuncrefGroup[key, b] = mouseEventFuncrefGroup[key, b];
                    }
                    a -= 1;
                }
            }
            ThreadWaitSet("MouseEventFuncref", "0");
        }

        /// <summary>
        /// 返回指定鼠标键位注册函数的序号
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        /// <returns>错误时返回-1</returns>
        public static int GetMouseEventFuncrefNearestNum(int key, KeyMouseEventFuncref funcref)
        {
            int num = -1;
            for (int a = 1; a <= mouseEventFuncrefGroupNum[key]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (mouseEventFuncrefGroup[key, a] == funcref)
                {
                    //返回最近的函数序号
                    num = a;
                    break;
                }
            }
            return num;
        }

        /// <summary>
        /// 返回指定鼠标键位指定注册函数的数量（>1则注册了多个同样的函数）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        public static int GetMouseEventFuncrefCount(int key, KeyMouseEventFuncref funcref)
        {
            int count = 0;
            for (int a = 1; a <= mouseEventFuncrefGroupNum[key]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (mouseEventFuncrefGroup[key, a] == funcref)
                {
                    count += 1;
                }
            }
            return count;
        }

        /// <summary>
        /// 归并鼠标按键指定函数（如存在则移除该函数注册并序号重排，之后重新注册1次）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        public static bool RedoMouseEventFuncref(int key, KeyMouseEventFuncref funcref)
        {
            bool torf = false;
            int num = GetMouseEventFuncrefCount(key, funcref);
            if (num > 1)
            {
                torf = true;
                //发现重复函数，移除后重新注册
                RemoveMouseEventFuncref(key, funcref);
                RegistMouseEventFuncref(key, funcref);
            }
            return torf;
        }

        /// <summary>
        /// 全局鼠标按键事件，对指定鼠标按键执行委托函数动作集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keydown"></param>
        /// <param name="player"></param>
        public static void MouseDownGlobalEvent(int key, bool keydown, int player)
        {
            int a = 0;
            for (a = 1; a <= mouseEventFuncrefGroupNum[key]; a += 1)
            {
                //这里不开新线程，是否另开线程运行宜由委托函数去写
                mouseEventFuncrefGroup[key, a](keydown, player);//执行鼠标按键委托
            }
        }

        //------------------------------------↑MouseDownEventEnd↑-----------------------------------------

        #endregion

        #region Functions 入口事件功能区

        //------------------------------------↓EntryFuncStart↓-----------------------------------------

        /// <summary>
        /// 注册指定主副循环入口的委托函数，每个入口最大注册数量限制（1），超过则什么也不做
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="funcref"></param>
        public static void RegistEntryEventFuncref(Entry entry, EntryEventFuncref funcref)
        {
            ThreadWait("EntryEventFuncref");//注册注销时进行等待
            ThreadWaitSet("EntryEventFuncref", "1");
            if (entryEventFuncrefGroupNum[(int)entry] >= c_regEntryMax)
            {
                return;
            }
            entryEventFuncrefGroupNum[(int)entry] += 1;
            entryEventFuncrefGroup[(int)entry, entryEventFuncrefGroupNum[(int)entry]] = funcref;
            ThreadWaitSet("EntryEventFuncref", "0");
        }

        /// <summary>
        /// 注册指定主副循环入口的委托函数（登录在指定注册序号num位置）
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="num">不能超过最大注册数量限制（8）</param>
        /// <param name="funcref"></param>
        public static void RegistEntryEventFuncref(Entry entry, int num, EntryEventFuncref funcref)
        {
            ThreadWait("EntryEventFuncref");//注册注销时进行等待
            ThreadWaitSet("EntryEventFuncref", "1");
            entryEventFuncrefGroup[(int)entry, num] = funcref;
            ThreadWaitSet("EntryEventFuncref", "0");
        }

        /// <summary>
        /// 注销指定主副循环入口的委托函数（发生序号重排）
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="funcref"></param>
        public static void RemoveEntryEventFuncref(Entry entry, EntryEventFuncref funcref)
        {
            ThreadWait("EntryEventFuncref");
            ThreadWaitSet("EntryEventFuncref", "1");
            for (int a = 1; a <= entryEventFuncrefGroupNum[(int)entry]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (entryEventFuncrefGroup[(int)entry, a] == funcref)
                {
                    //该键位注册总数减一
                    entryEventFuncrefGroupNum[(int)entry] -= 1;
                    for (int b = a; b <= entryEventFuncrefGroupNum[(int)entry]; b += 1)
                    {
                        //将后序有效函数（如有）按序重排
                        entryEventFuncrefGroup[(int)entry, b] = entryEventFuncrefGroup[(int)entry, b];
                    }
                    //新的序号下从可疑序号重新开始检查，确保该函数在键位中彻底消失
                    a -= 1;
                }
            }
            ThreadWaitSet("EntryEventFuncref", "0");
        }

        /// <summary>
        /// 返回指定主副循环入口注册函数的序号
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="funcref"></param>
        /// <returns>错误时返回-1</returns>
        public static int GetEntryEventFuncrefNearestNum(Entry entry, EntryEventFuncref funcref)
        {
            int num = -1;
            for (int a = 1; a <= entryEventFuncrefGroupNum[(int)entry]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (entryEventFuncrefGroup[(int)entry, a] == funcref)
                {
                    //返回最近的函数序号
                    num = a;
                    break;
                }
            }
            return num;
        }

        /// <summary>
        /// 返回指定主副循环入口指定函数的注册数量（>1则注册了多个同样的函数）
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        public static int GetEntryEventFuncrefCount(Entry entry, EntryEventFuncref funcref)
        {
            int count = 0;
            for (int a = 1; a <= entryEventFuncrefGroupNum[(int)entry]; a += 1)
            {
                //遍历检查所填函数注册序号
                if (entryEventFuncrefGroup[(int)entry, a] == funcref)
                {
                    count += 1;
                }
            }

            return count;
        }

        /// <summary>
        /// 归并主副循环入口指定函数（如存在则移除该函数注册并序号重排，之后重新注册1次）
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        public static bool RedoEntryEventFuncref(Entry entry, EntryEventFuncref funcref)
        {
            bool result = false;
            int num = GetEntryEventFuncrefCount(entry, funcref);
            if (num > 1)
            {
                result = true;
                //发现重复函数，移除后重新注册
                RemoveEntryEventFuncref(entry, funcref);
                RegistEntryEventFuncref(entry, funcref);
            }
            return result;
        }

        /// <summary>
        /// 全局主副循环入口事件，对指定入口执行委托函数动作集合
        /// </summary>
        /// <param name="entry"></param>
        public static void EntryGlobalEvent(Entry entry)
        {
            for (int a = 1; a <= entryEventFuncrefGroupNum[(int)entry]; a += 1)
            {
                entryEventFuncrefGroup[(int)entry, a]();//执行主副循环入口委托
            }
        }

        //------------------------------------↑EntryFuncEnd↑-----------------------------------------

        #endregion

        #region 循环体（周期触发器、运行时钟）功能区

        /// <summary>
        /// 开启主循环（默认0.05现实时间秒，如需修改请在开启前用属性方法MainUpdatePeriod、MainUpdateDuetime来调整计时器Update阶段的间隔、前摇，若已经开启想要修改，可使用MMCore.MainUpdateTimer.Change）
        /// </summary>
        /// <param name="isBackground"></param>
        public static void MainUpdateStart(bool isBackground)
        {
            if (mainUpdateThread == null)
            {
                mainUpdateThread = new Thread(MainUpdateFunc) { IsBackground = isBackground };
                mainUpdateThread.Start();
            }
        }

        /// <summary>
        /// 开启副循环（默认1.0现实时间秒，如需修改请在开启前用属性方法SubUpdatePeriod、SubUpdateDuetime来调整计时器Update阶段的间隔、前摇，若已经开启想要修改，可使用MMCore.SubUpdateTimer.Change）
        /// </summary>
        /// <param name="isBackground"></param>
        public static void SubUpdateStart(bool isBackground)
        {
            if (mainUpdateThread == null)
            {
                subUpdateThread = new Thread(SubUpdateFunc) { IsBackground = isBackground };
                subUpdateThread.Start();
            }
        }

        /// <summary>
        /// 主循环方法，若Update阶段属性未定义则默认每轮前摇0ms、间隔50ms
        /// </summary>
        private static void MainUpdateFunc()
        {
            if (MainUpdateDuetime < 0) { MainUpdateDuetime = 0; }
            if (MainUpdatePeriod <= 0) { MainUpdatePeriod = 50; }
            MainUpdateAction(MainUpdateDuetime, MainUpdatePeriod);
        }

        /// <summary>
        /// 副循环方法，若Update阶段属性未定义则默认每轮前摇0ms、间隔50ms
        /// </summary>
        private static void SubUpdateFunc()
        {
            if (SubUpdateDuetime < 0) { SubUpdateDuetime = 0; }
            if (SubUpdatePeriod <= 0) { SubUpdatePeriod = 1000; }
            SubUpdateAction(SubUpdateDuetime, SubUpdatePeriod);
        }

        /// <summary>
        /// 主循环唤醒阶段运行一次
        /// </summary>
        public static void MainAwake()
        {
            EntryGlobalEvent(Entry.MainAwake);
        }

        /// <summary>
        /// 主循环开始阶段运行一次
        /// </summary>
        public static void MainStart()
        {
            EntryGlobalEvent(Entry.MainStart);
        }

        /// <summary>
        /// 主循环每轮更新运行
        /// </summary>
        public static void MainUpdate()
        {
            EntryGlobalEvent(Entry.MainUpdate);
        }

        /// <summary>
        /// 主循环结束阶段运行一次
        /// </summary>
        public static void MainEnd()
        {
            EntryGlobalEvent(Entry.MainEnd);
        }

        /// <summary>
        /// 主循环摧毁阶段运行一次
        /// </summary>
        public static void MainDestroy()
        {
            EntryGlobalEvent(Entry.MainDestroy);
        }

        /// <summary>
        /// 副循环唤醒阶段运行一次
        /// </summary>
        public static void SubAwake()
        {
            EntryGlobalEvent(Entry.SubAwake);
        }

        /// <summary>
        /// 副循环开始阶段运行一次
        /// </summary>
        public static void SubStart()
        {
            EntryGlobalEvent(Entry.SubStart);
        }

        /// <summary>
        /// 副循环每轮更新运行
        /// </summary>
        public static void SubUpdate()
        {
            EntryGlobalEvent(Entry.SubUpdate);
        }

        /// <summary>
        /// 副循环结束阶段运行一次
        /// </summary>
        public static void SubEnd()
        {
            EntryGlobalEvent(Entry.SubEnd);
        }

        /// <summary>
        /// 副循环摧毁阶段运行一次
        /// </summary>
        public static void SubDestroy()
        {
            EntryGlobalEvent(Entry.SubDestroy);
        }

        /// <summary>
        /// 主循环主体事件发布动作（重复执行则什么也不做），若Update阶段属性未定义则默认每轮前摇0ms、间隔50ms
        /// </summary>
        private static void MainUpdateAction()
        {
            if (AutoResetEvent_MainUpdate == null)
            {
                _autoResetEvent_MainUpdate = new AutoResetEvent(false);
                MainAwake();
                MainStart();
                if (MainUpdateDuetime < 0) { MainUpdateDuetime = 0; }
                if (MainUpdatePeriod <= 0) { MainUpdatePeriod = 50; }
                //Timer自带线程，第一参数填入要间隔执行的方法（参数须符合委托类型），第二参数填状态对象，如自动复位事件对象来控制其他线程启停
                _mainUpdateTimer = new Timer(MainUpdateChecker.CheckStatus, AutoResetEvent_MainUpdate, MainUpdateDuetime, MainUpdatePeriod);
                AutoResetEvent_MainUpdate.WaitOne();
                MainEnd();
                AutoResetEvent_MainUpdate.WaitOne();
                MainUpdateTimer.Dispose();
                MainDestroy();
            }

        }

        /// <summary>
        /// 主循环主体事件发布动作（重复执行则什么也不做），可自定义Update阶段属性Duetime（前摇）、Period（间隔）
        /// </summary>
        /// <param name="duetime">Updata阶段执行开始前等待（毫秒），仅生效一次</param>
        /// <param name="period">Updata阶段执行间隔（毫秒）</param>
        private static void MainUpdateAction(int duetime, int period)
        {
            if (AutoResetEvent_MainUpdate == null)
            {
                _autoResetEvent_MainUpdate = new AutoResetEvent(false);
                MainAwake();
                MainStart();
                if (duetime < 0) { MainUpdateDuetime = 0; }
                if (period <= 0) { MainUpdatePeriod = 50; }
                //Timer自带线程，第一参数填入要间隔执行的方法（参数须符合委托类型），第二参数填状态对象，如自动复位事件对象来控制其他线程启停
                _mainUpdateTimer = new Timer(MainUpdateChecker.CheckStatus, AutoResetEvent_MainUpdate, MainUpdateDuetime, MainUpdatePeriod);
                AutoResetEvent_MainUpdate.WaitOne();
                MainEnd();
                AutoResetEvent_MainUpdate.WaitOne();
                MainUpdateTimer.Dispose();
                MainDestroy();
            }

        }

        /// <summary>
        /// 副循环主体事件发布动作（重复执行则什么也不做），若Update阶段属性未定义则默认每轮前摇0ms、间隔1000ms
        /// </summary>
        private static void SubUpdateAction()
        {
            if (AutoResetEvent_SubUpdate == null)
            {
                _autoResetEvent_SubUpdate = new AutoResetEvent(false);
                SubAwake();
                SubStart();
                if (SubUpdateDuetime < 0) { SubUpdateDuetime = 0; }
                if (SubUpdatePeriod <= 0) { SubUpdatePeriod = 1000; }
                _subUpdateTimer = new Timer(SubUpdateChecker.CheckStatus, AutoResetEvent_SubUpdate, SubUpdateDuetime, SubUpdatePeriod);//已经运行的话，后续使用SubUpdateTimer.Change改变间隔
                AutoResetEvent_SubUpdate.WaitOne();
                SubEnd();
                AutoResetEvent_SubUpdate.WaitOne();
                SubUpdateTimer.Dispose();
                SubDestroy();
            }

        }

        /// <summary>
        /// 副循环主体事件发布动作（重复执行则什么也不做），可自定义Update阶段属性Duetime（前摇）、Period（间隔）
        /// </summary>
        /// <param name="duetime">Updata阶段执行开始前等待（毫秒），仅生效一次</param>
        /// <param name="period">Updata阶段执行间隔（毫秒）</param>
        private static void SubUpdateAction(int duetime, int period)
        {
            if (AutoResetEvent_SubUpdate == null)
            {
                _autoResetEvent_SubUpdate = new AutoResetEvent(false);
                SubAwake();
                SubStart();
                if (duetime < 0) { SubUpdateDuetime = 0; }
                if (period <= 0) { SubUpdatePeriod = 1000; }
                _subUpdateTimer = new Timer(SubUpdateChecker.CheckStatus, AutoResetEvent_SubUpdate, SubUpdateDuetime, SubUpdatePeriod);//已经运行的话，后续使用SubUpdateTimer.Change改变间隔
                AutoResetEvent_SubUpdate.WaitOne();
                SubEnd();
                AutoResetEvent_SubUpdate.WaitOne();
                SubUpdateTimer.Dispose();
                SubDestroy();
            }

        }

        #endregion

    }

    /// <summary>
    /// 主循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，主循环Update事件被执行时创建计时器的父线程（mainUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
    /// </summary>
    public static class MainUpdateChecker
    {
        private static int _invokeCount;
        private static bool _timerState;

        /// <summary>
        /// 主循环Update事件运行次数
        /// </summary>
        public static int InvokeCount { get => _invokeCount; set => _invokeCount = value; }
        /// <summary>
        /// 主循环状态字段，手动设置为True则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        public static bool TimerState { get => _timerState; set => _timerState = value; }

        /// <summary>
        /// 主循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，主循环Update事件被执行时创建计时器的父线程（mainUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        static MainUpdateChecker()
        {
            InvokeCount = 0;
            TimerState = false;
        }

        /// <summary>
        /// 主循环的计时器实例创建时以参数填入、被反复执行的函数，Update事件被执行时创建计时器的父线程将暂停，直到本函数确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        /// <param name="state"></param>
        public static void CheckStatus(Object state)
        {
            if (TimerState)
            {
                ((AutoResetEvent)state).Set();
            }
            else
            {
                InvokeCount++;
                MMCore.MainUpdate();
            }
        }

    }

    /// <summary>
    /// 副循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，副循环Update事件被执行时创建计时器的父线程（subUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
    /// </summary>
    public static class SubUpdateChecker
    {
        //静态类不允许声明实例成员，因为无法判断每个实例的内存地址

        private static int _invokeCount;
        private static bool _timerState;

        /// <summary>
        /// 副循环Update事件运行次数
        /// </summary>
        public static int InvokeCount { get => _invokeCount; set => _invokeCount = value; }
        /// <summary>
        /// 副循环状态字段，手动设置为True则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        public static bool TimerState { get => _timerState; set => _timerState = value; }

        //一个类只能有一个静态构造函数，不能有访问修饰符（因为不是给用户调用的，且是由.net 框架在合适的时机调用）
        //静态构造函数也不能带任何参数（主要因为框架不可能知道我们需要在函数中添加什么参数，所以干脆规定不能使用参数）
        //静态构造函数是特殊的静态方法，同样不允许使用实例成员
        //无参静态构造函数和无参实例构造函数可并存不冲突，内存地址不同
        //所有静态数据只会从模板复制一份副本，所以静态构造函数只被执行一次
        //平时没在类中写构造函数也没继承那么框架会生成一个无参构造函数，当类中定义静态成员没定义静态构造函数时，框架亦会生成一个静态构造函数来让框架自身来调用

        /// <summary>
        /// 副循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，副循环Update事件被执行时创建计时器的父线程（subUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        static SubUpdateChecker()
        {
            InvokeCount = 0;
            TimerState = false;
        }

        //静态方法只能访问类中的静态成员（即便在实例类，同理因无法判断非静态变量等这些实例成员的活动内存地址，所以不允许使用实例成员）

        /// <summary>
        /// 副循环的计时器实例创建时以参数填入、被反复执行的函数，Update事件被执行时创建计时器的父线程将暂停，直到本函数确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        /// <param name="state"></param>
        public static void CheckStatus(Object state)
        {
            if (TimerState)
            {
                ((AutoResetEvent)state).Set();
            }
            else
            {
                InvokeCount++;
                MMCore.SubUpdate();
            }
        }

    }

    /// <summary>
    /// 周期触发器，创建实例后请给函数注册事件（语法：TimerUpdate.Awake/Start/Update/End/Destroy +=/-= 任意符合事件参数格式的函数的名称如MyFunc，其声明为void MyFun(object sender, EventArgs e)，sender传递本类实例（其他类型也可），e传递额外事件参数类的信息），TriggerStart方法将自动创建独立触发器线程并启动周期触发器（主体事件发布动作），启动前可用Duetime、Period属性方法设定Update阶段每次循环的前摇和间隔，启动后按序执行Awake/Start/Update/End/Destroy被这5种事件注册过的委托函数，其中事件Update阶段是一个计时器循环，直到用户手动调用TimerState属性方法，该属性为true时会让计时器到期退出Update循环，而计时器所在父线程（即触发器线程）将运行End和Destory事件
    /// </summary>
    public class TimerUpdate
    {
        #region 变量、字段及其属性方法

        /// <summary>
        /// 自动复位事件（用来控制触发线程信号）
        /// </summary>
        private AutoResetEvent _autoResetEvent_TimerUpdate;
        /// <summary>
        /// 自动复位事件，提供该属性方便随时读取，但不允许不安全赋值
        /// </summary>
        public AutoResetEvent AutoResetEvent_TimerUpdate { get => _autoResetEvent_TimerUpdate; }

        /// <summary>
        /// 周期触发器主体事件发布动作所在线程的实例
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// 周期触发器执行Update事件的计时器实例
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// 周期触发器Update事件运行次数
        /// </summary>
        private int _invokeCount;
        /// <summary>
        /// 周期触发器Update事件运行次数，该属性可随时读取或清零
        /// </summary>
        public int InvokeCount { get => _invokeCount; set => _invokeCount = value; }

        /// <summary>
        /// 周期触发器Update事件运行次数上限，该属性让计时器到期退出循环，计时器所在父线程将运行End和Destory事件
        /// </summary>
        private int _invokeCountMax;
        /// <summary>
        /// 周期触发器Update事件运行次数上限，该属性让计时器到期退出循环，计时器所在父线程将运行End和Destory事件
        /// </summary>
        public int InvokeCountMax { get => _invokeCountMax; set => _invokeCountMax = value; }

        /// <summary>
        /// 周期触发器Update事件前摇，未设置直接启动TriggerStart则默认为0
        /// </summary>
        private int _duetime;
        /// <summary>
        /// 周期触发器Update事件前摇，未设置直接启动TriggerStart则默认为0
        /// </summary>
        public int Duetime { get => _duetime; set => _duetime = value; }

        /// <summary>
        /// 周期触发器的运行间隔字段，未设置直接启动TriggerStart则默认为1s
        /// </summary>
        private int _period;
        /// <summary>
        /// 周期触发器的运行间隔属性，未设置直接启动TriggerStart则默认为1s
        /// </summary>
        public int Period { get => _period; set => _period = value; }

        /// <summary>
        /// 周期触发器的状态字段，手动设置为True则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        private bool _timerState;
        /// <summary>
        /// 周期触发器的状态属性，手动设置为True则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        public bool TimerState { get => _timerState; set => _timerState = value; }

        /// <summary>
        /// 事件委托列表，用来存储多个事件委托，用对象类型的键来取出，内部属性，用户不需要操作
        /// </summary>
        protected EventHandlerList _listEventDelegates = new EventHandlerList();

        /// <summary>
        /// 周期触发器主体事件发布动作所在线程的实例，提供该属性方便随时读取，但不允许不安全赋值
        /// </summary>
        public Thread Thread { get => _thread; }

        /// <summary>
        /// 周期触发器执行Update事件的计时器实例，提供该属性方便随时读取，但不允许不安全赋值
        /// </summary>
        public Timer Timer { get => _timer; }

        #endregion

        #region 定义区分对象类型的键

        //下方每个new object()都是一个单独的实例个体，所以定义的五个变量在内存ID是不同的

        private static readonly object awakeEventKey = new object();
        private static readonly object startEventKey = new object();
        private static readonly object updateEventKey = new object();
        private static readonly object endEventKey = new object();
        private static readonly object destroyEventKey = new object();

        #endregion

        #region 声明事件委托（用于用户注册事件给函数）

        //声明事件委托变量（首字母大写），相比常规委托，事件委托因安全考虑无法直接被执行，通过OnAwake内部函数确保安全执行（其实是声明临时常规委托在赋值后执行）

        /// <summary>
        /// 将周期触发器的唤醒事件注册到函数，语法：TimerUpdate.Awake +=/-= 实例或静态函数
        /// </summary>
        public event TimerEventHandler Awake
        {
            // Add the input delegate to the collection.
            add
            {
                _listEventDelegates.AddHandler(awakeEventKey, value);
            }
            // Remove the input delegate from the collection.
            remove
            {
                _listEventDelegates.RemoveHandler(awakeEventKey, value);
            }
        }

        /// <summary>
        /// 将周期触发器的开始事件注册到函数，语法：TimerUpdate.Start +=/-= 实例或静态函数
        /// </summary>
        public event TimerEventHandler Start
        {
            add
            {
                _listEventDelegates.AddHandler(startEventKey, value);
            }
            remove
            {
                _listEventDelegates.RemoveHandler(startEventKey, value);
            }
        }

        /// <summary>
        /// 将周期触发器的开始事件注册到函数，语法：TimerUpdate.Update +=/-= 实例或静态函数
        /// </summary>
        public event TimerEventHandler Update
        {
            add
            {
                _listEventDelegates.AddHandler(updateEventKey, value);
            }
            remove
            {
                _listEventDelegates.RemoveHandler(updateEventKey, value);
            }
        }

        /// <summary>
        /// 将周期触发器的开始事件注册到函数，语法：TimerUpdate.End +=/-= 实例或静态函数
        /// </summary>
        public event TimerEventHandler End
        {
            add
            {
                _listEventDelegates.AddHandler(endEventKey, value);
            }
            remove
            {
                _listEventDelegates.RemoveHandler(endEventKey, value);
            }
        }

        /// <summary>
        /// 将周期触发器的开始事件注册到函数，语法：TimerUpdate.Destroy +=/-= 实例或静态函数
        /// </summary>
        public event TimerEventHandler Destroy
        {
            add
            {
                _listEventDelegates.AddHandler(destroyEventKey, value);
            }
            remove
            {
                _listEventDelegates.RemoveHandler(destroyEventKey, value);
            }
        }

        #endregion

        #region 构造函数（本类创建时的2个重载方法）

        /// <summary>
        /// 创建一个不会到期的周期触发器
        /// </summary>
        public TimerUpdate()
        {
            InvokeCount = 0;
            InvokeCountMax = 0;
            TimerState = false;
        }

        /// <summary>
        /// 创建一个有执行次数的周期触发器
        /// </summary>
        /// <param name="invokeCountMax">决定计时器Update阶段循环次数</param>
        public TimerUpdate(int invokeCountMax)
        {
            InvokeCount = 0;
            InvokeCountMax = invokeCountMax;
            TimerState = false;
        }

        #endregion

        //非静态（实例）方法可以访问类中的任何成员

        /// <summary>
        /// 计时器实例创建时以参数填入、被反复执行的函数，Update事件被执行时创建计时器的父线程将暂停，直到本函数确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        /// <param name="state"></param>
        private void CheckStatus(Object state)
        {
            if (TimerState)
            {
                ((AutoResetEvent)state).Set();
            }
            else if (InvokeCountMax > 0 && InvokeCount >= InvokeCountMax)
            {
                ((AutoResetEvent)state).Set();
            }
            else
            {
                InvokeCount++;
                OnUpdate(this, new EventArgs());
            }
        }

        //注：Action函数在特殊需要时再设置为公开（不让用户直接使用），用户使用TimerUpdate.TriggerStart自带线程启动

        /// <summary>
        /// 周期触发器主体事件发布动作（重复执行则什么也不做），在当前线程创建周期触发器并执行事件委托（提前定义事件委托变量TimerUpdate.Awake/Start/Update/End/Destroy += 要委托执行的函数，即完成事件注册到函数），可预先自定义计时器Updata阶段的执行间隔（否则默认以Duetime=0、Period=1000运行计时器）。注：若直接调用本函数则在计时器Updata阶段会暂停当前线程，不想暂停请额外开线程手动加载Action运行或使用TimerUpdate.TriggerStart自带线程启动（推荐）
        /// </summary>
        private void Action()
        {
            if (_autoResetEvent_TimerUpdate == null)
            {
                //private void OnAwake(object sender, EventArgs e)
                //sender参数用于传递指向事件源对象的引用，简单来讲就是当前的对象
                //sender参数也可填任意类型，填this的话传递后能从本实例类中得到字段值等信息，要传递事件变量可写在本类里
                //e参数是是EventArgs类型，简单来理解就是记录事件传递过来的额外信息
                //e参数可通过自定义类继承EventArgs类，里面记录额外事件变量，该类以参数填入完成传递

                _autoResetEvent_TimerUpdate = new AutoResetEvent(false);
                //执行委托并传递事件参数
                OnAwake(this, new EventArgs());
                OnStart(this, new EventArgs());
                if (_duetime < 0) { _duetime = 0; }
                if (_period <= 0) { _period = 1000; }
                _timer = new Timer(CheckStatus, _autoResetEvent_TimerUpdate, _duetime, _period);
                _autoResetEvent_TimerUpdate.WaitOne();
                OnEnd(this, new EventArgs());
                _autoResetEvent_TimerUpdate.WaitOne();
                _timer.Dispose();
                OnDestroy(this, new EventArgs());
            }
        }

        /// <summary>
        /// 周期触发器主体事件发布动作（重复执行则什么也不做），在当前线程创建周期触发器并执行事件委托（提前定义事件委托变量TimerUpdate.Awake/Start/Update/End/Destroy += 要委托执行的函数，即完成事件注册到函数），可预先自定义计时器Updata阶段的执行间隔（否则默认以Duetime=0、Period=1000运行计时器）。注：若直接调用本函数则在计时器Updata阶段会暂停当前线程，不想暂停请额外开线程手动加载Action运行或使用TimerUpdate.TriggerStart自带线程启动（推荐）
        /// </summary>
        /// <param name="duetime">Updata阶段执行开始前等待（毫秒），仅生效一次</param>
        /// <param name="period">Updata阶段执行间隔（毫秒）</param>
        private void Action(int duetime, int period)
        {
            if (_autoResetEvent_TimerUpdate == null)
            {
                _autoResetEvent_TimerUpdate = new AutoResetEvent(false);
                OnAwake(this, new EventArgs());
                OnStart(this, new EventArgs());
                if (_duetime < 0) { _duetime = 0; }
                if (_period <= 0) { _period = 1000; }
                _timer = new Timer(CheckStatus, _autoResetEvent_TimerUpdate, duetime, period);
                _autoResetEvent_TimerUpdate.WaitOne();
                OnEnd(this, new EventArgs());
                _autoResetEvent_TimerUpdate.WaitOne();
                _timer.Dispose();
                OnDestroy(this, new EventArgs());
            }
        }

        #region 声明常规委托，安全执行事件委托并传递事件参数

        /// <summary>
        /// 计时器唤醒阶段时运行一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAwake(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[awakeEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 周期触发器开始阶段运行一次
        /// </summary>
        private void OnStart(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[startEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 周期触发器Update阶段按预设间隔反复运行
        /// </summary>
        private void OnUpdate(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[updateEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 周期触发器结束阶段运行一次
        /// </summary>
        private void OnEnd(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[endEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 周期触发器摧毁阶段运行一次
        /// </summary>
        private void OnDestroy(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[destroyEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        #endregion

        #region 自动创建线程执行周期触发器（模拟触发器运行）

        /// <summary>
        /// 自动创建线程启动周期触发器（模拟触发器运行），重复启动时什么也不做，未设置Update属性则默认以Duetime=0、Period=1000运行计时器循环
        /// </summary>
        /// <param name="isBackground">true将启动线程调整为后台线程</param>
        public void TriggerStart(bool isBackground)
        {
            if (_thread == null)
            {
                _thread = new Thread(Action) { IsBackground = isBackground };
                _thread.Start();
            }
        }

        #endregion

    }

    public class Unit
    {
        #region 字段声明（字段用于每个实例存储不同的值）

        private string _name;

        private double _age;
        private double _speed;
        private double _attackSpeed;
        private double _strength;
        private double _vitality;
        private double _agility;
        private double _intelligence;
        private double _dexterity;
        private double _luck;
        private double _atk;
        private double _def;
        private double _matk;
        private double _mdef;
        private double _critical;
        private double _antiCritical;
        private double _maspd;
        private double _hp;
        private double _mp;
        private double _sp;
        private double[] _tp;
        private double _exp;
        private double _hps;
        private double _mps;
        private double _sps;
        private double[] _tps;
        private double _evasion;
        private double _evasionRate;
        private double _perfectEvasion;
        private double _perfectEvasionRate;
        private double _hit;
        private double _hitRate;
        private double _perfectHit;
        private double _perfectHitRate;
        private double _killRate;
        private double _antiAntiKilledRate;
        private double _carryWeight = 300.0;

        private double _ageMax;
        private double _speedMax;
        private double _strengthMax;
        private double _vitalityMax;
        private double _agilityMax;
        private double _intelligenceMax;
        private double _dexterityMax;
        private double _luckMax;
        private double _atkMax;
        private double _defMax;
        private double _matkMax;
        private double _mdefMax;
        private double _criticalMax;
        private double _antiCriticalMax;
        private double _maspdMax;
        private double _hpMax;
        private double _mpMax;
        private double _spMax;
        private double[] _tpMax;
        private double _expMax;
        private double _hpsMax;
        private double _mpsMax;
        private double _spsMax;
        private double[] _tpsMax;
        private double _evasionMax;
        private double _evasionRateMax;
        private double _perfectEvasionMax;
        private double _perfectEvasionRateMax;
        private double _hitMax;
        private double _hitRateMax;
        private double _perfectHitMax;
        private double _perfectHitRateMax;
        private double _killRateMax;
        private double _antiAntiKilledRateMax;
        private double _carryWeightMax = 300.0;

        private int _Tag;
        private string _typeName;
        private Vector3D _vector3D;
        private Vector _vector;
        private double _terrainHeight = 0.0;
        private double _height = 1.8;
        private double _radius = 0.5;
        private double _selectedRadius = 0.5;

        #endregion

        #region 构造函数

        /// <summary>
        /// 创建单位实例
        /// </summary>
        public Unit() { }

        #endregion

        #region 属性方法

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// 年龄
        /// </summary>
        public double Age { get => _age; set => _age = value; }
        /// <summary>
        /// 移动速度（每帧行动距离换算到每秒），由游戏内部针对当前帧速进行修改（如每帧50ms时，实际每帧移动距离=50*Speed/1000）
        /// </summary>
        public double Speed { get => _speed; set => _speed = value; }
        /// <summary>
        /// ASPD：物理攻击速度（每帧行动次数换算到每秒），受到多个因素同时影响如AGI、DEX、职业、武器、药水等（设计ASPD=100即每秒攻击5次，则每4帧攻击1次），由游戏内部针对当前帧速进行修改（如每帧50ms时，实际每次攻击等待毫秒数=50*ASPD/（100/5）=200ms）
        /// </summary>
        public double AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
        /// <summary>
        /// STR：力量，影响物理攻击（每点加2）、负重上限（每点加30，按牛顿为单位约50~60个鸡蛋）、武器发挥（当STR很低时，武器攻击无法完全发挥，在STR为0时只能发挥25%），但不影响暴击和抵挡几率和每秒伤害输出DPS(Attack Power)
        /// 当用斧、书、剑、拳刃、拳套、钝器、矛等近战武器或空手时ATK +1、武器ATK+0.5%）仅对基础武器ATK有效，且强制无属性），当用弓、枪械、乐器、鞭等远程武器时ATK+0.2
        /// </summary>
        public double Strength { get => _strength; set => _strength = value; }
        /// <summary>
        /// VIT：耐力（体力），影响生命值上限和物理防御，每点增加HP上限30或1%、HP恢复+0.2、物理防御0.5~1、魔防0.2、HP恢复类道具效果+2%，每5点增加HP回复1
        /// 状态效果影响：中毒状态成功率-1%且持续时间减少，VIT≥100时免疫中毒状态，混乱状态成功率减少，昏迷状态成功率-1%且持续时间减少，VIT≥100时免疫昏迷状态，沉默状态成功率 -1%且持续时间-0.01秒，诅咒状态持续时间-0.01秒，霜冻状态成功率减少且持续时间-0.05秒，冷冻状态持续时间-0.1秒
        /// </summary>
        public double Vitality { get => _vitality; set => _vitality = value; }
        /// <summary>
        /// AGI：敏捷，每点增加攻击速度1%、闪避1、防御力0.2、致命一击或暴击
        /// 状态效果影响：出血状态成功率和持续时间减少，移动不可状态中部分陷阱类状态成功率和持续时间减少，灼烧状态成功率和持续时间减少
        /// </summary>
        public double Agility { get => _agility; set => _agility = value; }
        /// <summary>
        /// INT：智力，影响魔攻、魔法值和魔法致命一击几率，每点加魔攻2、魔防0.5~1、魔法值1%，每6点增Sp恢复速度（INT<120时，SP自然恢复 +0.6，INT≥120时，SP自然恢复+1.3），增至偶数时Sp上限加7、奇数时Sp上限加6，可变吟唱时间减少，恢复类道具效果+1%
        /// 状态效果影响：冰冻状态持续时间减少，对口职业睡眠状态成功率 -1%，持续时间减少，黑暗状态成功率减少，持续时间减少，恐惧状态成功率减少，持续时间减少，沉睡状态成功率减少，持续时间减少
        /// </summary>
        public double Intelligence { get => _intelligence; set => _intelligence = value; }
        /// <summary>
        /// DEX：敏捷（灵巧），影响玩家命中和可变吟唱时间，每点加命中1，每3点减0.1秒可变咏唱时间并加物理攻击1，每5点加物理攻击1，每30点减1秒可变咏唱时间（游戏中有个重要名词叫“不可变吟唱时间”也就是说某个技能存在一个最低下限）
        /// 当用弓、枪械、乐器、鞭等远程武器时ATK+1、武器ATK+0.5%（仅对基础武器ATK有效且强制无属性），当用斧、书、剑、拳刃、拳套、钝器、矛等近战武器或空手时ATK+0.2、MATK+0.2、HIT+1、MDEF+1、可变吟唱时间减少、按比例增加ASPD
        /// 状态效果影响：装备卸除状态持续时间减少，霜冻状态成功率减少且持续时间-0.05秒
        /// </summary>
        public double Dexterity { get => _dexterity; set => _dexterity = value; }
        /// <summary>
        /// LUK：幸运（厄运为负），影响装备道具的出现率&探索发现率、暴击率、完全回避等，每点CRI+0.3、ATK+0.3、MATK+0.3、HIT+0.3、回避+0.2、完全回避+0.1、免暴率+1，每3点加暴击1，每5点加物理攻击1和防暴击1
        /// 状态效果影响：中毒状态成功率&持续时间减少，冰冻状态成功率减少，混乱状态成功率减少，石化状态成功率减少，睡眠状态成功率减少，昏迷状态成功率减少，持续时间减少，沉默状态成功率减少，诅咒状态成功率减少，黑暗状态成功率减少，沉睡状态成功率减少
        /// </summary>
        public double Luck { get => _luck; set => _luck = value; }
        /// <summary>
        /// Atk：物理攻击力，力量的主要相关因素以及受其他辅助因素影响
        /// </summary>
        public double Atk { get => _atk; set => _atk = value; }
        /// <summary>
        /// DEF：防御力，敏捷的主要相关因素以及受其他辅助因素影响
        /// </summary>
        public double Def { get => _def; set => _def = value; }
        /// <summary>
        /// Matk：魔法攻击力
        /// </summary>
        public double Matk { get => _matk; set => _matk = value; }
        /// <summary>
        /// Mdef ：魔法防御力
        /// </summary>
        public double Mdef { get => _mdef; set => _mdef = value; }
        /// <summary>
        /// CRI：暴击率，一般根据人物等级设计攻击力来计算（按一定比率采取反推方式完成）
        /// </summary>
        public double Critical { get => _critical; set => _critical = value; }
        /// <summary>
        /// 抗暴率，实际暴击率按抗暴率程度将暴击率消减再修正数值得到实际暴击率，简单点就直接相减
        /// </summary>
        public double AntiCritical { get => _antiCritical; set => _antiCritical = value; }
        /// <summary>
        /// Maspd：魔法攻击速度，MASPD受到多个因素同时影响：INT、AGI、DEX、职业、武器、药水等，其中敏捷和常规物理攻击速度关联较大，施法频率参考ASPD
        /// </summary>
        public double Maspd { get => _maspd; set => _maspd = value; }
        /// <summary>
        /// 生命值
        /// </summary>
        public double Hp { get => _hp; set => _hp = value; }
        /// <summary>
        /// 魔法点：ManaPoint，当低于所需时，角色无法使用相关武器装备和常规技能
        /// </summary>
        public double Mp { get => _mp; set => _mp = value; }
        /// <summary>
        /// 特殊技能点：当低于所需时，角色无法使用特殊技能
        /// </summary>
        public double Sp { get => _sp; set => _sp = value; }
        /// <summary>
        /// 计时阶段状态条（可有多个），用于单位工作或施法阶段进行计时等用途
        /// </summary>
        public double[] Tp { get => _tp; set => _tp = value; }
        /// <summary>
        /// 经验值：experience，积累到一定数量可提高等级、能力
        /// </summary>
        public double Exp { get => _exp; set => _exp = value; }
        /// <summary>
        /// 生命值恢复速度（每帧），延时恢复另详设计
        /// </summary>
        public double Hps { get => _hps; set => _hps = value; }
        /// <summary>
        /// 魔法点恢复速度（每帧），延时恢复另详设计
        /// </summary>
        public double Mps { get => _mps; set => _mps = value; }
        /// <summary>
        /// 技能点恢复速度（每帧），延时恢复另详设计
        /// </summary>
        public double Sps { get => _sps; set => _sps = value; }
        /// <summary>
        /// 工作、施法时间阶段的状态值恢复或倒计速度
        /// </summary>
        public double[] Tps { get => _tps; set => _tps = value; }
        /// <summary>
        /// 闪避
        /// </summary>
        public double Evasion { get => _evasion; set => _evasion = value; }
        /// <summary>
        /// 闪避率，其值=1 / (1 + 100 / 闪避面板数值)，允许超过100的原因是面对命中率时可扣减，并不是最终结果
        /// </summary>
        public double EvasionRate { get => _evasionRate; set => _evasionRate = value; }
        /// <summary>
        /// 完美闪避，此项仅受幸运影响
        /// </summary>
        public double PerfectEvasion { get => _perfectEvasion; set => _perfectEvasion = value; }
        /// <summary>
        /// 完美闪避率，根据完美闪避计算后修正得出，并用来参与结果
        /// </summary>
        public double PerfectEvasionRate { get => _perfectEvasionRate; set => _perfectEvasionRate = value; }
        /// <summary>
        /// 命中
        /// </summary>
        public double Hit { get => _hit; set => _hit = value; }
        /// <summary>
        /// 命中率
        /// </summary>
        public double HitRate { get => _hitRate; set => _hitRate = value; }
        /// <summary>
        /// 完美命中，此项仅受幸运影响
        /// </summary>
        public double PerfectHit { get => _perfectHit; set => _perfectHit = value; }
        /// <summary>
        /// 完美命中率，根据完美命中计算后修正得出，并用来参与结果
        /// </summary>
        public double PerfectHitRate { get => _perfectHitRate; set => _perfectHitRate = value; }
        /// <summary>
        /// 致命一击率，允许超过100的原因是遇对方有抗即死率时进行扣减，并不是最终结果
        /// </summary>
        public double KillRate { get => _killRate; set => _killRate = value; }
        /// <summary>
        /// 抗即死率，用来防御致命一击
        /// </summary>
        public double AntiKilledRate { get => _antiAntiKilledRate; set => _antiAntiKilledRate = value; }
        /// <summary>
        /// 单位负重
        /// </summary>
        public double CarryWeight { get => _carryWeight; set => _carryWeight = value; }

        //↓限制

        public double AgeMax { get => _ageMax; set => _ageMax = value; }
        public double SpeedMax { get => _speedMax; set => _speedMax = value; }
        public double StrengthMax { get => _strengthMax; set => _strengthMax = value; }
        public double VitalityMax { get => _vitalityMax; set => _vitalityMax = value; }
        public double AgilityMax { get => _agilityMax; set => _agilityMax = value; }
        public double IntelligenceMax { get => _intelligenceMax; set => _intelligenceMax = value; }
        public double DexterityMax { get => _dexterityMax; set => _dexterityMax = value; }
        public double LuckMax { get => _luckMax; set => _luckMax = value; }
        public double AtkMax { get => _atkMax; set => _atkMax = value; }
        public double DefMax { get => _defMax; set => _defMax = value; }
        public double MatkMax { get => _matkMax; set => _matkMax = value; }
        public double MdefMax { get => _mdefMax; set => _mdefMax = value; }
        public double CriticalMax { get => _criticalMax; set => _criticalMax = value; }
        public double AntiCriticalMax { get => _antiCriticalMax; set => _antiCriticalMax = value; }
        public double MaspdMax { get => _maspdMax; set => _maspdMax = value; }
        public double HpMax { get => _hpMax; set => _hpMax = value; }
        public double MpMax { get => _mpMax; set => _mpMax = value; }
        public double SpMax { get => _spMax; set => _spMax = value; }
        public double[] TpMax { get => _tpMax; set => _tpMax = value; }
        public double ExpMax { get => _expMax; set => _expMax = value; }
        public double HpsMax { get => _hpsMax; set => _hpsMax = value; }
        public double MpsMax { get => _mpsMax; set => _mpsMax = value; }
        public double SpsMax { get => _spsMax; set => _spsMax = value; }
        public double[] TpsMax { get => _tpsMax; set => _tpsMax = value; }
        public double EvasionMax { get => _evasionMax; set => _evasionMax = value; }
        public double EvasionRateMax { get => _evasionRateMax; set => _evasionRateMax = value; }
        public double PerfectEvasionMax { get => _perfectEvasionMax; set => _perfectEvasionMax = value; }
        public double PerfectEvasionRateMax { get => _perfectEvasionRateMax; set => _perfectEvasionRateMax = value; }
        public double HitMax { get => _hitMax; set => _hitMax = value; }
        public double HitRateMax { get => _hitRateMax; set => _hitRateMax = value; }
        public double PerfectHitMax { get => _perfectHitMax; set => _perfectHitMax = value; }
        public double PerfectHitRateMax { get => _perfectHitRateMax; set => _perfectHitRateMax = value; }
        public double KillRateMax { get => _killRateMax; set => _killRateMax = value; }
        public double AntiKilledRateMax { get => _antiAntiKilledRateMax; set => _antiAntiKilledRateMax = value; }
        public double CarryWeightMax { get => _carryWeightMax; set => _carryWeightMax = value; }

        //↓其他属性

        /// <summary>
        /// 单位标签（句柄）
        /// </summary>
        public int Tag { get => _Tag; set => _Tag = value; }

        /// <summary>
        /// 单位类型在编辑器的名字
        /// </summary>
        public string TypeName { get => _typeName; set => _typeName = value; }

        /// <summary>
        /// 单位脚底坐标向量(三维)，Z坐标是根据计算得到（Z=MapHeight+TerrainHeight+Unit.TerrainHeight），平时只要实时更新平面坐标即可根据该二维点高度信息更新3D高度
        /// </summary>
        public Vector3D Vector3D { get => _vector3D; set => _vector3D = value; }

        /// <summary>
        /// 单位脚底坐标向量(二维)
        /// </summary>
        public Vector Vector { get => _vector; set => _vector = value; }

        /// <summary>
        /// 单位层地形物件高度（区别地图层地形物件），其值使单位浮空或嵌入地面
        /// </summary>
        public double TerrainHeight { get => _terrainHeight; set => _terrainHeight = value; }

        /// <summary>
        /// 单位高度，注意当鼠标划过或点击单位时返回的Z坐标是在其头顶的（mouseVectorZ=MapHeight+TerrainHeight+Unit.TerrainHeight+Unit.Height）
        /// </summary>
        public double Height { get => _height; set => _height = value; }

        /// <summary>
        /// 单位碰撞检查半径，默认与模型半径一致，可修改每个单位的碰撞范围
        /// </summary>
        public double Radius { get => _radius; set => _radius = value; }

        /// <summary>
        /// 鼠标点击单位时的选择及搜索范围
        /// </summary>
        public double SelectedRadius { get => _selectedRadius; set => _selectedRadius = value; }


        #endregion

    }

    public static class Player
    {
        #region 变量、字段及其属性方法

        public static Unit[] hero = new Unit[MMCore.c_maxPlayers + 1];
        public static bool[] canNotOperation = new bool[MMCore.c_maxPlayers + 1];
        public static bool[] moveLoop = new bool[MMCore.c_maxPlayers + 1];

        public static bool[,] keyDown = new bool[MMCore.c_maxPlayers + 1, MMCore.c_keyMax + 1];
        public static bool[,] keyDownState = new bool[MMCore.c_maxPlayers + 1, MMCore.c_keyMax + 1];
        public static int[] keyDownLoopOneBitNum = new int[MMCore.c_maxPlayers + 1];

        public static bool[] mouseLeftDown = new bool[MMCore.c_maxPlayers + 1];
        public static bool[] mouseRightDown = new bool[MMCore.c_maxPlayers + 1];
        public static bool[] mouseRightDownLoop = new bool[MMCore.c_maxPlayers + 1];
        public static bool[] mouseMiddleDown = new bool[MMCore.c_maxPlayers + 1];
        public static bool[,] mouseDownState = new bool[MMCore.c_maxPlayers + 1, MMCore.c_mouseMax + 1];
        public static int[] mouseDownLoopOneBitNum = new int[MMCore.c_maxPlayers + 1];

        public static int[] mouseUIX = new int[MMCore.c_maxPlayers + 1];
        public static int[] mouseUIY = new int[MMCore.c_maxPlayers + 1];

        public static double[] mouseVectorX = new double[MMCore.c_maxPlayers + 1];
        public static double[] mouseVectorY = new double[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 鼠标点高度，mouseVectorZ=MapHeight+TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// 悬崖、地形物件及单位在移动、诞生摧毁时应将高度信息刷新，以便实时获取
        /// </summary>
        public static double[] mouseVectorZ = new double[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 修正后的鼠标点高度（扣减了地面高度，所以这是相对地面的高度），mouseVectorZFixed=mouseVectorZ-MapHeight=TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// </summary>
        public static double[] mouseVectorZFixed = new double[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 世界中鼠标与玩家控制英雄形成的2D角度，象限分布：右=0度，上=90°，左=180°，下=270°
        /// </summary>
        public static double[] mouseToHeroAngle = new double[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 世界中鼠标与玩家控制英雄形成的2D距离
        /// </summary>
        public static double[] mouseToHeroRange = new double[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 世界中鼠标与玩家控制英雄形成的3D距离
        /// </summary>
        public static double[] mouseToHeroRange3D = new double[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 玩家镜头位置点
        /// </summary>
        public static Vector3D[] cameraVector3D = new Vector3D[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 鼠标3D点向量坐标，修正了鼠标点高度（扣减了地图高度，所以这是相对地面的高度），mouseVectorZFixed=mouseVectorZ-MapHeight=TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// </summary>
        public static Vector3D[] mouseVector3DFixed = new Vector3D[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 鼠标3D点向量坐标，鼠标Z点在单位高度顶部，Z=MapHeight+TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// </summary>
        public static Vector3D[] mouseVector3D = new Vector3D[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 鼠标3D点向量坐标，鼠标Z点在单位层地形物件高度顶部（单位脚底），Z=MapHeight+TerrainHeight+Unit.TerrainHeight
        /// </summary>
        public static Vector3D[] mouseVector3DUnitTerrain = new Vector3D[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 鼠标3D点向量坐标，鼠标Z点在悬崖、地形物件顶部，Z=MapHeight+TerrainHeight
        /// </summary>
        public static Vector3D[] mouseVector3DTerrain = new Vector3D[MMCore.c_maxPlayers + 1];

        /// <summary>
        /// 鼠标2D点向量坐标
        /// </summary>
        public static Vector[] mouseVector = new Vector[MMCore.c_maxPlayers + 1];

        #endregion

        #region 键鼠委托

        public static void KeyDown(int player, int key)
        {
            if (MMCore.stopKeyMouseEvent[player] == false)
            {
                keyDown[player, key] = true;  //当前按键值（决定内部函数运行状态）
                keyDownState[player, key] = true;  //当前按键状态值
                //---------------------------------------------------------------------
                keyDownLoopOneBitNum[player] += 1; //玩家当前注册的按键队列数量
                MMCore.DataTableSave2(true, "KeyDownLoopOneBit", player, keyDownLoopOneBitNum[player], key);
                //↑存储玩家注册序号对应按键队列键位
                MMCore.DataTableSave2(true, "KeyDownLoopOneBitKey", player, key, "true"); //玩家按键队列键位状态
                //---------------------------------------------------------------------蓄力管理
                // if (XuLiGuanLi == true){
                // libBC0D3AAD_gf_HD_RegKXL(key, "IntGroup_XuLi" + IntToString(player)); //HD_注册蓄力按键
                // libBC0D3AAD_gf_HD_SetKeyFixedXL(player, key, 1.0);
                // }
                //---------------------------------------------------------------------双击管理
                // if (ShuangJiGuanLi == true){
                //     lv_a = libBC0D3AAD_gf_HD_ReturnKeyFixedSJ(player, key);
                //     if ((0.0 < lv_a) && (lv_a <= ShuangJiShiXian)){
                //         //符合双击标准，发送事件
                //         libBC0D3AAD_gf_Send_KeyDoubleClicked(player, key, ShuangJiShiXian - lv_a);
                //     } 
                //     else {   
                //         libBC0D3AAD_gf_HD_RegKSJ(key, "IntGroup_DoubleClicked" + IntToString(player)); //HD_注册按键
                //         libBC0D3AAD_gf_HD_SetKeyFixedSJ(player, key, ShuangJiShiXian);
                //     }
                // }
                //---------------------------------------------------------------------
                KeyDownFunc(player, key);
            }
        }

        public static bool KeyDownFunc(int player, int key)
        {
            bool torf = true;

            // Console.WriteLine((IntToString(key) + "键被按下"));
            for (int i = 1; i <= 1; i += 1)
            {
                if (MMCore.stopKeyMouseEvent[player] == true)
                {
                    //由于按键时状态为真，阻止按键事件时，强制取消按键状态（延迟弹起成功也会自动置为false）
                    keyDown[player, key] = false;
                    torf = false;
                    break;
                }
                else
                {
                    MMCore.KeyDownGlobalEvent(key, true, player);
                    break;
                }
            }
            return torf;
        }

        public static void KeyUp(int player, int key)
        {
            keyDownState[player, key] = false;  //当前按键状态
            if ((MMCore.DataTableLoad2(true, "KeyDownLoopOneBitKey", player, key).ToString() == "true"))
            {
                //弹起时无按键队列（由延迟弹起清空造成），直接执行函数，清空按键状态
                keyDown[player, key] = false;
                KeyUpFunc(player, key);
            }
            else
            {
                //弹起时有按键队列，由延迟弹起管理运行（按键队列>0时，清空一次队列并执行它们的动作）
                MMCore.DataTableSave2(true, "KeyDownLoopOneBitEnd", player, key, true);
            }
        }

        public static bool KeyUpFunc(int player, int key)
        {
            bool torf = true;
            // Console.WriteLine((IntToString(key) + "键弹起"));
            for (int i = 1; i <= 1; i += 1)
            {
                if (MMCore.stopKeyMouseEvent[player] == true)
                {
                    torf = false;
                    break;
                }
                else
                {
                    MMCore.KeyDownGlobalEvent(key, false, player);
                    break;
                }
            }
            return torf;
        }

        #endregion

    }

    #endregion

    #region 键盘钩子类

    /// <summary>
    /// 键盘钩子
    /// </summary>
    public class KeyboardHook
    {
        //声明事件委托类型
        public event KeyEventHandler KeyDownEvent;
        public event KeyPressEventHandler KeyPressEvent;
        public event KeyEventHandler KeyUpEvent;

        //声明常规委托类型（类型名：钩子，拥有三个参数）
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        //声明键盘钩子处理的初始值（值在Microsoft SDK的Winuser.h里查询）
        static int hKeyboardHook = 0;
        //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        public const int WH_KEYBOARD_LL = 13;
        //声明HookProc委托类型的变量KeyboardHookProcedure
        HookProc KeyboardHookProcedure;
        //键盘结构
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;//定义一个虚拟键码，键码值必须在1～254之间
            public int scanCode;//定义该键的硬件扫描码，一般置0即可
            public int flags;//定义函数操作的各个方面的一个标志位集，应用程序可使用如下一些预定义常数的组合设置标志位
            public int time;//指定的时间戳记的这个讯息
            public int dwExtraInfo;//指定额外信息相关的信息
        }
        //虚拟键代码查询见 https://learn.microsoft.com/zh-cn/windows/win32/inputdev/virtual-key-codes

        //调用此函数安装钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        //使用此功能，通过信息钩子继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        //取得当前线程编号（线程钩子需要用到）
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        public void Start()
        {
            //安装键盘钩子开始
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                //************************************
                //键盘线程钩子

                //指定要监听的线程idGetCurrentThreadId(),
                SetWindowsHookEx(13, KeyboardHookProcedure, IntPtr.Zero, GetCurrentThreadId());
                //键盘全局钩子,需要引用空间(using System.Reflection;)
                //SetWindowsHookEx( 13,MouseHookProcedure,Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),0);
                //
                //关于SetWindowsHookEx (int idHook, HookProc lpfn, IntPtr hInstance, int threadId)函数将钩子加入到钩子链表中，说明一下四个参数：
                //idHook 钩子类型，即确定钩子监听何种消息，上面的代码中设为2，即监听键盘消息并且是线程钩子，如果是全局钩子监听键盘消息应设为13，
                //线程钩子监听鼠标消息设为7，全局钩子监听鼠标消息设为14。lpfn 钩子子程的地址指针。如果dwThreadId参数为0 或是一个由别的进程创建的
                //线程的标识，lpfn必须指向DLL中的钩子子程。 除此以外，lpfn可以指向当前进程的一段钩子子程代码。钩子函数的入口地址，当钩子钩到任何
                //消息后便调用这个函数。hInstance应用程序实例的句柄。标识包含lpfn所指的子程的DLL。如果threadId 标识当前进程创建的一个线程，而且子
                //程代码位于当前进程，hInstance必须为NULL。可以很简单的设定其为本应用程序的实例句柄。threaded 与安装的钩子子程相关联的线程的标识符
                //如果为0，钩子子程与所有的线程关联，即为全局钩子
                //************************************
                //如果SetWindowsHookEx失败
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }
        public void Stop()
        {
            bool retKeyboard = true;


            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            if (!(retKeyboard)) throw new Exception("卸载钩子失败！");
        }
        //ToAscii职能的转换指定的虚拟键码和键盘状态的相应字符或字符
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, //[in] 指定虚拟关键代码进行翻译。
                                         int uScanCode, // [in] 指定的硬件扫描码的关键须翻译成英文。高阶位的这个值设定的关键，如果是（不压）
                                         byte[] lpbKeyState, // [in] 指针，以256字节数组，包含当前键盘的状态。每个元素（字节）的数组包含状态的一个关键。如果高阶位的字节是一套，关键是下跌（按下）。在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。在切换状态的NUM个锁和滚动锁定键被忽略。
                                         byte[] lpwTransKey, // [out] 指针的缓冲区收到翻译字符或字符。
                                         int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.

        //获取按键的状态
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 侦听键盘事件
            if ((nCode >= 0) && (KeyDownEvent != null || KeyUpEvent != null || KeyPressEvent != null))
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                // raise KeyDown
                if (KeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyDownEvent(this, e);
                }

                //键盘按下
                if (KeyPressEvent != null && wParam == WM_KEYDOWN)
                {
                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                    {
                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                        KeyPressEvent(this, e);
                    }
                }

                // 键盘抬起
                if (KeyUpEvent != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyUpEvent(this, e);
                }

            }
            //如果返回1，则结束消息，这个消息到此为止，不再传递。
            //如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        //析构函数
        ~KeyboardHook()
        {
            Stop();
        }
    }

    #endregion

    #region 键盘钩子类使用示范

    /// <summary>
    /// 按键测试
    /// </summary>
    public class KeyUpDown
    {
        //创建实例
        KeyboardHook keyhook = new KeyboardHook();
        KeyEventHandler myKeyEventHandeler;

        //要运行的按键事件委托函数
        private void KeyDown(object sender, KeyEventArgs e)
        {
            //如果按下截图键做某某事
            if (e.KeyCode.Equals(Keys.PrintScreen))
            {
                //这里写具体实现
            }
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void StartListen()
        {
            myKeyEventHandeler = new KeyEventHandler(KeyDown);
            keyhook.KeyDownEvent += myKeyEventHandeler;//钩住键按下
            keyhook.Start();//安装键盘钩子
        }

        /// <summary>
        /// 结束监听
        /// </summary>
        public void StopListen()
        {
            if (myKeyEventHandeler != null)
            {
                keyhook.KeyDownEvent -= myKeyEventHandeler;//取消按键事件
                myKeyEventHandeler = null;
                keyhook.Stop();//关闭键盘钩子
            }
        }
    }

    #endregion

}

