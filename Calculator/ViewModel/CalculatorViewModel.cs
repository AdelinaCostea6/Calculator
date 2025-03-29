using Calculator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Text.Json;
using System.IO;

namespace Calculator.ViewModel
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private CalculatorModel _calculator;
        private string _display;
        private double _firstOperand;
        private double _secondOperand;
        private string _currentOperation;
        private bool _isNewEntry;
        private string _historyDisplay;
        private int _selectedBase = 10;
        private bool _isDigitGroupingEnabled;
        private ObservableCollection<double> _memoryValues = new ObservableCollection<double>();
        private bool _isMemoryPopupOpen;
        private bool _isProgrammerMode;
        private string _binValue;
        private string _octValue;
        private string _decValue;
        private string _hexValue;
        private string _rawDisplay = "";
        private string _internalClipboard = "";



        public ICommand NumberClickCommand { get; }
        public ICommand OperationClickCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand SquareRootCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand NegativeCommand { get; }
        public ICommand InverseCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryShowPopupCommand { get; }
        public ICommand SetStandardModeCommand { get; }
        public ICommand SetProgrammerModeCommand { get; }

        public ICommand SwitchBaseCommand { get; }


        public CalculatorViewModel()
        {
            _calculator = new CalculatorModel();
            _display = "0";
            _firstOperand = 0;
            _secondOperand = 0;
            _currentOperation = string.Empty;
            _isNewEntry = true;
            _historyDisplay = string.Empty;

           
            NumberClickCommand = new RelayCommand<string>(NumberClick);
            OperationClickCommand = new RelayCommand<string>(OperationClick);
            CalculateCommand = new RelayCommand(Calculate);
            SquareRootCommand = new RelayCommand(SquareRoot);
            SquareCommand = new RelayCommand(Square);
            NegativeCommand = new RelayCommand(Negative);
            InverseCommand = new RelayCommand(Inverse);
            BackspaceCommand = new RelayCommand(Backspace);
            ClearEntryCommand = new RelayCommand(ClearEntry);
            ClearAllCommand = new RelayCommand(ClearAll);
            CopyCommand = new RelayCommand(Copy);
            CutCommand = new RelayCommand(Cut);
            PasteCommand = new RelayCommand(Paste);
            MemoryAddCommand = new RelayCommand(MemoryAdd);
            MemorySubtractCommand = new RelayCommand(MemorySubtract);
            MemoryStoreCommand = new RelayCommand(MemoryStore);
            MemoryRecallCommand = new RelayCommand(MemoryRecall);
            MemoryClearCommand = new RelayCommand(MemoryClear);
            MemoryShowPopupCommand = new RelayCommand(() => IsMemoryPopupOpen = true);
            SetStandardModeCommand = new RelayCommand(() => IsProgrammerMode = false);
            SetProgrammerModeCommand = new RelayCommand(() => IsProgrammerMode = true);
            SwitchBaseCommand = new RelayCommand<string>(param =>
            {
                if (!IsProgrammerMode)
                    return;
                if (int.TryParse(param, out int newBase))
                {
                    SelectedBase = newBase;
                }
            });

            CalculatorSettings settings = LoadSettings();
            IsDigitGroupingEnabled = settings.DigitGroupingEnabled;
            IsProgrammerMode = settings.IsProgrammerMode;
            SelectedBase = settings.SelectedBase;
        }


        public int SelectedBase
        {
            get => _selectedBase;
            set
            {
                if (!IsProgrammerMode)
                    return;
                if (_selectedBase != value)
                {
                    double decVal;
                    try
                    {
                        decVal = ConvertBaseToDecimal(Display, _selectedBase);
                    }
                    catch
                    {
                        Display = "Error";
                        return;
                    }
                    _selectedBase = value;
                    OnPropertyChanged(nameof(SelectedBase));
                    Display = ConvertDecimalToBase(decVal, _selectedBase);
                }
            }
        }



        public bool IsDigitGroupingEnabled
        {
            get => _isDigitGroupingEnabled;
            set
            {
                if (_isDigitGroupingEnabled != value)
                {
                    _isDigitGroupingEnabled = value;
                    OnPropertyChanged(nameof(IsDigitGroupingEnabled));
                }
            }
        }


        public string Display
        {
            get { return _display; }
            set
            {
                if (_display != value)
                {
                    _display = value;
                    OnPropertyChanged(nameof(Display));
                }
            }
        }





        public ObservableCollection<double> MemoryValues
        {
            get => _memoryValues;
            set { _memoryValues = value; OnPropertyChanged(nameof(MemoryValues)); }
        }

        
        public bool IsMemoryPopupOpen
        {
            get => _isMemoryPopupOpen;
            set { _isMemoryPopupOpen = value; OnPropertyChanged(nameof(IsMemoryPopupOpen)); }
        }

        public string HistoryDisplay
        {
            get { return _historyDisplay; }
            set
            {
                if (_historyDisplay != value)
                {
                    _historyDisplay = value;
                    OnPropertyChanged(nameof(HistoryDisplay));
                }
            }
        }

       
        public bool IsProgrammerMode
        {
            get => _isProgrammerMode;
            set
            {
                _isProgrammerMode = value;
                OnPropertyChanged(nameof(IsProgrammerMode));
                OnPropertyChanged(nameof(StandardGridVisibility));
                OnPropertyChanged(nameof(ProgrammerGridVisibility));
            }
        }

        public Visibility StandardGridVisibility
            => IsProgrammerMode ? Visibility.Collapsed : Visibility.Visible;
        public Visibility ProgrammerGridVisibility
            => IsProgrammerMode ? Visibility.Visible : Visibility.Collapsed;

       
        public string HexValue
        {
            get => _hexValue;
            set { _hexValue = value; 
                OnPropertyChanged(nameof(HexValue)); }
        }

        public string DecValue
        {
            get => _decValue;
            set { _decValue = value; 
                OnPropertyChanged(nameof(DecValue)); }
        }

        public string OctValue
        {
            get => _octValue;
            set { _octValue = value; 
                OnPropertyChanged(nameof(OctValue)); }
        }

        
        public string BinValue
        {
            get => _binValue;
            set { _binValue = value; 
                OnPropertyChanged(nameof(BinValue)); }
        }

        public bool IsNewEntry
        {
            get { return _isNewEntry; }
            set
            {
                if (_isNewEntry != value)
                {
                    _isNewEntry = value;
                    OnPropertyChanged(nameof(IsNewEntry));
                }
            }
        }

      



        public void NumberClick(string input)
        {
            if (IsProgrammerMode)
            {
                if (!IsDigitValidForBase(input, SelectedBase))
                {
                    return;
                }
                if (_isNewEntry)
                {
                    _rawDisplay = input;
                    _isNewEntry = false;
                }
                else
                {
                    _rawDisplay += input;
                }
                try
                {
                    
                    double decVal = ConvertBaseToDecimal(_rawDisplay, SelectedBase);

                    if (SelectedBase == 10 && IsDigitGroupingEnabled)
                    {
                        Display = decVal.ToString("#,0", CultureInfo.GetCultureInfo("en-GB"));
                    }
                    else
                    {
                        Display = _rawDisplay;
                    }
                    UpdateAllBaseDisplays(decVal);
                }
                catch
                {
                    Display = "Error";
                }
            }
            else
            {
                if (_isNewEntry)
                {
                    _rawDisplay = input;
                    _isNewEntry = false;
                }
                else
                {
                    _rawDisplay += input;
                }
                CultureInfo ukCulture = new CultureInfo("en-GB");
                if (_rawDisplay == "." || _rawDisplay.EndsWith("."))
                {
                    Display = _rawDisplay;
                }
                else if (double.TryParse(_rawDisplay, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, ukCulture, out double result))
                {
                    
                    string format = IsDigitGroupingEnabled ? "#,0.####" : "0.####";
                    Display = result.ToString(format, ukCulture);
                }
                else
                {
                    Display = _rawDisplay;
                }
            }
        }


        private bool IsDigitValidForBase(string digit, int baseNum)
        {
            string validHex = "0123456789ABCDEFabcdef";
            string validDec = "0123456789";
            string validOct = "01234567";
            string validBin = "01";

            return baseNum switch
            {
                2 => digit.All(c => validBin.Contains(c)),
                8 => digit.All(c => validOct.Contains(c)),
                10 => digit.All(c => validDec.Contains(c)),
                16 => digit.All(c => validHex.Contains(c)),
                _ => false,
            };
        }

        private double ConvertBaseToDecimal(string text, int baseNum)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException("Textul este gol.");

            text = text.Trim().ToUpper();

            try
            {
                int intVal = baseNum switch
                {
                    2 => Convert.ToInt32(text, 2),
                    8 => Convert.ToInt32(text, 8),
                    16 => Convert.ToInt32(text, 16),
                    _ => int.Parse(text),
                };
                return intVal;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Textul '{text}' nu este valid pentru baza {baseNum}.", ex);
            }
        }

        private string ConvertDecimalToBase(double val, int baseNum)
        {
            int intVal = (int)val;
            return baseNum switch
            {
                2 => Convert.ToString(intVal, 2),
                8 => Convert.ToString(intVal, 8),
                16 => intVal.ToString("X"),
                _ => intVal.ToString(),
            };
        }


        private void UpdateAllBaseDisplays(double decVal)
        {
            int val = (int)decVal;
            HexValue = val.ToString("X");
            DecValue = val.ToString();
            OctValue = Convert.ToString(val, 8);
            BinValue = Convert.ToString(val, 2);
        }


        public void OperationClick(string operation)
        {
            if (!_isNewEntry)
            {
                string operandStr = Display;
                string[] operators = { "+", "-", "*", "/" };
                foreach (var op in operators)
                {
                    if (operandStr.EndsWith(op))
                    {
                        operandStr = operandStr.Substring(0, operandStr.Length - op.Length);
                        break;
                    }
                }

                if (IsProgrammerMode)
                {
                    try
                    {
                        
                        if (SelectedBase == 10)
                        {
                            var culture = CultureInfo.GetCultureInfo("en-GB");
                            operandStr = operandStr.Replace(culture.NumberFormat.NumberGroupSeparator, "");
                        }
                        _secondOperand = ConvertBaseToDecimal(operandStr, SelectedBase);
                    }
                    catch
                    {
                        Display = "Error";
                        return;
                    }
                }
                else
                {
                    if (!double.TryParse(operandStr, out _secondOperand))
                    {
                        Display = "Error";
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(_currentOperation))
                {
                    Calculate();
                }
                else
                {
                    _firstOperand = _secondOperand;
                }
            }

            _currentOperation = operation;
            if (IsProgrammerMode)
                HistoryDisplay = ConvertDecimalToBase(_firstOperand, SelectedBase) + " " + operation;
            else
                HistoryDisplay = _firstOperand.ToString("G15") + " " + operation;
            _isNewEntry = true;
        }

       

        public void Calculate()
        {
            if (string.IsNullOrEmpty(_currentOperation) || _isNewEntry)
                return;

            if (IsProgrammerMode)
            {
                try
                {
                    string textToConvert = _rawDisplay;
                    if (SelectedBase == 10)
                    {
                        var culture = CultureInfo.GetCultureInfo("en-GB");
                        textToConvert = textToConvert.Replace(culture.NumberFormat.NumberGroupSeparator, "");
                    }
                    _secondOperand = ConvertBaseToDecimal(textToConvert, SelectedBase);
                }
                catch
                {
                    Display = "Error";
                    return;
                }
            }
            else
            {
                string operandStr = Display;
                if (!double.TryParse(operandStr, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-GB"), out _secondOperand))
                {
                    Display = "Error";
                    return;
                }
            }

            try
            {
                switch (_currentOperation)
                {
                    case "+":
                        _firstOperand = _calculator.Add(_firstOperand, _secondOperand);
                        break;
                    case "-":
                        _firstOperand = _calculator.Substract(_firstOperand, _secondOperand);
                        break;
                    case "*":
                        _firstOperand = _calculator.Multiply(_firstOperand, _secondOperand);
                        break;
                    case "/":
                        if (_secondOperand == 0)
                        {
                            Display = "Error";
                            return;
                        }
                        _firstOperand = _calculator.Divide(_firstOperand, _secondOperand);
                        break;
                    case "%":
                        _firstOperand = _calculator.Modulus(_firstOperand, _secondOperand);
                        break;
                }

               
                if (IsProgrammerMode)
                    HistoryDisplay += " " + ConvertDecimalToBase(_secondOperand, SelectedBase) + " = ";
                else
                    HistoryDisplay += " " + _secondOperand.ToString("G15") + " = ";



                if (IsProgrammerMode)
                {
                    var culture = CultureInfo.GetCultureInfo("en-GB");
                    if (SelectedBase == 10 && IsDigitGroupingEnabled)
                    {
                        Display = _firstOperand.ToString("#,0.####", culture);
                    }
                    else
                    {
                        Display = ConvertDecimalToBase(_firstOperand, SelectedBase);
                    }
                    UpdateAllBaseDisplays(_firstOperand);
                    _rawDisplay = _firstOperand.ToString();
                }
                else
                {
                    if (!IsDigitGroupingEnabled)
                        Display = _firstOperand.ToString("G15");
                    else
                        Display = _firstOperand.ToString("#,0.####", CultureInfo.GetCultureInfo("en-GB"));
                    _rawDisplay = _firstOperand.ToString();
                }

                _currentOperation = string.Empty;
                _isNewEntry = true;
            }
            catch (Exception)
            {
                Display = "Error";
            }
        }



      
        public void SquareRoot()
        {
            try
            {
                if (!double.TryParse(Display, out double num) || num < 0)
                {
                    Display = "Error";
                    return;
                }

                double result = _calculator.SquareRoot(num);

                if (!string.IsNullOrEmpty(HistoryDisplay) && !HistoryDisplay.Contains("="))
                    HistoryDisplay += $" sqrt({num})";
                else
                    HistoryDisplay = $"sqrt({num})";
               
                Display = result.ToString("G15");
            }
            catch (Exception)
            {
                Display = "Error";
            }
        }

        public void Square()
        {
            try
            {
                if (!double.TryParse(Display, out double num))
                {
                    Display = "Error";
                    return;
                }

                double result = _calculator.Square(num);

                if (!string.IsNullOrEmpty(HistoryDisplay) && !HistoryDisplay.Contains("="))
                    HistoryDisplay += $" pow({num}, 2)";
                else
                    HistoryDisplay = $"pow({num}, 2)";

                Display = result.ToString("G15");
            }
            catch (Exception)
            {
                Display = "Error";
            }
        }

        public void Negative()
        {
            try
            {
                if (!double.TryParse(Display, out double num))
                {
                    Display = "Error";
                    return;
                }

                double result = _calculator.Negative(num);

                if (!string.IsNullOrEmpty(HistoryDisplay) && !HistoryDisplay.Contains("="))
                    HistoryDisplay += $" negate({num})";
                else
                    HistoryDisplay = $"negate({num})";

                Display = result.ToString("G15");
            }
            catch (Exception)
            {
                Display = "Error";
            }
        }

        public void Inverse()
        {
            try
            {
                if (!double.TryParse(Display, out double num) || num == 0)
                {
                    Display = "Error";
                    return;
                }

                double result = _calculator.Inverse(num);

                if (!string.IsNullOrEmpty(HistoryDisplay) && !HistoryDisplay.Contains("="))
                    HistoryDisplay += $" 1/({num})";
                else
                    HistoryDisplay = $"1/({num})";

                Display = result.ToString("G15");
            }
            catch (Exception)
            {
                Display = "Error";
            }
        }

        public void ClearAll()
        {
            Display = "0";
            _firstOperand = 0;
            _secondOperand = 0;
            _currentOperation = string.Empty;
            _isNewEntry = true;
            HistoryDisplay = string.Empty;
            UpdateAllBaseDisplays(0);
        }

        public void Backspace()
        {
           
            if (!string.IsNullOrEmpty(_rawDisplay) && _rawDisplay.Length > 1)
            {
                _rawDisplay = _rawDisplay.Substring(0, _rawDisplay.Length - 1);
                Display = _rawDisplay; 
            }
            else
            {
                _rawDisplay = "0";
                Display = "0";
                _isNewEntry = true;
            }
        }


        public void ClearEntry()
        {
            string[] tokens = Display.Split(' ');
            Display = tokens.Length == 1 ? "0" : string.Join(" ", tokens.Take(tokens.Length - 1));
            _isNewEntry = true;
        }

        public void Copy()
        {
            _internalClipboard = Display;
        }

        public void Cut()
        {

            _internalClipboard = Display;
            Display = "";
            _rawDisplay = "";
            _isNewEntry = true;
        }

        

        public void Paste()
        {
            if (!string.IsNullOrEmpty(_internalClipboard))
            {
                
                if (_isNewEntry)
                {
                    _rawDisplay = _internalClipboard;
                    _isNewEntry = false;
                }
                else
                {
                    _rawDisplay += _internalClipboard;
                }

                CultureInfo ukCulture = CultureInfo.GetCultureInfo("en-GB");
                if (IsProgrammerMode)
                {
                    try
                    {
                        double decVal = ConvertBaseToDecimal(_rawDisplay, SelectedBase);
                        if (SelectedBase == 10 && IsDigitGroupingEnabled)
                        {
                            Display = decVal.ToString("#,0", ukCulture);
                        }
                        else
                        {
                            Display = _rawDisplay;
                        }
                        UpdateAllBaseDisplays(decVal);
                    }
                    catch
                    {
                        Display = "Error";
                    }
                }
                else
                {
                    if (_rawDisplay == "." || _rawDisplay.EndsWith("."))
                    {
                        Display = _rawDisplay;
                    }
                    else if (double.TryParse(_rawDisplay, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, ukCulture, out double result))
                    {
                        string format = IsDigitGroupingEnabled ? "#,0.####" : "0.####";
                        Display = result.ToString(format, ukCulture);
                    }
                    else
                    {
                        Display = _rawDisplay;
                    }
                }
            }
        }


        private void MemoryAdd()
        {
            if (!double.TryParse(Display, out double val))
            {
                Display = "Error";
                return;
            }
            if (MemoryValues.Count == 0)
                MemoryValues.Add(val);
            else
                MemoryValues[MemoryValues.Count - 1] += val;
        }

        private void MemorySubtract()
        {
            if (!double.TryParse(Display, out double val))
            {
                Display = "Error";
                return;
            }
            if (MemoryValues.Count == 0)
                MemoryValues.Add(-val);
            else
                MemoryValues[MemoryValues.Count - 1] -= val;
        }

        private void MemoryStore()
        {
            if (!double.TryParse(Display, out double val))
            {
                Display = "Error";
                return;
            }
            MemoryValues.Add(val);
        }

        private void MemoryRecall()
        {
            if (MemoryValues.Count == 0)
                return;
            double lastValue = MemoryValues[MemoryValues.Count - 1];
            Display = lastValue.ToString(CultureInfo.InvariantCulture);
            _isNewEntry = false;
        }

        private void MemoryClear()
        {
            MemoryValues.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));




        public void SaveSettings(CalculatorSettings settings)
        {
            
            string path = "settings.json";
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(path, json);
        }

        public CalculatorSettings LoadSettings()
        {
            string path = "settings.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                try
                {
                    var settings = JsonSerializer.Deserialize<CalculatorSettings>(json);
                    return settings;
                }
                catch
                {

                    return new CalculatorSettings { DigitGroupingEnabled = false, IsProgrammerMode = false, SelectedBase = 10 };
                }
            }
            else
            {
                return new CalculatorSettings { DigitGroupingEnabled = false, IsProgrammerMode = false, SelectedBase = 10 };
            }
        }

    }
}
