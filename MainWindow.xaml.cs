using AMS.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace App_Launcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);

		private Ini config = null;

		private void DragWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				ReleaseCapture();
				SendMessage(new WindowInteropHelper(this).Handle, 161, 2, 0);
			}
		}

		private void CloseWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				Application.Current.Shutdown();
		}

		private void MaximizeWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (WindowState == WindowState.Maximized)
				{
					WindowState = WindowState.Normal;
					Maximize.Content = "1";
				}
				else if (WindowState == WindowState.Normal)
				{
					WindowState = WindowState.Maximized;
					Maximize.Content = "2";
				}
			}
		}

		private void MinimizeWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				WindowState = WindowState.Minimized;
		}

		private void ViewInExplorer(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Process.Start("explorer.exe", Path.GetDirectoryName(config.Name));
			}
		}

		private void AddCategory(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				CategoryPrompt categoryPrompt = new CategoryPrompt
				{
					Owner = this,
				};
				categoryPrompt.Closed += (s, ee) =>
				{
					if (categoryPrompt.PromptSucceeded() && categoryPrompt.ConfirmClicked)
					{
						string categoryName = categoryPrompt.GetCategoryName();
						ListBoxItem categoryItem = CreateCategory(categoryName);

						Categories.Items.Add(categoryItem);
						programs.Add(categoryItem, new List<ListBoxItem>());

						string configInfo = string.Empty;
						using (StreamReader reader = new StreamReader(config.Name))
						{
							configInfo = reader.ReadToEnd();
							reader.Close();
						}
						using (StreamWriter writer = new StreamWriter(config.Name))
						{
							writer.WriteLine($"{configInfo}\n[{categoryName}]");
							writer.Close();
						}

						categoryItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SelectCategory);

						currentCategory = categoryItem;
						LoadPrograms();
					}
					else
					{
						MessageBox.Show("Could not add category because no information was specified.", "No Category Information Given",
							MessageBoxButton.OK, MessageBoxImage.Error);
					}
				};
				categoryPrompt.ShowDialog();
			}
		}

		private void AddProgram(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				ProgramPrompt programPrompt = new ProgramPrompt
				{
					Owner = this,
				};
				programPrompt.Closed += (s, ee) =>
				{
					if (programPrompt.PromptSucceeded() && programPrompt.ConfirmClicked)
					{
						(string, string) program = programPrompt.GetProgram();
						ListBoxItem programItem = CreateItem(program.Item1, program.Item2);

						Programs.Items.Add(programItem);
						List<ListBoxItem> programs = this.programs[currentCategory];
						programs.Add(programItem);
						this.programs[currentCategory] = programs;

						config.SetValue(((TextBlock)currentCategory.Content).Text, program.Item1, program.Item2);

						programItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SelectProgram);
					}
					else
					{
						MessageBox.Show("Could not add program because no information was specified.", "No Program Information Given",
							MessageBoxButton.OK, MessageBoxImage.Error);
					}
				};
				programPrompt.ShowDialog();
			}
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			if (WindowState == WindowState.Maximized)
			{
				Maximize.Content = "2";
				Maximize.ToolTip = "Restore";
				ContentWindow.Padding = new Thickness(8);
			}
			else if (WindowState == WindowState.Normal)
			{
				Maximize.Content = "1";
				Maximize.ToolTip = "Maximize";
				ContentWindow.Padding = new Thickness(0);
			}
		}

		private ListBoxItem CreateItem(string Title, string Path)
		{
			ListBoxItem listItem = new ListBoxItem();
			Grid itemLayout = new Grid();
			TextBlock itemTitle = new TextBlock();
			TextBlock itemPath = new TextBlock();
			RowDefinition row1 = new RowDefinition();
			RowDefinition row2 = new RowDefinition();

			row1.Height = new GridLength(20);
			row2.Height = new GridLength(15);

			itemLayout.Margin = new Thickness(10);
			itemLayout.RowDefinitions.Add(row1);
			itemLayout.RowDefinitions.Add(row2);

			itemTitle.Text = Title;
			itemTitle.FontFamily = new FontFamily("Segoe UI");
			itemTitle.FontSize = 14;
			itemTitle.VerticalAlignment = VerticalAlignment.Center;

			itemPath.Text = Path;
			itemPath.SetValue(Grid.RowProperty, 1);
			itemPath.FontFamily = new FontFamily("Segoe UI");
			itemPath.FontSize = 11;
			itemPath.VerticalAlignment = VerticalAlignment.Center;

			listItem.BorderThickness = new Thickness(0);
			listItem.Cursor = Cursors.Hand;
			listItem.Focusable = false;

			ContextMenu contextMenu = new ContextMenu();
			MenuItem modifyItem = new MenuItem();
			MenuItem removeItem = new MenuItem();

			modifyItem.PreviewMouseLeftButtonDown += (s, e) =>
			{
				ModifyProgram(listItem);
			};
			modifyItem.Loaded += (s, e) =>
			{
				modifyItem.Header = $"Modify '{itemTitle.Text}'";
			};
			modifyItem.FontFamily = new FontFamily("Segoe UI");
			modifyItem.FontSize = 12;
			modifyItem.Cursor = Cursors.Hand;

			removeItem.PreviewMouseLeftButtonDown += (s, e) =>
			{
				List<ListBoxItem> programs = this.programs[currentCategory];
				programs.Remove(listItem);

				this.programs[currentCategory] = programs;
				Programs.Items.Remove(listItem);

				config.RemoveEntry(((TextBlock)currentCategory.Content).Text, itemTitle.Text);
			};
			removeItem.Loaded += (s, e) =>
			{
				removeItem.Header = $"Remove '{itemTitle.Text}'";
			};
			removeItem.FontFamily = new FontFamily("Segoe UI");
			removeItem.FontSize = 12;
			removeItem.Cursor = Cursors.Hand;

			contextMenu.Items.Add(modifyItem);
			contextMenu.Items.Add(removeItem);

			itemLayout.Children.Add(itemTitle);
			itemLayout.Children.Add(itemPath);
			listItem.Content = itemLayout;
			listItem.ContextMenu = contextMenu;

			GC.Collect();

			return listItem;
		}

		private ListBoxItem CreateCategory(string Title)
		{
			ListBoxItem listItem = new ListBoxItem();
			TextBlock itemTitle = new TextBlock();

			itemTitle.FontFamily = new FontFamily("Segoe UI");
			itemTitle.FontSize = 16;
			itemTitle.Margin = new Thickness(10);

			listItem.BorderThickness = new Thickness(0);
			listItem.Cursor = Cursors.Hand;
			listItem.Focusable = false;

			ContextMenu contextMenu = new ContextMenu();
			MenuItem modifyItem = new MenuItem();
			MenuItem removeItem = new MenuItem();

			modifyItem.PreviewMouseLeftButtonDown += (s, e) =>
			{
				ModifyCategory(listItem);
			};
			modifyItem.Loaded += (s, e) =>
			{
				modifyItem.Header = $"Modify '{itemTitle.Text}'";
			};
			modifyItem.FontFamily = new FontFamily("Segoe UI");
			modifyItem.FontSize = 12;
			modifyItem.Cursor = Cursors.Hand;

			removeItem.PreviewMouseLeftButtonDown += (s, e) =>
			{
				programs.Remove(listItem);
				Categories.Items.Remove(listItem);

				currentCategory = (ListBoxItem)Categories.Items[0];
				LoadPrograms();

				config.RemoveSection(itemTitle.Text);

				File.Delete(config.Name);
				File.Create(config.Name).Close();

				foreach (ListBoxItem category in programs.Keys)
				{
					string section = ((TextBlock)category.Content).Text;

					foreach (ListBoxItem program in programs[category])
					{
						string name = ((TextBlock)((Grid)program.Content).Children[0]).Text;
						string path = ((TextBlock)((Grid)program.Content).Children[1]).Text;

						config.SetValue(section, name, path);
					}

					string currentIni = File.ReadAllText(config.Name);
					currentIni += category == programs.Keys.ElementAt(programs.Keys.Count - 1) ? "" : "\n";

					File.WriteAllText(config.Name, currentIni);
				}
			};
			removeItem.Loaded += (s, e) =>
			{
				removeItem.Header = $"Remove '{itemTitle.Text}'";
			};
			removeItem.FontFamily = new FontFamily("Segoe UI");
			removeItem.FontSize = 12;
			removeItem.Cursor = Cursors.Hand;

			contextMenu.Items.Add(modifyItem);
			contextMenu.Items.Add(removeItem);

			itemTitle.Text = Title;
			listItem.Content = itemTitle;
			listItem.ContextMenu = contextMenu;

			GC.Collect();

			return listItem;
		}

		private Dictionary<ListBoxItem, List<ListBoxItem>> programs = new Dictionary<ListBoxItem, List<ListBoxItem>>();
		private ListBoxItem currentCategory = null;

		private void LoadPrograms()
		{
			Programs.Items.Clear();

			foreach (ListBoxItem program in programs[currentCategory])
			{
				Programs.Items.Add(program);
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			if (!File.Exists(Environment.CurrentDirectory + "\\config.ini"))
			{
				using (StreamWriter writer = new StreamWriter(File.Create(Environment.CurrentDirectory + "\\config.ini")))
				{
					writer.WriteLine("[Basic Utilities]");
					writer.WriteLine("Notepad=notepad.exe");
					writer.WriteLine("WordPad=wordpad.exe");
					writer.WriteLine("Calculator=calc.exe");
					writer.WriteLine("");
					writer.WriteLine("[Text Editing]");
					writer.WriteLine("Notepad=notepad.exe");
					writer.WriteLine("WordPad=wordpad.exe");
					writer.WriteLine("");
					writer.WriteLine("[Mathematics]");
					writer.WriteLine("Calculator=calc.exe");
				}
			}

			config = new Ini(Environment.CurrentDirectory + "\\config.ini");

			string[] categories = config.GetSectionNames();

			foreach (string category in categories)
			{
				ListBoxItem categoryItem = CreateCategory(category);
				string[] programs = config.GetEntryNames(category);
				List<ListBoxItem> programItems = new List<ListBoxItem>();

				categoryItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SelectCategory);

				foreach (string program in programs)
				{
					ListBoxItem programItem = CreateItem(program, config.GetValue(category, program).ToString());
					programItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SelectProgram);

					programItems.Add(programItem);
				}

				this.programs.Add(categoryItem, programItems);
				Categories.Items.Add(categoryItem);
			}

			currentCategory = programs.Keys.ElementAt(0);
			LoadPrograms();
		}

		private void SelectProgram(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				ListBoxItem listItem = (ListBoxItem)sender;
				Grid itemLayout = (Grid)listItem.Content;
				TextBlock itemPath = (TextBlock)itemLayout.Children[1];
				string path = itemPath.Text;

				Process.Start(path);

				GC.Collect();
			}
		}

		private void SelectCategory(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				currentCategory = (ListBoxItem)sender;

				LoadPrograms();
			}
		}

		private void ModifyProgram(object sender)
		{
			Grid grid = (Grid)((ListBoxItem)sender).Content;
			string name = ((TextBlock)grid.Children[0]).Text;
			string path = ((TextBlock)grid.Children[1]).Text;

			ProgramPrompt programPrompt = new ProgramPrompt
			{
				Owner = this,
			};
			programPrompt.SetDefault(name, path);
			programPrompt.Closed += (s, ee) =>
			{
				if (programPrompt.PromptSucceeded() && programPrompt.ConfirmClicked)
				{
					(string, string) program = programPrompt.GetProgram();
					ListBoxItem programItem = CreateItem(program.Item1, program.Item2);

					List<ListBoxItem> programs = this.programs[currentCategory];

					int programIndex = 0;
					foreach (ListBoxItem tempItem in programs)
					{
						string name_ = ((TextBlock)((Grid)tempItem.Content).Children[0]).Text;
						string path_ = ((TextBlock)((Grid)tempItem.Content).Children[1]).Text;

						if (name_ == name && path_ == path)
						{
							programIndex = programs.IndexOf(tempItem);
						}
					}

					ChangeEntryName(config, ((TextBlock)currentCategory.Content).Text, name, program.Item1);
					config.SetValue(((TextBlock)currentCategory.Content).Text, program.Item1, program.Item2);

					programs[programIndex] = programItem;
					this.programs[currentCategory] = programs;
					LoadPrograms();

					programItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(SelectProgram);
				}
				else
				{
					MessageBox.Show("Could not add program because no information was specified.", "No Program Information Given",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			};
			programPrompt.ShowDialog();
		}

		private void ModifyCategory(object sender)
		{
			string title = ((TextBlock)((ListBoxItem)sender).Content).Text;

			CategoryPrompt categoryPrompt = new CategoryPrompt
			{
				Owner = this,
			};
			categoryPrompt.SetDefault(title);
			categoryPrompt.Closed += (s, ee) =>
			{
				if (categoryPrompt.PromptSucceeded() && categoryPrompt.ConfirmClicked)
				{
					string categoryName = categoryPrompt.GetCategoryName();

					string iniConfig = string.Empty;
					using (StreamReader reader = new StreamReader(config.Name))
					{
						iniConfig = reader.ReadToEnd();
						iniConfig = iniConfig.Replace($"[{title}]", $"[{categoryName}]");

						reader.Close();
					}

					File.Delete(config.Name);
					File.Create(config.Name).Close();

					using (StreamWriter writer = new StreamWriter(config.Name))
					{
						writer.Write(iniConfig);
					}

					List<(ListBoxItem, List<ListBoxItem>)> tempPrograms = new List<(ListBoxItem, List<ListBoxItem>)>();

					foreach (ListBoxItem category in programs.Keys.ToArray())
					{
						tempPrograms.Add((category, programs[category]));
						programs.Remove(category);
					}

					foreach ((ListBoxItem, List<ListBoxItem>) programSet in tempPrograms)
					{
						int programIndex = tempPrograms.IndexOf(programSet);
						(ListBoxItem, List<ListBoxItem>) tempSet = programSet;
						string tempCategory = ((TextBlock)tempSet.Item1.Content).Text;

						if (tempCategory == categoryName)
						{
							((TextBlock)tempSet.Item1.Content).Text = categoryName;
							tempPrograms[programIndex] = tempSet;

						}

						programs.Add(tempSet.Item1, tempSet.Item2);
					}

					((TextBlock)((ListBoxItem)sender).Content).Text = categoryName;

					//MessageBox.Show("The application needs to restart to apply changes to the category list.", "Application Restart Required",
					//	MessageBoxButton.OK, MessageBoxImage.Information);

					//Process.Start($"{Environment.CurrentDirectory}\\{Assembly.GetExecutingAssembly().GetName().Name}.exe");
					//Application.Current.Shutdown();
				}
				else
				{
					MessageBox.Show("Could not add category because no information was specified.", "No Category Information Given",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			};
			categoryPrompt.ShowDialog();
		}

		private void ChangeEntryName(Ini config, string section, string entry, string newEntry)
		{
			foreach (string entry_ in config.GetEntryNames(section))
			{
				bool newEntry_ = entry_ == entry;
				string value = config.GetValue(section, entry_).ToString();

				config.RemoveEntry(section, entry_);
				config.SetValue(section, newEntry_ ? newEntry : entry_, value);
			}
		}
	}
}
