using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Analytics.IAnalytic;

namespace MikanLab.Advancement
{
    /// <summary>
    /// 进度节点，与Runtime中一一对应
    /// </summary>
    public class AdvancementGraphNode : Node
    {
        private SerializedProperty serializedData;
        private AdvancementNode data;

        public AdvancementGraphNode(AdvancementNode data, SerializedProperty serializedData)
        {
            this.data = data;
            this.serializedData = serializedData;
            //if (!data.OnGraphData.Deleteable) capabilities -= Capabilities.Deletable;

            var parentPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Advancement));
            parentPort.portName = "Parent";
            inputContainer.Add(parentPort);
            var sonsPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Advancement));
            sonsPort.portName = "Sons";
            outputContainer.Add(sonsPort);

            //DrawNode();
        }


        /// <summary>
        /// 绘制节点
        /// </summary>
        public void DrawNode()
        {
            //extensionContainer.style.backgroundColor = new Color(0x7a / 256f, 0x9f / 256f, 0xaa / 256f);

            PropertyField description = new();
            description.BindProperty(serializedData.FindPropertyRelative("Description"));
            description.focusable = true;
            
            description.AddToClassList("property_field");
            extensionContainer.Add(description);
            description.BringToFront();

            PropertyField single = new();
            single.BindProperty(serializedData.FindPropertyRelative("Single"));
            single.focusable = true;
            single.AddToClassList("property_field");
            extensionContainer.Add(single);

            RefreshExpandedState();
        }


    }
}