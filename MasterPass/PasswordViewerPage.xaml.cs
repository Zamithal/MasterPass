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

namespace MasterPass
{
	/**********************************************************************************************************
	*	Page PasswordViewerPage
	*       Purpose:	This is the main page of the application. From here you can see each application and
	*					their respective passwords. There is options to add new applications as well as
	*					generate new passwords.
	**********************************************************************************************************/
	public partial class PasswordViewerPage : Page
	{
		/**********************************************************************************************************
		*	PasswordViewer(string MasterPassword), UserData Data)
		*       Purpose:	Constructs the display object from an existing set of data.
		*       
		*       Parameters:
		*			string MasterPassword
		*				The master password to use for all hashed passwords.
		*			UserData Data
		*				The location to store data in. The existing data to load.
		**********************************************************************************************************/
		public PasswordViewerPage(string MasterPassword, UserData Data)
		{
			InitializeComponent();

			this.MasterPassword = MasterPassword;

			this.Data = Data;

			foreach(ApplicationEntry application in this.Data.Applications)
			{
				ApplicationEntryDisplay newApplicationDisplay = new ApplicationEntryDisplay(this.MasterPassword, application);
				newApplicationDisplay.DeleteApplicationEvent += DeleteApplicationEventListener;

				uxApplicationEntryContainer.Children.Add(newApplicationDisplay);
			}

		}
		/**********************************************************************************************************
		*	void uxNewApplicationButton_Click(...)
		*       Purpose:	Creates a new application when the button is clicked.
		*       
		*       Parameters:
		*			Standard click event parameters.
		**********************************************************************************************************/
		private void uxNewApplicationButton_Click(object sender, RoutedEventArgs e)
		{
			ApplicationCreationWindow newAppWindow = new ApplicationCreationWindow();

			newAppWindow.CreateApplicationEvent += CreateApplicationEventListener;

			newAppWindow.Show();
		}
		/**********************************************************************************************************
		*	void CreateApplicationEventListener(ApplicationCreationWindow CreatingWindow)
		*       Purpose:	Event listener to be fired when an application is to be created. Processes the values
		*					from the creating window and creates a new application from it.
		*       
		*       Parameters:
		*			ApplicationCreationWindow CreatingWindow
		*				The window that issued the creation event.
		**********************************************************************************************************/
		private void CreateApplicationEventListener(ApplicationCreationWindow CreatingWindow)
		{
			ApplicationEntry newApplication
				= new ApplicationEntry(CreatingWindow.ApplicationName, CreatingWindow.PasswordLength,
										Convert.ToByte(CreatingWindow.Seed % 255), CreatingWindow.LowerCaseAllowed,
										CreatingWindow.UpperCaseAllowed, CreatingWindow.NumbersAllowed,
										CreatingWindow.SpecialCharactersAllowed);

			Data.Applications.Add(newApplication);

			ApplicationEntryDisplay newApplicationDisplay = new ApplicationEntryDisplay(MasterPassword, newApplication);
			newApplicationDisplay.DeleteApplicationEvent += DeleteApplicationEventListener;

			uxApplicationEntryContainer.Children.Add(newApplicationDisplay);
		}
		/**********************************************************************************************************
		*	void DeleteApplicationEventListener(ApplicationEntry Application)
		*       Purpose:	Listens for the event to delete an application. Removes that application from the
		*					display and removes it from the user's data.
		*					
		*		Parameters:
		*			ApplicationEntry Application
		*				The application to remove.
		**********************************************************************************************************/
		private void DeleteApplicationEventListener(ApplicationEntry Application)
		{
			foreach (ApplicationEntryDisplay element in uxApplicationEntryContainer.Children)
			{
				if (element.Application == Application)
				{
					uxApplicationEntryContainer.Children.Remove(element);
					break;
				}
			}

			Data.Applications.Remove(Application);
		}
		/**********************************************************************************************************
		*	UserData Data
		*       Purpose:	Represents all the data about the user's applications and hashed passwords. This does
		*					not contain the master password in a non-hashed form and is safe to write to disk.
		**********************************************************************************************************/
		public UserData Data { get; }
		/**********************************************************************************************************
		*	string MasterPassword
		*       Purpose:	The master password to use when generating new hashed passwords. This is safe to store
		*					here because this is a Visual element and will never be saved to the disk.
		**********************************************************************************************************/
		public string MasterPassword { get; }
	}
}
