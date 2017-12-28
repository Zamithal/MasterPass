using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
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
	*	Window MainWindow
	*       Purpose:	Functions as the main window for the entire application. Performs various functions
	*					depending on the loaded page.
	**********************************************************************************************************/
	public partial class MainWindow : Window
	{
		/**********************************************************************************************************
		*	MainWindow
		*       Purpose:	Constructor.
		**********************************************************************************************************/
		public MainWindow()
		{
			InitializeComponent();

			LaunchPage loginPage = new LaunchPage();

			loginPage.LoginEvent += LoginEventListener;

			uxMainFrame.Navigate(loginPage);
		}
		/**********************************************************************************************************
		*	void LoginEventListener(string MasterPassword, UserData Data)
		*       Purpose:	Listens for a successful login. Sets the data used for all passwords and launches the
		*					password viewer page.
		*					
		*		Parameters:
		*			string MasterPassword
		*				The password used to log in.
		*			UserData Data
		*				All data relating to the user's password that can be safely stored on a disk.
		**********************************************************************************************************/
		private void LoginEventListener(string MasterPassword, UserData Data)
		{
			this.MasterPassword = MasterPassword;
			this.Data = Data;

			PasswordViewerPage passwordPage = new PasswordViewerPage(this.MasterPassword, this.Data);

			uxMainFrame.Navigate(passwordPage);

			Closing += MainWindow_Closing;

			NavigationCommands.BrowseBack.InputGestures.Clear();
			NavigationCommands.BrowseForward.InputGestures.Clear();
		}

		/**********************************************************************************************************
		*	void MainWindow_Closing
		*       Purpose:	Fires when the window is to be closed. Prompts the user to save changes.
		*					Encrypts the file using AES using a SHA-3 key derived from the MasterPassword.
		**********************************************************************************************************/
		void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			MessageBoxResult save = MessageBox.Show("Save your data?", "Save Prompt", MessageBoxButton.YesNoCancel);
			if (save == MessageBoxResult.No)
				return;
			else if (save == MessageBoxResult.Cancel)
			{
				e.Cancel = true;
				return;
			}			

			// Need 128 bits password for the encryption key.
			ApplicationEntry self = new ApplicationEntry("MasterPass", 128 / sizeof(char) / 8, 0, true, true, true, false);
			HashedPassword fileName = self.GeneratePassword(MasterPassword);
			HashedPassword aesKey = self.GeneratePassword(MasterPassword);
			HashedPassword aesIV = self.GeneratePassword(MasterPassword);

			System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();

			// Even if aes is broken the master password is unrecoverable.
			aes.Key = Encoding.Unicode.GetBytes(aesKey.Password);
			aes.IV = Encoding.Unicode.GetBytes(aesIV.Password);
			aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

			ICryptoTransform encryptor = aes.CreateEncryptor();

			// Open the file
			using (FileStream outputStream = new FileStream(fileName.Password + ".pass", FileMode.OpenOrCreate))
			{
				// Use a safe to file encryption method
				using (CryptoStream csEncrypt = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
				{
					// Convert the object to a byte array
					using (MemoryStream objectStream = new MemoryStream())
					{
						// Throw the userData into the object stream.
						IFormatter formatter = new BinaryFormatter();
						formatter.Serialize(objectStream, Data);

						objectStream.Position = 0;

						byte[] buffer = new byte[1024];
						int bytesRead = objectStream.Read(buffer, 0, buffer.Length);

						// While there are still more bytes to write
						while (bytesRead > 0)
						{
							// Write them to the file.
							csEncrypt.Write(buffer, 0, bytesRead);
							bytesRead = objectStream.Read(buffer, 0, buffer.Length);
						}

						// Flush the final block.
						csEncrypt.FlushFinalBlock();
					}
						
				}
				
			}
		}

		/**********************************************************************************************************
		*	string MasterPassword
		*       Purpose:	The master password used to generate all hashed passwords.
		**********************************************************************************************************/
		private string MasterPassword;
		/**********************************************************************************************************
		*	UserData Data
		*       Purpose:	All data related to hashed passwords and applications for this master password.
		**********************************************************************************************************/
		private UserData Data;
	}


}
