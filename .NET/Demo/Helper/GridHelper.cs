using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Demo_WPF.Helper
{
    /// <summary>
    /// Data grid helper
    /// </summary>
    public class GridHelper
    {
        /// <summary>
        /// Border
        /// </summary>
        public static readonly DependencyProperty ShowBorderProperty = DependencyProperty.RegisterAttached(
            "ShowBorder", typeof(bool), typeof(GridHelper), new PropertyMetadata(OnShowBorderChanged));
        public static bool GetShowBorder(DependencyObject obj) => (bool)obj.GetValue(ShowBorderProperty);
        public static void SetShowBorder(DependencyObject obj, bool value) => obj.SetValue(ShowBorderProperty, value);

        public static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid grid) return;
            if ((bool)e.OldValue)
                grid.Loaded -= (s, arg) => { };
            else
                grid.Loaded += new RoutedEventHandler(DrawBorder);
        }

        /// <summary>
        /// Line thickness
        /// </summary>
        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.RegisterAttached(
            "LineThickness", typeof(double), typeof(GridHelper), new PropertyMetadata(1d, OnGridLineThicknessChanged));
        public static double GetLineThickness(DependencyObject obj) => (double)obj.GetValue(LineThicknessProperty);
        public static void SetLineThickness(DependencyObject obj, double value) => obj.SetValue(LineThicknessProperty, value);

        public static void OnGridLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Line brush
        /// </summary>
        public static readonly DependencyProperty LineBrushProperty = DependencyProperty.RegisterAttached(
            "LineBrush", typeof(Brush), typeof(GridHelper), new PropertyMetadata(Brushes.Gray, OnGridLineBrushChanged));
        public static Brush GetLineBrush(DependencyObject obj) => (Brush)obj.GetValue(LineBrushProperty) ?? Brushes.Black;
        public static void SetLineBrush(DependencyObject obj, Brush value) => obj.SetValue(LineBrushProperty, value);

        public static void OnGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Draw border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DrawBorder(object sender, RoutedEventArgs e)
        {
            if (sender is not Grid grid) return;

            var rowCount = Math.Max(1, grid.RowDefinitions.Count);
            var columnCount = Math.Max(1, grid.ColumnDefinitions.Count);
            int[,] grids = new int[rowCount, columnCount];
            var controls = grid.Children;
            var count = controls.Count;
            var thickness = GetLineThickness(grid);
            var brush = GetLineBrush(grid);
            for (int i = 0; i < count; i++)
            {
                var item = controls[i] as FrameworkElement;
                var row = Grid.GetRow(item);
                var column = Grid.GetColumn(item);
                var rowSpan = Grid.GetRowSpan(item);
                var columnSpan = Grid.GetColumnSpan(item);
                for (int rowTemp = 0; rowTemp < rowSpan; rowTemp++)
                {
                    for (int colTemp = 0; colTemp < columnSpan; colTemp++)
                    {
                        grids[rowTemp + row, colTemp + column] = 1;
                    }
                }

                var border = CreateBorder(row, column, rowSpan, columnSpan, thickness, brush);
                grid.Children.RemoveAt(i);
                border.Child = item;
                grid.Children.Insert(i, border);
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    if (grids[i, k] == 0)
                    {
                        grid.Children.Add(CreateBorder(i, k, 1, 1, thickness, brush));
                    }
                }
            }
        }

        /// <summary>
        /// Create border
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <param name="rowSpan">Row span</param>
        /// <param name="colSpan">Column span</param>
        /// <param name="thickness">Thickness</param>
        /// <param name="brush">Brush</param>
        /// <returns>Border</returns>
        private static Border CreateBorder(int row, int col, int rowSpan, int colSpan, double thickness, Brush brush)
        {
            var borderThickness = new Thickness(col == 0 ? thickness : 0, row == 0 ? thickness : 0, thickness, thickness);
            var border = new Border()
            {
                BorderThickness = borderThickness,
                BorderBrush = brush
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            Grid.SetRowSpan(border, rowSpan);
            Grid.SetColumnSpan(border, colSpan);
            return border;
        }
    }
}
