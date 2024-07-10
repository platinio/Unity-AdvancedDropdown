using UnityEngine;

namespace ArcaneOnyx.AdvancedDropdown
{
    public delegate Texture2D GetTexture(Object instance);
    
    public class DropdownItem<T>
    {
        private string name;
        private Sprite icon;
        private T item;
        private bool isSelected = false;
        private GetTexture getTextureDelegate;
        private Texture2D texture;

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
        
        public DropdownItem(string name, GetTexture getTextureDelegate, bool isSelected, T item)
        {
            this.getTextureDelegate = getTextureDelegate;
            this.name = name;
            this.item = item;
            this.isSelected = isSelected;
            
            TryUpdateTexture();
        }

        public void TryUpdateTexture()
        {
            if (icon != null || getTextureDelegate == null) return;
            
            var sourceText = getTextureDelegate(item as Object);
            if (sourceText == null) return;

            //clone texture since unity will delete it
            texture = new Texture2D(sourceText.width, sourceText.height);
            texture.SetPixels(sourceText.GetPixels());
            texture.Apply();
            
            icon = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        public void OnDestroy()
        {
            //destroy texture
            if (texture == null) return;
            Object.DestroyImmediate(texture);
        }

        public DropdownItem(string name, bool isSelected, T item)
        {
            this.name = name;
            icon = null;
            this.item = item;
            this.isSelected = isSelected;
        }
    }
}

