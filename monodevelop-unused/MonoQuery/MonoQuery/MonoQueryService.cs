//
// MonoQueryService.cs
//
// Authors:
//   Christian Hergert <chris@mosaix.net>
//
// Copyright (c) 2005 Christian Hergert
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

using Mono.Data.Sql;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Core.Gui;

namespace MonoQuery
{
	public class MonoQueryService : AbstractService
	{
		DbProviderCollection providers = null;
		SqlDefinitionPad definitionPad = null;
		
		static string serializedFile = Environment.GetFolderPath (
				Environment.SpecialFolder.ApplicationData)
				+ System.IO.Path.DirectorySeparatorChar
				+ "MonoDevelop"
				+ System.IO.Path.DirectorySeparatorChar
				+ "MonoQuery-Providers.xml";
		
		public MonoQueryService () : base () {}
		
		public DbProviderCollection Providers {
			get {
				if (providers == null)
					providers = new DbProviderCollection ();
				
				return providers;
			}
		}
		
		public Type[] ProviderTypes {
			get {
				ArrayList types = new ArrayList ();
				Assembly asm = Assembly.GetAssembly (typeof (Mono.Data.Sql.DbProviderBase));
				foreach (Type type in asm.GetTypes ()) {
					if (type.IsSubclassOf (typeof (DbProviderBase)))
						types.Add (type);
				}
				return (Type[]) types.ToArray (typeof (Type));
			}
		}
		
		public SqlDefinitionPad SqlDefinitionPad {
			get {
				return definitionPad;
			}
			set {
				definitionPad = value;
			}
		}
		
		public override void InitializeService ()
		{
			if (File.Exists (serializedFile)) {
				try {
					using (FileStream fs = File.OpenRead (serializedFile)) {
						XmlSerializer serializer
							= new XmlSerializer (typeof (DbProviderCollection));
						providers
							= (DbProviderCollection) serializer.Deserialize (fs);
					}
				} catch {
					Runtime.LoggingService.Error ("Invalid monoquery file.");
					Runtime.FileService.DeleteFile (serializedFile);
				}
			}
		}
		
		public override void UnloadService ()
		{
			if(File.Exists(serializedFile)) {
				using (FileStream fs = new FileStream(serializedFile, FileMode.Create)) {
					XmlSerializer serializer = new XmlSerializer (typeof (DbProviderCollection));
					serializer.Serialize (fs, providers);
				}
			}
		}
	}
}
