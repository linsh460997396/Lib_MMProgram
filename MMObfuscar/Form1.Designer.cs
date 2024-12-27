
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
            panel_Top = new System.Windows.Forms.Panel();
            label_Statistics = new System.Windows.Forms.Label();
            label_Tips = new System.Windows.Forms.Label();
            checkBox_LC4 = new System.Windows.Forms.CheckBox();
            comboBox_SelectFunc = new System.Windows.Forms.ComboBox();
            label_SelectFunc = new System.Windows.Forms.Label();
            label_ExclusionRulesFile = new System.Windows.Forms.Label();
            button_SelectExclusionRulesFile = new System.Windows.Forms.Button();
            textBox_ExclusionRulesPath = new System.Windows.Forms.TextBox();
            button_Run = new System.Windows.Forms.Button();
            richTextBox_Code = new System.Windows.Forms.RichTextBox();
            panel_Top.SuspendLayout();
            SuspendLayout();
            //
            //panel_Top
            //
            panel_Top.Controls.Add(label_Statistics);
            panel_Top.Controls.Add(label_Tips);
            panel_Top.Controls.Add(checkBox_LC4);
            panel_Top.Controls.Add(comboBox_SelectFunc);
            panel_Top.Controls.Add(label_SelectFunc);
            panel_Top.Controls.Add(label_ExclusionRulesFile);
            panel_Top.Controls.Add(button_SelectExclusionRulesFile);
            panel_Top.Controls.Add(textBox_ExclusionRulesPath);
            panel_Top.Controls.Add(button_Run);
            panel_Top.Location = new System.Drawing.Point(7, 17);
            panel_Top.Margin = new System.Windows.Forms.Padding(4);
            panel_Top.Name = "panel_Top";
            panel_Top.Size = new System.Drawing.Size(918, 96);
            panel_Top.TabIndex = 4;
            //
            //label_Statistics
            //
            label_Statistics.Location = new System.Drawing.Point(650, 1);
            label_Statistics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_Statistics.Name = "label_Statistics";
            label_Statistics.Size = new System.Drawing.Size(264, 30);
            label_Statistics.TabIndex = 11;
            label_Statistics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //label_Tips
            //
            label_Tips.Location = new System.Drawing.Point(4, 1);
            label_Tips.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_Tips.Name = "label_Tips";
            label_Tips.Size = new System.Drawing.Size(638, 30);
            label_Tips.TabIndex = 10;
            label_Tips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //checkBox_LC4
            //
            checkBox_LC4.Enabled = false;
            checkBox_LC4.Location = new System.Drawing.Point(772, 61);
            checkBox_LC4.Margin = new System.Windows.Forms.Padding(4);
            checkBox_LC4.Name = "checkBox_LC4";
            checkBox_LC4.Size = new System.Drawing.Size(63, 30);
            checkBox_LC4.TabIndex = 7;
            checkBox_LC4.Text = "LC4";
            checkBox_LC4.UseVisualStyleBackColor = true;
            //
            //comboBox_SelectFunc
            //
            comboBox_SelectFunc.AccessibleRole = System.Windows.Forms.AccessibleRole.IpAddress;
            comboBox_SelectFunc.FormattingEnabled = true;
            comboBox_SelectFunc.Location = new System.Drawing.Point(124, 62);
            comboBox_SelectFunc.Margin = new System.Windows.Forms.Padding(4);
            comboBox_SelectFunc.Name = "comboBox_SelectFunc";
            comboBox_SelectFunc.Size = new System.Drawing.Size(641, 25);
            comboBox_SelectFunc.TabIndex = 7;
            comboBox_SelectFunc.SelectedIndexChanged += comboBox_SelectFunc_SelectedIndexChanged;
            //
            //label_SelectFunc
            //
            label_SelectFunc.Location = new System.Drawing.Point(4, 61);
            label_SelectFunc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_SelectFunc.Name = "label_SelectFunc";
            label_SelectFunc.Size = new System.Drawing.Size(117, 30);
            label_SelectFunc.TabIndex = 7;
            label_SelectFunc.Text = "选择功能";
            label_SelectFunc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //label_ExclusionRulesFile
            //
            label_ExclusionRulesFile.Location = new System.Drawing.Point(4, 31);
            label_ExclusionRulesFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_ExclusionRulesFile.Name = "label_ExclusionRulesFile";
            label_ExclusionRulesFile.Size = new System.Drawing.Size(117, 30);
            label_ExclusionRulesFile.TabIndex = 5;
            label_ExclusionRulesFile.Text = "排除规则文件";
            label_ExclusionRulesFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //button_SelectExclusionRulesFile
            //
            button_SelectExclusionRulesFile.Location = new System.Drawing.Point(772, 31);
            button_SelectExclusionRulesFile.Margin = new System.Windows.Forms.Padding(4);
            button_SelectExclusionRulesFile.Name = "button_SelectExclusionRulesFile";
            button_SelectExclusionRulesFile.Size = new System.Drawing.Size(63, 30);
            button_SelectExclusionRulesFile.TabIndex = 4;
            button_SelectExclusionRulesFile.Text = "Sclect";
            button_SelectExclusionRulesFile.UseVisualStyleBackColor = true;
            button_SelectExclusionRulesFile.Click += button_SelectExclusionRulesFile_Click;
            //
            //textBox_ExclusionRulesPath
            //
            textBox_ExclusionRulesPath.Location = new System.Drawing.Point(124, 31);
            textBox_ExclusionRulesPath.Margin = new System.Windows.Forms.Padding(4);
            textBox_ExclusionRulesPath.Name = "textBox_ExclusionRulesPath";
            textBox_ExclusionRulesPath.Size = new System.Drawing.Size(641, 23);
            textBox_ExclusionRulesPath.TabIndex = 3;
            //
            //button_Run
            //
            button_Run.Location = new System.Drawing.Point(835, 31);
            button_Run.Margin = new System.Windows.Forms.Padding(4);
            button_Run.Name = "button_Run";
            button_Run.Size = new System.Drawing.Size(79, 61);
            button_Run.TabIndex = 2;
            button_Run.Text = "执行";
            button_Run.UseVisualStyleBackColor = true;
            button_Run.Click += button_Run_Click;
            //
            //richTextBox_Code
            //
            richTextBox_Code.Location = new System.Drawing.Point(7, 122);
            richTextBox_Code.Margin = new System.Windows.Forms.Padding(4);
            richTextBox_Code.Name = "richTextBox_Code";
            richTextBox_Code.Size = new System.Drawing.Size(917, 610);
            richTextBox_Code.TabIndex = 6;
            richTextBox_Code.Text = "";
            //
            //Form1
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(933, 740);
            Controls.Add(richTextBox_Code);
            Controls.Add(panel_Top);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "Form1";
            Text = "代码混肴器V0.2（For Galaxy） By 蔚蓝星海";
            Load += Form1_Load;
            panel_Top.ResumeLayout(false);
            panel_Top.PerformLayout();
            ResumeLayout(false);
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

