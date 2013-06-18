// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chiisana.Hosting.Self {
	/// <summary>
	/// Represents a HTTP cookie collection.
	/// </summary>
	internal sealed class HttpCookieCollection : IGroup<ICookie> {
		/// <summary>
		/// Contains the collection.
		/// </summary>
		private readonly Dictionary<string, ICookie> _Collection;

		/// <summary>
		/// Contains the HTTP request context.
		/// </summary>
		private readonly IContextRequest _Request;

		/// <summary>
		/// Indicates whether each cookie is secure.
		/// </summary>
		private readonly bool _Secure;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the HttpCookieCollection class.
		/// </summary>
		/// <param name="Request">The HTTP request context.</param>
		/// <param name="Secure">Indicates whether each cookie applies exclusively to secure sockets (HTTPS).</param>
		public HttpCookieCollection(IContextRequest Request, bool Secure) {
			// Set the collection.
			_Collection = new Dictionary<string, ICookie>(StringComparer.OrdinalIgnoreCase);
			// Set the HTTP request context.
			_Request = Request;
			// Set the status indicating whether each cookie applies exclusively to secure sockets (HTTPS).
			_Secure = Secure;
		}
		#endregion

		#region IGroup
		/// <summary>
		/// Gets or sets the entry with the specified key.
		/// </summary>
		/// <param name="Key">The key.</param>
		public ICookie this[string Key] {
			get {
				// Determine whether the group does not contain the specified key.
				if (!Contains(Key)) {
					// Initialize a new instance of the HttpCookie class.
					HttpCookie HttpCookie = new HttpCookie(Key);
					// Set the status indicating the cookie is HTTP only.
					HttpCookie.HttpOnly = true;
					// Set the status indicating the cookie applies exclusively to secure sockets (HTTPS).
					HttpCookie.Secure = _Secure;
					// Check whether the specified key exists as a request cookie.
					if (_Request.Cookies.Contains(Key)) {
						// Set the value.
						HttpCookie.Value = _Request.Cookies[Key].Value;
					}
					// Add the cookie to the collection.
					_Collection[Key] = HttpCookie;			
					// Return the cookie.
					return HttpCookie;
				}
				// Return the cookie.
				return _Collection[Key];
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
			return (IEnumerator)GetEnumerator();
		}
		#endregion
	}
}