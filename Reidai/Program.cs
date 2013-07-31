// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using Chiisana;
using Chiisana.Hosting.Self;
using Chiisana.WebSocket;
using System;

namespace Reidai {
	/// <summary>
	/// Represents the application.
	/// </summary>
	class Program {
		/// <summary>
		/// The entry point of the application.
		/// </summary>
		/// <param name="Arguments">Each command line argument.</param>
		static void Main(string[] Arguments) {
			// Initialize a new instance of the HttpServer class.
			HttpServer HttpServer = new HttpServer(80);
			// Initialize a new instance of the WsServer class.
			WsServer WsServer = new WsServer();

			WsServer.Open += (WebSocket) => {
				Console.WriteLine("Connection open from {0}", WebSocket.Request.UserHostAddress);
				WebSocket.Message += (Message) => {
					if (Message is string) {
						Console.WriteLine(Message);
					}
				};
				WebSocket.Close += () => {
					Console.WriteLine("Connection close from {0}", WebSocket.Request.UserHostAddress);
				};
			};

			// Set the chained middleware ...
			HttpServer.Middleware = new Middleware(
				// ... with the web socket implementation ...
				WsServer,
				// ... with file serving from resources ...
				new MiddlewareResource(new string[] { "index.htm", "index.html" }, "Root"),
				// ... with a default handler.
				new Activity(async (Context) => {
					// Write to the output stream and release the context.
					await Context.SendAsync("Hello world!");
					// Return true to indicate this middleware claimed the request.
					return true;
				}));
			// Run the server.
			HttpServer.RunAsync().Wait();
		}
	}
}