// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chiisana {
	/// <summary>
	/// Represents a grouped collection.
	/// </summary>
	public sealed class Group<T> : IGroup<T> {
		/// <summary>
		/// Contains the collection.
		/// </summary>
		private readonly Dictionary<string, T> _Collection;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Group class.
		/// </summary>
		public Group() {
			// Initialize a new instance of the Dictionary class.
			_Collection = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region IGroup
		/// <summary>
		/// Gets or sets the entry with the specified key.
		/// </summary>
		/// <param name="Key">The key.</param>
		public T this[string Key] {
			get {
				// Gets the entry with the specified key.
				return Contains(Key) ? _Collection[Key] : default(T);
			}
			set {
				// Sets the entry with the specified key.
				_Collection[Key] = value;
			}
		}

		/// <summary>
		/// Removes all keys and values from the collection.
		/// </summary>
		public void Clear() {
			// Removes all keys and values from the collection.
			_Collection.Clear();
		}
		/// <summary>
		/// Removes the value with the specified key.
		/// </summary>
		/// <param name="Key">The key.</param>
		public bool Remove(string Key) {
			// Remove the value with the specified key.
			return _Collection.Remove(Key);
		}
		#endregion

		#region IGroupConstant
		/// <summary>
		/// Determines whether the group contains the specific key.
		/// </summary>
		/// <param name="Key">The key.</param>
		public bool Contains(string Key) {
			// Return the status indicating whether the group contains the specific key.
			return _Collection.ContainsKey(Key);
		}

		/// <summary>
		/// Gets the number of contained elements.
		/// </summary>
		public int Count {
			get {
				// Return the number of contained elements.
				return _Collection.Count;
			}
		}
		#endregion

		#region IEnumerable
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<string> GetEnumerator() {
			// Return an enumerator that iterates through the collection.
			return _Collection.Keys.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() {
			// Return an enumerator that iterates through the collection.
			return (IEnumerator) GetEnumerator();
		}
		#endregion
	}
}