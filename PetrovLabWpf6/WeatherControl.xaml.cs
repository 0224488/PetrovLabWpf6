using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetrovLabWpf6
{
	// Разработать в WPF приложении класс WeatherControl, моделирующий погодную сводку – 
	// температуру(целое число в диапазоне от -50 до +50), направление ветра(строка), скорость ветра(целое число), 
	// наличие осадков(возможные значения: 0 – солнечно, 1 – облачно, 2 – дождь, 3 – снег.
	// Можно использовать целочисленное значение, либо создать перечисление enum). 
	// Свойство «температура» преобразовать в свойство зависимости.

	public enum WeatherConditions
	{
		Sunny = 0,  // солнечно
		Cloudy = 1, // облачно
		Rain = 2,   // дождь
		Snow = 3,   // снег
	}

	// Interaction logic for WeatherControl.xaml
	public partial class WeatherControl : UserControl
	{
		public WeatherControl()
		{
			InitializeComponent();

			MinValue = -50;
			MaxValue = 50;

			var conditions = new Dictionary<WeatherConditions, string>();
			conditions.Add(WeatherConditions.Sunny, "солнечно");
			conditions.Add(WeatherConditions.Cloudy, "пасмурно");
			conditions.Add(WeatherConditions.Rain, "дождь");
			conditions.Add(WeatherConditions.Snow, "снег");
			comboBoxCondition.ItemsSource = conditions;
		}

		public static readonly DependencyProperty TemperatureProperty =
			DependencyProperty.Register(nameof(Temperature), typeof(int), typeof(WeatherControl),
				new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTemperatureChanged),
					new CoerceValueCallback(OnCoerceTemperature)));

		// температура
		public int Temperature
		{
			get { return (int)GetValue(TemperatureProperty); }
			set { SetValue(TemperatureProperty, value); }
		}

		// направление ветра
		public string WindDirection
		{
			get { return textBoxWindDirection.Text; }
			set { textBoxWindDirection.Text = value; }
		}

		// скорость ветра
		public int WindSpeed
		{
			get
			{
				if(int.TryParse(textBoxWindSpeed.Text, out int speed))
					return speed;
				return 0;
			}
			set { textBoxWindSpeed.Text = value.ToString(); }
		}

		// наличие осадков
		public WeatherConditions WeatherCondition
		{
			get { return (WeatherConditions)comboBoxCondition.SelectedValue; }
			set { comboBoxCondition.SelectedValue = value; }
		}

		public string WeatherConditionsStr
		{
			get
			{
				if(comboBoxCondition.SelectedItem != null)
					return ((KeyValuePair<WeatherConditions, string>)comboBoxCondition.SelectedItem).Value;
				return string.Empty;
			}
		}

		// обработка события изменения температуры
		private static void OnTemperatureChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			WeatherControl control = sender as WeatherControl;
			control.OnTemperatureChanged((int)e.NewValue, (int)e.OldValue);
		}

		private static object OnCoerceTemperature(DependencyObject d, object value)
		{
			WeatherControl control = (WeatherControl)d;
			if(control != null && value is int)
				return control.OnCoerceTemperature((int)value);
			else
				return value;
		}

		// обработка события изменения температуры
		private void OnTemperatureChanged(int newValue, int oldValue)
		{
			if(newValue.ToString() != textBoxTemperature.Text)
				UpdateTextBox(newValue);
		}

		public int MaxValue { get; set; }
		public int MinValue { get; set; }

		private int OnCoerceTemperature(int value)
		{
			if(value < MinValue)
			{
				// выставляем нижний порог температуры
				UpdateTextBox(MinValue);
				return MinValue;
			}
			if(value > MaxValue)
			{
				// выставляем верхний порог температуры
				UpdateTextBox(MaxValue);
				return MaxValue;
			}
			return value;
		}

		private void UpdateTextBox(int value)
		{
			// запоминаем позицию курсора
			int position = textBoxTemperature.CaretIndex;

			textBoxTemperature.Text = value.ToString();

			// восстанавливаем позицию курсора
			textBoxTemperature.CaretIndex = position;
		}

		// обработка события измененения textBoxTemperature
		private void textBoxTemperature_TextChanged(object sender, TextChangedEventArgs e)
		{
			// если введено число меняем температуру
			if(int.TryParse(textBoxTemperature.Text, out int value))
			{
				Temperature = value;
			}
			else
			{
				// возвращаем последнее введеное значение
				UpdateTextBox(Temperature);
			}
		}
	}
}
