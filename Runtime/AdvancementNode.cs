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
        public string Description;                  //节点描述
        public bool Single;                         //节点下游是否只能走单路
        public List<int> SonNodes;                  //节点下游节点
        public AdvancementAction Actions;           //节点达成行为
        public AdvancementCondition Triggers;       //节点达成条件
    }

    [Serializable]
    public class AdvancementCondition
    {

    }

    /// <summary>
    /// 进度行为，节点达成后执行的行为，可以是继续剧情，可以是给予奖励
    /// </summary>
    [Serializable]
    public class AdvancementAction
    {

    }
}
