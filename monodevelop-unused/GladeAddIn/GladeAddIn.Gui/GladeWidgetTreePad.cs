//
// GladeWidgetTreePad.cs
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
using System.Collections;
using Gtk;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Components.Commands;

namespace GladeAddIn.Gui
{
	public class GladeWidgetTreePad: AbstractPadContent
	{
		TreeView tree;
		TreeStore store;
		Gladeui.Widget rootWidget;
		ScrolledWindow control;
		Hashtable items = new Hashtable ();
		TreeViewColumn col;
		bool selecting;
		
		public GladeWidgetTreePad (): base ("")
		{
			DefaultPlacement = "GladeAddIn.Gui.GladePropertiesPad/left; bottom";
			store = new Gtk.TreeStore (
				typeof (Gdk.Pixbuf), // image
				typeof (string),     // name
				typeof (Gladeui.Widget));    // widget

			tree = new TreeView (store);
			
			col = new TreeViewColumn ();
			CellRendererPixbuf pr = new CellRendererPixbuf ();
			col.PackStart (pr, false);
			col.AddAttribute (pr, "pixbuf", 0);
			
			CellRendererText crt = new CellRendererText ();
			col.PackStart (crt, true);
			col.AddAttribute (crt, "text", 1);
			
			tree.AppendColumn (col);
			tree.HeadersVisible = false;
			
			control = new ScrolledWindow ();
			control.ShadowType = Gtk.ShadowType.In;
			control.Add (tree);
			control.ShowAll ();
			
			GladeService.WidgetTreePad = this;
			
			tree.CursorChanged += new EventHandler (OnSelectionChanged);
			control.PopupMenu += new Gtk.PopupMenuHandler (OnPopupMenu);
			tree.ButtonPressEvent += new ButtonPressEventHandler (OnButtonPressed);
		}
		
		public void Fill (Gladeui.Widget w)
		{
			if (rootWidget != null) {
				rootWidget.Project.AddWidget -= new Gladeui.AddWidgetHandler (OnAddWidget);
				rootWidget.Project.RemoveWidget -= new Gladeui.RemoveWidgetHandler (OnRemoveWidget);
				rootWidget.Project.SelectionChangedEvent -= new EventHandler (OnWidgetSelectionChanged);
				rootWidget.Project.WidgetNameChangedEvent -= new Gladeui.WidgetNameChangedEventHandler (OnWidgetNameChanged);
			}
			
			rootWidget = w;
			store.Clear ();
			items.Clear ();
			
			if (w != null) {
				Gtk.TreeIter iter = store.AppendValues (w.Class.Icon, w.Name, w);
				items [w] = iter;
				AddChildren (iter, w);
				
				rootWidget.Project.AddWidget += new Gladeui.AddWidgetHandler (OnAddWidget);
				rootWidget.Project.RemoveWidget += new Gladeui.RemoveWidgetHandler (OnRemoveWidget);
				rootWidget.Project.SelectionChangedEvent += new EventHandler (OnWidgetSelectionChanged);
				rootWidget.Project.WidgetNameChangedEvent += new Gladeui.WidgetNameChangedEventHandler (OnWidgetNameChanged);
			}
		}
		
		void AddChildren (Gtk.TreeIter iter, Gladeui.Widget widget)
		{
			Gtk.Container container = widget.Object as Gtk.Container;
			if (container != null) {
				foreach (Gtk.Widget cw in container.Children) {
					Gladeui.Widget gw = Gladeui.Widget.FromObject (cw);
					if (gw != null) {
						Gtk.TreeIter citer = store.AppendValues (iter, gw.Class.Icon, gw.Name, gw);
						items [gw] = citer;
						AddChildren (citer, gw);
					}
				}
			}
		}
		
		public void Refresh ()
		{
			object ob = SaveStatus ();
			Fill (rootWidget);
			RestoreStatus (ob);
		}
		
		public object SaveStatus ()
		{
			TreeIter iter;
			
			if (!store.GetIterFirst (out iter))
				return null;
			ArrayList list = new ArrayList ();
			
			do {
				SaveStatus (list, iter);
			} while (store.IterNext (ref iter));
			
			return list;
		}
		
		public void RestoreStatus (object ob)
		{
			if (ob == null)
				return;
			foreach (TreePath path in (ArrayList)ob)
				tree.ExpandRow (path, false);			
		}
		
		void SaveStatus (ArrayList list, TreeIter iter)
		{
			Gtk.TreePath path = store.GetPath (iter);
			if (tree.GetRowExpanded (path))
				list.Add (path);
			else if (store.IterChildren (out iter, iter)) {
				do {
					SaveStatus (list, iter);
				} while (store.IterNext (ref iter));
			}
		}
		
		public override Gtk.Widget Control {
			get { return control; }
		}
		
		
		void OnSelectionChanged (object sender, EventArgs args)
		{
			Gladeui.Widget widget = GetSelectedWidget ();
			if (widget != null) {
				selecting = true;
				widget.Project.SelectionSet (widget.Object, true);
				selecting = false;
			}
		}
		
		public Gladeui.Widget GetSelectedWidget ()
		{
			Gtk.TreeModel foo;
			Gtk.TreeIter iter;
			if (!tree.Selection.GetSelected (out foo, out iter))
				return null;
			
			return (Gladeui.Widget) store.GetValue (iter, 2);
		}
		
		void OnAddWidget (object s, Gladeui.AddWidgetArgs args)
		{
			Refresh ();
		}
		
		void OnRemoveWidget (object s, Gladeui.RemoveWidgetArgs args)
		{
			Refresh ();
		}
		
		void OnWidgetSelectionChanged (object s, EventArgs a)
		{
			if (selecting)
				return;

			foreach (Gtk.Widget w in rootWidget.Project.SelectionGet ()) {
				Gladeui.Widget gw = Gladeui.Widget.FromObject (w);
				if (gw != null && items.Contains (gw)) {
					TreeIter iter = (TreeIter) items [gw];
					Gtk.TreePath path = store.GetPath (iter);
					tree.ExpandToPath (path);
					tree.Selection.SelectIter (iter);
					tree.SetCursor (store.GetPath (iter), col, false);
					return;
				}
			}
		}
		
		[GLib.ConnectBefore]
		void OnPopupMenu (object o, Gtk.PopupMenuArgs args)
		{
			ShowPopup ();
		}

		[GLib.ConnectBefore]
		void OnButtonPressed (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3)
				ShowPopup ();
		}
		
		void ShowPopup ()
		{
			CommandEntrySet eset = new CommandEntrySet ();
			eset.AddItem (EditCommands.Cut);
			eset.AddItem (EditCommands.Copy);
			eset.AddItem (EditCommands.Paste);
			eset.AddItem (EditCommands.Delete);
			IdeApp.CommandService.ShowContextMenu (eset);
		}
		
		void OnWidgetNameChanged (object s, Gladeui.WidgetNameChangedEventArgs a)
		{
			Refresh ();
		}
		
		[CommandHandler (EditCommands.Undo)]
		protected void OnUndo ()
		{
			GladeService.App.CommandUndo ();
		}
		
		[CommandHandler (EditCommands.Redo)]
		protected void OnRedo ()
		{
			GladeService.App.CommandRedo ();
		}
		
		[CommandHandler (EditCommands.Copy)]
		protected void OnCopy ()
		{
			GladeService.App.CommandCopy ();
		}
		
		[CommandHandler (EditCommands.Cut)]
		protected void OnCut ()
		{
			GladeService.App.CommandCut ();
		}
		
		[CommandHandler (EditCommands.Paste)]
		protected void OnPaste ()
		{
			GladeService.App.CommandPaste ();
		}
		
		[CommandHandler (EditCommands.Delete)]
		protected void OnDelete ()
		{
			GladeService.App.CommandDelete ();
		}
	}
}
