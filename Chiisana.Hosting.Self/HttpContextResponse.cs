// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Net;
using System.Text;

namespace Chiisana.Hosting.Self {
	/// <summary>
	/// Represents a HTTP response context.
	/// </summary>
	internal sealed class HttpContextResponse : IContextResponse {
		/// <summary>
		/// Contains the character set.
		/// </summary>
		private Encoding _ContentEncoding;

		/// <summary>
		/// Contains the content type.
		/// </summary>
		private string _ContentType;

		/// <summary>
		/// Contains the protocol version used by the requesting client.
		/// </summary>
		private readonly ProtocolVersion _ProtocolVersion;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpContextResponse class.
		/// </summary>
		/// <param name="Request">The HTTP request context.</param>
		/// <param name="Settings">The collection of HTTP server settings.</param>
		public HttpContextResponse(IContextRequest Request, HttpServerSettings Settings) {
			// Set the protocol version used by the requesting client.
			_ProtocolVersion = Request.ProtocolVersion;
			// Set the collection of cookies returned with the response.
			this.Cookies = new HttpCookieCollection(Request, Settings.Certificate != null);
			// Set the collection of response headers. 
			this.Headers = new Group<string>();
			// Set the status indicating whether the server requests a persistent connection.
			this.KeepAlive = Request.KeepAlive;
			// Set the status code of the output returned to the client.
			this.StatusCode = HttpStatusCode.OK;
		}
		#endregion

		#region IContextResponse:Properties
		/// <summary>
		/// Contains the character set.
		/// </summary>
		public Encoding ContentEncoding {
			get {
				// Get the character set.
				return _ContentEncoding;
			}
			set {
				// Set the character set.
				_ContentEncoding = value;
				// Update the content type
				ContentType = _ContentType;
			}
		}

		/// <summary>
		/// Contains the number of bytes in the body data included in the response.
		/// </summary>
		public int ContentLength {
			get {
				// Initialize the value.
				int Value;
				// Return the number of bytes in the body data included in the response.
				return int.TryParse(Headers["Content-Length"], out Value) ? Value : 0;
			}
			set {
				// Set the number of bytes in the body data included in the response.
				Headers["Content-Length"] = value.ToString();
			}
		}

		/// <summary>
		/// Contains the content type.
		/// </summary>
		public string ContentType {
			get {
				// Get the content type.
				return _ContentType;
			}
			set {
				// Set the content type.
				_ContentType = value;
				// Check if the value is null.
				if (_ContentEncoding == null && string.IsNullOrEmpty(value)) {
					// Remove the content type.
					Headers.Remove("Content-Type");
				} else {
					// Initialize the content type for the header.
					string ContentType = _ContentType ?? "text/plain";
					// Add the Content-Type header to the response headers.
					Headers["Content-Type"] = _ContentEncoding == null ? ContentType : ContentType + "; charset=" + _ContentEncoding.BodyName;
				}
			}
		}

		/// <summary>
		/// Contains the collection of cookies returned with the response.
		/// </summary>
		public IGroupReadOnly<ICookie> Cookies { get; private set; }

		/// <summary>
		/// Contains the collection of response headers.
		/// </summary>
		public IGroup<string> Headers { get; private set; }

		/// <summary>
		/// Indicates whether the server requests a persistent connection.
		/// </summary>
		public bool KeepAlive {
			get {
				// Initialize the header.
				string Header = Headers["Connection"];
				// Return the status indicating whether the server requests a persistent connection.
				return _ProtocolVersion == ProtocolVersion.Http11 && Header == null || Header.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase);
			}
			set {
				// Check if the protocol version supports persistent connections.
				if (_ProtocolVersion == ProtocolVersion.Http11) {
					// Check if the server requests a persistent connection.
					if (value) {
						// Remove the connection header.
						Headers.Remove("Connection");
						// Stop the function.
						return;
					}
					// Set the value of the connection header.
					Headers["Connection"] = "close";
				}
			}
		}

		/// <summary>
		/// Contains the value of the location header.
		/// </summary>
		public string RedirectLocation {
			get {
				// Get the value of the location header.
				return Headers["Location"];
			}
			set {
				// Check if the value is valid.
				if (value != null) {
					// Set the status code of the output returned to the client.
					StatusCode = HttpStatusCode.MovedPermanently;
				}
				// Check if the value is invalid and the status code indicates a redirect.
				if (value == null && StatusCode == HttpStatusCode.MovedPermanently) {
					// Set the status code of the output returned to the client.
					StatusCode = HttpStatusCode.OK;
				}
				// Set the value of the location header.
				Headers["Location"] = value;
			}
		}

		/// <summary>
		/// Contains the status code of the output returned to the client.
		/// </summary>
		public HttpStatusCode StatusCode { get; set; }
		#endregion
	}
}