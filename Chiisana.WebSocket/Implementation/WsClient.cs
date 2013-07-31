// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.IO;
using System.Threading.Tasks;

namespace Chiisana.WebSocket {
	/// <summary>
	/// Represents a WS client.
	/// </summary>
	internal sealed class WsClient {
		/// <summary>
		/// Contains the buffer.
		/// </summary>
		private readonly byte[] _Buffer;

		/// <summary>
		/// Contains the mask buffer.
		/// </summary>
		private readonly byte[] _Mask;

		/// <summary>
		/// Indicates whether this frame is the last in the message.
		/// </summary>
		private bool _IsLastFrameInMessage;

		/// <summary>
		/// Indicates whether the payload of this frame is masked.
		/// </summary>
		private bool _IsMasked;

		/// <summary>
		/// Contains the payload length.
		/// </summary>
		private int _PayloadLength;

		/// <summary>
		/// Contains the position.
		/// </summary>
		private int _Position;

		/// <summary>
		/// Contains the collection of WS server settings.
		/// </summary>
		private readonly WsServerSettings _Settings;

		/// <summary>
		/// Contains the swappable buffer.
		/// </summary>
		private readonly byte[] _Swap;

		/// <summary>
		/// Contains the frame type.
		/// </summary>
		private WsFrameType _Type;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the WsClient class.
		/// </summary>
		/// <param name="Settings">The collection of WS server settings.</param>
		public WsClient(WsServerSettings Settings) {
			// Set the buffer.
			_Buffer = new byte[Settings.MaximumMessageSize];
			// Initialize the mask buffer.
			_Mask = new byte[4];
			// Set the collection of WS server settings.
			_Settings = Settings;
			// Set the swappable buffer.
			_Swap = new byte[8];
		}
		#endregion

		#region Methods
		/// <summary>
		/// Read from the context and establish communication.
		/// </summary>
		/// <param name="Context">The WS context.</param>
		public async Task RunAsync(WsContext Context) {
			// Initialize the stream.
			Stream Stream = Context.Request.InputStream;
			// Set the position.
			_Position = 0;
			// Iterate while the stream is available.
			while (!Context.IsClosed) {
				// Read the frame type and payload length.
				if (await Stream.ReadFullAsync(_Swap, 0, 2)) {
					// Set the status indicating whether this frame is the last in the message.
					_IsLastFrameInMessage = (_Swap[0] & 1) == 1;
					// Set the status indicating whether the payload of this frame is masked.
					_IsMasked = (_Swap[1] & 128) == 128;
					// Set the frame type when not a continuation and check if this is a close frame.
					if ((WsFrameType)(_Swap[0] & 15) != WsFrameType.Contiuation && (_Type = (WsFrameType)(_Swap[0] & 15)) == WsFrameType.Close) {
						// Break iteration.
						break;
					}
					// Set the payload length and check if additional bytes are required.
					if ((_PayloadLength = (byte)(_Swap[1] & 127)) == 126) {
						// Read the additional bytes for the payload length.
						if (!await Stream.ReadFullAsync(_Swap, 0, 2)) {
							// Break iteration.
							break;
						}
						// Check if this computer is using little endian byte order.
						if (BitConverter.IsLittleEndian) {
							// Reverse the bytes.
							Array.Reverse(_Swap, 0, 2);
						}
						// Set the payload length.
						_PayloadLength = BitConverter.ToInt16(_Swap, 0);
					} else if (_PayloadLength == 127) {
						// Read the additional bytes for the payload length.
						if (!await Stream.ReadFullAsync(_Swap, 0, 8)) {
							// Break iteration.
							break;
						}
						// Check if this computer is using little endian byte order.
						if (BitConverter.IsLittleEndian) {
							// Reverse the bytes.
							Array.Reverse(_Swap);
						}
						// Set the payload length.
						_PayloadLength = (int)BitConverter.ToInt64(_Swap, 0);
					}
					// Check if the payload length is valid.
					if (_PayloadLength > 0 && _PayloadLength + _Position < _Settings.MaximumMessageSize) {
						// Check if the payload of this frame is masked and read the mask when necessary.
						if (_IsMasked && !await Stream.ReadFullAsync(_Mask, 0, 4)) {
							// Break iteration.
							break;
						}
						// Read the payload into the buffer.
						await Stream.ReadFullAsync(_Buffer, _Position, _PayloadLength);
						// Check if the payload of this frame is masked.
						if (_IsMasked) {
							// Iterate through the frame payload.
							for (int i = 0, j = _Position; i < _PayloadLength; i++, j++) {
								// Decode the byte.
								_Buffer[j] = (byte)(_Buffer[j] ^ _Mask[i % 4]);
							}
						}
						// Set the position.
						_Position = _IsLastFrameInMessage ? 0 : _PayloadLength + _Position;
						// Check if this is the last frame in the message.
						if (_IsLastFrameInMessage) {
							// Broadcast a message to each subscriber.
							Context.Broadcast(_Type, _Buffer, _PayloadLength);
						}
						// Continue iteration.
						continue;
					}
				}
				// Break the iteration.
				break;
			}
			// Release the context.
			await Context.ReleaseAsync();
		}
		#endregion
	}
}