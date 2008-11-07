//  IEditAction.cs
//
//  This file was derived from a file from #Develop. 
//
//  Copyright (C) 2001-2007 Mike Krüger <mkrueger@novell.com>
// 
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using MonoDevelop.TextEditor.Document;

using Gdk;

namespace MonoDevelop.TextEditor.Actions
{
	/// <summary>
	/// To define a new key for the textarea, you must write a class which
	/// implements this interface.
	/// </summary>
	public interface IEditAction
	{
		/// <value>
		/// An array of keys on which this edit action occurs.
		/// </value>
		Gdk.Key[] Keys {
			get;
			set;
		}
		
		/// <remarks>
		/// When the key which is defined per XML is pressed, this method will be launched.
		/// </remarks>
		void Execute(TextArea textArea);
	}
	
	/// <summary>
	/// To define a new key for the textarea, you must write a class which
	/// implements this interface.
	/// </summary>
	public abstract class AbstractEditAction : IEditAction
	{
		Gdk.Key[] keys = null;
		
		/// <value>
		/// An array of keys on which this edit action occurs.
		/// </value>
		public Gdk.Key[] Keys {
			get {
				return keys;
			}
			set {
				keys = value;
			}
		}
		
		/// <remarks>
		/// When the key which is defined per XML is pressed, this method will be launched.
		/// </remarks>
		public abstract void Execute(TextArea textArea);
	}		
}
