﻿// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace Chiisana {
	/// <summary>
	/// Represents a POST module handler.
	/// </summary>
	public sealed class HttpPostAttribute : Attribute, IModuleHandler {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpPostAttribute class.
		/// </summary>
		public HttpPostAttribute() {
			// Set the data transfer method (such as GET, POST or HEAD).
			HttpMethod = "POST";
		}

		/// <summary>
		/// Initialize a new instance of the HttpPostAttribute class.
		/// </summary>
		/// <param name="Pattern">The pattern.</param>
		public HttpPostAttribute(string Pattern)
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
		/// Contains the transfer method.
		/// </summary>
		public string HttpMethod { get; private set; }

		/// <summary>
		/// Contains the pattern.
		/// </summary>
		public string Pattern { get; private set; }
		#endregion
	}
}