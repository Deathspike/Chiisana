// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace Chiisana {
	/// <summary>
	/// Represents a read-only cookie.
	/// </summary>
	public interface ICookieReadOnly {
		#region Properties
		/// <summary>
		/// Contains the name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Contains the value.
		/// </summary>
		string Value { get; }
		#endregion
	}
}