using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	*	Window ApplicationCreationWindow
	*       Purpose:	Allows the user to specify the rules of generating passwords for the give application.
	**********************************************************************************************************/
	public partial class ApplicationCreationWindow : Window
	{
		/**********************************************************************************************************
		*	ApplicationCreationWindow
		*       Purpose:	Constructor.
		**********************************************************************************************************/
		public ApplicationCreationWindow()
		{
			InitializeComponent();
		}
		/**********************************************************************************************************
		*	void uxCreateApplicationButton_Click(...)
		*       Purpose:	Verifies that password rules are valid and, if they are, fires the event that says
		*					it is OK to generate passwords.
		*		
		*		Parameters:
		*			Standard click event parameters.
		**********************************************************************************************************/
		private void uxCreateApplicationButton_Click(object sender, RoutedEventArgs e)
		{
			if (uxApplicationName.Text.Length == 0)
			{
				MessageBox.Show("Application name required.");
				return;
			}

			ApplicationName = uxApplicationName.Text;

			Regex numberRegex = new Regex(@"^\d+$");

			if (numberRegex.IsMatch(uxPasswordLength.Text))
				PasswordLength = int.Parse(uxPasswordLength.Text);
			else
			{
				MessageBox.Show("Password Length requires a valid number.");
				return;
			}

			if (PasswordLength <= 0)
			{
				MessageBox.Show("Password length must be at least 1 character long.");
				return;
			}


			if (numberRegex.IsMatch(uxSeed.Text))
				Seed = int.Parse(uxSeed.Text);
			else
			{
				MessageBox.Show("Seed must be a valid number. Set this to 0 if you are unsure.");
				return;
			}

			bool anyOptions = false;

			LowerCaseAllowed = uxLowerCaseAllowed.IsChecked.Value;
			anyOptions |= LowerCaseAllowed;

			UpperCaseAllowed = uxUpperCaseAllowed.IsChecked.Value;
			anyOptions |= UpperCaseAllowed;

			NumbersAllowed = uxNumbersAllowed.IsChecked.Value;
			anyOptions |= NumbersAllowed;

			SpecialCharactersAllowed = uxSpecialCharactersAllowed.IsChecked.Value;
			anyOptions |= SpecialCharactersAllowed;

			if (anyOptions == false)
			{
				MessageBox.Show("At least one character option is required.");
				return;
			}

			CreateApplicationEvent(this);
			this.Close();

		}
		/**********************************************************************************************************
		*	string ApplicationName
		*       Purpose:	The name of the application.
		**********************************************************************************************************/
		public string ApplicationName;
		/**********************************************************************************************************
		*	int PasswordLength
		*       Purpose:	The length in characters to generate passwords at.
		**********************************************************************************************************/
		public int PasswordLength;
		/**********************************************************************************************************
		*	int Seed
		*       Purpose:	The shift value to use on passwords so that they generate uniquely.
		**********************************************************************************************************/
		public int Seed;
		/**********************************************************************************************************
		*	bool LowerCaseAllowed
		*       Purpose:	Specifies if lower case letters are allowed.
		**********************************************************************************************************/
		public bool LowerCaseAllowed;
		/**********************************************************************************************************
		*	bool UpperCaseAllowed
		*       Purpose:	Specifies if upper case letters are allowed.
		**********************************************************************************************************/
		public bool UpperCaseAllowed;
		/**********************************************************************************************************
		*	bool NumbersAllowed
		*       Purpose:	Specifies if numbers are allowed.
		**********************************************************************************************************/
		public bool NumbersAllowed;
		/**********************************************************************************************************
		*	bool SpecialCharactersAllowed
		*       Purpose:	Specifies if special characters are allowed.
		**********************************************************************************************************/
		public bool SpecialCharactersAllowed;

		/**********************************************************************************************************
		*	Action CreateApplicationEvent
		*       Purpose:	Custom event is fired after all inputs have been verified and set.
		**********************************************************************************************************/
		public Action<ApplicationCreationWindow> CreateApplicationEvent;
	}
}
