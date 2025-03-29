using Calculator.Model;
using Calculator.ViewModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    private void MemoryListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListBox list && list.SelectedItem is double selectedValue)
        {
            var vm = DataContext as CalculatorViewModel;
            if (vm != null)
            {
                vm.Display = selectedValue.ToString(CultureInfo.InvariantCulture);
                vm.IsMemoryPopupOpen = false;
                vm.IsNewEntry = false;
            }
        }
    }

    private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
    {

        MessageBox.Show("Aplicatie realizata de:\nNume: Costea Denisa Adelina\nGrupa: 10LF232",
                        "About",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
    }


    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        var _viewModel = DataContext as CalculatorViewModel;
        if (_viewModel == null) return;

        switch (e.Key)
        {
            
            case Key.D0: _viewModel.NumberClick("0"); break;
            case Key.D1: _viewModel.NumberClick("1"); break;
            case Key.D2: _viewModel.NumberClick("2"); break;
            case Key.D3: _viewModel.NumberClick("3"); break;
            case Key.D4: _viewModel.NumberClick("4"); break;
            case Key.D5: _viewModel.NumberClick("5"); break;
            case Key.D6: _viewModel.NumberClick("6"); break;
            case Key.D7: _viewModel.NumberClick("7"); break;
            case Key.D8: _viewModel.NumberClick("8"); break;
            case Key.D9: _viewModel.NumberClick("9"); break;
            
           
            case Key.NumPad0: _viewModel.NumberClick("0"); break;
            case Key.NumPad1: _viewModel.NumberClick("1"); break;
            case Key.NumPad2: _viewModel.NumberClick("2"); break;
            case Key.NumPad3: _viewModel.NumberClick("3"); break;
            case Key.NumPad4: _viewModel.NumberClick("4"); break;
            case Key.NumPad5: _viewModel.NumberClick("5"); break;
            case Key.NumPad6: _viewModel.NumberClick("6"); break;
            case Key.NumPad7: _viewModel.NumberClick("7"); break;
            case Key.NumPad8: _viewModel.NumberClick("8"); break;
            case Key.NumPad9: _viewModel.NumberClick("9"); break;
            

           
            case Key.Add:       
                _viewModel.OperationClick("+");
                break;
            case Key.OemPlus:   
                _viewModel.OperationClick("+");
                break;
            case Key.Subtract:  
                _viewModel.OperationClick("-");
                break;
            case Key.Multiply:  
                _viewModel.OperationClick("*");
                break;
            case Key.Divide: 
                _viewModel.OperationClick("/");
                break;

            case Key.Enter:
                _viewModel.Calculate();
                break;

            
            case Key.Escape:
                _viewModel.ClearAll();
                break;

            case Key.Back:
                _viewModel.Backspace();
                break;



            case Key.OemComma:
            case Key.OemPeriod:
            case Key.Decimal:
                
                _viewModel.NumberClick(".");
                break;
        }

    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        
        if (DataContext is CalculatorViewModel vm)
        {
            CalculatorSettings settings = new CalculatorSettings
            {
                DigitGroupingEnabled = vm.IsDigitGroupingEnabled,
                IsProgrammerMode = vm.IsProgrammerMode,
                SelectedBase = vm.SelectedBase
            };
            vm.SaveSettings(settings);
        }
    }

}