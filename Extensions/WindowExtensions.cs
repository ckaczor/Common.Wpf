using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.Wpf.Extensions
{
    public static class WindowExtensions
    {
        public static Dictionary<FrameworkElement, BindingExpression> GetExplicitBindingExpressions(this DependencyObject parent)
        {
            // Create a dictionary of framework elements and binding expressions
            Dictionary<FrameworkElement, BindingExpression> bindingExpressions = new Dictionary<FrameworkElement, BindingExpression>();

            // Get all explict bindings into the list
            GetExplicitBindingExpressions(parent, ref bindingExpressions);

            return bindingExpressions;
        }

        private static void GetExplicitBindingExpressions(DependencyObject parent, ref Dictionary<FrameworkElement, BindingExpression> bindingExpressions)
        {
            // Get the number of children
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            // Loop over each child
            for (int childIndex = 0; childIndex < childCount; childIndex++)
            {
                // Get the child
                DependencyObject dependencyObject = VisualTreeHelper.GetChild(parent, childIndex);

                // Check if the object is a tab control
                if (dependencyObject is TabControl)
                {
                    // Cast the tab control
                    TabControl tabControl = (dependencyObject as TabControl);

                    // Loop over each tab
                    foreach (TabItem tabItem in tabControl.Items)
                        GetExplicitBindingExpressions((DependencyObject) tabItem.Content, ref bindingExpressions);
                }
                else
                {
                    // See if the child is a framework element
                    if (dependencyObject is FrameworkElement)
                    {
                        // Cast to framework element
                        FrameworkElement frameworkElement = (FrameworkElement) dependencyObject;

                        // Get the list of properties
                        IEnumerable<DependencyProperty> dependencyProperties = (from PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dependencyObject)
                                                                                select DependencyPropertyDescriptor.FromProperty(propertyDescriptor)
                                                                                    into dependencyPropertyDescriptor
                                                                                    where dependencyPropertyDescriptor != null
                                                                                    select dependencyPropertyDescriptor.DependencyProperty).ToList();

                        // Loop over each dependency property in the list
                        foreach (DependencyProperty dependencyProperty in dependencyProperties)
                        {
                            // Try to get the binding expression for the property
                            BindingExpression bindingExpression = frameworkElement.GetBindingExpression(dependencyProperty);

                            // If there is a binding expression and it is set to explicit then make it update the source
                            if (bindingExpression != null && bindingExpression.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
                                bindingExpressions.Add(frameworkElement, bindingExpression);
                        }
                    }

                    // If the dependency object has any children then check them
                    if (VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
                        GetExplicitBindingExpressions(dependencyObject, ref bindingExpressions);
                }
            }

        }

        public static void UpdateAllSources(this DependencyObject window)
        {
            UpdateAllSources(window, GetExplicitBindingExpressions(window).Values);
        }

        public static void UpdateAllSources(this DependencyObject window, IEnumerable<BindingExpression> bindingExpressions)
        {
            foreach (var expression in bindingExpressions)
                expression.UpdateSource();
        }

        public static void ClearAllValidationErrors(this DependencyObject window)
        {
            ClearAllValidationErrors(window, GetExplicitBindingExpressions(window).Values);
        }

        public static void ClearAllValidationErrors(this DependencyObject window, IEnumerable<BindingExpression> bindingExpressions)
        {
            foreach (var expression in bindingExpressions)
                Validation.ClearInvalid(expression);
        }

    }
}
