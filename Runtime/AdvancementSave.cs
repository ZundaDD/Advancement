using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using UnityEngine;

namespace MikanLab.Advancement
{
    /// <summary>
    /// 进度存档，用于保存和加载进度数据
    /// </summary>
    [Serializable]
    public class AdvancementSave
    {
        [Serializable]
        public class SectionSave
        {
            public string SectionName;  // 分区名称
            public List<int> Pos;       // 分区位置
        }

        [JsonProperty, SerializeField] private List<SectionSave> SectionSaves;
        [JsonIgnore] public Dictionary<string, List<int>> SectionPos;

        [OnSerializing]
        internal void OnBeforeSerialize(StreamingContext context)
        {
            foreach (var kvp in SectionPos)
            {
                SectionSaves.Add(new SectionSave
                {
                    SectionName = kvp.Key,
                    Pos = kvp.Value
                });
            }
        }

        [OnDeserialized]
        internal void OnAfterDeserialize(StreamingContext context)
        {
            foreach (var item in SectionSaves)
            {
                SectionPos.Add(item.SectionName, item.Pos);
            }
            SectionSaves.Clear();
        }
    }
}