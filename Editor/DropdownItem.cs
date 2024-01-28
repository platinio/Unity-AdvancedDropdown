using UnityEngine;

namespace Platinio.AdvancedDropdown
{
    public class DropdownItem<T>
    {
        private string name;
        private Sprite icon;
        private T item;
        private bool isSelected = false;

        public string Name => name;
        public Sprite Icon => icon;
        public T Item => item;
        public bool IsSelected => isSelected;

        public DropdownItem(string name, Sprite icon, bool isSelected, T item)
        {
            this.name = name;
            this.icon = icon;
            this.item = item;
            this.isSelected = isSelected;
        }
    }
}

