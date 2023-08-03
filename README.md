# Lib_MMProgram
 MetalMaxSystem.FuncLib（MM_函数库）  
 个人用来写游戏、文件处理用的C#方法集合，方便组织逻辑，暂内容不多，喜欢可下载  
 Visual Studio 2019可单独运行测试，生成的Lib_MMCore.dll可供其他C#、C++项目引用
 
 1）Lib_MMCore是MM_函数库本体，内容包括但不限于：
> >>系统事件监听（含游戏常用键鼠事件委托）
> >>
> >>游戏常用数据表（哈希数据表写的Object类型和其他常用类型，下次更新的时候得改良用 dictionary 泛型 写个通用的~避免频繁读写时拆装箱及写一堆类型）及用它封装的互动功能（将数字、字符、对话框及控件UI、计时器、点、区域等任意类型写入自定义值、安排唯一句柄、当做个体去使用、放入类型小组去随机、挑选形成多个数据类型互动。互动功能：如你遗忘在Unit类写单位坐标字段，那么可用互动功能在Unit的"坐标"键区写自定义值，到时候读出来也是一样的；能设计频繁注册注销时的动作队列处理，你不必设计队列每个槽位，只要在函数用互动功能写这个函数名对应的键区正在工作；也可结合事件委托设计较复杂AI但维护非常简单，如让建筑知道自己的砌砖工作组工人在赶来的路上死了然后重新呼唤5范围内的闲置工人，可通过事件得到触发建筑，有了建筑可得到该建筑的工人（数据表键区"建筑标签"里取到工人句柄字符值，通过这个字符从互动函数里取得实例工人，然后读取到工人在何时何地被何种打死，抑或这工人不是被打死的是用鼠标拖走打断了工作，你只需在事件发生时在工人的自定义值中写入，建筑每次判断工人状态即可保证总是有1个工人来工作）
> >>
> >>游戏常用主副循环（静态类）+周期触发器（实例类），能随意注册注销更换Awake、start、Update、End、Destroy事件的委托函数，可设定运行次数、周期间隔及前摇，随时打断触发器线程（执行End事件，未注册则跳过）
> >>
> >>周期触发器（类名TimerUpdate），它总是自动开一个线程去执行委托，其中Update阶段反复执行委托函数是由Timer类控制的，此时线程会等待Timer退出才会继续执行End、Destroy事件。周期触发器可new一堆实例去按设定间隔运行，参数填数字即运行次数（不填则无限循环）具体详TimerUpdate类。复制周期触发器可组织更多自定义事件，设计“单位死亡”、“倒计时结束”等事件中获取触发单位、触发倒计时，方便事后复活、控制倒计时UI（可让每次倒计时结束就重制为5分钟或安排该倒计时所属建筑去表演动画）。库内预制的事件委托类型可传递2个参数，第一个参数是事件发生的类本身，第二个参数可传递其他类的信息如鼠标当前位置；

 2）FileMaster演示了库内方法调用，用文件操作相关函数去制作了"仓鼠文件处理程序"（可不下载），该工具目前情况介绍：
> >>批量修改、移动、删除文件（夹）及打印整个目录文件名（模拟Bat命令“DIR . /s/a/o:n C:\MyDir\ > C:\output.txt”）
> >>
> >>目前批量移动文件功能不完善，表现为发生路径重名会忽略动作，所以待设计自动给重名文件安排后缀随机字符或序号的功能（随机字符也可重名，将生成过的字符放入库内数据表功能-字符组，来判断是否已经使用过）
> >>
> >>修复了上一版工作目录带中文符号时不工作的BUG（正则匹配没加入（【】）：这些字符支持导致），后面可能还会遇到Bug，可自己改或等待工具更新；

 3）MMTest是空的控制台项目，用来进行一些测试（可不下载）；
 
 4）MMWFM是空的Winform项目，用来进行一些测试（可不下载）。
