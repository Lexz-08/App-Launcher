using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace App_Launcher
{
	/// <summary>
	/// Interaction logic for CategoryPrompt.xaml
	/// </summary>
	public partial class ProgramPrompt : Window
	{
		public ProgramPrompt()
		{
			InitializeComponent();
		}

		public bool ConfirmClicked { get; private set; } = false;

		public (string, string) GetProgram()
		{
			return (ProgramName.Text, ProgramPath.Text);
		}

		public bool PromptSucceeded()
		{
			return !string.IsNullOrEmpty(ProgramName.Text) && !string.IsNullOrEmpty(ProgramPath.Text);
		}

		public void SetDefault(string Name, string Path)
		{
			ProgramName.Text = Name;
			ProgramPath.Text = Path;
		}

		private void ConfirmProgram(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (string.IsNullOrEmpty(ProgramName.Text))
				{
					MessageBox.Show("New program requires a name to be added.", "Program Name Required",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else if (string.IsNullOrEmpty(ProgramPath.Text))
				{
					MessageBox.Show("New program requires a path to be added.", "Program Path Required",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else if (string.IsNullOrEmpty(ProgramName.Text) && string.IsNullOrEmpty(ProgramPath.Text))
				{
					MessageBox.Show("New program requires a name and path to be added.", "Program Name/Path Required",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					ConfirmClicked = true;
					Close();
				}
			}
		}

		private void SelectProgram(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				OpenFileDialog ofd = new OpenFileDialog
				{
					Title = "Please select a program to add...",
					Filter = "Program|*.exe",
				};

				if (ofd.ShowDialog() == true)
				{
					string programPath = ofd.FileName;
					ProgramPath.Text = programPath;
				}
			}
		}
	}
}
