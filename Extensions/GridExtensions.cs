using System.Windows.Controls;

namespace Common.Wpf.Extensions
{
    public static class GridExtensions
    {
        public static int GetRowIndex(this Grid grid, RowDefinition rowDefinition)
        {
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (grid.RowDefinitions[i] == rowDefinition)
                    return i;
            }

            return -1;
        }
    }
}
