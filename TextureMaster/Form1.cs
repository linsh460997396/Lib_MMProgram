using MetalMaxSystem;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TextureMaster
{
    public partial class Form1 : Form
    {
        // 新增控件
        private GroupBox groupBox, groupBox2;
        private RadioButton radioTopLeft, radioPrintStyleRC;
        private RadioButton radioBottomLeft, radioPrintStyleID;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtInputPath.Text = dlg.FileName;
                }
            }
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            string tempImagePath;
            try
            {
                // 验证输入
                if (!File.Exists(txtInputPath.Text))
                {
                    MessageBox.Show("请选择有效的图片文件");
                    return;
                }

                if (!int.TryParse(txtRows.Text, out int rows) || rows <= 0 ||
                    !int.TryParse(txtColumns.Text, out int columns) || columns <= 0)
                {
                    MessageBox.Show("请输入有效的行列数");
                    return;
                }

                // 准备输出目录
                string outputDir = Path.Combine(Path.GetDirectoryName(txtInputPath.Text), Path.GetFileNameWithoutExtension(txtInputPath.Text) + "_split");

                Directory.CreateDirectory(outputDir);

                bool startFromBottom = radioBottomLeft.Checked;
                bool printID = radioPrintStyleID.Checked;

                // 修改后的循环逻辑
                var rowIndices = startFromBottom
                    ? Enumerable.Range(0, rows).Reverse()
                    : Enumerable.Range(0, rows);


                // 处理图像
                using (Bitmap original = new Bitmap(txtInputPath.Text))
                {
                    int chunkWidth = original.Width / columns;
                    int chunkHeight = original.Height / rows;

                    foreach (int y in rowIndices)
                    {
                        //MMCore.Tell("y: " + y + " rows: " + rows);
                        for (int x = 0; x < columns; x++)
                        {
                            // 新增行号计算
                            int currentRow = startFromBottom ? (rows - y) : (y + 1);

                            // 计算实际分块尺寸
                            int actualWidth = (x == columns - 1) ?
                                original.Width - chunkWidth * (columns - 1) : chunkWidth;
                            int actualHeight = (y == rows - 1) ?
                                original.Height - chunkHeight * (rows - 1) : chunkHeight;

                            Rectangle sourceRect = new Rectangle(
                                x * chunkWidth,
                                y * chunkHeight,
                                actualWidth,
                                actualHeight);

                            // 创建分块
                            using (Bitmap chunk = new Bitmap(actualWidth, actualHeight))
                            using (Graphics g = Graphics.FromImage(chunk))
                            {
                                g.DrawImage(original, new Rectangle(0, 0, actualWidth, actualHeight),
                                    sourceRect, GraphicsUnit.Pixel);

                                // 保存文件
                                string extension = Path.GetExtension(txtInputPath.Text);
                                ImageFormat format = GetImageFormat(extension);

                                if (printID == true)
                                {
                                    tempImagePath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(txtInputPath.Text)+$"_{(currentRow - 1) * columns + x + 1}{extension}");
                                }
                                else
                                {
                                    tempImagePath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(txtInputPath.Text)+$"_{currentRow}_{x + 1}{extension}");
                                }
                                chunk.Save(tempImagePath, format);
                            }
                        }
                    }
                }

                MessageBox.Show($"图片分割完成！保存至：\n{outputDir}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}");
            }
        }

        private ImageFormat GetImageFormat(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Png;
            }
        }

        //void StartSlice()
        //{
        //    //应用示范
        //    string folderPath = "C:/Users/linsh/Desktop/地图"; //填写要扫描的目录
        //    string savePathFrontStr01 = "C:/Users/linsh/Desktop/MapSP/"; //输出纹理集图片的目录前缀字符
        //    string savePathFrontStr02 = "C:/Users/linsh/Desktop/MapIndex/"; //输出纹理文本的目录前缀字符
        //    TextureAnalyzerNet.StartSliceTextureAndSetSpriteIDMultiMergerAsync(folderPath, "*.png", 0.9f, savePathFrontStr01, savePathFrontStr02, 10, 16, 16, 8); //目录下多个图片合批特征图
        //}
    }
}
