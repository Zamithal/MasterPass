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
	*	Page LaunchPage
	*       Purpose:	Processes the master password and attempts to load existing data for that password.
	**********************************************************************************************************/
	public partial class LaunchPage : Page
	{
		/**********************************************************************************************************
		*	LaunchPage()
		*       Purpose:	Constructor.
		**********************************************************************************************************/
		public LaunchPage()
		{
			InitializeComponent();
		}
		/**********************************************************************************************************
		*	void uxLogin_Click(...)
		*       Purpose:	Processes the master password and, if it exists, attempts to load the users data.
		*					Broadcasts that the login event was successfully fired.
		*       
		*       Parameters:
		*			Standard click event parameters.
		**********************************************************************************************************/
		private void uxLogin_Click(object sender, RoutedEventArgs e)
		{
			if (uxPassword.Text.Length == 0)
			{
				MessageBox.Show("Master password required.");
				return;
			}

			UserData loadedData = LoadUserData(uxPassword.Text);

			if (loadedData == null)
				loadedData = new UserData();

			LoginEvent(uxPassword.Text, loadedData);
		}
		/**********************************************************************************************************
		*	UserData LoadUserData(string MasterPassword)
		*       Purpose:	Loads data, if it exists, from the disk. The data is encrypted using AES using the
		*					master password's hash as the secret key.
		**********************************************************************************************************/
		private UserData LoadUserData(string MasterPassword)
		{

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

			ICryptoTransform decryptor = aes.CreateDecryptor();

			UserData loadedData = null;

			// If there is no data don't load it.
			if (File.Exists(fileName.Password + ".pass") == false)
				return loadedData;

			// Open the file
			using (FileStream outputStream = new FileStream(fileName.Password + ".pass", FileMode.Open))
			{
				// Use a safe to file encryption method
				using (CryptoStream csDecrypt = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Read))
				{
					// Convert the object to a byte array
					using (MemoryStream objectStream = new MemoryStream())
					{
						byte[] buffer = new byte[1024];
						int bytesRead = csDecrypt.Read(buffer, 0, buffer.Length);

						while (bytesRead > 0)
						{
							objectStream.Write(buffer, 0, bytesRead);
							bytesRead = csDecrypt.Read(buffer, 0, buffer.Length);
						}

						csDecrypt.Flush();

						objectStream.Position = 0;

						IFormatter formatter = new BinaryFormatter();
						loadedData = formatter.Deserialize(objectStream) as UserData;
					}

				}

			}

			return loadedData;
		}
		/**********************************************************************************************************
		*	Action<string, ApplicationEntry> LoginEvent
		*       Purpose:	Event fires when the user successfully enters a password.
		**********************************************************************************************************/
		public Action<string, UserData> LoginEvent;
	}
}
