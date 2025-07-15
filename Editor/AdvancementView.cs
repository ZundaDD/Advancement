using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.Advancement
{
    /// <summary>
    /// 进度视图，内恰的保存与修改
    /// </summary>
    public class AdvancementView : GraphView
    {
        private Advancement advancement;
        private Proxy advancementObject;
        private SerializedObject advancementObjectSerialized;

        #region 工具类
        private class AdvancementGridBackground : GridBackground { }
        
        private class Proxy : ScriptableObject
        {
            [SerializeField] public Advancement adv;
        }
        #endregion
        
        public AdvancementView() : base()
        {
            this.AddManipulator(new ContentDragger());
            SetupZoom(0.5f, ContentZoomer.DefaultMaxScale);
            Insert(0, new AdvancementGridBackground());
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (startAnchor.node == port.node ||
                    startAnchor.direction == port.direction ||
                    startAnchor.portType != port.portType)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }

        /// <summary>
        /// 连接到一个分区，对其副本展开编辑
        /// </summary>
        /// <param name="advancement"></param>
        public void LinkTo(Advancement advancement)
        {
            this.advancement = advancement;

            this.advancementObject = ScriptableObject.CreateInstance<Proxy>();
            this.advancementObject.hideFlags = HideFlags.HideAndDontSave;

            var temp = JsonConvert.SerializeObject(advancement);
            this.advancementObject.adv = JsonConvert.DeserializeObject<Advancement>(temp);

            this.advancementObjectSerialized = new(this.advancementObject);

            ConstructView();
        }

        /// <summary>
        /// 将编辑后的结果设置回
        /// </summary>
        public void SaveTo(bool clear = true)
        {
            if (this.advancementObject == null) return;
            
            advancementObjectSerialized.ApplyModifiedProperties();
            
            this.advancement.Nodes.Clear();
            var temp = JsonConvert.SerializeObject(this.advancementObject.adv);
            JsonConvert.PopulateObject(temp, advancement);

            if (clear) ResetToDefault();
        }

        /// <summary>
        /// 将视图恢复至一开始的样子
        /// </summary>
        private void ResetToDefault()
        {
            if (this.advancementObject != null)
            {
                ScriptableObject.DestroyImmediate(this.advancementObject);
                this.advancementObject = null;
            }

            this.advancementObjectSerialized = null;
            this.advancement = null;
           
            var elementsToRemove = graphElements.ToList();
            DeleteElements(elementsToRemove);

            viewTransform.position = Vector3.zero;
            viewTransform.scale = Vector3.one;
        }

        /// <summary>
        /// 构建初始视图
        /// </summary>
        private void ConstructView()
        {
            var property = advancementObjectSerialized.FindProperty("adv").FindPropertyRelative("Nodes");

            for (int i = 0;i < advancement.Nodes.Count;i++)
            {
                var node = advancement.Nodes[i];
                var graphNode = new AdvancementGraphNode(node, property.GetArrayElementAtIndex(i));

                AddElement(graphNode);
                graphNode.DrawNode();
                graphNode.title = i.ToString();
                graphNode.SetPosition(new Rect(new(0f, 100 * i), new Vector2(100f, 200f)));
            }
        }
    }
}