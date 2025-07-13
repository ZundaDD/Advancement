using System;
using System.Collections.Generic;
using UnityEngine;

namespace MikanLab.Advancement
{
    /// <summary>
    /// Advancement 是一个进度系统，用于给出游戏中的事件或时间节点。
    /// 实际进度的控制由 Monitor 控制。一个Advancement对象为一个分区，
    /// 单个分区可以被解析，也可以和多个分区一起被解析。
    /// </summary>
    [Serializable]
    public class Advancement
    {
        public List<AdvancementNode> Nodes;     //分区节点
        public string SectionName;              //分区名称
    }

    /// <summary>
    /// 进度节点，最基本的单位
    /// </summary>
    [Serializable]
    public class AdvancementNode
    {
        public List<int> SonNodes;                  //节点下游节点
        public AdvancementAction Actions;     //节点达成行为
        public AdvancementCondition Triggers;       //节点达成条件
    }

    [Serializable]
    public class AdvancementCondition
    {

    }

    [Serializable]
    public class AdvancementAction
    {

    }
}