using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MetalMaxSystem;

namespace MM_Obfuscar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Button of Run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //正则匹配抓取全部函数名（包含重复）
            if (richTextBox1.Text != "")
            {
                // 定义正则表达式模式，匹配函数名
                string pattern = @"(?<=^|[^a-zA-Z_])[a-zA-Z_][\w]*(?=\s*\([^\)]*\)(\s+|\n|$))";
                MatchCollection matches = Regex.Matches(richTextBox1.Text, pattern);
                CodeObfuscator obfuscator = new CodeObfuscator();
                //遍历全部函数名
                foreach (Match match in matches)
                {
                    Console.WriteLine("Function Name: " + match.Value);
                    //添加函数名及混肴后名称到混肴规则（这过程会自动去重，也不会生成相同混肴名称）
                    obfuscator.AddReplacement(match.Value);
                }
                string obfuscatedCode = obfuscator.ObfuscateCode(richTextBox1.Text);
                richTextBox1.Text = obfuscatedCode;
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
