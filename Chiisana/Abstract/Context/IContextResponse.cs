// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Net;
using System.Text;

namespace Chiisana {
	/// <summary>
	/// Represents a HTTP response context.
	/// </summary>
	public interface IContextResponse {
		#region Properties
		/// <summary>
		/// Contains the character set.
		/// </summary>
		Encoding ContentEncoding { get; set; }

		/// <summary>
		/// Contains the number of bytes in the body data included in the response.
		/// </summary>
		int ContentLength { get; set; }

		/// <summary>
		/// Contains the content type.
		/// </summary>
		string ContentType { get; set; }

		/// <summary>
		/// Contains the collection of cookies returned with the response.
		/// </summary>
		IGroupReadOnly<ICookie> Cookies { get; }

		/// <summary>
		/// Contains the collection of response headers.
		/// </summary>
		IGroup<string> Headers { get; }

		/// <summary>
		/// Indicates whether the server requests a persistent connection.
		/// </summary>
		bool KeepAlive { get; set; }

		/// <summary>
		/// Contains the value of the location header.
		/// </summary>
		string RedirectLocation { get; set; }

		/// <summary>
		/// Contains the status code of the output returned to the client.
		/// </summary>
		HttpStatusCode StatusCode { get; set; }
		#endregion
	}
}