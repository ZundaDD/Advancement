using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MikanLab.Advancement
{
    /// <summary>
    /// 监控器，根据传入的状态控制进度的实际位置
    /// </summary>
    public class AdvancementMonitor
    {
        private AdvancementCluster cluster;
        private AdvancementSave save;

        public AdvancementMonitor(AdvancementCluster cluster, AdvancementSave save)
        {
            this.cluster = cluster;
            this.save = save ?? new AdvancementSave();
        }


        /// <summary>
        /// 更新进度,Monitor根据传入的条件更新进度的位置，返回应该做什么
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>指令列表</returns>
        public List<T> UpdateAdvancement<T>() where T : System.Enum
        {
            if (typeof(T) != cluster.ActionEnumName) return new();
            
            var actions = UpdateAdvancement();

            return actions.Select(
                intValue => (T)Enum.ToObject(cluster.ActionEnumName, intValue)
                ).ToList();
        }

        /// <summary>
        /// 更新进度,Monitor根据传入的条件更新进度的位置，返回应该做什么
        /// </summary>
        /// <returns>指令列表</returns>
        public List<int> UpdateAdvancement()
        {
            return new();
        } 
    }
}