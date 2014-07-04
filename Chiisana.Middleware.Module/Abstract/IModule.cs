// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace Chiisana {
	/// <summary>
	/// Represents a module.
	/// </summary>
	public interface IModule : IReleasable {
		#region Properties
		/// <summary>
		/// Contains the context.
		/// </summary>
		IContext Context { get;  set; }
		#endregion
	}
}