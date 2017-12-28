using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SHA3;

namespace MasterPass
{
	/**********************************************************************************************************
	*	class ApplicationEntry
	*       Purpose:	Represents a website, application, or other service that requires the use of
	*					passwords. Stores a set of HashedPasswords and a set of rules for generating them.
	**********************************************************************************************************/
	[Serializable]
	public class ApplicationEntry
	{
		/**********************************************************************************************************
		*	ApplicationEntry(...)
		*       Purpose:	Constructor.
		*       
		*       Parameters:
		*			String ApplicationName 
		*				The name of the application
		*			int PasswordLenght
		*				The length in characters of the passwords to generate.
		*			byte StartingSeed
		*				The starting byte to differentiate passwords of the same application.
		*			bool LowerCaseAllowed
		*				Are lower case letters allowed?
		*			bool UpperCaseAllowed
		*				Are upper case letters allowed?
		*			bool NumbersAllowed
		*				Are numbers allowed?
		*			bool SpecialCharactersAllowed
		*				Are special characters allowed?
		**********************************************************************************************************/
		public ApplicationEntry(string ApplicationName, int PasswordLength, byte StartingSeed, bool LowerCaseAllowed, bool UpperCaseAllowed, bool NumbersAllowed, bool SpecialCharactersAllowed)
		{
			this.ApplicationName = ApplicationName;
			this.PasswordLength = PasswordLength;
			m_currentSeed = StartingSeed;
			this.LowerCaseAllowed = LowerCaseAllowed;
			this.CapitalsAllowed = UpperCaseAllowed;
			this.NumbersAllowed = NumbersAllowed;
			this.SpecialCharactersAllowed = SpecialCharactersAllowed;

			m_passwords = new List<HashedPassword>();
		}


		/**********************************************************************************************************
		*	HashedPassword GeneratePassword(string MasterPassword)
		*       Purpose:	The name of the application the HashedPass's are for. This is used when generating
		*					unique HashedPasses.
		*		Parameters:
		*				string MasterPassword
		*					The master password to generate the hash with.
		*		
		*		Return:
		*			Returns the HashedPassword generated.
		**********************************************************************************************************/
		public HashedPassword GeneratePassword(string MasterPassword)
		{
			//Create character alphabet.
			List<char> alphabetList = new List<char>();

			if (LowerCaseAllowed)
				alphabetList.AddRange(LOWERCASEALPHABET);
			if (CapitalsAllowed)
				alphabetList.AddRange(UPPERCASEALPHABET);
			if (NumbersAllowed)
				alphabetList.AddRange(NUMBERALPHABET);
			if (SpecialCharactersAllowed)
				alphabetList.AddRange(SPECIALCHARACTERALPHABET);

			char[] alphabet = alphabetList.ToArray();

			SHA3Managed SHAHasher = new SHA3Managed(256);

			// master password in bytes
			byte[] ByteMasterPass = Encoding.Unicode.GetBytes(MasterPassword);

			// application name in bytes.
			byte[] applicationShift = Encoding.Unicode.GetBytes(ApplicationName);

			// the set of master password + application name + seed
			byte[] mangledBytes = new byte[PasswordLength * sizeof(char)];
			// Modify the bytes so that they are website and instance dependent.
			for(int i = 0; i < mangledBytes.Length; i++)
			{
				// Modify the password to be unique for the website.
				mangledBytes[i] = ByteMasterPass[i % ByteMasterPass.Length];
				mangledBytes[i] += applicationShift[i % applicationShift.Length];
				// Modify the password to be unique from each other for the website.
				mangledBytes[i] += CurrentSeed;
			}

			byte[] hashedPassword = new byte[PasswordLength * sizeof(char)];
			// mangled bytes fed through a secure hash.
			for (int i = 0; i < mangledBytes.Length;)
			{
				int bytesHashed = Math.Min(mangledBytes.Length - i, 32);
				byte[] hashedChunk = SHAHasher.ComputeHash(mangledBytes, i, bytesHashed);
				Array.Copy(hashedChunk, 0, hashedPassword, i, bytesHashed);
				i += bytesHashed;
			}

			

			StringBuilder languageCompliantPass = new StringBuilder();

			for(int i = 0, value = 0; i < hashedPassword.Length; i += sizeof(char))
			{
				for(int j = i; j < i + sizeof(char); j++)
				{
					value += hashedPassword[j];
				}

				languageCompliantPass.Append(alphabet[value % alphabet.Length]);
			}

			HashedPassword newPassword = new HashedPassword(languageCompliantPass.ToString(), DateTime.Now, CurrentSeed);

			m_currentSeed++;

			m_passwords.Add(newPassword);

			return newPassword;
		}

		/**********************************************************************************************************
		*	string ApplicationName
		*       Purpose:	The name of the application the HashedPass's are for. This is used when generating
		*					unique HashedPasses.
		**********************************************************************************************************/
		public string ApplicationName { get; }
		/**********************************************************************************************************
		*	int PasswordLength
		*       Purpose:	The maximum length of passwords allowed by the application. All HashedPasses will be
		*					of this length.
		**********************************************************************************************************/
		public int PasswordLength { get; }
		/**********************************************************************************************************
		*	List<HashedPassword> Passwords
		*       Purpose:	A list of all the HashedPasses generated for this application.
		**********************************************************************************************************/
		public List<HashedPassword> Passwords { get { return m_passwords; } }
		private List<HashedPassword> m_passwords;
		/**********************************************************************************************************
		*	byte CurrentSeed
		*       Purpose:	Defines the current modifier to the password producing algorithm. Each time a new
		*					hashed password is generated, this is incremented by 1 to ensure that the following
		*					passwords will be unique.
		**********************************************************************************************************/
		public byte CurrentSeed { get { return m_currentSeed; } }
		private byte m_currentSeed;
		/**********************************************************************************************************
		*	bool LowerCaseAllowed
		*       Purpose:	Defines if the website allows the use of lowercase letters when generating passwords.
		*					if it does not, HashedPasses will never contain them.
		**********************************************************************************************************/
		public bool LowerCaseAllowed { get; }
		/**********************************************************************************************************
		*	bool CapitalsAllowed
		*       Purpose:	Defines if the website allows the use of uppercase letters when generating passwords.
		*					if it does not, HashedPasses will never contain them.
		**********************************************************************************************************/
		public bool CapitalsAllowed { get; }
		/**********************************************************************************************************
		*	bool NumbersAllowed
		*       Purpose:	Defines if the website allows the use of numbers when generating passwords.
		*					if it does not, HashedPasses will never contain them.
		**********************************************************************************************************/
		public bool NumbersAllowed { get; }
		/**********************************************************************************************************
		*	bool SpecialCharactersAllowed
		*       Purpose:	Defines if the website allows the use special characters (!@#$) when generating 
		*					passwords. if it does not, HashedPasses will never contain them.
		**********************************************************************************************************/
		public bool SpecialCharactersAllowed { get; }

		/**********************************************************************************************************
		*	char[] LOWERCASEALPHABET
		*       Purpose:	Defines all characters that are lowercase. These are hand defined to avoid encoding
		*					issues.
		**********************************************************************************************************/
		protected static readonly char[] LOWERCASEALPHABET = 
			{'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
			 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
		/**********************************************************************************************************
		*	char[] UPPERCASEALPHABET
		*       Purpose:	Defines all characters that are uppercase. These are hand defined to avoid encoding
		*					issues.
		**********************************************************************************************************/
		protected static readonly char[] UPPERCASEALPHABET = 
			{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
			 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
		/**********************************************************************************************************
		*	char[] NUMBERALPHABET
		*       Purpose:	Defines all characters that are numbers. These are hand defined to avoid encoding
		*					issues.
		**********************************************************************************************************/
		protected static readonly char[] NUMBERALPHABET =
			{'1', '2', '3', '4', '5', '6', '7', '8', '9'};
		/**********************************************************************************************************
		*	char[] SPECIALCHARACTERALPHABET
		*       Purpose:	Defines all characters that are special. These are hand defined to avoid encoding
		*					issues.
		**********************************************************************************************************/
		protected static readonly char[] SPECIALCHARACTERALPHABET =
			{'!', '@', '#', '$', '%', '^', '*', '(', ')', '-', '_', '=', '+', '[', '{', ']', '}',
			 '`', '~', ',', '<', '.', '>', '/', '?', ';', ':', '\'', '"', '|', '\\'};
	}
}
