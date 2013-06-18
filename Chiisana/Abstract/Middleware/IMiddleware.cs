// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents HTTP middleware.
	/// </summary>
	public interface IMiddleware : IReleasable {
		#region Methods
		/// <summary>
		/// Handle the HTTP request.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		Task<bool> HandleAsync(IContext Context);
		#endregion
	}
}