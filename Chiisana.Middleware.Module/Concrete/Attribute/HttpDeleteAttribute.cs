// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace Chiisana {
	/// <summary>
	/// Represents a DELETE module handler.
	/// </summary>
	public sealed class HttpDeleteAttribute : Attribute, IModuleHandler {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpDeleteAttribute class.
		/// </summary>
		public HttpDeleteAttribute() {
			// Set the data transfer method (such as GET, POST or HEAD).
			HttpMethod = "DELETE";
		}

		/// <summary>
		/// Initialize a new instance of the HttpDeleteAttribute class.
		/// </summary>
		/// <param name="Pattern">The pattern.</param>
		public HttpDeleteAttribute(string Pattern)
			: this() {
			// Set the pattern.
			this.Pattern = Pattern;
		}
		#endregion

		#region IModuleHandler
		/// <summary>
		/// Indicates whether the method name is ignored while routing.
		/// </summary>
		public bool Api { get; set; }

		/// <summary>
		/// Contains the data transfer method (such as GET, POST or HEAD).
		/// </summary>
		public string HttpMethod { get; private set; }

		/// <summary>
		/// Contains the pattern.
		/// </summary>
		public string Pattern { get; private set; }
		#endregion
	}
}