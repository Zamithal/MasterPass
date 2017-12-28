using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterPass
{
	/**********************************************************************************************************
	*	class UserData
	*       Purpose:	This class encompasses all stored data about the user's passwords. The purpose of this
	*					object is to be written to the disk after encryption.
	**********************************************************************************************************/
	[Serializable]
	public class UserData
	{
		/**********************************************************************************************************
		*	UserData()
		*       Purpose:	Constructor.
		**********************************************************************************************************/
		public UserData()
		{
			Applications = new List<ApplicationEntry>();
		}
		/**********************************************************************************************************
		*	List<ApplicationEntry> Applications
		*       Purpose:	A list of all applications the user has specified passwords for.
		**********************************************************************************************************/
		public List<ApplicationEntry> Applications { get; }
	}
}
