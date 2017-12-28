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
	*	UserControl HashedPasswordCompactDisplay
	*       Purpose:	This class takes in a HashedPassword and displays its date in the center of a button.
	*					the button can be left or right clicked with the following effects:
	*						Left Click:
	*							Copies the password to the clipboard.
	*						Right Click:
	*							Opens up the details of the password.
	**********************************************************************************************************/
	public partial class HashedPasswordCompactDisplay : UserControl
	{
		/**********************************************************************************************************
		*	HashedPasswordCompactDisplay(HashedPassword Password)
		*       Purpose:	Constructor.
		*       
		*		Parameters:
		*				HashedPassword Password
		*					The password to display.
		**********************************************************************************************************/
		public HashedPasswordCompactDisplay(HashedPassword Password)
		{
			InitializeComponent();

			this.Password = Password;

			StringBuilder sb = new StringBuilder();

			sb.Append(this.Password.DateCreated.Month.ToString());
			sb.Append('/');
			sb.Append(this.Password.DateCreated.Day.ToString());
			sb.Append('/');
			sb.Append(this.Password.DateCreated.Year.ToString());

			uxDateCreated.Text = sb.ToString();
		}
		/**********************************************************************************************************
		*	void uxHashButton_MouseLeftButtonDown(...)
		*       Purpose:	Copies the hashed password to the user's clipboard when they left click.
		*       
		*		Parameters:
		*				Standard MouseButtonEvent Parameters.
		**********************************************************************************************************/
		private void uxHashButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Clipboard.SetText(Password.Password);
		}
		/**********************************************************************************************************
		*	void uxHashButton_MouseRightButtonDown(...)
		*       Purpose:	Opens up an advanced display of the hashed password's information. and options.
		*       
		*		Parameters:
		*				Standard MouseButtonEvent Parameters.
		**********************************************************************************************************/
		private void uxHashButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			HashedPasswordDisplayWindow openWindow = new HashedPasswordDisplayWindow(Password);

			openWindow.Show();

			openWindow.DeletePasswordEvent += OpenWindowDeletePasswordEventListener;
		}
		/**********************************************************************************************************
		*	void OpenWindowDeletePasswordEventListener()
		*       Purpose:	Event listener that listens for a delete request from the advanced display window and
		*					sends the call to its own event for other listeners.
		**********************************************************************************************************/
		private void OpenWindowDeletePasswordEventListener()
		{
			if (DeletePasswordEvent != null)
				DeletePasswordEvent(Password);
		}

		/**********************************************************************************************************
		*	HashedPassword Password
		*       Purpose:	The password this display represents.
		**********************************************************************************************************/
		public HashedPassword Password { get; }

		/**********************************************************************************************************
		*	Action<HashedPasswordCompactDisplay> DeletePasswordEvent
		*       Purpose:	This is an event that fires when this object is to be deleted.
		**********************************************************************************************************/
		public Action<HashedPassword> DeletePasswordEvent;
	}
}
