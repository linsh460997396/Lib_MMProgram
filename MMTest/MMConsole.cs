using System;
using HtmlAgilityPack;
using MetalMaxSystem;
using System.Threading;

namespace MMConsole
{
    //【范例】控制台引用MM_函数库进行测试
    public static class MMConsole
    {
        public static void Main() //入口主函数只有1个，设置静态后它只从模板形成1个活动副本
        {
            MMCore.RegistEntryEventFuncref(Entry.MainUpdate, MUpdate);
            MMCore.MainUpdateStart(false);

            //MMCore.RegistEntryEventFuncref(Entry.SubUpdate, MUpdate);
            //MMCore.SubUpdateStart(false);

            //TimerUpdate t = new TimerUpdate();//创建一个周期触发器（自带线程，有5个阶段，其中循环阶段会暂停触发线程执行Timer线程，当接受到t.TimerState = false，退出循环且恢复触发线程去执行End、Destory）
            //t.Awake += TimerTrigger;//
            //t.Start += TimerTrigger;
            //t.Update += TimerTrigger;
            //t.End += TimerTrigger; //t.TimerState = false 触发器线程终止时运行一次
            //t.Destory += TimerTrigger;//触发器线程摧毁阶段运行一次（可以自己指定一个委托函数，不指定也没事）
            //t.TriggerStart(false);

            //t.TimerState = false; //触发器线程终止

            //SubActionEventFuncref Actions = SubActionTest;
            //SubActionEventFuncref Actions += SubActionTest1;
            //SubActionEventFuncref Actions += SubActionTest2;
            //MMCore.HD_ForEachObjectNumFromGroup("A11", 1, 1, Actions);
            //MMCore.HD_ForEachObjectNumFromGroup("A11", 1, 1, SubActionTest);

        }

        public static void MUpdate()
        {
            Console.WriteLine("{0} Main Update {1,2}.", DateTime.Now.ToString("h:mm:ss.fff"), MainUpdateChecker.InvokeCount.ToString());
        }

        public static void TimerTrigger(object sender, EventArgs e)
        {
            Console.WriteLine("{0} Main Update {1,2}.", DateTime.Now.ToString("h:mm:ss.fff"), MainUpdateChecker.InvokeCount.ToString());
        }

        //public static void SubActionTest(object sender)
        //{
        //    //填入HD_ForEachPointNumFromGroupFunc后，每次遍历序号会自动作为lp_var 
        //}

    }
}
