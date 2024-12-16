using System;
using System.Windows.Forms;
using BattleTest;

namespace MMWFM
{
    //【范例】WinForm引用MM_函数库进行测试
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            AI.GameInit();
        }
    }
}
