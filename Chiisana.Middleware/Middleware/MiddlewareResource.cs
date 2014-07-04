// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using Chiisana.Extension;
using System.Reflection;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents middleware for resource hosting.
	/// </summary>
	public sealed class MiddlewareResource : IMiddleware {
		/// <summary>
		/// Contains the assembly.
		/// </summary>
		private readonly Assembly _Assembly;

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
		/// Initialize a new instance of the MiddlewareResource class.
		/// </summary>
		/// <param name="Assembly">The assembly.</param>
		/// <param name="Paths">Each path.</param>
		public MiddlewareResource(Assembly Assembly, params string[] Paths)
			: this(Assembly, null, Paths) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareResource class.
		/// </summary>
		/// <param name="Assembly">The assembly.</param>
		/// <param name="Default">Each possible default file.</param>
		/// <param name="Paths">Each path.</param>
		public MiddlewareResource(Assembly Assembly, string[] Default, params string[] Paths) {
			// Set the assembly.
			_Assembly = Assembly;
			// Set each possible default file.
			_Default = Default;
			// Set each path.
			_Paths = Paths;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareResource class.
		/// </summary>
		/// <param name="Paths">Each path.</param>
		public MiddlewareResource(params string[] Paths)
			: this(Assembly.GetCallingAssembly(), Paths) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareResource class.
		/// </summary>
		/// <param name="Default">Each possible default file.</param>
		/// <param name="Paths">Each path.</param>
		public MiddlewareResource(string[] Default, params string[] Paths)
			: this(Assembly.GetCallingAssembly(), Default, Paths) {
			// Stop the function.
			return;
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
						if (await Context.ResourceAsync(false, _Assembly, Context.Request.Url.LocalPath + Default, _Paths)) {
							// Return true.
							return true;
						}
					}
				}
				// Return false.
				return false;
			}
			// Write to the stream and dispose of the context.
			return await Context.ResourceAsync(false, _Assembly, Context.Request.Url.LocalPath, _Paths);
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