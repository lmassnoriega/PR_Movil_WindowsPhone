using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPTaller1.Resources;

namespace WPTaller1
{
    public partial class MainPage : PhoneApplicationPage
    {
        public double operator1 = Double.NaN;
        public double operator2 = Double.NaN;
        private double memory = Double.NaN;
        private bool waitingSecond = false;
        private string LastOper;
        private bool EqualPressed = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Código de ejemplo para traducir ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Type(object sender, RoutedEventArgs e)
        {
            String code = ((Button)sender).Content.ToString();
            if (waitingSecond)
            {
                Results.Text = "0";
            }
            if (code.Equals("."))
            {
                if (!Results.Text.Contains(code))
                {
                    Results.Text += code;
                }
            }
            else
            {
                if (code.Equals("0"))
                {
                    if (!Results.Text.StartsWith("0"))
                    {
                        Results.Text += code;
                    }
                }
                else
                {
                    if (Results.Text.StartsWith("0"))
                    {
                        Results.Text = code;
                    }
                    else
                    {
                        Results.Text += code;
                    }
                }
            }
        }

        private void Erase(object sender, RoutedEventArgs e)
        {
            if (Results.Text.Length != 1)
            {
                Results.Text = Results.Text.Remove(Results.Text.Length - 1);
            }
            else
            {
                Results.Text = "0";
            }
        }

        private void Operate(object sender, RoutedEventArgs e)
        {
            String operation = ((Button)sender).Content.ToString();
            switch (operation)
            {
                case "MC":
                    memory = Double.NaN;
                    break;
                case "MR":
                    if (!Double.IsNaN(memory))
                    {
                        Results.Text = memory.ToString();
                    }
                    else
                    {
                        Results.Text = "0";
                    }
                    break;
                case "M+":
                    try
                    {
                        if (Double.IsNaN(memory))
                        {
                            memory = Convert.ToDouble(Results.Text);
                        }
                        else
                        {
                            memory += Convert.ToDouble(Results.Text);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case "M-":
                    try
                    {
                        if (Double.IsNaN(memory))
                        {
                            memory = Convert.ToDouble(Results.Text) * (-1);
                        }
                        else
                        {
                            memory -= Convert.ToDouble(Results.Text);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case "%":
                    if (!Double.IsNaN(operator1))
                    {
                        operator2 = operator1 * Convert.ToDouble(Results.Text) / 100;
                        Results.Text = operator2.ToString();
                    }
                    else
                    {
                        Results.Text = "0";
                    }
                    break;
                case "-/+":
                    if (Results.Text.StartsWith("-"))
                    {
                        Results.Text = Results.Text.Replace("-", "");
                    }
                    else
                    {
                        if (!Results.Text.StartsWith("0"))
                        {
                            Results.Text = "-" + Results.Text;
                        }
                    }
                    break;
                case "1/x":
                    try
                    {
                        double value = Convert.ToDouble(Results.Text);
                        if (value != 0)
                        {
                            Results.Text = (1 / value).ToString();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("No se puede dividir por Cero");
                    }
                    break;
                case "sqrt":
                    try
                    {
                        double value = Convert.ToDouble(Results.Text);
                        if (value >= 0)
                        {
                            Results.Text = Math.Sqrt(value).ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se puede hallar raices negativas");
                        }
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "C":
                    operator1 = operator2 = memory = Double.NaN;
                    Results.Text = "0";
                    waitingSecond = false;
                    break;
                default:
                    break;
            }
        }

        private void DoMath(object sender, RoutedEventArgs e)
        {
            String op = ((Button)sender).Content.ToString();

            if (!waitingSecond)
            {
                waitingSecond = true;
                operator1 = Convert.ToDouble(Results.Text);
            }
            else
            {
                if (EqualPressed)
                {
                    EqualPressed = false;
                    operator1 = Convert.ToDouble(Results.Text);
                    operator2 = Double.NaN;
                }
                else
                {
                    operator2 = Convert.ToDouble(Results.Text);
                    operator1 = ReturnResult(operator1, operator2, LastOper);
                    Results.Text = operator1.ToString();
                }
            }
            LastOper = op;
        }

        private double ReturnResult(double oper1, double oper2, String operation)
        {
            switch (operation)
            {
                case "+":
                    return operator1 + operator2;
                case "-":
                    return operator1 - operator2;
                case "X":
                    return operator1 * operator2;
                case "/":
                    try
                    {
                        return operator1 / operator2;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("No se puede dividir entre cero");
                        return operator1;
                    }
                default:
                    return 0;
            }
        }

        private void Finalize(object sender, RoutedEventArgs e)
        {
            if (waitingSecond)
            {
                if (Double.IsNaN(operator2))
                {
                    operator2 = Convert.ToDouble(Results.Text);

                }
                operator1 = ReturnResult(operator1, operator2, LastOper);
                Results.Text = operator1.ToString();
                EqualPressed = true;
            }
        }

        // Código de ejemplo para compilar una ApplicationBar traducida
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Establecer ApplicationBar de la página en una nueva instancia de ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crear un nuevo botón y establecer el valor de texto en la cadena traducida de AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crear un nuevo elemento de menú con la cadena traducida de AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}