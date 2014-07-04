// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using System.Reflection;

namespace Chiisana.Extension {
	/// <summary>
	/// Represents the class providing extensions for the Assembly class.
	/// </summary>
	internal static class ExtensionForAssembly {
		#region Methods
		/// <summary>
		/// Loads the specified manifest resource from this assembly.
		/// </summary>
		/// <param name="Assembly">The assembly.</param>
		/// <param name="Path">The case-sensitive name of the manifest resource being requested.</param>
		public static Stream GetNormalizedManifestResourceStream(this Assembly Assembly, string Path) {
			// Split the name to retrieve each segment.
			string[] Segment = Path.Replace('\\', '/').Split('/');
			// Iterate through each segment.
			for (int i = 0; i < Segment.Length - 1; i++) {
				// Append the directory seperation
				Segment[i] += '/';
			}
			// Write a resource to the stream and close the connection.
			return Assembly.GetNormalizedManifestResourceStream(Segment);
		}

		/// <summary>
		/// Loads the specified manifest resource from this assembly.
		/// </summary>
		/// <param name="Assembly">The assembly.</param>
		/// <param name="Path">The path.</param>
		public static Stream GetNormalizedManifestResourceStream(this Assembly Assembly, params string[] Path) {
			// Initialize the variables for the dot- and find position.
			int CurrentPosition, SeparatorPosition;
			// Initialize the result.
			string Result = Assembly.GetName().Name + '.';
			// Iterate through each segment.
			for (int i = 0; i < Path.Length; i++) {
				// Find the current position of a dot.
				CurrentPosition = Path[i].IndexOf('.');
				// Check if this segment indicates a folder.
				if (i < Path.Length - 1) {
					// Check if a dashing character can be found.
					if ((SeparatorPosition = Path[i].IndexOf('-')) != -1 && CurrentPosition != -1) {
						// Set the segment to cause each occurrence to be appended with an underscore.
						Path[i] = Path[i].Substring(0, SeparatorPosition + 1) + Path[i].Substring(SeparatorPosition + 1).Replace(".", "._");
					}
					// Replace each dash with an underscore.
					Path[i] = Path[i].Replace('-', '_');
				}
				// Replace each slash with a dot.
				Result += Path[i].Replace('/', '.');
			}
			// Return the specified manifest resource from the assembly.
			return Assembly.GetManifestResourceStream(Result);
		}
		#endregion
	}
}