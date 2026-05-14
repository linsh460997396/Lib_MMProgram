using MetalMaxSystem;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MMConsole
{
    public class MyConsole
    {
        private static int _playerId = 1;

        private static void OnKeySpacePressed(bool isKeyDown, int player)
        {
            Console.WriteLine($"玩家{player} 空格键 {(isKeyDown ? "按下" : "释放")}");
        }

        private static void OnKeyWPressed(bool isKeyDown, int player)
        {
            Console.WriteLine($"玩家{player} W键 {(isKeyDown ? "按下" : "释放")}");
        }

        private static void OnKeyEscapePressed(bool isKeyDown, int player)
        {
            Console.WriteLine($"玩家{player} ESC键 {(isKeyDown ? "按下" : "释放")}");
            if (isKeyDown)
            {
                Console.WriteLine("检测到ESC按下，准备退出测试...");
                Environment.Exit(0);
            }
        }

        private static void OnKeyCharge(int player, int key, float chargeValue)
        {
            string keyName = GetKeyName(key);
            Console.WriteLine($"【蓄力事件】玩家{player} 按键[{keyName}] 蓄力值: {chargeValue:F2}");
        }

        private static void OnKeyDoubleClick(int player, int key, float timeDiff)
        {
            string keyName = GetKeyName(key);
            Console.WriteLine($"【双击事件】玩家{player} 按键[{keyName}] 双击成功！间隔: {timeDiff:F3}秒");
        }

        private static string GetKeyName(int key)
        {
            switch (key)
            {
                case 39: return "空格键";
                case 35: return "W键";
                case 31: return "S键";
                case 36: return "X键";
                case 66: return "ESC键";
                default: return $"按键码{key}";
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("=== MMCore 按键事件功能测试（含蓄力和双击）===");
            Console.WriteLine("操作说明：");
            Console.WriteLine("  - 按空格键/W键测试普通按键");
            Console.WriteLine("  - 按住X键测试蓄力（按住越久蓄力值越高）");
            Console.WriteLine("  - 快速按两次S键测试双击");
            Console.WriteLine("  - 按ESC退出\n");

            // 注册普通按键事件
            Console.WriteLine("1. 注册普通按键事件委托...");
            MMCore.RegistKeyEventFuncref(MMCore.c_keySpace, OnKeySpacePressed);
            MMCore.RegistKeyEventFuncref(MMCore.c_keyW, OnKeyWPressed);
            MMCore.RegistKeyEventFuncref(MMCore.c_keyEscape, OnKeyEscapePressed);

            // 启用蓄力和双击管理
            Console.WriteLine("2. 启用蓄力和双击管理...");
            MMCore.StartXuLiGuanLi(true);
            MMCore.StartShuangJiGuanLi(true);

            // 订阅蓄力和双击事件
            Console.WriteLine("3. 订阅蓄力和双击事件...");
            MMCore.KeyChargeEvent += OnKeyCharge;
            MMCore.KeyDoubleClickEvent += OnKeyDoubleClick;

            Console.WriteLine("\n4. 进入实时按键监听循环...");
            Console.WriteLine("按任意键测试（按ESC退出）\n");

            // 创建一个后台线程来处理蓄力和双击的更新循环
            Thread updateThread = new Thread(UpdateLoop);
            updateThread.IsBackground = true;
            updateThread.Start();

            // 主循环：监听键盘输入
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    int keyCode = ConvertConsoleKeyToMMCoreKey(keyInfo.Key);

                    if (keyCode != -1)
                    {
                        // 触发普通按键事件
                        MMCore.KeyDownGlobalEvent(keyCode, true, _playerId);

                        // 处理蓄力和双击（按键按下）
                        MMCore.OnKeyDown(_playerId, keyCode);

                        // 模拟按键释放（测试蓄力需要手动释放）
                        if (keyInfo.Key == ConsoleKey.X)
                        {
                            // 对于X键，等待一会儿再释放以模拟蓄力
                            Console.WriteLine("按住X键蓄力中...(按任意键释放)");
                            Console.ReadKey(true);
                            MMCore.OnKeyUp(_playerId, keyCode);
                            MMCore.KeyDownGlobalEvent(keyCode, false, _playerId);
                        }
                        else if (keyInfo.Key == ConsoleKey.S)
                        {
                            // 对于S键，快速双击测试
                            Thread.Sleep(100);
                        }
                        else
                        {
                            // 其他按键立即释放
                            Thread.Sleep(50);
                            MMCore.OnKeyUp(_playerId, keyCode);
                            MMCore.KeyDownGlobalEvent(keyCode, false, _playerId);
                        }
                    }

                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
                Thread.Sleep(50);
            }

            // 清理
            Console.WriteLine("\n5. 清理资源...");
            MMCore.KeyChargeEvent -= OnKeyCharge;
            MMCore.KeyDoubleClickEvent -= OnKeyDoubleClick;
            MMCore.RemoveKeyEventFuncref(MMCore.c_keySpace, OnKeySpacePressed);
            MMCore.RemoveKeyEventFuncref(MMCore.c_keyW, OnKeyWPressed);
            MMCore.RemoveKeyEventFuncref(MMCore.c_keyEscape, OnKeyEscapePressed);
            MMCore.StartXuLiGuanLi(false);
            MMCore.StartShuangJiGuanLi(false);

            Console.WriteLine("测试完成！");
        }

        private static void UpdateLoop()
        {
            while (true)
            {
                // 更新蓄力值（每帧调用）
                MMCore.UpdateXuLi(_playerId);
                // 更新双击计时器（每帧调用）
                MMCore.UpdateShuangJi(_playerId);
                Thread.Sleep(16); // 约60fps
            }
        }

        private static int ConvertConsoleKeyToMMCoreKey(ConsoleKey consoleKey)
        {
            switch (consoleKey)
            {
                case ConsoleKey.Spacebar: return MMCore.c_keySpace;
                case ConsoleKey.W: return MMCore.c_keyW;
                case ConsoleKey.A: return MMCore.c_keyA;
                case ConsoleKey.S: return MMCore.c_keyS;
                case ConsoleKey.D: return MMCore.c_keyD;
                case ConsoleKey.X: return MMCore.c_keyX;
                case ConsoleKey.Escape: return MMCore.c_keyEscape;
                case ConsoleKey.Enter: return MMCore.c_keyEnter;
                case ConsoleKey.Backspace: return MMCore.c_keyBackSpace;
                case ConsoleKey.Tab: return MMCore.c_keyTab;
                case ConsoleKey.LeftArrow: return MMCore.c_keyLeft;
                case ConsoleKey.RightArrow: return MMCore.c_keyRight;
                case ConsoleKey.UpArrow: return MMCore.c_keyUp;
                case ConsoleKey.DownArrow: return MMCore.c_keyDown;
                default: return -1;
            }
        }
    }
}