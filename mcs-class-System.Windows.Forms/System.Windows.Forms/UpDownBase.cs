//
// System.Windows.Forms.UpDownBase.cs
//
// Author:
//	 stubbed out by Stefan Warnke (StefanW@POBox.com)
//   Dennis Hayes (dennish@Raytek.com)
//   Gianandrea Terzi (gianandrea.terzi@lario.com)
//	 Alexandre Pigolkine (pigolkine@gxm.de)
//
// (C) Ximian, Inc., 2002/3
//

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
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	internal class SpinnerControl : Control {
		internal const int DEF_BUDDYBORDER = 2;
		internal LeftRightAlignment updownAlign;

		public SpinnerControl() {
			SubClassWndProc_ = true;
			TabStop = false;
			updownAlign = LeftRightAlignment.Right;
		}
		
		internal int AlignStyle {
			get { return updownAlign == LeftRightAlignment.Right ? 
				(int) UpDownControlStyles.UDS_ALIGNRIGHT : (int) UpDownControlStyles.UDS_ALIGNLEFT;
			}
		}


		internal LeftRightAlignment UpDownAlign {
			get {	return updownAlign; }
			set {	
				updownAlign = value;

				if ( IsHandleCreated )
					RecreateHandle ( );
			}
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams createParams = base.CreateParams;
	
				createParams.ClassName = "msctls_updown32";

				createParams.Style = (int) (
					WindowStyles.WS_CHILD | 
					WindowStyles.WS_VISIBLE);

				createParams.Style |= (int)( UpDownControlStyles.UDS_AUTOBUDDY ) | AlignStyle;
				return createParams;
			}
		}
		
		protected override void WndProc(ref Message m) { 
			switch ((Msg) m.Msg) {
			case Msg.WM_NOTIFY:
				NM_UPDOWN nmupdown = (NM_UPDOWN)Marshal.PtrToStructure ( m.LParam, typeof ( NM_UPDOWN ) );
				// With default setup 
				// NM_UPDOWN.iDelta < 0, then Up button pressed
				// NM_UPDOWN.iDelta > 0, then Down button pressed
				// CHECKME: do we need to call Up/Down Abs(delta) times ?
				UpDownBase parentUpDown = Parent as UpDownBase;
				if( parentUpDown != null) {
					if( nmupdown.iDelta < 0) {
						parentUpDown.UpButton();
					}
					else {
						parentUpDown.DownButton();
					}
				}
				else {
					CallControlWndProc(ref m);
				}
				break;
			case Msg.WM_HSCROLL:
			case Msg.WM_VSCROLL:
				CallControlWndProc(ref m);
				break;
			default:
				base.WndProc(ref m);
				break;
			}
		}
	}

	// <summary>
	// </summary>


	/// <summary>
	/// The up-down control consists of a text box and a small vertical scroll 
	/// bar, commonly referred to as a spinner control.
	/// </summary>
	public abstract class UpDownBase : ContainerControl {

		//UpDownBase+ButtonID
		enum ButtonID 
		{
			Down = 2, 
			None = 0, 
			Up = 1
		}

		internal TextBox	EditBox_;
		internal SpinnerControl	Spinner_;
		private bool		UserEdit_;
		private bool            interceptArrowKeys;

		/// --- Constructor ---
		public UpDownBase()	
		{
			UserEdit_ = false;
			Win32.InitCommonControls();
			EditBox_ = new TextBox();
			EditBox_.TextAlign = HorizontalAlignment.Left;
			EditBox_.TextChanged += new System.EventHandler( this.EditBox_TextChanged );
			EditBox_.KeyDown += new KeyEventHandler ( this.OnTextBoxKeyDown );
			EditBox_.KeyPress += new KeyPressEventHandler ( this.OnTextBoxKeyPress );
			EditBox_.LostFocus+= new System.EventHandler ( this.OnTextBoxLostFocus );
			EditBox_.Resize += new System.EventHandler ( this.OnTextBoxResize );
			EditBox_.Location = new System.Drawing.Point(0, 0);
			Spinner_ = new SpinnerControl();
			this.Controls.Add(EditBox_);
			this.Controls.Add(Spinner_);
			this.TabStop = false;
			InterceptArrowKeys = true;
		}

		/// --- Destructor ---

		/// --- Public Properties ---
		#region Public Properties
		// Gets or sets the background color for the control
		public override Color BackColor {
			get {	return EditBox_.BackColor; }
			set {   EditBox_.BackColor = value;}
		}

		// Gets or sets the background image displayed in the control
		[EditorBrowsable (EditorBrowsableState.Never)]	 
		public override Image BackgroundImage {
			get {	return base.BackgroundImage; }
			set {	base.BackgroundImage = value;}
		}

		// Gets or sets the border style for the up-down control
		public BorderStyle BorderStyle {
			get {
				throw new NotImplementedException ();
			}
			set {
				//FIXME:
			}
		}

		// Gets or sets the shortcut menu associated with the control
		public override ContextMenu ContextMenu {
			get {
				//FIXME:
				return base.ContextMenu;
			}
			set {
				//FIXME:
				base.ContextMenu = value;
			}
		}

		// Gets a value indicating whether the control has input focus
		public override bool Focused {
			get {
				//FIXME:
				return base.Focused;
			}
		}
		
		// Gets or sets the foreground color of the control
		public override Color ForeColor {
			get {
				//FIXME:
				return base.ForeColor;
			}
			set {
				//FIXME:
				base.ForeColor = value;
			}
		}

		// Gets or sets a value indicating whether the user can use the 
		// UP ARROW and DOWN ARROW keys to select values
		public bool InterceptArrowKeys {
			get { return interceptArrowKeys;  }
			set { interceptArrowKeys = value; }
		}

		// Gets the height of the up-down control
		public int PreferredHeight {
			get {
				throw new NotImplementedException ();
			}
		}

		// Gets or sets a value indicating whether the text may be 
		// changed by the use of the up or down buttons only
		public bool ReadOnly {
			get { return EditBox_.ReadOnly; }
			set { EditBox_.ReadOnly = value;}
		}

		// Gets or sets the site of the control.
		public override ISite Site {
			get {
				//FIXME:
				return base.Site;
			}
			set {
				//FIXME:
				base.Site = value;
			}
		}

		// Gets or sets the text displayed in the up-down control
		public override string Text {
			get {
				return EditBox_.Text;
			}
			set {
				EditBox_.Text = value;
			}
		}

		// Gets or sets the alignment of the text in the up-down control
		public HorizontalAlignment TextAlign {
			get {	return EditBox_.TextAlign; }
			set {	
				EditBox_.TextAlign = value;
			}
		}

		// Gets or sets the alignment of the up and down buttons on the 
		// up-down control
		public LeftRightAlignment UpDownAlign {
			get {	return Spinner_.UpDownAlign; }
			set {
				if ( !Enum.IsDefined ( typeof(LeftRightAlignment), value ) )
					throw new InvalidEnumArgumentException( "UpDownAlign",
						(int)value,
						typeof(LeftRightAlignment));

				if ( Spinner_.UpDownAlign != value ) {
					Spinner_.UpDownAlign = value;
					
					if ( IsHandleCreated && Spinner_.IsHandleCreated ) {
						RECT spinRect = new RECT ( );
						Win32.GetWindowRect ( Spinner_.Handle, ref spinRect );

						int SpinWidth = spinRect.right - spinRect.left;

						if ( Spinner_.UpDownAlign == LeftRightAlignment.Left ) {
							EditBox_.Left = SpinWidth - SpinnerControl.DEF_BUDDYBORDER;
							EditBox_.Width = Width - SpinWidth;
						}
						else {
							EditBox_.Left = 0;
							EditBox_.Width = Width - SpinWidth;
						}
					}
				}
			}
		}
		#endregion // Public Properties


		/// --- Public Methods ---
		#region Public Methods

		// When overridden in a derived class, handles the pressing of the down 
		// button on the up-down control. 
		public abstract void DownButton();
	
		// Selects a range of text in the up-down control specifying the 
		// starting position and number of characters to select.
		public void Select(int start,int length) 
		{
			EditBox_.Select ( start, length );
		}
		
		// When overridden in a derived class, handles the pressing of 
		// the up button on the up-down control
		public abstract void UpButton();

		#endregion // Public Methods


		/// --- Protected Properties ---
		#region Protected Properties

		// Gets or sets a value indicating whether the text property is being 
		// changed internally by its parent class
		protected bool ChangingText {
			get {
				throw new NotImplementedException ();
			}
			set {
				//FIXME:
			}
		}

		// Gets the required creation parameters when the control handle is created
		protected override CreateParams CreateParams {
			get {
				return base.CreateParams;
			}
		}

		// Gets the default size of the control.
		protected override Size DefaultSize {
			get {
				return new System.Drawing.Size(100,20);
			}
		}

		// Gets or sets a value indicating whether a value has been entered by the user
		protected bool UserEdit {
			get {
				return UserEdit_;
			}
			set {
				UserEdit_ = true;
			}
		}
		#endregion // Protected Properties

	
		/// --- Protected Methods ---
		#region Protected Methods
        [MonoTODO]
		protected virtual void OnChanged(object source, EventArgs e) {
		}

		// Raises the FontChanged event
		protected override void OnFontChanged(EventArgs e) 
		{
			//FIXME:
			base.OnFontChanged(e);
		}

		// Raises the HandleCreated event
		protected override void OnHandleCreated(EventArgs e) 
		{
			//FIXME:
			base.OnHandleCreated(e);
			EditBox_.Text = this.Text;
		}

		// Raises the Layout event
		protected override void OnLayout(LayoutEventArgs e) 
		{
			base.OnLayout( e );
		}
	
		// Raises the MouseWheel event
		protected override void OnMouseWheel(MouseEventArgs e) 
		{
			//FIXME:
		}

		// Raises the KeyDown event
		protected virtual void OnTextBoxKeyDown(object source, KeyEventArgs e) 
		{
			if ( InterceptArrowKeys ) {
				switch ( e.KeyData ) {
				case Keys.Up:
					UpButton ( );
					e.Handled = true;
				break;
				case Keys.Down:
					DownButton ( );
					e.Handled = true;
				break;
				}
			}
			OnKeyDown ( e );
		}

		// Raises the KeyPress event
		protected virtual void OnTextBoxKeyPress(object source, KeyPressEventArgs e) 
		{
			OnKeyPress ( e );
		}

		// Raises the LostFocus event
		protected virtual void OnTextBoxLostFocus(object source, EventArgs e) 
		{
			OnLostFocus ( e );
		}

		// Raises the Resize event
		protected virtual void OnTextBoxResize(object source, EventArgs e) 
		{
			OnResize ( e );
		}
		
		// Raises the TextChanged event.
		protected virtual void OnTextBoxTextChanged(object source, EventArgs e) 
		{
			OnTextChanged ( e );
		}
		
		// This member overrides Control.SetBoundsCore.
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)  {
			//FIXME: use PreferredHeight ?
			base.SetBoundsCore(x,y,width,height,specified);
			EditBox_.Size = new System.Drawing.Size(width, height);
		}

		// When overridden in a derived class, updates the text displayed in the
		// up-down control
		protected abstract void UpdateEditText();

		// When overridden in a derived class, validates the text displayed in the
		// up-down control
		protected virtual void ValidateEditText() 
		{
			//FIXME:
		}

		[MonoTODO]

			//FIXME shoould this be (ref message m)??
		protected override void WndProc(ref Message m) { // .NET V1.1 Beta
			//FIXME:
			base.WndProc(ref m);
		}
		
		[MonoTODO]
		protected override void Dispose(bool disposing) { // .NET V1.1 Beta
			base.Dispose(disposing);
		}

		private void EditBox_TextChanged ( object sender, EventArgs e )
		{
			OnTextBoxTextChanged ( EditBox_ , EventArgs.Empty );
		}

		#endregion // Protected Methods

	}
}