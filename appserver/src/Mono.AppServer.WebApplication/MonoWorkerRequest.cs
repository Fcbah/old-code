// Revision 1.12, Thu Feb 6 18:49:17 2003 UTC by gonzalo 
// Modifications by Brian Ritchie (blr)
// - made mappedPath protected

/* ====================================================================
 * The Apache Software License, Version 1.1
 *
 * Copyright (c) 2002 Daniel Lopez Ridruejo.
 *           (c) 2002 Ximian, Inc.
 *           All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution.
 *
 * 3. The end-user documentation included with the redistribution,
 *    if any, must include the following acknowledgment:
 *       "This product includes software developed by 
 *        Daniel Lopez Ridruejo (daniel@rawbyte.com) and
 *        Ximian Inc. (http://www.ximian.com)"
 *    Alternately, this acknowledgment may appear in the software itself,
 *    if and wherever such third-party acknowledgments normally appear.
 *
 * 4. The name "mod_mono" must not be used to endorse or promote products 
 *    derived from this software without prior written permission. For written
 *    permission, please contact daniel@rawbyte.com.
 *
 * 5. Products derived from this software may not be called "mod_mono",
 *    nor may "mod_mono" appear in their name, without prior written
 *    permission of Daniel Lopez Ridruejo and Ximian Inc.
 *
 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED.  IN NO EVENT SHALL DANIEL LOPEZ RIDRUEJO OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 * ====================================================================
 *
 */
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Mono.ASPNET
{
	public abstract class MonoWorkerRequest : SimpleWorkerRequest
	{
		IApplicationHost appHost;
		Hashtable headers;
		string [][] unknownHeaders;
		ArrayList response;
		Encoding encoding;
		protected string mappedPath; //blr
		byte [] queryStringBytes;
        
        [Obsolete("Try using Math.Min")]
        private static long Min(long val1, long val2)
        {
          return (val1 < val2)? val1: val2;
        }

		public MonoWorkerRequest (IApplicationHost appHost)
			: base (String.Empty, String.Empty, null)
		{
			if (appHost == null)
				throw new ArgumentNullException ("appHost");

			this.appHost = appHost;
			response = new ArrayList ();
		}

		protected virtual Encoding Encoding 
		{
			get 
			{
				if (encoding == null)
					encoding = new UTF8Encoding (false);

				return encoding;
			}

			set { encoding = value; }
		}

		public override string GetAppPath ()
		{
			if (appHost.VPath == "/")
				return "";

			return appHost.VPath;
		}

		public override string GetAppPathTranslated ()
		{
			return appHost.Path;
		}

		public override string GetFilePathTranslated ()
		{
			if (mappedPath == null)
				mappedPath = MapPath (GetFilePath ());

			return mappedPath;
		}

		public override string GetLocalAddress ()
		{
			return "localhost";
		}

		public override int GetLocalPort ()
		{
			return 0;
		}

		public override string GetPathInfo ()
		{
			return "GetPathInfo"; //???
		}

		public override byte [] GetPreloadedEntityBody ()
		{
			return null;
		}

		public override byte [] GetQueryStringRawBytes ()
		{
			if (queryStringBytes == null) 
			{
				string queryString = GetQueryString ();
				if (queryString != null)
					queryStringBytes = Encoding.GetBytes (queryString);
			}

			return queryStringBytes;
		}

		public override string GetRawUrl ()
		{
			string queryString = GetQueryString ();
			string path = GetFilePath ();
			if (queryString != null && queryString.Length > 0)
				return path + "?" + queryString;

			return path;
		}

		public override string GetUnknownRequestHeader (string name)
		{
			if (headers == null)
				return null;

			return headers [name] as string;
		}

		public override string [][] GetUnknownRequestHeaders ()
		{
			if (unknownHeaders == null) 
			{
				if (headers == null)
					return (unknownHeaders = new string [0][]);

				ICollection keysColl = headers.Keys;
				ICollection valuesColl = headers.Values;
				string [] keys = new string [keysColl.Count];
				string [] values = new string [valuesColl.Count];
				keysColl.CopyTo (keys, 0);
				valuesColl.CopyTo (values, 0);

				int count = keys.Length;
				ArrayList pairs = new ArrayList ();
				for (int i = 0; i < count; i++) 
				{
					int index = HttpWorkerRequest.GetKnownRequestHeaderIndex (keys [i]);
					if (index == -1)
						continue;
					pairs.Add (new string [] { keys [i], values [i]});
				}
				
				if (pairs.Count != 0) 
				{
					unknownHeaders = new string [pairs.Count][];
					for (int i = 0; i < pairs.Count; i++)
						unknownHeaders [i] = (string []) pairs [i];
					//unknownHeaders = (string [][]) pairs.ToArray (typeof (string [][]));
				}
			}

			return unknownHeaders;
		}

		public override string MapPath (string path)
		{
			if (path == null || path.Length == 0 || path == "/")
				return appHost.Path.Replace ('/', Path.DirectorySeparatorChar);

			int len = path.Length;
			if (path [0] == '/' && len > 1) 
			{
				path = path.Substring (1);
			} 
			else if (path [0] == '~' && len > 2 && path [1] == '/') 
			{
				path = path.Substring (2);
			}
				
			return Path.Combine (appHost.Path, path.Replace ('/', Path.DirectorySeparatorChar));
		}

		protected abstract bool GetRequestData ();

		public void ProcessRequest ()
		{
			if (!GetRequestData ())
				return;

			HttpRuntime.ProcessRequest (this);
		}

		public override void SendCalculatedContentLength (int contentLength)
		{
			//FIXME: Should we ignore this for apache2?
			SendUnknownResponseHeader ("Content-Length", contentLength.ToString ());
		}

		public override void SendKnownResponseHeader (int index, string value)
		{
			if (HeadersSent ())
				return;

			string headerName = HttpWorkerRequest.GetKnownResponseHeaderName (index);
			SendUnknownResponseHeader (headerName, value);
		}

		private void SendStream (Stream stream, long offset, long length)
		{
			if (offset < 0 || length <= 0)
				return;
			
			long stLength = stream.Length;
			if (offset + length > stLength)
				length = stLength - offset;

			if (offset > 0)
				stream.Seek (offset, SeekOrigin.Begin);

			byte [] fileContent = new byte [8192];
			int count = fileContent.Length;
			while (length > 0 && (count = stream.Read (fileContent, 0, count)) != 0) 
			{
				SendResponseFromMemory (fileContent, count);
				length -= count;
				count = (int) Min ((int)length, (int)fileContent.Length);
			}
		}

		public override void SendResponseFromFile (string filename, long offset, long length)
		{
			Stream file = null;
			try 
			{
				file = File.OpenRead (filename);
				SendStream (file, offset, length);
			} 
			finally 
			{
				if (file != null)
					file.Close ();
			}
		}

		public override void SendResponseFromFile (IntPtr handle, long offset, long length)
		{
			Stream file = null;
			try 
			{
				file = new FileStream (handle, FileAccess.Read);
				SendStream (file, offset, length);
			} 
			finally 
			{
				if (file != null)
					file.Close ();
			}
		}
	}
}


