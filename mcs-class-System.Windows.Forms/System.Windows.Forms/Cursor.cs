//
// System.Windows.Forms.Cursor.cs
//
// Author:
//   stubbed out by Jaak Simm (jaaksimm@firm.ee)
//   Dennis Hayes (dennish@Raytek.com)
//   Aleksey Ryabchuk (ryabchuk@yahoo.com)
//
// (C) Ximian, Inc., 2002
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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO;
using System.Drawing;

namespace System.Windows.Forms {

	/// <summary>
	/// Represents the image used to paint the mouse pointer.
	/// </summary>

	[MonoTODO]
	[Serializable]
	public sealed class Cursor : IDisposable, ISerializable {

		#region Fields
		private IntPtr handle;
		private bool   fromResource    = false;
		private bool   disposed        = false;
		private CursorType ctype;
		#endregion
		
		#region Constructors
		private Cursor(){
		}
		internal Cursor ( CursorType type )//For signiture compatablity.
		{
			handle = Win32.LoadCursor ( IntPtr.Zero, type );
			fromResource = true;
			ctype  = type;
		}

		[MonoTODO]
		public Cursor( IntPtr handle ) 
		{
			this.handle = handle;	
		}
		
		[MonoTODO]
		public Cursor(Stream stream) 
		{
			
		}
		
		[MonoTODO]
		public Cursor(string fileName) 
		{
			
		}
		
		[MonoTODO]
		public Cursor(Type type,string resource) 
		{
			
		}
		#endregion
		
		#region Properties
		[MonoTODO]
		public static Rectangle Clip {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		
		[MonoTODO]
		public static Cursor Current {
			get { 
				throw new NotImplementedException (); 
			}
			set { 
				throw new NotImplementedException (); 
			}
		}
		
		[MonoTODO]
		public IntPtr Handle {
			get { return handle; }
		}
		
		[MonoTODO]
		public static Point Position {
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}
		
		[MonoTODO]
		public Size Size {
			get { throw new NotImplementedException (); }
		}
		#endregion
		
		#region Methods

		public IntPtr CopyHandle() 
		{
			return Win32_WineLess.CopyCursor ( Handle );
		}
		
		public void Dispose() 
		{
			GC.SuppressFinalize( this );
			Dispose( true );
		}
		
		private void Dispose( bool disposing )
		{
			lock ( this ) {
				if ( disposing ) {
					// dispose managed resources
				}
				if ( !this.disposed ) {
					// release all unmanaged resources
					// shared cursor should not be destroyed
					if ( !fromResource )
						Win32.DestroyCursor ( handle );
					handle = IntPtr.Zero;
				}
				disposed = true;         
			}
		}

		[MonoTODO]
		public void Draw(Graphics g,Rectangle targetRect) 
		{
			throw new NotImplementedException ();
		}
		
		[MonoTODO]
		public void DrawStretched(Graphics g,Rectangle targetRect) 
		{
			throw new NotImplementedException ();
		}
		
		[MonoTODO]
		public override bool Equals(object obj) 
		{
			if ( obj == null ) return false;
			if ( this.GetType ( ) != obj.GetType ( ) ) return false;
			Cursor other = ( Cursor ) obj;
			if ( !fromResource.Equals ( other.fromResource ) ) return false;
			if ( !ctype.Equals ( other.ctype ) ) return false;

			return true;
		}
		
		~Cursor() {
			Dispose ( false );
		}
		
		[MonoTODO]
		public override int GetHashCode() 
		{
			//FIXME:
			return base.GetHashCode();
		}
		
		public static void Hide() 
		{
			Win32.ShowCursor ( false );
		}
		
		/// ISerializable.GetObjectData only supports .NET framework:
		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo si,StreamingContext context) 
		{
			throw new NotImplementedException ();
		}
		
		public static void Show() 
		{
			Win32.ShowCursor ( true );
		}
		
		[MonoTODO]
		public override string ToString() 
		{
			//FIXME:
			return base.ToString();
		}
		#endregion
		
		#region Operators
		[MonoTODO]
		public static bool operator ==(Cursor left, Cursor right) 
		{
			return Object.Equals ( left, right );
		}
		
		[MonoTODO]
		public static bool operator !=(Cursor left, Cursor right) 
		{
			return ! ( left == right );
		}
		#endregion
	}
}