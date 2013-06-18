// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chiisana.Hosting.Self {
	/// <summary>
	/// Represents a socket-based HTTP provider.
	/// </summary>
	public sealed class HttpServer : IProvider {
		/// <summary>
		/// Indicates whether the server has been released.
		/// </summary>
		private bool _Released;

		/// <summary>
		/// Contains the collection of HTTP server settings.
		/// </summary>
		private readonly HttpServerSettings _Settings;

		/// <summary>
		/// Contains the TCP listener.
		/// </summary>
		private readonly TcpListener _TcpListener;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpServer class.
		/// </summary>
		/// <param name="Port">The port.</param>
		public HttpServer(int Port)
			: this(Port, new HttpServerSettings()) {
			// Return.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the HttpServer class.
		/// </summary>
		/// <param name="Port">The port.</param>
		/// <param name="Settings">The collection of HTTP server settings.</param>
		public HttpServer(int Port, HttpServerSettings Settings) {
			// Initialize a new instance of the IPEndPoint class.
			IPEndPoint EndPoint = new IPEndPoint(0, Port);
			// Set the collection of HTTP server settings.
			_Settings = Settings.Clone() as HttpServerSettings;
			// Set the TCP listener.
			_TcpListener = new TcpListener(EndPoint);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Establish a HTTP context for the connection.
		/// </summary>
		/// <param name="Socket">The socket.</param>
		/// <param name="Stream">The stream.</param>
		public async Task<IContext> ContextAsync(Socket Socket, Stream Stream) {
			// Initialize the current byte.
			int Byte;
			// Initialize the buffer.
			byte[] Buffer = new byte[_Settings.MaximumLineSize];
			// Initialize a new instance of the Group class.
			Group<string> Headers = new Group<string>();
			// Initialize the match.
			Match Match = null;
			// Initialize the number of available bytes.
			int NumberOfAvailableBytes = 0;
			// Initialize the number of headers.
			int NumberOfHeaders = 0;
			// Initialize the current position.
			int Position = 0;
			// Read bytes until the headers have been read.
			while (Position < Buffer.Length) {
				// Attempt the following code.
				try {
					// Check if data is available to be read.
					if (NumberOfAvailableBytes != 0 || (NumberOfAvailableBytes = Socket.Available) != 0) {
						// Read the byte and set it in the buffer.
						Buffer[Position] = (byte)(Byte = Stream.ReadByte());
						// Decrement the number of available bytes.
						NumberOfAvailableBytes--;
					} else if (await Stream.ReadAsync(Buffer, Position, 1) == 0) {
						// Break the iteration.
						break;
					} else {
						// Set the current byte.
						Byte = Buffer[Position];
					}
				} catch {
					// Break the iteration.
					break;
				}
				// Check if the current byte is an indication of a CRLF termination.
				if (Position > 0 && Byte == '\n' && Buffer[Position - 1] == '\r') {
					// Check if the line is empty.
					if (Position <= 1) {
						// Check if this is not the first line.
						if (Match != null) {
							// Return a new instance of the HttpContext class.
							return new HttpContext(this, _Settings, Socket, Stream, Headers, Match.Groups[1].Value, Match.Groups[2].Value, Match.Groups[3].Value);
						}
						// Break the iteration.
						break;
					} else {
						// Retrieve the line.
						string Line = Encoding.ASCII.GetString(Buffer, 0, Position - 1);
						// Check if this is the first line.
						if (Match == null) {
							// Match the pattern to the line.
							if (!(Match = Regex.Match(Line, @"^(CONNECT|DELETE|HEAD|GET|OPTIONS|POST|PUT|TRACE) (\/[\S]*) HTTP\/(1.0|1.1)$", RegexOptions.Compiled)).Success) {
								// Break the iteration.
								break;
							}
						} else if (NumberOfHeaders >= _Settings.MaximumNumberOfHeaders) {
							// Break the iteration.
							break;
						} else {
							// Initialize the pair.
							string[] Pair = Line.Split(new[] { ':' }, 2);
							// Check if the pair is invalid.
							if (Pair.Length != 2) {
								// Break the iteration.
								break;
							} else {
								// Set the header.
								Headers[Pair[0]] = Pair[1].TrimStart();
								// Increment the number of headers.
								NumberOfHeaders++;
							}
						}
						// Reset the current position.
						Position = 0;
					}
				} else {
					// Increment the current position.
					Position++;
				}
			}
			// Return a new instance of the HttpContext class.
			return new HttpContext(this, _Settings, Socket, Stream, Headers, null, "/", Match == null ? "1.0" : Match.Groups[3].Value);
		}

		/// <summary>
		/// Process a HTTP request for the connection.
		/// </summary>
		/// <param name="Socket">The socket.</param>
		/// <param name="Stream">The stream.</param>
		public async Task ClientAsync(Socket Socket, Stream Stream) {
			// Initialize the HTTP context.
			IContext Context;
			// Establish a HTTP context for the connection and check if it is valid.
			if ((Context = await ContextAsync(Socket, Stream)).Request.HttpMethod != null) {
				// Initialize the exception.
				Exception Exception;
				// Attempt the following code.
				try {
					// Retrieve the middleware to avoid concurrent access.
					IMiddleware Middleware = this.Middleware;
					// Check if the middleware is invalid.
					if (Middleware == null) {
						// Set the status code of the output returned to the client.
						Context.Response.StatusCode = HttpStatusCode.InternalServerError;
						// Release the context.
						await Context.ReleaseAsync();
					} else if (!await Middleware.HandleAsync(Context)) {
						// Set the status code of the output returned to the client.
						Context.Response.StatusCode = HttpStatusCode.NotFound;
						// Release the context.
						await Context.ReleaseAsync();
					}
					// Stop the function.
					return;
				} catch (Exception ThrownException) {
					// Set the exception.
					Exception = ThrownException;
				}
				// Check if the connection has not been closed.
				if (!Context.IsClosed) {
					// Check if the request is local.
					if (Context.Request.IsLocal) {
						// Set the status code of the output returned to the client.
						Context.Response.StatusCode = HttpStatusCode.InternalServerError;
						// Send the exception.
						await Context.SendAsync(Exception.ToString());
					} else {
						// Set the status code of the output returned to the client.
						Context.Response.StatusCode = HttpStatusCode.InternalServerError;
						// Release the context.
						await Context.ReleaseAsync();
					}
				}
			} else {
				// Set the status indicating whether the server requests a persistent connection.
				Context.Response.KeepAlive = false;
				// Release the context.
				await Context.ReleaseAsync();
			}
		}

		/// <summary>
		/// Start listening for incoming connections.
		/// </summary>
		public async Task RunAsync() {
			// Check if the listener is not disposed.
			if (!_Released) {
				// Start listening for incoming connections.
				_TcpListener.Start(_Settings.MaximumConnectionQueue);
				// Iterate on a thread.
				while (!_Released) {
					// Accept a pending connection request.
					Socket Socket = await _TcpListener.AcceptSocketAsync();
					// Check if the socket is valid.
					if (Socket != null) {
						// Set the socket stream to use the Nagle algorithm.
						Socket.NoDelay = true;
						// Run the handler using a task.
						Task InParallel = Task.Run(async () => {
							// Initialize new instances of the NetworkStream classes.
							Stream Stream = new NetworkStream(Socket, true);
							// Set the value, in milliseconds, that determines how long the stream will attempt to read before timing out.
							Stream.ReadTimeout = _Settings.ReadTimeout;
							Socket.ReceiveTimeout = _Settings.ReadTimeout;


							System.Threading.Thread.Sleep(2000);
							// Attempt the following code.
							try {
								// Initialize new instances of the BufferedStream classes.
								// Stream = new BufferedStream(Stream);
								// Check if a certificate is available 
								if (_Settings.Certificate != null) {
									// Initialise SslStream and instruct it to supersede the network stream.
									SslStream SslStream = new SslStream(Stream, false);
									// Authenticate the client browser.
									await SslStream.AuthenticateAsServerAsync(_Settings.Certificate, false, SslProtocols.Tls, true);
									// Set the stream.
									Stream = SslStream;
								}
								// Process a HTTP request for the connection.
								InParallel = ClientAsync(Socket, Stream);
							} catch {
								// Check if the stream is valid.
								if (Stream != null) {
									// Dispose of the stream.
									Stream.Dispose();
								}
							}
						});
					}
				}
			}
		}
		#endregion

		#region IProvider
		/// <summary>
		/// Contains middleware.
		/// </summary>
		public IMiddleware Middleware { get; set; }
		#endregion

		#region IReleasable
		/// <summary>
		/// Release the object.
		/// </summary>
		public async Task ReleaseAsync() {
			// Check if the server has not been released.
			if (!_Released) {
				// Set the server status to released.
				_Released = true;
				// Close the TCP listener.
				_TcpListener.Stop();
			}
		}
		#endregion
	}
}