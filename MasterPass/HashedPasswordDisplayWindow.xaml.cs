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
using System.Windows.Shapes;

namespace MasterPass
{
	/**********************************************************************************************************
	*	Window HashedPasswordDisplayWindow
	*       Purpose:	Displays additional information about a hashed password.
	**********************************************************************************************************/
	public partial class HashedPasswordDisplayWindow : Window
	{
		/**********************************************************************************************************
		*	HashedPasswordDisplayWindow(HashedPassword) Password
		*       Purpose:	Constructor.
		*       
		*       Parameters:
		*			HashedPassword Password
		*				The Password to display.
		**********************************************************************************************************/
		public HashedPasswordDisplayWindow(HashedPassword Password)
		{
			InitializeComponent();

			uxPasswordDisplay.Text = Password.Password;
			uxDateDisplay.Text = Password.DateCreated.ToString();
			uxSeedDisplay.Text = Password.Seed.ToString();
		}
		/**********************************************************************************************************
		*	void uxDelete_Click(...)
		*       Purpose:	Prompts the user for deletion confirmation.
		*       
		*       Parameters:
		*			Standard click parameters.
		**********************************************************************************************************/
		private void uxDelete_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure? You cannot recover the deleted password.", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				DeletePasswordEvent();
				this.Close();
			}
		}
		/**********************************************************************************************************
		*	Action DeletePasswordEvent
		*       Purpose:	Informs listeners that the password should be deleted.
		**********************************************************************************************************/
		public event Action DeletePasswordEvent;
	}
}
