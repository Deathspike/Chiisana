// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.Collections.Generic;

namespace Chiisana {
	/// <summary>
	/// Represents a read-only grouped collection.
	/// </summary>
	public interface IGroupReadOnly<T> : IEnumerable<string> {
		#region Methods
		/// <summary>
		/// Determines whether the group contains the specific key.
		/// </summary>
		/// <param name="Key">The key.</param>
		bool Contains(string Key);

		/// <summary>
		/// Gets the number of contained elements.
		/// </summary>
		int Count { get; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the entry with the specified key.
		/// </summary>
		/// <param name="Key">The key.</param>
		T this[string Key] { get; }
		#endregion
	}
}