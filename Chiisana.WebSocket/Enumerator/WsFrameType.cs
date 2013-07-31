// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace Chiisana.WebSocket {
	/// <summary>
	/// Represents the WS frame type.
	/// </summary>
	public enum WsFrameType : byte {
		/// <summary>
		/// Indicates a Continuation Frame.
		/// </summary>
		Contiuation = 0x0,
		/// <summary>
		/// Indicates a Text Frame.
		/// </summary>
		Text = 0x1,
		/// <summary>
		/// Indicates a Binary Frame.
		/// </summary>
		Binary = 0x2,
		/// <summary>
		/// Indicates a Connection Close Frame.
		/// </summary>
		Close = 0x8,
		/// <summary>
		/// Indicates a Ping Frame.
		/// </summary>
		Ping = 0x9,
		/// <summary>
		/// Indicates a Pong Frame.
		/// </summary>
		Pong = 0xA
	}
}