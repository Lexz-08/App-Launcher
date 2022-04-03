using System.Windows;
using System.Windows.Input;

namespace App_Launcher
{
	/// <summary>
	/// Interaction logic for CategoryPrompt.xaml
	/// </summary>
	public partial class CategoryPrompt : Window
	{
		public CategoryPrompt()
		{
			InitializeComponent();
		}

		public bool ConfirmClicked { get; private set; } = false;

		public string GetCategoryName()
		{
			return CategoryName.Text;
		}

		public bool PromptSucceeded()
		{
			return !string.IsNullOrEmpty(CategoryName.Text);
		}

		public void SetDefault(string Title)
		{
			CategoryName.Text = Title;
		}

		private void ConfirmCategory(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (string.IsNullOrEmpty(CategoryName.Text))
				{
					MessageBox.Show("New category requires a name to be added.", "Category Name Required",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					ConfirmClicked = true;
					Close();
				}
			}
		}
	}
}
