using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using MetalMaxSystem;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MMConsole
{
    public class MyConsole
    {
        const int IIl=((((~24868140)-(0XD909DDD+((0x8<<(~-0Xe))^(~227209790))))^(~-(((~-6245796)-(~-166309)+0X63661)^(~-0xA17))))+(274500^(((~-0x205aff3)^0X68D793)-(~-8820654))));
        const int ll=(((((~0X4D6A2D3)-0x8b957b)^0X6f4d96)+(~-0x51175e3))^(((~-0X40002)^(1<<0x12))<<(~-(967800^((0x199f2^0x60acba)-0X5270DD)))));
        const int lI1=(((((~-0xc3855)+(0x8f8^((-0X369c0-178428+0X63524)^(~-8159)))+((~-800853)^(((~0X30C5B)-(~-0x3a63)+0X35132)^0x7c2))+((~-2207425)^0x2fcc9b)+-0x26c829)-((~-1522326)^(~-454271)))^-12744)^(((-(~-323417)+((~-0XAC4)^(2664^(175614-(~-0X35F59)+415442)))+((~-0X4ef59)^(0X944^((~-0x5E551)-28374+(~-12251))))+((~-3388786)^(~-2404319))+(~1270369))^(~-(-(~-0Xd18ca)+(280^((~-8487)^(0X5a9c9-0x259FC+(~-11149561))))+((~-0xd18ca)^(9766^((~-342389)-0x38098+(~-0XABBEEA))))+(1653944^0X22310D)+-13516078)))-(-(~-621641)+((~-3438)^((~-9092)^((~-0X21135)-(~-31877)+(~-9803543))))+(0x97C48^(0xdf^(0X5f939-(~-0xb977)+9554135)))+((~-0x1BAC75)^0X20B52F)+-13635075)));

        public static void Main(string[] args)
        {
            //TimerUpdate A = new TimerUpdate();
            //A.Awake += AwakeTest;
            //A.Update += UpdateTest;
            //A.Period = 500;//周期间隔
            //A.TriggerStart(false);//触发器执行

            //int number = 10; // 示例数字
            //string expression = ConvertToBitwiseExpression(number); // 获取位运算表达式
            //Debug.WriteLine(expression); // 输出表达式
            //                               // 计算表达式的值
            //int result = 0;
            //for (int i = expression.Length - 1; i >= 0; i--)
            //{
            //    if (expression[i] == '1') result += (int)Math.Pow(2, expression.Length - i - 1);
            //}
            //Debug.WriteLine("Result: " + result); // 输出结果

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
