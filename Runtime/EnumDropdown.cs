using System;
using UnityEngine;

namespace ArcaneOnyx.AdvancedDropdown
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDropdown : PropertyAttribute
    {
        public EnumDropdown()
        {
            
        }
    }
}

