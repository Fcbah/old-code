//
// GuiBuilderDisplayBinding.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Codons;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace GladeAddIn.Gui
{
	public class GuiBuilderDisplayBinding: IDisplayBinding
	{
		bool excludeThis = false;
		
		public virtual bool CanCreateContentForFile (string fileName)
		{
			if (excludeThis) return false;
			
			if (GetWindow (fileName) == null)
				return false;
			
			excludeThis = true;
			IDisplayBinding db = IdeApp.Workbench.DisplayBindings.GetBindingPerFileName (fileName);
			excludeThis = false;
			return db != null;
		}

		public virtual bool CanCreateContentForMimeType (string mimetype)
		{
			return false;
		}
		
		public virtual IViewContent CreateContentForFile (string fileName)
		{
			excludeThis = true;
			IDisplayBinding db = IdeApp.Workbench.DisplayBindings.GetBindingPerFileName (fileName);
			GuiBuilderView view = new GuiBuilderView (db.CreateContentForFile (fileName), GetWindow (fileName));
			excludeThis = false;
			view.Load (fileName);
			return view;
		}
		
		public virtual IViewContent CreateContentForMimeType (string mimeType, System.IO.Stream content)
		{
			return null;
		}
		
		GuiBuilderWindow GetWindow (string file)
		{
			if (IdeApp.ProjectOperations.CurrentOpenCombine == null)
				return null;

			Project project = null;
			foreach (Project p in IdeApp.ProjectOperations.CurrentOpenCombine.GetAllProjects ()) {
				if (p.IsFileInProject (file)) {
					project = p;
					break;
				}
			}
			
			if (project == null)
				return null;

			GuiBuilderProject[] gprojects = GladeService.GetGuiBuilderProjects (project);
			foreach (GuiBuilderProject gproject in gprojects) {
				foreach (GuiBuilderWindow win in gproject.Windows) {
					if (win.SourceCodeFile == file)
						return win;
				}
			}

			return null;
		}
	}
}
