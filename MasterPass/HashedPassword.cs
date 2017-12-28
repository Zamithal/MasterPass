using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterPass
{
	/**********************************************************************************************************
	*	class HashedPassword
	*       Purpose:	Stores all the information unique to a hashed password, including the hashed password
	*					itself.
	**********************************************************************************************************/
	[Serializable]
	public class HashedPassword
	{
		/**********************************************************************************************************
		*	HashedPassword(string Password, DateTime DateCreated, int Seed)
		*       Purpose:	Constructor.
		**********************************************************************************************************/
		public HashedPassword(string Password, DateTime DateCreated, int Seed)
		{
			this.Password = Password;
			this.DateCreated = DateCreated;
			this.Seed = Seed;
		}
		/**********************************************************************************************************
		*	string Password
		*       Purpose:	The hashed password generated for the application entry using the seed.
		**********************************************************************************************************/
		public string Password { get; }
		/**********************************************************************************************************
		*	int Seed
		*       Purpose:	A modifier allowing passwords generated for the same application entry to differ.
		**********************************************************************************************************/
		public int Seed { get; }
		/**********************************************************************************************************
		*	DateTime DateCreated
		*       Purpose:	The time and date this HashedPassword was generated.
		**********************************************************************************************************/
		public DateTime DateCreated { get; }

		/**********************************************************************************************************
		*	public static bool operator==(HashedPassword lhs, HashedPassword rhs)
		*       Purpose:	Comparison Operator.
		**********************************************************************************************************/
		public static bool operator==(HashedPassword lhs, HashedPassword rhs)
		{
			return !(lhs != rhs);
		}
		/**********************************************************************************************************
		*	public static bool operator==(HashedPassword lhs, string rhs)
		*       Purpose:	Comparison Operator.
		**********************************************************************************************************/
		public static bool operator==(HashedPassword lhs, string rhs)
		{
			return !(lhs != rhs);
		}
		/**********************************************************************************************************
		*	public static bool operator!=(HashedPassword lhs, HashedPassword rhs)
		*       Purpose:	Inverse Comparison Operator.
		**********************************************************************************************************/
		public static bool operator!=(HashedPassword lhs, HashedPassword rhs)
		{
			if (lhs.Password != rhs.Password)
				return true;

			return false;
		}
		/**********************************************************************************************************
		*	public static bool operator!=(HashedPassword lhs, string rhs)
		*       Purpose:	Inverse Comparison Operator.
		**********************************************************************************************************/
		public static bool operator!=(HashedPassword lhs, string rhs)
		{
			if (lhs.Password != rhs)
				return true;

			return false;
		}
	}
}
