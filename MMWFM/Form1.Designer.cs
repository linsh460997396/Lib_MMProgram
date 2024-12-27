
namespace MMWFM
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            richTextBox_Code = new System.Windows.Forms.RichTextBox();
            button_Run = new System.Windows.Forms.Button();
            SuspendLayout();
            //
            //richTextBox_Code
            //
            richTextBox_Code.Location = new System.Drawing.Point(7, 154);
            richTextBox_Code.Margin = new System.Windows.Forms.Padding(4);
            richTextBox_Code.Name = "richTextBox_Code";
            richTextBox_Code.Size = new System.Drawing.Size(918, 470);
            richTextBox_Code.TabIndex = 7;
            richTextBox_Code.Text = "";
            //
            //button_Run
            //
            button_Run.Location = new System.Drawing.Point(841, 85);
            button_Run.Margin = new System.Windows.Forms.Padding(4);
            button_Run.Name = "button_Run";
            button_Run.Size = new System.Drawing.Size(79, 61);
            button_Run.TabIndex = 2;
            button_Run.Text = "执行";
            button_Run.UseVisualStyleBackColor = true;
            //
            //Form1
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(933, 638);
            Controls.Add(button_Run);
            Controls.Add(richTextBox_Code);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_Code;
        private System.Windows.Forms.Button button_Run;
    }
}

