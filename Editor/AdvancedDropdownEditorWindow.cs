using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Platinio.AdvancedDropdown
{
    public class AdvancedDropdownEditorWindow : EditorWindow 
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] protected VisualTreeAsset listElementTreeAsset;

        private Dictionary<int, Sprite> iconCache = new();
        private List<dynamic> dropdownElements;
        
        private readonly Color selectedColor = new (66.0f / 255.0f, 135.0f / 255.0f, 245.0f / 255.0f);
        private readonly Color normalColor = new (56.0f / 255.0f, 56.0f / 255.0f, 56.0f / 255.0f);

        private static readonly float MaxWindowHeight = 500.0f;
        private static readonly float DropdownItemHeight = 60.0f;
        private static readonly float WindowWidth = 350.0f;
        private static Func<Vector2> getCurrentMousePositionFunc;

        private void OnDestroy()
        {
            foreach (var dropdownElement in dropdownElements)
            {
                dropdownElement.OnDestroy();
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement visualTreeClone = visualTreeAsset.Instantiate();
            root.Add(visualTreeClone);
        }

        public static void ShowDropdown<T>(List<DropdownItem<T>> elements, Action<T> onDropdownSelectionChanged)
        {
            var dropdown = CreateInstance<AdvancedDropdownEditorWindow>();
            
            dropdown.ShowAsDropDown(new Rect(dropdown.GetCurrentMousePosition(), Vector2.zero), dropdown.CalculateWindowHeight(elements.Count));
            
            dropdown.dropdownElements = new List<dynamic>();
            
            foreach (var element in elements)
            {
                dropdown.dropdownElements.Add(element);
            }

            dropdown.CreateDatabaseListGUI(dropdown.rootVisualElement, elements, onDropdownSelectionChanged);
        }

        private Vector2 CalculateWindowHeight(int elementCount)
        {
            float height = DropdownItemHeight * elementCount;
            float preferredHeight = height > MaxWindowHeight ? MaxWindowHeight : height;
            return new Vector2(WindowWidth, preferredHeight);
        }

        private Vector2 GetCurrentMousePosition()
        {
            if (getCurrentMousePositionFunc == null)
            {
                var currentMousePositionMethod = typeof(Editor).GetMethod("GetCurrentMousePosition", BindingFlags.NonPublic | BindingFlags.Static);
                Assert.IsNotNull(currentMousePositionMethod);
                getCurrentMousePositionFunc = (Func<Vector2>) Delegate.CreateDelegate(typeof(Func<Vector2>), currentMousePositionMethod);
            }

            return getCurrentMousePositionFunc();
        }

        private VisualElement Root;
        
        protected void CreateDatabaseListGUI<T>(VisualElement root, List<DropdownItem<T>> elements, Action<T> onDropdownSelectionChanged)
        {
            Root = root;
            var listView = root.Q("ItemListView") as ListView;
            listView.Clear();
            listView.makeItem = MakeItem;
            listView.bindItem = BindEntryItem;
            listView.itemsSource = elements;
            listView.selectionType = SelectionType.Single;
            listView.selectionChanged += delegate(IEnumerable<object> objects)
            {
                var dropdownItem = objects.FirstOrDefault() as DropdownItem<T>;
                onDropdownSelectionChanged.Invoke(dropdownItem.Item);
                Close();
            };
        }

        private void BindEntryItem(VisualElement element, int index)
        {
            element.Q<Label>().text = dropdownElements[index].Name;

            var icon = iconCache.ContainsKey(index) ? iconCache[index] : null;

            element.Q("Icon").style.backgroundImage = new StyleBackground(icon);
            bool selected = dropdownElements[index].IsSelected;

            Color color = selected ? selectedColor : normalColor;
            element.style.backgroundColor = new StyleColor(color);
        }

        private void Update()
        {
            if (Root == null) return;
            var listView = Root.Q("ItemListView") as ListView;
            if (listView == null) return;

            var elements = listView.itemsSource;

            for (int i = 0; i < elements.Count; i++)
            {
                VisualElement visualElement = listView.GetRootElementForId(i);
                if (visualElement == null) continue;
               
                dynamic element = elements[i];
                if (element.Icon != null) continue;

                element.TryUpdateTexture();
                if (element.Icon == null) continue;

                iconCache[i] = element.Icon;
                BindEntryItem(visualElement, i);
            }
        }

        private VisualElement MakeItem() => listElementTreeAsset.CloneTree();
    }
}
