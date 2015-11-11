using Common.Wpf.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace Common.Wpf.Windows
{
    public partial class CategoryWindow
    {
        private readonly List<CategoryPanel> _optionPanels;

        private readonly object _data;

        public CategoryWindow(object data, List<CategoryPanel> panels, ResourceManager resourceManager, string resourcePrefix)
        {
            InitializeComponent();

            _data = data;

            Icon = ((Icon) resourceManager.GetObject(resourcePrefix + "_Icon")).ToImageSource();
            Title = resourceManager.GetString(resourcePrefix + "_Title");
            OkayButton.Content = resourceManager.GetString(resourcePrefix + "_OkayButton");
            CancelButton.Content = resourceManager.GetString(resourcePrefix + "_CancelButton");

            // Add all the option categories
            _optionPanels = panels;

            // Load the category list
            LoadCategories();
        }

        private void LoadCategories()
        {
            // Loop over each panel
            foreach (CategoryPanel optionsPanel in _optionPanels)
            {
                // Tell the panel to load itself
                optionsPanel.LoadPanel(_data);

                // Add the panel to the category ist
                CategoryList.Items.Add(new CategoryListItem(optionsPanel));

                // Set the panel into the right side
                CategoryContent.Content = optionsPanel;
            }

            // Select the first item
            CategoryList.SelectedItem = CategoryList.Items[0];
        }

        private void SelectCategory(CategoryPanel panel)
        {
            // Set the content
            CategoryContent.Content = panel;
        }

        private void HandleSelectedCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            // Select the right category
            SelectCategory(((CategoryListItem) CategoryList.SelectedItem).Panel);
        }

        private class CategoryListItem
        {
            public CategoryPanel Panel { get; private set; }

            public CategoryListItem(CategoryPanel panel)
            {
                Panel = panel;
            }

            public override string ToString()
            {
                return Panel.CategoryName;
            }
        }

        private void HandleOkayButtonClick(object sender, RoutedEventArgs e)
        {
            // Loop over each panel and ask them to validate
            foreach (var optionsPanel in _optionPanels)
            {
                // If validation fails...
                if (!optionsPanel.ValidatePanel())
                {
                    // ...select the right category
                    SelectCategory(optionsPanel);

                    // Stop validation
                    return;
                }
            }

            // Loop over each panel and ask them to save
            foreach (CategoryPanel optionsPanel in _optionPanels)
            {
                // Save!
                optionsPanel.SavePanel();
            }

            // Save the actual settings
            //_data.SaveChanges();
            //Properties.Settings.Default.Save();

            DialogResult = true;

            // Close the window
            Close();
        }
    }
}
