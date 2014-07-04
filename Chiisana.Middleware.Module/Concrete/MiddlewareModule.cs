// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chiisana {
	/// <summary>
	/// Represents middleware for module hosting.
	/// </summary>
	public sealed class MiddlewareModule : IMiddleware {
		/// <summary>
		/// Contains the default action.
		/// </summary>
		private readonly string _DefaultAction;

		/// <summary>
		/// Contains the default module.
		/// </summary>
		private readonly string _DefaultModule;

		/// <summary>
		/// Contains inversion-of-control.
		/// </summary>
		private readonly Func<Type, object> _IoC;

		/// <summary>
		/// Contains the result property.
		/// </summary>
		private readonly PropertyInfo _Result;

		/// <summary>
		/// Contains a collection of routes.
		/// </summary>
		private readonly Dictionary<string, ModuleMatch[]> _Routes;

		/// <summary>
		/// Contains a collection of loaded types.
		/// </summary>
		private readonly HashSet<Type> _Types;

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the MiddlewareModule class.
		/// </summary>
		public MiddlewareModule()
			: this(null) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareModule class.
		/// </summary>
		/// <param name="IoC">The inversion-of-control.</param>
		public MiddlewareModule(Func<Type, object> IoC)
			: this(null, null, IoC) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareModule class.
		/// </summary>
		/// <param name="DefaultModule">The default module.</param>
		/// <param name="DefaultAction">The default action.</param>
		public MiddlewareModule(string DefaultModule, string DefaultAction)
			: this(DefaultModule, DefaultAction, null) {
			// Stop the function.
			return;
		}

		/// <summary>
		/// Initialize a new instance of the MiddlewareModule class.
		/// </summary>
		/// <param name="DefaultModule">The default module.</param>
		/// <param name="DefaultAction">The default action.</param>
		/// <param name="IoC">The inversion-of-control.</param>
		public MiddlewareModule(string DefaultModule, string DefaultAction, Func<Type, object> IoC) {
			// Set the default action.
			_DefaultAction = DefaultAction;
			// Set the default module.
			_DefaultModule = DefaultModule;
			// Set inversio-of-control.
			_IoC = IoC ?? Activator.CreateInstance;
			// Initialize a new instance of the Dictionary class.
			_Routes = new Dictionary<string, ModuleMatch[]>();
			// Set the result property.
			_Result = typeof(Task<object>).GetProperty("Result");
			// Initialize a new instance of the HashSet class.
			_Types = new HashSet<Type>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Load modules for the calling assembly.
		/// </summary>
		/// <param name="ModuleRouting">The module routing scheme.</param>
		public MiddlewareModule Load(ModuleRouting ModuleRouting) {
			// Load modules for the assembly.
			return Load(ModuleRouting, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Load modules for the assembly.
		/// </summary>
		/// <param name="ModuleRouting">The module routing scheme.</param>
		/// <param name="Assembly">The assembly.</param>
		public MiddlewareModule Load(ModuleRouting ModuleRouting, Assembly Assembly) {
			// Load a module for each type.
			return Load(ModuleRouting, Assembly.GetTypes());
		}

		/// <summary>
		/// Load a module for the type.
		/// </summary>
		/// <param name="ModuleRouting">The module routing scheme.</param>
		/// <param name="Type">The type.</param>
		public MiddlewareModule Load(ModuleRouting ModuleRouting, Type Type) {
			// Check if the type has not been loaded and implements the IModule interface.
			if (!_Types.Contains(Type) && Type.GetInterface(typeof(IModule).FullName) != null) {
				// Lock this object.
				lock (this) {
					// Check if the type has not been loaded
					if (!_Types.Contains(Type)) {
						// Retrieve the module name.
						string Name = Type.Name;
						// Iterate through each removable suffix.
						foreach (string Suffix in new string[] { "Api", "Controller", "Module" }) {
							// Check if the module name ends with the suffix.
							if (Name.Length > Suffix.Length && Name.EndsWith(Suffix)) {
								// Remove the suffix.
								Name = Name.Remove(Name.Length - Suffix.Length);
							}
						}
						// Process the module.
						if (true) {
							// Initialize whether this is the default module.
							bool IsDefaultModule = string.Compare(_DefaultModule, Name, StringComparison.InvariantCultureIgnoreCase) == 0;
							// Initialize a new instance of the Dictionary class.
							Dictionary<string, List<ModuleMatch>> Routes = new Dictionary<string, List<ModuleMatch>>();
							// Iterate through each method.
							foreach (MethodInfo Method in Type.GetMethods(BindingFlags.Instance | BindingFlags.Public)) {
								// Initialize the handlers.
								CustomAttributeData[] HandlerAttributes = Method.CustomAttributes.Where(x => !x.AttributeType.IsAbstract && x.AttributeType.GetInterface(typeof(IModuleHandler).FullName) != null).ToArray();
								// Check if a handler has been found.
								if (HandlerAttributes.Length != 0) {
									// Initialize the filters.
									CustomAttributeData[] FilterAttributes = Method.CustomAttributes.Union(Method.DeclaringType.CustomAttributes).Where(x => x.AttributeType.IsPublic && x.AttributeType.GetInterface(typeof(IMiddleware).FullName) != null).ToArray();
									// Retrieve the parameters.
									ParameterInfo[] Parameters = Method.GetParameters();
									// Check if model binding is not possible due to having too many model binding candidates.
									if (Parameters.Where(x => !x.ParameterType.Equals(typeof(string)) && (x.ParameterType.IsClass || x.ParameterType.IsInterface)).Count() > 1) {
										// Throw an exception.
										throw new NotSupportedException(string.Format("The method {0} in class {1} has multiple parameters for model binding.", Method.Name, Type.Name));
									}
									// Iterate through each handler.
									foreach (CustomAttributeData HandlerAttribute in HandlerAttributes) {
										// Initialize a new instance of the IModuleHandler interface.
										IModuleHandler Handler = HandlerAttribute.Constructor.Invoke(HandlerAttribute.ConstructorArguments.Select(x => x.Value).ToArray()) as IModuleHandler;
										// Initialize the data transfer method (such as GET, POST or HEAD).
										string HttpMethod = string.IsNullOrWhiteSpace(Handler.HttpMethod) ? "GET" : Handler.HttpMethod.ToUpperInvariant();
										// Initialize whether this is the default action.
										bool IsDefaultAction = string.Compare(_DefaultAction, Method.Name, StringComparison.InvariantCultureIgnoreCase) == 0;
										// Initialize a new instance of the StringBuilder class.
										StringBuilder StringBuilder = new StringBuilder();
										// Iterate through each named argument.
										foreach (CustomAttributeNamedArgument CustomAttributeNamedArgument in HandlerAttribute.NamedArguments) {
											// Set the value for the named argument.
											typeof(IModuleHandler).GetProperty(CustomAttributeNamedArgument.MemberName).SetValue(Handler, CustomAttributeNamedArgument.TypedValue.Value);
										}
										// Check if this is a complete pattern and use it as it is.
										if (!string.IsNullOrWhiteSpace(Handler.Pattern) && Handler.Pattern.StartsWith("^")) {
											// Append the pattern.
											StringBuilder.Append(Handler.Pattern);
										} else {
											// Append the beginning.
											StringBuilder.Append("^");
											// Check if this is not the default module.
											if (!IsDefaultModule) {
												// Iterate through each camel case word in the module name.
												foreach (string ModulePiece in Regex.Replace(Name, "([A-Z])", "\n$1").Split('\n')) {
													// Check if the module piece is valid.
													if (!string.IsNullOrWhiteSpace(ModulePiece)) {
														// Append the controller routing expression.
														StringBuilder.Append(string.Format("(/{0})", ModuleRouting == ModuleRouting.StrictAndLowerCase ? ModulePiece.ToLowerInvariant() : ModulePiece));
													}
												}
											}
											// Check if the pattern has not been provided.
											if (string.IsNullOrWhiteSpace(Handler.Pattern)) {
												// Check if this is not the default action.
												if (!IsDefaultAction && !Handler.Api) {
													// Append the action routing expression.
													StringBuilder.Append(string.Format("(/{0})", ModuleRouting == ModuleRouting.StrictAndLowerCase ? Method.Name.ToLowerInvariant() : Method.Name));
												}
												// Iterate through each parameter.
												foreach (ParameterInfo Parameter in Parameters) {
													// Check if the parameter is a string or primitive.
													if (Parameter.ParameterType == typeof(string) || Parameter.ParameterType.IsPrimitive) {
														// Append the parameter routing expression.
														StringBuilder.Append(string.Format("(?<{0}>/{1}){2}", Parameter.Name, "[A-Za-z0-9-$_.+!*'()]+", (Parameter.IsOptional ? "?" : null)));
													}
												}
												// Check if the routing is flexible.
												if (ModuleRouting == ModuleRouting.Flexible) {
													// Append the optional trailing slash.
													StringBuilder.Append("/?");
												} else {
													// Append the trailing slash.
													StringBuilder.Append("/");
												}
											} else {
												// Append the pattern.
												StringBuilder.Append(Handler.Pattern);
											}
											// Append the ending.
											StringBuilder.Append("$");
										}
										// Check if the routes does not contain the data transfer method.
										if (!Routes.ContainsKey(HttpMethod)) {
											// Initialize a new instance of the List class.
											Routes[HttpMethod] = new List<ModuleMatch>();
										}
										// Add the route.
										Routes[HttpMethod].Add(new ModuleMatch {
											// Set the regular expression.
											Expression = new Regex(StringBuilder.ToString(), RegexOptions.Compiled | RegexOptions.ExplicitCapture | (ModuleRouting == ModuleRouting.Flexible ? RegexOptions.IgnoreCase : 0)),
											// Set each filter.
											Filters = FilterAttributes,
											// Set the whether this is the default action.
											IsDefaultAction = IsDefaultAction,
											// Set the whether this is the default module.
											IsDefaultModule = IsDefaultModule,
											// Set the whether this route uses default routing.
											IsDefaultRouting = string.IsNullOrWhiteSpace(Handler.Pattern),
											// Set each parameter.
											Parameters = Parameters,
											// Set the target.
											Target = Method,
											// Set the type.
											Type = Type
										});
									}
								}
							}
							// Iterate through each key.
							foreach (string Key in Routes.Keys) {
								// Check if the routes contains the key ...
								IEnumerable<ModuleMatch> ModuleMatch = _Routes.ContainsKey(Key)
									// ... and select an union of both collections when it does ...
									? _Routes[Key].Union(Routes[Key])
									// ... or use the collection as it is.
									: Routes[Key];
								// Set the routes for the key ...
								_Routes[Key] = ModuleMatch
									// ... order the routes to attempt the default module after other routes ...
									.OrderBy(x => x.IsDefaultModule)
									// ... order the routes to attempt the default action after other routes ...
									.OrderBy(x => x.IsDefaultAction)
									// ... and create an array.
									.ToArray();
							}
							// Add the type.
							_Types.Add(Type);
						}
					}
				}
			}
			// Return this object.
			return this;
		}

		/// <summary>
		/// Load a module for each type.
		/// </summary>
		/// <param name="ModuleRouting">The module routing scheme.</param>
		/// <param name="Types">The types.</param>
		public MiddlewareModule Load(ModuleRouting ModuleRouting, Type[] Types) {
			// Iterate through each type.
			foreach (Type Type in Types) {
				// Load a module for the type.
				Load(ModuleRouting, Type);
			}
			// Return this object.
			return this;
		}
		#endregion

		/// <summary>
		/// Handle the HTTP request.
		/// </summary>
		/// <param name="Context">The HTTP context.</param>
		public async Task<bool> HandleAsync(IContext Context) {
			// Retrieve the method.
			string HttpMethod = Context.Request.HttpMethod.ToUpperInvariant();
			// Check if the method is available.
			if (_Routes.ContainsKey(HttpMethod)) {
				// Iterate through each available route.
				foreach (ModuleMatch ModuleMatch in _Routes[HttpMethod]) {
					// Retrieve the match.
					Match Match = ModuleMatch.Expression.Match(Context.Request.Url.LocalPath);
					// Check if this route matches the request.
					if (Match.Success) {
						// Initialize the arguments.
						object[] Arguments = new object[ModuleMatch.Parameters.Length];
						// Iterate through each parameter.
						for (int i = 0; i < ModuleMatch.Parameters.Length; i++) {
							// Initialize the parameter.
							ParameterInfo Parameter = ModuleMatch.Parameters[i];
							// Initialize the parameter type.
							Type ParameterType = Parameter.ParameterType;

							/*// Set the boolean indicating whether this is a string.
							bool IsString = ParameterType.Equals(typeof(string));
							// Check the parameter type.
							if (!IsString && (ParameterType.IsClass || ParameterType.IsInterface)) {
								// Retrieve the value indicating whether this is a GET request.
								bool IsGet = string.Compare("GET", Context.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase) == 0;
								// Check if this is not a GET request and the request contains an object.
								if (!IsGet && string.Compare("application/json", Context.Request.ContentType, StringComparison.InvariantCultureIgnoreCase) == 0) {
									try {
										// Initialize a new instance of the MemoryStream class.
										using (MemoryStream MemoryStream = new MemoryStream()) {
											// Copy the contents of the incoming HTTP entity body to the memory stream.
											await Context.Request.InputStream.CopyToAsync(MemoryStream);
											// Create a binding for a type with the provided values.
											Arguments[i++] = JsonSerializer.DeserializeFromString(Context.Request.ContentEncoding.GetString(MemoryStream.ToArray()), ParameterType);
										}
									} catch {
										// Create a binding using default values.
										Arguments[i++] = _IoC(ParameterType);
									}
								} else {
									// Create a binding for a type with the provided values.
									Arguments[i++] = (IsGet ? Context.Request.QueryString : await Context.Request.GetFormAsync()).BindTo(ParameterType, _IoC);
								}
							} else {
								// Retrieve the value.
								string Value = Match.Groups[Parameter.Name].Value;
								// Check if the value is invalid and 
								if (string.IsNullOrWhiteSpace(Value)) {
									// Check if the parameter is optional.
									if (Parameter.IsOptional) {
										// Add the default value to the invocation.
										Arguments[i++] = Parameter.DefaultValue;
									} else {
										// Invalidate the invocation array.
										Arguments = null;
										// Break from the iteration.
										break;
									}
								} else {
									// Check if this is default routing.
									if (ModuleMatch.IsDefaultRouting) {
										// Remove the prefixed slash from the value.
										Value = Value.Substring(1);
									}
									// Check if this is a string.
									if (IsString) {
										// Add the value to the invocation.
										Arguments[i++] = Value;
									} else {
										// Attempt the following code.
										try {
											// Add the value to the invocation.
											Arguments[i++] = Value.ConvertUncheckedTo(ParameterType);
										} catch (NotSupportedException) {
											// Invalidate the invocation array.
											Arguments = null;
											// Break from the iteration.
											break;
										}
									}
								}
							}*/
						}
					}
				}
			}
			// Return false.
			return false;
		}

		public System.Threading.Tasks.Task ReleaseAsync() {
			throw new NotImplementedException();
		}
	}
}