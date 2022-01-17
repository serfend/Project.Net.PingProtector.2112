using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApi.Models
{
	/// <summary>
	/// Represents the state of a connection.
	/// </summary>
	public enum ConnectionState
	{
		/// <summary>
		/// Connection has no state.
		/// </summary>
		None,

		/// <summary>
		/// Connection is OK.
		/// </summary>
		OK,

		/// <summary>
		/// Connection is warning.
		/// </summary>
		Warning,

		/// <summary>
		/// Connection is critical.
		/// </summary>
		Critical,

		/// <summary>
		/// Informations.
		/// </summary>
		Info
	}
}