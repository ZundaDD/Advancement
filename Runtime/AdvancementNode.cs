using System.Collections.Generic;
using System;
using UnityEngine;

namespace MikanLab.Advancement
{
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
