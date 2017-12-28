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
	*	class ApplicationEntry
	*       Purpose:	This class is a visual container for an ApplicationEntry. It contains the application
	*					name and buttons for manipulating old passwords and generating new ones.
	**********************************************************************************************************/
	public partial class ApplicationEntryDisplay : UserControl
	{
		public ApplicationEntryDisplay(string MasterPassword, ApplicationEntry Application)
		{
			InitializeComponent();

			this.Application = Application;

			uxApplicationName.Text = Application.ApplicationName;

			this.MasterPassword = MasterPassword;

			foreach(HashedPassword password in Application.Passwords)
			{
				HashedPasswordCompactDisplay newDisplay = new HashedPasswordCompactDisplay(password);

				newDisplay.DeletePasswordEvent += DeletePasswordEventListener;

				uxHashedPassContainer.Children.Add(newDisplay);
			}
		}
		/**********************************************************************************************************
		*	void DeletePasswordEventListener(HashedPassword Password)
		*       Purpose:	Custom event listener that listens for when a password is deleted. Updates the UI and
		*					removes the entry for the password.
		*       
		*		Parameters:
		*				HashedPassword Password
		*					The password to remove.
		**********************************************************************************************************/
		private void DeletePasswordEventListener(HashedPassword Password)
		{
			foreach (HashedPasswordCompactDisplay element in uxHashedPassContainer.Children)
			{
				if (element.Password == Password)
				{
					uxHashedPassContainer.Children.Remove(element);
					break;
				}
			}

			Application.Passwords.Remove(Password);
		}
		/**********************************************************************************************************
		*	void uxNewHashedPasswordButton_Click(...)
		*       Purpose:	Creates a new hashed password and adds it to the displayed passwords.
		*       
		*		Parameters:
		*				Standard MouseButtonEvent Parameters.
		**********************************************************************************************************/
		private void uxNewHashedPasswordButton_Click(object sender, RoutedEventArgs e)
		{
			HashedPassword newPassword = Application.GeneratePassword(MasterPassword);

			HashedPasswordCompactDisplay newPasswordDispay = new HashedPasswordCompactDisplay(newPassword);
			newPasswordDispay.DeletePasswordEvent += DeletePasswordEventListener;
			uxHashedPassContainer.Children.Add(newPasswordDispay);
		}
		/**********************************************************************************************************
		*	void uxApplicationName_MouseRightButtonDown(...)
		*       Purpose:	Allows the user to delete a website on right click of the name.
		*       
		*		Parameters:
		*				Standard MouseButtonEvent Parameters.
		**********************************************************************************************************/
		private void uxApplicationName_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure? You cannot recover the deleted application.", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				if (DeleteApplicationEvent != null)
					DeleteApplicationEvent(Application);
			}
		}
		/**********************************************************************************************************
		*	ApplicationEntry Application
		*       Purpose:	The ApplicationEntry this represents.
		**********************************************************************************************************/
		public ApplicationEntry Application { get; }
		/**********************************************************************************************************
		*	Action<ApplicationEntry> DeleteApplicationEvent
		*       Purpose:	Event that fires when this application is supposed to be deleted.
		**********************************************************************************************************/
		public Action<ApplicationEntry> DeleteApplicationEvent;
		/**********************************************************************************************************
		*	string MasterPassword
		*       Purpose:	The master password to use when generating new hashed passwords. This is safe to store
		*					here because this is a Visual element and will never be saved to the disk.
		**********************************************************************************************************/
		public string MasterPassword { get; }
	}
}
