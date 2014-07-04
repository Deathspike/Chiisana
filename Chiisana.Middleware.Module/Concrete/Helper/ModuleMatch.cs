// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Chiisana {
	/// <summary>
	/// Represents a module match.
	/// </summary>
	internal sealed class ModuleMatch {
		#region Properties
		/// <summary>
		/// Contains the regular expression.
		/// </summary>
		public Regex Expression { get; set; }

		/// <summary>
		/// Indicates whether this is the default action.
		/// </summary>
		public bool IsDefaultAction { get; set; }

		/// <summary>
		/// Indicates whether this is the default module.
		/// </summary>
		public bool IsDefaultModule { get; set; }

		/// <summary>
		/// Indicates whether this route uses default routing.
		/// </summary>
		public bool IsDefaultRouting { get; set; }

		/// <summary>
		/// Contains each filter.
		/// </summary>
		public CustomAttributeData[] Filters { get; set; }

		/// <summary>
		/// Contains each parameter.
		/// </summary>
		public ParameterInfo[] Parameters { get; set; }

		/// <summary>
		/// Contains the target.
		/// </summary>
		public MethodInfo Target { get; set; }

		/// <summary>
		/// Contains the type.
		/// </summary>
		public Type Type { get; set; }
		#endregion
	}
}