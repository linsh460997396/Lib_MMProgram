# Lib_MMProgram
 MetalMaxSystem.FuncLib（MM_函数库）  
 个人用来写游戏逻辑、文件处理用的方法集合，暂内容不多，喜欢可下载
 
 1）Lib_MMCore是MM_函数库本体，生成后的dll方法可供其他C#、C++引擎调用
> >>系统事件监听（含游戏常用键鼠事件委托）  
> >>游戏常用数据表（哈希写的Object类型和其他常用类型，下次改良为用 dictionary 泛型 写个通用的~频繁读写时避免拆装箱及可以避免写一堆）  
> >>游戏常用主副循环（静态类）+周期触发器（实例类），能随意注册注销更换Awake、start、Update、End、Destroy事件的委托函数，可设定运行次数、周期间隔及前摇，随时打断触发器线程（执行End事件，未注册则跳过）  
> >>周期触发器的库内类名为TimerUpdate，它总是自动开一个线程去执行委托，其中Update阶段反复执行委托函数是由Timer类控制的，此时线程会等待Timer退出才会继续执行End、Destroy事件。周期触发器可new一堆实例去按设定间隔运行，参数填数字即运行次数（不填则无限循环）具体详TimerUpdate类。复制周期触发器可组织更多自定义事件，设计“单位死亡”、“倒计时结束”等事件中获取触发单位、触发倒计时，方便事后复活、控制倒计时UI（可让每次倒计时结束就重制为5分钟或安排该倒计时所属建筑去表演动画）。库内预制的事件委托类型可传递2个参数，第一个参数是事件发生的类本身，第二个参数可传递其他类的信息如鼠标当前位置；  

 2）FileMaster演示了库内方法调用，用文件操作相关函数去制作了"仓鼠文件处理程序"，可不下载
> >>批量修改、移动、删除文件（夹）及打印整个目录文件名（模拟Bat命令“DIR . /s/a/o:n C:\MyDir\ > C:\output.txt”）  
> >>修复了上一版工作目录带中文符号时不工作的BUG（正则匹配没加入（【】）：这些字符支持导致）；

 3）MMTest是空的控制台项目，用来进行一些测试，可不下载；
 
 4）MMWFM是空的Winform项目，用来进行一些测试，可不下载。
