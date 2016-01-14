# MyChat
模拟微信UI的简单游戏引擎，现在可以从文本文件导入剧本内容并以模拟微信的UI显示/交互。

# TODO
网页微信UI -> 手机微信UI
多剧本并行处理

# 更新日志

2016-01-14 剧本可以从外部文件导入；增加了WaitLine和Comment的逻辑。
2016-01-14之前：略

# 剧本文件格式

剧本文件目前限定为和exe本体处在同一文件夹内的"test.txt"。

其中每一行为一个剧本单元，主要的几种剧本单元如下：
其中第一个字符串为标识剧本单元类型的固定内容；<>代表一个字符串（目前其中还不能含有空格）；[]框住的内容代表可以不填。

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
台词内容
- askVar，askValue：
仅当编号为askVar的全局变量值为askValue时，该剧本单元才会起作用——此功能用以标记不同的选择支。下同。为了方便，建议把每个选择支赋上互异的askVar-askValue对，以方便剧本文件编写。

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

## WaitLine 死锁
	Wait <wait_index> <wait_value> [<askVar> <askValue>]
- 说明：
添加了多剧本并行处理之后，此类剧本单元可以用来维持各个分剧本的时间一致性。
- wait_index，wait_value：
当处理此剧本单元时，该剧本停止继续计算——即卡死。此后每一秒钟检查一次，仅当编号为wait_index的全局变量值为wait_value时才继续处理其后的剧本单元。

## Comment 注释
	// <comment>
- 说明：
以"// "（注意有空格）开头的行会被无视掉。可以用以给剧情添加注释。

# 剧本文件示例：
	// test.txt
	Sp 0.2 npc1 我问你，你他娘的就是老子的Master么？
	Ch 0.2 1 是 不是

	// 如果选择了 是： 后缀 1 0
	Sp 0.2 npc2 为您赴汤蹈火，在所不辞。 1 0

	// 如果选择了 不是： 后缀 1 1
	Sp 0.2 npc2 那可真是遗憾。 1 1
