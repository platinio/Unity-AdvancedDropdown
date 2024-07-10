using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneOnyx.AdvancedDropdown
{
    public class AdvancedDropdownEditorWindow : EditorWindow 
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] protected VisualTreeAsset listElementWithIconTreeAsset;
        [SerializeField] protected VisualTreeAsset listElementTreeAsset;

        private Dictionary<int, Sprite> iconCache = new();
        private List<dynamic> dropdownElements;
        
        private readonly Color selectedColor = new (66.0f / 255.0f, 135.0f / 255.0f, 245.0f / 255.0f);
        private readonly Color normalColor = new (56.0f / 255.0f, 56.0f / 255.0f, 56.0f / 255.0f);

        private static readonly float MaxWindowHeight = 500.0f;
        private static readonly float DropdownItemHeight = 60.0f;
        private static readonly float WindowWidth = 350.0f;
        private static Func<Vector2> getCurrentMousePositionFunc;
        private static AdvancedDropdownEditorWindow instance;
       
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
            instance = CreateInstance<AdvancedDropdownEditorWindow>();
            
            instance.ShowAsDropDown(new Rect(instance.GetCurrentMousePosition(), Vector2.zero), instance.CalculateWindowHeight(elements.Count));
            ToolbarSearchField toolbarSearchField = instance.rootVisualElement.Q<ToolbarSearchField>();
            
            toolbarSearchField.RegisterValueChangedCallback(evt =>
            {
                instance.CreateDatabaseListGUI(instance.rootVisualElement, elements, onDropdownSelectionChanged, evt.newValue);
            });

            instance.CreateDatabaseListGUI(instance.rootVisualElement, elements, onDropdownSelectionChanged);
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
                getCurrentMousePositionFunc = (Func<Vector2>) Delegate.CreateDelegate(typeof(Func<Vector2>), currentMousePositionMethod);
            }

            return getCurrentMousePositionFunc();
        }

        private VisualElement Root;
        
        protected void CreateDatabaseListGUI<T>(VisualElement root, List<DropdownItem<T>> elements, Action<T> onDropdownSelectionChanged, string filter = "")
        {
            elements = FilterElements(elements, filter);
            dropdownElements = new List<dynamic>();

            foreach (var element in elements)
            {
                dropdownElements.Add(element);
            }

            Root = root;
            var listView = root.Q("ItemListView") as ListView;
            listView.Clear();
            listView.itemsSource = elements;
            listView.makeItem = delegate
            {
                return MakeItem(HasIcon(elements));
            }; 
            listView.bindItem = BindEntryItem;
            listView.selectionType = SelectionType.Single;
            listView.selectionChanged += delegate(IEnumerable<object> objects)
            {
                var dropdownItem = objects.FirstOrDefault() as DropdownItem<T>;
                onDropdownSelectionChanged.Invoke(dropdownItem.Item);
                Close();
            };
        }

        private List<DropdownItem<T>> FilterElements<T>(List<DropdownItem<T>> elements, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                elements = elements.ToList();
                filter = filter.ToLower();
                
                for (int i = elements.Count - 1; i >= 0; i--)
                {
                    string elementName = elements[i].Name.ToLower();
                    
                    if (!elementName.Contains(filter))
                    {
                        elements.RemoveAt(i);
                    }
                }
            }

            return elements.ToList();
        }

        private bool HasIcon<T>(List<DropdownItem<T>> elements)
        {
            foreach (var element in elements)
            {
                if (element.Icon != null) return true;
            }

            return false;
        }

        private void BindEntryItem(VisualElement element, int index)
        {
            element.Q<Label>().text = dropdownElements[index].Name;

            Sprite icon = iconCache.ContainsKey(index) ? iconCache[index] : dropdownElements[index].Icon;
            VisualElement iconElement = element.Q("Icon");
            
            if (iconElement != null) iconElement.style.backgroundImage = new StyleBackground(icon);

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

        private VisualElement MakeItem(bool hasIcon)
        {
            return hasIcon ? listElementWithIconTreeAsset.CloneTree() : listElementTreeAsset.CloneTree();
        }
    }
}
