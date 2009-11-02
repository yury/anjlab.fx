using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnjLab.FX.Wpf.Calculator
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class Calculator : UserControl
    {
        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register("Result", typeof (double),
                                                                                               typeof (Calculator));
        public Calculator()
        {
            InitializeComponent();

            ProcessKey('0');
            EraseDisplay = true;
        }
        private enum Operation
        {
            None,
            Devide,
            Multiply,
            Subtract,
            Add,
            Percent,
            Sqrt,
            OneX,
            Negate
        }
        private Operation LastOper;
        private string _last_val;
        private string _mem_val;

        public double Result
        {
            get { return (double)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        //flag to erase or just add to current display flag
        private bool EraseDisplay { get; set; }
        //Get/Set Memory cell value
        private Double Memory
        {
            get
            {
                if (_mem_val == string.Empty)
                    return 0.0;
                return Convert.ToDouble(_mem_val);
            }
            set
            {
                _mem_val = value.ToString();
            }
        }
        //Lats value entered
        private string LastValue
        {
            get
            {
                if (_last_val == string.Empty)
                    return "0";
                return _last_val;

            }
            set
            {
                _last_val = value;
            }
        }
        //The current Calculator display
        private string Display { get; set; }

        // Sample event handler:  
        private void OnWindowKeyDown(object sender, TextCompositionEventArgs /*System.Windows.Input.KeyEventArgs*/ e)
        {
            string s = e.Text;
            char c = (s.ToCharArray())[0];
            e.Handled = true;

            if ((c >= '0' && c <= '9') || c == '.' || c == '\b')  // '\b' is backspace
            {
                ProcessKey(c);
                return;
            }
            switch (c)
            {
                case '+':
                    ProcessOperation("BPlus");
                    break;
                case '-':
                    ProcessOperation("BMinus");
                    break;
                case '*':
                    ProcessOperation("BMultiply");
                    break;
                case '/':
                    ProcessOperation("BDevide");
                    break;
                case '%':
                    ProcessOperation("BPercent");
                    break;
                case '=':
                    ProcessOperation("BEqual");
                    break;
            }

        }
        private void DigitBtn_Click(object sender, RoutedEventArgs e)
        {
            string s = ((Button)sender).Content.ToString();

            //char[] ids = ((Button)sender).ID.ToCharArray();
            char[] ids = s.ToCharArray();
            ProcessKey(ids[0]);

        }
        private void ProcessKey(char c)
        {
            if (EraseDisplay)
            {
                Display = string.Empty;
                EraseDisplay = false;
            }
            AddToDisplay(c);
        }
        private void ProcessOperation(string s)
        {
            switch (s)
            {
                case "BPM":
                    LastOper = Operation.Negate;
                    LastValue = Display;
                    CalcResults();
                    LastValue = Display;
                    EraseDisplay = true;
                    LastOper = Operation.None;
                    break;
                case "BDevide":

                    if (EraseDisplay)    //stil wait for a digit...
                    {  //stil wait for a digit...
                        LastOper = Operation.Devide;
                        break;
                    }
                    CalcResults();
                    LastOper = Operation.Devide;
                    LastValue = Display;
                    EraseDisplay = true;
                    break;
                case "BMultiply":
                    if (EraseDisplay)    //stil wait for a digit...
                    {  //stil wait for a digit...
                        LastOper = Operation.Multiply;
                        break;
                    }
                    CalcResults();
                    LastOper = Operation.Multiply;
                    LastValue = Display;
                    EraseDisplay = true;
                    break;
                case "BMinus":
                    if (EraseDisplay)    //stil wait for a digit...
                    {  //stil wait for a digit...
                        LastOper = Operation.Subtract;
                        break;
                    }
                    CalcResults();
                    LastOper = Operation.Subtract;
                    LastValue = Display;
                    EraseDisplay = true;
                    break;
                case "BPlus":
                    if (EraseDisplay)
                    {  //stil wait for a digit...
                        LastOper = Operation.Add;
                        break;
                    }
                    CalcResults();
                    LastOper = Operation.Add;
                    LastValue = Display;
                    EraseDisplay = true;
                    break;
                case "BEqual":
                    if (EraseDisplay)    //stil wait for a digit...
                        break;
                    CalcResults();
                    EraseDisplay = true;
                    LastOper = Operation.None;
                    LastValue = Display;
                    //val = Display;
                    break;
                case "BSqrt":
                    LastOper = Operation.Sqrt;
                    LastValue = Display;
                    CalcResults();
                    LastValue = Display;
                    EraseDisplay = true;
                    LastOper = Operation.None;
                    break;
                case "BPercent":
                    if (EraseDisplay)    //stil wait for a digit...
                    {  //stil wait for a digit...
                        LastOper = Operation.Percent;
                        break;
                    }
                    CalcResults();
                    LastOper = Operation.Percent;
                    LastValue = Display;
                    EraseDisplay = true;
                    //LastOper = Operation.None;
                    break;
                case "BOneOver":
                    LastOper = Operation.OneX;
                    LastValue = Display;
                    CalcResults();
                    LastValue = Display;
                    EraseDisplay = true;
                    LastOper = Operation.None;
                    break;
                case "BC":  //clear All
                    LastOper = Operation.None;
                    Display = LastValue = string.Empty;
                    UpdateDisplay();
                    break;
                case "BCE":  //clear entry
                    LastOper = Operation.None;
                    Display = LastValue;
                    UpdateDisplay();
                    break;
                case "BMemClear":
                    Memory = 0.0F;
                    DisplayMemory();
                    break;
                case "BMemSave":
                    Memory = Convert.ToDouble(Display);
                    DisplayMemory();
                    EraseDisplay = true;
                    break;
                case "BMemRecall":
                    Display = /*val =*/ Memory.ToString();
                    UpdateDisplay();
                    //if (LastOper != Operation.None)   //using MR is like entring a digit
                    EraseDisplay = false;
                    break;
                case "BMemPlus":
                    Double d = Memory + Convert.ToDouble(Display);
                    Memory = d;
                    DisplayMemory();
                    EraseDisplay = true;
                    break;
            }

        }

        private void OperBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcessOperation(((Button)sender).Name);
        }


        private double Calc(Operation lastOper)
        {
            double d = 0.0;


            try
            {
                switch (lastOper)
                {
                    case Operation.Devide:
                        d = (Convert.ToDouble(LastValue) / Convert.ToDouble(Display));
                        CheckResult(d);
                        break;
                    case Operation.Add:
                        d = Convert.ToDouble(LastValue) + Convert.ToDouble(Display);
                        CheckResult(d);
                        break;
                    case Operation.Multiply:
                        d = Convert.ToDouble(LastValue) * Convert.ToDouble(Display);
                        CheckResult(d);
                        break;
                    case Operation.Percent:
                        //Note: this is different (but make more sense) then Windows calculator
                        d = (Convert.ToDouble(LastValue) * Convert.ToDouble(Display)) / 100.0F;
                        CheckResult(d);
                        break;
                    case Operation.Subtract:
                        d = Convert.ToDouble(LastValue) - Convert.ToDouble(Display);
                        CheckResult(d);
                        break;
                    case Operation.Sqrt:
                        d = Math.Sqrt(Convert.ToDouble(LastValue));
                        CheckResult(d);
                        break;
                    case Operation.OneX:
                        d = 1.0F / Convert.ToDouble(LastValue);
                        CheckResult(d);
                        break;
                    case Operation.Negate:
                        d = Convert.ToDouble(LastValue) * (-1.0F);
                        break;
                }
            }
            catch
            {
                d = 0;
                var parent = (Window)MyPanel.Parent;
                MessageBox.Show(parent, "Operation cannot be perfomed", parent.Title);
            }

            return d;
        }
        private static void CheckResult(double d)
        {
            if (Double.IsNegativeInfinity(d) || Double.IsPositiveInfinity(d) || Double.IsNaN(d))
                throw new Exception("Illegal value");
        }

        private void DisplayMemory()
        {
            if (_mem_val != String.Empty)
                BMemBox.Text = "Memory: " + _mem_val;
            else
                BMemBox.Text = "Memory: [empty]";
        }

        private void CalcResults()
        {
            if (LastOper == Operation.None)
                return;

            double d = Calc(LastOper);

            Result = d;

            Display = d.ToString();

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            DisplayBox.Text = Display == String.Empty ? "0" : Display;
        }

        private void AddToDisplay(char c)
        {
            if (c == '.')
            {
                if (Display.IndexOf('.', 0) >= 0)  //already exists
                    return;
                Display = Display + c;
            }
            else
            {
                if (c >= '0' && c <= '9')
                {
                    Display = Display + c;
                }
                else
                    if (c == '\b')  //backspace ?
                    {
                        if (Display.Length <= 1)
                            Display = String.Empty;
                        else
                        {
                            int i = Display.Length;
                            Display = Display.Remove(i - 1, 1);  //remove last char 
                        }
                    }

            }

            UpdateDisplay();
        }

        private void displayBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
