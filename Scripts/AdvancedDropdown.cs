using System;
using System.Collections.Generic;
using System.Reflection;
using Platinio;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class AdvancedDropdown : EditorWindow 
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField] protected VisualTreeAsset listElementTreeAsset;

    private List<dynamic> dropdownElements; 

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement visualTreeClone = m_VisualTreeAsset.Instantiate();
        root.Add(visualTreeClone);
        
        
    }
    
    

    public void ShowDropdown<T>(List<DropdownItem<T>> elements)
    {
        dropdownElements = new List<dynamic>();
        
        foreach (var element in elements)
        {
            dropdownElements.Add(element);
        }

        CreateDatabaseListGUI(rootVisualElement, elements);
    }

    protected void CreateDatabaseListGUI<T>(VisualElement root, List<DropdownItem<T>> elements)
    {
        var listView = root.Q("ItemListView") as ListView;
        listView.Clear();
        listView.makeItem = MakeItem;
        listView.bindItem = BindEntryItem;
        listView.itemsSource = elements;
        listView.selectionType = SelectionType.Single;
        //listView.selectionChanged += OnEntrySelectionChanged;
    }
    
    private void BindEntryItem(VisualElement element, int index)
    {
        element.Q<Label>().text = dropdownElements[index].Name;
        element.Q("Icon").style.backgroundImage = new StyleBackground(dropdownElements[index].Icon as Sprite);
    }
    
    private VisualElement MakeItem() => listElementTreeAsset.CloneTree();
}
