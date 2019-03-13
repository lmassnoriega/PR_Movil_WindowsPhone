using TaskHelper.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TaskHelper.Core.Models;

// La plantilla de elemento Página básica está documentada en http://go.microsoft.com/fwlink/?LinkID=390556

namespace TaskHelper
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class FeedBack : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
		private int ParentProcedure = -1;
        public FeedBack()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Obtiene el <see cref="NavigationHelper"/> asociado a esta <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Obtiene el modelo de vista para esta <see cref="Page"/>.
        /// Este puede cambiarse a un modelo de vista fuertemente tipada.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Rellena la página con el contenido pasado durante la navegación.  Cualquier estado guardado se
        /// proporciona también al crear de nuevo una página a partir de una sesión anterior.
        /// </summary>
        /// <param name="sender">
        /// El origen del evento; suele ser <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Datos de evento que proporcionan tanto el parámetro de navegación pasado a
        /// <see cref="Frame.Navigate(Type, Object)"/> cuando se solicitó inicialmente esta página y
        /// un diccionario del estado mantenido por esta página durante una sesión
        /// anterior. El estado será null la primera vez que se visite una página.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter != null)
            {
                Step current = e.NavigationParameter as Step;
				ParentProcedure = current.procedure_id;
                DataContext = current.content2;
                CreateUI();
            }
        }

        /// <summary>
        /// Mantiene el estado asociado con esta página en caso de que se suspenda la aplicación o
        /// se descarte la página de la memoria caché de navegación.  Los valores deben cumplir los requisitos
        /// de serialización de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">El origen del evento; suele ser <see cref="NavigationHelper"/></param>
        /// <param name="e">Datos de evento que proporcionan un diccionario vacío para rellenar con
        /// un estado serializable.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Registro de NavigationHelper

        /// <summary>
        /// Los métodos proporcionados en esta sección se usan simplemente para permitir
        /// que NavigationHelper responda a los métodos de navegación de la página.
        /// <para>
        /// Debe incluirse lógica específica de página en los controladores de eventos para 
        /// <see cref="NavigationHelper.LoadState"/>
        /// y <see cref="NavigationHelper.SaveState"/>.
        /// El parámetro de navegación está disponible en el método LoadState 
        /// junto con el estado de página mantenido durante una sesión anterior.
        /// </para>
        /// </summary>
        /// <param name="e">Proporciona los datos para el evento y los métodos de navegación
        /// controladores que no pueden cancelar la solicitud de navegación.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region Methods

        private void CreateUI()
        {
			var step = DataContext as StepInfo;
            int type = Convert.ToInt32(step.Fields.First().field_type);
			CreateGridRows(2);
			switch (type)
            {
                case 0:
                    CreateEnumTemplate();
                    break;
                case 1:
                    CreateBooleanTemplate();
                    break;
                case 2:
                    CreateNumericTemplate();
                    break;
				case 3:
					CreateMessageTemplate();
					break;
                default:
                    break;
            }
        }

		private void CreateGridRows(int rowCount)
		{
			LayoutRoot.Children.Clear();
			LayoutRoot.RowDefinitions.Clear();
			for (int i = 0; i < rowCount; i++)
			{
				LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			}
		}

        private void CreateBooleanTemplate()
        {
            var process = DataContext as StepInfo;

			Binding questBind = new Binding() { Path = new PropertyPath("caption"), Mode = BindingMode.OneWay, Source = process.Fields.First() };
			TextBlock question = new TextBlock() { Style = App.Current.Resources["PhoneQuestionStyle"] as Style };
			question.SetBinding(TextBlock.TextProperty, questBind);
			Grid.SetRow(question, 0);
			LayoutRoot.Children.Add(question);

			ToggleSwitch selection = new ToggleSwitch() { Style = App.Current.Resources["ToogleSelectionStyle"] as Style };
            Grid.SetRow(selection, 1);
            LayoutRoot.Children.Add(selection);


        } //Ready

        private void CreateEnumTemplate()
        {
            var process = DataContext as StepInfo;

			Binding questBind = new Binding() { Path = new PropertyPath("caption"), Mode = BindingMode.OneWay, Source= process.Fields.First() };
            TextBlock question = new TextBlock() { Style = App.Current.Resources["PhoneQuestionStyle"] as Style };
            question.SetBinding(TextBlock.TextProperty, questBind);
            Grid.SetRow(question, 0);
            LayoutRoot.Children.Add(question);

            Binding optionsBind = new Binding() { Path = new PropertyPath("possible_values"), Mode = BindingMode.OneWay, Source= process.Fields.First() };
            ComboBox options = new ComboBox() { Style = App.Current.Resources["ComboBoxSelectionStyle"] as Style };
            options.SetBinding(ComboBox.ItemsSourceProperty, optionsBind);
            Grid.SetRow(options, 1);
            LayoutRoot.Children.Add(options);

        } // Ready

        private void CreateNumericTemplate()
        {
            var process = DataContext as StepInfo;
						
			Binding questBind = new Binding() { Path = new PropertyPath("caption"), Mode = BindingMode.OneWay, Source = process.Fields.First() };
			TextBlock question = new TextBlock() { Style = App.Current.Resources["PhoneQuestionStyle"] as Style };
			question.SetBinding(TextBlock.TextProperty, questBind);
			Grid.SetRow(question, 0);
			LayoutRoot.Children.Add(question);

			TextBox answer = new TextBox() { Style = App.Current.Resources["TextBoxInputStyle"] as Style, InputScope = new InputScope { Names = { new InputScopeName { NameValue = InputScopeNameValue.Number } } } };
			Grid.SetRow(answer, 1);
			LayoutRoot.Children.Add(answer);
		} // Ready

		private void CreateMessageTemplate()
		{
			var process = DataContext as StepInfo;

			TextBlock question = new TextBlock() { Style = App.Current.Resources["PhoneQuestionStyle"] as Style };
			question.Text = "Resultados del procedimiento";
			Grid.SetRow(question, 0);
			LayoutRoot.Children.Add(question);

			Binding message = new Binding() { Path = new PropertyPath("caption"), Mode = BindingMode.OneWay, Source = process.Fields.First() };
			TextBlock label = new TextBlock() { Style = App.Current.Resources["PhoneInstructionStyle"] as Style };
			label.SetBinding(TextBlock.TextProperty, message);
			Grid.SetRow(label, 1);
			LayoutRoot.Children.Add(label);

			Navigate.Visibility = Visibility.Collapsed;
		} // Ready

		private void NextQuestion(object sender, RoutedEventArgs e)
		{
			var process = DataContext as StepInfo;
			int FieldType = Convert.ToInt32(process.Fields.FirstOrDefault().field_type);
			string value = "";
			//Obtain Value associated with field type and control.
			switch (FieldType)
			{
				case 0:
					foreach (var child in LayoutRoot.Children)
					{
						try
						{
							ComboBox cmb = child as ComboBox;
							value = cmb.SelectedItem.ToString();
						}
						catch (Exception)
						{

						}
					}
					break;
				case 1:
					foreach (var element in LayoutRoot.Children)
					{
						try
						{
							ToggleSwitch sw = element as ToggleSwitch;
							value = sw.IsOn.ToString();
						}
						catch (Exception)
						{

						}
					}
					break;
				case 2:
					foreach (var control in LayoutRoot.Children)
					{
						try
						{
							TextBox txt = control as TextBox;
							value = txt.Text;
						}
						catch (Exception)
						{

						}
					}
					break;
				default:
					break;
			}

			//Process conditional to execute branch to next step.
			
			bool match = false; //Once a brancch is evaluated successfully it stops!
			Step following = null;
			foreach (var decision in process.Decisions)
			{
				var item = decision.branch.FirstOrDefault();
				switch (FieldType)
				{
					case 0:
					case 1:
						if (value.Equals(item.value))
						{
							match = true;
						}
						break;
					case 2:
						switch (item.comparison_type)
						{
							case "=":
								if (Convert.ToDouble(value).CompareTo(Convert.ToDouble(item.value)) == 0)
								{
									match = true;
								}
								break;
							case ">":
								if (Convert.ToDouble(value).CompareTo(Convert.ToDouble(item.value)) > 0)
								{
									match = true;
								}
								break;
							case "<":
								if (Convert.ToDouble(value).CompareTo(Convert.ToDouble(item.value)) < 0)
								{
									match = true;
								}
								break;
							default:
								break;
						}
						break;
					default:
						break;
				}
				if (match)
				{
					following = App.ViewModel.Steps.Where(next => next.step_id == Convert.ToInt32(decision.go_to_step) && next.procedure_id == ParentProcedure).FirstOrDefault();
					break;
				}
			}
			
			if (match)
			{
				Frame.Navigate(typeof(FeedBack), following);
			}

		}

		#endregion
	}
}
