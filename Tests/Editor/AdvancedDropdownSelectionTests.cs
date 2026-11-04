using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneOnyx.AdvancedDropdown.Tests
{
    public class AdvancedDropdownSelectionTests
    {
        private readonly List<string> pickedItems = new();

        [SetUp]
        public void OpenDropdown()
        {
            pickedItems.Clear();

            var elements = new List<DropdownItem<string>>
            {
                new("Alpha", false, "alpha-item"),
                new("Beta", false, "beta-item"),
                new("Gamma", false, "gamma-item"),
            };

            AdvancedDropdownEditorWindow.ShowDropdown(elements, pickedItems.Add);
        }

        [TearDown]
        public void CloseOpenDropdowns()
        {
            foreach (var window in Resources.FindObjectsOfTypeAll<AdvancedDropdownEditorWindow>())
            {
                window.Close();
            }
        }

        [Test]
        public void PickWithoutSearching_ReportsThePickOnce()
        {
            PickRow(1);

            CollectionAssert.AreEqual(new[] { "beta-item" }, pickedItems);
        }

        [Test]
        public void PickAfterTwoSearchKeystrokes_ReportsThePickOnce()
        {
            TypeInSearchField("a");
            TypeInSearchField("al");

            PickRow(0);

            CollectionAssert.AreEqual(new[] { "alpha-item" }, pickedItems);
        }

        [Test]
        public void PickAfterSearch_ReportsTheEntryFromTheFilteredList()
        {
            TypeInSearchField("gam");

            PickRow(0);

            CollectionAssert.AreEqual(new[] { "gamma-item" }, pickedItems);
        }

        [Test]
        public void Pick_ClosesTheDropdown()
        {
            PickRow(0);

            Assert.That(Resources.FindObjectsOfTypeAll<AdvancedDropdownEditorWindow>(), Is.Empty);
        }

        private static VisualElement OpenDropdownRoot() =>
            Resources.FindObjectsOfTypeAll<AdvancedDropdownEditorWindow>().Single().rootVisualElement;

        private static void TypeInSearchField(string text) =>
            OpenDropdownRoot().Q<ToolbarSearchField>().value = text;

        private static void PickRow(int rowIndex) =>
            OpenDropdownRoot().Q<ListView>().selectedIndex = rowIndex;
    }
}
