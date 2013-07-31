// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents middleware for chaining middleware.
	/// </summary>
	public sealed class Middleware : IMiddleware {
		/// <summary>
		/// Contains the collection of middleware.
		/// </summary>
		private readonly Dictionary<string, List<IMiddleware>> _Middleware;

		#region Abstract
		/// <summary>
		/// Handle the request for the host.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="Host">The host.</param>
		private async Task<bool> _HandleHostAsync(IContext Context, string Host) {
			// Check if the virtual host is available.
			if (_Middleware.ContainsKey(Host)) {
				// Iterate through each resulting middleware.
				foreach (IMiddleware Middleware in _Middleware[Host]) {
					// Handle the request.
					if (await Middleware.HandleAsync(Context)) {
						// Return true.
						return true;
					}
				}
			}
			// Return false.
			return false;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Middleware class.
		/// </summary>
		/// <param name="Middlewares">The middlewares.</param>
		public Middleware(params IMiddleware[] Middlewares) {
			// Initialize a new instance of the Dictionary class.
			_Middleware = new Dictionary<string, List<IMiddleware>>();
			// Iterate through each middleware.
			foreach (IMiddleware Middleware in Middlewares) {
				// Use the middleware.
				Use(Middleware);
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Use the middleware.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		public Middleware Use(Func<IContext, bool> Middleware) {
			// Use the middleware.
			return Use(new Activity(Middleware));
		}

		/// <summary>
		/// Use the middleware.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		public Middleware Use(Func<IContext, Task<bool>> Middleware) {
			// Use the middleware.
			return Use(new Activity(Middleware));
		}

		/// <summary>
		/// Use the middleware.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		public Middleware Use(IMiddleware Middleware) {
			// Add the middleware.
			return Use(Middleware, null);
		}

		/// <summary>
		/// Use the middleware.
		/// </summary>
		/// <param name="Middleware">The middleware.</param>
		/// <param name="VirtualHost">The virtual host.</param>
		public Middleware Use(IMiddleware Middleware, string VirtualHost) {
			// Check if the middleware is valid.
			if (Middleware != null) {
				// Check if the virtual host is invalid.
				if (VirtualHost == null) {
					// Set the virtual host.
					VirtualHost = string.Empty;
				}
				// Lock this object.
				lock (this) {
					// Check if the middleware does not contain the virtual host.
					if (!_Middleware.ContainsKey(VirtualHost)) {
						// Initialize a new instance of the Dictionary class.
						_Middleware[VirtualHost] = new List<IMiddleware>();
					}
					// Add the middleware.
					_Middleware[VirtualHost].Add(Middleware);
				}
			}
			// Return this object.
			return this;
		}
		#endregion

		#region IMiddleware
		/// <summary>
		/// Handle the request.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		public async Task<bool> HandleAsync(IContext Context) {
			// Return the status indicating whether the context has been handled.
			return await _HandleHostAsync(Context, Context.Request.Url.Host) || await _HandleHostAsync(Context, string.Empty);
		}
		#endregion

		#region IReleasable
		/// <summary>
		/// Release the object.
		/// </summary>
		public async Task ReleaseAsync() {
			// Iterate through each key.
			foreach (string Key in _Middleware.Keys) {
				// Iterate through each middleware.
				foreach (IMiddleware Middleware in _Middleware[Key]) {
					// Release the object.
					await Middleware.ReleaseAsync();
				}
			}
			// Clear the middleware.
			_Middleware.Clear();
		}
		#endregion
	}
}