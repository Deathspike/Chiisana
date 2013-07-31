// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Chiisana.WebSocket {
	/// <summary>
	/// Represents a WS context.
	/// </summary>
	internal sealed class WsContext : IWebSocket {
		/// <summary>
		/// Contains the HTTP context.
		/// </summary>
		private readonly IContext _Context;

		/// <summary>
		/// Indicates whether the context has been disposed.
		/// </summary>
		private bool _Disposed;

		#region Abstract
		/// <summary>
		/// Write a message frame to the stream.
		/// </summary>
		/// <param name="Type">The type.</param>
		/// <param name="PayloadLength">The payload length.</param>
		private async Task<bool> _FrameAsync(WsFrameType Type, int PayloadLength) {
			// Check if the connection has not been closed.
			if (!IsClosed) {
				// Declare the transfer buffer.
				byte[] Transfer;
				// Check the length of the buffer.
				if (PayloadLength <= 125) {
					// Initialize the transfer buffer.
					Transfer = new byte[2];
					// Set the payload length.
					Transfer[1] = (byte)PayloadLength;
				} else if (PayloadLength >= 126 && PayloadLength <= 65535) {
					// Initialize the length.
					byte[] Length = BitConverter.GetBytes(PayloadLength);
					// Initialize the transfer buffer.
					Transfer = new byte[4];
					// Set the payload length.
					Transfer[1] = 126;
					// Check if this computer is using little endian byte order.
					if (BitConverter.IsLittleEndian) {
						// Reverse the bytes.
						Array.Reverse(Length, 0, 2);
					}
					// Transfer the length buffer to the transfer buffer.
					System.Buffer.BlockCopy(Length, 0, Transfer, 2, 2);
				} else {
					// Initialize the length.
					byte[] Length = BitConverter.GetBytes(PayloadLength);
					// Initialize the transfer buffer.
					Transfer = new byte[9];
					// Set the payload length.
					Transfer[1] = 127;
					// Check if this computer is using little endian byte order.
					if (BitConverter.IsLittleEndian) {
						// Reverse the bytes.
						Array.Reverse(Length, 0, 8);
					}
					// Transfer the length buffer to the transfer buffer.
					System.Buffer.BlockCopy(Length, 0, Transfer, 2, 8);
				}
				// Set the type and indicate this is the last frame in the message.
				Transfer[0] = (byte)((byte)Type | 128);
				// Write the transfer buffer and content buffer to the stream.
				return await _Context.WriteAsync(Transfer);
			}
			// Return false.
			return false;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the WsClient class.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		public WsContext(IContext Context) {
			// Set the HTTP context.
			_Context = Context;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Broadcast a message to each subscriber.
		/// </summary>
		/// <param name="Type">The type.</param>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="PayloadLength">The payload length.</param>
		public void Broadcast(WsFrameType Type, byte[] Buffer, int PayloadLength) {
			// Check if the type is valid for broadcast.
			if ((Type == WsFrameType.Binary || Type == WsFrameType.Text) && Message != null) {
				// Send a notification to each subscriber.
				Message(Type == WsFrameType.Text ? (object)Encoding.UTF8.GetString(Buffer, 0, PayloadLength) : (object) Buffer);
			}
		}
		#endregion

		#region IContext:Events
		/// <summary>
		/// Occurs when the context is disposing.
		/// </summary>
		public event Func<IContext, Task> Disposing;
		#endregion

		#region IContext:Methods
		/// <summary>
		/// Write to the stream and dispose of the context.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		public async Task<bool> SendAsync(byte[] Buffer) {
			// Write to the stream and dispose of the context.
			return await SendAsync(Buffer, 0, Buffer == null ? 0 : Buffer.Length);
		}

		/// <summary>
		/// Write to the stream and dispose of the context.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="Position">The position from where to write.</param>
		/// <param name="Count">The number of bytes to write.</param>
		public async Task<bool> SendAsync(byte[] Buffer, int Position, int Count) {
			// Write to the stream and dispose of the context.
			return await _FrameAsync(WsFrameType.Binary, Count) && await _Context.SendAsync(Buffer, Position, Count);
		}

		/// <summary>
		/// Write to the stream and dispose of the context.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		public async Task<bool> SendAsync(Stream Stream) {
			// Check if the stream cannot seek or has an invalid length.
			if (!Stream.CanSeek || Stream.Length > int.MaxValue) {
				// Return false.
				return false;
			}
			// Write to the stream and dispose of the context.
			return await _FrameAsync(WsFrameType.Binary, (int) Stream.Length) && await _Context.SendAsync(Stream);
		}

		/// <summary>
		/// Write to the stream and dispose of the context.
		/// </summary>
		/// <param name="Content">The content.</param>
		public async Task<bool> SendAsync(string Content) {
			// Initialize the buffer.
			byte[] Buffer = Encoding.UTF8.GetBytes(Content);
			// Write to the stream and dispose of the context.
			return await _FrameAsync(WsFrameType.Text, Buffer.Length) &&  await _Context.SendAsync(Buffer);
		}

		/// <summary>
		/// Write to the stream and dispose of the context.
		/// </summary>
		/// <param name="Content">The content.</param>
		/// <param name="Arguments">Each argument.</param>
		public async Task<bool> SendAsync(string Content, params object[] Arguments) {
			// Write to the stream and dispose of the context.
			return await SendAsync(string.Format(Content, Arguments));
		}

		/// <summary>
		/// Write to the stream.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		public async Task<bool> WriteAsync(byte[] Buffer) {
			// Write to the stream.
			return await WriteAsync(Buffer, 0, Buffer == null ? 0 : Buffer.Length);
		}

		/// <summary>
		/// Write to the stream.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="Position">The position from where to write.</param>
		/// <param name="Count">The number of bytes to write.</param>
		public async Task<bool> WriteAsync(byte[] Buffer, int Position, int Count) {
			// Write to the stream.
			return await _FrameAsync(WsFrameType.Binary, Count) && await _Context.WriteAsync(Buffer, Position, Count);
		}

		/// <summary>
		/// Write to the stream.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		public async Task<bool> WriteAsync(Stream Stream) {
			// Check if the stream cannot seek or has an invalid length.
			if (!Stream.CanSeek || Stream.Length > int.MaxValue) {
				// Return false.
				return false;
			}
			// Write to the stream.
			return await _FrameAsync(WsFrameType.Binary, (int)Stream.Length) && await _Context.WriteAsync(Stream);
		}

		/// <summary>
		/// Write to the stream.
		/// </summary>
		/// <param name="Content">The content.</param>
		public async Task<bool> WriteAsync(string Content) {
			// Initialize the buffer.
			byte[] Buffer = Encoding.UTF8.GetBytes(Content);
			// Write to the stream.
			return await _FrameAsync(WsFrameType.Text, Buffer.Length) && await _Context.WriteAsync(Buffer);
		}

		/// <summary>
		/// Write to the stream.
		/// </summary>
		/// <param name="Content">The content.</param>
		/// <param name="Arguments">Each argument.</param>
		public async Task<bool> WriteAsync(string Content, params object[] Arguments) {
			// Write to the stream.
			return await WriteAsync(string.Format(Content, Arguments));
		}
		#endregion

		#region IContext:Properties
		/// <summary>
		/// Indicates whether the context has been closed.
		/// </summary>
		public bool IsClosed {
			get {
				// Return the status indicating whether the context has been closed.
				return _Context.IsClosed;
			}
		}

		/// <summary>
		/// Contains the request context.
		/// </summary>
		public IContextRequest Request {
			get {
				// Return the request context.
				return _Context.Request;
			}
		}

		/// <summary>
		/// Contains the response context.
		/// </summary>
		public IContextResponse Response {
			get {
				// Return the response context.
				return _Context.Response;
			}
		}
		#endregion

		#region IReleasable
		/// <summary>
		/// Release the object.
		/// </summary>
		public async Task ReleaseAsync() {
			// Check if a subscriber is available for a disposing event.
			if (Disposing != null) {
				// Raise the disposing event and wait for completion.
				Disposing(this).Wait();
			}
			// Check if the context has not been disposed.
			if (!_Disposed) {
				// Set the context status to disposed.
				_Disposed = true;
				// Check if the close event has subscribers.
				if (Close != null) {
					// Send a notification to each subscriber.
					Close();
				}
				// Write a message frame to the stream.
				_FrameAsync(WsFrameType.Close, 0).Wait();
				// Release the context.
				await _Context.ReleaseAsync();
			}
		}
		#endregion

		#region IWebSocket
		/// <summary>
		/// Occurs when a message is received.
		/// </summary>
		public event Action<object> Message;

		/// <summary>
		/// Occurs when closed.
		/// </summary>
		public event Action Close;
		#endregion
	}
}