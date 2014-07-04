// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using Chiisana.Extension;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents middleware for file hosting.
	/// </summary>
	public sealed class MiddlewareFile : IMiddleware {
		/// <summary>
		/// Contains each possible default file.
		/// </summary>
		private readonly string[] _Default;

		/// <summary>
		/// Contains each path.
		/// </summary>
		private readonly string[] _Paths;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the MiddlewareFile class.
		/// </summary>
		/// <param name="Paths">Each path.</param>
		public MiddlewareFile(params string[] Paths)
			: this(null, Paths) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareFile class.
		/// </summary>
		/// <param name="Default">Each possible default file.</param>
		/// <param name="Paths">Each path.</param>
		public MiddlewareFile(string[] Default, params string[] Paths) {
			// Set each possible default file.
			_Default = Default;
			// Set the path.
			_Paths = Paths;
		}
		#endregion

		#region IMiddleware
		/// <summary>
		/// Handle the request.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		public async Task<bool> HandleAsync(IContext Context) {
			// Check if the request does end with a trailing slash.
			if (Context.Request.Url.LocalPath.EndsWith("/")) {
				// Check if a possible default file is available.
				if (_Default != null) {
					// Iterate through each possible default file.
					foreach (string Default in _Default) {
						// Write to the stream and dispose of the context.
						if (await Context.FileAsync(false, Context.Request.Url.LocalPath + Default, _Paths)) {
							// Return true.
							return true;
						}
					}
				}
				// Return false.
				return false;
			}
			// Write to the stream and dispose of the context.
			return await Context.FileAsync(false, Context.Request.Url.LocalPath, _Paths);
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