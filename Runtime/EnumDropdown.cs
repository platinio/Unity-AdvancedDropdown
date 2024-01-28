using System;
using UnityEngine;

namespace Platinio.AdvancedDropdown
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDropdown : PropertyAttribute
    {
        public EnumDropdown()
        {
            
        }
    }
}

