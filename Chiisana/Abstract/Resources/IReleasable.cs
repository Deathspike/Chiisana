// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents a releasable object.
	/// </summary>
	public interface IReleasable {
		#region Methods
		/// <summary>
		/// Release the object.
		/// </summary>
		Task ReleaseAsync();
		#endregion
	}
}