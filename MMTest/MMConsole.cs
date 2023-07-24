using System;
using HtmlAgilityPack;
using MetalMaxSystem;
using System.Threading;

namespace MMConsole
{
    public class MMConsole
    {
        public static void Main() //入口主函数只有1个，设置静态后它只从模板形成1个活动副本
        {
            MMCore.RegistEntryEventFuncref(Entry.MainUpdate, MUpdate);
            MMCore.MainUpdateStart(false);

            //MMCore.RegistEntryEventFuncref(Entry.SubUpdate, MUpdate);
            //MMCore.SubUpdateStart(false);

            //TimerUpdate t = new TimerUpdate();
            //t.TriggerStart(false);

        }

        public static void MUpdate()
        {
            Console.WriteLine("{0} Main Update {1,2}.", DateTime.Now.ToString("h:mm:ss.fff"), MainUpdateChecker.InvokeCount.ToString());
        }

    }
}
