# MyChat
模拟微信UI的简单游戏引擎，现在可以从文本文件导入剧本内容并以模拟微信的UI显示/交互。

# 编程方面的备忘

人物头像需要以Image的形式挂进起好名字的Image对象中，并且在Assets\Script\GameManager.cs的ImageManager类中登记注册，才能被剧本文件索引调用。

其他好像没什么了……

# 剧本文件格式

“剧本文件信息文件”和所有的剧本都要放在和游戏本体相同路径下的文件夹PlotFile中。这些文件必须都以UTF-8格式编码，因此Windows环境下的用户可能需要多加注意。此外，由于神秘的原因，建议剧本**内容部分（而非所有部分！）**都使用汉字或全角字符，并且不能含有空格。

其中，“剧本文件信息文件”（文件名固定为 plot_name_info.ini）不可以省略：其中每一行为一个剧本的名字。除了空行之外，不可以有多余的行。

剧本文件的命名为 剧本名字.txt 。其中每一行为一个剧本单元。

下面介绍各种剧本单元：其中第一个字符串为标识剧本单元类型的固定内容；<>代表一个字符串（目前其中还不能含有空格）；[]框住的内容代表可以不填。目前不可以存在多余的空格。此外，所有的tab（\t）都会被忽略；可以用来缩进以改善观感。

## SpeechLine 台词
	Sp <timeDelay> <person> <content> [<askVar> <askValue>]
- 说明：
模拟一句台词。非玩家的台词显示在左边，玩家的台词显示在右边。

只要把命名好名字的头像图片示例丢进场景中，就可以自动加载对应的头像。

过长的台词会自动换行。
- timeDelay：
该台词距上一句台词的时间间隔。下同。
- person：
该台词的说话者，You代表玩家角度，其他值代表非玩家角度。
- content：
台词内容，不可以含有空格；建议全部采用全角字符。下同。
- askVar，askValue：
仅当编号为askVar的全局变量值为askValue时，该剧本单元才会起作用——此功能用以标记不同的选择支。下同。

为了方便，建议把每个选择支赋上互异的askVar-askValue对，以方便剧本文件编写。

## SystemLine 系统消息
	Sys <timeDelay> <content> [<askVar> <askValue>]
- 说明：
模拟微信系统消息。
- content：
系统消息内容

## ChoiceLine 选项
	Ch <timeDelay> <aff_index> <choice_0> [<choice_1> ... <choice_n>] [<askVar> <askValue>]
- 说明：
选项会以“打字框”中的选项形式出现。用户可以选择其中的一个发出（即自动发出一个person值为"You"的SpeechLine）；选择之前游戏处于全局暂停状态。
- aff_index：
该选项影响的全局变量编号。
- choice_k：
编号为k（从0开始）的选项的文本内容。选择编号为k的选项会使得编号为aff_index的全局变量的值变为k。

需要注意的是，由于作者的实现十分丑陋，目前禁止出现以数字开头的选择文本（会被错认为是askVar）。

此外，请不要以此方式修改编号为0的全局变量（0，0为askVar，askValue的缺省值）。

## WaitLine 死锁
	Wait <wait_index> <wait_value> [<askVar> <askValue>]
- 说明：
添加了多剧本并行处理之后，此类剧本单元可以用来维持各个分剧本的时间一致性。
- wait_index，wait_value：
当处理此剧本单元时，该剧本停止继续计算——即卡死。此后每一秒钟检查一次，仅当编号为wait_index的全局变量值为wait_value时才继续处理其后的剧本单元。

## SetLine 标记
	Set <set_index> <set_value> [<askVar> <askValue>]
- 说明：
自动设置全局变量的值，主要用来标记该段剧本的发展状况，例如某句话是否出现、某个分歧点是否已经经过。
- set_index, set_value：
该剧本单元处理时，将会把编号为set_index的全局变量的值赋为set_value。同样地，请不要以此方式修改编号为0的全局变量。

## Comment 注释
	// <comment>
- 说明：
以"// "（注意有空格）开头的行会被无视掉。可以用以给剧情添加注释。
