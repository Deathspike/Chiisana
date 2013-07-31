// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chiisana.WebSocket {
	/// <summary>
	/// Represents middleware for WebSocket communication.
	/// </summary>
	public sealed class WsServer : IMiddleware {
		/// <summary>
		/// Contains the collection of WS server settings.
		/// </summary>
		private readonly WsServerSettings _Settings;

		/// <summary>
		/// Contains the origin restriction regular expression.
		/// </summary>
		private readonly Regex _Origin;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the WsServer class.
		/// </summary>
		public WsServer()
			: this(new WsServerSettings()) {
			// Return.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the WsServer class.
		/// </summary>
		/// <param name="Settings">The collection of WS server settings.</param>
		public WsServer(WsServerSettings Settings) {
			// Check if the origin to which access is restricted is set.
			if (Settings.Origin != null) {
				// Set the origin restriction regular expression.
				_Origin = new Regex(string.Format(@"^http(s)?\://{0}$", Settings.Origin));
			}
			// Set the collection of HTTP server settings.
			_Settings = Settings.Clone() as WsServerSettings;
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when a socket is opened.
		/// </summary>
		public event Action<IWebSocket> Open;
		#endregion

		#region IMiddleware
		/// <summary>
		/// Handle the request.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		public async Task<bool> HandleAsync(IContext Context) {
			// Initialize the header.
			string Header;
			// Check if the origin to which access is restricted is set and check the origin.
			if (_Origin != null && (Header = Context.Request.Headers["Origin"]) != null && !_Origin.Match(Header).Success) {
				// Return false.
				return false;
			}
			// Check if the Sec-WebSocket-Version header indicates Rf6455.
			if (Context.Request.Headers["Sec-WebSocket-Version"] == "13") {
				// Set the status code of the output returned to the client.
				Context.Response.StatusCode = HttpStatusCode.SwitchingProtocols;
				// Check if the Connection header is valid.
				if ((Header = Context.Request.Headers["Connection"]) != null) {
					// Set the Connection header.
					Context.Response.Headers["Connection"] = Header;
				}
				// Check if the Sec-WebSocket-Protocol header is valid.
				if ((Header = Context.Request.Headers["Sec-WebSocket-Protocol"]) != null) {
					// Set the Sec-WebSocket-Protocol header.
					Context.Response.Headers["Sec-WebSocket-Protocol"] = Header;
				}
				// Check if the Sec-WebSocket-Key header is valid.
				if ((Header = Context.Request.Headers["Sec-WebSocket-Key"]) != null) {
					// Initialize a new instance of the SHA1 class.
					SHA1 SHA1 = SHA1CryptoServiceProvider.Create();
					// Compute the accept hash for the input key.
					byte[] Hash = SHA1.ComputeHash(Encoding.ASCII.GetBytes(Header + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
					// Set the Sec-WebSocket-Accept header with the Base64 representation of the accept hash.
					Context.Response.Headers["Sec-WebSocket-Accept"] = Convert.ToBase64String(Hash);
				}
				// Check if the Upgrade header is valid.
				if ((Header = Context.Request.Headers["Upgrade"]) != null) {
					// Set the Upgrade header.
					Context.Response.Headers["Upgrade"] = Header;
				}
				// Write to the stream to flush each header.
				if (await Context.WriteAsync(string.Empty)) {
					// Run the socket handler on a thread.
					Task InParallel = Task.Run(async () => {
						// Initialize a new instance of the WsClient class.
						WsClient WsClient = new WsClient(_Settings);
						// Initialize a new instance of the WsContext class.
						WsContext WsContext = new WsContext(Context);
						// Check if the close event has subscribers.
						if (Open != null) {
							// Send a notification to each subscriber.
							Open(WsContext);
						}
						// Read from the context and establish communication.
						InParallel = WsClient.RunAsync(WsContext);
					});
					// Return true.
					return true;
				}
			}
			// Return false.
			return false;
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