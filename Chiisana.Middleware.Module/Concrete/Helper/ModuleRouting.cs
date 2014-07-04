// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace Chiisana {
	/// <summary>
	/// Represents a module routing schema.
	/// </summary>
	public enum ModuleRouting : byte {
		/// <summary>
		/// Case insensitive, optional trailing slash.
		/// </summary>
		Flexible = 0,
		/// <summary>
		/// Case sensitive, requires trailing slash.
		/// </summary>
		Strict,
		/// <summary>
		/// Case sensitive, requires trailing slash, requires lower-case.
		/// </summary>
		StrictAndLowerCase
	}
}