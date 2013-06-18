// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Security.Cryptography.X509Certificates;

namespace Chiisana.Hosting.Self {
	/// <summary>
	/// Represents a collection of HTTP server settings.
	/// </summary>
	public sealed class HttpServerSettings : ICloneable {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpServerSettings class.
		/// </summary>
		public HttpServerSettings() {
			// Set the maximum number of pending connections.
			this.MaximumConnectionQueue = 100;
			// Set the maximum size of a form.
			this.MaximumFormSize = 2 * 1024 * 1024;
			// Set the maximum size of a request line or header.
			this.MaximumLineSize = 8192;
			// Set the maximum number of headers.
			this.MaximumNumberOfHeaders = 100;
			// Set the duration in milliseconds until a read timeout occurs.
			this.ReadTimeout = 5000;
		}
		#endregion

		#region Properties
		/// <summary>
		/// The certificate to be applied on the stream (Default: null).
		/// </summary>
		public X509Certificate2 Certificate { get; set; }

		/// <summary>
		/// The maximum number of pending connections (Default: 100).
		/// </summary>
		public int MaximumConnectionQueue { get; set; }

		/// <summary>
		/// The maximum size of a form (Default: 2MB).
		/// </summary>
		public int MaximumFormSize { get; set; }

		/// <summary>
		/// The maximum size of a request line or header (Default: 8192).
		/// </summary>
		public int MaximumLineSize { get; set; }

		/// <summary>
		/// The maximum number of headers (Default: 100).
		/// </summary>
		public int MaximumNumberOfHeaders { get; set; }

		/// <summary>
		/// The duration in milliseconds until a read timeout occurs (Default 5000).
		/// </summary>
		public int ReadTimeout { get; set; }
		#endregion

		#region ICloneable
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		public object Clone() {
			// Initialize a new instance of the HttpServerSettings class.
			return new HttpServerSettings {
				// Set the certificate to be applied on the stream.
				Certificate = Certificate,
				// Set the maximum number of pending connections.
				MaximumConnectionQueue = MaximumConnectionQueue,
				// Set the maximum size of a form.
				MaximumFormSize = MaximumFormSize,
				// Set the maximum size of a request line or header.
				MaximumLineSize = MaximumLineSize,
				// Set the maximum number of headers.
				MaximumNumberOfHeaders = MaximumNumberOfHeaders,
				// Set the duration in milliseconds until a read timeout occurs.
				ReadTimeout = ReadTimeout
			};
		}
		#endregion
	}
}