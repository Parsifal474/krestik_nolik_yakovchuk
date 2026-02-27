using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace krestik_nolik_yakovchuk
{
    public partial class CustomizationWindow : Window
    {
        public string SymbolX { get; private set; } = "❌";
        public Color ColorX { get; private set; } = Color.FromRgb(0xFF, 0x6B, 0x6B);
        public string SymbolO { get; private set; } = "⭕";
        public Color ColorO { get; private set; } = Color.FromRgb(0x4E, 0xCD, 0xC4);

        private Button? selectedColorButtonX;
        private Button? selectedColorButtonO;

        public CustomizationWindow()
        {
            InitializeComponent();
            selectedColorButtonX = null;
            selectedColorButtonO = null;
        }

        private void SymbolXCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SymbolXCombo.SelectedItem is ComboBoxItem item)
            {
                SymbolX = item.Content.ToString() ?? "❌";
            }
        }

        private void SymbolOCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SymbolOCombo.SelectedItem is ComboBoxItem item)
            {
                SymbolO = item.Content.ToString() ?? "⭕";
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string hexColor)
            {
                selectedColorButtonX = button;
                ColorX = (Color)ColorConverter.ConvertFromString(hexColor);
                UpdateBorderSelection();
            }
        }

        private void ColorButtonO_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string hexColor)
            {
                selectedColorButtonO = button;
                ColorO = (Color)ColorConverter.ConvertFromString(hexColor);
                UpdateBorderSelection();
            }
        }

        private void UpdateBorderSelection()
        {
            // Получаем ScrollViewer
            if (this.Content is Grid mainGrid && mainGrid.Children.Count > 1)
            {
                if (mainGrid.Children[1] is ScrollViewer scrollViewer && scrollViewer.Content is StackPanel mainStackPanel)
                {
                    // Обновляем выделение для ❌ (первый Border)
                    if (mainStackPanel.Children.Count > 0 && mainStackPanel.Children[0] is Border borderX)
                    {
                        if (borderX.Child is StackPanel stackPanelX && stackPanelX.Children.Count > 3)
                        {
                            if (stackPanelX.Children[3] is StackPanel colorPanelX)
                            {
                                foreach (var child in colorPanelX.Children)
                                {
                                    if (child is Button btn)
                                    {
                                        btn.BorderThickness = (btn == selectedColorButtonX) ? new Thickness(2) : new Thickness(0);
                                        btn.BorderBrush = Brushes.White;
                                    }
                                }
                            }
                        }
                    }

                    // Обновляем выделение для ⭕ (второй Border)
                    if (mainStackPanel.Children.Count > 1 && mainStackPanel.Children[1] is Border borderO)
                    {
                        if (borderO.Child is StackPanel stackPanelO && stackPanelO.Children.Count > 3)
                        {
                            if (stackPanelO.Children[3] is StackPanel colorPanelO)
                            {
                                foreach (var child in colorPanelO.Children)
                                {
                                    if (child is Button btn)
                                    {
                                        btn.BorderThickness = (btn == selectedColorButtonO) ? new Thickness(2) : new Thickness(0);
                                        btn.BorderBrush = Brushes.White;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}