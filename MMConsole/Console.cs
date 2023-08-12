using System;
using MetalMaxSystem;

namespace MMConsole
{
    public class Console
    {

        public static void Main(string[] args)
        {

            TimerUpdate A = new TimerUpdate();
            A.Awake += AwakeTest;
            A.Update += UpdateTest;
            A.Period = 500;//周期间隔
            A.TriggerStart(false);//触发器执行

        }

        public static void AwakeTest(object sender, EventArgs e) 
        {
            //要执行的动作
        }

        public static void UpdateTest(object sender, EventArgs e)
        {
            //要周期执行的动作
        }

        public static void TestA(params object[] e)
        {
            //测试多参任意类型
        }

    }
}
