# Lib_MMProgram
 MetalMaxSystem.FuncLib（MM_函数库）
 个人用来写游戏逻辑、文件处理用的方法集合，喜欢的可下载
 
 1）Lib_MMCore是MM_函数库本体，生成后就1个dll，里面的方法可供其他C#、C++引擎调用
> >>系统事件监听（含键鼠事件委托）
> >>游戏常用数据表（哈希写的Object类型和其他常用类型，下次改良为用 dictionary 泛型 写个通用的~频繁读写时避免拆装箱及可以避免写一堆）
 2）FileMaster演示了库内方法调用，用文件操作相关函数去制作了"仓鼠文件处理程序"，可不下载
> >>批量修改、移动、删除文件（夹）及打印整个目录文件名（模拟Bat命令“DIR . /s/a/o:n C:\MyDir\ > C:\output.txt”）
> >>修复了上一版工作目录带中文符号时不工作的BUG（正则匹配没加入（【】）：这些字符支持导致）
 3）MMTest是空的控制台项目，用来进行一些测试，可不下载
 4）MMWFM是空的Winform项目，用来进行一些测试，可不下载
