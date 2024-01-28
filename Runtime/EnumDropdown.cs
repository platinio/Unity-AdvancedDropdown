using System;
using UnityEngine;

namespace Platinio
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDropdown : PropertyAttribute
    {
        public EnumDropdown()
        {
            
        }
    }
}

