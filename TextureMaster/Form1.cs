using MetalMaxSystem;

namespace TextureMaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartSlice();
        }

        void StartSlice()
        {
            //应用示范
            string folderPath = "C:/Users/linsh/Desktop/地图"; //填写要扫描的目录
            string savePathFrontStr01 = "C:/Users/linsh/Desktop/MapSP/"; //输出纹理集图片的目录前缀字符
            string savePathFrontStr02 = "C:/Users/linsh/Desktop/MapIndex/"; //输出纹理文本的目录前缀字符
            TextureAnalyzerNet.StartSliceTextureAndSetSpriteIDMultiMergerAsync(folderPath, "*.png", 0.9f, savePathFrontStr01, savePathFrontStr02, 10, 16, 16, 8); //目录下多个图片合批特征图
        }
    }
}
