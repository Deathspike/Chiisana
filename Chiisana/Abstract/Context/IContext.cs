// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents a HTTP context.
	/// </summary>
	public interface IContext : IReleasable {
		#region Methods
		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		Task<bool> SendAsync(byte[] Buffer);

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="Position">The position from which to write.</param>
		/// <param name="Count">The number of bytes to write.</param>
		Task<bool> SendAsync(byte[] Buffer, int Position, int Count);

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		Task<bool> SendAsync(Stream Stream);

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Content">The content.</param>
		Task<bool> SendAsync(string Content);

		/// <summary>
		/// Write to the output stream and release the context.
		/// </summary>
		/// <param name="Content">The content.</param>
		/// <param name="Arguments">Each argument.</param>
		Task<bool> SendAsync(string Content, params object[] Arguments);

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		Task<bool> WriteAsync(byte[] Buffer);

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <param name="Position">The position from which to write.</param>
		/// <param name="Count">The number of bytes to write.</param>
		Task<bool> WriteAsync(byte[] Buffer, int Position, int Count);

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		Task<bool> WriteAsync(Stream Stream);

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Content">The content.</param>
		Task<bool> WriteAsync(string Content);

		/// <summary>
		/// Write to the output stream.
		/// </summary>
		/// <param name="Content">The content.</param>
		/// <param name="Arguments">Each argument.</param>
		Task<bool> WriteAsync(string Content, params object[] Arguments);
		#endregion

		#region Properties
		/// <summary>
		/// Indicates whether the context has been closed.
		/// </summary>
		bool IsClosed { get; }

		/// <summary>
		/// Contains the request context.
		/// </summary>
		IContextRequest Request { get; }

		/// <summary>
		/// Contains the response context.
		/// </summary>
		IContextResponse Response { get; }
		#endregion
	}
}