
namespace MMObfuscar
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
            this.panel_Top = new System.Windows.Forms.Panel();
            this.checkBox_LC4 = new System.Windows.Forms.CheckBox();
            this.comboBox_SelectFunc = new System.Windows.Forms.ComboBox();
            this.label_SelectFunc = new System.Windows.Forms.Label();
            this.label_ExclusionRulesFile = new System.Windows.Forms.Label();
            this.button_SelectExclusionRulesFile = new System.Windows.Forms.Button();
            this.textBox_ExclusionRulesPath = new System.Windows.Forms.TextBox();
            this.button_Run = new System.Windows.Forms.Button();
            this.richTextBox_Code = new System.Windows.Forms.RichTextBox();
            this.label_Tips = new System.Windows.Forms.Label();
            this.label_Statistics = new System.Windows.Forms.Label();
            this.panel_Top.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Top
            // 
            this.panel_Top.Controls.Add(this.label_Statistics);
            this.panel_Top.Controls.Add(this.label_Tips);
            this.panel_Top.Controls.Add(this.checkBox_LC4);
            this.panel_Top.Controls.Add(this.comboBox_SelectFunc);
            this.panel_Top.Controls.Add(this.label_SelectFunc);
            this.panel_Top.Controls.Add(this.label_ExclusionRulesFile);
            this.panel_Top.Controls.Add(this.button_SelectExclusionRulesFile);
            this.panel_Top.Controls.Add(this.textBox_ExclusionRulesPath);
            this.panel_Top.Controls.Add(this.button_Run);
            this.panel_Top.Location = new System.Drawing.Point(6, 12);
            this.panel_Top.Name = "panel_Top";
            this.panel_Top.Size = new System.Drawing.Size(787, 68);
            this.panel_Top.TabIndex = 4;
            // 
            // checkBox_LC4
            // 
            this.checkBox_LC4.Checked = true;
            this.checkBox_LC4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_LC4.Location = new System.Drawing.Point(662, 43);
            this.checkBox_LC4.Name = "checkBox_LC4";
            this.checkBox_LC4.Size = new System.Drawing.Size(54, 21);
            this.checkBox_LC4.TabIndex = 7;
            this.checkBox_LC4.Text = "LC4";
            this.checkBox_LC4.UseVisualStyleBackColor = true;
            // 
            // comboBox_SelectFunc
            // 
            this.comboBox_SelectFunc.FormattingEnabled = true;
            this.comboBox_SelectFunc.Items.AddRange(new object[] {
            "对Galaxy代码进行混肴"});
            this.comboBox_SelectFunc.Location = new System.Drawing.Point(106, 44);
            this.comboBox_SelectFunc.Name = "comboBox_SelectFunc";
            this.comboBox_SelectFunc.Size = new System.Drawing.Size(550, 20);
            this.comboBox_SelectFunc.TabIndex = 7;
            // 
            // label_SelectFunc
            // 
            this.label_SelectFunc.Location = new System.Drawing.Point(3, 43);
            this.label_SelectFunc.Name = "label_SelectFunc";
            this.label_SelectFunc.Size = new System.Drawing.Size(100, 21);
            this.label_SelectFunc.TabIndex = 7;
            this.label_SelectFunc.Text = "选择功能";
            this.label_SelectFunc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_ExclusionRulesFile
            // 
            this.label_ExclusionRulesFile.Location = new System.Drawing.Point(3, 22);
            this.label_ExclusionRulesFile.Name = "label_ExclusionRulesFile";
            this.label_ExclusionRulesFile.Size = new System.Drawing.Size(100, 21);
            this.label_ExclusionRulesFile.TabIndex = 5;
            this.label_ExclusionRulesFile.Text = "排除规则文件";
            this.label_ExclusionRulesFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_SelectExclusionRulesFile
            // 
            this.button_SelectExclusionRulesFile.Location = new System.Drawing.Point(662, 22);
            this.button_SelectExclusionRulesFile.Name = "button_SelectExclusionRulesFile";
            this.button_SelectExclusionRulesFile.Size = new System.Drawing.Size(54, 21);
            this.button_SelectExclusionRulesFile.TabIndex = 4;
            this.button_SelectExclusionRulesFile.Text = "Sclect";
            this.button_SelectExclusionRulesFile.UseVisualStyleBackColor = true;
            this.button_SelectExclusionRulesFile.Click += new System.EventHandler(this.button_SelectExclusionRulesFile_Click);
            // 
            // textBox_ExclusionRulesPath
            // 
            this.textBox_ExclusionRulesPath.Location = new System.Drawing.Point(106, 22);
            this.textBox_ExclusionRulesPath.Name = "textBox_ExclusionRulesPath";
            this.textBox_ExclusionRulesPath.Size = new System.Drawing.Size(550, 21);
            this.textBox_ExclusionRulesPath.TabIndex = 3;
            // 
            // button_Run
            // 
            this.button_Run.Location = new System.Drawing.Point(716, 22);
            this.button_Run.Name = "button_Run";
            this.button_Run.Size = new System.Drawing.Size(68, 43);
            this.button_Run.TabIndex = 2;
            this.button_Run.Text = "执行";
            this.button_Run.UseVisualStyleBackColor = true;
            this.button_Run.Click += new System.EventHandler(this.button_Run_Click);
            // 
            // richTextBox_Code
            // 
            this.richTextBox_Code.Location = new System.Drawing.Point(6, 86);
            this.richTextBox_Code.Name = "richTextBox_Code";
            this.richTextBox_Code.Size = new System.Drawing.Size(787, 432);
            this.richTextBox_Code.TabIndex = 6;
            this.richTextBox_Code.Text = "";
            // 
            // label_Tips
            // 
            this.label_Tips.Location = new System.Drawing.Point(106, 1);
            this.label_Tips.Name = "label_Tips";
            this.label_Tips.Size = new System.Drawing.Size(550, 21);
            this.label_Tips.TabIndex = 10;
            this.label_Tips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Statistics
            // 
            this.label_Statistics.Location = new System.Drawing.Point(662, 1);
            this.label_Statistics.Name = "label_Statistics";
            this.label_Statistics.Size = new System.Drawing.Size(122, 21);
            this.label_Statistics.TabIndex = 11;
            this.label_Statistics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 522);
            this.Controls.Add(this.richTextBox_Code);
            this.Controls.Add(this.panel_Top);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel_Top.ResumeLayout(false);
            this.panel_Top.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel_Top;
        private System.Windows.Forms.CheckBox checkBox_LC4;
        private System.Windows.Forms.ComboBox comboBox_SelectFunc;
        private System.Windows.Forms.Label label_SelectFunc;
        private System.Windows.Forms.Label label_ExclusionRulesFile;
        private System.Windows.Forms.Button button_SelectExclusionRulesFile;
        private System.Windows.Forms.TextBox textBox_ExclusionRulesPath;
        private System.Windows.Forms.Button button_Run;
        private System.Windows.Forms.RichTextBox richTextBox_Code;
        private System.Windows.Forms.Label label_Tips;
        private System.Windows.Forms.Label label_Statistics;
    }
}

