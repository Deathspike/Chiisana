// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;

namespace Chiisana.WebSocket {
	/// <summary>
	/// Represents a WebSocket based on an existing HTTP context.
	/// </summary>
	public interface IWebSocket : IContext {
		#region Events
		/// <summary>
		/// Occurs when a message is received.
		/// </summary>
		event Action<object> Message;

		/// <summary>
		/// Occurs when closed.
		/// </summary>
		event Action Close;
		#endregion
	}
}