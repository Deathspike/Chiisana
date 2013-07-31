// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chiisana.Extension {
	/// <summary>
	/// Represents the class providing extensions for the IContext interface.
	/// </summary>
	public static class ExtensionForContext {
		#region Methods
		/// <summary>
		/// Write content to the stream and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="Content">The content.</param>
		public static async Task<bool> ContentAsync(this IContext Context, string Content) {
			// Write content to the stream and dispose of the context.
			return await Context.ContentAsync(Content, null, null);
		}

		/// <summary>
		/// Write content to the stream and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="Content">The content.</param>
		/// <param name="ContentType">The MiME type of the content.</param>
		public static async Task<bool> ContentAsync(this IContext Context, string Content, string ContentType) {
			// Write content to the stream and dispose of the context.
			return await Context.ContentAsync(Content, ContentType, null);
		}

		/// <summary>
		/// Write content to the stream and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="Content">The content.</param>
		/// <param name="ContentType">The MiME type of the content.</param>
		/// <param name="Encoding">The encoding of the content.</param>
		public static async Task<bool> ContentAsync(this IContext Context, string Content, string ContentType, Encoding Encoding) {
			// Check if the content is valid.
			if (!string.IsNullOrEmpty(Content)) {
				// Set the character set of the ouput stream.
				Context.Response.ContentEncoding = Encoding ?? Context.Response.ContentEncoding ?? Encoding.UTF8;
				// Set the MiME type of the output strema.
				Context.Response.ContentType = ContentType ?? Context.Response.ContentType ?? "text/html";
				// Write to the stream and dispose of the context.
				await Context.SendAsync(Content);
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Write a file to the stream and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="IsAttachment">Indicates whether the file is an attachment.</param>
		/// <param name="Name">The name.</param>
		/// <param name="Paths">Each path.</param>
		public static async Task<bool> FileAsync(this IContext Context, bool IsAttachment, string Name, params string[] Paths) {
			// Check if the name is valid.
			if (!string.IsNullOrEmpty(Name)) {
				// Remove the slash from the name.
				Name = Name.TrimStart('/');
				// Iterate through each path.
				foreach (string CurrentPath in Paths) {
					// Retrieve the file path.
					string FilePath = Regex.Replace(Path.Combine(CurrentPath, Name), @"\\|/", Path.DirectorySeparatorChar.ToString());
					// Check if the file exists.
					if (File.Exists(FilePath)) {
						// Check if the file is an attachment.
						if (IsAttachment) {
							// Set the disposition header.
							Context.Response.Headers["Content-Disposition"] = string.Format("attachment; filename*=UTF-8''{0}", Name);
						}
						// Set the content type.
						Context.Response.ContentType = MiME.Find(Path.GetExtension(Name));
						// Open the file stream.
						using (Stream Stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
							// Write to the stream and dispose of the context.
							await Context.SendAsync(Stream);
							// Return true.
							return true;
						}
					}
				}
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Write a redirect location and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="Location">The location.</param>
		public static async Task<bool> RedirectTo(this IContext Context, string Location) {
			// Check if the location is valid.
			if (!string.IsNullOrEmpty(Location)) {
				// Set the value of the location header.
				Context.Response.RedirectLocation = Location;
				// Release the context.
				await Context.ReleaseAsync();
				// Return true.
				return true;
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Write a resource to the stream and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="IsAttachment">Indicates whether the file is an attachment.</param>
		/// <param name="Assembly">The assembly.</param>
		/// <param name="Name">The name.</param>
		/// <param name="Paths">Each path.</param>
		public static async Task<bool> ResourceAsync(this IContext Context, bool IsAttachment, Assembly Assembly, string Name, params string[] Paths) {
			// Iterate through each path.
			foreach (string CurrentPath in Paths) {
				// Retrieve the stream.
				using (Stream ResourceStream = Assembly.GetNormalizedManifestResourceStream(Regex.Replace(CurrentPath, @"\\|/", ".") + Name)) {
					// Check if the stream exists.
					if (ResourceStream != null) {
						// Check if the file is an attachment.
						if (IsAttachment) {
							// Set the disposition header.
							Context.Response.Headers["Content-Disposition"] = string.Format("attachment; filename*=UTF-8''{0}", Name);
						}
						// Set the MiME type.
						Context.Response.ContentType = MiME.Find(Path.GetExtension(Name));
						// Write to the stream and dispose of the context.
						await Context.SendAsync(ResourceStream);
						// Return true.
						return true;
					}
				}
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Write a resource to the stream and dispose of the context.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		/// <param name="IsAttachment">Indicates whether the file is an attachment.</param>
		/// <param name="Name">The name.</param>
		/// <param name="Paths">Each path.</param>
		public static async Task<bool> ResourceAsync(this IContext Context, bool IsAttachment, string Name, params string[] Paths) {
			// Write a resource to the stream and dispose of the context.
			return await Context.ResourceAsync(IsAttachment, Assembly.GetCallingAssembly(), Name, Paths);
		}
		#endregion
	}
}