// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace Chiisana {
	/// <summary>
	/// Represents a module handler.
	/// </summary>
	public interface IModuleHandler {
		#region Properties
		/// <summary>
		/// Indicates whether the method name is ignored while routing.
		/// </summary>
		bool Api { get; set; }

		/// <summary>
		/// Contains the data transfer method (such as GET, POST or HEAD).
		/// </summary>
		string HttpMethod { get; }

		/// <summary>
		/// Contains the pattern.
		/// </summary>
		string Pattern { get; }
		#endregion
	}
}