// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace Chiisana {
	/// <summary>
	/// Represents a cookie.
	/// </summary>
	public interface ICookie : ICookieReadOnly {
		#region Properties
		/// <summary>
		/// Contains the domain for which the cookie applies.
		/// </summary>
		string Domain { get; set; }

		/// <summary>
		/// Contains the expiration date and time.
		/// </summary>
		DateTime Expires { get; set; }

		/// <summary>
		/// Indicates whether the cookie is inaccessible using a scripting language.
		/// </summary>
		bool HttpOnly { get; set; }

		/// <summary>
		/// Coontains the path for which the cookie applies.
		/// </summary>
		string Path { get; set; }

		/// <summary>
		/// Indicates whether the cookie is applies exclusively to secure sockets (HTTPS).
		/// </summary>
		bool Secure { get; set; }

		/// <summary>
		/// Contains the value.
		/// </summary>
		new string Value { get; set; }
		#endregion
	}
}