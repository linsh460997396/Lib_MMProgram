using System;

namespace MMConsole
{
    public class MyConsole
    {

        public static void Main(string[] args)
        {
            //TimerUpdate A = new TimerUpdate();
            //A.Awake += AwakeTest;
            //A.Update += UpdateTest;
            //A.Period = 500;//周期间隔
            //A.TriggerStart(false);//触发器执行

            //int number = 10; //示例数字
            //string expression = ConvertToBitwiseExpression(number); //获取位运算表达式
            //MMCore.Tell(expression); //输出表达式
            //                              //计算表达式的值
            //int result = 0;
            //for (int i = expression.Length - 1; i >= 0; i--)
            //{
            //   if (expression[i] == '1') result += (int)Math.Pow(2, expression.Length - i - 1);
            //}
            //MMCore.Tell("Result: " + result); //输出结果

            //MMCore.Tell(MMCore.ConvertStringToHex("TriggerLibs/NativeLib"));
            //MMCore.Tell("\x54\x72\x69\x67\x67\x65\x72\x4C\x69\x62\x73\x2F\x4E\x61\x74\x69\x76\x65\x4C\x69\x62");
            //MMCore.Tell(MMCore.ConvertStringToHOMixed("TriggerLibs/NativeLib", 0.7)); //"\0124\0114"的十进制是84和76，Galaxy脚本中识别为TL

        }
        public static int Confuse(int input)
        {
            int gv_inlineprevent = 0;
            int I1l = 123456789;
            int llll = 987654321;
            gv_inlineprevent += ((~0x117C3C1E ^ I1l) ^ (1 << (~6 + llll)));
            return gv_inlineprevent;
        }

        public static int Confuse2(int input)
        {
            int gv_inlineprevent = 0;
            int I1l = 123456789;
            int llll = 987654321;
            int confusedValue = ((~0x117C3C1E ^ I1l) ^ (1 << (~6 + llll)));
            gv_inlineprevent += confusedValue;
            int deconfusedValue = ((~confusedValue ^ I1l) ^ (1 << (~6 + llll)));
            gv_inlineprevent -= deconfusedValue;
            return gv_inlineprevent;
        }

        /// <summary>
        /// 转二进制表达式
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertToBitwiseExpression(int number)
        {
            if (number == 0) return "0";
            string expression = "";
            while (number > 0)
            {
                if (number % 2 == 1)
                {
                    expression = "1" + expression;
                }
                else
                {
                    expression = "0" + expression;
                }
                number /= 2;
            }
            return expression;
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
