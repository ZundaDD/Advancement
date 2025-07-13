using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.Advancement
{
    [EditorWindowTitle(title = "Advancement Editor", icon = "d_UnityEditor.ConsoleWindow")]
    public class AdvancementWindow : EditorWindow
    {
        private enum FileState { NoFile, ParseValid, ParseError }

        [MenuItem("Window/MikanLab/Advancement")]
        public static void ShowWindow()
        {
            GetWindow<AdvancementWindow>();
        }
        
        private VisualElement errorElements;
        private VisualElement validElements;
        private Label errorBox;

        public void OnEnable()
        {
            minSize = new(800, 600);

            var root = rootVisualElement;

            var textField = new ObjectField("进度文件");
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

        private void ConstructValidUI()
        {

        }

        private void ConstructErrorUI()
        {
            errorBox = new();
            
            errorElements.Add(errorBox);

        }

        private void OnSelectionChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            TextAsset asset = evt.newValue as TextAsset;
            
            if (asset == null)
            {
                ReconstructUI(FileState.NoFile);
                return;
            }

            try
            {
                JsonConvert.DeserializeObject<Advancement>(asset.text);
                ReconstructUI(FileState.ParseValid);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error parsing '{asset.name}':\t{e.Message}";
                ReconstructUI(FileState.ParseError, errorMessage);
            }
        }

        private void ReconstructUI(FileState state, string errorMessage = "")
        {
            validElements.style.display = (state == FileState.ParseValid) ? DisplayStyle.Flex : DisplayStyle.None;
            errorElements.style.display = (state == FileState.ParseError) ? DisplayStyle.Flex : DisplayStyle.None;
            errorBox.text = errorMessage;
        }
    }
}