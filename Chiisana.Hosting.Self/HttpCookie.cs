// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Text;

namespace Chiisana.Hosting.Self {
	/// <summary>
	/// Represents a HTTP cookie.
	/// </summary>
	internal sealed class HttpCookie : ICookie {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpCookie class.
		/// </summary>
		/// <param name="Name">The name.</param>
		public HttpCookie(string Name)
			: this(Name, null) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the HttpCookie class.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="Value">The value.</param>
		public HttpCookie(string Name, string Value) {
			// Set the name.
			this.Name = Name;
			// Set the value.
			this.Value = string.IsNullOrEmpty(Value) ? null : Uri.UnescapeDataString(Value.Replace("+", "%20"));
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString() {
			// Initialize a new instance of the StringBuilder class.
			StringBuilder StringBuilder = new StringBuilder();
			// Add the name and value to the serialized cookie.
			StringBuilder.AppendFormat("{0}={1}", Name, string.IsNullOrEmpty(Value) ? null : Uri.EscapeDataString(Value).Replace("+", "%20"));
			// Check if the domain for which the cookie applies is valid.
			if (!string.IsNullOrEmpty(Domain)) {
				// Add the domain for which the cookie applies to the serialized cookie.
				StringBuilder.AppendFormat("; Domain={0}", Domain);
			}
			// Check if the expiration date and time are valid.
			if (Expires != default(DateTime)) {
				// Add the expiration date and time.
				StringBuilder.AppendFormat("; Expires={0}", Expires.ToString("R"));
			}
			// Check whether the cookie is inaccessible using a scripting language.
			if (HttpOnly) {
				// Add the flag indicating the cookie is inaccessible using a scripting language.
				StringBuilder.Append("; HttpOnly");
			}
			// Check if the path for which the cookie applies has been set.
			if (!string.IsNullOrEmpty(Path)) {
				// Check if the path is not prefixed with a slash.
				if (Path[0] != '/') {
					// Prefix the path with a slash.
					Path = '/' + Path;
				}
				// Add the path for which the cookie applies.
				StringBuilder.AppendFormat("; Path={0}", Path);
			}
			// Check whether the cookie is applies exclusively to secure sockets (HTTPS).
			if (Secure) {
				// Add the flag indicating the cookie applies exclusively to secure sockets (HTTPS).
				StringBuilder.Append("; Secure");
			}
			// Join the pieces into the serialized cookie.
			return StringBuilder.ToString();
		}
		#endregion

		#region ICookie:Properties
		/// <summary>
		/// Contains the domain for which the cookie applies.
		/// </summary>
		public string Domain { get; set; }

		/// <summary>
		/// Contains the expiration date and time.
		/// </summary>
		public DateTime Expires { get; set; }

		/// <summary>
		/// Indicates whether the cookie is inaccessible using a scripting language.
		/// </summary>
		public bool HttpOnly { get; set; }

		/// <summary>
		/// Contains the name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Coontains the path for which the cookie applies.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Indicates whether the cookie is applies exclusively to secure sockets (HTTPS).
		/// </summary>
		public bool Secure { get; set; }

		/// <summary>
		/// Contains the value.
		/// </summary>
		public string Value { get; set; }
		#endregion
	}
}