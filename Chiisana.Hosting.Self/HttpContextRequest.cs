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
	/// Represents a HTTP request context.
	/// </summary>
	internal sealed class HttpContextRequest : IContextRequest {
		/// <summary>
		/// Contains the collection of cookies sent with the request.
		/// </summary>
		private Group<ICookieReadOnly> _Cookies;

		/// <summary>
		/// Contains the collection of form variables.
		/// </summary>
		private Group<string> _Form;

		/// <summary>
		/// Contains the collection of query variables.
		/// </summary>
		private Group<string> _QueryString;

		/// <summary>
		/// Contains the collection of HTTP server settings.
		/// </summary>
		private readonly HttpServerSettings _Settings;

		/// <summary>
		/// Contains the socket.
		/// </summary>
		private readonly Socket _Socket;

		/// <summary>
		/// Contains the stream.
		/// </summary>
		private readonly Stream _Stream;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpContextRequest class.
		/// </summary>
		/// <param name="Settings">The collection of HTTP server settings.</param>
		/// <param name="Socket">The socket.</param>
		/// <param name="Stream">The stream.</param>
		/// <param name="Headers">The collection of request headers.</param>
		/// <param name="HttpMethod">The data transfer method (such as GET, POST or HEAD).</param>
		/// <param name="Url">The information about the URL of the current request.</param>
		/// <param name="ProtocolVersion">The protocol version used by the requesting version.</param>
		public HttpContextRequest(HttpServerSettings Settings, Socket Socket, Stream Stream, IGroup<string> Headers, string HttpMethod, string Url, string ProtocolVersion) {
			// Set the stream.
			_Stream = InputStream;
			// Set the collection of HTTP server settings.
			_Settings = Settings;
			// Set the socket.
			_Socket = Socket;
			// Set the protocol version used by the requesting version.
			this.ProtocolVersion = ProtocolVersion.Equals("1.0", StringComparison.Ordinal) ? Chiisana.ProtocolVersion.Http10 : Chiisana.ProtocolVersion.Http11;
			// Set the collection of request headers.
			this.Headers = Headers;
			// Set the HTTP method.
			this.HttpMethod = HttpMethod;
			// Set information about the URL of the current request.
			this.Url = new Uri((_Settings.Certificate == null ? "http" : "https") + "://" + "aba" + Url);
		}
		#endregion

		#region IContextRequest:Methods
		/// <summary>
		/// Get the collection of form variables.
		/// </summary>
		public async Task<IGroupReadOnly<string>> GetFormAsync() {
			// Check of the collection of form variables is invalid.
			if (_Form == null) {
				// Initialize a new instance of the Group class.
				_Form = new Group<string>();
				// Check the data transfer method ...
				if ((string.Equals(HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) || string.Equals(HttpMethod, "PUT", StringComparison.OrdinalIgnoreCase))
					// ... check the content type ...
					&& string.Equals(ContentType, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)
					// ... and check the content length.
					&& (ContentLength > 0 && ContentLength < _Settings.MaximumFormSize)) {
					// Attempt the following code.
					try {
						// Initialize the buffer.
						byte[] Buffer = new byte[ContentLength];
						// Initialize the number of read bytes.
						int NumberOfBytesRead = 0;
						// Initialize the position.
						int Position = 0;
						// Read bytes until the count has been reached.
						while ((NumberOfBytesRead = await _Stream.ReadAsync(Buffer, Position, ContentLength - Position)) > 0) {
							// Increment the position with the number of read bytes.
							Position += NumberOfBytesRead;
							// Check if the position has matched or exceeded the number of bytes to read.
							if (Position >= ContentLength) {
								// Retrieve the content and split it into pieces.
								foreach (string Piece in ContentEncoding.GetString(Buffer).Split('&')) {
									// Retrieve the key value pair.
									string[] KeyValuePair = Uri.UnescapeDataString(Piece.Replace("+", "%20")).Split(new[] { '=' }, 2);
									// Check if the content type pair is valid.
									if (KeyValuePair.Length == 2) {
										// Set the key value pair in the group.
										_Form[KeyValuePair[0]] = KeyValuePair[1];
									}
								}
								// Return the collection of form variables.
								return _Form.Count == 0 ? null : _Form;
							}
						}
						// Remove all keys and values from the collection.
						_Form.Clear();
					} catch {
						// Remove all keys and values from the collection.
						_Form.Clear();
					}
				}
			}
			// Return the collection of form variables.
			return _Form.Count == 0 ? null : _Form;
		}
		#endregion

		#region IContextRequest:Properties
		/// <summary>
		/// Contains the accepted MIME types.
		/// </summary>
		public string[] AcceptType {
			get {
				// Initialize the header.
				string Header = Headers["Accept"];
				// Return the value.
				return string.IsNullOrEmpty(Header) ? null : Header.Split(',');
			}
		}

		/// <summary>
		/// Contains the character set of the input stream.
		/// </summary>
		public Encoding ContentEncoding {
			get {
				// Initialize the header.
				string Header = Headers["Content-Type"];
				// Check if the header is valid.
				if (Header != null) {
					// Retrieve the content type pair.
					string[] ContentTypePair = Header.Split(new[] { ';' }, 2);
					// Check if the content type pair is valid.
					if (ContentTypePair.Length == 2) {
						// Retrieve the character set pair.
						string[] CharacterSetPair = ContentTypePair[1].Split(new[] { '=' }, 2);
						// Check if the character set pair is valid.
						if (CharacterSetPair.Length == 2 && CharacterSetPair[0].TrimStart().Equals("charset", StringComparison.InvariantCultureIgnoreCase)) {
							// Attempt the following code.
							try {
								// Return the character set of the input stream.
								return Encoding.GetEncoding(CharacterSetPair[1]);
							} catch (ArgumentException) {
								// Return the default encoding.
								return Encoding.Default;
							}
						}
					}
				}
				// Return the default encoding.
				return Encoding.Default;
			}
		}

		/// <summary>
		/// Contains the length of the data included in the request.
		/// </summary>
		public int ContentLength {
			get {
				// Initialize the value.
				int Value;
				// Return the value.
				return int.TryParse(Headers["Content-Length"], out Value) ? Value : -1;
			}
		}

		/// <summary>
		/// Contains the MIME content type.
		/// </summary>
		public string ContentType {
			get {
				// Initialize the header.
				string Header = Headers["Content-Type"];
				// Return the value.
				return string.IsNullOrEmpty(Header) ? null : Header.Split(new[] { ';' }, 2)[0];
			}
		}

		/// <summary>
		/// Contains the collection of cookies sent with the request.
		/// </summary>
		public IGroupReadOnly<ICookieReadOnly> Cookies {
			get {
				// Check of the collection of cookies sent with the request is invalid.
				if (_Cookies == null) {
					// Initialize the header.
					string Header = Headers["Cookie"];
					// Initialize a new instance of the Group class.
					_Cookies = new Group<ICookieReadOnly>();
					// Check if the cookie header is available.
					if (!string.IsNullOrEmpty(Header)) {
						// Retrieve the cookie and split it into pieces.
						foreach (string Piece in Header.Split(';')) {
							// Retrieve the key value pair.
							string[] KeyValuePair = Piece.Split(new[] { '=' }, 2);
							// Check if the content type pair is valid.
							if (KeyValuePair.Length == 2) {
								// Trim the cookie name.
								KeyValuePair[0] = KeyValuePair[0].TrimStart();
								// Set the key value pair in the group.
								_Cookies[KeyValuePair[0]] = new HttpCookie(KeyValuePair[0], KeyValuePair[1]);
							}
						}
					}
				}
				// Return the collection of cookies sent with the request.
				return _Cookies.Count == 0 ? null : _Cookies;
			}
		}

		/// <summary>
		/// Contains the collection of request headers.
		/// </summary>
		public IGroupReadOnly<string> Headers { get; private set; }

		/// <summary>
		/// Contains the data transfer method (such as GET, POST or HEAD).
		/// </summary>
		public string HttpMethod { get; private set; }

		/// <summary>
		/// Contains the incoming data.
		/// </summary>
		public Stream InputStream {
			get {
				// Return the incoming data.
				return _Stream;
			}
		}

		/// <summary>
		/// Indicates whether the request is from the local computer.
		/// </summary>
		public bool IsLocal {
			get {
				// Return the status indicating whether the request is from the local computer.
				return Url.IsLoopback;
			}
		}

		/// <summary>
		/// Indicates whether the connection uses secure sockets (HTTPS).
		/// </summary>
		public bool IsSecureConnection {
			get {
				// Return the status indicating whether the connection uses secure sockets (HTTPS).
				return _Settings.Certificate != null;
			}
		}

		/// <summary>
		/// Indicates whether the client requests a persistent connection.
		/// </summary>
		public bool KeepAlive {
			get {
				// Initialize the header.
				string Header = Headers["Connection"];
				// Return the value.
				return !string.IsNullOrEmpty(Header) && Header.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase);
			}
		}

		/// <summary>
		/// Contains the protocol version used by the requesting client.
		/// </summary>
		public ProtocolVersion ProtocolVersion { get; private set; }

		/// <summary>
		/// Contains the collection of query variables.
		/// </summary>
		public IGroupReadOnly<string> QueryString {
			get {
				// Check of the collection of query variables is invalid.
				if (_QueryString == null) {
					// Initialize a new instance of the Group class.
					_QueryString = new Group<string>();
					// Retrieve the content and split it into pieces.
					foreach (string Piece in Url.Query.TrimStart('?').Split('&')) {
						// Retrieve the key value pair.
						string[] KeyValuePair = Uri.UnescapeDataString(Piece.Replace("+", "%20")).Split(new[] { '=' }, 2);
						// Check if the content type pair is valid.
						if (KeyValuePair.Length == 2) {
							// Set the key value pair in the group.
							_QueryString[KeyValuePair[0]] = KeyValuePair[1];
						}
					}
				}
				// Return the collection of query variables.
				return _QueryString.Count == 0 ? null : _QueryString;
			}
		}

		/// <summary>
		/// Contains information about the URL of the current request.
		/// </summary>
		public Uri Url { get; private set; }

		/// <summary>
		/// Contains information about the URL of the previous request that linked to the current URL.
		/// </summary>
		public Uri UrlReferrer {
			get {
				// Initialize the header.
				string Header = Headers["Referer"];
				// Return the value.
				return string.IsNullOrEmpty(Header) ? null : (Uri.IsWellFormedUriString(Header, UriKind.Absolute) ? new Uri(Header) : null);
			}
		}

		/// <summary>
		/// Contains the user agent string of the client browser.
		/// </summary>
		public string UserAgent {
			get {
				// Return the user agent string of the client browser.
				return Headers["User-Agent"];
			}
		}

		/// <summary>
		/// Contains the host address of the remote client.
		/// </summary>
		public string UserHostAddress {
			get {
				// Return the host address of the remote client.
				return _Socket.RemoteEndPoint == null ? null : _Socket.RemoteEndPoint.ToString();
			}
		}

		/// <summary>
		/// Contains the DNS name of the remote client.
		/// </summary>
		public string UserHostName {
			get {
				// Initialize the end point.
				IPEndPoint EndPoint = (IPEndPoint)_Socket.RemoteEndPoint;
				// Check if the host address is valid.
				if (_Socket.RemoteEndPoint != null) {
					// Initialize the host name of the remote client.
					IPHostEntry Host = Dns.GetHostEntry(EndPoint.Address);
					// Check if the host name is valid.
					if (Host != null) {
						// Return the DNS name of the host.
						return Host.HostName + ":" + EndPoint.Port;
					}
				}
				// Return null.
				return null;
			}
		}

		/// <summary>
		/// Contains a sorted array of client language preferences.
		/// </summary>
		public string[] UserLanguages {
			get {
				// Initialize the header.
				string Header = Headers["Accept-Language"];
				// Return the value.
				return string.IsNullOrEmpty(Header) ? null : Header.Split(',');
			}
		}
		#endregion
	}
}