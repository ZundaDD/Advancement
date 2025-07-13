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
    public class AdvancementWindow : EditorWindow
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

        private bool isCluster = false;
        private TextAsset curAsset;
        private AdvancementCluster curCluster;


        private void OnEnable()
        {
            minSize = new(800, 600);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(styleSheetGUID));
            
            var root = rootVisualElement;
            root.styleSheets.Add(styleSheet);

            var textField = new ObjectField("JSON FILE:");
            textField.objectType = typeof(TextAsset);
            textField.RegisterValueChangedCallback(OnSelectionChanged);

            validElements = new();
            errorElements = new();

            root.Add(textField);
            root.Add(validElements);
            root.Add(errorElements);

            ConstructValidUI();
            ConstructErrorUI();

            ReconstructUI(FileState.NoFile);
        }

        
        private void OnDisable()
        {
            SaveToFile();   
        }


        private void ConstructValidUI()
        {

        }

        private void ConstructErrorUI()
        {
            errorLabel = new();
            var box = new Box();
            box.Add(errorLabel);
            box.AddToClassList("error_box");
            errorElements.Add(box);

        }

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
                ReconstructUI(FileState.ParseError, errorMessage);
                curAsset = null;
                curCluster = null;
            }
        }

        //从文件中加载数据
        private void LoadFromAsset(TextAsset asset)
        {
            //首先尝试解析为Advancement
            var adv = JsonConvert.DeserializeObject<Advancement>(asset.text);

            //如果解析成功且内容有效
            if (adv.SectionName != string.Empty && adv.Nodes != null)
            {
                curCluster = new();
                curCluster.Clusters = new();
                curCluster.Clusters.Add(adv);
                isCluster = false;
                
                return;
            }

            //尝试解析为Cluster
            curCluster = JsonConvert.DeserializeObject<AdvancementCluster>(asset.text);
            isCluster = true;

            //如果无效则抛出异常
            if (curCluster == null || curCluster.Clusters == null)
                throw new Exception("Invalid Json Structure!");
        }

        //将数据保存到文件中
        private void SaveToFile()
        {
            if(curAsset != null && curCluster != null)
            {
                var path = AssetDatabase.GetAssetPath(curAsset);
                if(path == string.Empty)
                {
                    Debug.LogError("File Deleted, Save Failed!!!");
                    return;
                }

                //按集群序列化
                if(isCluster)
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(curCluster, Formatting.Indented));
                }
                //按单体序列化
                else
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(curCluster.Clusters[0], Formatting.Indented));
                }
                AssetDatabase.Refresh();
            }

            curAsset = null;
            curCluster = null;
        }


        private void ReconstructUI(FileState state, string errorMessage = "")
        {
            validElements.style.display = (state == FileState.ParseValid) ? DisplayStyle.Flex : DisplayStyle.None;
            errorElements.style.display = (state == FileState.ParseError) ? DisplayStyle.Flex : DisplayStyle.None;
            errorLabel.text = errorMessage;
        }
    }
}