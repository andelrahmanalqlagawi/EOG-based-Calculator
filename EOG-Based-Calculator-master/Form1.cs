using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EOG_based_calculator
{
    public partial class Form1 : Form
    {
        private List<Guna2Button> buttonsList;
        private Dictionary<string, Guna2Button> buttonNames;
        private Dictionary<Guna2Button, Dictionary<string, Guna2Button>> buttonAdjacency;
        private Guna2Button currentButton;

        private string currentOperator;
        private string firstOperand;
        private bool operatorEntered;


        public Form1()
        {
            InitializeComponent();
            InitializeButtonList();
            InitializeButtonAdjacency();
            AddButtonEventHandlers();
            currentButton = guna2Button8;
        }

        private void InitializeButtonList()
        {
            buttonsList = new List<Guna2Button>
            {
                guna2Button1, //1
                guna2Button2, //5
                guna2Button3, //7
                guna2Button4, //8
                guna2Button5, //6
                guna2Button6, //4
                guna2Button7, //3
                guna2Button8, //red button
                guna2Button9, // +
                guna2Button10, // /
                guna2Button11, // x
                guna2Button12, //9
                guna2Button13, // -
                guna2Button14, //2
                guna2Button15, //clear button
                guna2Button16, //exit button
                guna2Button17 //0
            };

            buttonNames = buttonsList.ToDictionary(button => button.Name, button => button);
        }

        private void InitializeButtonAdjacency()
        {
            buttonAdjacency = new Dictionary<Guna2Button, Dictionary<string, Guna2Button>>
            {
                // Define neighbors for each button
                { guna2Button1, new Dictionary<string, Guna2Button> { { "Down", guna2Button9 } } },
                { guna2Button2, new Dictionary<string, Guna2Button> { { "Right", guna2Button11 } } },
                { guna2Button3, new Dictionary<string, Guna2Button> { { "Right", guna2Button10 } } },
                { guna2Button4, new Dictionary<string, Guna2Button> { { "Left", guna2Button10 } } },
                { guna2Button5, new Dictionary<string, Guna2Button> { { "Right", guna2Button9 } } },
                { guna2Button6, new Dictionary<string, Guna2Button> { { "Left", guna2Button13 } } },
                { guna2Button7, new Dictionary<string, Guna2Button> { { "Up", guna2Button10 } } },
                { guna2Button8, new Dictionary<string, Guna2Button> { { "Up", guna2Button9 }, { "Left", guna2Button11 }, { "Down", guna2Button10 }, { "Right", guna2Button13 } } },
                { guna2Button9, new Dictionary<string, Guna2Button> { { "Up", guna2Button1 }, { "Right", guna2Button12 }, { "Left", guna2Button5 } } },
                { guna2Button10, new Dictionary<string, Guna2Button> { { "Up", guna2Button8 }, { "Left", guna2Button3 }, { "Down", guna2Button7 }, { "Right", guna2Button4 } } },
                { guna2Button11, new Dictionary<string, Guna2Button> { { "Up", guna2Button15 }, { "Left", guna2Button2 }, { "Down", guna2Button14 }, { "Right", guna2Button8 } } },
                { guna2Button12, new Dictionary<string, Guna2Button> { { "Left", guna2Button9 } } },
                { guna2Button13, new Dictionary<string, Guna2Button> { { "Up", guna2Button16 }, { "Left", guna2Button8 }, { "Down", guna2Button17 }, { "Right", guna2Button6 } } },
                { guna2Button14, new Dictionary<string, Guna2Button> { { "Up", guna2Button11 } } },
                { guna2Button15, new Dictionary<string, Guna2Button> { { "Down", guna2Button11 } } },
                { guna2Button16, new Dictionary<string, Guna2Button> { { "Down", guna2Button13 } } },
                { guna2Button17, new Dictionary<string, Guna2Button> { { "Up", guna2Button13 } } },
            };
        }

        private void AddButtonEventHandlers()
        {
            foreach (var button in buttonsList)
            {
                button.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Guna2Button clickedButton = sender as Guna2Button;

            if (clickedButton != null)
            {
                string buttonText = clickedButton.Text;

                if (buttonText == "clear")
                {
                    guna2TextBox1.Text = string.Empty;
                    ResetCalculator();
                }
                else if (buttonText == "exit")
                {
                    Application.Exit();
                }
                else if (IsOperator(buttonText))
                {
                    currentOperator = buttonText;
                    operatorEntered = true;
                    guna2TextBox1.Text += buttonText;
                }
                else
                {
                    if (operatorEntered)
                    {
                        // Second digit is entered, perform calculation
                        guna2TextBox1.Text += buttonText;
                        guna2TextBox1.Text = Calculate(firstOperand, currentOperator, buttonText);
                        ResetCalculator();
                    }
                    else
                    {
                        // First digit is entered
                        firstOperand = buttonText;
                        guna2TextBox1.Text += buttonText;
                    }
                }
            }
        }

        private void ResetCalculator()
        {
            currentOperator = null;
            firstOperand = null;
            operatorEntered = false;
            currentButton = guna2Button8;
        }
        private bool IsOperator(string text)
        {
            return text == "+" || text == "-" || text == "x" || text == "/";
        }

        private string Calculate(string operand1, string operatorText, string operand2)
        {
            double num1 = Convert.ToDouble(operand1);
            double num2 = Convert.ToDouble(operand2);
            double result = 0;

            switch (operatorText)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "x":
                    result = num1 * num2;
                    break;
                case "/":
                    if (num2 != 0)
                        result = num1 / num2;
                    else
                        return "Error";
                    break;
            }

            return result.ToString();
        }

        public async Task SimulateButtonClicksAsync(List<int> sequence, Dictionary<int, string> classNames)
        {
            currentButton = guna2Button8;

            foreach (var direction in sequence)
            {
                var className = classNames[direction];
                currentButton = Move(currentButton, className);
                SimulateClick(currentButton, className);
                await Task.Delay(2000);
            }
        }

        private Guna2Button Move(Guna2Button currentButton, string direction)
        {
            if (buttonAdjacency[currentButton].TryGetValue(direction, out Guna2Button nextButton))
            {
                return nextButton;
            }

            return currentButton; // Stay in the same place if no valid move
        }

        private async void SimulateClick(Guna2Button button, string direction)
        {
            bool blink = false;
            if (button != null)
            {
                // Simulate button click if the direction is "Blink"
                if (direction == "Blink")
                {
                    button.PerformClick();
                    var btn_color = button.FillColor;
                    button.FillColor = Color.Violet; // Example color

                    // Use a timer to reset the color after a short delay
                    await Task.Delay(1000); // Keep the color changed for 1 second
                    button.FillColor = btn_color;

                    blink = true;

                    currentButton = guna2Button8;
                  /*  var origin_btn = currentButton.FillColor;
                    currentButton.FillColor = Color.Gold;
                    await Task.Delay(1000);
                    currentButton.FillColor = origin_btn;*/
                }

                if (blink)
                {
                    //do nothing

                }
                else
                {
                    // Change color to indicate hover
                    var originalColor = button.FillColor;
                    button.FillColor = Color.Red; // Example color

                    // Use a timer to reset the color after a short delay
                    await Task.Delay(1000); // Keep the color changed for 1 second
                    button.FillColor = originalColor;
                }

               
            }
        }

   
    }
}
