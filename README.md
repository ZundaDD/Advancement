# Advancement

这是一个Unity内置的进度系统，可用于对话剧情，成就系统等树状流程模拟，取名来源是Minecraft的进度系统

## 如何使用

在编辑器中，使用Assets/Create/MikanLab/Advancement/AdvancementCluster或者Advancement来创建资源文件，接着使用Window/MikanLab/Advancement窗口编辑资源文件。

在运行时，需要加载对应的Advancement和AdvancementSave（如果没有就创建）文件，通过二者生成AdvancementMonitor。通过AdvancementMonitor的UpdateAdvancement来更新进度并获取指令，负责对接的对象负责实现指令执行和条件判断的逻辑。在需要的时候

## 其他

如果您在使用过程中发现问题或者觉得有可以改进的地方，可以在Issues板块中提出。</br>

## 引用资源

本项目引用了NewtonSoft.Json的官方仓库版本，可以从[发布地址](https://github.com/JamesNK/Newtonsoft.Json/releases/tag/13.0.3)中获取，请确保在项目中存在Newtonsoft.Json.dll文件即可。同时对于AdvancementSave，请使用NewtonSoft.Json来进行序列化和反序列化，包内置了相关代码，也可以根据需求修改。

本项目引用的一切图标均来源于[FlatIcon](https://www.flaticon.com/ "免费图标素材")</br>
