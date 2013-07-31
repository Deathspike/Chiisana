// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using System.Threading.Tasks;

namespace Chiisana.WebSocket {
	/// <summary>
	/// Represents the class providing extensions for the Stream class.
	/// </summary>
	internal static class ExtensionForStream {
		#region Methods
		/// <summary>
		/// Asynchronously reads a sequence of bytes from the current stream and advances the position within the stream.
		/// </summary>
		/// <param name="Stream">The stream.</param>
		/// <param name="Buffer">The buffer to write the data into.</param>
		/// <param name="Position">The byte offset in buffer at which to begin writing data from the stream.</param>
		/// <param name="Count">The number of bytes to read.</param>
		public static async Task<bool> ReadFullAsync(this Stream Stream, byte[] Buffer, int Position, int Count) {
			// Initialize the current position.
			int CurrentPosition = Position;
			// Initialize the number of read bytes.
			int NumberOfBytesRead = 0;
			// Read bytes until the count has been reached.
			while ((NumberOfBytesRead = await Stream.ReadAsync(Buffer, CurrentPosition, Count - CurrentPosition)) > 0) {
				// Increment the current position with the number of read bytes.
				CurrentPosition += NumberOfBytesRead;
				// Check if the position has matched or exceeded the number of bytes to read.
				if (CurrentPosition >= Count) {
					// Return true.
					return true;
				}
			}
			// Return false.
			return false;
		}
		#endregion
	}
}