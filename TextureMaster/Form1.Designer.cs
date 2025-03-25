namespace TextureMaster
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(800, 450);
            //this.Text = "Form1";

            this.txtInputPath = new TextBox();
            this.btnBrowse = new Button();
            this.label1 = new Label();
            this.txtRows = new TextBox();
            this.label2 = new Label();
            this.txtColumns = new TextBox();
            this.btnSplit = new Button();
            this.SuspendLayout();

            // txtInputPath
            this.txtInputPath.Location = new Point(12, 12);
            this.txtInputPath.Size = new Size(260, 20);

            // btnBrowse
            this.btnBrowse.Location = new Point(278, 10);
            this.btnBrowse.Text = "浏览";
            this.btnBrowse.Click += new EventHandler(this.btnBrowse_Click);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 45);
            this.label1.Text = "行数:";

            // txtRows
            this.txtRows.Location = new Point(50, 42);
            this.txtRows.Size = new Size(50, 20);

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new Point(120, 45);
            this.label2.Text = "列数:";

            // txtColumns
            this.txtColumns.Location = new Point(160, 42);
            this.txtColumns.Size = new Size(50, 20);

            // btnSplit
            this.btnSplit.Location = new Point(12, 75);
            this.btnSplit.Size = new Size(316, 30);
            this.btnSplit.Text = "开始分割";
            this.btnSplit.Click += new EventHandler(this.btnSplit_Click);

            // Form1
            this.ClientSize = new Size(340, 120);
            this.Controls.AddRange(new Control[] {
                this.txtInputPath, this.btnBrowse,
                this.label1, this.txtRows,
                this.label2, this.txtColumns,
                this.btnSplit
            });
            this.Text = "图片分割器";
            this.ResumeLayout(false);
            this.PerformLayout();

            // 新增控件初始化
            this.groupBox = new GroupBox();
            this.radioTopLeft = new RadioButton();
            this.radioBottomLeft = new RadioButton();

            // groupBox
            this.groupBox.SuspendLayout();
            this.groupBox.Location = new Point(220, 40);
            this.groupBox.Size = new Size(110, 65);
            this.groupBox.Text = "起始位置";

            // radioTopLeft
            this.radioTopLeft.AutoSize = true;
            this.radioTopLeft.Location = new Point(10, 20);
            this.radioTopLeft.Text = "左上";
            this.radioTopLeft.Checked = true;

            // radioBottomLeft
            this.radioBottomLeft.AutoSize = true;
            this.radioBottomLeft.Location = new Point(10, 40);
            this.radioBottomLeft.Text = "左下";

            this.groupBox.Controls.Add(this.radioTopLeft);
            this.groupBox.Controls.Add(this.radioBottomLeft);
            this.groupBox.ResumeLayout(false);

            this.groupBox2 = new GroupBox();
            this.radioPrintStyleRC = new RadioButton();
            this.radioPrintStyleID = new RadioButton();

            // groupBox2
            this.groupBox2.SuspendLayout();
            this.groupBox2.Location = new Point(220, 105);
            this.groupBox2.Size = new Size(110, 65);
            this.groupBox2.Text = "命名方式";

            // radioPrintStyleRC
            this.radioPrintStyleRC.AutoSize = true;
            this.radioPrintStyleRC.Location = new Point(10, 20);
            this.radioPrintStyleRC.Text = "行列";
            this.radioPrintStyleRC.Checked = true;

            // radioPrintStyleID
            this.radioPrintStyleID.AutoSize = true;
            this.radioPrintStyleID.Location = new Point(10, 40);
            this.radioPrintStyleID.Text = "编号";

            this.groupBox2.Controls.Add(this.radioPrintStyleRC);
            this.groupBox2.Controls.Add(this.radioPrintStyleID);
            this.groupBox2.ResumeLayout(false);

            // 调整原有按钮位置
            this.btnSplit.Location = new Point(12, 165);

            // 更新窗体尺寸
            this.ClientSize = new Size(340, 205);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.groupBox2);
        }

        private TextBox txtInputPath;
        private Button btnBrowse;
        private Label label1;
        private TextBox txtRows;
        private Label label2;
        private TextBox txtColumns;
        private Button btnSplit;

        #endregion
    }
}
