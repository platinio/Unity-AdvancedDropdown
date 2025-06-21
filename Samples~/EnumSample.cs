using ArcaneOnyx.AdvancedDropdown;
using UnityEngine;

public enum DamageType
{
    Fire,
    Water,
    Air,
    Poison,
    Physical
}

public class EnumSample : MonoBehaviour
{
    [EnumDropdown] public DamageType DamageType;
}
