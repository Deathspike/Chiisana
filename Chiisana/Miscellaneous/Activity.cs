// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents an action implementing middleware.
	/// </summary>
	public sealed class Activity : IMiddleware {
		/// <summary>
		/// Contains the middleware.
		/// </summary>
		private readonly Func<IContext, Task<bool>> _Middleware;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Activity class.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		public Activity(Func<IContext, bool> Middleware) {
			// Set the middleware.
			_Middleware = async (Context) => Middleware(Context);
		}

		/// <summary>
		/// Initialize a new instance of the Activity class.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		public Activity(Func<IContext, Task<bool>> Middleware) {
			// Set the middleware.
			_Middleware = Middleware;
		}
		#endregion

		#region IMiddleware
		/// <summary>
		/// Handle the request.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		public async Task<bool> HandleAsync(IContext Context) {
			// Invoke the middleware.
			return await _Middleware(Context);
		}
		#endregion

		#region IReleasable
		/// <summary>
		/// Release the object.
		/// </summary>
		public async Task ReleaseAsync() {
			// Stop the function.
			return;
		}
		#endregion
	}
}