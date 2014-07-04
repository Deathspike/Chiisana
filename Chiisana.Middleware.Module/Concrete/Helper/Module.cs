// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents a module.
	/// </summary>
	public abstract class Module : IModule {
		#region IModule
		/// <summary>
		/// Contains the context.
		/// </summary>
		public IContext Context { get; set; }
		#endregion

		#region IReleasable
		/// <summary>
		/// Release the object.
		/// </summary>
		public async Task ReleaseAsync() {
			// Release the object.
			await Context.ReleaseAsync();
		}
		#endregion
	}
}