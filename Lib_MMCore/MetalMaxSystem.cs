﻿#region 序言

//--------------------------------------------------------------------------------------------------
//MetalMaxSystem.FuncLib（MM_函数库）
//Code By Prinny（蔚蓝星海）
//Github：https://github.com/linsh460997396/Lib_MMProgram.git
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
using Point = System.Windows.Point;
using static MetalMaxSystem.MouseHook;
using static MetalMaxSystem.KeyboardHook;

#endregion

/// <summary>
/// MetalMax系统引用空间
/// </summary>
namespace MetalMaxSystem
{
    #region 枚举存放区

    /// <summary>
    /// 【MetalMaxSystem】主副循环的入口索引
    /// </summary>
    public enum Entry
    {
        /// <summary>
        /// 主循环唤醒阶段
        /// </summary>
        MainAwake,
        /// <summary>
        /// 主循环开始阶段
        /// </summary>
        MainStart,
        /// <summary>
        /// 主循环Update阶段
        /// </summary>
        MainUpdate,
        /// <summary>
        /// 主循环结束阶段
        /// </summary>
        MainEnd,
        /// <summary>
        /// 主循环摧毁阶段
        /// </summary>
        MainDestroy,
        /// <summary>
        /// 副循环唤醒阶段
        /// </summary>
        SubAwake,
        /// <summary>
        /// 副循环开始阶段
        /// </summary>
        SubStart,
        /// <summary>
        /// 副循环Update阶段
        /// </summary>
        SubUpdate,
        /// <summary>
        /// 副循环结束阶段
        /// </summary>
        SubEnd,
        /// <summary>
        /// 副循环摧毁阶段
        /// </summary>
        SubDestroy,
    }

    #endregion

    #region 结构存放区

    //暂无

    #endregion

    #region 委托类型

    //个人书写习惯↓
    //声明的委托类型首字母大写、委托类型变量（执行函数）首字母大写；
    //结尾Funcref 表示无事件event记号的委托类型（不安全使用）
    //结尾Handler 表示有事件event记号的委托类型（安全使用）

    /// <summary>
    /// 键鼠常规函数引用（委托类型），特征：void KeyMouseEventFuncref(bool ifKeyDown, int player)
    /// </summary>
    /// <param name="ifKeyDown"></param>
    /// <param name="player"></param>
    public delegate void KeyMouseEventFuncref(bool ifKeyDown, int player);

    /// <summary>
    /// 主副循环入口常规函数引用（委托类型），特征：void EntryEventFuncref()
    /// </summary>
    public delegate void EntryEventFuncref();

    /// <summary>
    /// 计时器事件函数引用（委托类型），特征：void TimerEventHandler(object sender, EventArgs e)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TimerEventHandler(object sender, EventArgs e);

    /// <summary>
    /// 动作集合常规函数引用（委托类型），特征：void SubActionEventFuncref(int lp_var)
    /// </summary>
    public delegate void SubActionEventFuncref(object sender);

    #endregion

    #region 类

    //静态类数据在内存中唯一（从模板也只产生一个可修改副本），公开的静态类可供其他程序集调用。
    //类的访问修饰符只有public和internal，其中internal修饰后只能在自身程序集（dll或exe）使用，类中创建类默认为internal（内部类）。
    //前缀加partial（分部类型）用来定义要拆分到多个文件中的类，亦称"部分类"
    //提供给程序集外或用户无限制使用的类及成员（字段、方法）标public，不让外部操作或需隐藏则不标
    //若需额外让派生类（子类）使用则标protected，会限制在基类派生类（父子类）中，要注意其结合privite（修饰成员）、internal（修饰类）时反而是扩大使用范围
    //静态数据的副本创建后只有在程序结束才会清理

    /// <summary>
    /// 【MetalMaxSystem】核心类
    /// </summary>
    public static class MMCore
    {
        #region 常量

        //键盘按键映射

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

        //鼠标按键映射

        public const int c_mouseButtonNone = 0;
        public const int c_mouseButtonLeft = 1;
        public const int c_mouseButtonMiddle = 2;
        public const int c_mouseButtonRight = 3;
        public const int c_mouseButtonXButton1 = 4;
        public const int c_mouseButtonXButton2 = 5;

        //其他常量

        public const int c_vehicleTypeMax = 200;

        //键鼠函数引用上限及单键注册上限

        /// <summary>
        /// 【MM_函数库】键盘按键句柄上限（句柄范围0~98，无按键-1）
        /// </summary>
        public const int c_keyMax = 98;
        /// <summary>
        /// 【MM_函数库】每个键盘按键可注册函数上限
        /// </summary>
        public const int c_regKeyMax = 8;
        /// <summary>
        /// 【MM_函数库】鼠标按键句柄上限（句柄范围1~5，无按键0）
        /// </summary>
        public const int c_mouseMax = 5;
        /// <summary>
        /// 【MM_函数库】每个鼠标按键可注册函数上限
        /// </summary>
        public const int c_regMouseMax = 24;

        //主副循环入口函数引用上限及单入口注册上限

        /// <summary>
        /// 【MM_函数库】主副循环入口句柄上限（句柄范围0~9）
        /// </summary>
        private const int c_entryMax = 9;//内部使用，无需给用户使用
        /// <summary>
        /// 【MM_函数库】每个主副循环入口可注册函数上限
        /// </summary>
        private const int c_regEntryMax = 1;//内部使用，无需给用户使用

        //玩家句柄及其上限

        /// <summary>
        /// 【MM_函数库】玩家句柄（0默认中立玩家，1用户本人，2-14玩家（电脑或其他用户），15默认敌对玩家，16由系统触发，活动玩家=用户+电脑（不含中立））
        /// </summary>
        public const int c_playerAny = 16;
        /// <summary>
        /// 【MM_函数库】玩家句柄上限（限制最大玩家数，玩家句柄从0-15共16个，16是上帝（由系统执行））
        /// </summary>
        public const int c_maxPlayers = 16;

        #endregion

        #region 全局和局部"变量"（无属性字段）

        //类只有字段没变量，但理论上公有静态字段是该程序在内存中唯一的全局变量，无论类实例化多次或多线程从模板调用，它只生成一次副本直到程序结束才清理
        //而非静态（实例）类每次实例化都复制一份模板去形成多个副本，私有实例字段相当于类的局部变量
        //不标Static则类及其成员在结束时垃圾回收，标Static则副本唯一且程序结束才从内存消失
        //静态局部变量在函数结束时不参与垃圾回收，以便相同函数重复访问
        //静态数据是从模板形成的内存中唯一的可修改副本（不同类同名也不一样，要考虑命名空间和类名路径，无需担心重复）
        //数组元素数量上限均+1是习惯问题，防止某些循环以数组判断时最后退出还+1导致超限

        /// <summary>
        /// 【MM_函数库】键盘按键已注册数量（每个数组元素算1个，即使它们+=多个委托函数）
        /// </summary>
        private static int[] keyEventFuncrefGroupNum = new int[c_keyMax + 1];//内部使用
        /// <summary>
        /// 【MM_函数库】鼠标按键的已注册数量（每个数组元素算1个，即使它们+=多个委托函数）
        /// </summary>
        private static int[] mouseEventFuncrefGroupNum = new int[c_mouseMax + 1];//内部使用
        /// <summary>
        /// 【MM_函数库】用户按键事件禁用状态（用于过场、剧情对话、特殊技能如禁锢时强制停用用户的按键事件）
        /// </summary>
        public static bool[] stopKeyMouseEvent = new bool[c_maxPlayers + 1];
        /// <summary>
        /// 【MM_函数库】主副循环每个入口的已注册数量（每个数组元素算1个，即使它们+=多个委托函数）
        /// </summary>
        private static int[] entryEventFuncrefGroupNum = new int[c_entryMax + 1];//内部使用
        /// <summary>
        /// 【MM_函数库】主副循环事件禁用状态（用于特殊情况如个人处理队列过多、玩家间未同步时间过长情况下停用用户主副循环事件）
        /// </summary>
        public static bool[] stopEntryEvent = new bool[c_maxPlayers + 1];

        /// <summary>
        /// 【MM_函数库】主循环线程
        /// </summary>
        private static Thread mainUpdateThread;
        /// <summary>
        /// 【MM_函数库】副循环线程
        /// </summary>
        private static Thread subUpdateThread;

        /// <summary>
        /// 【MM_函数库】全局数据表（不排泄，直到程序结束）
        /// </summary>
        private static Hashtable systemDataTable = new Hashtable();//内部使用
        /// <summary>
        /// 【MM_函数库】局部数据表（函数或动作集结束时应手动排泄）
        /// </summary>
        private static Hashtable tempDataTable = new Hashtable();//内部使用

        //声明用于存放键盘、鼠标"按键事件引用类型"委托变量的二维数组集合（单元素也是集合能+=多个委托函数），C#自带委托列表类型能继续存储这些委托类型变量

        /// <summary>
        /// 【MM_函数库】键盘按键事件引用委托类型变量数组[c_keyMax + 1, c_regKeyMax + 1]，用于自定义委托函数注册
        /// </summary>
        private static KeyMouseEventFuncref[,] keyEventFuncrefGroup = new KeyMouseEventFuncref[c_keyMax + 1, c_regKeyMax + 1];//内部使用
        /// <summary>
        /// 【MM_函数库】鼠标按键事件引用委托类型变量数组[c_mouseMax + 1, c_regMouseMax + 1]，用于自定义委托函数注册
        /// </summary>
        private static KeyMouseEventFuncref[,] mouseEventFuncrefGroup = new KeyMouseEventFuncref[c_mouseMax + 1, c_regMouseMax + 1];//内部使用

        //声明用于存放"主副循环入口事件引用类型"委托变量二维数组集合

        /// <summary>
        /// 【MM_函数库】主副循环入口事件引用委托类型变量数组[c_entryMax + 1, c_regEntryMax + 1]，用于自定义委托函数注册
        /// </summary>
        private static EntryEventFuncref[,] entryEventFuncrefGroup = new EntryEventFuncref[c_entryMax + 1, c_regEntryMax + 1];//内部使用

        #endregion

        #region 字段及其属性方法

        //字段及其属性方法（避免不安全读写，private保护和隐藏字段，设计成只允许通过public修饰的属性方法间接去安全读写）

        private static int _directoryEmptyUserDefIndex = 0;
        /// <summary>
        /// 【MM_函数库】用户定义的空文件夹形式，以供内部判断：0是子文件（夹）数量为0，1是文件夹大小为0，2是前两者必须都符合，如果用户输入错误，本属性方法将纠正为默认值0
        /// </summary>
        public static int DirectoryEmptyUserDefIndex
        {
            get => _directoryEmptyUserDefIndex;
            //如果用户输入错误，纠正为默认值0
            set
            {
                if (value >= 0 && value <= 2)
                {
                    _directoryEmptyUserDefIndex = value;
                }
                else
                {
                    _directoryEmptyUserDefIndex = 0;
                }
            }
        }

        private static AutoResetEvent _autoResetEvent_MainUpdate;
        /// <summary>
        /// 【MM_函数库】主循环自动复位事件对象（用来向主循环线程发送信号），属性动作AutoResetEvent_MainUpdate.Set()可让触发器线程终止（效果等同MMCore.MainUpdateChecker.TimerState = true）
        /// </summary>
        public static AutoResetEvent AutoResetEvent_MainUpdate { get => _autoResetEvent_MainUpdate; }//不提供外部赋值

        private static AutoResetEvent _autoResetEvent_SubUpdate;
        /// <summary>
        /// 【MM_函数库】副循环自动复位事件对象（用来向主循环线程发送信号），属性动作AutoResetEvent_SubUpdate.Set()可让触发器线程终止（效果等同MMCore.SubUpdateChecker.TimerState = true）
        /// </summary>
        public static AutoResetEvent AutoResetEvent_SubUpdate { get => _autoResetEvent_SubUpdate; }//不提供外部赋值

        private static Timer _mainUpdateTimer, _subUpdateTimer;
        /// <summary>
        /// 【MM_函数库】主循环Update阶段，用来实现周期循环的计时器
        /// </summary>
        public static Timer MainUpdateTimer { get => _mainUpdateTimer; }//不提供外部赋值
        /// <summary>
        /// 【MM_函数库】主循环Update阶段，用来实现周期循环的计时器
        /// </summary>
        public static Timer SubUpdateTimer { get => _subUpdateTimer; }//不提供外部赋值

        private static int _mainUpdateDuetime, _mainUpdatePeriod, _subUpdateDuetime, _subUpdatePeriod;
        /// <summary>
        /// 【MM_函数库】主循环Update阶段前摇时间，设置后每次循环前都会等待
        /// </summary>
        public static int MainUpdateDuetime { get => _mainUpdateDuetime; set => _mainUpdateDuetime = value; }
        /// <summary>
        /// 【MM_函数库】主循环Update阶段间隔运行时间
        /// </summary>
        public static int MainUpdatePeriod { get => _mainUpdatePeriod; set => _mainUpdatePeriod = value; }
        /// <summary>
        /// 【MM_函数库】副循环Update阶段前摇时间，设置后每次循环前都会等待
        /// </summary>
        public static int SubUpdateDuetime { get => _subUpdateDuetime; set => _subUpdateDuetime = value; }
        /// <summary>
        /// 【MM_函数库】副循环Update阶段间隔运行时间
        /// </summary>
        public static int SubUpdatePeriod { get => _subUpdatePeriod; set => _subUpdatePeriod = value; }

        //地图相关字段↓

        private static double _mapHeight;
        private static double[,] _terrainHeight = new double[2560 + 1, 2560 + 1];
        private static double[,,] _terrainType;

        /// <summary>
        /// 【MM_函数库】地图首个纹理图层顶面高度，默认值=8（m），亦称地面高度或地图高度
        /// </summary>
        public static double MapHeight { get => _mapHeight; set => _mapHeight = value; }

        /// <summary>
        /// 【MM_函数库】地面上附加的悬崖、地形物件的高度，二维坐标数组元素[2560+1,2560+1]（设计精度0.1m，按256m计）
        /// </summary>
        public static double[,] TerrainHeight { get => _terrainHeight; set => _terrainHeight = value; }

        /// <summary>
        /// 【MM_函数库】土、矿、水、气等空间内每个点的属性类型和数量（密度），数组元素[2560+1,2560+1,2560+1]，设计精度0.1m，小数点左侧表示土的类型，右侧为数值（密度）
        /// </summary>
        public static double[,,] TerrainType { get => _terrainType; set => _terrainType = value; }

        #endregion

        #region Functions 数学公式

        /// <summary>
        /// 【MM_函数库】随机整数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(min, max);
        }

        /// <summary>
        /// 【MM_函数库】将Vector3D转Vector（去掉Z轴）
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector ToVector(Vector3D vector)
        {
            return new Vector(vector.X, vector.Y);
        }

        /// <summary>
        ///  【MM_函数库】以实数返回二维坐标（x,y）与（a,b）形成的角度（单位：度）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double AngleBetween(double x, double y, double a, double b)
        {
            return Vector.AngleBetween(new Vector(x, y), new Vector(a, b));
        }

        /// <summary>
        /// 【MM_函数库】以实数返回三维坐标（x,y,z）与（a,b,c）形成的角度（单位：度）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double AngleBetween(double x, double y, double z, double a, double b, double c)
        {
            return Vector3D.AngleBetween(new Vector3D(x, y, z), new Vector3D(a, b, z));
        }

        /// <summary>
        /// 【MM_函数库】以实数返回二维点1点2形成的角度（单位：度）
        /// Returns the angle from point 1 to point 2 as a real value, in degrees
        /// </summary>
        /// <param name="point1">二维点</param>
        /// <param name="point2">二维点</param>
        /// <returns></returns>
        public static double AngleBetween(Point point1, Point point2)
        {
            //double X1 = point1.X, Y1 = point1.Y, X2 = point2.Y, Y2 = point2.Y;
            //double angleOfLine = Math.Atan2((Y2 - Y1), (X2 - X2)) * 180 / Math.PI;
            return Vector.AngleBetween(new Vector(point1.X, point1.Y), new Vector(point2.X, point2.Y));
        }

        /// <summary>
        /// 【MM_函数库】以实数返回三维点1点2形成的角度（单位：度）
        /// </summary>
        /// <param name="point1">三维点</param>
        /// <param name="point2">三维点</param>
        /// <returns></returns>
        public static double AngleBetween(Point3D point1, Point3D point2)
        {
            return Vector3D.AngleBetween(new Vector3D(point1.X, point1.Y, point1.Z), new Vector3D(point2.X, point2.Y, point2.Z));
        }

        /// <summary>
        /// 【MM_函数库】以实数返回二维向量之间形成的角度（单位：度）
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            return Vector.AngleBetween(vector1, vector2);
        }

        /// <summary>
        /// 【MM_函数库】以实数返回三维向量之间形成的角度（单位：度）
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static double AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            return Vector3D.AngleBetween(vector1, vector2);
        }

        /// <summary>
        /// 【MM_函数库】以实数返回二维坐标（x,y）与（a,b）形成的距离（单位：m）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Distance(double x, double y, double a, double b)
        {
            double x1 = x;
            double y1 = y;

            double x2 = a;
            double y2 = b;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
            return result;
        }

        /// <summary>
        /// 【MM_函数库】以实数返回二维点之间形成的距离（单位：m）
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double Distance(Point point1, Point point2)
        {
            double x1 = point1.X;
            double y1 = point1.Y;

            double x2 = point2.X;
            double y2 = point2.Y;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
            return result;
        }

        /// <summary>
        /// 【MM_函数库】以实数返回二维向量之间形成的距离（单位：m）
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static double Distance(Vector vector1, Vector vector2)
        {
            double x1 = vector1.X;
            double y1 = vector1.Y;

            double x2 = vector2.X;
            double y2 = vector2.Y;

            double result = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
            return result;
        }

        /// <summary>
        /// 【MM_函数库】以实数返回三维坐标（x,y,z）与（a,b,c）形成的距离（单位：度）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 【MM_函数库】以实数返回三维点之间形成的距离（单位：m）
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 【MM_函数库】以实数返回三维向量之间形成的距离（单位：m）
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
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

        #region Functions 通用功能

        #region 判断文件是否被占用

        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        /// <summary>
        /// 文件是否被占用（WIN32 API调用）
        /// </summary>
        /// <param name="fileFullNmae"></param>
        /// <returns></returns>
        public static bool IsOccupied(string fileFullNmae)
        {
            if (!File.Exists(fileFullNmae)) return false;
            IntPtr vHandle = _lopen(fileFullNmae, OF_READWRITE | OF_SHARE_DENY_NONE);
            var flag = vHandle == HFILE_ERROR;
            CloseHandle(vHandle);
            return flag;
        }

        /// <summary>
        /// 文件是否被占用（文件流判断法）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>true表示正在使用,false没有使用 </returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;
            }
            catch { }
            finally
            {
                if (fs != null) fs.Dispose();
            }
            return inUse;
        }

        #endregion

        /// <summary>
        /// 【MM_函数库】递归方式强制删除文件夹（进最里层删除文件使文件夹为空后删除这个空文件夹，层层递出时重复动作），删除前会去掉文件（夹）的Archive、ReadOnly、Hidden属性以确保删除
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
        /// 【MM_函数库】递归方式强制删除文件夹（进最里层删除文件使文件夹为空后删除这个空文件夹，层层递出时重复动作），删除前会去掉文件（夹）的Archive、ReadOnly、Hidden属性以确保删除
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
        /// 【MM_函数库】删除文件夹
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
        /// 【MM_函数库】删除文件夹
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
        /// 【MM_函数库】删除文件到回收站功能专用属性，已添加Shell API特性[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        /// </summary>
        /// <param name="lpFileOp"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        /// <summary>
        /// 【MM_函数库】删除文件到回收站功能专用结构体，已添加特性[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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
        /// 【MM_函数库】删除文件到回收站功能专用枚举，已添加特性[Flags]
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
        /// 【MM_函数库】删除文件到回收站
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
        /// 【MM_函数库】删除文件夹到回收站
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
        /// 【MM_函数库】将字节大小转字符串Byte、KB、MB、GB、TB、PB、EB、ZB、YB、NB形式
        /// </summary>
        /// <param name="Size">字节大小</param>
        /// <param name="Byte">true=强制输出字节单位</param>
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
        /// 【MM_函数库】将字节大小转字符串Byte、KB、MB、GB、TB、PB、EB、ZB、YB、NB形式
        /// </summary>
        /// <param name="Size">字节大小</param>
        /// <param name="Byte">true=强制输出字节单位</param>
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
        /// 【MM_函数库】为字符串str每隔every位添加symbol
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
        /// 【MM_函数库】获取文件大小
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
        /// 【MM_函数库】获取文件大小
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
        /// 【MM_函数库】递归方法获取文件夹大小
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
        /// 【MM_函数库】取得设备硬盘的卷序列号
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
        /// 【MM_函数库】验证字符串是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            Regex reg1 = new Regex(@"^[0-9]\d*$");
            return reg1.IsMatch(str);
        }

        /// <summary>
        /// 【MM_函数库】验证字符串是否为合法文件（夹）名称，可以是虚拟路径（本函数不验证其真实存在）
        /// </summary>
        /// <param name="path">文件（夹）路径全名</param>
        /// <returns></returns>
        public static bool IsDFPath(string path)
        {
            Regex regex = new Regex(
                @"^([a-zA-Z]:\\)([-\u4e00-\u9fa5\w\s.（）【】：~!@#$%^&()\[\]{}+=]+\\?)*$"
            );
            Match result = regex.Match(path);
            return result.Success;
        }

        /// <summary>
        /// 【MM_函数库】验证字符串路径的文件（夹）是否真实存在
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
        /// 【MM_函数库】判断目标属性是否为真实文件夹
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
        /// 【MM_函数库】验证字符串路径的文件夹是否真实存在
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
        /// 【MM_函数库】验证字符串路径的文件是否真实存在
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
        /// 【MM_函数库】验证目录是否为空
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
        /// 【MM_函数库】验证路径是否为用户定义的空文件夹，通过MMCore.DirectoryEmptyUserDefIndex属性可定义空文件夹形式
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
        /// 【MM_函数库】写文本每行，文件若不存在则自动新建
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
        /// 【MM_函数库】验证文件大小是否在用户定义的[a,b]范围
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
        /// 【MM_函数库】创建GET请求
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
        /// 【MM_函数库】创建POST请求
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
        /// 【MM_函数库】下载指定网站的指定节点内容到指定文件夹并保存为自定义文件名
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
        ///【MM_函数库】生成随机字符串 
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
        /// 【MM_函数库】创建文件夹，若已存在则什么也不干
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
        /// 【MM_函数库】创建文件，若已存在则什么也不干
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
        /// 【MM_函数库】用WinRAR解压带密码的压缩包
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
        /// 【MM_函数库】判断系统上是否安装WinRAR
        /// </summary>
        /// <returns></returns>
        public static bool IsOwnWinRAR()
        {
            RegistryKey the_Reg =
                Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
            return !string.IsNullOrEmpty(the_Reg.GetValue("").ToString());

        }

        #region 弹幕爬取

        //功能出处：https://blog.csdn.net/qq_15505341/article/details/79212070/

        /// <summary>
        /// 获取弹幕信息（本函数待改中请勿使用）
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static string Post(string room)
        {
            string postString = "roomid=" + room + "&token=&csrf_token=我是图中的马赛克";//要发送的数据
            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码，尤其是汉字，事先要看下抓取网页的编码方式  
            string url = @"http://api.live.bilibili.com/ajax/msg";//地址  

            WebClient webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            webClient.Headers.Add("Cookie",
                "可耻的马赛克"
                );
            byte[] responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流  
            string srcString = Encoding.UTF8.GetString(responseData);//解码  
            return srcString;
        }

        /// <summary>
        /// 处理弹幕信息为中文（本函数待改中请勿使用）
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static List<string> GetDanMu(string room)
        {
            string danmu = Post(room);
            List<string> list = new List<string>();
            //正则匹配
            foreach (Match item in Regex.Matches(danmu, "text\":\".*?\""))
            {
                //截取字符串，将unicode码转换为中文
                list.Add(Regex.Unescape(item.Value.Substring(7, item.Value.Length - 8)));
            }
            return list;
        }


        #endregion

        #endregion

        #region Functions 数据表功能

        /// <summary>
        /// 【MM_函数库】添加数据表键值对（重复添加则覆盖）
        /// </summary>
        /// <param name="place">true=全局，false=局部</param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        private static void DataTableSet(bool place, string key, object val)//内部使用
        {
            if (place)
            {
                //存入全局数据表
                if (systemDataTable.Contains(key)) { systemDataTable.Remove(key); }
                systemDataTable.Add(key, val);
            }
            else
            {
                //存入局部数据表
                if (tempDataTable.Contains(key)) { tempDataTable.Remove(key); }
                tempDataTable.Add(key, val);
            }
        }

        /// <summary>
        /// 【MM_函数库】判断数据表键是否存在
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
        /// 【MM_函数库】获取数据表键对应的值
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
        /// 【MM_函数库】从数据表中清除Key
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        public static void DataTableClear0(bool place, string key)
        {
            DataTableRemove(place, key);
        }

        /// <summary>
        /// 【MM_函数库】从数据表中清除Key[]，模拟1维数组
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="lp_1"></param>
        public static void DataTableClear1(bool place, string key, int lp_1)
        {
            DataTableRemove(place, (key + "_" + lp_1.ToString()));
        }

        /// <summary>
        /// 【MM_函数库】从数据表中清除Key[,]，模拟2维数组
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
        /// 【MM_函数库】从数据表中清除Key[,,]，模拟3维数组
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
        /// 【MM_函数库】从数据表中清除Key[,,,]，模拟4维数组
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
        /// 【MM_函数库】移除数据表键值对
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        private static void DataTableRemove(bool place, string key)//内部函数
        {
            if (place) { systemDataTable.Remove(key); }
            else { tempDataTable.Remove(key); }
        }

        /// <summary>
        /// 【MM_函数库】保存数据表键值对
        /// </summary>
        /// <param name="place"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void DataTableSave0(bool place, string key, object val)
        {
            DataTableSet(place, key, val);
        }

        /// <summary>
        /// 【MM_函数库】保存数据表键值对，模拟1维数组
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
        /// 【MM_函数库】保存数据表键值对，模拟2维数组
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
        /// 【MM_函数库】保存数据表键值对，模拟3维数组
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
        /// 【MM_函数库】保存数据表键值对，模拟4维数组
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
        /// 【MM_函数库】读取数据表键值对
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
        /// 【MM_函数库】读取数据表键值对，模拟1维数组
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
        /// 【MM_函数库】读取数据表键值对，模拟2维数组
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
        /// 【MM_函数库】读取数据表键值对，模拟3维数组
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
        /// 【MM_函数库】读取数据表键值对，模拟4维数组
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
        /// 【MM_函数库】存储区容错处理函数，当数据表键值存在时执行线程等待。常用于多线程触发器频繁写值，如大量注册注销动作使存储区数据重排序的，因数据表正在使用需排队等待完成才给执行下一个。执行原理：将调用该函数的当前线程反复挂起50毫秒，直到动作要写入的存储区闲置
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
        /// 【MM_函数库】存储区容错处理函数，当数据表键值存在时执行线程等待。常用于多线程触发器频繁写值，如大量注册注销动作使存储区数据重排序的，因数据表正在使用需排队等待完成才给执行下一个。执行原理：将调用该函数的当前线程反复挂起period毫秒，直到动作要写入的存储区闲置
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
        /// 【MM_函数库】存储区容错处理函数，引发注册注销等存储区频繁重排序的动作，在函数开始/完成写入存储区时，应设置线程等待（val=1）/闲置（val=0）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">函数动作完成，所写入存储区闲置时填"0"，反之填"1"</param>
        private static void ThreadWaitSet(string key, string val)
        {
            DataTableSet(true, "MMCore_ThreadWait_" + key, val);
        }

        #endregion

        #region Functions 键盘、鼠标按键事件函数引用（委托函数），注册注销查询更换归并执行等管理功能

        //------------------------------------↓KeyDownEventStart↓-----------------------------------------

        /// <summary>
        /// 【MM_函数库】将（1个或多个）委托函数注册到键盘按键事件（或者说给委托函数添加指定事件，完成事件注册）。
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
            keyEventFuncrefGroupNum[key] += 1;//注册成功记录+1
            keyEventFuncrefGroup[key, keyEventFuncrefGroupNum[key]] = funcref;//这里采用等于，设计为覆盖
            ThreadWaitSet("MMCore_KeyEventFuncref_", "0");
        }
        /// <summary>
        /// 【MM_函数库】注册指定键盘按键的委托函数（登录在指定注册序号num位置）
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
        /// 【MM_函数库】注销指定键盘按键的委托函数（发生序号重排）
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
        /// 【MM_函数库】返回指定键盘按键注册函数的序号
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
        /// 【MM_函数库】返回指定键盘按键指定函数的注册数量（>1则注册了多个同样的函数）
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
        /// 【MM_函数库】归并键盘按键指定函数（如存在则移除该函数注册并序号重排，之后重新注册1次）
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
        /// 【MM_函数库】全局键盘按键事件，对指定键盘按键执行委托函数动作集合
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
        /// 【MM_函数库】将（1个或多个）委托函数注册到鼠标按键事件（或者说给委托函数添加指定事件，完成事件注册）。
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
            mouseEventFuncrefGroupNum[key] += 1;//注册成功记录+1
            mouseEventFuncrefGroup[key, mouseEventFuncrefGroupNum[key]] = funcref;//这里采用等于，设计为覆盖
            ThreadWaitSet("MouseEventFuncref", "0");
        }

        /// <summary>
        /// 【MM_函数库】注册指定鼠标键位的委托函数（登录在指定注册序号num位置）
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
        /// 【MM_函数库】注销指定鼠标键位的委托函数（发生序号重排）
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
        /// 【MM_函数库】返回指定鼠标键位注册函数的序号
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
        /// 【MM_函数库】返回指定鼠标键位指定注册函数的数量（>1则注册了多个同样的函数）
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
        /// 【MM_函数库】归并鼠标按键指定函数（如存在则移除该函数注册并序号重排，之后重新注册1次）
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
        /// 【MM_函数库】全局鼠标按键事件，对指定鼠标按键执行委托函数动作集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keydown"></param>
        /// <param name="player"></param>
        public static void MouseDownGlobalEvent(int key, bool keydown, int player)
        {
            int a = 1;
            for (; a <= mouseEventFuncrefGroupNum[key]; a += 1)
            {
                //这里不开新线程，是否另开线程运行宜由委托函数去写
                mouseEventFuncrefGroup[key, a](keydown, player);//执行鼠标按键委托
            }
        }

        //------------------------------------↑MouseDownEventEnd↑-----------------------------------------

        #endregion

        #region Functions 主副循环入口事件函数引用（委托函数），注册注销查询更换归并执行等管理功能

        //------------------------------------↓EntryFuncStart↓-----------------------------------------

        /// <summary>
        /// 【MM_函数库】将（1个或多个）委托函数注册到主副循环入口事件（或者说给委托函数添加指定事件，完成事件注册）。
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
            entryEventFuncrefGroupNum[(int)entry] += 1;//注册成功记录+1
            entryEventFuncrefGroup[(int)entry, entryEventFuncrefGroupNum[(int)entry]] = funcref;//这里采用等于，设计为覆盖
            ThreadWaitSet("EntryEventFuncref", "0");
        }

        /// <summary>
        /// 【MM_函数库】注册指定主副循环入口的委托函数（登录在指定注册序号num位置）
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
        /// 【MM_函数库】注销指定主副循环入口的委托函数（发生序号重排）
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
        /// 【MM_函数库】返回指定主副循环入口注册函数的序号
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
        /// 【MM_函数库】返回指定主副循环入口指定函数的注册数量（>1则注册了多个同样的函数）
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
        /// 【MM_函数库】归并主副循环入口指定函数（如存在则移除该函数注册并序号重排，之后重新注册1次）
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
        /// 【MM_函数库】全局主副循环入口事件，对指定入口执行委托函数动作集合
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

        #region 循环体（周期触发器、运行时钟）功能

        /// <summary>
        /// 【MM_函数库】开启主循环（默认0.05现实时间秒，如需修改请在开启前用属性方法MainUpdatePeriod、MainUpdateDuetime来调整计时器Update阶段的间隔、前摇，若已经开启想要修改，可使用MMCore.MainUpdateTimer.Change）
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
        /// 【MM_函数库】开启副循环（默认1.0现实时间秒，如需修改请在开启前用属性方法SubUpdatePeriod、SubUpdateDuetime来调整计时器Update阶段的间隔、前摇，若已经开启想要修改，可使用MMCore.SubUpdateTimer.Change）
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
        /// 【MM_函数库】主循环方法，若Update阶段属性未定义则默认每轮前摇0ms、间隔50ms
        /// </summary>
        private static void MainUpdateFunc()//内部使用
        {
            if (MainUpdateDuetime < 0) { MainUpdateDuetime = 0; }
            if (MainUpdatePeriod <= 0) { MainUpdatePeriod = 50; }
            MainUpdateAction(MainUpdateDuetime, MainUpdatePeriod);
        }

        /// <summary>
        /// 【MM_函数库】副循环方法，若Update阶段属性未定义则默认每轮前摇0ms、间隔50ms
        /// </summary>
        private static void SubUpdateFunc()//内部使用
        {
            if (SubUpdateDuetime < 0) { SubUpdateDuetime = 0; }
            if (SubUpdatePeriod <= 0) { SubUpdatePeriod = 1000; }
            SubUpdateAction(SubUpdateDuetime, SubUpdatePeriod);
        }

        /// <summary>
        /// 【MM_函数库】主循环唤醒阶段运行一次，允许主动调用
        /// </summary>
        public static void MainAwake()
        {
            EntryGlobalEvent(Entry.MainAwake);
        }

        /// <summary>
        /// 【MM_函数库】主循环开始阶段运行一次，允许主动调用
        /// </summary>
        public static void MainStart()
        {
            EntryGlobalEvent(Entry.MainStart);
        }

        /// <summary>
        /// 【MM_函数库】主循环每轮更新运行，主动调用时跟Unity引擎一样只运行一次
        /// </summary>
        public static void MainUpdate()
        {
            EntryGlobalEvent(Entry.MainUpdate);
        }

        /// <summary>
        /// 【MM_函数库】主循环结束阶段运行一次，允许主动调用
        /// </summary>
        public static void MainEnd()
        {
            EntryGlobalEvent(Entry.MainEnd);
        }

        /// <summary>
        /// 【MM_函数库】主循环摧毁阶段运行一次，允许主动调用
        /// </summary>
        public static void MainDestroy()
        {
            EntryGlobalEvent(Entry.MainDestroy);
        }

        /// <summary>
        /// 【MM_函数库】副循环唤醒阶段运行一次，允许主动调用
        /// </summary>
        public static void SubAwake()
        {
            EntryGlobalEvent(Entry.SubAwake);
        }

        /// <summary>
        /// 【MM_函数库】副循环开始阶段运行一次，允许主动调用
        /// </summary>
        public static void SubStart()
        {
            EntryGlobalEvent(Entry.SubStart);
        }

        /// <summary>
        /// 【MM_函数库】副循环每轮更新运行，主动调用时跟Unity引擎一样只运行一次
        /// </summary>
        public static void SubUpdate()
        {
            EntryGlobalEvent(Entry.SubUpdate);
        }

        /// <summary>
        /// 【MM_函数库】副循环结束阶段运行一次，允许主动调用
        /// </summary>
        public static void SubEnd()
        {
            EntryGlobalEvent(Entry.SubEnd);
        }

        /// <summary>
        /// 【MM_函数库】副循环摧毁阶段运行一次，允许主动调用
        /// </summary>
        public static void SubDestroy()
        {
            EntryGlobalEvent(Entry.SubDestroy);
        }

        /// <summary>
        /// 【MM_函数库】主循环主体事件发布动作（重复执行则什么也不做），若Update阶段属性未定义则默认每轮前摇0ms、间隔50ms
        /// </summary>
        private static void MainUpdateAction()//内部使用
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
        /// 【MM_函数库】主循环主体事件发布动作（重复执行则什么也不做），可自定义Update阶段属性Duetime（前摇）、Period（间隔）
        /// </summary>
        /// <param name="duetime">Updata阶段执行开始前等待（毫秒），仅生效一次</param>
        /// <param name="period">Updata阶段执行间隔（毫秒）</param>
        private static void MainUpdateAction(int duetime, int period)//内部使用
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
        /// 【MM_函数库】副循环主体事件发布动作（重复执行则什么也不做），若Update阶段属性未定义则默认每轮前摇0ms、间隔1000ms
        /// </summary>
        private static void SubUpdateAction()//内部使用
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
        /// 【MM_函数库】副循环主体事件发布动作（重复执行则什么也不做），可自定义Update阶段属性Duetime（前摇）、Period（间隔）
        /// </summary>
        /// <param name="duetime">Updata阶段执行开始前等待（毫秒），仅生效一次</param>
        /// <param name="period">Updata阶段执行间隔（毫秒）</param>
        private static void SubUpdateAction(int duetime, int period)//内部使用
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

        #region 互动管理（利用数据表实现不同类型的数据互动及信息管理）

        #region 任意类型

        //提示：可以将任意类型作为模板修改后产生其他类型
        //提示：尽可能使用对口类型，以防值类型与引用类型发生转换时拆装箱降低性能

        //--------------------------------------------------------------------------------------------------
        // 任意类型组Start
        //--------------------------------------------------------------------------------------------------

        /// <summary>
        /// 【MM_函数库】互动O_注册Object标签句柄并返回。为Object自动设置新的标签句柄，重复时会返回已注册的Object标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        private static int HD_RegObjectTagAndReturn_Int(object lp_object)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_ObjectJBNum");
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_ObjectJBNum", lv_j);
                DataTableSave0(true, ("HD_Object_" + lv_j.ToString()), lp_object);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((object)DataTableLoad0(true, ("HD_Object_" + lv_j.ToString())) == lp_object)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_ObjectJBNum", lv_j);
                            DataTableSave0(true, ("HD_Object_" + lv_j.ToString()), lp_object);
                        }
                    }
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object已注册标签句柄。返回一个Object的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        public static int HD_ReturnObjectTag_Int(object lp_object)
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_ObjectJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((object)DataTableLoad0(true, "HD_Object_" + lv_j.ToString()) == lp_object)
                {
                    break;
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动O_注册Object标签句柄并返回。为Object自动设置新的标签句柄，重复时会返回已注册的Object标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        private static string HD_RegObjectTagAndReturn(object lp_object)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_ObjectJBNum");
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_ObjectJBNum", lv_j);
                DataTableSave0(true, ("HD_Object_" + lv_j.ToString()), lp_object);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((object)DataTableLoad0(true, "HD_Object_" + lv_j.ToString()) == lp_object)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_ObjectJBNum", lv_j);
                            DataTableSave0(true, ("HD_Object_" + lv_j.ToString()), lp_object);
                        }
                    }
                }
            }
            lv_tag = lv_j.ToString();
            //Console.WriteLine(("Tag：" + lv_tag));
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object已注册标签句柄。返回一个Object的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        public static string HD_ReturnObjectTag(object lp_object)
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_ObjectJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((object)DataTableLoad0(true, "HD_Object_" + lv_j.ToString()) == lp_object)
                {
                    lv_tag = lv_j.ToString();
                    break;
                }
            }
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动O_注册Object(高级)。在指定Key存入Object，固有状态、自定义值是Object独一无二的标志（本函数重复注册会刷新），之后可用互动O_"返回Object注册总数"、"返回Object序号"、"返回序号对应Object"、"返回序号对应Object标签"、"返回Object自定义值"。Object组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Object组转为Key。首次注册时固有状态为true（相当于单位组单位活体），如需另外设置多个标记可使用"互动O_设定Object状态/自定义值"
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <param name="lp_inherentStats">固有状态</param>
        /// <param name="lp_inherentCustomValue">固有自定义值</param>
        public static void HD_RegObject(object lp_object, string lp_key, string lp_inherentStats, string lp_inherentCustomValue)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;

            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegObjectTagAndReturn(lp_object);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfObjectTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfObjectTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if (DataTableLoad1(true, lv_str + "Tag", lv_i).ToString() == lv_tagStr)
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfObjectTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfObjectTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            DataTableSave0(true, ("HD_ObjectState" + "" + "_" + lv_tagStr), lp_inherentStats);
            DataTableSave0(true, ("HD_ObjectCV" + "" + "_" + lv_tagStr), lp_inherentCustomValue);
        }

        /// <summary>
        /// 【MM_函数库】互动O_注册Object。在指定Key存入Object，固有状态、自定义值是Object独一无二的标志（本函数重复注册不会刷新），之后可用互动O_"返回Object注册总数"、"返回Object序号"、"返回序号对应Object"、"返回Object自定义值"。Object组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Object组转为Key。首次注册时固有状态为true（相当于单位组单位活体），之后只能通过"互动O_注册Object（高级）"改写，如需另外设置多个标记可使用"互动O_设定Object状态/自定义值"
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        public static void HD_RegObject_Simple(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;

            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegObjectTagAndReturn(lp_object);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfObjectTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfObjectTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if (DataTableLoad1(true, lv_str + "Tag", lv_i).ToString() == lv_tagStr)
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfObjectTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfObjectTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            if ((DataTableKeyExists(true, ("HD_Object" + "State" + "_" + lv_tag.ToString())) == false))
            {
                DataTableSave1(true, (("HD_Object" + "State")), lv_tag, "true");
            }
        }

        /// <summary>
        /// 【MM_函数库】互动O_注销Object。用"互动O_注册Object"到Key，之后可用本函数彻底摧毁注册信息并将序号重排（包括Object标签有效状态、固有状态及自定义值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动O_设定Object状态"让Object状态失效（类似单位组的单位活体状态）。Object组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Object组转为Key。本函数无法摧毁用"互动O_设定Object状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Object组变量ID时会清空Object组专用状态
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        public static void HD_DestroyObject(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag;
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_ObjectGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "ObjectTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfObjectTag_" + lv_tag);
                        DataTableClear0(true, "HD_IfObjectTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_Object_" + lv_tag);
                        DataTableClear0(true, "HD_ObjectCV_" + lv_tag);
                        DataTableClear0(true, "HD_ObjectState_" + lv_tag);
                        DataTableClear0(true, "HD_ObjectCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_ObjectState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "ObjectNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "ObjectTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "ObjectTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_ObjectGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动O_移除Object。用"互动O_注册Object"到Key，之后可用本函数仅摧毁Key区注册的信息并将序号重排，用于Object组或多个键区仅移除Object（保留Object标签有效状态、固有值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动O_设定Object状态"让Object状态失效（类似单位组的单位活体状态）。Object组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Object组转为Key。本函数无法摧毁用"互动O_设定Object状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Object组变量ID时会清空Object组专用状态
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        public static void HD_RemoveObject(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag;
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_ObjectGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "ObjectTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfObjectTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_ObjectCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_ObjectState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "ObjectNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "ObjectTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "ObjectTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_ObjectGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object注册总数。必须先使用"互动O_注册Object"才能返回指定Key里的注册总数。Object组使用时，可用"获取变量的内部名称"将Object组转为Key。
        /// </summary>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <returns></returns>
        public static int HD_ReturnObjectNumMax(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            return lv_num;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object序号。使用"互动O_注册Object"后使用本函数可返回Key里的注册序号，Key无元素返回0，Key有元素但对象不在里面则返回-1，Object标签尚未注册则返回-2。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <returns></returns>
        public static int HD_ReturnObjectNum(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_i;
            string lv_tag;
            int lv_torf;
            // Automatic Variable Declarations
            const int auto_n = 1;
            int auto_i;
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_torf = -1;
            // Implementation
            for (auto_i = 1; auto_i <= auto_n; auto_i += 1)
            {
                if ((lv_tag != null))
                {
                    lv_torf = -2;
                    break;
                }
                if ((lv_num == 0))
                {
                    lv_torf = 0;
                }
                else
                {
                    if ((lv_num >= 1))
                    {
                        auto_ae = lv_num;
                        auto_var = 1;
                        for (; auto_var <= auto_ae; auto_var += 1)
                        {
                            lv_i = auto_var;
                            if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tag))
                            {
                                lv_torf = lv_i;
                                break;
                            }
                        }
                    }
                }
            }
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回序号对应Object。使用"互动O_注册Object"后，在参数填入注册序号可返回Object。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_regNum"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <returns></returns>
        public static object HD_ReturnObjectFromRegNum(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            object lv_object;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_object = (object)DataTableLoad0(true, ("HD_Object_" + lv_tag));
            // Implementation
            return lv_object;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回句柄标签对应Object。使用"互动O_注册Object"后，在参数填入句柄标签（整数）可返回Object，标签是Object的句柄。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_tag">句柄标签</param>
        /// <returns></returns>
        public static object HD_ReturnObjectFromTag(int lp_tag)
        {
            // Variable Declarations
            string lv_tag;
            object lv_object;
            // Variable Initialization
            lv_tag = lp_tag.ToString();
            lv_object = (object)DataTableLoad0(true, ("HD_Object_" + lv_tag));
            // Implementation
            return lv_object;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回序号对应Object标签句柄。使用"互动O_注册Object"后，在参数填入注册序号可返回Object标签（字符串）。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <returns></returns>
        public static string HD_ReturnObjectTagFromRegNum_String(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回序号对应Object标签句柄。使用"互动O_注册Object"后，在参数填入注册序号可返回Object标签（整数）。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <returns></returns>
        public static int HD_ReturnObjectTagFromRegNum_Int(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return Convert.ToInt32(lv_tag);
        }

        /// <summary>
        /// 【MM_函数库】互动O_设置Object状态。必须先"注册"获得功能库内部句柄，再使用本函数给Object设定一个状态值，之后可用"互动O_返回Object状态"。类型参数用以记录多个不同状态，仅当"类型"参数填Object组ID转的Object串时，状态值"true"和"false"是Object的Object组专用状态值，用于内部函数筛选Object状态（相当于单位组单位索引是否有效），其他类型不会干扰系统内部，可随意填写。虽然注销时反向清空注册信息，但用"互动O_设定Object状态/自定义值"创建的值需要手工填入""来排泄（非大量注销则提升内存量极小，可不管）。注：固有状态值是注册函数赋予的系统内部变量（相当于单位组单位是否活体），只能通过"互动O_注册Object（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <param name="lp_stats">状态</param>
        public static void HD_SetObjectState(object lp_object, string lp_key, string lp_stats)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = HD_RegObjectTagAndReturn(lp_object);
            // Implementation
            DataTableSave0(true, ("HD_ObjectState" + lv_str + "_" + lv_tag), lp_stats);
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object状态。使用"互动O_设定Object状态"后可使用本函数，将本函数参数"类型"设为空时返回固有值。类型参数用以记录多个不同状态，仅当"类型"参数为Object组ID转的字符串时，返回的状态值"true"和"false"是Object的Object组专用状态值，用于内部函数筛选Object状态（相当于单位组单位索引是否有效）
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <returns></returns>
        public static string HD_ReturnObjectState(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_stats = DataTableLoad0(true, ("HD_ObjectState" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动O_设置Object自定义值。必须先"注册"获得功能库内部句柄，再使用本函数设定Object的自定义值，之后可使用"互动O_返回Object自定义值"，类型参数用以记录多个不同自定义值。注：固有自定义值是注册函数赋予的系统内部变量，只能通过"互动O_注册Object（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <param name="lp_customValue">自定义值</param>
        public static void HD_SetObjectCV(object lp_object, string lp_key, string lp_customValue)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = HD_RegObjectTagAndReturn(lp_object);
            // Implementation
            DataTableSave0(true, ("HD_ObjectCV" + lv_str + "_" + lv_tag), lp_customValue);
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object自定义值。使用"互动O_设定Object自定义值"后可使用本函数，将本函数参数"类型"设为空时返回固有值，该参数用以记录多个不同自定义值
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <returns></returns>
        public static string HD_ReturnObjectCV(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_customValue = DataTableLoad0(true, ("HD_ObjectCV" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object固有状态。必须先使用"互动O_注册Object"才能返回到该值，固有状态是独一无二的标记（相当于单位组单位是否活体）
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        public static string HD_ReturnObjectState_Only(object lp_object)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_stats = DataTableLoad0(true, ("HD_ObjectState" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object固有自定义值。必须先使用"互动O_注册Object"才能返回到该值，固有值是独一无二的标记
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        public static string HD_ReturnObjectCV_Only(object lp_object)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_customValue = DataTableLoad0(true, ("HD_ObjectCV" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动O_设置Object的实数标记。必须先"注册"获得功能库内部句柄，再使用本函数让Object携带一个实数值，之后可使用"互动O_返回Object的实数标记"。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_realNumTag">实数标记</param>
        public static void HD_SetObjectDouble(object lp_object, double lp_realNumTag)
        {
            // Variable Declarations
            string lv_tag = "";
            // Variable Initialization
            lv_tag = HD_RegObjectTagAndReturn(lp_object);
            // Implementation
            DataTableSave0(true, ("HD_CDDouble_T_" + lv_tag), lp_realNumTag);
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object的实数标记。使用"互动O_设定Object的实数标记"后可使用本函数。Object组使用时，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        public static double HD_ReturnObjectDouble(object lp_object)
        {
            // Variable Declarations
            string lv_tag = "";
            double lv_f;
            // Variable Initialization
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_f = (double)DataTableLoad0(true, ("HD_CDDouble_T_" + lv_tag));
            // Implementation
            return lv_f;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object标签句柄有效状态。将Object视作独一无二的个体，标签是它本身，有效状态则类似"单位是否有效"，当使用"互动O_注册Object"或"互动OG_添加Object到Object组"后激活Object有效状态（值为"true"），除非使用"互动O_注册Object（高级）"改写，否则直到注销才会摧毁
        /// </summary>
        /// <param name="lp_object"></param>
        /// <returns></returns>
        public static bool HD_ReturnIfObjectTag(object lp_object)
        {
            // Variable Declarations
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfObjectTag" + "" + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动O_返回Object注册状态。使用"互动O_注册Object"或"互动OG_添加Object到Object组"后可使用本函数获取注册Object在Key中的注册状态，该状态只能注销或从Object组中移除时清空。Object组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Object组转为Key
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_key">存储键区，默认值"_Object"</param>
        /// <returns></returns>
        public static bool HD_ReturnIfObjectTagKey(object lp_object, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_tag = HD_ReturnObjectTag(lp_object);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfObjectTag" + lv_str + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动OG_根据自定义值类型将Object组排序。根据Object携带的自定义值类型，对指定的Object组元素进行冒泡排序。Object组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Object组名称</param>
        /// <param name="lp_cVStr">自定义值类型</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_ObjectGSortCV(string lp_key, string lp_cVStr, bool lp_big)
        {
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            string lv_tagValuestr;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_ObjectGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnObjectTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValuestr = HD_ReturnObjectCV(HD_ReturnObjectFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnObjectTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "ObjectTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValuestr = HD_ReturnObjectCV(HD_ReturnObjectFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "ObjectTag"), lv_a, lv_tag); //lv_tag放入新序号
                    //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_ObjectGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动OG_Object组排序。对指定的Object组元素进行冒泡排序（根据元素句柄）。Object组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Object组名称</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_ObjectGSort(string lp_key, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_ObjectGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnObjectTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValue = lv_tag;
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnObjectTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "ObjectTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValue = lv_tag;
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                                                                                    //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "ObjectTag"), lv_a, lv_tag); //lv_tag放入新序号
                                                                                //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_ObjectGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动OG_设定Object的Object组专用状态。给Object组的Object设定一个状态值（字符串），之后可用"互动O_返回Object、互动OG_返回Object组的Object状态"。状态值"true"和"false"是Object的Object组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效），而本函数可以重设干预，影响函数"互动OG_返回Object组元素数量（仅检索XX状态）"。与"互动O_设定Object状态"功能相同，只是状态参数在Object组中被固定为"Object组变量的内部ID"。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_objectGroup"></param>
        /// <param name="lp_groupState"></param>
        public static void HD_SetObjectGState(object lp_object, string lp_objectGroup, string lp_groupState)
        {
            HD_SetObjectState(lp_object, lp_objectGroup, lp_groupState);
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object的Object组专用状态。使用"互动O_设定Object、互动OG_设定Object组的Object状态"后可使用本函数。与"互动O_返回Object状态"功能相同，只是状态参数在Object组中被固定为"Object组变量的内部ID"。状态值"true"和"false"是Object的Object组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效）。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_objectGroup"></param>
        public static void HD_ReturnObjectGState(object lp_object, string lp_objectGroup)
        {
            HD_ReturnObjectState(lp_object, lp_objectGroup);
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组元素序号对应元素。返回Object组元素序号指定Object。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static object HD_ReturnObjectFromObjectGFunc(int lp_regNum, string lp_gs)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            object lv_object;
            // Variable Initialization
            lv_str = (lp_gs + "Object");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_object = (object)DataTableLoad0(true, ("HD_Object_" + lv_tag));
            // Implementation
            return lv_object;
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组元素总数。返回指定Object组的元素数量。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnObjectGNumMax(string lp_gs)
        {
            return (int)DataTableLoad0(true, lp_gs + "ObjectNum");
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组元素总数（仅检测Object组专用状态="true"）。返回指定Object组的元素数量。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnObjectGNumMax_StateTrueFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            object lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnObjectNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnObjectFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnObjectState(lv_c, lp_gs);
                if ((lv_b == "true"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组元素总数（仅检测Object组专用状态="false"）。返回指定Object组的元素数量。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnObjectGNumMax_StateFalseFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            object lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnObjectNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnObjectFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnObjectState(lv_c, lp_gs);
                if ((lv_b == "false"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组元素总数（仅检测Object组无效专用状态："false"或""）。返回指定Object组的元素数量（false、""、null）。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnObjectGNumMax_StateUselessFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            object lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnObjectNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnObjectFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnObjectState(lv_c, lp_gs);
                if (((lv_b == "false") || (lv_b == "") || (lv_b == null)))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组元素总数（仅检测Object组指定专用状态）。返回指定Object组的元素数量。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_State">Object组专用状态</param>
        /// <returns></returns>
        public static int HD_ReturnObjectGNumMax_StateFunc_Specify(string lp_gs, string lp_State)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            object lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnObjectNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnObjectFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnObjectState(lv_c, lp_gs);
                if ((lv_b == lp_State))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动OG_添加Object到Object组。相同Object被认为是同一个，非高级功能不提供专用状态检查，如果Object没有设置过Object组专用状态，那么首次添加到Object组不会赋予"true"（之后可通过"互动O_设定Object状态"、"互动OG_设定Object组的Object状态"修改）。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddObjectToGroup_Simple(object lp_object, string lp_gs)
        {
            HD_RegObject_Simple(lp_object, lp_gs);
        }

        /// <summary>
        /// 【MM_函数库】互动OG_添加Object到Object组（高级）。相同Object被认为是同一个，高级功能提供专用状态检查，如果Object没有设置过Object组专用状态，那么首次添加到Object组会赋予"true"（之后可通过"互动O_设定Object状态"、"互动OG_设定Object组的Object状态"修改）。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddObjectToGroup(object lp_object, string lp_gs)
        {
            HD_RegObject_Simple(lp_object, lp_gs);
            if (DataTableKeyExists(true, ("HD_ObjectState" + lp_gs + "Object_" + HD_RegObjectTagAndReturn(lp_object))) == false)
            {
                DataTableSave0(true, ("HD_ObjectState" + lp_gs + "Object_" + HD_RegObjectTagAndReturn(lp_object)), "true");
                //Console.WriteLine(lp_gs + "=>" + HD_RegObjectTagAndReturn(lp_object));
            }
        }

        /// <summary>
        /// 【MM_函数库】互动OG_移除Object组中的元素。使用"互动OG_添加Object到Object组"后可使用本函数进行移除元素。移除使用了"互动O_移除Object"，同一个存储区（Object组ID）序号重排，移除时该存储区如有其他操作会排队等待。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_object"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_ClearObjectFromGroup(object lp_object, string lp_gs)
        {
            HD_RemoveObject(lp_object, lp_gs);
        }

        //互动OG_为Object组中的每个序号
        //GE（星际2的Galaxy Editor）的宏让编辑器保存时自动生成脚本并整合进脚本进行格式调整，C#仅参考需自行编写
        // #AUTOVAR(vs, string) = "#PARAM(group)";//"#PARAM(group)"是与字段、变量名一致的元素组名称，宏去声明string类型名为“Auto随机编号_vs”的自动变量，然后=右侧字符
        // #AUTOVAR(ae) = HD_ReturnObjectNumMax(#AUTOVAR(vs));//宏去声明默认int类型名为“Auto随机编号_ae”的自动变量，然后=右侧字符
        // #INITAUTOVAR(ai,increment)//宏去声明int类型名为“Auto随机编号_ai”的自动变量，用于下面for循环增量（increment是传入参数）
        // #PARAM(var) = #PARAM(s);//#PARAM(var)是传进来的参数，用作“当前被挑选到的元素”（任意变量-整数 lp_var）， #PARAM(s)是传进来的参数用作"开始"（int lp_s）
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #PARAM(var) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #PARAM(var) >= #AUTOVAR(ae)) ) ; #PARAM(var) += #AUTOVAR(ai) ) {
        //     #SUBFUNCS(actions)//代表用户GUI填写的所有动作
        // }

        /// <summary>
        /// 【MM_函数库】互动OG_为Object组中的每个序号。每次挑选的元素序号会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素序号，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachObjectNumFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            int lv_ae = HD_ReturnObjectNumMax(lp_gs);
            int lv_var = lp_start;
            int lv_ai = lp_increment;
            for (; (lv_ai >= 0 && lv_var <= lv_ae) || (lv_ai < 0 && lv_var >= lv_ae); lv_var += lv_ai)
            {
                lp_funcref(lv_var);//用户填写的所有动作
            }
        }

        //互动OG_为Object组中的每个元素
        // #AUTOVAR(vs, string) = "#PARAM(group)";
        // #AUTOVAR(ae) = HD_ReturnObjectNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= #PARAM(s);
        // #INITAUTOVAR(ai,increment)
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     DataTableSave(false, "ObjectGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)), HD_ReturnObjectFromRegNum(#AUTOVAR(va),#AUTOVAR(vs)));
        // }
        // #AUTOVAR(va)= #PARAM(s);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #PARAM(var) = DataTableLoad(false, "ObjectGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)));
        //     #SUBFUNCS(actions)
        // }

        /// <summary>
        /// 【MM_函数库】互动OG_为Object组中的每个元素。每次挑选的元素会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachObjectFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            string lv_vs = lp_gs;
            int lv_ae = HD_ReturnObjectNumMax(lv_vs);
            int lv_va = lp_start;
            int lv_ai = lp_increment;
            object lv_object;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                DataTableSave0(false, "ObjectGFor" + lv_vs + lv_va.ToString(), HD_ReturnObjectFromRegNum(lv_va, lv_vs));
            }
            lv_va = lp_start;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                lv_object = DataTableLoad0(false, "ObjectGFor" + lv_vs + lv_va.ToString());
                lp_funcref(lv_object);//用户填写的所有动作
            }
        }

        /// <summary>
        /// 【MM_函数库】互动OG_返回Object组中随机元素。返回指定Object组中的随机Object。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static object HD_ReturnRandomObjectFromObjectGFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_num;
            int lv_a;
            object lv_c = null;
            // Variable Initialization
            lv_num = HD_ReturnObjectNumMax(lp_gs);
            // Implementation
            if ((lv_num >= 1))
            {
                lv_a = RandomInt(1, lv_num);
                lv_c = HD_ReturnObjectFromRegNum(lv_a, lp_gs);
            }
            return lv_c;
        }

        //互动OG_添加Object组到Object组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnObjectNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnObjectFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_AddObjectToGroup(#AUTOVAR(var), #AUTOVAR(vsb));
        // }


        /// <summary>
        /// 【MM_函数库】互动OG_添加Object组到Object组。添加一个Object组A的元素到另一个Object组B，相同Object被认为是同一个。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_AddObjectGToObjectG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnObjectNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            object lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnObjectFromRegNum(lv_va, lv_vsa);
                HD_AddObjectToGroup(lv_var, lv_vsb);
            }
        }

        //互动OG_从Object组移除Object组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnObjectNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnObjectFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_RemoveObject(#AUTOVAR(var), #AUTOVAR(vsb));
        // }

        /// <summary>
        /// 【MM_函数库】互动OG_从Object组移除Object组。将Object组A的元素从Object组B中移除，相同Object被认为是同一个。移除使用了"互动O_移除Object"，同一个存储区（Object组ID）序号重排，移除时该存储区如有其他操作会排队等待。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_ClearObjectGFromObjectG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnObjectNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            object lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnObjectFromRegNum(lv_va, lv_vsa);
                HD_RemoveObject(lv_var, lv_vsb);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动OG_移除Object组全部元素。将Object组（Key区）存储的元素全部移除，相同Object被认为是同一个。移除时同一个存储区（Object组ID）序号不进行重排，但该存储区如有其他操作会排队等待。Object组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Object组到Object组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Object组名称</param>
        public static void HD_RemoveObjectGAll(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            // Variable Initialization
            lv_str = (lp_key + "Object");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_ObjectGroup" + lv_str, 1);
            for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
            {
                lv_tag = DataTableLoad1(true, (lp_key + "ObjectTag"), lv_a).ToString();
                lv_num -= 1;
                DataTableClear0(true, "HD_IfObjectTag" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_ObjectCV" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_ObjectState" + lv_str + "_" + lv_tag);
                DataTableSave0(true, (lp_key + "ObjectNum"), lv_num);
            }
            DataTableSave0(true, "Key_ObjectGroup" + lv_str, 0);
        }

        //--------------------------------------------------------------------------------------------------
        // 任意类型组End
        //--------------------------------------------------------------------------------------------------

        #endregion

        #region 二维向量

        //提示：尽可能使用对口类型，以防值类型与引用类型发生转换时拆装箱降低性能

        //--------------------------------------------------------------------------------------------------
        // 二维向量组Start
        //--------------------------------------------------------------------------------------------------

        /// <summary>
        /// 【MM_函数库】互动V_注册Vector标签句柄并返回。为Vector自动设置新的标签句柄，重复时会返回已注册的Vector标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        private static int HD_RegVectorTagAndReturn_Int(Vector lp_vector)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_VectorJBNum");
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_VectorJBNum", lv_j);
                DataTableSave0(true, ("HD_Vector_" + lv_j.ToString()), lp_vector);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((Vector)DataTableLoad0(true, ("HD_Vector_" + lv_j.ToString())) == lp_vector)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_VectorJBNum", lv_j);
                            DataTableSave0(true, ("HD_Vector_" + lv_j.ToString()), lp_vector);
                        }
                    }
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector已注册标签句柄。返回一个Vector的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        public static int HD_ReturnVectorTag_Int(Vector lp_vector)
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_VectorJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((Vector)DataTableLoad0(true, "HD_Vector_" + lv_j.ToString()) == lp_vector)
                {
                    break;
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动V_注册Vector标签句柄并返回。为Vector自动设置新的标签句柄，重复时会返回已注册的Vector标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        private static string HD_RegVectorTagAndReturn(Vector lp_vector)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_VectorJBNum");
            lv_tag = "";
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_VectorJBNum", lv_j);
                DataTableSave0(true, ("HD_Vector_" + lv_j.ToString()), lp_vector);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((Vector)DataTableLoad0(true, "HD_Vector_" + lv_j.ToString()) == lp_vector)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_VectorJBNum", lv_j);
                            DataTableSave0(true, ("HD_Vector_" + lv_j.ToString()), lp_vector);
                        }
                    }
                }
            }
            lv_tag = lv_j.ToString();
            //Console.WriteLine(("Tag：" + lv_tag));
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector已注册标签句柄。返回一个Vector的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        public static string HD_ReturnVectorTag(Vector lp_vector)
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_VectorJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((Vector)DataTableLoad0(true, "HD_Vector_" + lv_j.ToString()) == lp_vector)
                {
                    lv_tag = lv_j.ToString();
                    break;
                }
            }
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动V_注册Vector(高级)。在指定Key存入Vector，固有状态、自定义值是Vector独一无二的标志（本函数重复注册会刷新），之后可用互动V_"返回Vector注册总数"、"返回Vector序号"、"返回序号对应Vector"、"返回序号对应Vector标签"、"返回Vector自定义值"。Vector组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Vector组转为Key。首次注册时固有状态为true（相当于单位组单位活体），如需另外设置多个标记可使用"互动V_设定Vector状态/自定义值"
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <param name="lp_inherentStats">固有状态</param>
        /// <param name="lp_inherentCustomValue">固有自定义值</param>
        public static void HD_RegVector(Vector lp_vector, string lp_key, string lp_inherentStats, string lp_inherentCustomValue)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegVectorTagAndReturn(lp_vector);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfVectorTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfVectorTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfVectorTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfVectorTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            DataTableSave0(true, ("HD_VectorState" + "" + "_" + lv_tagStr), lp_inherentStats);
            DataTableSave0(true, ("HD_VectorCV" + "" + "_" + lv_tagStr), lp_inherentCustomValue);
        }

        /// <summary>
        /// 【MM_函数库】互动V_注册Vector。在指定Key存入Vector，固有状态、自定义值是Vector独一无二的标志（本函数重复注册不会刷新），之后可用互动V_"返回Vector注册总数"、"返回Vector序号"、"返回序号对应Vector"、"返回Vector自定义值"。Vector组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Vector组转为Key。首次注册时固有状态为true（相当于单位组单位活体），之后只能通过"互动V_注册Vector（高级）"改写，如需另外设置多个标记可使用"互动V_设定Vector状态/自定义值"
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        public static void HD_RegVector_Simple(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegVectorTagAndReturn(lp_vector);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfVectorTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfVectorTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfVectorTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfVectorTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            if ((DataTableKeyExists(true, ("HD_Vector" + "State" + "_" + lv_tag.ToString())) == false))
            {
                DataTableSave1(true, (("HD_Vector" + "State")), lv_tag, "true");
            }
        }

        /// <summary>
        /// 【MM_函数库】互动V_注销Vector。用"互动V_注册Vector"到Key，之后可用本函数彻底摧毁注册信息并将序号重排（包括Vector标签有效状态、固有状态及自定义值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动V_设定Vector状态"让Vector状态失效（类似单位组的单位活体状态）。Vector组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Vector组转为Key。本函数无法摧毁用"互动V_设定Vector状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Vector组变量ID时会清空Vector组专用状态
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        public static void HD_DestroyVector(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_VectorGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "VectorTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfVectorTag_" + lv_tag);
                        DataTableClear0(true, "HD_IfVectorTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_Vector_" + lv_tag);
                        DataTableClear0(true, "HD_VectorCV_" + lv_tag);
                        DataTableClear0(true, "HD_VectorState_" + lv_tag);
                        DataTableClear0(true, "HD_VectorCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_VectorState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "VectorNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "VectorTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "VectorTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_VectorGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动V_移除Vector。用"互动V_注册Vector"到Key，之后可用本函数仅摧毁Key区注册的信息并将序号重排，用于Vector组或多个键区仅移除Vector（保留Vector标签有效状态、固有值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动V_设定Vector状态"让Vector状态失效（类似单位组的单位活体状态）。Vector组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Vector组转为Key。本函数无法摧毁用"互动V_设定Vector状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Vector组变量ID时会清空Vector组专用状态
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        public static void HD_RemoveVector(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_VectorGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "VectorTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfVectorTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_VectorCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_VectorState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "VectorNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "VectorTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "VectorTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_VectorGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector注册总数。必须先使用"互动V_注册Vector"才能返回指定Key里的注册总数。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key。
        /// </summary>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <returns></returns>
        public static int HD_ReturnVectorNumMax(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            return lv_num;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector序号。使用"互动V_注册Vector"后使用本函数可返回Key里的注册序号，Key无元素返回0，Key有元素但对象不在里面则返回-1，Vector标签尚未注册则返回-2。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <returns></returns>
        public static int HD_ReturnVectorNum(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_i;
            string lv_tag = "";
            int lv_torf;
            // Automatic Variable Declarations
            const int auto_n = 1;
            int auto_i;
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_torf = -1;
            // Implementation
            for (auto_i = 1; auto_i <= auto_n; auto_i += 1)
            {
                if ((lv_tag != null))
                {
                    lv_torf = -2;
                    break;
                }
                if ((lv_num == 0))
                {
                    lv_torf = 0;
                }
                else
                {
                    if ((lv_num >= 1))
                    {
                        auto_ae = lv_num;
                        auto_var = 1;
                        for (; auto_var <= auto_ae; auto_var += 1)
                        {
                            lv_i = auto_var;
                            if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tag))
                            {
                                lv_torf = lv_i;
                                break;
                            }
                        }
                    }
                }
            }
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回序号对应Vector。使用"互动V_注册Vector"后，在参数填入注册序号可返回Vector。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_regNum"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <returns></returns>
        public static Vector HD_ReturnVectorFromRegNum(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            Vector lv_vector;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_vector = (Vector)DataTableLoad0(true, ("HD_Vector_" + lv_tag));
            // Implementation
            return lv_vector;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回句柄标签对应Vector。使用"互动V_注册Vector"后，在参数填入句柄标签（整数）可返回Vector，标签是Vector的句柄。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_tag">句柄标签</param>
        /// <returns></returns>
        public static Vector HD_ReturnVectorFromTag(int lp_tag)
        {
            // Variable Declarations
            string lv_tag = "";
            Vector lv_vector;
            // Variable Initialization
            lv_tag = lp_tag.ToString();
            lv_vector = (Vector)DataTableLoad0(true, ("HD_Vector_" + lv_tag));
            // Implementation
            return lv_vector;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回序号对应Vector标签句柄。使用"互动V_注册Vector"后，在参数填入注册序号可返回Vector标签（字符串）。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <returns></returns>
        public static string HD_ReturnVectorTagFromRegNum_String(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回序号对应Vector标签句柄。使用"互动V_注册Vector"后，在参数填入注册序号可返回Vector标签（整数）。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <returns></returns>
        public static int HD_ReturnVectorTagFromRegNum_Int(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return Convert.ToInt32(lv_tag);
        }

        /// <summary>
        /// 【MM_函数库】互动V_设置Vector状态。必须先"注册"获得功能库内部句柄，再使用本函数给Vector设定一个状态值，之后可用"互动V_返回Vector状态"。类型参数用以记录多个不同状态，仅当"类型"参数填Vector组ID转的Vector串时，状态值"true"和"false"是Vector的Vector组专用状态值，用于内部函数筛选Vector状态（相当于单位组单位索引是否有效），其他类型不会干扰系统内部，可随意填写。虽然注销时反向清空注册信息，但用"互动V_设定Vector状态/自定义值"创建的值需要手工填入""来排泄（非大量注销则提升内存量极小，可不管）。注：固有状态值是注册函数赋予的系统内部变量（相当于单位组单位是否活体），只能通过"互动V_注册Vector（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <param name="lp_stats">状态</param>
        public static void HD_SetVectorState(Vector lp_vector, string lp_key, string lp_stats)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = HD_RegVectorTagAndReturn(lp_vector);
            // Implementation
            DataTableSave0(true, ("HD_VectorState" + lv_str + "_" + lv_tag), lp_stats);
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector状态。使用"互动V_设定Vector状态"后可使用本函数，将本函数参数"类型"设为空时返回固有值。类型参数用以记录多个不同状态，仅当"类型"参数为Vector组ID转的字符串时，返回的状态值"true"和"false"是Vector的Vector组专用状态值，用于内部函数筛选Vector状态（相当于单位组单位索引是否有效）
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <returns></returns>
        public static string HD_ReturnVectorState(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_stats = DataTableLoad0(true, ("HD_VectorState" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动V_设置Vector自定义值。必须先"注册"获得功能库内部句柄，再使用本函数设定Vector的自定义值，之后可使用"互动V_返回Vector自定义值"，类型参数用以记录多个不同自定义值。注：固有自定义值是注册函数赋予的系统内部变量，只能通过"互动V_注册Vector（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <param name="lp_customValue">自定义值</param>
        public static void HD_SetVectorCV(Vector lp_vector, string lp_key, string lp_customValue)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = HD_RegVectorTagAndReturn(lp_vector);
            // Implementation
            DataTableSave0(true, ("HD_VectorCV" + lv_str + "_" + lv_tag), lp_customValue);
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector自定义值。使用"互动V_设定Vector自定义值"后可使用本函数，将本函数参数"类型"设为空时返回固有值，该参数用以记录多个不同自定义值
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <returns></returns>
        public static string HD_ReturnVectorCV(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_customValue = DataTableLoad0(true, ("HD_VectorCV" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector固有状态。必须先使用"互动V_注册Vector"才能返回到该值，固有状态是独一无二的标记（相当于单位组单位是否活体）
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        public static string HD_ReturnVectorState_Only(Vector lp_vector)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_stats = DataTableLoad0(true, ("HD_VectorState" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector固有自定义值。必须先使用"互动V_注册Vector"才能返回到该值，固有值是独一无二的标记
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        public static string HD_ReturnVectorCV_Only(Vector lp_vector)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_customValue = DataTableLoad0(true, ("HD_VectorCV" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动V_设置Vector的实数标记。必须先"注册"获得功能库内部句柄，再使用本函数让Vector携带一个实数值，之后可使用"互动V_返回Vector的实数标记"。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_realNumTag">实数标记</param>
        public static void HD_SetVectorDouble(Vector lp_vector, double lp_realNumTag)
        {
            // Variable Declarations
            string lv_tag = "";
            // Variable Initialization
            lv_tag = HD_RegVectorTagAndReturn(lp_vector);
            // Implementation
            DataTableSave0(true, ("HD_CDDouble_T_" + lv_tag), lp_realNumTag);
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector的实数标记。使用"互动V_设定Vector的实数标记"后可使用本函数。Vector组使用时，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        public static double HD_ReturnVectorDouble(Vector lp_vector)
        {
            // Variable Declarations
            string lv_tag = "";
            double lv_f;
            // Variable Initialization
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_f = (double)DataTableLoad0(true, ("HD_CDDouble_T_" + lv_tag));
            // Implementation
            return lv_f;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector标签句柄有效状态。将Vector视作独一无二的个体，标签是它本身，有效状态则类似"单位是否有效"，当使用"互动V_注册Vector"或"互动VG_添加Vector到Vector组"后激活Vector有效状态（值为"true"），除非使用"互动V_注册Vector（高级）"改写，否则直到注销才会摧毁
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <returns></returns>
        public static bool HD_ReturnIfVectorTag(Vector lp_vector)
        {
            // Variable Declarations
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfVectorTag" + "" + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动V_返回Vector注册状态。使用"互动V_注册Vector"或"互动VG_添加Vector到Vector组"后可使用本函数获取注册Vector在Key中的注册状态，该状态只能注销或从Vector组中移除时清空。Vector组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Vector组转为Key
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_key">存储键区，默认值"_Vector"</param>
        /// <returns></returns>
        public static bool HD_ReturnIfVectorTagKey(Vector lp_vector, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_tag = HD_ReturnVectorTag(lp_vector);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfVectorTag" + lv_str + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动VG_根据自定义值类型将Vector组排序。根据Vector携带的自定义值类型，对指定的Vector组元素进行冒泡排序。Vector组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Vector组名称</param>
        /// <param name="lp_cVStr">自定义值类型</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_VectorGSortCV(string lp_key, string lp_cVStr, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            string lv_tagValuestr;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_VectorGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnVectorTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValuestr = HD_ReturnVectorCV(HD_ReturnVectorFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnVectorTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "VectorTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValuestr = HD_ReturnVectorCV(HD_ReturnVectorFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "VectorTag"), lv_a, lv_tag); //lv_tag放入新序号
                    //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_VectorGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动VG_Vector组排序。对指定的Vector组元素进行冒泡排序（根据元素句柄）。Vector组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Vector组名称</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_VectorGSort(string lp_key, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_VectorGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnVectorTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValue = lv_tag;
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnVectorTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "VectorTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValue = lv_tag;
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                                                                                    //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "VectorTag"), lv_a, lv_tag); //lv_tag放入新序号
                                                                                //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_VectorGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动VG_设定Vector的Vector组专用状态。给Vector组的Vector设定一个状态值（字符串），之后可用"互动V_返回Vector、互动VG_返回Vector组的Vector状态"。状态值"true"和"false"是Vector的Vector组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效），而本函数可以重设干预，影响函数"互动VG_返回Vector组元素数量（仅检索XX状态）"。与"互动V_设定Vector状态"功能相同，只是状态参数在Vector组中被固定为"Vector组变量的内部ID"。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_vectorGroup"></param>
        /// <param name="lp_groupState"></param>
        public static void HD_SetVectorGState(Vector lp_vector, string lp_vectorGroup, string lp_groupState)
        {
            HD_SetVectorState(lp_vector, lp_vectorGroup, lp_groupState);
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector的Vector组专用状态。使用"互动V_设定Vector、互动VG_设定Vector组的Vector状态"后可使用本函数。与"互动V_返回Vector状态"功能相同，只是状态参数在Vector组中被固定为"Vector组变量的内部ID"。状态值"true"和"false"是Vector的Vector组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效）。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_vectorGroup"></param>
        public static void HD_ReturnVectorGState(Vector lp_vector, string lp_vectorGroup)
        {
            HD_ReturnVectorState(lp_vector, lp_vectorGroup);
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组元素序号对应元素。返回Vector组元素序号指定Vector。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static Vector HD_ReturnVectorFromVectorGFunc(int lp_regNum, string lp_gs)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            Vector lv_vector;
            // Variable Initialization
            lv_str = (lp_gs + "Vector");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_vector = (Vector)DataTableLoad0(true, ("HD_Vector_" + lv_tag));
            // Implementation
            return lv_vector;
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组元素总数。返回指定Vector组的元素数量。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnVectorGNumMax(string lp_gs)
        {
            return (int)DataTableLoad0(true, lp_gs + "VectorNum");
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组元素总数（仅检测Vector组专用状态="true"）。返回指定Vector组的元素数量。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnVectorGNumMax_StateTrueFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Vector lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnVectorNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnVectorFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnVectorState(lv_c, lp_gs);
                if ((lv_b == "true"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组元素总数（仅检测Vector组专用状态="false"）。返回指定Vector组的元素数量。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnVectorGNumMax_StateFalseFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Vector lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnVectorNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnVectorFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnVectorState(lv_c, lp_gs);
                if ((lv_b == "false"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组元素总数（仅检测Vector组无效专用状态："false"或""）。返回指定Vector组的元素数量（false、""、null）。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnVectorGNumMax_StateUselessFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Vector lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnVectorNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnVectorFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnVectorState(lv_c, lp_gs);
                if (((lv_b == "false") || (lv_b == "") || (lv_b == null)))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组元素总数（仅检测Vector组指定专用状态）。返回指定Vector组的元素数量。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_State">Vector组专用状态</param>
        /// <returns></returns>
        public static int HD_ReturnVectorGNumMax_StateFunc_Specify(string lp_gs, string lp_State)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Vector lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnVectorNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnVectorFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnVectorState(lv_c, lp_gs);
                if ((lv_b == lp_State))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动VG_添加Vector到Vector组。相同Vector被认为是同一个，非高级功能不提供专用状态检查，如果Vector没有设置过Vector组专用状态，那么首次添加到Vector组不会赋予"true"（之后可通过"互动V_设定Vector状态"、"互动VG_设定Vector组的Vector状态"修改）。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddVectorToGroup_Simple(Vector lp_vector, string lp_gs)
        {
            HD_RegVector_Simple(lp_vector, lp_gs);
        }

        /// <summary>
        /// 【MM_函数库】互动VG_添加Vector到Vector组（高级）。相同Vector被认为是同一个，高级功能提供专用状态检查，如果Vector没有设置过Vector组专用状态，那么首次添加到Vector组会赋予"true"（之后可通过"互动V_设定Vector状态"、"互动VG_设定Vector组的Vector状态"修改）。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddVectorToGroup(Vector lp_vector, string lp_gs)
        {
            HD_RegVector_Simple(lp_vector, lp_gs);
            if (DataTableKeyExists(true, ("HD_VectorState" + lp_gs + "Vector_" + HD_RegVectorTagAndReturn(lp_vector))) == false)
            {
                DataTableSave0(true, ("HD_VectorState" + lp_gs + "Vector_" + HD_RegVectorTagAndReturn(lp_vector)), "true");
                //Console.WriteLine(lp_gs + "=>" + HD_RegVectorTagAndReturn(lp_vector));
            }
        }

        /// <summary>
        /// 【MM_函数库】互动VG_移除Vector组中的元素。使用"互动VG_添加Vector到Vector组"后可使用本函数进行移除元素。移除使用了"互动V_移除Vector"，同一个存储区（Vector组ID）序号重排，移除时该存储区如有其他操作会排队等待。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_vector"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_ClearVectorFromGroup(Vector lp_vector, string lp_gs)
        {
            HD_RemoveVector(lp_vector, lp_gs);
        }

        //互动VG_为Vector组中的每个序号
        //GE（星际2的Galaxy Editor）的宏让编辑器保存时自动生成脚本并整合进脚本进行格式调整，C#仅参考需自行编写
        // #AUTOVAR(vs, string) = "#PARAM(group)";//"#PARAM(group)"是与字段、变量名一致的元素组名称，宏去声明string类型名为“Auto随机编号_vs”的自动变量，然后=右侧字符
        // #AUTOVAR(ae) = HD_ReturnVectorNumMax(#AUTOVAR(vs));//宏去声明默认int类型名为“Auto随机编号_ae”的自动变量，然后=右侧字符
        // #INITAUTOVAR(ai,increment)//宏去声明int类型名为“Auto随机编号_ai”的自动变量，用于下面for循环增量（increment是传入参数）
        // #PARAM(var) = #PARAM(s);//#PARAM(var)是传进来的参数，用作“当前被挑选到的元素”（任意变量-整数 lp_var）， #PARAM(s)是传进来的参数用作"开始"（int lp_s）
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #PARAM(var) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #PARAM(var) >= #AUTOVAR(ae)) ) ; #PARAM(var) += #AUTOVAR(ai) ) {
        //     #SUBFUNCS(actions)//代表用户GUI填写的所有动作
        // }

        /// <summary>
        /// 【MM_函数库】互动VG_为Vector组中的每个序号。每次挑选的元素序号会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素序号，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachVectorNumFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            int lv_ae = HD_ReturnVectorNumMax(lp_gs);
            int lv_var = lp_start;
            int lv_ai = lp_increment;
            for (; (lv_ai >= 0 && lv_var <= lv_ae) || (lv_ai < 0 && lv_var >= lv_ae); lv_var += lv_ai)
            {
                lp_funcref(lv_var);//用户填写的所有动作
            }
        }

        //互动VG_为Vector组中的每个元素
        // #AUTOVAR(vs, string) = "#PARAM(group)";
        // #AUTOVAR(ae) = HD_ReturnVectorNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= #PARAM(s);
        // #INITAUTOVAR(ai,increment)
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     DataTableSave(false, "VectorGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)), HD_ReturnVectorFromRegNum(#AUTOVAR(va),#AUTOVAR(vs)));
        // }
        // #AUTOVAR(va)= #PARAM(s);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #PARAM(var) = DataTableLoad(false, "VectorGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)));
        //     #SUBFUNCS(actions)
        // }

        /// <summary>
        /// 【MM_函数库】互动VG_为Vector组中的每个元素。每次挑选的元素会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachVectorFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            string lv_vs = lp_gs;
            int lv_ae = HD_ReturnVectorNumMax(lv_vs);
            int lv_va = lp_start;
            int lv_ai = lp_increment;
            Vector lv_vector;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                DataTableSave0(false, "VectorGFor" + lv_vs + lv_va.ToString(), HD_ReturnVectorFromRegNum(lv_va, lv_vs));
            }
            lv_va = lp_start;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                lv_vector = (Vector)DataTableLoad0(false, "VectorGFor" + lv_vs + lv_va.ToString());
                lp_funcref(lv_vector);//用户填写的所有动作
            }
        }

        /// <summary>
        /// 【MM_函数库】互动VG_返回Vector组中随机元素。返回指定Vector组中的随机Vector。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static Vector? HD_ReturnRandomVectorFromVectorGFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_num;
            int lv_a;
            Vector? lv_c = null;
            // Variable Initialization
            lv_num = HD_ReturnVectorNumMax(lp_gs);
            // Implementation
            if ((lv_num >= 1))
            {
                lv_a = RandomInt(1, lv_num);
                lv_c = HD_ReturnVectorFromRegNum(lv_a, lp_gs);
            }
            return lv_c;
        }

        //互动VG_添加Vector组到Vector组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnVectorNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnVectorFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_AddVectorToGroup(#AUTOVAR(var), #AUTOVAR(vsb));
        // }


        /// <summary>
        /// 【MM_函数库】互动VG_添加Vector组到Vector组。添加一个Vector组A的元素到另一个Vector组B，相同Vector被认为是同一个。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_AddVectorGToVectorG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnVectorNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            Vector lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnVectorFromRegNum(lv_va, lv_vsa);
                HD_AddVectorToGroup(lv_var, lv_vsb);
            }
        }

        //互动VG_从Vector组移除Vector组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnVectorNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnVectorFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_RemoveVector(#AUTOVAR(var), #AUTOVAR(vsb));
        // }

        /// <summary>
        /// 【MM_函数库】互动VG_从Vector组移除Vector组。将Vector组A的元素从Vector组B中移除，相同Vector被认为是同一个。移除使用了"互动V_移除Vector"，同一个存储区（Vector组ID）序号重排，移除时该存储区如有其他操作会排队等待。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_ClearVectorGFromVectorG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnVectorNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            Vector lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnVectorFromRegNum(lv_va, lv_vsa);
                HD_RemoveVector(lv_var, lv_vsb);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动VG_移除Vector组全部元素。将Vector组（Key区）存储的元素全部移除，相同Vector被认为是同一个。移除时同一个存储区（Vector组ID）序号不进行重排，但该存储区如有其他操作会排队等待。Vector组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Vector组到Vector组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Vector组名称</param>
        public static void HD_RemoveVectorGAll(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            // Variable Initialization
            lv_str = (lp_key + "Vector");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_VectorGroup" + lv_str, 1);
            for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
            {
                lv_tag = DataTableLoad1(true, (lp_key + "VectorTag"), lv_a).ToString();
                lv_num -= 1;
                DataTableClear0(true, "HD_IfVectorTag" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_VectorCV" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_VectorState" + lv_str + "_" + lv_tag);
                DataTableSave0(true, (lp_key + "VectorNum"), lv_num);
            }
            DataTableSave0(true, "Key_VectorGroup" + lv_str, 0);
        }

        //--------------------------------------------------------------------------------------------------
        // 二维向量组End
        //--------------------------------------------------------------------------------------------------

        #endregion

        #region 计时器

        //提示：尽可能使用对口类型，以防值类型与引用类型发生转换时拆装箱降低性能

        //--------------------------------------------------------------------------------------------------
        // 计时器组Start
        //--------------------------------------------------------------------------------------------------

        /// <summary>
        /// 【MM_函数库】互动T_注册Timer标签句柄并返回。为Timer自动设置新的标签句柄，重复时会返回已注册的Timer标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        private static int HD_RegTimerTagAndReturn_Int(Timer lp_timer)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_TimerJBNum");
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_TimerJBNum", lv_j);
                DataTableSave0(true, ("HD_Timer_" + lv_j.ToString()), lp_timer);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((Timer)DataTableLoad0(true, ("HD_Timer_" + lv_j.ToString())) == lp_timer)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_TimerJBNum", lv_j);
                            DataTableSave0(true, ("HD_Timer_" + lv_j.ToString()), lp_timer);
                        }
                    }
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer已注册标签句柄。返回一个Timer的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        public static int HD_ReturnTimerTag_Int(Timer lp_timer)
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_TimerJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((Timer)DataTableLoad0(true, "HD_Timer_" + lv_j.ToString()) == lp_timer)
                {
                    break;
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动T_注册Timer标签句柄并返回。为Timer自动设置新的标签句柄，重复时会返回已注册的Timer标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        private static string HD_RegTimerTagAndReturn(Timer lp_timer)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_TimerJBNum");
            lv_tag = "";
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_TimerJBNum", lv_j);
                DataTableSave0(true, ("HD_Timer_" + lv_j.ToString()), lp_timer);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((Timer)DataTableLoad0(true, "HD_Timer_" + lv_j.ToString()) == lp_timer)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_TimerJBNum", lv_j);
                            DataTableSave0(true, ("HD_Timer_" + lv_j.ToString()), lp_timer);
                        }
                    }
                }
            }
            lv_tag = lv_j.ToString();
            //Console.WriteLine(("Tag：" + lv_tag));
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer已注册标签句柄。返回一个Timer的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        public static string HD_ReturnTimerTag(Timer lp_timer)
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_TimerJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((Timer)DataTableLoad0(true, "HD_Timer_" + lv_j.ToString()) == lp_timer)
                {
                    lv_tag = lv_j.ToString();
                    break;
                }
            }
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动T_注册Timer(高级)。在指定Key存入Timer，固有状态、自定义值是Timer独一无二的标志（本函数重复注册会刷新），之后可用互动T_"返回Timer注册总数"、"返回Timer序号"、"返回序号对应Timer"、"返回序号对应Timer标签"、"返回Timer自定义值"。Timer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Timer组转为Key。首次注册时固有状态为true（相当于单位组单位活体），如需另外设置多个标记可使用"互动T_设定Timer状态/自定义值"
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <param name="lp_inherentStats">固有状态</param>
        /// <param name="lp_inherentCustomValue">固有自定义值</param>
        public static void HD_RegTimer(Timer lp_timer, string lp_key, string lp_inherentStats, string lp_inherentCustomValue)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegTimerTagAndReturn(lp_timer);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfTimerTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfTimerTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfTimerTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfTimerTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            DataTableSave0(true, ("HD_TimerState" + "" + "_" + lv_tagStr), lp_inherentStats);
            DataTableSave0(true, ("HD_TimerCV" + "" + "_" + lv_tagStr), lp_inherentCustomValue);
        }

        /// <summary>
        /// 【MM_函数库】互动T_注册Timer。在指定Key存入Timer，固有状态、自定义值是Timer独一无二的标志（本函数重复注册不会刷新），之后可用互动T_"返回Timer注册总数"、"返回Timer序号"、"返回序号对应Timer"、"返回Timer自定义值"。Timer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Timer组转为Key。首次注册时固有状态为true（相当于单位组单位活体），之后只能通过"互动T_注册Timer（高级）"改写，如需另外设置多个标记可使用"互动T_设定Timer状态/自定义值"
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        public static void HD_RegTimer_Simple(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegTimerTagAndReturn(lp_timer);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfTimerTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfTimerTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfTimerTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfTimerTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            if ((DataTableKeyExists(true, ("HD_Timer" + "State" + "_" + lv_tag.ToString())) == false))
            {
                DataTableSave1(true, (("HD_Timer" + "State")), lv_tag, "true");
            }
        }

        /// <summary>
        /// 【MM_函数库】互动T_注销Timer。用"互动T_注册Timer"到Key，之后可用本函数彻底摧毁注册信息并将序号重排（包括Timer标签有效状态、固有状态及自定义值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动T_设定Timer状态"让Timer状态失效（类似单位组的单位活体状态）。Timer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Timer组转为Key。本函数无法摧毁用"互动T_设定Timer状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Timer组变量ID时会清空Timer组专用状态
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        public static void HD_DestroyTimer(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_TimerGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "TimerTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfTimerTag_" + lv_tag);
                        DataTableClear0(true, "HD_IfTimerTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_Timer_" + lv_tag);
                        DataTableClear0(true, "HD_TimerCV_" + lv_tag);
                        DataTableClear0(true, "HD_TimerState_" + lv_tag);
                        DataTableClear0(true, "HD_TimerCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_TimerState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "TimerNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "TimerTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "TimerTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_TimerGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动T_移除Timer。用"互动T_注册Timer"到Key，之后可用本函数仅摧毁Key区注册的信息并将序号重排，用于Timer组或多个键区仅移除Timer（保留Timer标签有效状态、固有值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动T_设定Timer状态"让Timer状态失效（类似单位组的单位活体状态）。Timer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Timer组转为Key。本函数无法摧毁用"互动T_设定Timer状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Timer组变量ID时会清空Timer组专用状态
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        public static void HD_RemoveTimer(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_TimerGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "TimerTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfTimerTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_TimerCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_TimerState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "TimerNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "TimerTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "TimerTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_TimerGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer注册总数。必须先使用"互动T_注册Timer"才能返回指定Key里的注册总数。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key。
        /// </summary>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <returns></returns>
        public static int HD_ReturnTimerNumMax(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            return lv_num;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer序号。使用"互动T_注册Timer"后使用本函数可返回Key里的注册序号，Key无元素返回0，Key有元素但对象不在里面则返回-1，Timer标签尚未注册则返回-2。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <returns></returns>
        public static int HD_ReturnTimerNum(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_i;
            string lv_tag = "";
            int lv_torf;
            // Automatic Variable Declarations
            const int auto_n = 1;
            int auto_i;
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_torf = -1;
            // Implementation
            for (auto_i = 1; auto_i <= auto_n; auto_i += 1)
            {
                if ((lv_tag != null))
                {
                    lv_torf = -2;
                    break;
                }
                if ((lv_num == 0))
                {
                    lv_torf = 0;
                }
                else
                {
                    if ((lv_num >= 1))
                    {
                        auto_ae = lv_num;
                        auto_var = 1;
                        for (; auto_var <= auto_ae; auto_var += 1)
                        {
                            lv_i = auto_var;
                            if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tag))
                            {
                                lv_torf = lv_i;
                                break;
                            }
                        }
                    }
                }
            }
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回序号对应Timer。使用"互动T_注册Timer"后，在参数填入注册序号可返回Timer。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_regNum"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <returns></returns>
        public static Timer HD_ReturnTimerFromRegNum(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            Timer lv_timer;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_timer = (Timer)DataTableLoad0(true, ("HD_Timer_" + lv_tag));
            // Implementation
            return lv_timer;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回句柄标签对应Timer。使用"互动T_注册Timer"后，在参数填入句柄标签（整数）可返回Timer，标签是Timer的句柄。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_tag">句柄标签</param>
        /// <returns></returns>
        public static Timer HD_ReturnTimerFromTag(int lp_tag)
        {
            // Variable Declarations
            string lv_tag = "";
            Timer lv_timer;
            // Variable Initialization
            lv_tag = lp_tag.ToString();
            lv_timer = (Timer)DataTableLoad0(true, ("HD_Timer_" + lv_tag));
            // Implementation
            return lv_timer;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回序号对应Timer标签句柄。使用"互动T_注册Timer"后，在参数填入注册序号可返回Timer标签（字符串）。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <returns></returns>
        public static string HD_ReturnTimerTagFromRegNum_String(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回序号对应Timer标签句柄。使用"互动T_注册Timer"后，在参数填入注册序号可返回Timer标签（整数）。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <returns></returns>
        public static int HD_ReturnTimerTagFromRegNum_Int(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return Convert.ToInt32(lv_tag);
        }

        /// <summary>
        /// 【MM_函数库】互动T_设置Timer状态。必须先"注册"获得功能库内部句柄，再使用本函数给Timer设定一个状态值，之后可用"互动T_返回Timer状态"。类型参数用以记录多个不同状态，仅当"类型"参数填Timer组ID转的Timer串时，状态值"true"和"false"是Timer的Timer组专用状态值，用于内部函数筛选Timer状态（相当于单位组单位索引是否有效），其他类型不会干扰系统内部，可随意填写。虽然注销时反向清空注册信息，但用"互动T_设定Timer状态/自定义值"创建的值需要手工填入""来排泄（非大量注销则提升内存量极小，可不管）。注：固有状态值是注册函数赋予的系统内部变量（相当于单位组单位是否活体），只能通过"互动T_注册Timer（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <param name="lp_stats">状态</param>
        public static void HD_SetTimerState(Timer lp_timer, string lp_key, string lp_stats)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = HD_RegTimerTagAndReturn(lp_timer);
            // Implementation
            DataTableSave0(true, ("HD_TimerState" + lv_str + "_" + lv_tag), lp_stats);
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer状态。使用"互动T_设定Timer状态"后可使用本函数，将本函数参数"类型"设为空时返回固有值。类型参数用以记录多个不同状态，仅当"类型"参数为Timer组ID转的字符串时，返回的状态值"true"和"false"是Timer的Timer组专用状态值，用于内部函数筛选Timer状态（相当于单位组单位索引是否有效）
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <returns></returns>
        public static string HD_ReturnTimerState(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_stats = DataTableLoad0(true, ("HD_TimerState" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动T_设置Timer自定义值。必须先"注册"获得功能库内部句柄，再使用本函数设定Timer的自定义值，之后可使用"互动T_返回Timer自定义值"，类型参数用以记录多个不同自定义值。注：固有自定义值是注册函数赋予的系统内部变量，只能通过"互动T_注册Timer（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <param name="lp_customValue">自定义值</param>
        public static void HD_SetTimerCV(Timer lp_timer, string lp_key, string lp_customValue)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = HD_RegTimerTagAndReturn(lp_timer);
            // Implementation
            DataTableSave0(true, ("HD_TimerCV" + lv_str + "_" + lv_tag), lp_customValue);
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer自定义值。使用"互动T_设定Timer自定义值"后可使用本函数，将本函数参数"类型"设为空时返回固有值，该参数用以记录多个不同自定义值
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <returns></returns>
        public static string HD_ReturnTimerCV(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_customValue = DataTableLoad0(true, ("HD_TimerCV" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer固有状态。必须先使用"互动T_注册Timer"才能返回到该值，固有状态是独一无二的标记（相当于单位组单位是否活体）
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        public static string HD_ReturnTimerState_Only(Timer lp_timer)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_stats = DataTableLoad0(true, ("HD_TimerState" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer固有自定义值。必须先使用"互动T_注册Timer"才能返回到该值，固有值是独一无二的标记
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        public static string HD_ReturnTimerCV_Only(Timer lp_timer)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_customValue = DataTableLoad0(true, ("HD_TimerCV" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动T_设置Timer的实数标记。必须先"注册"获得功能库内部句柄，再使用本函数让Timer携带一个实数值，之后可使用"互动T_返回Timer的实数标记"。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_realNumTag">实数标记</param>
        public static void HD_SetTimerDouble(Timer lp_timer, double lp_realNumTag)
        {
            // Variable Declarations
            string lv_tag = "";
            // Variable Initialization
            lv_tag = HD_RegTimerTagAndReturn(lp_timer);
            // Implementation
            DataTableSave0(true, ("HD_CDDouble_T_" + lv_tag), lp_realNumTag);
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer的实数标记。使用"互动T_设定Timer的实数标记"后可使用本函数。Timer组使用时，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        public static double HD_ReturnTimerDouble(Timer lp_timer)
        {
            // Variable Declarations
            string lv_tag = "";
            double lv_f;
            // Variable Initialization
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_f = (double)DataTableLoad0(true, ("HD_CDDouble_T_" + lv_tag));
            // Implementation
            return lv_f;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer标签句柄有效状态。将Timer视作独一无二的个体，标签是它本身，有效状态则类似"单位是否有效"，当使用"互动T_注册Timer"或"互动TG_添加Timer到Timer组"后激活Timer有效状态（值为"true"），除非使用"互动T_注册Timer（高级）"改写，否则直到注销才会摧毁
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <returns></returns>
        public static bool HD_ReturnIfTimerTag(Timer lp_timer)
        {
            // Variable Declarations
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfTimerTag" + "" + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动T_返回Timer注册状态。使用"互动T_注册Timer"或"互动TG_添加Timer到Timer组"后可使用本函数获取注册Timer在Key中的注册状态，该状态只能注销或从Timer组中移除时清空。Timer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Timer组转为Key
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_key">存储键区，默认值"_Timer"</param>
        /// <returns></returns>
        public static bool HD_ReturnIfTimerTagKey(Timer lp_timer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_tag = HD_ReturnTimerTag(lp_timer);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfTimerTag" + lv_str + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动TG_根据自定义值类型将Timer组排序。根据Timer携带的自定义值类型，对指定的Timer组元素进行冒泡排序。Timer组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Timer组名称</param>
        /// <param name="lp_cVStr">自定义值类型</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_TimerGSortCV(string lp_key, string lp_cVStr, bool lp_big)
        {
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            string lv_tagValuestr;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_TimerGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnTimerTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValuestr = HD_ReturnTimerCV(HD_ReturnTimerFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnTimerTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "TimerTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValuestr = HD_ReturnTimerCV(HD_ReturnTimerFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "TimerTag"), lv_a, lv_tag); //lv_tag放入新序号
                    //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_TimerGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动TG_Timer组排序。对指定的Timer组元素进行冒泡排序（根据元素句柄）。Timer组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Timer组名称</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_TimerGSort(string lp_key, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_TimerGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnTimerTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValue = lv_tag;
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnTimerTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "TimerTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValue = lv_tag;
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                                                                                    //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "TimerTag"), lv_a, lv_tag); //lv_tag放入新序号
                                                                               //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_TimerGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动TG_设定Timer的Timer组专用状态。给Timer组的Timer设定一个状态值（字符串），之后可用"互动T_返回Timer、互动TG_返回Timer组的Timer状态"。状态值"true"和"false"是Timer的Timer组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效），而本函数可以重设干预，影响函数"互动TG_返回Timer组元素数量（仅检索XX状态）"。与"互动T_设定Timer状态"功能相同，只是状态参数在Timer组中被固定为"Timer组变量的内部ID"。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_timerGroup"></param>
        /// <param name="lp_groupState"></param>
        public static void HD_SetTimerGState(Timer lp_timer, string lp_timerGroup, string lp_groupState)
        {
            HD_SetTimerState(lp_timer, lp_timerGroup, lp_groupState);
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer的Timer组专用状态。使用"互动T_设定Timer、互动TG_设定Timer组的Timer状态"后可使用本函数。与"互动T_返回Timer状态"功能相同，只是状态参数在Timer组中被固定为"Timer组变量的内部ID"。状态值"true"和"false"是Timer的Timer组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效）。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_timerGroup"></param>
        public static void HD_ReturnTimerGState(Timer lp_timer, string lp_timerGroup)
        {
            HD_ReturnTimerState(lp_timer, lp_timerGroup);
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组元素序号对应元素。返回Timer组元素序号指定Timer。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static Timer HD_ReturnTimerFromTimerGFunc(int lp_regNum, string lp_gs)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            Timer lv_timer;
            // Variable Initialization
            lv_str = (lp_gs + "Timer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_timer = (Timer)DataTableLoad0(true, ("HD_Timer_" + lv_tag));
            // Implementation
            return lv_timer;
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组元素总数。返回指定Timer组的元素数量。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnTimerGNumMax(string lp_gs)
        {
            return (int)DataTableLoad0(true, lp_gs + "TimerNum");
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组元素总数（仅检测Timer组专用状态="true"）。返回指定Timer组的元素数量。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnTimerGNumMax_StateTrueFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Timer lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnTimerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnTimerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnTimerState(lv_c, lp_gs);
                if ((lv_b == "true"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组元素总数（仅检测Timer组专用状态="false"）。返回指定Timer组的元素数量。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnTimerGNumMax_StateFalseFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Timer lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnTimerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnTimerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnTimerState(lv_c, lp_gs);
                if ((lv_b == "false"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组元素总数（仅检测Timer组无效专用状态："false"或""）。返回指定Timer组的元素数量（false、""、null）。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnTimerGNumMax_StateUselessFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Timer lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnTimerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnTimerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnTimerState(lv_c, lp_gs);
                if (((lv_b == "false") || (lv_b == "") || (lv_b == null)))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组元素总数（仅检测Timer组指定专用状态）。返回指定Timer组的元素数量。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_State">Timer组专用状态</param>
        /// <returns></returns>
        public static int HD_ReturnTimerGNumMax_StateFunc_Specify(string lp_gs, string lp_State)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            Timer lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnTimerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnTimerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnTimerState(lv_c, lp_gs);
                if ((lv_b == lp_State))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动TG_添加Timer到Timer组。相同Timer被认为是同一个，非高级功能不提供专用状态检查，如果Timer没有设置过Timer组专用状态，那么首次添加到Timer组不会赋予"true"（之后可通过"互动T_设定Timer状态"、"互动TG_设定Timer组的Timer状态"修改）。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddTimerToGroup_Simple(Timer lp_timer, string lp_gs)
        {
            HD_RegTimer_Simple(lp_timer, lp_gs);
        }

        /// <summary>
        /// 【MM_函数库】互动TG_添加Timer到Timer组（高级）。相同Timer被认为是同一个，高级功能提供专用状态检查，如果Timer没有设置过Timer组专用状态，那么首次添加到Timer组会赋予"true"（之后可通过"互动T_设定Timer状态"、"互动TG_设定Timer组的Timer状态"修改）。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddTimerToGroup(Timer lp_timer, string lp_gs)
        {
            HD_RegTimer_Simple(lp_timer, lp_gs);
            if (DataTableKeyExists(true, ("HD_TimerState" + lp_gs + "Timer_" + HD_RegTimerTagAndReturn(lp_timer))) == false)
            {
                DataTableSave0(true, ("HD_TimerState" + lp_gs + "Timer_" + HD_RegTimerTagAndReturn(lp_timer)), "true");
                //Console.WriteLine(lp_gs + "=>" + HD_RegTimerTagAndReturn(lp_timer));
            }
        }

        /// <summary>
        /// 【MM_函数库】互动TG_移除Timer组中的元素。使用"互动TG_添加Timer到Timer组"后可使用本函数进行移除元素。移除使用了"互动T_移除Timer"，同一个存储区（Timer组ID）序号重排，移除时该存储区如有其他操作会排队等待。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_timer"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_ClearTimerFromGroup(Timer lp_timer, string lp_gs)
        {
            HD_RemoveTimer(lp_timer, lp_gs);
        }

        //互动TG_为Timer组中的每个序号
        //GE（星际2的Galaxy Editor）的宏让编辑器保存时自动生成脚本并整合进脚本进行格式调整，C#仅参考需自行编写
        // #AUTOVAR(vs, string) = "#PARAM(group)";//"#PARAM(group)"是与字段、变量名一致的元素组名称，宏去声明string类型名为“Auto随机编号_vs”的自动变量，然后=右侧字符
        // #AUTOVAR(ae) = HD_ReturnTimerNumMax(#AUTOVAR(vs));//宏去声明默认int类型名为“Auto随机编号_ae”的自动变量，然后=右侧字符
        // #INITAUTOVAR(ai,increment)//宏去声明int类型名为“Auto随机编号_ai”的自动变量，用于下面for循环增量（increment是传入参数）
        // #PARAM(var) = #PARAM(s);//#PARAM(var)是传进来的参数，用作“当前被挑选到的元素”（任意变量-整数 lp_var）， #PARAM(s)是传进来的参数用作"开始"（int lp_s）
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #PARAM(var) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #PARAM(var) >= #AUTOVAR(ae)) ) ; #PARAM(var) += #AUTOVAR(ai) ) {
        //     #SUBFUNCS(actions)//代表用户GUI填写的所有动作
        // }

        /// <summary>
        /// 【MM_函数库】互动TG_为Timer组中的每个序号。每次挑选的元素序号会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素序号，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachTimerNumFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            int lv_ae = HD_ReturnTimerNumMax(lp_gs);
            int lv_var = lp_start;
            int lv_ai = lp_increment;
            for (; (lv_ai >= 0 && lv_var <= lv_ae) || (lv_ai < 0 && lv_var >= lv_ae); lv_var += lv_ai)
            {
                lp_funcref(lv_var);//用户填写的所有动作
            }
        }

        //互动TG_为Timer组中的每个元素
        // #AUTOVAR(vs, string) = "#PARAM(group)";
        // #AUTOVAR(ae) = HD_ReturnTimerNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= #PARAM(s);
        // #INITAUTOVAR(ai,increment)
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     DataTableSave(false, "TimerGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)), HD_ReturnTimerFromRegNum(#AUTOVAR(va),#AUTOVAR(vs)));
        // }
        // #AUTOVAR(va)= #PARAM(s);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #PARAM(var) = DataTableLoad(false, "TimerGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)));
        //     #SUBFUNCS(actions)
        // }

        /// <summary>
        /// 【MM_函数库】互动TG_为Timer组中的每个元素。每次挑选的元素会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachTimerFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            string lv_vs = lp_gs;
            int lv_ae = HD_ReturnTimerNumMax(lv_vs);
            int lv_va = lp_start;
            int lv_ai = lp_increment;
            Timer lv_timer;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                DataTableSave0(false, "TimerGFor" + lv_vs + lv_va.ToString(), HD_ReturnTimerFromRegNum(lv_va, lv_vs));
            }
            lv_va = lp_start;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                lv_timer = (Timer)DataTableLoad0(false, "TimerGFor" + lv_vs + lv_va.ToString());
                lp_funcref(lv_timer);//用户填写的所有动作
            }
        }

        /// <summary>
        /// 【MM_函数库】互动TG_返回Timer组中随机元素。返回指定Timer组中的随机Timer。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static Timer HD_ReturnRandomTimerFromTimerGFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_num;
            int lv_a;
            Timer lv_c = null;
            // Variable Initialization
            lv_num = HD_ReturnTimerNumMax(lp_gs);
            // Implementation
            if ((lv_num >= 1))
            {
                lv_a = RandomInt(1, lv_num);
                lv_c = HD_ReturnTimerFromRegNum(lv_a, lp_gs);
            }
            return lv_c;
        }

        //互动TG_添加Timer组到Timer组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnTimerNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnTimerFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_AddTimerToGroup(#AUTOVAR(var), #AUTOVAR(vsb));
        // }


        /// <summary>
        /// 【MM_函数库】互动TG_添加Timer组到Timer组。添加一个Timer组A的元素到另一个Timer组B，相同Timer被认为是同一个。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_AddTimerGToTimerG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnTimerNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            Timer lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnTimerFromRegNum(lv_va, lv_vsa);
                HD_AddTimerToGroup(lv_var, lv_vsb);
            }
        }

        //互动TG_从Timer组移除Timer组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnTimerNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnTimerFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_RemoveTimer(#AUTOVAR(var), #AUTOVAR(vsb));
        // }

        /// <summary>
        /// 【MM_函数库】互动TG_从Timer组移除Timer组。将Timer组A的元素从Timer组B中移除，相同Timer被认为是同一个。移除使用了"互动T_移除Timer"，同一个存储区（Timer组ID）序号重排，移除时该存储区如有其他操作会排队等待。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_ClearTimerGFromTimerG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnTimerNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            Timer lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnTimerFromRegNum(lv_va, lv_vsa);
                HD_RemoveTimer(lv_var, lv_vsb);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动TG_移除Timer组全部元素。将Timer组（Key区）存储的元素全部移除，相同Timer被认为是同一个。移除时同一个存储区（Timer组ID）序号不进行重排，但该存储区如有其他操作会排队等待。Timer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Timer组到Timer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Timer组名称</param>
        public static void HD_RemoveTimerGAll(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            // Variable Initialization
            lv_str = (lp_key + "Timer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_TimerGroup" + lv_str, 1);
            for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
            {
                lv_tag = DataTableLoad1(true, (lp_key + "TimerTag"), lv_a).ToString();
                lv_num -= 1;
                DataTableClear0(true, "HD_IfTimerTag" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_TimerCV" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_TimerState" + lv_str + "_" + lv_tag);
                DataTableSave0(true, (lp_key + "TimerNum"), lv_num);
            }
            DataTableSave0(true, "Key_TimerGroup" + lv_str, 0);
        }

        //--------------------------------------------------------------------------------------------------
        // 计时器组End
        //--------------------------------------------------------------------------------------------------

        #endregion

        #region 数字

        //提示：可以将数字作为模板修改后产生其他类型
        //提示：尽可能使用对口类型，以防值类型与引用类型发生转换时拆装箱降低性能

        //--------------------------------------------------------------------------------------------------
        // 数字组Start
        //--------------------------------------------------------------------------------------------------

        /// <summary>
        /// 【MM_函数库】互动I_注册Integer标签句柄并返回。为Integer自动设置新的标签句柄，重复时会返回已注册的Integer标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        private static int HD_RegIntegerTagAndReturn_Int(int lp_integer)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_IntegerJBNum");
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_IntegerJBNum", lv_j);
                DataTableSave0(true, ("HD_Integer_" + lv_j.ToString()), lp_integer);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((int)DataTableLoad0(true, ("HD_Integer_" + lv_j.ToString())) == lp_integer)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_IntegerJBNum", lv_j);
                            DataTableSave0(true, ("HD_Integer_" + lv_j.ToString()), lp_integer);
                        }
                    }
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer已注册标签句柄。返回一个Integer的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        public static int HD_ReturnIntegerTag_Int(int lp_integer)
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_IntegerJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((int)DataTableLoad0(true, "HD_Integer_" + lv_j.ToString()) == lp_integer)
                {
                    break;
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动I_注册Integer标签句柄并返回。为Integer自动设置新的标签句柄，重复时会返回已注册的Integer标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        private static string HD_RegIntegerTagAndReturn(int lp_integer)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_IntegerJBNum");
            lv_tag = "";
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_IntegerJBNum", lv_j);
                DataTableSave0(true, ("HD_Integer_" + lv_j.ToString()), lp_integer);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((int)DataTableLoad0(true, "HD_Integer_" + lv_j.ToString()) == lp_integer)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_IntegerJBNum", lv_j);
                            DataTableSave0(true, ("HD_Integer_" + lv_j.ToString()), lp_integer);
                        }
                    }
                }
            }
            lv_tag = lv_j.ToString();
            //Console.WriteLine(("Tag：" + lv_tag));
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer已注册标签句柄。返回一个Integer的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        public static string HD_ReturnIntegerTag(int lp_integer)
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_IntegerJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((int)DataTableLoad0(true, "HD_Integer_" + lv_j.ToString()) == lp_integer)
                {
                    lv_tag = lv_j.ToString();
                    break;
                }
            }
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动I_注册Integer(高级)。在指定Key存入Integer，固有状态、自定义值是Integer独一无二的标志（本函数重复注册会刷新），之后可用互动I_"返回Integer注册总数"、"返回Integer序号"、"返回序号对应Integer"、"返回序号对应Integer标签"、"返回Integer自定义值"。Integer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Integer组转为Key。首次注册时固有状态为true（相当于单位组单位活体），如需另外设置多个标记可使用"互动I_设定Integer状态/自定义值"
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <param name="lp_inherentStats">固有状态</param>
        /// <param name="lp_inherentCustomValue">固有自定义值</param>
        public static void HD_RegInteger(int lp_integer, string lp_key, string lp_inherentStats, string lp_inherentCustomValue)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegIntegerTagAndReturn(lp_integer);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfIntegerTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfIntegerTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfIntegerTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfIntegerTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            DataTableSave0(true, ("HD_IntegerState" + "" + "_" + lv_tagStr), lp_inherentStats);
            DataTableSave0(true, ("HD_IntegerCV" + "" + "_" + lv_tagStr), lp_inherentCustomValue);
        }

        /// <summary>
        /// 【MM_函数库】互动I_注册Integer。在指定Key存入Integer，固有状态、自定义值是Integer独一无二的标志（本函数重复注册不会刷新），之后可用互动I_"返回Integer注册总数"、"返回Integer序号"、"返回序号对应Integer"、"返回Integer自定义值"。Integer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Integer组转为Key。首次注册时固有状态为true（相当于单位组单位活体），之后只能通过"互动I_注册Integer（高级）"改写，如需另外设置多个标记可使用"互动I_设定Integer状态/自定义值"
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        public static void HD_RegInteger_Simple(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegIntegerTagAndReturn(lp_integer);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfIntegerTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfIntegerTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfIntegerTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfIntegerTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            if ((DataTableKeyExists(true, ("HD_Integer" + "State" + "_" + lv_tag.ToString())) == false))
            {
                DataTableSave1(true, (("HD_Integer" + "State")), lv_tag, "true");
            }
        }

        /// <summary>
        /// 【MM_函数库】互动I_注销Integer。用"互动I_注册Integer"到Key，之后可用本函数彻底摧毁注册信息并将序号重排（包括Integer标签有效状态、固有状态及自定义值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动I_设定Integer状态"让Integer状态失效（类似单位组的单位活体状态）。Integer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Integer组转为Key。本函数无法摧毁用"互动I_设定Integer状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Integer组变量ID时会清空Integer组专用状态
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        public static void HD_DestroyInteger(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_IntegerGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "IntegerTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfIntegerTag_" + lv_tag);
                        DataTableClear0(true, "HD_IfIntegerTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_Integer_" + lv_tag);
                        DataTableClear0(true, "HD_IntegerCV_" + lv_tag);
                        DataTableClear0(true, "HD_IntegerState_" + lv_tag);
                        DataTableClear0(true, "HD_IntegerCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_IntegerState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "IntegerNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "IntegerTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "IntegerTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_IntegerGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动I_移除Integer。用"互动I_注册Integer"到Key，之后可用本函数仅摧毁Key区注册的信息并将序号重排，用于Integer组或多个键区仅移除Integer（保留Integer标签有效状态、固有值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动I_设定Integer状态"让Integer状态失效（类似单位组的单位活体状态）。Integer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Integer组转为Key。本函数无法摧毁用"互动I_设定Integer状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填Integer组变量ID时会清空Integer组专用状态
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        public static void HD_RemoveInteger(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_IntegerGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "IntegerTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfIntegerTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_IntegerCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_IntegerState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "IntegerNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "IntegerTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "IntegerTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_IntegerGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer注册总数。必须先使用"互动I_注册Integer"才能返回指定Key里的注册总数。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key。
        /// </summary>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerNumMax(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            return lv_num;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer序号。使用"互动I_注册Integer"后使用本函数可返回Key里的注册序号，Key无元素返回0，Key有元素但对象不在里面则返回-1，Integer标签尚未注册则返回-2。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerNum(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_i;
            string lv_tag = "";
            int lv_torf;
            // Automatic Variable Declarations
            const int auto_n = 1;
            int auto_i;
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_torf = -1;
            // Implementation
            for (auto_i = 1; auto_i <= auto_n; auto_i += 1)
            {
                if ((lv_tag != null))
                {
                    lv_torf = -2;
                    break;
                }
                if ((lv_num == 0))
                {
                    lv_torf = 0;
                }
                else
                {
                    if ((lv_num >= 1))
                    {
                        auto_ae = lv_num;
                        auto_var = 1;
                        for (; auto_var <= auto_ae; auto_var += 1)
                        {
                            lv_i = auto_var;
                            if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tag))
                            {
                                lv_torf = lv_i;
                                break;
                            }
                        }
                    }
                }
            }
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回序号对应Integer。使用"互动I_注册Integer"后，在参数填入注册序号可返回Integer。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_regNum"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerFromRegNum(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            int lv_integer;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_integer = (int)DataTableLoad0(true, ("HD_Integer_" + lv_tag));
            // Implementation
            return lv_integer;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回句柄标签对应Integer。使用"互动I_注册Integer"后，在参数填入句柄标签（整数）可返回Integer，标签是Integer的句柄。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_tag">句柄标签</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerFromTag(int lp_tag)
        {
            // Variable Declarations
            string lv_tag = "";
            int lv_integer;
            // Variable Initialization
            lv_tag = lp_tag.ToString();
            lv_integer = (int)DataTableLoad0(true, ("HD_Integer_" + lv_tag));
            // Implementation
            return lv_integer;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回序号对应Integer标签句柄。使用"互动I_注册Integer"后，在参数填入注册序号可返回Integer标签（字符串）。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <returns></returns>
        public static string HD_ReturnIntegerTagFromRegNum_String(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回序号对应Integer标签句柄。使用"互动I_注册Integer"后，在参数填入注册序号可返回Integer标签（整数）。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerTagFromRegNum_Int(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return Convert.ToInt32(lv_tag);
        }

        /// <summary>
        /// 【MM_函数库】互动I_设置Integer状态。必须先"注册"获得功能库内部句柄，再使用本函数给Integer设定一个状态值，之后可用"互动I_返回Integer状态"。类型参数用以记录多个不同状态，仅当"类型"参数填Integer组ID转的Integer串时，状态值"true"和"false"是Integer的Integer组专用状态值，用于内部函数筛选Integer状态（相当于单位组单位索引是否有效），其他类型不会干扰系统内部，可随意填写。虽然注销时反向清空注册信息，但用"互动I_设定Integer状态/自定义值"创建的值需要手工填入""来排泄（非大量注销则提升内存量极小，可不管）。注：固有状态值是注册函数赋予的系统内部变量（相当于单位组单位是否活体），只能通过"互动I_注册Integer（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <param name="lp_stats">状态</param>
        public static void HD_SetIntegerState(int lp_integer, string lp_key, string lp_stats)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = HD_RegIntegerTagAndReturn(lp_integer);
            // Implementation
            DataTableSave0(true, ("HD_IntegerState" + lv_str + "_" + lv_tag), lp_stats);
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer状态。使用"互动I_设定Integer状态"后可使用本函数，将本函数参数"类型"设为空时返回固有值。类型参数用以记录多个不同状态，仅当"类型"参数为Integer组ID转的字符串时，返回的状态值"true"和"false"是Integer的Integer组专用状态值，用于内部函数筛选Integer状态（相当于单位组单位索引是否有效）
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <returns></returns>
        public static string HD_ReturnIntegerState(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_stats = DataTableLoad0(true, ("HD_IntegerState" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动I_设置Integer自定义值。必须先"注册"获得功能库内部句柄，再使用本函数设定Integer的自定义值，之后可使用"互动I_返回Integer自定义值"，类型参数用以记录多个不同自定义值。注：固有自定义值是注册函数赋予的系统内部变量，只能通过"互动I_注册Integer（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <param name="lp_customValue">自定义值</param>
        public static void HD_SetIntegerCV(int lp_integer, string lp_key, string lp_customValue)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = HD_RegIntegerTagAndReturn(lp_integer);
            // Implementation
            DataTableSave0(true, ("HD_IntegerCV" + lv_str + "_" + lv_tag), lp_customValue);
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer自定义值。使用"互动I_设定Integer自定义值"后可使用本函数，将本函数参数"类型"设为空时返回固有值，该参数用以记录多个不同自定义值
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <returns></returns>
        public static string HD_ReturnIntegerCV(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_customValue = DataTableLoad0(true, ("HD_IntegerCV" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer固有状态。必须先使用"互动I_注册Integer"才能返回到该值，固有状态是独一无二的标记（相当于单位组单位是否活体）
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        public static string HD_ReturnIntegerState_Only(int lp_integer)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_stats = DataTableLoad0(true, ("HD_IntegerState" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer固有自定义值。必须先使用"互动I_注册Integer"才能返回到该值，固有值是独一无二的标记
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        public static string HD_ReturnIntegerCV_Only(int lp_integer)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_customValue = DataTableLoad0(true, ("HD_IntegerCV" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动I_设置Integer的实数标记。必须先"注册"获得功能库内部句柄，再使用本函数让Integer携带一个实数值，之后可使用"互动I_返回Integer的实数标记"。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_realNumTag">实数标记</param>
        public static void HD_SetIntegerDouble(int lp_integer, double lp_realNumTag)
        {
            // Variable Declarations
            string lv_tag = "";
            // Variable Initialization
            lv_tag = HD_RegIntegerTagAndReturn(lp_integer);
            // Implementation
            DataTableSave0(true, ("HD_CDDouble_T_" + lv_tag), lp_realNumTag);
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer的实数标记。使用"互动I_设定Integer的实数标记"后可使用本函数。Integer组使用时，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        public static double HD_ReturnIntegerDouble(int lp_integer)
        {
            // Variable Declarations
            string lv_tag = "";
            double lv_f;
            // Variable Initialization
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_f = (double)DataTableLoad0(true, ("HD_CDDouble_T_" + lv_tag));
            // Implementation
            return lv_f;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer标签句柄有效状态。将Integer视作独一无二的个体，标签是它本身，有效状态则类似"单位是否有效"，当使用"互动I_注册Integer"或"互动IG_添加Integer到Integer组"后激活Integer有效状态（值为"true"），除非使用"互动I_注册Integer（高级）"改写，否则直到注销才会摧毁
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <returns></returns>
        public static bool HD_ReturnIfIntegerTag(int lp_integer)
        {
            // Variable Declarations
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfIntegerTag" + "" + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动I_返回Integer注册状态。使用"互动I_注册Integer"或"互动IG_添加Integer到Integer组"后可使用本函数获取注册Integer在Key中的注册状态，该状态只能注销或从Integer组中移除时清空。Integer组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将Integer组转为Key
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_key">存储键区，默认值"_Integer"</param>
        /// <returns></returns>
        public static bool HD_ReturnIfIntegerTagKey(int lp_integer, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_tag = HD_ReturnIntegerTag(lp_integer);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfIntegerTag" + lv_str + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动IG_根据自定义值类型将Integer组排序。根据Integer携带的自定义值类型，对指定的Integer组元素进行冒泡排序。Integer组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Integer组名称</param>
        /// <param name="lp_cVStr">自定义值类型</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_IntegerGSortCV(string lp_key, string lp_cVStr, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            string lv_tagValuestr;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_IntegerGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnIntegerTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValuestr = HD_ReturnIntegerCV(HD_ReturnIntegerFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnIntegerTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "IntegerTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValuestr = HD_ReturnIntegerCV(HD_ReturnIntegerFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "IntegerTag"), lv_a, lv_tag); //lv_tag放入新序号
                    //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_IntegerGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动IG_Integer组排序。对指定的Integer组元素进行冒泡排序（根据元素句柄）。Integer组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Integer组名称</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_IntegerGSort(string lp_key, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_IntegerGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnIntegerTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValue = lv_tag;
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnIntegerTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "IntegerTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValue = lv_tag;
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                                                                                    //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "IntegerTag"), lv_a, lv_tag); //lv_tag放入新序号
                                                                                 //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_IntegerGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动IG_设定Integer的Integer组专用状态。给Integer组的Integer设定一个状态值（字符串），之后可用"互动I_返回Integer、互动IG_返回Integer组的Integer状态"。状态值"true"和"false"是Integer的Integer组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效），而本函数可以重设干预，影响函数"互动IG_返回Integer组元素数量（仅检索XX状态）"。与"互动I_设定Integer状态"功能相同，只是状态参数在Integer组中被固定为"Integer组变量的内部ID"。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_integerGroup"></param>
        /// <param name="lp_groupState"></param>
        public static void HD_SetIntegerGState(int lp_integer, string lp_integerGroup, string lp_groupState)
        {
            HD_SetIntegerState(lp_integer, lp_integerGroup, lp_groupState);
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer的Integer组专用状态。使用"互动I_设定Integer、互动IG_设定Integer组的Integer状态"后可使用本函数。与"互动I_返回Integer状态"功能相同，只是状态参数在Integer组中被固定为"Integer组变量的内部ID"。状态值"true"和"false"是Integer的Integer组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效）。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_integerGroup"></param>
        public static void HD_ReturnIntegerGState(int lp_integer, string lp_integerGroup)
        {
            HD_ReturnIntegerState(lp_integer, lp_integerGroup);
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组元素序号对应元素。返回Integer组元素序号指定Integer。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerFromIntegerGFunc(int lp_regNum, string lp_gs)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            int lv_integer;
            // Variable Initialization
            lv_str = (lp_gs + "Integer");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_integer = (int)DataTableLoad0(true, ("HD_Integer_" + lv_tag));
            // Implementation
            return lv_integer;
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组元素总数。返回指定Integer组的元素数量。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerGNumMax(string lp_gs)
        {
            return (int)DataTableLoad0(true, lp_gs + "IntegerNum");
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组元素总数（仅检测Integer组专用状态="true"）。返回指定Integer组的元素数量。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerGNumMax_StateTrueFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            int lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnIntegerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnIntegerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnIntegerState(lv_c, lp_gs);
                if ((lv_b == "true"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组元素总数（仅检测Integer组专用状态="false"）。返回指定Integer组的元素数量。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerGNumMax_StateFalseFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            int lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnIntegerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnIntegerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnIntegerState(lv_c, lp_gs);
                if ((lv_b == "false"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组元素总数（仅检测Integer组无效专用状态："false"或""）。返回指定Integer组的元素数量（false、""、null）。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerGNumMax_StateUselessFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            int lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnIntegerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnIntegerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnIntegerState(lv_c, lp_gs);
                if (((lv_b == "false") || (lv_b == "") || (lv_b == null)))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组元素总数（仅检测Integer组指定专用状态）。返回指定Integer组的元素数量。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_State">Integer组专用状态</param>
        /// <returns></returns>
        public static int HD_ReturnIntegerGNumMax_StateFunc_Specify(string lp_gs, string lp_State)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            int lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnIntegerNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnIntegerFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnIntegerState(lv_c, lp_gs);
                if ((lv_b == lp_State))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动IG_添加Integer到Integer组。相同Integer被认为是同一个，非高级功能不提供专用状态检查，如果Integer没有设置过Integer组专用状态，那么首次添加到Integer组不会赋予"true"（之后可通过"互动I_设定Integer状态"、"互动IG_设定Integer组的Integer状态"修改）。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddIntegerToGroup_Simple(int lp_integer, string lp_gs)
        {
            HD_RegInteger_Simple(lp_integer, lp_gs);
        }

        /// <summary>
        /// 【MM_函数库】互动IG_添加Integer到Integer组（高级）。相同Integer被认为是同一个，高级功能提供专用状态检查，如果Integer没有设置过Integer组专用状态，那么首次添加到Integer组会赋予"true"（之后可通过"互动I_设定Integer状态"、"互动IG_设定Integer组的Integer状态"修改）。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddIntegerToGroup(int lp_integer, string lp_gs)
        {
            HD_RegInteger_Simple(lp_integer, lp_gs);
            if (DataTableKeyExists(true, ("HD_IntegerState" + lp_gs + "Integer_" + HD_RegIntegerTagAndReturn(lp_integer))) == false)
            {
                DataTableSave0(true, ("HD_IntegerState" + lp_gs + "Integer_" + HD_RegIntegerTagAndReturn(lp_integer)), "true");
                //Console.WriteLine(lp_gs + "=>" + HD_RegIntegerTagAndReturn(lp_integer));
            }
        }

        /// <summary>
        /// 【MM_函数库】互动IG_移除Integer组中的元素。使用"互动IG_添加Integer到Integer组"后可使用本函数进行移除元素。移除使用了"互动I_移除Integer"，同一个存储区（Integer组ID）序号重排，移除时该存储区如有其他操作会排队等待。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_integer"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_ClearIntegerFromGroup(int lp_integer, string lp_gs)
        {
            HD_RemoveInteger(lp_integer, lp_gs);
        }

        //互动IG_为Integer组中的每个序号
        //GE（星际2的Galaxy Editor）的宏让编辑器保存时自动生成脚本并整合进脚本进行格式调整，C#仅参考需自行编写
        // #AUTOVAR(vs, string) = "#PARAM(group)";//"#PARAM(group)"是与字段、变量名一致的元素组名称，宏去声明string类型名为“Auto随机编号_vs”的自动变量，然后=右侧字符
        // #AUTOVAR(ae) = HD_ReturnIntegerNumMax(#AUTOVAR(vs));//宏去声明默认int类型名为“Auto随机编号_ae”的自动变量，然后=右侧字符
        // #INITAUTOVAR(ai,increment)//宏去声明int类型名为“Auto随机编号_ai”的自动变量，用于下面for循环增量（increment是传入参数）
        // #PARAM(var) = #PARAM(s);//#PARAM(var)是传进来的参数，用作“当前被挑选到的元素”（任意变量-整数 lp_var）， #PARAM(s)是传进来的参数用作"开始"（int lp_s）
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #PARAM(var) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #PARAM(var) >= #AUTOVAR(ae)) ) ; #PARAM(var) += #AUTOVAR(ai) ) {
        //     #SUBFUNCS(actions)//代表用户GUI填写的所有动作
        // }

        /// <summary>
        /// 【MM_函数库】互动IG_为Integer组中的每个序号。每次挑选的元素序号会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素序号，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachIntegerNumFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            int lv_ae = HD_ReturnIntegerNumMax(lp_gs);
            int lv_var = lp_start;
            int lv_ai = lp_increment;
            for (; (lv_ai >= 0 && lv_var <= lv_ae) || (lv_ai < 0 && lv_var >= lv_ae); lv_var += lv_ai)
            {
                lp_funcref(lv_var);//用户填写的所有动作
            }
        }

        //互动IG_为Integer组中的每个元素
        // #AUTOVAR(vs, string) = "#PARAM(group)";
        // #AUTOVAR(ae) = HD_ReturnIntegerNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= #PARAM(s);
        // #INITAUTOVAR(ai,increment)
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     DataTableSave(false, "IntegerGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)), HD_ReturnIntegerFromRegNum(#AUTOVAR(va),#AUTOVAR(vs)));
        // }
        // #AUTOVAR(va)= #PARAM(s);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #PARAM(var) = DataTableLoad(false, "IntegerGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)));
        //     #SUBFUNCS(actions)
        // }

        /// <summary>
        /// 【MM_函数库】互动IG_为Integer组中的每个元素。每次挑选的元素会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachIntegerFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            string lv_vs = lp_gs;
            int lv_ae = HD_ReturnIntegerNumMax(lv_vs);
            int lv_va = lp_start;
            int lv_ai = lp_increment;
            int lv_integer;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                DataTableSave0(false, "IntegerGFor" + lv_vs + lv_va.ToString(), HD_ReturnIntegerFromRegNum(lv_va, lv_vs));
            }
            lv_va = lp_start;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                lv_integer = (int)DataTableLoad0(false, "IntegerGFor" + lv_vs + lv_va.ToString());
                lp_funcref(lv_integer);//用户填写的所有动作
            }
        }

        /// <summary>
        /// 【MM_函数库】互动IG_返回Integer组中随机元素。返回指定Integer组中的随机Integer。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnRandomIntegerFromIntegerGFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_num;
            int lv_a;
            int lv_c = 0;
            // Variable Initialization
            lv_num = HD_ReturnIntegerNumMax(lp_gs);
            // Implementation
            if ((lv_num >= 1))
            {
                lv_a = RandomInt(1, lv_num);
                lv_c = HD_ReturnIntegerFromRegNum(lv_a, lp_gs);
            }
            return lv_c;
        }

        //互动IG_添加Integer组到Integer组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnIntegerNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnIntegerFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_AddIntegerToGroup(#AUTOVAR(var), #AUTOVAR(vsb));
        // }


        /// <summary>
        /// 【MM_函数库】互动IG_添加Integer组到Integer组。添加一个Integer组A的元素到另一个Integer组B，相同Integer被认为是同一个。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_AddIntegerGToIntegerG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnIntegerNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            int lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnIntegerFromRegNum(lv_va, lv_vsa);
                HD_AddIntegerToGroup(lv_var, lv_vsb);
            }
        }

        //互动IG_从Integer组移除Integer组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnIntegerNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnIntegerFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_RemoveInteger(#AUTOVAR(var), #AUTOVAR(vsb));
        // }

        /// <summary>
        /// 【MM_函数库】互动IG_从Integer组移除Integer组。将Integer组A的元素从Integer组B中移除，相同Integer被认为是同一个。移除使用了"互动I_移除Integer"，同一个存储区（Integer组ID）序号重排，移除时该存储区如有其他操作会排队等待。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_ClearIntegerGFromIntegerG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnIntegerNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            int lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnIntegerFromRegNum(lv_va, lv_vsa);
                HD_RemoveInteger(lv_var, lv_vsb);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动IG_移除Integer组全部元素。将Integer组（Key区）存储的元素全部移除，相同Integer被认为是同一个。移除时同一个存储区（Integer组ID）序号不进行重排，但该存储区如有其他操作会排队等待。Integer组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加Integer组到Integer组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_key">存储键区，默认填Integer组名称</param>
        public static void HD_RemoveIntegerGAll(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            // Variable Initialization
            lv_str = (lp_key + "Integer");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_IntegerGroup" + lv_str, 1);
            for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
            {
                lv_tag = DataTableLoad1(true, (lp_key + "IntegerTag"), lv_a).ToString();
                lv_num -= 1;
                DataTableClear0(true, "HD_IfIntegerTag" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_IntegerCV" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_IntegerState" + lv_str + "_" + lv_tag);
                DataTableSave0(true, (lp_key + "IntegerNum"), lv_num);
            }
            DataTableSave0(true, "Key_IntegerGroup" + lv_str, 0);
        }

        //--------------------------------------------------------------------------------------------------
        // 数字组End
        //--------------------------------------------------------------------------------------------------

        #endregion

        #region 字符

        //提示：可以将字符作为模板修改后产生其他类型
        //提示：尽可能使用对口类型，以防值类型与引用类型发生转换时拆装箱降低性能

        //--------------------------------------------------------------------------------------------------
        // 字符组Start
        //--------------------------------------------------------------------------------------------------

        /// <summary>
        /// 【MM_函数库】互动S_注册String标签句柄并返回。为String自动设置新的标签句柄，重复时会返回已注册的String标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        private static int HD_RegStringTagAndReturn_Int(string lp_string)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_StringJBNum");
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_StringJBNum", lv_j);
                DataTableSave0(true, ("HD_String_" + lv_j.ToString()), lp_string);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((string)DataTableLoad0(true, ("HD_String_" + lv_j.ToString())) == lp_string)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_StringJBNum", lv_j);
                            DataTableSave0(true, ("HD_String_" + lv_j.ToString()), lp_string);
                        }
                    }
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String已注册标签句柄。返回一个String的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        public static int HD_ReturnStringTag_Int(string lp_string)
        {
            // Variable Declarations
            int lv_jBNum;
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_StringJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((string)DataTableLoad0(true, "HD_String_" + lv_j.ToString()) == lp_string)
                {
                    break;
                }
            }
            return lv_j;
        }

        /// <summary>
        /// 【MM_函数库】互动S_注册String标签句柄并返回。为String自动设置新的标签句柄，重复时会返回已注册的String标签。这是一个内部函数，一般不需要自动使用
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        private static string HD_RegStringTagAndReturn(string lp_string)//内部使用
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_StringJBNum");
            lv_tag = "";
            // Implementation
            if ((lv_jBNum == 0))
            {
                lv_j = (lv_jBNum + 1);
                DataTableSave0(true, "HD_StringJBNum", lv_j);
                DataTableSave0(true, ("HD_String_" + lv_j.ToString()), lp_string);
            }
            else
            {
                auto_ae = lv_jBNum;
                auto_var = 1;
                for (; auto_var <= auto_ae; auto_var += 1)
                {
                    lv_j = auto_var;
                    if ((string)DataTableLoad0(true, "HD_String_" + lv_j.ToString()) == lp_string)
                    {
                        break;
                    }
                    else
                    {
                        if ((lv_j == lv_jBNum))
                        {
                            lv_j = (lv_jBNum + 1);
                            DataTableSave0(true, "HD_StringJBNum", lv_j);
                            DataTableSave0(true, ("HD_String_" + lv_j.ToString()), lp_string);
                        }
                    }
                }
            }
            lv_tag = lv_j.ToString();
            //Console.WriteLine(("Tag：" + lv_tag));
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String已注册标签句柄。返回一个String的已注册标签，如果失败返回null
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        public static string HD_ReturnStringTag(string lp_string)
        {
            // Variable Declarations
            int lv_jBNum;
            string lv_tag = "";
            int lv_j = 0;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_jBNum = (int)DataTableLoad0(true, "HD_StringJBNum");
            // Implementation
            auto_ae = lv_jBNum;
            auto_var = 1;
            for (; auto_var <= auto_ae; auto_var += 1)
            {
                lv_j = auto_var;
                if ((string)DataTableLoad0(true, "HD_String_" + lv_j.ToString()) == lp_string)
                {
                    lv_tag = lv_j.ToString();
                    break;
                }
            }
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动S_注册String(高级)。在指定Key存入String，固有状态、自定义值是String独一无二的标志（本函数重复注册会刷新），之后可用互动S_"返回String注册总数"、"返回String序号"、"返回序号对应String"、"返回序号对应String标签"、"返回String自定义值"。String组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将String组转为Key。首次注册时固有状态为true（相当于单位组单位活体），如需另外设置多个标记可使用"互动S_设定String状态/自定义值"
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <param name="lp_inherentStats">固有状态</param>
        /// <param name="lp_inherentCustomValue">固有自定义值</param>
        public static void HD_RegString(string lp_string, string lp_key, string lp_inherentStats, string lp_inherentCustomValue)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegStringTagAndReturn(lp_string);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfStringTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfStringTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfStringTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfStringTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            DataTableSave0(true, ("HD_StringState" + "" + "_" + lv_tagStr), lp_inherentStats);
            DataTableSave0(true, ("HD_StringCV" + "" + "_" + lv_tagStr), lp_inherentCustomValue);
        }

        /// <summary>
        /// 【MM_函数库】互动S_注册String。在指定Key存入String，固有状态、自定义值是String独一无二的标志（本函数重复注册不会刷新），之后可用互动S_"返回String注册总数"、"返回String序号"、"返回序号对应String"、"返回String自定义值"。String组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将String组转为Key。首次注册时固有状态为true（相当于单位组单位活体），之后只能通过"互动S_注册String（高级）"改写，如需另外设置多个标记可使用"互动S_设定String状态/自定义值"
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        public static void HD_RegString_Simple(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_jBNum;
            string lv_tagStr;
            int lv_tag;
            int lv_i;
            // Automatic Variable Declarations
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_jBNum = (int)DataTableLoad0(true, (lv_str + "JBNum"));
            lv_tagStr = "";
            // Implementation
            ThreadWait(lv_str);
            lv_tagStr = HD_RegStringTagAndReturn(lp_string);
            lv_tag = Convert.ToInt32(lv_tagStr);
            if ((lv_num == 0))
            {
                lv_i = (lv_num + 1);
                DataTableSave0(true, (lv_str + "Num"), lv_i);
                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                DataTableSave0(true, (("HD_IfStringTag" + "") + "_" + lv_tagStr), true);
                DataTableSave1(true, ("HD_IfStringTag" + lv_str), lv_tag, true);
            }
            else
            {
                if ((lv_num >= 1))
                {
                    auto_ae = lv_num;
                    auto_var = 1;
                    for (; auto_var <= auto_ae; auto_var += 1)
                    {
                        lv_i = auto_var;
                        if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tagStr))
                        {
                            break;
                        }
                        else
                        {
                            if ((lv_i == lv_num))
                            {
                                lv_i = (lv_num + 1);
                                DataTableSave0(true, (lv_str + "Num"), lv_i);
                                DataTableSave1(true, (lv_str + "Tag"), lv_i, lv_tagStr);
                                DataTableSave0(true, (("HD_IfStringTag" + "") + "_" + lv_tagStr), true);
                                DataTableSave1(true, ("HD_IfStringTag" + lv_str), lv_tag, true);
                            }
                        }
                    }
                }
            }
            if ((DataTableKeyExists(true, ("HD_String" + "State" + "_" + lv_tag.ToString())) == false))
            {
                DataTableSave1(true, (("HD_String" + "State")), lv_tag, "true");
            }
        }

        /// <summary>
        /// 【MM_函数库】互动S_注销String。用"互动S_注册String"到Key，之后可用本函数彻底摧毁注册信息并将序号重排（包括String标签有效状态、固有状态及自定义值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动S_设定String状态"让String状态失效（类似单位组的单位活体状态）。String组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将String组转为Key。本函数无法摧毁用"互动S_设定String状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填String组变量ID时会清空String组专用状态
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        public static void HD_DestroyString(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_StringGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "StringTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfStringTag_" + lv_tag);
                        DataTableClear0(true, "HD_IfStringTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_String_" + lv_tag);
                        DataTableClear0(true, "HD_StringCV_" + lv_tag);
                        DataTableClear0(true, "HD_StringState_" + lv_tag);
                        DataTableClear0(true, "HD_StringCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_StringState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "StringNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "StringTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "StringTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_StringGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动S_移除String。用"互动S_注册String"到Key，之后可用本函数仅摧毁Key区注册的信息并将序号重排，用于String组或多个键区仅移除String（保留String标签有效状态、固有值）。注册注销同时进行会排队等待0.0625s直到没有注销动作，注销并不提升多少内存只是变量内容清空并序号重利用，非特殊要求一般不注销，而是用"互动S_设定String状态"让String状态失效（类似单位组的单位活体状态）。String组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将String组转为Key。本函数无法摧毁用"互动S_设定String状态/自定义值"创建的状态和自定义值，需手工填入""来排泄（非大量注销则提升内存量极小，可不管）。本函数参数Key若填String组变量ID时会清空String组专用状态
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        public static void HD_RemoveString(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            int lv_b;
            string lv_c;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_c = "";
            // Implementation
            if ((lv_tag != null))
            {
                ThreadWait(lv_str);
                DataTableSave0(true, "Key_StringGroup" + lv_str, 1);
                for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
                {
                    if ((DataTableLoad1(true, (lp_key + "StringTag"), lv_a).ToString() == lv_tag))
                    {
                        lv_num -= 1;
                        DataTableClear0(true, "HD_IfStringTag" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_StringCV" + lv_str + "_" + lv_tag);
                        DataTableClear0(true, "HD_StringState" + lv_str + "_" + lv_tag);
                        DataTableSave0(true, (lp_key + "StringNum"), lv_num);
                        for (lv_b = lv_a; lv_b <= lv_num; lv_b += 1)
                        {
                            lv_c = DataTableLoad1(true, (lp_key + "StringTag"), lv_b + 1).ToString();
                            DataTableSave1(true, (lp_key + "StringTag"), lv_b, lv_c);
                        }
                        //注销后触发序号重列，这里-1可以让挑选回滚，以再次检查重排后的当前挑选序号
                        lv_a -= 1;
                    }
                }
                DataTableSave0(true, "Key_StringGroup" + lv_str, 0);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String注册总数。必须先使用"互动S_注册String"才能返回指定Key里的注册总数。String组使用时，可用"获取变量的内部名称"将String组转为Key。
        /// </summary>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <returns></returns>
        public static int HD_ReturnStringNumMax(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            return lv_num;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String序号。使用"互动S_注册String"后使用本函数可返回Key里的注册序号，Key无元素返回0，Key有元素但对象不在里面则返回-1，String标签尚未注册则返回-2。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <returns></returns>
        public static int HD_ReturnStringNum(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            int lv_i;
            string lv_tag = "";
            int lv_torf;
            // Automatic Variable Declarations
            const int auto_n = 1;
            int auto_i;
            int auto_ae;
            int auto_var;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_torf = -1;
            // Implementation
            for (auto_i = 1; auto_i <= auto_n; auto_i += 1)
            {
                if ((lv_tag != null))
                {
                    lv_torf = -2;
                    break;
                }
                if ((lv_num == 0))
                {
                    lv_torf = 0;
                }
                else
                {
                    if ((lv_num >= 1))
                    {
                        auto_ae = lv_num;
                        auto_var = 1;
                        for (; auto_var <= auto_ae; auto_var += 1)
                        {
                            lv_i = auto_var;
                            if ((DataTableLoad1(true, (lv_str + "Tag"), lv_i).ToString() == lv_tag))
                            {
                                lv_torf = lv_i;
                                break;
                            }
                        }
                    }
                }
            }
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回序号对应String。使用"互动S_注册String"后，在参数填入注册序号可返回String。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_regNum"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <returns></returns>
        public static string HD_ReturnStringFromRegNum(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_string;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_string = (string)DataTableLoad0(true, ("HD_String_" + lv_tag));
            // Implementation
            return lv_string;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回句柄标签对应String。使用"互动S_注册String"后，在参数填入句柄标签（整数）可返回String，标签是String的句柄。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_tag">句柄标签</param>
        /// <returns></returns>
        public static string HD_ReturnStringFromTag(int lp_tag)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_string;
            // Variable Initialization
            lv_tag = lp_tag.ToString();
            lv_string = (string)DataTableLoad0(true, ("HD_String_" + lv_tag));
            // Implementation
            return lv_string;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回序号对应String标签句柄。使用"互动S_注册String"后，在参数填入注册序号可返回String标签（字符串）。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <returns></returns>
        public static string HD_ReturnStringTagFromRegNum_String(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return lv_tag;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回序号对应String标签句柄。使用"互动S_注册String"后，在参数填入注册序号可返回String标签（整数）。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <returns></returns>
        public static int HD_ReturnStringTagFromRegNum_Int(int lp_regNum, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            // Implementation
            return Convert.ToInt32(lv_tag);
        }

        /// <summary>
        /// 【MM_函数库】互动S_设置String状态。必须先"注册"获得功能库内部句柄，再使用本函数给String设定一个状态值，之后可用"互动S_返回String状态"。类型参数用以记录多个不同状态，仅当"类型"参数填String组ID转的String串时，状态值"true"和"false"是String的String组专用状态值，用于内部函数筛选String状态（相当于单位组单位索引是否有效），其他类型不会干扰系统内部，可随意填写。虽然注销时反向清空注册信息，但用"互动S_设定String状态/自定义值"创建的值需要手工填入""来排泄（非大量注销则提升内存量极小，可不管）。注：固有状态值是注册函数赋予的系统内部变量（相当于单位组单位是否活体），只能通过"互动S_注册String（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <param name="lp_stats">状态</param>
        public static void HD_SetStringState(string lp_string, string lp_key, string lp_stats)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = HD_RegStringTagAndReturn(lp_string);
            // Implementation
            DataTableSave0(true, ("HD_StringState" + lv_str + "_" + lv_tag), lp_stats);
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String状态。使用"互动S_设定String状态"后可使用本函数，将本函数参数"类型"设为空时返回固有值。类型参数用以记录多个不同状态，仅当"类型"参数为String组ID转的字符串时，返回的状态值"true"和"false"是String的String组专用状态值，用于内部函数筛选String状态（相当于单位组单位索引是否有效）
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储类型，默认值"State"</param>
        /// <returns></returns>
        public static string HD_ReturnStringState(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_stats = DataTableLoad0(true, ("HD_StringState" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动S_设置String自定义值。必须先"注册"获得功能库内部句柄，再使用本函数设定String的自定义值，之后可使用"互动S_返回String自定义值"，类型参数用以记录多个不同自定义值。注：固有自定义值是注册函数赋予的系统内部变量，只能通过"互动S_注册String（高级）"函数或将本函数参数"类型"设为空时改写
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <param name="lp_customValue">自定义值</param>
        public static void HD_SetStringCV(string lp_string, string lp_key, string lp_customValue)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = HD_RegStringTagAndReturn(lp_string);
            // Implementation
            DataTableSave0(true, ("HD_StringCV" + lv_str + "_" + lv_tag), lp_customValue);
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String自定义值。使用"互动S_设定String自定义值"后可使用本函数，将本函数参数"类型"设为空时返回固有值，该参数用以记录多个不同自定义值
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储类型，默认值"A"</param>
        /// <returns></returns>
        public static string HD_ReturnStringCV(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_customValue = DataTableLoad0(true, ("HD_StringCV" + lv_str + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String固有状态。必须先使用"互动S_注册String"才能返回到该值，固有状态是独一无二的标记（相当于单位组单位是否活体）
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        public static string HD_ReturnStringState_Only(string lp_string)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_stats;
            // Variable Initialization
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_stats = DataTableLoad0(true, ("HD_StringState" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_stats;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String固有自定义值。必须先使用"互动S_注册String"才能返回到该值，固有值是独一无二的标记
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        public static string HD_ReturnStringCV_Only(string lp_string)
        {
            // Variable Declarations
            string lv_tag = "";
            string lv_customValue;
            // Variable Initialization
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_customValue = DataTableLoad0(true, ("HD_StringCV" + "" + "_" + lv_tag)).ToString();
            // Implementation
            return lv_customValue;
        }

        /// <summary>
        /// 【MM_函数库】互动S_设置String的实数标记。必须先"注册"获得功能库内部句柄，再使用本函数让String携带一个实数值，之后可使用"互动S_返回String的实数标记"。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_realNumTag">实数标记</param>
        public static void HD_SetStringDouble(string lp_string, double lp_realNumTag)
        {
            // Variable Declarations
            string lv_tag = "";
            // Variable Initialization
            lv_tag = HD_RegStringTagAndReturn(lp_string);
            // Implementation
            DataTableSave0(true, ("HD_CDDouble_T_" + lv_tag), lp_realNumTag);
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String的实数标记。使用"互动S_设定String的实数标记"后可使用本函数。String组使用时，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        public static double HD_ReturnStringDouble(string lp_string)
        {
            // Variable Declarations
            string lv_tag = "";
            double lv_f;
            // Variable Initialization
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_f = (double)DataTableLoad0(true, ("HD_CDDouble_T_" + lv_tag));
            // Implementation
            return lv_f;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String标签句柄有效状态。将String视作独一无二的个体，标签是它本身，有效状态则类似"单位是否有效"，当使用"互动S_注册String"或"互动SG_添加String到String组"后激活String有效状态（值为"true"），除非使用"互动S_注册String（高级）"改写，否则直到注销才会摧毁
        /// </summary>
        /// <param name="lp_string"></param>
        /// <returns></returns>
        public static bool HD_ReturnIfStringTag(string lp_string)
        {
            // Variable Declarations
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfStringTag" + "" + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动S_返回String注册状态。使用"互动S_注册String"或"互动SG_添加String到String组"后可使用本函数获取注册String在Key中的注册状态，该状态只能注销或从String组中移除时清空。String组使用时，Key被强制为变量ID，可用"获取变量的内部名称"将String组转为Key
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_key">存储键区，默认值"_String"</param>
        /// <returns></returns>
        public static bool HD_ReturnIfStringTagKey(string lp_string, string lp_key)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            bool lv_torf;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_tag = HD_ReturnStringTag(lp_string);
            lv_torf = (bool)DataTableLoad0(true, ("HD_IfStringTag" + lv_str + "_" + lv_tag));
            // Implementation
            return lv_torf;
        }

        /// <summary>
        /// 【MM_函数库】互动SG_根据自定义值类型将String组排序。根据String携带的自定义值类型，对指定的String组元素进行冒泡排序。String组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填String组名称</param>
        /// <param name="lp_cVStr">自定义值类型</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_StringGSortCV(string lp_key, string lp_cVStr, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            string lv_tagValuestr;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "String");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_StringGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnStringTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValuestr = HD_ReturnStringCV(HD_ReturnStringFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnStringTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "StringTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValuestr = HD_ReturnStringCV(HD_ReturnStringFromTag(lv_tag), lp_cVStr);
                lv_tagValue = Convert.ToInt32(lv_tagValuestr);
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "StringTag"), lv_a, lv_tag); //lv_tag放入新序号
                    //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_StringGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动SG_String组排序。对指定的String组元素进行冒泡排序（根据元素句柄）。String组变量字符可通过"转换变量内部名称"获得
        /// </summary>
        /// <param name="lp_key">存储键区，默认填String组名称</param>
        /// <param name="lp_big">是否大值靠前</param>
        public static void HD_StringGSort(string lp_key, bool lp_big)
        {
            // Automatic Variable Declarations
            // Implementation
            // Variable Declarations
            int lv_a;
            int lv_b;
            int lv_c;
            bool lv_bool;
            int lv_tag;
            int lv_tagValue;
            string lv_str;
            int lv_num;
            int lv_intStackOutSize;
            // Automatic Variable Declarations
            int autoB_ae;
            const int autoB_ai = 1;
            int autoC_ae;
            const int autoC_ai = 1;
            int autoD_ae;
            const int autoD_ai = -1;
            int autoE_ae;
            const int autoE_ai = 1;
            // Variable Initialization
            lv_str = (lp_key + "String");
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_StringGroup" + lv_str, 1);
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            lv_intStackOutSize = 0;
            // Implementation
            autoB_ae = lv_num;
            lv_a = 1;
            for (; ((autoB_ai >= 0 && lv_a <= autoB_ae) || (autoB_ai < 0 && lv_a >= autoB_ae)); lv_a += autoB_ai)
            {
                lv_tag = HD_ReturnStringTagFromRegNum_Int(lv_a, lp_key);
                lv_tagValue = lv_tag;
                //Console.WriteLine("循环" + IntToString(lv_a) +"tag"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue));
                if ((lv_intStackOutSize == 0))
                {
                    lv_intStackOutSize += 1;
                    DataTableSave1(false, "IntStackOutTag", 1, lv_tag);
                    DataTableSave1(false, "IntStackOutTagValue", 1, lv_tagValue);
                    DataTableSave1(false, "IntStackOutTagIteraOrig", 1, lv_a);
                    //Console.WriteLine("尺寸" + IntToString(lv_intStackOutSize) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue)+"，IteraOrig="+IntToString(lv_a));
                }
                else
                {
                    lv_bool = false;
                    autoC_ae = lv_intStackOutSize;
                    lv_b = 1;
                    //Console.WriteLine("For" + IntToString(1) +"到"+IntToString(autoC_ae));
                    for (; ((autoC_ai >= 0 && lv_b <= autoC_ae) || (autoC_ai < 0 && lv_b >= autoC_ae)); lv_b += autoC_ai)
                    {
                        if (lp_big == false)
                        {
                            //Console.WriteLine("小值靠前");
                            if (lv_tagValue < (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                lv_intStackOutSize += 1;
                                autoD_ae = (lv_b + 1);
                                lv_c = lv_intStackOutSize;
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                lv_bool = true;
                                break;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("大值靠前"+"，当前lv_b=" +IntToString(lv_b));
                            if (lv_tagValue > (int)DataTableLoad1(false, "IntStackOutTagValue", lv_b))
                            {
                                //Console.WriteLine("Num" + IntToString(lv_a) +"元素"+IntToString(lv_tag) +"值"+IntToString(lv_tagValue) + ">第Lv_b="+IntToString(lv_b)+"元素"+IntToString(HD_ReturnStringTagFromRegNum(lv_b, lp_key))+"值"+IntToString(DataTableLoad1(false, "IntStackOutTagValue", lv_b)));
                                //Console.WriteLine("生效的lv_b：" + IntToString(lv_b));
                                lv_intStackOutSize += 1;
                                //Console.WriteLine("lv_intStackOutSize：" + IntToString(lv_intStackOutSize));
                                autoD_ae = (lv_b + 1);
                                //Console.WriteLine("autoD_ae：" + IntToString(autoD_ae));
                                lv_c = lv_intStackOutSize;
                                //Console.WriteLine("lv_c：" + IntToString(lv_c));
                                //Console.WriteLine("递减For lv_c=" + IntToString(lv_c) +"≥"+IntToString(autoD_ae));
                                for (; ((autoD_ai >= 0 && lv_c <= autoD_ae) || (autoD_ai < 0 && lv_c >= autoD_ae)); lv_c += autoD_ai)
                                {
                                    DataTableSave1(false, "IntStackOutTag", lv_c, DataTableLoad1(false, "IntStackOutTag", (lv_c - 1)));
                                    //Console.WriteLine("交换元素" + IntToString(DataTableLoad1(false, "IntStackOutTag", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagValue", lv_c, DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1)));
                                    //Console.WriteLine("交换值" + IntToString(DataTableLoad1(false, "IntStackOutTagValue", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                    DataTableSave1(false, "IntStackOutTagIteraOrig", lv_c, DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1)));
                                    //Console.WriteLine("交换新序值" + IntToString(DataTableLoad1(false, "IntStackOutTagIteraOrig", (lv_c - 1))) +"从序号"+IntToString(lv_c - 1) +"到"+IntToString(lv_c));
                                }
                                DataTableSave1(false, "IntStackOutTag", lv_b, lv_tag);
                                //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagValue", lv_b, lv_tagValue);
                                //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到lv_b="+IntToString(lv_b) +"位置");
                                DataTableSave1(false, "IntStackOutTagIteraOrig", lv_b, lv_a);
                                //Console.WriteLine("值IteraOrig=lv_a=" + IntToString(lv_a) +"存到序号lv_b="+IntToString(lv_b) +"位置");
                                lv_bool = true;
                                break;
                            }
                        }
                    }
                    if ((lv_bool == false))
                    {
                        lv_intStackOutSize += 1;
                        DataTableSave1(false, "IntStackOutTag", lv_intStackOutSize, lv_tag);
                        //Console.WriteLine("lv_tag=" + IntToString(lv_tag) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagValue", lv_intStackOutSize, lv_tagValue);
                        //Console.WriteLine("lv_tagValue=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                        DataTableSave1(false, "IntStackOutTagIteraOrig", lv_intStackOutSize, lv_a);
                        //Console.WriteLine("IteraOrig=lv_a=" + IntToString(lv_tagValue) +"存到尺寸="+IntToString(lv_intStackOutSize) +"位置");
                    }
                }
            }
            autoE_ae = lv_num; //此时lv_intStackOutSize=Num
            lv_a = 1;
            //Console.WriteLine("最终处理For 1~" + IntToString(lv_num));
            for (; ((autoE_ai >= 0 && lv_a <= autoE_ae) || (autoE_ai < 0 && lv_a >= autoE_ae)); lv_a += autoE_ai)
            {
                //从序号里取出元素Tag、自定义值、新老句柄，让元素交换
                //lv_tag = DataTableLoad1(true, (lp_key + "StringTag"), lv_a).ToString(); //原始序号元素
                lv_tag = (int)DataTableLoad1(false, "IntStackOutTag", lv_a);
                lv_tagValue = lv_tag;
                //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag));
                lv_b = (int)DataTableLoad1(false, "IntStackOutTagIteraOrig", lv_a); //lv_tag的原序号位置
                                                                                    //Console.WriteLine("第"+IntToString(lv_a) +"个元素：" + IntToString(lv_tag) + "值"+ IntToString(lv_tagValue)+"原序号：" + IntToString(lv_tag));
                if (lv_a != lv_b)
                {
                    //Console.WriteLine("lv_a："+IntToString(lv_a) +"不等于lv_b" + IntToString(lv_b));
                    DataTableSave1(true, (lp_key + "StringTag"), lv_a, lv_tag); //lv_tag放入新序号
                                                                                //Console.WriteLine("元素"+IntToString(lv_tag) +"放入lv_b=" + IntToString(lv_b)+"位置");
                }
            }
            DataTableSave0(true, "Key_StringGroup" + lv_str, 0);
        }

        /// <summary>
        /// 【MM_函数库】互动SG_设定String的String组专用状态。给String组的String设定一个状态值（字符串），之后可用"互动S_返回String、互动SG_返回String组的String状态"。状态值"true"和"false"是String的String组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效），而本函数可以重设干预，影响函数"互动SG_返回String组元素数量（仅检索XX状态）"。与"互动S_设定String状态"功能相同，只是状态参数在String组中被固定为"String组变量的内部ID"。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_stringGroup"></param>
        /// <param name="lp_groupState"></param>
        public static void HD_SetStringGState(string lp_string, string lp_stringGroup, string lp_groupState)
        {
            HD_SetStringState(lp_string, lp_stringGroup, lp_groupState);
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String的String组专用状态。使用"互动S_设定String、互动SG_设定String组的String状态"后可使用本函数。与"互动S_返回String状态"功能相同，只是状态参数在String组中被固定为"String组变量的内部ID"。状态值"true"和"false"是String的String组专用状态值，用于内部函数筛选字符状态（相当于单位组单位索引是否有效）。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_stringGroup"></param>
        public static void HD_ReturnStringGState(string lp_string, string lp_stringGroup)
        {
            HD_ReturnStringState(lp_string, lp_stringGroup);
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组元素序号对应元素。返回String组元素序号指定String。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_regNum">注册序号</param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static string HD_ReturnStringFromStringGFunc(int lp_regNum, string lp_gs)
        {
            // Variable Declarations
            string lv_str;
            string lv_tag = "";
            string lv_string;
            // Variable Initialization
            lv_str = (lp_gs + "String");
            lv_tag = DataTableLoad1(true, (lv_str + "Tag"), lp_regNum).ToString();
            lv_string = (string)DataTableLoad0(true, ("HD_String_" + lv_tag));
            // Implementation
            return lv_string;
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组元素总数。返回指定String组的元素数量。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnStringGNumMax(string lp_gs)
        {
            return (int)DataTableLoad0(true, lp_gs + "StringNum");
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组元素总数（仅检测String组专用状态="true"）。返回指定String组的元素数量。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnStringGNumMax_StateTrueFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            string lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnStringNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnStringFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnStringState(lv_c, lp_gs);
                if ((lv_b == "true"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组元素总数（仅检测String组专用状态="false"）。返回指定String组的元素数量。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnStringGNumMax_StateFalseFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            string lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnStringNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnStringFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnStringState(lv_c, lp_gs);
                if ((lv_b == "false"))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组元素总数（仅检测String组无效专用状态："false"或""）。返回指定String组的元素数量（false、""、null）。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static int HD_ReturnStringGNumMax_StateUselessFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            string lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnStringNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnStringFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnStringState(lv_c, lp_gs);
                if (((lv_b == "false") || (lv_b == "") || (lv_b == null)))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组元素总数（仅检测String组指定专用状态）。返回指定String组的元素数量。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_State">String组专用状态</param>
        /// <returns></returns>
        public static int HD_ReturnStringGNumMax_StateFunc_Specify(string lp_gs, string lp_State)
        {
            // Variable Declarations
            int lv_a;
            string lv_b;
            string lv_c;
            int lv_i = 0;
            // Automatic Variable Declarations
            int auto_ae;
            const int auto_ai = 1;
            // Variable Initialization
            lv_b = "";
            // Implementation
            auto_ae = HD_ReturnStringNumMax(lp_gs);
            lv_a = 1;
            for (; ((auto_ai >= 0 && lv_a <= auto_ae) || (auto_ai < 0 && lv_a >= auto_ae)); lv_a += auto_ai)
            {
                lv_c = HD_ReturnStringFromRegNum(lv_a, lp_gs);
                lv_b = HD_ReturnStringState(lv_c, lp_gs);
                if ((lv_b == lp_State))
                {
                    lv_i += 1;
                }
            }
            return lv_i;
        }

        /// <summary>
        /// 【MM_函数库】互动SG_添加String到String组。相同String被认为是同一个，非高级功能不提供专用状态检查，如果String没有设置过String组专用状态，那么首次添加到String组不会赋予"true"（之后可通过"互动S_设定String状态"、"互动SG_设定String组的String状态"修改）。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddStringToGroup_Simple(string lp_string, string lp_gs)
        {
            HD_RegString_Simple(lp_string, lp_gs);
        }

        /// <summary>
        /// 【MM_函数库】互动SG_添加String到String组（高级）。相同String被认为是同一个，高级功能提供专用状态检查，如果String没有设置过String组专用状态，那么首次添加到String组会赋予"true"（之后可通过"互动S_设定String状态"、"互动SG_设定String组的String状态"修改）。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_AddStringToGroup(string lp_string, string lp_gs)
        {
            HD_RegString_Simple(lp_string, lp_gs);
            if (DataTableKeyExists(true, ("HD_StringState" + lp_gs + "String_" + HD_RegStringTagAndReturn(lp_string))) == false)
            {
                DataTableSave0(true, ("HD_StringState" + lp_gs + "String_" + HD_RegStringTagAndReturn(lp_string)), "true");
                //Console.WriteLine(lp_gs + "=>" + HD_RegStringTagAndReturn(lp_string));
            }
        }

        /// <summary>
        /// 【MM_函数库】互动SG_移除String组中的元素。使用"互动SG_添加String到String组"后可使用本函数进行移除元素。移除使用了"互动S_移除String"，同一个存储区（String组ID）序号重排，移除时该存储区如有其他操作会排队等待。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_string"></param>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        public static void HD_ClearStringFromGroup(string lp_string, string lp_gs)
        {
            HD_RemoveString(lp_string, lp_gs);
        }

        //互动SG_为String组中的每个序号
        //GE（星际2的Galaxy Editor）的宏让编辑器保存时自动生成脚本并整合进脚本进行格式调整，C#仅参考需自行编写
        // #AUTOVAR(vs, string) = "#PARAM(group)";//"#PARAM(group)"是与字段、变量名一致的元素组名称，宏去声明string类型名为“Auto随机编号_vs”的自动变量，然后=右侧字符
        // #AUTOVAR(ae) = HD_ReturnStringNumMax(#AUTOVAR(vs));//宏去声明默认int类型名为“Auto随机编号_ae”的自动变量，然后=右侧字符
        // #INITAUTOVAR(ai,increment)//宏去声明int类型名为“Auto随机编号_ai”的自动变量，用于下面for循环增量（increment是传入参数）
        // #PARAM(var) = #PARAM(s);//#PARAM(var)是传进来的参数，用作“当前被挑选到的元素”（任意变量-整数 lp_var）， #PARAM(s)是传进来的参数用作"开始"（int lp_s）
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #PARAM(var) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #PARAM(var) >= #AUTOVAR(ae)) ) ; #PARAM(var) += #AUTOVAR(ai) ) {
        //     #SUBFUNCS(actions)//代表用户GUI填写的所有动作
        // }

        /// <summary>
        /// 【MM_函数库】互动SG_为String组中的每个序号。每次挑选的元素序号会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素序号，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachStringNumFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            int lv_ae = HD_ReturnStringNumMax(lp_gs);
            int lv_var = lp_start;
            int lv_ai = lp_increment;
            for (; (lv_ai >= 0 && lv_var <= lv_ae) || (lv_ai < 0 && lv_var >= lv_ae); lv_var += lv_ai)
            {
                lp_funcref(lv_var);//用户填写的所有动作
            }
        }

        //互动SG_为String组中的每个元素
        // #AUTOVAR(vs, string) = "#PARAM(group)";
        // #AUTOVAR(ae) = HD_ReturnStringNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= #PARAM(s);
        // #INITAUTOVAR(ai,increment)
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     DataTableSave(false, "StringGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)), HD_ReturnStringFromRegNum(#AUTOVAR(va),#AUTOVAR(vs)));
        // }
        // #AUTOVAR(va)= #PARAM(s);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #PARAM(var) = DataTableLoad(false, "StringGFor"+ #AUTOVAR(vs) + IntToString(#AUTOVAR(va)));
        //     #SUBFUNCS(actions)
        // }

        /// <summary>
        /// 【MM_函数库】互动SG_为String组中的每个元素。每次挑选的元素会自行在动作组（委托函数）中使用，委托函数特征：void SubActionTest(int lp_var)，参数lp_var即每次遍历到的元素，请自行组织它在委托函数内如何使用，SubActionTest可直接作为本函数最后一个参数填入，填入多个动作范例：SubActionEventFuncref Actions += SubActionTest，然后Actions作为参数填入。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <param name="lp_start">开始</param>
        /// <param name="lp_increment">增量</param>
        /// <param name="lp_funcref">委托类型变量或函数引用</param>
        public static void HD_ForEachStringFromGroup(string lp_gs, int lp_start, int lp_increment, SubActionEventFuncref lp_funcref)
        {
            string lv_vs = lp_gs;
            int lv_ae = HD_ReturnStringNumMax(lv_vs);
            int lv_va = lp_start;
            int lv_ai = lp_increment;
            string lv_string;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                DataTableSave0(false, "StringGFor" + lv_vs + lv_va.ToString(), HD_ReturnStringFromRegNum(lv_va, lv_vs));
            }
            lv_va = lp_start;
            for (; (lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae); lv_va += lv_ai)
            {
                lv_string = (string)DataTableLoad0(false, "StringGFor" + lv_vs + lv_va.ToString());
                lp_funcref(lv_string);//用户填写的所有动作
            }
        }

        /// <summary>
        /// 【MM_函数库】互动SG_返回String组中随机元素。返回指定String组中的随机String。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_gs">元素组的名称，建议与字段、变量名一致，数组使用时字符应写成：组[一维][二维]...以此类推</param>
        /// <returns></returns>
        public static string HD_ReturnRandomStringFromStringGFunc(string lp_gs)
        {
            // Variable Declarations
            int lv_num;
            int lv_a;
            string lv_c = null;
            // Variable Initialization
            lv_num = HD_ReturnStringNumMax(lp_gs);
            // Implementation
            if ((lv_num >= 1))
            {
                lv_a = RandomInt(1, lv_num);
                lv_c = HD_ReturnStringFromRegNum(lv_a, lp_gs);
            }
            return lv_c;
        }

        //互动SG_添加String组到String组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnStringNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnStringFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_AddStringToGroup(#AUTOVAR(var), #AUTOVAR(vsb));
        // }


        /// <summary>
        /// 【MM_函数库】互动SG_添加String组到String组。添加一个String组A的元素到另一个String组B，相同String被认为是同一个。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_AddStringGToStringG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnStringNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            string lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnStringFromRegNum(lv_va, lv_vsa);
                HD_AddStringToGroup(lv_var, lv_vsb);
            }
        }

        //互动SG_从String组移除String组
        // #AUTOVAR(vs, string) = "#PARAM(groupA)";
        // #AUTOVAR(vsb, string) = "#PARAM(groupB)";
        // #AUTOVAR(ae) = HD_ReturnStringNumMax(#AUTOVAR(vs));
        // #AUTOVAR(va)= 1;
        // #AUTOVAR(ai)= 1;
        // #AUTOVAR(var);
        // for ( ; ( (#AUTOVAR(ai) >= 0 && #AUTOVAR(va) <= #AUTOVAR(ae)) || (#AUTOVAR(ai) < 0 && #AUTOVAR(va) >= #AUTOVAR(ae)) ) ; #AUTOVAR(va) += #AUTOVAR(ai) ) {
        //     #AUTOVAR(var) = HD_ReturnStringFromRegNum(#AUTOVAR(va), #AUTOVAR(vs));
        //     HD_RemoveString(#AUTOVAR(var), #AUTOVAR(vsb));
        // }

        /// <summary>
        /// 【MM_函数库】互动SG_从String组移除String组。将String组A的元素从String组B中移除，相同String被认为是同一个。移除使用了"互动S_移除String"，同一个存储区（String组ID）序号重排，移除时该存储区如有其他操作会排队等待。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_groupA"></param>
        /// <param name="lp_groupB"></param>
        public static void HD_ClearStringGFromStringG(string lp_groupA, string lp_groupB)
        {
            string lv_vsa = lp_groupA;
            string lv_vsb = lp_groupB;
            int lv_ae = HD_ReturnStringNumMax(lv_vsa);
            int lv_va = 1;
            int lv_ai = 1;
            string lv_var;
            for (; ((lv_ai >= 0 && lv_va <= lv_ae) || (lv_ai < 0 && lv_va >= lv_ae)); lv_va += lv_ai)
            {
                lv_var = HD_ReturnStringFromRegNum(lv_va, lv_vsa);
                HD_RemoveString(lv_var, lv_vsb);
            }
        }

        /// <summary>
        /// 【MM_函数库】互动SG_移除String组全部元素。将String组（Key区）存储的元素全部移除，相同String被认为是同一个。移除时同一个存储区（String组ID）序号不进行重排，但该存储区如有其他操作会排队等待。String组目前不支持赋值其他变量，绝对ID对应绝对Key，可使用"添加String组到String组"函数来完成赋值需求
        /// </summary>
        /// <param name="lp_key">存储键区，默认填String组名称</param>
        public static void HD_RemoveStringGAll(string lp_key)
        {
            // Variable Declarations
            string lv_str;
            int lv_num;
            string lv_tag = "";
            int lv_a;
            // Variable Initialization
            lv_str = (lp_key + "String");
            lv_num = (int)DataTableLoad0(true, (lv_str + "Num"));
            // Implementation
            ThreadWait(lv_str);
            DataTableSave0(true, "Key_StringGroup" + lv_str, 1);
            for (lv_a = 1; lv_a <= lv_num; lv_a += 1)
            {
                lv_tag = DataTableLoad1(true, (lp_key + "StringTag"), lv_a).ToString();
                lv_num -= 1;
                DataTableClear0(true, "HD_IfStringTag" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_StringCV" + lv_str + "_" + lv_tag);
                DataTableClear0(true, "HD_StringState" + lv_str + "_" + lv_tag);
                DataTableSave0(true, (lp_key + "StringNum"), lv_num);
            }
            DataTableSave0(true, "Key_StringGroup" + lv_str, 0);
        }

        //--------------------------------------------------------------------------------------------------
        // 字符组End
        //--------------------------------------------------------------------------------------------------

        #endregion

        #endregion

        #region 键鼠事件动作主体（加入按键监听并传参执行）

        /// <summary>
        /// 【MM_函数库】键盘按下事件主要动作（加入按键监听并传参执行）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        internal static bool KeyDown(int player, int key)
        {
            bool torf = !MMCore.stopKeyMouseEvent[player];
            Player.KeyDownState[player, key] = torf;  //当前按键状态值
            Player.KeyDown[player, key] = true;  //当前按键值

            if (MMCore.stopKeyMouseEvent[player] == false)
            {
                Player.KeyDownLoopOneBitNum[player] += 1; //玩家当前注册的按键队列数量
                MMCore.DataTableSave2(true, "KeyDownLoopOneBit", player, Player.KeyDownLoopOneBitNum[player], key);
                //↑存储玩家注册序号对应按键队列键位
                MMCore.DataTableSave2(true, "KeyDownLoopOneBitKey", player, key, true); //玩家按键队列键位状态
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
                MMCore.KeyDownGlobalEvent(key, true, player);
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】键盘弹起事件主要动作（加入按键监听并传参执行）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool KeyUp(int player, int key)
        {
            bool torf = !MMCore.stopKeyMouseEvent[player];
            Player.KeyDownState[player, key] = false;  //当前按键状态值，本事件始终为false
            Player.KeyDown[player, key] = false;  //当前按键值

            if (MMCore.stopKeyMouseEvent[player] == false)
            {
                //直接执行动作或通知延迟弹起函数去执行动作
                if ((bool)MMCore.DataTableLoad2(true, "KeyDownLoopOneBitKey", player, key) == false)
                {
                    //弹起时无该键动作队列（由延迟弹起执行完），则直接执行本次事件动作
                    MMCore.KeyUpFunc(player, key);
                }
                else
                {
                    //弹起时有该键动作队列，通知延迟弹起函数运行（按键队列>0时，清空一次队列并执行它们的动作）
                    MMCore.DataTableSave2(true, "KeyDownLoopOneBitEnd", player, key, true);
                }
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】键盘弹起事件处理函数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool KeyUpFunc(int player, int key)
        {
            bool torf = true;
            if (MMCore.stopKeyMouseEvent[player] == true)
            {
                torf = false;
            }
            else
            {
                MMCore.KeyDownGlobalEvent(key, false, player);
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】鼠标移动事件主要动作（加入按键监听并传参执行）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="lp_mouseVector3D"></param>
        /// <param name="uiX"></param>
        /// <param name="uiY"></param>
        internal static void MouseMove(int player, Vector3D lp_mouseVector3D, int uiX, int uiY)
        {
            if (MMCore.stopKeyMouseEvent[player] == false)
            {
                Player.MouseVector[player] = new Vector(lp_mouseVector3D.X, lp_mouseVector3D.Y);

                //↓注意取出来的是该点最高位Unit
                double unitTerrainHeight = double.Parse(MMCore.HD_ReturnVectorCV(Player.MouseVector[player], "Unit.TerrainHeight"));
                double unitHeight = double.Parse(MMCore.HD_ReturnVectorCV(Player.MouseVector[player], "Unit.Height"));

                Player.MouseVectorX[player] = lp_mouseVector3D.X;
                Player.MouseVectorY[player] = lp_mouseVector3D.Y;
                Player.MouseVectorZ[player] = lp_mouseVector3D.Z;
                Player.MouseVectorZFixed[player] = lp_mouseVector3D.Z - MMCore.MapHeight;

                Player.MouseUIX[player] = uiX;
                Player.MouseUIY[player] = uiY;

                Player.MouseVector3DFixed[player] = new Vector3D(lp_mouseVector3D.X, lp_mouseVector3D.Y, Player.MouseVectorZFixed[player]);
                Player.MouseVector3D[player] = lp_mouseVector3D;
                //下面2个动作应该要从二维点读取单位（可多个），将最高的单位的头顶坐标填入以修正鼠标Z点
                Player.MouseVector3DUnitTerrain[player] = new Vector3D(lp_mouseVector3D.X, lp_mouseVector3D.Y, lp_mouseVector3D.Z - unitTerrainHeight);
                Player.MouseVector3DTerrain[player] = new Vector3D(lp_mouseVector3D.X, lp_mouseVector3D.Y, lp_mouseVector3D.Z - unitTerrainHeight - unitHeight);

                //玩家控制单位存在时，计算鼠标距离控制单位的2D角度和3D距离
                if (Player.UnitControl[player] != null)
                {
                    //计算鼠标与控制单位的2D角度，用于调整角色在二维坐标系四象限内的的朝向
                    Player.MouseToUnitControlAngle[player] = MMCore.AngleBetween(Player.UnitControl[player].Vector, Player.MouseVector[player]);
                    //计算鼠标与控制单位的2D距离（由于点击的位置是单位头顶位置，2个单位重叠则返回最高位的，所以玩家会点到最高位单位）
                    Player.MouseToUnitControlRange[player] = MMCore.Distance(Player.UnitControl[player].Vector, Player.MouseVector[player]);
                    //计算鼠标与控制单位的3D距离（由于点击的位置是单位头顶位置，2个单位重叠则返回最高位的，所以玩家会点到最高位单位）
                    Player.MouseToUnitControlRange3D[player] = MMCore.Distance(Player.UnitControl[player].Vector3D, lp_mouseVector3D);
                }
            }
        }

        /// <summary>
        /// 【MM_函数库】鼠标按下事件主要动作（加入按键监听并传参执行）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="lp_mouseVector3D"></param>
        /// <param name="uiX"></param>
        /// <param name="uiY"></param>
        /// <returns></returns>
        internal static bool MouseDown(int player, int key, Vector3D lp_mouseVector3D, int uiX, int uiY)
        {
            bool torf = !MMCore.stopKeyMouseEvent[player];
            Player.MouseDownState[player, key] = torf;  //当前按键状态值
            Player.MouseDown[player, key] = true;  //当前按键值
            if (key == c_mouseButtonLeft)
            {
                Player.MouseDownLeft[player] = true;
            }
            if (key == c_mouseButtonRight)
            {
                Player.MouseDownRight[player] = true;
            }
            if (key == c_mouseButtonMiddle)
            {
                Player.MouseDownMiddle[player] = true;
            }

            if (MMCore.stopKeyMouseEvent[player] == false)
            {
                Player.MouseVector[player] = new Vector(lp_mouseVector3D.X, lp_mouseVector3D.Y);

                //↓注意取出来的是该点最高位Unit
                double unitTerrainHeight = double.Parse(MMCore.HD_ReturnVectorCV(Player.MouseVector[player], "Unit.TerrainHeight"));
                double unitHeight = double.Parse(MMCore.HD_ReturnVectorCV(Player.MouseVector[player], "Unit.Height"));

                Player.MouseVectorX[player] = lp_mouseVector3D.X;
                Player.MouseVectorY[player] = lp_mouseVector3D.Y;
                Player.MouseVectorZ[player] = lp_mouseVector3D.Z;
                Player.MouseVectorZFixed[player] = lp_mouseVector3D.Z - MMCore.MapHeight;

                Player.MouseUIX[player] = uiX;
                Player.MouseUIY[player] = uiY;

                Player.MouseVector3DFixed[player] = new Vector3D(lp_mouseVector3D.X, lp_mouseVector3D.Y, Player.MouseVectorZFixed[player]);
                Player.MouseVector3D[player] = lp_mouseVector3D;
                //下面2个动作应该要从二维点读取单位（可多个），将最高的单位的头顶坐标填入以修正鼠标Z点
                Player.MouseVector3DUnitTerrain[player] = new Vector3D(lp_mouseVector3D.X, lp_mouseVector3D.Y, lp_mouseVector3D.Z - unitTerrainHeight);
                Player.MouseVector3DTerrain[player] = new Vector3D(lp_mouseVector3D.X, lp_mouseVector3D.Y, lp_mouseVector3D.Z - unitTerrainHeight - unitHeight);

                //玩家控制单位存在时，计算鼠标距离控制单位的2D角度和3D距离
                if (Player.UnitControl[player] != null)
                {
                    //计算鼠标与控制单位的2D角度，用于调整角色在二维坐标系四象限内的的朝向
                    Player.MouseToUnitControlAngle[player] = MMCore.AngleBetween(Player.UnitControl[player].Vector, Player.MouseVector[player]);
                    //计算鼠标与控制单位的2D距离（由于点击的位置是单位头顶位置，2个单位重叠则返回最高位的，所以玩家会点到最高位单位）
                    Player.MouseToUnitControlRange[player] = MMCore.Distance(Player.UnitControl[player].Vector, Player.MouseVector[player]);
                    //计算鼠标与控制单位的3D距离（由于点击的位置是单位头顶位置，2个单位重叠则返回最高位的，所以玩家会点到最高位单位）
                    Player.MouseToUnitControlRange3D[player] = MMCore.Distance(Player.UnitControl[player].Vector3D, lp_mouseVector3D);
                }

                //---------------------------------------------------------------------
                Player.MouseDownLoopOneBitNum[player] += 1;
                MMCore.DataTableSave2(true, "MouseDownLoopOneBit", player, Player.MouseDownLoopOneBitNum[player], key);
                MMCore.DataTableSave2(true, "MouseDownLoopOneBitKey", player, key, true);
                //---------------------------------------------------------------------
                //if (libBC0D3AAD_gv_XuLiGuanLi == true)
                //{
                //    libBC0D3AAD_gf_HD_RegKXL(lv_mouseButton, "libBC0D3AAD_gv_IntGroup_XuLi" + IntToString(lv_player)); //HD_注册按键
                //    libBC0D3AAD_gf_HD_SetKeyFixedXL(lv_player, lv_mouseButton, 1.0);
                //}
                ////---------------------------------------------------------------------
                //if (libBC0D3AAD_gv_ShuangJiGuanLi == true)
                //{
                //    libBC0D3AAD_gf_HD_RegPTwo(lv_point1, "DoubleClicked_PTwo_" + IntToString(lv_player));
                //    lv_a = libBC0D3AAD_gf_HD_ReturnKeyFixedSJ(lv_player, lv_mouseButton);
                //    if ((0.0 < lv_a) && (lv_a <= libBC0D3AAD_gv_ShuangJiShiXian) && libBC0D3AAD_gf_HD_PTwoRangeTrue("DoubleClicked_PTwo_" + IntToString(lv_player)))
                //    {
                //        //符合双击标准（鼠标双击多个2点验证），发送事件
                //        libBC0D3AAD_gf_Send_MouseDoubleClicked(lv_player, lv_mouseButton, libBC0D3AAD_gv_ShuangJiShiXian - lv_a, lv_point0, lv_uiX, lv_uiY);
                //    }
                //    else
                //    {
                //        libBC0D3AAD_gf_HD_RegKSJ(lv_mouseButton, "libBC0D3AAD_gv_IntGroup_DoubleClicked" + IntToString(lv_player)); //HD_注册按键
                //        libBC0D3AAD_gf_HD_SetKeyFixedSJ(lv_player, lv_mouseButton, libBC0D3AAD_gv_ShuangJiShiXian);
                //    }
                //}
                ////---------------------------------------------------------------------
                MMCore.MouseDownFunc(player, key, lp_mouseVector3D, uiX, uiY);
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】鼠标按下事件处理函数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="lp_mouseVector3D"></param>
        /// <param name="uiX"></param>
        /// <param name="uiY"></param>
        /// <returns></returns>
        internal static bool MouseDownFunc(int player, int key, Vector3D lp_mouseVector3D, int uiX, int uiY)
        {
            // Variable Declarations
            bool torf = true;

            // Implementation
            if (MMCore.stopKeyMouseEvent[player] == true)
            {
                //阻止按键事件时强制取消按键状态
                Player.MouseDownState[player, key] = false;
                if (key == c_mouseButtonLeft)
                {
                    Player.MouseDownLeft[player] = false;
                }
                if (key == c_mouseButtonRight)
                {
                    Player.MouseDownRight[player] = false;
                }
                if (key == c_mouseButtonMiddle)
                {
                    Player.MouseDownMiddle[player] = false;
                }
                torf = false;
            }
            else
            {
                MMCore.MouseDownGlobalEvent(key, true, player);
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】鼠标弹起事件主要动作（加入按键监听并传参执行）
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="lp_mouseVector3D"></param>
        /// <param name="uiX"></param>
        /// <param name="uiY"></param>
        internal static bool MouseUp(int player, int key, Vector3D lp_mouseVector3D, int uiX, int uiY)
        {
            bool torf = !MMCore.stopKeyMouseEvent[player];
            Player.MouseDownState[player, key] = false;  //当前按键状态值，本事件始终为false
            Player.MouseDown[player, key] = false;  //当前按键值
            if (key == c_mouseButtonLeft)
            {
                Player.MouseDownLeft[player] = false;
            }
            if (key == c_mouseButtonRight)
            {
                Player.MouseDownRight[player] = false;
            }
            if (key == c_mouseButtonMiddle)
            {
                Player.MouseDownMiddle[player] = false;
            }

            if (MMCore.stopKeyMouseEvent[player] == false)
            {
                //直接执行动作或通知延迟弹起函数去执行动作
                if ((bool)MMCore.DataTableLoad2(true, "MouseDownLoopOneBitKey", player, key) == false)
                {
                    //弹起时无该键动作队列（由延迟弹起执行完），则直接执行本次事件动作
                    MMCore.MouseUpFunc(player, key);
                }
                else
                {
                    //弹起时有该键动作队列，通知延迟弹起函数运行（按键队列>0时，清空一次队列并执行它们的动作）
                    MMCore.DataTableSave2(true, "MouseDownLoopOneBitEnd", player, key, true);
                }
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】鼠标弹起事件处理函数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        internal static bool MouseUpFunc(int player, int key)
        {
            bool torf = true;
            if (MMCore.stopKeyMouseEvent[player] == true)
            {
                torf = false;
            }
            else
            {
                MMCore.MouseDownGlobalEvent(key, false, player);
            }
            return torf;
        }

        /// <summary>
        /// 【MM_函数库】键鼠弹起事件延迟执行函数，会按序执行键鼠事件动作队列，需加入到每帧执行
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="lp_mouseVector3D"></param>
        /// <param name="uiX"></param>
        /// <param name="uiY"></param>
        internal static void MouseKeyUpWait(int player, int key)
        {
            int ae, be, a, ai = 1, bi = 1;
            //玩家有鼠标按键事件动作队列时
            if (Player.MouseDownLoopOneBitNum[player] > 0)
            {
                ae = Player.MouseDownLoopOneBitNum[player];//获取动作队列数量
                a = 1;
                for (; ((ai >= 0 && a <= ae) || (ai < 0 && a >= ae)); a += ai)
                {
                    key = (int)DataTableLoad2(true, "MouseDownLoopOneBit", player, a);//读取玩家指定动作队列按键
                    if ((bool)DataTableLoad2(true, "MouseDownLoopOneBitEnd", player, key) == true)//判断玩家指定按键的动作队列是否结束
                    {
                        //如果该键的动作队列结束，重置按键状态
                        if (key == c_mouseButtonLeft)
                        {
                            Player.MouseDown[player, MMCore.c_mouseButtonLeft] = false;
                        }
                        if (key == c_mouseButtonRight)
                        {
                            Player.MouseDown[player, MMCore.c_mouseButtonRight] = false;
                        }
                        if (key == c_mouseButtonMiddle)
                        {
                            Player.MouseDown[player, MMCore.c_mouseButtonMiddle] = false;
                        }
                        //
                        MMCore.MouseDownFunc(player, key, Player.MouseVector3D[player], Player.MouseUIX[player], Player.MouseUIY[player]);
                    }
                    DataTableClear2(true, "MouseDownLoopOneBit", player, a);
                    DataTableClear2(true, "MouseDownLoopOneBitKey", player, key);
                    DataTableClear2(true, "MouseDownLoopOneBitEnd", player, key);
                }
                Player.MouseDownLoopOneBitNum[player] = 0; //动作全部执行，全队列清空
            }
            //玩家有键盘按键事件动作队列时
            if (Player.KeyDownLoopOneBitNum[player] > 0)//获取动作队列数量
            {
                be = Player.KeyDownLoopOneBitNum[player];
                a = 1;
                for (; ((bi >= 0 && a <= be) || (bi < 0 && a >= be)); a += bi)
                {
                    key = (int)DataTableLoad2(true, "KeyDownLoopOneBit", player, a);//读取玩家指定动作队列按键
                    if ((bool)DataTableLoad2(true, "KeyDownLoopOneBitEnd", player, key) == true)//判断玩家指定按键的动作队列是否结束
                    {
                        //如果该键的动作队列结束，重置按键状态
                        Player.KeyDown[player, key] = false;
                        MMCore.KeyUpFunc(player, key);
                    }
                    DataTableClear2(true, "KeyDownLoopOneBit", player, a);
                    DataTableClear2(true, "KeyDownLoopOneBitKey", player, key);
                    DataTableClear2(true, "KeyDownLoopOneBitEnd", player, key);
                }
                Player.KeyDownLoopOneBitNum[player] = 0; //全键盘队列清空
            }
        }

        #endregion

    }

    /// <summary>
    /// 【MetalMaxSystem】主循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，主循环Update事件被执行时创建计时器的父线程（mainUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
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
        /// 主循环状态，手动设置为false则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
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
        /// 主循环的计时器实例创建时以参数填入、被反复执行的函数，Update事件被执行时创建计时器的父线程将暂停，直到本函数确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）。一般不需要用户操作，TimerState为true时手动调用会额外增加Update次数
        /// </summary>
        /// <param name="state"></param>
        public static void CheckStatus(object state)
        {
            if (!TimerState)
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
    /// 【MetalMaxSystem】副循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，副循环Update事件被执行时创建计时器的父线程（subUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
    /// </summary>
    public static class SubUpdateChecker
    {
        //静态类不允许声明实例成员，因为无法判断每个实例的内存地址

        private static int _invokeCount;
        private static bool _timerState;

        /// <summary>
        /// 【MM_函数库】副循环Update事件运行次数
        /// </summary>
        public static int InvokeCount { get => _invokeCount; set => _invokeCount = value; }
        /// <summary>
        /// 【MM_函数库】副循环状态，手动设置为false则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        public static bool TimerState { get => _timerState; set => _timerState = value; }

        //一个类只能有一个静态构造函数，不能有访问修饰符（因为不是给用户调用的，且是由.net 框架在合适的时机调用）
        //静态构造函数也不能带任何参数（主要因为框架不可能知道我们需要在函数中添加什么参数，所以干脆规定不能使用参数）
        //静态构造函数是特殊的静态方法，同样不允许使用实例成员
        //无参静态构造函数和无参实例构造函数可并存不冲突，内存地址不同
        //所有静态数据只会从模板复制一份副本，所以静态构造函数只被执行一次
        //平时没在类中写构造函数也没继承那么框架会生成一个无参构造函数，当类中定义静态成员没定义静态构造函数时，框架亦会生成一个静态构造函数来让框架自身来调用

        /// <summary>
        /// 【MM_函数库】副循环状态监控类（用来读写InvokeCount、TimerState属性），计时器实例创建时本类方法CheckStatus以参数填入被反复执行，副循环Update事件被执行时创建计时器的父线程（subUpdateThread）将暂停，直到该方法确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）。一般不需要用户操作，TimerState为true时手动调用会额外增加Update次数
        /// </summary>
        static SubUpdateChecker()
        {
            InvokeCount = 0;
            TimerState = false;
        }

        //静态方法只能访问类中的静态成员（即便在实例类，同理因无法判断非静态变量等这些实例成员的活动内存地址，所以不允许使用实例成员）

        /// <summary>
        /// 【MM_函数库】副循环的计时器实例创建时以参数填入、被反复执行的函数，Update事件被执行时创建计时器的父线程将暂停，直到本函数确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        /// <param name="state"></param>
        public static void CheckStatus(object state)
        {
            if (!TimerState)
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
    /// 【MetalMaxSystem】周期触发器，创建实例后请给函数注册事件（语法：TimerUpdate.Awake/Start/Update/End/Destroy +=/-= 任意符合事件参数格式的函数的名称如MyFunc，其声明为void MyFun(object sender, EventArgs e)，sender传递本类实例（其他类型也可），e传递额外事件参数类的信息），TriggerStart方法将自动创建独立触发器线程并启动周期触发器（主体事件发布动作），启动前可用Duetime、Period属性方法设定Update阶段每次循环的前摇和间隔，启动后按序执行Awake/Start/Update/End/Destroy被这5种事件注册过的委托函数，其中事件Update阶段是一个计时器循环，直到用户手动调用TimerState属性方法，该属性为true时会让计时器到期退出Update循环，而计时器所在父线程（即触发器线程）将运行End和Destory事件
    /// </summary>
    public class TimerUpdate
    {
        #region 变量、字段及其属性方法

        /// <summary>
        /// 【MM_函数库】自动复位事件（用来控制触发线程信号）
        /// </summary>
        private AutoResetEvent _autoResetEvent_TimerUpdate;
        /// <summary>
        /// 【MM_函数库】自动复位事件，提供该属性方便随时读取，属性动作AutoResetEvent_TimerUpdate.Set()可让触发器线程终止（效果等同TimerUpdate.TimerState = true）
        /// </summary>
        public AutoResetEvent AutoResetEvent_TimerUpdate { get => _autoResetEvent_TimerUpdate; }

        /// <summary>
        /// 【MM_函数库】周期触发器主体事件发布动作所在线程的实例
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// 【MM_函数库】周期触发器执行Update事件的计时器实例
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// 【MM_函数库】周期触发器Update事件运行次数
        /// </summary>
        private int _invokeCount;
        /// <summary>
        /// 【MM_函数库】周期触发器Update事件运行次数，该属性可随时读取或清零
        /// </summary>
        public int InvokeCount { get => _invokeCount; set => _invokeCount = value; }

        /// <summary>
        /// 【MM_函数库】周期触发器Update事件运行次数上限，该属性让计时器到期退出循环，计时器所在父线程将运行End和Destory事件
        /// </summary>
        private int _invokeCountMax;
        /// <summary>
        /// 【MM_函数库】周期触发器Update事件运行次数上限，该属性让计时器到期退出循环，计时器所在父线程将运行End和Destory事件
        /// </summary>
        public int InvokeCountMax { get => _invokeCountMax; set => _invokeCountMax = value; }

        /// <summary>
        /// 【MM_函数库】周期触发器Update事件前摇，未设置直接启动TriggerStart则默认为0
        /// </summary>
        private int _duetime;
        /// <summary>
        /// 【MM_函数库】周期触发器Update事件前摇，未设置直接启动TriggerStart则默认为0
        /// </summary>
        public int Duetime { get => _duetime; set => _duetime = value; }

        /// <summary>
        /// 【MM_函数库】周期触发器的运行间隔字段，未设置直接启动TriggerStart则默认为1s
        /// </summary>
        private int _period;
        /// <summary>
        /// 【MM_函数库】周期触发器的运行间隔属性，未设置直接启动TriggerStart则默认为1s
        /// </summary>
        public int Period { get => _period; set => _period = value; }

        /// <summary>
        /// 【MM_函数库】周期触发器的状态字段，手动设置为false则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        private bool _timerState;
        /// <summary>
        /// 【MM_函数库】周期触发器的状态属性，手动设置为false则计时器工作时将收到信号退出循环（不执行Update事件），计时器所在父线程将运行End和Destory事件
        /// </summary>
        public bool TimerState { get => _timerState; set => _timerState = value; }

        /// <summary>
        /// 【MM_函数库】事件委托列表，用来存储多个事件委托，用对象类型的键来取出，内部属性，用户不需要操作
        /// </summary>
        protected EventHandlerList _listEventDelegates = new EventHandlerList();

        /// <summary>
        /// 【MM_函数库】周期触发器主体事件发布动作所在线程的实例，提供该属性方便随时读取，但不允许不安全赋值
        /// </summary>
        public Thread Thread { get => _thread; }

        /// <summary>
        /// 【MM_函数库】周期触发器执行Update事件的计时器实例，提供该属性方便随时读取，但不允许不安全赋值
        /// </summary>
        public Timer Timer { get => _timer; }

        #endregion

        #region 定义区分委托对象的键

        //每个new object()都是一个单独的实例个体，所以定义的五个变量相当于内存ID不同的object类型的键
        //由于EventHandlerList类型的_listEventDelegates委托队列是实例成员，相同键返回的委托对象不会相同
        //所以即便创建了多个实例，以下键只需内存独此一份且私有只读

        /// <summary>
        /// 【MM_函数库】周期触发器用于返回事件委托队列中Awake事件委托对象的键
        /// </summary>
        private static readonly object awakeEventKey = new object();
        /// <summary>
        /// 【MM_函数库】周期触发器用于返回事件委托队列中Start事件委托对象的键
        /// </summary>
        private static readonly object startEventKey = new object();
        /// <summary>
        /// 【MM_函数库】周期触发器用于返回事件委托队列中Update事件委托对象的键
        /// </summary>
        private static readonly object updateEventKey = new object();
        /// <summary>
        /// 【MM_函数库】周期触发器用于返回事件委托队列中End事件委托对象的键
        /// </summary>
        private static readonly object endEventKey = new object();
        /// <summary>
        /// 【MM_函数库】周期触发器用于返回事件委托队列中Destroy事件委托对象的键
        /// </summary>
        private static readonly object destroyEventKey = new object();

        #endregion

        #region 声明事件委托

        //事件委托必须安全方式注册事件给函数，不能直接运行，而常规委托则相反。从事件委托列表通过键取出的事件委托可赋值给常规委托去执行，常规委托可以赋值给事件委托但不能交换顺序
        //声明事件委托变量（首字母大写），相比常规委托，事件委托因安全考虑无法直接被执行，通过OnAwake内部函数确保安全执行（其实是声明临时常规委托在赋值后执行）

        /// <summary>
        /// 【MM_函数库】将周期触发器的唤醒事件注册到函数，语法：TimerUpdate.Awake +=/-= 实例或静态函数（特征格式：void 函数名(object sender, EventArgs e)）
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
        /// 【MM_函数库】将周期触发器的开始事件注册到函数，语法：TimerUpdate.Start +=/-= 实例或静态函数（特征格式：void 函数名(object sender, EventArgs e)）
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
        /// 【MM_函数库】将周期触发器的开始事件注册到函数，语法：TimerUpdate.Update +=/-= 实例或静态函数（特征格式：void 函数名(object sender, EventArgs e)）
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
        /// 【MM_函数库】将周期触发器的开始事件注册到函数，语法：TimerUpdate.End +=/-= 实例或静态函数（特征格式：void 函数名(object sender, EventArgs e)）
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
        /// 【MM_函数库】将周期触发器的开始事件注册到函数，语法：TimerUpdate.Destroy +=/-= 实例或静态函数（特征格式：void 函数名(object sender, EventArgs e)）
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
        /// 【MM_函数库】创建一个不会到期的周期触发器
        /// </summary>
        public TimerUpdate()//构造函数
        {
            InvokeCount = 0;
            InvokeCountMax = 0;
            TimerState = false;
        }

        /// <summary>
        /// 【MM_函数库】创建一个有执行次数的周期触发器
        /// </summary>
        /// <param name="invokeCountMax">决定计时器Update阶段循环次数</param>
        public TimerUpdate(int invokeCountMax)//构造函数
        {
            InvokeCount = 0;
            InvokeCountMax = invokeCountMax;
            TimerState = false;
        }

        #endregion

        //非静态（实例）方法可以访问类中的任何成员

        /// <summary>
        /// 【MM_函数库】计时器实例创建时以参数填入、被反复执行的函数，Update事件被执行时创建计时器的父线程将暂停，直到本函数确认到TimerState为真，退出计时器循环，并通知计时器所在父线程恢复运行（将执行End和Destory事件）
        /// </summary>
        /// <param name="state"></param>
        private void CheckStatus(object state)
        {
            if (!TimerState)
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
        /// 【MM_函数库】周期触发器主体事件发布动作（重复执行则什么也不做），在当前线程创建周期触发器并执行事件委托（提前定义事件委托变量TimerUpdate.Awake/Start/Update/End/Destroy += 要委托执行的函数，即完成事件注册到函数），可预先自定义计时器Updata阶段的执行间隔（否则默认以Duetime=0、Period=1000运行计时器）。注：若直接调用本函数则在计时器Updata阶段会暂停当前线程，不想暂停请额外开线程手动加载Action运行或使用TimerUpdate.TriggerStart自带线程启动（推荐）
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
        /// 【MM_函数库】周期触发器主体事件发布动作（重复执行则什么也不做），在当前线程创建周期触发器并执行事件委托（提前定义事件委托变量TimerUpdate.Awake/Start/Update/End/Destroy += 要委托执行的函数，即完成事件注册到函数），可预先自定义计时器Updata阶段的执行间隔（否则默认以Duetime=0、Period=1000运行计时器）。注：若直接调用本函数则在计时器Updata阶段会暂停当前线程，不想暂停请额外开线程手动加载Action运行或使用TimerUpdate.TriggerStart自带线程启动（推荐）
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
        /// 【MM_函数库】计时器唤醒阶段时运行一次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAwake(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[awakeEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 【MM_函数库】周期触发器开始阶段运行一次
        /// </summary>
        private void OnStart(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[startEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 【MM_函数库】周期触发器Update阶段按预设间隔反复运行
        /// </summary>
        private void OnUpdate(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[updateEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 【MM_函数库】周期触发器结束阶段运行一次
        /// </summary>
        private void OnEnd(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[endEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 【MM_函数库】周期触发器摧毁阶段运行一次
        /// </summary>
        private void OnDestroy(object sender, EventArgs e)
        {
            TimerEventHandler timerEventHandler = (TimerEventHandler)_listEventDelegates[destroyEventKey];
            timerEventHandler?.Invoke(sender, e);
        }

        #endregion

        #region 自动创建线程执行周期触发器（模拟触发器运行）

        /// <summary>
        /// 【MM_函数库】自动创建线程启动周期触发器（模拟触发器运行），重复启动时什么也不做，未设置Update属性则默认以Duetime=0、Period=1000运行计时器循环
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

    /// <summary>
    /// 【MetalMaxSystem】单位类
    /// </summary>
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
        public Unit()
        {
            //创建新类时的初始化动作
        }

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

    /// <summary>
    /// 【MetalMaxSystem】玩家类
    /// </summary>
    public static class Player
    {
        #region 字段

        private static Unit[] _hero = new Unit[MMCore.c_maxPlayers + 1];
        private static Unit[,] vehicle = new Unit[MMCore.c_maxPlayers + 1, MMCore.c_vehicleTypeMax];
        private static Unit[] currentVehicle = new Unit[MMCore.c_maxPlayers + 1];
        private static Unit[] unitMain = new Unit[MMCore.c_maxPlayers + 1];
        private static Unit[] unitControl = new Unit[MMCore.c_maxPlayers + 1];
        private static bool[] _canNotOperation = new bool[MMCore.c_maxPlayers + 1];

        private static bool[,] _keyDown = new bool[MMCore.c_maxPlayers + 1, MMCore.c_keyMax + 1];
        private static bool[,] _keyDownState = new bool[MMCore.c_maxPlayers + 1, MMCore.c_keyMax + 1];
        private static bool[] _keyDownLoop = new bool[MMCore.c_maxPlayers + 1];
        private static int[] _keyDownLoopOneBitNum = new int[MMCore.c_maxPlayers + 1];

        private static bool[] _mouseDownLeft = new bool[MMCore.c_maxPlayers + 1];
        private static bool[] _mouseDownMiddle = new bool[MMCore.c_maxPlayers + 1];
        private static bool[] _mouseDownRight = new bool[MMCore.c_maxPlayers + 1];
        private static bool[,] _mouseDown = new bool[MMCore.c_maxPlayers + 1, MMCore.c_mouseMax + 1];
        private static bool[,] _mouseDownState = new bool[MMCore.c_maxPlayers + 1, MMCore.c_mouseMax + 1];
        private static bool[] _mouseDownLoop = new bool[MMCore.c_maxPlayers + 1];
        private static int[] _mouseDownLoopOneBitNum = new int[MMCore.c_maxPlayers + 1];

        private static bool[,] _keyDownTwice = new bool[MMCore.c_maxPlayers + 1, MMCore.c_mouseMax + 1];
        private static bool[,] _mouseDownTwice = new bool[MMCore.c_maxPlayers + 1, MMCore.c_mouseMax + 1];

        private static int[] _mouseUIX = new int[MMCore.c_maxPlayers + 1];
        private static int[] _mouseUIY = new int[MMCore.c_maxPlayers + 1];

        private static double[] _mouseVectorX = new double[MMCore.c_maxPlayers + 1];
        private static double[] _mouseVectorY = new double[MMCore.c_maxPlayers + 1];
        private static double[] _mouseVectorZ = new double[MMCore.c_maxPlayers + 1];
        private static double[] _mouseVectorZFixed = new double[MMCore.c_maxPlayers + 1];
        private static double[] _mouseToUnitControlAngle = new double[MMCore.c_maxPlayers + 1];
        private static double[] _mouseToUnitControlRange = new double[MMCore.c_maxPlayers + 1];
        private static double[] _mouseToUnitControlRange3D = new double[MMCore.c_maxPlayers + 1];
        private static Vector3D[] _cameraVector3D = new Vector3D[MMCore.c_maxPlayers + 1];
        private static Vector3D[] _mouseVector3DFixed = new Vector3D[MMCore.c_maxPlayers + 1];
        private static Vector3D[] _mouseVector3D = new Vector3D[MMCore.c_maxPlayers + 1];
        private static Vector3D[] _mouseVector3DUnitTerrain = new Vector3D[MMCore.c_maxPlayers + 1];
        private static Vector3D[] _mouseVector3DTerrain = new Vector3D[MMCore.c_maxPlayers + 1];
        private static Vector[] _mouseVector = new Vector[MMCore.c_maxPlayers + 1];

        private static string[] _type = new string[MMCore.c_maxPlayers + 1];

        #endregion

        #region 属性方法

        /// <summary>
        /// 英雄单位
        /// </summary>
        public static Unit[] Hero { get => _hero; set => _hero = value; }
        /// <summary>
        /// 私人载具
        /// </summary>
        public static Unit[,] Vehicle { get => vehicle; set => vehicle = value; }
        /// <summary>
        /// 当前载具
        /// </summary>
        public static Unit[] CurrentVehicle { get => currentVehicle; set => currentVehicle = value; }
        /// <summary>
        /// 主单位
        /// </summary>
        public static Unit[] UnitMain { get => unitMain; set => unitMain = value; }
        /// <summary>
        /// 控制单位
        /// </summary>
        public static Unit[] UnitControl { get => unitControl; set => unitControl = value; }
        /// <summary>
        /// 禁止操作
        /// </summary>
        public static bool[] CanNotOperation { get => _canNotOperation; set => _canNotOperation = value; }

        /// <summary>
        /// 键盘按键按下[键,玩家]
        /// </summary>
        public static bool[,] KeyDown { get => _keyDown; set => _keyDown = value; }
        /// <summary>
        /// 键盘按键按下的有效状态（因为即便按下也能逻辑否决，所以真实有效按键必须键按下+有效状态同时符合）。禁用玩家操作时总是false，某些情况可设计针对某键禁止操作
        /// </summary>
        public static bool[,] KeyDownState { get => _keyDownState; set => _keyDownState = value; }
        /// <summary>
        /// 键盘按键队列
        /// </summary>
        public static bool[] KeyDownLoop { get => _keyDownLoop; set => _keyDownLoop = value; }
        /// <summary>
        /// 键盘按键队列数
        /// </summary>
        public static int[] KeyDownLoopOneBitNum { get => _keyDownLoopOneBitNum; set => _keyDownLoopOneBitNum = value; }

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        public static bool[] MouseDownLeft { get => _mouseDownLeft; set => _mouseDownLeft = value; }
        /// <summary>
        /// 鼠标中键按下
        /// </summary>
        public static bool[] MouseDownMiddle { get => _mouseDownMiddle; set => _mouseDownMiddle = value; }
        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        public static bool[] MouseDownRight { get => _mouseDownRight; set => _mouseDownRight = value; }
        /// <summary>
        /// 鼠标按键按下
        /// </summary>
        public static bool[,] MouseDown { get => _mouseDown; set => _mouseDown = value; }
        /// <summary>
        /// 鼠标按键按下的有效状态（因为即便按下也能逻辑否决，所以真实有效按键必须键按下+有效状态同时符合）
        /// </summary>
        public static bool[,] MouseDownState { get => _mouseDownState; set => _mouseDownState = value; }
        /// <summary>
        /// 鼠标按键队列
        /// </summary>
        public static bool[] MouseDownLoop { get => _mouseDownLoop; set => _mouseDownLoop = value; }
        /// <summary>
        /// 鼠标按键队列数
        /// </summary>
        public static int[] MouseDownLoopOneBitNum { get => _mouseDownLoopOneBitNum; set => _mouseDownLoopOneBitNum = value; }

        /// <summary>
        /// 按键双击
        /// </summary>
        public static bool[,] KeyDownTwice { get => _keyDownTwice; set => _keyDownTwice = value; }
        /// <summary>
        /// 鼠标双击
        /// </summary>
        public static bool[,] MouseDownTwice { get => _mouseDownTwice; set => _mouseDownTwice = value; }

        /// <summary>
        /// 鼠标在UI的X坐标
        /// </summary>
        public static int[] MouseUIX { get => _mouseUIX; set => _mouseUIX = value; }
        /// <summary>
        /// 鼠标在UI的Y坐标
        /// </summary>
        public static int[] MouseUIY { get => _mouseUIY; set => _mouseUIY = value; }
        /// <summary>
        /// 鼠标在世界的X坐标
        /// </summary>
        public static double[] MouseVectorX { get => _mouseVectorX; set => _mouseVectorX = value; }
        /// <summary>
        /// 鼠标在世界的Y坐标
        /// </summary>
        public static double[] MouseVectorY { get => _mouseVectorY; set => _mouseVectorY = value; }
        /// <summary>
        /// 鼠标点高度，mouseVectorZ=MapHeight+TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// 悬崖、地形物件及单位在移动、诞生摧毁时应将高度信息刷新，以便实时获取
        /// </summary>
        public static double[] MouseVectorZ { get => _mouseVectorZ; set => _mouseVectorZ = value; }
        /// <summary>
        /// 修正后的鼠标点高度（扣减了地面高度，所以这是相对地面的高度），mouseVectorZFixed=mouseVectorZ-MapHeight=TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// </summary>
        public static double[] MouseVectorZFixed { get => _mouseVectorZFixed; set => _mouseVectorZFixed = value; }
        /// <summary>
        /// 鼠标与玩家控制单位在世界中的2D角度，象限分布：右=0度，上=90°，左=180°，下=270°，可用于调整行走方向
        /// </summary>
        public static double[] MouseToUnitControlAngle { get => _mouseToUnitControlAngle; set => _mouseToUnitControlAngle = value; }
        /// <summary>
        /// 鼠标与玩家控制单位在世界中的2D距离
        /// </summary>
        public static double[] MouseToUnitControlRange { get => _mouseToUnitControlRange; set => _mouseToUnitControlRange = value; }
        /// <summary>
        /// 鼠标与玩家控制单位在世界中的3D角度，常用于调整鼠标自动镜头
        /// </summary>
        public static double[] MouseToUnitControlRange3D { get => _mouseToUnitControlRange3D; set => _mouseToUnitControlRange3D = value; }
        /// <summary>
        /// 相机位置
        /// </summary>
        public static Vector3D[] CameraVector3D { get => _cameraVector3D; set => _cameraVector3D = value; }
        /// <summary>
        /// 鼠标3D点向量坐标，修正了鼠标点高度（扣减了地图高度，所以这是相对地面的高度），mouseVectorZFixed=mouseVectorZ-MapHeight=TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// </summary>
        public static Vector3D[] MouseVector3DFixed { get => _mouseVector3DFixed; set => _mouseVector3DFixed = value; }
        /// <summary>
        /// 鼠标3D点向量坐标，鼠标Z点在单位高度顶部，Z=MapHeight+TerrainHeight+Unit.TerrainHeight+Unit.Height
        /// </summary>
        public static Vector3D[] MouseVector3D { get => _mouseVector3D; set => _mouseVector3D = value; }
        /// <summary>
        /// 鼠标3D点向量坐标，鼠标Z点在单位层地形物件高度顶部（单位脚底），Z=MapHeight+TerrainHeight+Unit.TerrainHeight
        /// </summary>
        public static Vector3D[] MouseVector3DUnitTerrain { get => _mouseVector3DUnitTerrain; set => _mouseVector3DUnitTerrain = value; }
        /// <summary>
        /// 鼠标3D点向量坐标，鼠标Z点在悬崖、地形物件顶部，Z=MapHeight+TerrainHeight
        /// </summary>
        public static Vector3D[] MouseVector3DTerrain { get => _mouseVector3DTerrain; set => _mouseVector3DTerrain = value; }
        /// <summary>
        /// 鼠标2D点向量坐标
        /// </summary>
        public static Vector[] MouseVector { get => _mouseVector; set => _mouseVector = value; }

        /// <summary>
        /// 玩家类型（中立Neutral、电脑Ai、用户User、玩家Palyer、敌人Enemy）
        /// </summary>
        public static string[] Type { get => _type; set => _type = value; }



        #endregion

    }

    #region 键鼠钩子及监听服务

    #region 监听服务

    /// <summary>
    /// 【MetalMaxSystem】监听服务
    /// </summary>
    public class RecordService
    {
        private int _period = 50;//默认逻辑帧是50ms
        /// <summary>
        /// 监听服务逻辑每帧（可设定范围0~1000），用于鼠标移动、键按下时计算最大持续值（实际每帧持续值+1时，最大持续值=蓄力值100 * 1000毫秒/逻辑帧毫秒），实际设置监听每帧超过1000ms基本不太可能，如果真的那样做，每次蓄力会很慢，默认为50ms（建议与默认运行时钟保持一致，如果监听循环不是50ms可自行匹配一致）
        /// </summary>
        public int Period
        {
            get => _period;
            //如果用户输入错误，则什么也不变
            set
            {
                if (value >= 0 && value <= 2)
                {
                    _period = value;
                }
            }
        }

        private int _loop = 10;
        /// <summary>
        /// 设定多少逻辑帧算一次鼠标移动、键按下等操作的成功持续，默认10帧激活持续状态（若监听实际每帧50ms，那么就是按住0.5现实时间秒算一次持续）
        /// </summary>
        public int Loop { get => _loop; set => _loop = value; }

        private readonly MouseHook MyMouseHook;
        private readonly KeyboardHook MyKeyboardHook;

        #region 钩子开关

        /// <summary>
        /// 创建监听服务（默认玩家编号=1）
        /// </summary>
        public RecordService()//构造函数
        {
            _playerID = 1;
            MyMouseHook = MouseHook.GetMouseHook();
            MyKeyboardHook = KeyboardHook.GetKeyboardHook();
        }

        /// <summary>
        /// 创建监听服务
        /// </summary>
        public RecordService(int player)//构造函数
        {
            _playerID = player;
            MyMouseHook = MouseHook.GetMouseHook();
            MyKeyboardHook = KeyboardHook.GetKeyboardHook();
        }

        /// <summary>
        /// 开启鼠标钩子
        /// </summary>
        /// <param name="handler"></param>
        public void StartMouseHook()
        {
            MyMouseHook.AddMouseHandler(MouseEventHandler);
            MyMouseHook.Start();
        }

        /// <summary>
        /// 关闭鼠标钩子
        /// </summary>
        public void StopMouseHook()
        {
            MyMouseHook.Stop();
        }

        /// <summary>
        /// 开启键盘钩子
        /// </summary>
        /// <param name="handler"></param>
        public void StartKeyboardHook()
        {
            MyKeyboardHook.AddKeyboardHandler(KeyboardHandler);
            MyKeyboardHook.Start();
        }

        /// <summary>
        /// 关闭键盘钩子
        /// </summary>
        public void StopKeyboardHook()
        {
            MyKeyboardHook.Stop();
        }

        #endregion

        #region 热键操作

        #region 成员变量

        private int _x;

        /// <summary>
        /// 鼠标当前位置的X坐标
        /// </summary>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        private int _y;

        /// <summary>
        /// 鼠标当前位置的Y坐标
        /// </summary>
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private double _z;

        /// <summary>
        /// 鼠标当前位置的Z坐标
        /// </summary>
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        private int _wParam;

        /// <summary>
        /// 被按下的鼠标按键
        /// </summary>
        public int WParam
        {
            get { return _wParam; }
            set { _wParam = value; }
        }


        private int _KeyStatus;

        /// <summary>
        /// 键钮状态
        /// </summary>
        public int KeyStatus
        {
            get { return _KeyStatus; }
            set { _KeyStatus = value; }
        }

        private int _KeyValue;

        /// <summary>
        /// 键码
        /// </summary>
        public int KeyValue
        {
            get { return _KeyValue; }
            set { _KeyValue = value; }
        }

        private bool[] _CtrlAlt = new bool[2] { false, false };

        /// <summary>
        /// Ctrl 和 Alt
        /// </summary>
        public bool[] CtrlAlt
        {
            get { return _CtrlAlt; }
            set { _CtrlAlt = value; }
        }

        private bool[] _OneToNine = new bool[10] { false, false, false, false, false, false, false, false, false, false };

        /// <summary>
        /// 1-9
        /// </summary>
        public bool[] OneToNine
        {
            get { return _OneToNine; }
            set { _OneToNine = value; }
        }

        private int _playerID;

        /// <summary>
        /// 玩家编号
        /// </summary>
        public int PlayerID { get => _playerID; set => _playerID = value; }

        #endregion

        //以下处理动作不宜放时间复杂度高的函数，建议只记录变量变化，由其他线程读取这些变量后决定触发动作

        #region 鼠标事件处理

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wParam">鼠标事件状态</param>
        /// <param name="mouseMsg">存储着鼠标信息</param>
        private void MouseEventHandler(Int32 wParam, MouseHookStruct mouseMsg)
        {
            this.WParam = wParam;
            switch (wParam)
            {
                case WM_MOUSEMOVE:
                    // 记录鼠标移动
                    X = mouseMsg.pt.x;//UI坐标，是整数
                    Y = mouseMsg.pt.y;//UI坐标，是整数

                    //注：到此记录了WParam和X,Y即可，不宜写时间复杂度较高的逻辑，剩下的"按键总控"功能（给按键注册注销更换委托函数，用于蓄力、移动、释放技能、按弹菜单等游戏世界逻辑）通过读取本类信息另外开线程去制作即可

                    //思考：X,Y如何转化为世界坐标？
                    //Z从上述（X,Y）的信息中获得
                    //Z = MMCore.MapHeight + MMCore.TerrainHeight[X, Y] + (double)MMCore.DataTableLoad0(true, "Unit.TerrainHeight");
                    //MMCore.MouseMove(_playerID, new Vector3D(X, Y, Z), X, Y);
                    break;
                case WM_LBUTTONDOWN:
                    // 记录鼠标左键按下
                    //MMCore.MouseDown(_playerID, MMCore.c_mouseButtonLeft, Player.MouseVector3D[_playerID], X, Y);

                    //记录按键持续值，蓄力统计↓

                    //每帧持续值+1，最大持续值=蓄力值100 * (1000毫秒 /逻辑帧毫秒)
                    //if (MetalMaxSystem.Player.MouseDownLoopOneBitNum[Player, MMCore.c_mouseButtonLeft] < 100 * (1000 / _period))
                    //{
                    //    MetalMaxSystem.Player.MouseDownLoopOneBitNum[Player, MMCore.c_mouseButtonLeft] += 1;
                    //}
                    ////持续值如果>1，则算鼠标在持续操作
                    //if (MetalMaxSystem.Player.MouseDownLoopOneBitNum[Player, MMCore.c_mouseButtonLeft] > 1)
                    //{
                    //    MetalMaxSystem.Player.MouseDownLoop[Player, MMCore.c_mouseButtonLeft] = true;
                    //}
                    //else
                    //{
                    //    MetalMaxSystem.Player.MouseDownLoop[Player, MMCore.c_mouseButtonLeft] = false;
                    //}
                    break;
                case WM_LBUTTONUP:
                    // 记录鼠标左键弹起
                    //MMCore.MouseUp(_playerID, MMCore.c_mouseButtonLeft, Player.MouseVector3D[_playerID], X, Y);
                    break;
                case WM_LBUTTONDBLCLK:
                    // 记录鼠标左键双击
                    //Player.MouseDownTwice[_playerID, MMCore.c_mouseButtonLeft] = true;
                    break;
                case WM_RBUTTONDOWN:
                    // 记录鼠标右键按下
                    //MMCore.MouseDown(_playerID, MMCore.c_mouseButtonRight, Player.MouseVector3D[_playerID], X, Y);
                    break;
                case WM_RBUTTONUP:
                    // 记录鼠标右键弹起
                    //MMCore.MouseUp(_playerID, MMCore.c_mouseButtonRight, Player.MouseVector3D[_playerID], X, Y);
                    break;
                case WM_RBUTTONDBLCLK:
                    // 记录鼠标右键双击
                    //Player.MouseDownTwice[_playerID, MMCore.c_mouseButtonRight] = true;
                    break;
            }

            //以下动作可以搬到别的线程↓

            //检查全鼠标按键，如果松开，则该键有持续值的话每帧-1
            //for (; i <= 5; i++) { }
            //if (MetalMaxSystem.Player.MouseDown[Player, i] = true && MetalMaxSystem.Player.MouseDownLoopOneBitNum[Player, i] > 0)
            //{
            //    MetalMaxSystem.Player.MouseDownLoopOneBitNum[Player, i] -= 1;
            //}
        }

        #endregion

        #region 键盘事件处理
        // 虚拟键码
        private const int CTRL = 162;
        private const int ALT = 164;
        private const int ZERO = 48;
        /*
         * ctrl 162
         * alt 164
         * 1-9 -》 49-57
         * 0 -》48
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wParam">键盘事件状态</param>
        /// <param name="keyboardHookStruct">存储着虚拟键码</param>
        private void KeyboardHandler(Int32 wParam, KeyboardHookStruct keyboardHookStruct)
        {
            KeyStatus = wParam;
            KeyValue = keyboardHookStruct.vkCode;
            // 热键判断
            if (KeyStatus == KeyboardHook.WM_KEYDOWN || KeyStatus == KeyboardHook.WM_SYSKEYDOWN)
            {
                // 按下某个按钮
                switch (KeyValue)
                {
                    case CTRL:
                        CtrlAlt[0] = true;
                        break;
                    case ALT:
                        CtrlAlt[1] = true;
                        break;
                }
                if (KeyValue >= ZERO && KeyValue <= (ZERO + 9))
                {
                    // 按下了0-9
                    int temp = KeyValue - ZERO;
                    OneToNine[temp] = true;
                }
            }
            else if (KeyStatus == KeyboardHook.WM_KEYUP || KeyStatus == KeyboardHook.WM_SYSKEYUP)
            {
                // 松开某个按钮
                switch (KeyValue)
                {
                    case CTRL:
                        CtrlAlt[0] = false;
                        break;
                    case ALT:
                        CtrlAlt[1] = false;
                        break;
                }
                if (KeyValue >= ZERO && KeyValue <= (ZERO + 9))
                {
                    // 按下了0-9
                    int temp = KeyValue - ZERO;
                    OneToNine[temp] = false;
                }
            }
        }

        /// <summary>
        /// 是否按下了 Ctrl + alt + 0-9
        /// </summary>
        /// <returns>返回0-9代表按下了Ctrl+alt+0-9，返回-1代表没有按下</returns>
        public int IsPressTarget()
        {
            if ((!CtrlAlt[0]) || (!CtrlAlt[1]))
            {
                return -1;
            }
            for (int i = 0; i < 10; i++)
            {
                if (OneToNine[i])
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        #endregion
    }

    #endregion

    #region 键鼠钩子

    //虚拟键码：https://learn.microsoft.com/zh-cn/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
    //CSDN原址：https://blog.csdn.net/qq_43851684/article/details/113096306

    /// <summary>
    /// 【MetalMaxSystem】键盘钩子
    /// </summary>
    public class KeyboardHook
    {
        #region 常数和结构

        #region wParam键盘输入通知

        public const int WM_KEYDOWN = 0x100;    // 键盘被按下
        public const int WM_KEYUP = 0x101;      // 键盘被松开
        public const int WM_SYSKEYDOWN = 0x104; // 系统键被按下，例如Alt、Ctrl等键
        public const int WM_SYSKEYUP = 0x105;   // 系统键被松开，例如Alt、Ctrl等键
        public const int WH_KEYBOARD_LL = 13;

        #endregion

        [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型 
        public class KeyboardHookStruct

        {
            public int vkCode; //表示一个在1到254间的虚似键盘码 
            public int scanCode; //表示硬件扫描码 
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        #region 成员变量、委托、事件
        private static int hHook;
        private static HookProc KeyboardHookDelegate;
        /// <summary>
        /// 键盘回调委托
        /// </summary>
        /// <param name="wParam">按键的状态</param>
        /// <param name="keyboardHookStruct">存储着虚拟键码</param>
        public delegate void KeyboardHandler(Int32 wParam, KeyboardHookStruct keyboardHookStruct);
        // 键盘回调事件
        private static event KeyboardHandler Handlers;
        // 锁
        private readonly object lockObject = new object();
        // 当前状态,是否已经启动
        private volatile bool isStart = false;
        #endregion

        #region Win32的Api
        private delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        //安装钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        //下一个钩挂的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region 单例模式
        private static volatile KeyboardHook MyKeyboard;
        private readonly static object createLock = new object();
        private KeyboardHook() { }
        public static KeyboardHook GetKeyboardHook()
        {
            if (MyKeyboard == null)
            {
                lock (createLock)
                {
                    if (MyKeyboard == null)
                    {
                        MyKeyboard = new KeyboardHook();
                    }
                }
            }
            return MyKeyboard;
        }
        #endregion

        /// <summary>
        /// 安装钩子
        /// </summary>
        public void Start()
        {
            if (isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (isStart)
                {
                    return;
                }
                if (Handlers == null)
                {
                    throw new Exception("Please set handler first!Then run Start");
                }
                KeyboardHookDelegate = new HookProc(KeyboardHookProc);
                Process cProcess = Process.GetCurrentProcess();
                ProcessModule cModule = cProcess.MainModule;
                var mh = GetModuleHandle(cModule.ModuleName);
                hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
                isStart = true;
            }
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public void Stop()
        {
            if (!isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (!isStart)
                {
                    return;
                }
                UnhookWindowsHookEx(hHook);
                // 清除所有事件
                Handlers = null;
                isStart = false;
            }
        }

        /// <summary>
        /// 键盘的系统回调函数
        /// </summary>
        /// <param name="nCode">如果代码小于零，则挂钩过程必须将消息传递给CallNextHookEx函数，而无需进一步处理，并且应返回CallNextHookEx返回的值。此参数可以是下列值之一(见虚拟键码)</param>
        /// <param name="wParam">鼠标输入通知，代表发生的鼠标的事件，各种虚拟键事件发生后传入此参（WM_）</param>
        /// <param name="lParam">平台特定整数类型（是一个结构体），用于本机资源（存储着互动窗口句柄等相关事件信息）多线程安全使用</param>
        /// <returns></returns>
        private static int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
            if ((nCode >= 0) && Handlers != null)
            {
                KeyboardHookStruct KeyDataFromHook = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Handlers(wParam, KeyDataFromHook);
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 添加按键的回调函数
        /// </summary>
        /// <param name="handler"></param>
        public void AddKeyboardHandler(KeyboardHandler handler)
        {
            Handlers += handler;
        }

        /// <summary>
        /// 删除指定按键的回调函数
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveKeyboardHandler(KeyboardHandler handler)
        {
            if (Handlers != null)
            {
                Handlers -= handler;
            }
        }
    }

    /// <summary>
    /// 【MetalMaxSystem】鼠标钩子
    /// </summary>
    public class MouseHook
    {
        #region 常量（用于匹配鼠标输入通知）
        public const int WM_MOUSEMOVE = 0x200; // 鼠标移动
        public const int WM_LBUTTONDOWN = 0x201;// 鼠标左键按下
        public const int WM_RBUTTONDOWN = 0x204;// 鼠标右键按下
        public const int WM_MBUTTONDOWN = 0x207;// 鼠标中键按下
        public const int WM_LBUTTONUP = 0x202;// 鼠标左键抬起
        public const int WM_RBUTTONUP = 0x205;// 鼠标右键抬起
        public const int WM_MBUTTONUP = 0x208;// 鼠标中键抬起
        public const int WM_LBUTTONDBLCLK = 0x203;// 鼠标左键双击
        public const int WM_RBUTTONDBLCLK = 0x206;// 鼠标右键双击
        public const int WM_MBUTTONDBLCLK = 0x209;// 鼠标中键双击
        public const int WH_MOUSE_LL = 14; //可以截获整个系统所有模块的鼠标事件。
        #endregion

        #region 成员变量、回调函数、事件
        /// <summary>
        /// 钩子回调常规函数引用（委托类型），特征：int HookProc(int nCode, Int32 wParam, IntPtr lParam)
        /// </summary>
        /// <param name="nCode">如果代码小于零，则挂钩过程必须将消息传递给CallNextHookEx函数，而无需进一步处理，并且应返回CallNextHookEx返回的值。此参数可以是下列值之一(见虚拟键码)</param>
        /// <param name="wParam">鼠标输入通知，代表发生的鼠标的事件，各种虚拟键事件发生后传入此参（WM_）</param>
        /// <param name="lParam">平台特定整数类型（是一个结构体），用于本机资源（存储着互动窗口句柄等相关事件信息）多线程安全使用</param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        /// <summary>
        /// 全局鼠标常规函数引用（委托类型）
        /// </summary>
        /// <param name="wParam"> 鼠标输入通知，代表发生的鼠标的事件，各种虚拟键事件发生后传入此参（WM_）</param>
        /// <param name="mouseMsg">鼠标钩子结构体，存储着鼠标位置、互动窗口句柄等相关事件信息</param>
        public delegate void MyMouseEventHandler(Int32 wParam, MouseHookStruct mouseMsg);
        private event MyMouseEventHandler OnMouseActivity;
        // 声明鼠标钩子事件类型
        private HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄
        // 锁
        private readonly object lockObject = new object();
        // 当前状态,是否已经启动
        private bool isStart = false;
        #endregion

        #region Win32的API
        /// <summary>
        /// 钩子结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            /// <summary>
            /// 指定在屏幕坐标系下，包含有光标x、y坐标的POINT结构（鼠标位置）
            /// </summary>
            public POINT pt;
            /// <summary>
            /// 希望对鼠标事件做出响应、接收鼠标消息的窗体的句柄
            /// </summary>
            public int hWnd;
            /// <summary>
            /// 指定点击测试值，查看WM_NCHITTEST消息可以得到值的列表
            /// </summary>
            public int wHitTestCode;
            /// <summary>
            /// 指定和该消息相关联的附加信息
            /// </summary>
            public int dwExtraInfo;
        }

        //声明一个Point的封送类型  
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        // 装置钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        #endregion


        #region 构造(单例模式)与析构函数
        private static volatile MouseHook MyMouseHook;
        private readonly static object createLock = new object();
        private MouseHook() { }

        public static MouseHook GetMouseHook()
        {
            if (MyMouseHook == null)
            {
                lock (createLock)
                {
                    if (MyMouseHook == null)
                    {
                        MyMouseHook = new MouseHook();
                    }
                }
            }
            return MyMouseHook;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MouseHook()
        {
            Stop();
        }
        #endregion


        /// <summary>
        /// 启动全局钩子
        /// </summary>
        public void Start()
        {
            if (isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (isStart)
                {
                    return;
                }
                if (OnMouseActivity == null)
                {
                    throw new Exception("Please set handler first!Then run Start");
                }
                // 安装鼠标钩子
                if (_hMouseHook == 0)
                {
                    // 生成一个HookProc的实例.
                    _mouseHookProcedure = new HookProc(MouseHookProc);
                    _hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                    //假设装置失败停止钩子
                    if (_hMouseHook == 0)
                    {
                        Stop();
                        throw new Exception("SetWindowsHookEx failed.");
                    }
                }
                isStart = true;
            }
        }

        /// <summary>
        /// 停止全局钩子
        /// </summary>
        public void Stop()
        {
            if (!isStart)
            {
                return;
            }
            lock (lockObject)
            {
                if (!isStart)
                {
                    return;
                }
                bool retMouse = true;
                if (_hMouseHook != 0)
                {
                    retMouse = UnhookWindowsHookEx(_hMouseHook);
                    _hMouseHook = 0;
                }
                // 假设卸下钩子失败
                if (!(retMouse))
                    throw new Exception("UnhookWindowsHookEx failed.");
                // 删除所有事件
                OnMouseActivity = null;
                // 标志位改变
                isStart = false;
            }
        }

        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 假设正常执行而且用户要监听鼠标的消息
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                OnMouseActivity(wParam, MyMouseHookStruct);
            }
            // 启动下一次钩子
            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 注册全局鼠标事件
        /// </summary>
        /// <param name="handler"></param>
        public void AddMouseHandler(MyMouseEventHandler handler)
        {
            OnMouseActivity += handler;
        }

        /// <summary>
        /// 注销全局鼠标事件
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveMouseHandler(MyMouseEventHandler handler)
        {
            if (OnMouseActivity != null)
            {
                OnMouseActivity -= handler;
            }
        }
    }

    #endregion

    #endregion

    #endregion

}

