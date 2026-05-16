using MetalMaxSystem;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MMConsole
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("[DEBUG] 程序启动...");
                Console.Out.Flush();
                
                // ========== 阶段1: 初始化配置 ==========

                // 1. 创建输入服务（绑定玩家1）
                Console.WriteLine("[DEBUG] 创建RecordService...");
                Console.Out.Flush();
                RecordService recordService = new RecordService(1);
                Console.WriteLine("[DEBUG] RecordService创建完成");
                Console.Out.Flush();

                // 2. [关键]先订阅事件（防止事件丢失）
                MMCore.KeyDoubleClickEvent += (player, key, timeDiff) =>
                    Console.WriteLine($"[双击检测] 玩家{player} 双击键[{key}] 时间差: {timeDiff:F3}s");

                MMCore.MouseDoubleClickEvent += (player, btn, timeDiff) =>
                    Console.WriteLine($"[双击检测] 玩家{player} 双击鼠标[{btn}] 时间差: {timeDiff:F3}s");

                // 可选：订阅更多调试事件
                // MMCore.KeyDownGlobalEvent += (key, isDown, player) => 
                //     Console.WriteLine($"[按键状态] 玩家{player} 键[{key}] {(isDown ? "按下" : "弹起")}");

                // ========== 阶段2: 绑定事件 ==========

                // 3. 绑定MMCore内置方法到输入服务
                MMCore.AddKeyMouseEvent(recordService, true);

                // 4. 注册周期更新方法到主循环
                MMCore.RegistEntryEventFuncref(Entry.MainUpdate, MMCore.ChargeManager);
                MMCore.RegistEntryEventFuncref(Entry.MainUpdate, MMCore.DoubleClickManager);

                // 处理所有玩家的按键队列（假设有1~15个玩家）
                MMCore.RegistEntryEventFuncref(Entry.MainUpdate, () =>
                {
                    for (int player = 1; player <= 1; player++)
                    {
                        MMCore.MouseKeyUpWait(player);
                    }
                });

                // ========== 阶段3: 启动系统 ==========

                // 5. [关键]使用前台线程启动主循环
                Console.Write("正在启动主循环...");
                Console.Out.Flush();
                MainUpdate.Start(isBackground: false);
                Console.WriteLine("完成");

                // 6. 启动系统钩子
                Console.Write("正在启动鼠标钩子...");
                Console.Out.Flush();
                recordService.StartMouseHook();
                Console.WriteLine("完成");

                Console.Write("正在启动键盘钩子...");
                Console.Out.Flush();
                recordService.StartKeyboardHook();
                Console.WriteLine("完成");

                // ========== 阶段4: 运行与阻塞 ==========

                Console.WriteLine();
                Console.WriteLine("========== 系统已就绪 ==========");
                Console.WriteLine("测试说明:");
                Console.WriteLine("  - 双击键盘任意键测试双击功能");
                Console.WriteLine("  - 长按按键测试蓄力功能");
                Console.WriteLine("  - 按 ESC 键退出程序");
                Console.WriteLine("==================================");
                Console.WriteLine();

                // [关键]阻塞主线程，等待用户退出
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("收到退出信号...");
                        break;
                    }
                }

                // ========== 阶段5: 资源清理 ==========

                Console.WriteLine("正在停止钩子...");
                recordService.StopMouseHook();
                recordService.StopKeyboardHook();

                Console.WriteLine("正在停止主循环...");
                MainUpdate.TimerStop = true;

                Console.WriteLine("程序已退出");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"程序异常: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}