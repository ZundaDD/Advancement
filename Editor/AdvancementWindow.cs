using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.Advancement
{
    [EditorWindowTitle(title = "Advancement Editor", icon = "d_UnityEditor.ConsoleWindow")]
    public partial class AdvancementWindow : EditorWindow
    {
        #region 静态内容
        private static readonly string styleSheetGUID = "b3ff1c1a8b23bd142b1288128b31a25b";

        private enum FileState { NoFile, ParseValid, ParseError }

        [MenuItem("Window/MikanLab/Advancement")]
        public static void ShowWindow()
        {
            GetWindow<AdvancementWindow>();
        }
        #endregion

        private VisualElement errorElements;
        private VisualElement validElements;
        private Label errorLabel;

        private TextAsset curAsset;
        private AdvancementCluster curCluster;
        private Advancement curSection;

        private void OnEnable()
        {
            minSize = new(100, 400);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(styleSheetGUID));
            
            var root = rootVisualElement;
            root.styleSheets.Add(styleSheet);
            root.AddToClassList("root_container");

            var textField = new ObjectField("JSON FILE:");
            textField.objectType = typeof(TextAsset);
            textField.RegisterValueChangedCallback(OnSelectionChanged);

            validElements = new();
            validElements.AddToClassList("section_view");
            errorElements = new();

            root.Add(textField);
            root.Add(validElements);
            root.Add(errorElements);
            
            ConstructValidUI();
            ConstructErrorUI();

            ReconstructUI(FileState.NoFile);
        }

        private void OnDisable() => SaveToFile();

        /// <summary>
        /// 选择文件改变
        /// </summary>
        /// <param name="evt"></param>
        private void OnSelectionChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            //先保存之前的文件
            SaveToFile();
            
            curAsset = evt.newValue as TextAsset;

            if (curAsset == null)
            {
                ReconstructUI(FileState.NoFile);
                return;
            }

            try
            {
                LoadFromAsset(curAsset);
                ReconstructUI(FileState.ParseValid);
            }
            catch (Exception e)
            {
                
                string errorMessage = $"<color=orange>Error parsing '{curAsset.name}'</color>:\t{e.Message}";
                curAsset = null;
                curCluster = null;
                curSection = null;
                ReconstructUI(FileState.ParseError, errorMessage);
            }
        }

        /// <summary>
        /// 从文件中加载数据
        /// </summary>
        /// <param name="asset">文件资源</param>
        /// <exception cref="Exception">解析失败</exception>
        private void LoadFromAsset(TextAsset asset)
        {
            //尝试解析为Cluster
            curCluster = JsonConvert.DeserializeObject<AdvancementCluster>(asset.text);

            //如果无效则抛出异常
            if (curCluster == null || curCluster.Clusters == null)
                throw new Exception("Invalid Json Structure!");

            if(curCluster.Clusters.Count == 0 )
                throw new Exception("Runined Cluster!");
        }

        /// <summary>
        /// 将数据保存到文件中
        /// </summary>
        private void SaveToFile()
        {
            if(curAsset != null && curCluster != null)
            {
                //先将已经修改的内容复制回去
                sectionView.SaveTo();

                var path = AssetDatabase.GetAssetPath(curAsset);
                if(path == string.Empty)
                {
                    Debug.LogError("File Deleted, Save Failed!!!");
                    return;
                }

                //序列化
                File.WriteAllText(path, JsonConvert.SerializeObject(curCluster, Formatting.Indented));
                AssetDatabase.Refresh();
            }

            curAsset = null;
            curCluster = null;
            curSection = null;
        }

        /// <summary>
        /// 重新构建所有UI
        /// </summary>
        /// <param name="state">文件状态</param>
        /// <param name="errorMessage">错误信息</param>
        private void ReconstructUI(FileState state, string errorMessage = "")
        {
            validElements.style.display = (state == FileState.ParseValid) ? DisplayStyle.Flex : DisplayStyle.None;
            errorElements.style.display = (state == FileState.ParseError) ? DisplayStyle.Flex : DisplayStyle.None;
            errorLabel.text = errorMessage;

            ReconstrcutSectionButtons();
        }
    }
}