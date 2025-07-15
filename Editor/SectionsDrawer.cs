using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikanLab.Advancement
{
    /// <summary>
    /// 绘制整体UI的部分
    /// </summary>
    public partial class AdvancementWindow : EditorWindow
    {
        private ToggleButtonGroup sections;
        private AdvancementView sectionView;

        #region 初始化
        /// <summary>
        /// 构建有效UI部分
        /// </summary>
        private void ConstructValidUI()
        {
            var toolbar = new VisualElement();
            toolbar.AddToClassList("tool_bar");
            validElements.Add(toolbar);

            //Section选择部分
            var scrollView = new ScrollView(ScrollViewMode.Horizontal);
            scrollView.AddToClassList("button_scrollview");
            toolbar.Add(scrollView);

            sections = new();
            sections.AddToClassList("button_group");
            scrollView.Add(sections);

            //Section绘制部分
            sectionView = new();
            sectionView.AddToClassList("section_view");
            validElements.Add(sectionView);
        }

        /// <summary>
        /// 构建错误UI部分
        /// </summary>
        private void ConstructErrorUI()
        {
            errorLabel = new();

            var box = new Box();
            box.Add(errorLabel);
            box.AddToClassList("error_box");

            errorElements.Add(box);

        }
        #endregion

        /// <summary>
        /// 重新构建选定分区的视图
        /// </summary>
        /// <param name="section">选定分区</param>
        private void ReconstructView(Advancement section)
        {
            sectionView.SaveTo();            

            if (curSection == section) return;
            curSection = section;

            Debug.Log("<color=cyan>当前分区：</color>" + section.SectionName);
            sectionView.LinkTo(curSection);
        }

        #region 整体UI
        /// <summary>
        /// 重新构建Button UI
        /// </summary>
        private void ReconstrcutSectionButtons()
        {
            //清除掉旧的Button
            sections.Clear();

            //清理之后再退出
            if (curAsset == null || curCluster == null) return;

            foreach(var section in curCluster.Clusters)
            {
                sections.Add(GetButton(section));
            }

            //构建默认打开的分区的内容
            SelectFirstButton();
        }

        /// <summary>
        /// 构建右键菜单
        /// </summary>
        /// <param name="e"></param>
        private void ConstructContextMenu(ContextualMenuPopulateEvent e)
        {
            var clickedButton = e.target as Button;

            if (clickedButton == null) { return; }

            e.menu.ClearItems();
            e.menu.AppendAction("Delete Section", (action) => DeleteSection(clickedButton), (action) =>
            {
                return curCluster.Clusters.Count > 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
            });
            e.menu.AppendAction("Add New Section", (action) => AddNewSection());
            e.menu.AppendAction("Rename Section", (action) => StartRename(clickedButton));
        }

        /// <summary>
        /// 根据分区创建Button
        /// </summary>
        /// <param name="section"></param>
        private Button GetButton(Advancement section)
        {
            var button = new Button();
            button.text = section.SectionName;
            button.userData = section;
            button.AddToClassList("section_button");

            button.AddManipulator(new ContextualMenuManipulator((evt) => { }));
            button.RegisterCallback<ContextualMenuPopulateEvent>(ConstructContextMenu);
            button.RegisterCallback<ClickEvent>((evt) =>
            {
                var section = button.userData as Advancement;
                if (section == null) return;

                //当点击按钮时，重构分区View
                ReconstructView(section);
            });
            return button;
        }
        #endregion
     

        #region 分区整体操作
        /// <summary>
        /// 删除按钮对应的分区
        /// </summary>
        /// <param name="btn"></param>
        private void DeleteSection(Button btn)
        {
            var section = btn.userData as Advancement;
            if (section == null) return;

            if (curCluster.Clusters.Count <= 1) return;

            curCluster.Clusters.Remove(section);

            btn.RemoveFromHierarchy();

            if(curSection == section)
            {
                SelectFirstButton();
            }
        }

        private void SelectFirstButton()
        {
            var firstButton = sections[0] as Button;
            
            if (firstButton != null)
            {
                using (var evt = ClickEvent.GetPooled())
                {
                    evt.target = firstButton;
                    firstButton.SendEvent(evt);
                }
            }
        }

        /// <summary>
        /// 添加新的分区
        /// </summary>
        private void AddNewSection()
        {
            //先写数据
            curCluster.Clusters.Add(new Advancement() { SectionName = "New Section", Nodes = new()});

            //再创建UI
            sections.Add(GetButton(curCluster.Clusters[^1]));
        }

        /// <summary>
        /// 给分区重命名
        /// </summary>
        /// <param name="btn"></param>
        private void StartRename(Button btn)
        {
            var section = btn.userData as Advancement;
            if (section == null) return;

            btn.text = "";

            var textField = new TextField();
            textField.AddToClassList("rename-textfield");
            
            textField.value = section.SectionName;

            btn.Add(textField);

            textField.Focus();
            textField.SelectAll();

            System.Action<bool> endRenameAction = (saveChanges) => {
                if (saveChanges)
                {
                    section.SectionName = textField.value;
                }
                btn.text = section.SectionName;

                textField.RemoveFromHierarchy();
            };

            //注册文本框事件
            textField.RegisterCallback<FocusOutEvent>(evt => endRenameAction(true));
            textField.RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    endRenameAction(true);
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    endRenameAction(false);
                }
            });
        }
        #endregion

    }
}
