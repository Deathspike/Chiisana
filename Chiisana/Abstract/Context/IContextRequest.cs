// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents a HTTP request context.
	/// </summary>
	public interface IContextRequest {
		#region Methods
		/// <summary>
		/// Get the collection of form variables.
		/// </summary>
		Task<IGroupReadOnly<string>> GetFormAsync();
		#endregion

		#region Properties
		/// <summary>
		/// Contains the accepted MIME types.
		/// </summary>
		string[] AcceptType { get; }

		/// <summary>
		/// Contains the character set of the input stream.
		/// </summary>
		Encoding ContentEncoding { get; }

		/// <summary>
		/// Contains the length of the data included in the request.
		/// </summary>
		int ContentLength { get; }

		/// <summary>
		/// Contains the MIME content type.
		/// </summary>
		string ContentType { get; }

		/// <summary>
		/// Contains the collection of cookies sent with the request.
		/// </summary>
		IGroupReadOnly<ICookieReadOnly> Cookies { get; }

		/// <summary>
		/// Contains the collection of request headers.
		/// </summary>
		IGroupReadOnly<string> Headers { get; }

		/// <summary>
		/// Contains the data transfer method (such as GET, POST or HEAD).
		/// </summary>
		string HttpMethod { get; }

		/// <summary>
		/// Contains the incoming data.
		/// </summary>
		Stream InputStream { get; }

		/// <summary>
		/// Indicates whether the request is from the local computer.
		/// </summary>
		bool IsLocal { get; }

		/// <summary>
		/// Indicates whether the connection uses secure sockets (HTTPS).
		/// </summary>
		bool IsSecureConnection { get; }

		/// <summary>
		/// Indicates whether the client requests a persistent connection.
		/// </summary>
		bool KeepAlive { get; }

		/// <summary>
		/// Contains the protocol version used by the requesting client.
		/// </summary>
		Protocol Protocol { get; }

		/// <summary>
		/// Contains the collection of query variables.
		/// </summary>
		IGroupReadOnly<string> QueryString { get; }

		/// <summary>
		/// Contains information about the URL of the current request.
		/// </summary>
		Uri Url { get; }

		/// <summary>
		/// Contains information about the URL of the previous request that linked to the current URL.
		/// </summary>
		Uri UrlReferrer { get; }

		/// <summary>
		/// Contains the user agent string of the client browser.
		/// </summary>
		string UserAgent { get; }

		/// <summary>
		/// Contains the host address of the remote client.
		/// </summary>
		string UserHostAddress { get; }

		/// <summary>
		/// Contains the DNS name of the remote client.
		/// </summary>
		string UserHostName { get; }

		/// <summary>
		/// Contains a sorted array of client language preferences.
		/// </summary>
		string[] UserLanguages { get; }
		#endregion
	}
}