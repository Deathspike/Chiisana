// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace Chiisana {
	/// <summary>
	/// Represents a collection of WS server settings.
	/// </summary>
	public sealed class WsServerSettings : ICloneable {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the WsServerSettings class.
		/// </summary>
		public WsServerSettings() {
			// Set the maximum size of a message.
			MaximumMessageSize = 1024;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The maximum size of a message (Default: 1024).
		/// </summary>
		public int MaximumMessageSize { get; set; }

		/// <summary>
		/// The origin to which access is restricted (Default: null).
		/// </summary>
		public string Origin { get; set; }
		#endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		public object Clone() {
			// Initialize a new instance of the WsServerSettings class.
			return new WsServerSettings {
				// Set the maximum size of a message.
				MaximumMessageSize = MaximumMessageSize,
				// Set the origin to which access is restricted.
				Origin = Origin
			};
		}
		#endregion
	}
}