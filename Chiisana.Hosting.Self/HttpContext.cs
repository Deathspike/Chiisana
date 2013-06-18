// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chiisana.Hosting.Self {
	/// <summary>
	/// Represents a HTTP context.
	/// </summary>
	internal sealed class HttpContext : IContext {
		/// <summary>
		/// Indicates whether the header has been written.
		/// </summary>
		private bool _IsHeaderWritten;

		/// <summary>
		/// Indicates whether the context has been released.
		/// </summary>
		private bool _Released;

		/// <summary>
		/// Contains the socket-based HTTP provider.
		/// </summary>
		private readonly HttpServer _Server;

		/// <summary>
		/// Contains the soc
		/// </summary>
		private readonly Socket _Socket;

		/// <summary>
		/// Contains the stream.
		/// </summary>
		private readonly Stream _Stream;

		#region Abstract
		/// <summary>
		/// Write the response header.
		/// </summary>
		public async Task WriteHeaderAsync() {
			// Check if the header has not been written.
			if (!_IsHeaderWritten) {
				// Initialize the buffer.
				byte[] Buffer;
				// Initialize the index.
				int Index = 0;
				// Initialize each header.
				string[] Headers = new string[Response.Cookies.Count + Response.Headers.Count + 4];
				// Set the response line.
				Headers[Index++] = "HTTP/" + (Request.ProtocolVersion == ProtocolVersion.Http10 ? "1.0" : "1.1") + " " + (int)Response.StatusCode + " " + Response.StatusCode;
				// Set the date header.
				Headers[Index++] = "Date: " + DateTime.Now.ToString("R");
				// Iterate through each cookie.
				foreach (string Key in Response.Cookies) {
					// Set the current header.
					Headers[Index++] ="Set-Cookie: " + Response.Cookies[Key];
				}
				// Iterate through each response header.
				foreach (string Key in Response.Headers) {
					// Set the current header.
					Headers[Index++] = Key + ": " + Response.Headers[Key];
				}
				// Initialize and check the buffer for the response header.
				if ((Buffer = Encoding.ASCII.GetBytes(string.Join("\r\n", Headers))) != null) {
					// Set the header status to written.
					_IsHeaderWritten = true;
					// Write the sequence of bytes to the output stream.
					await _Stream.WriteAsync(Buffer, 0, Buffer.Length);
				}
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpContext class.
		/// </summary>
		/// <param name="Server">The socket-based HTTP provider.</param>
		/// <param name="Settings">The collection of HTTP server settings.</param>
		/// <param name="Socket">The socket.</param>
		/// <param name="Stream">The stream.</param>
		/// <param name="Headers">The collection of request headers.</param>
		/// <param name="HttpMethod">The data transfer method (such as GET, POST or HEAD).</param>
		/// <param name="Url">The information about the URL of the current request.</param>
		/// <param name="ProtocolVersion">The protocol version used by the requesting version.</param>
		public HttpContext(HttpServer Server, HttpServerSettings Settings, Socket Socket, Stream Stream, IGroup<string> Headers, string HttpMethod, string Url, string ProtocolVersion) {
			// Set the socket-based HTTP provider.
			_Server = Server;
			// Set the socket.
			_Socket = Socket;
			// Set the stream.
			_Stream = Stream;
			// Set the request context.
			this.Request = new HttpContextRequest(Settings, Socket, Stream, Headers, HttpMethod, Url, ProtocolVersion);
			// Set the response context.
			this.Response = new HttpContextResponse(Request, Settings);
		}
		#endregion

		#region IContext:Methods
		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		public async Task<bool> SendAsync(byte[] Buffer) {
			// Write to the output stream and release the context.
			return await SendAsync(Buffer, 0, Buffer == null ? 0 : Buffer.Length);
		}

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="Position">The position from which to write.</param>
		/// <param name="Count">The number of bytes to write.</param>
		public async Task<bool> SendAsync(byte[] Buffer, int Position, int Count) {
			// Check if the buffer is valid.
			if (Buffer != null && Count != 0) {
				// Set the number of bytes in the body data included in the response.
				Response.ContentLength = Count;
			}
			// Process the content.
			if (true) {
				// Write to the output stream.
				bool Success = await WriteAsync(Buffer, Position, Count);
				// Release the context.
				await ReleaseAsync();
				// Return the response.
				return Success;
			}
		}

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		public async Task<bool> SendAsync(Stream Stream) {
			// Check if the stream is valid and can be seeked.
			if (Stream != null && Stream.CanSeek) {
				// Set the number of bytes in the body data included in the response.
				Response.ContentLength = (int)Stream.Length;
			}
			// Process the content.
			if (true) {
				// Write to the output stream.
				bool Success = await WriteAsync(Stream);
				// Release the context.
				await ReleaseAsync();
				// Return the response.
				return Success;
			}
		}


		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Content">The content.</param>
		public async Task<bool> SendAsync(string Content) {
			// Write to the output stream and release the context.
			return await SendAsync((Response.ContentEncoding ?? Encoding.GetEncoding("ISO-8859-1")).GetBytes(Content ?? string.Empty));
		}

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Content">The content.</param>
		/// <param name="Arguments">Each argument.</param>
		public async Task<bool> SendAsync(string Content, params object[] Arguments) {
			// Write to the output stream and release the context.
			return await SendAsync(string.Format(Content, Arguments));
		}

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		public async Task<bool> WriteAsync(byte[] Buffer) {
			// Write to the output stream.
			return await WriteAsync(Buffer, 0, Buffer == null ? 0 : Buffer.Length);
		}

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="Position">The position from which to write.</param>
		/// <param name="Count">The number of bytes to write.</param>
		public async Task<bool> WriteAsync(byte[] Buffer, int Position, int Count) {
			// Check if the connection has not been closed.
			if (!IsClosed) {
				// Attempt the following code.
				try {
					// Initialize the status indicating whether the buffer is valid.
					bool IsValid = Buffer != null && Count != 0;
					// Check if the header has not been written.
					if (!_IsHeaderWritten) {
						// Write the response header.
						await WriteHeaderAsync();
						// Check if the buffer is invalid.
						if (!IsValid) {
							// Flush the buffers for the output stream.
							await _Stream.FlushAsync();
						}
					}
					// Check if the buffer is valid.
					if (IsValid) {
						// Write the sequence of bytes to the output stream.
						await _Stream.WriteAsync(Buffer, Position, Count);
						// Flush the buffers for the output stream.
						await _Stream.FlushAsync();
					}
					// Return true.
					return true;
				} catch {
					// Set the context status to closed.
					IsClosed = true;
				}
				// Release the context.
				await ReleaseAsync();
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		public async Task<bool> WriteAsync(Stream Stream) {
			// Check if the connection has not been closed.
			if (!IsClosed) {
				// Attempt the following code.
				try {
					// Initialize the status indicating whether the stream is valid.
					bool IsValid = Stream != null;
					// Check if the header has not been written.
					if (!_IsHeaderWritten) {
						// Write the response header.
						await WriteHeaderAsync();
						// Check if the buffer is invalid.
						if (!IsValid) {
							// Flush the buffers for the output stream.
							await _Stream.FlushAsync();
						}
					}
					// Check if the stream is valid.
					if (IsValid) {
						// Read from the stream and write to the output stream.
						await Stream.CopyToAsync(_Stream);
						// Flush the buffers for the output stream.
						await _Stream.FlushAsync();
					}
					// Return true.
					return true;
				} catch {
					// Set the context status to closed.
					IsClosed = true;
				}
				// Release the context.
				await ReleaseAsync();
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Content">The content.</param>
		public async Task<bool> WriteAsync(string Content) {
			// Write to the output stream.
			return await WriteAsync((Response.ContentEncoding ?? Encoding.GetEncoding("ISO-8859-1")).GetBytes(Content ?? string.Empty));
		}

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Content">The content.</param>
		/// <param name="Arguments">Each argument.</param>
		public async Task<bool> WriteAsync(string Content, params object[] Arguments) {
			// Write to the output stream.
			return await WriteAsync(string.Format(Content, Arguments));
		}
		#endregion

		#region IContext:Properties
		/// <summary>
		/// Indicates whether the context has been closed.
		/// </summary>
		public bool IsClosed { get; private set; }

		/// <summary>
		/// Contains the request context.
		/// </summary>
		public IContextRequest Request { get; private set; }

		/// <summary>
		/// Contains the response context.
		/// </summary>
		public IContextResponse Response { get; private set; }
		#endregion

		#region IReleasable
		/// <summary>
		/// Release the object.
		/// </summary>
		public async Task ReleaseAsync() {
			// Check if the context has not been released.
			if (!_Released) {
				// Set the context status to released.
				_Released = true;
				// Check if the context is not closed and the header has not been written.
				if (!IsClosed && !_IsHeaderWritten) {
					// Check if the status code is OK.
					if (Response.StatusCode == HttpStatusCode.OK) {
						// Set the status code of the output returned to the client.
						Response.StatusCode = HttpStatusCode.BadRequest;
					}
					// Set the number of bytes in the body data included in the response.
					Response.ContentLength = 0;
					// Write the response header.
					await WriteHeaderAsync();
					// Flush the buffers for the output stream.
					await _Stream.FlushAsync();
				}
				// Set the context status to closed.
				IsClosed = true;
				// Check whether the server requests a persistent connection.
				if (Response.KeepAlive) {
					// Process a HTTP request for the connection.
					Task InParallel = _Server.ClientAsync(_Socket, _Stream);
					// Stop the function.
					return;
				}
				// Dispose of the stream.
				_Stream.Dispose();
			}
		}
		#endregion
	}
}