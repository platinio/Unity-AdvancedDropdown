using UnityEngine;

namespace Platinio
{
    public class DropdownItem<T>
    {
        private string name;
        private Sprite icon;
        private T item;

        public string Name => name;
        public Sprite Icon => icon;
        public T Item => item;

        public DropdownItem(string name, Sprite icon, T item)
        {
            this.name = name;
            this.icon = icon;
            this.item = item;
        }
    }
}

