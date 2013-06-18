// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================

namespace Chiisana {
	/// <summary>
	/// Represents a grouped collection.
	/// </summary>
	public interface IGroup<T> : IGroupReadOnly<T> {
		#region Methods
		/// <summary>
		/// Removes all keys and values from the collection.
		/// </summary>
		void Clear();

		/// <summary>
		/// Removes the value with the specified key.
		/// </summary>
		/// <param name="Key">The key.</param>
		bool Remove(string Key);
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the entry with the specified key.
		/// </summary>
		/// <param name="Key">The key.</param>
		new T this[string Key] { get; set; }
		#endregion
	}
}