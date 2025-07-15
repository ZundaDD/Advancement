using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    /// Advancement集群，包含多个分区，分区之间允许相互引用
    /// 可以设置从一个分区的路到另一个分区的路，例如用于剧情分歧
    /// </summary>
    [Serializable]
    public class AdvancementCluster
    {
        [JsonConverter(typeof(TypeJsonConverter))]
        public Type ActionEnumName;
        public List<Advancement> Clusters;

        public class TypeJsonConverter : JsonConverter<Type>
        {
            public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
            {
                writer.WriteValue(value?.AssemblyQualifiedName);
            }

            public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string typeName = (string)reader.Value;
                return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName);
            }
        }
    }

}