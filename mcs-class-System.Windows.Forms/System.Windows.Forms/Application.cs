//
// System.Windows.Forms.Application.cs
//
// Author:
//   stubbed out by Jaak Simm (jaaksimm@firm.ee)
//   Miguel de Icaza (miguel@ximian.com)
//	Dennis hayes (dennish@raytek.com)
//   WINELib implementation started by John Sohn (jsohn@columbus.rr.com)
//
// (C) Ximian, Inc 2002/3
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
using Microsoft.Win32;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Runtime.CompilerServices;
namespace System.Windows.Forms 
{

	/// <summary>
	/// Provides static methods and properties to manage an application,
	/// such as methods to start and stop an application, to process
	/// Windows messages, and properties to get information about an
	/// application. This class cannot be inherited.
	/// </summary>

	[MonoTODO]
	public sealed class Application 
	{
		static private ApplicationContext applicationContext = null;
		static private bool messageLoopStarted = false;
		static private bool messageLoopStopRequest = false;
		static private  ArrayList messageFilters = new ArrayList ();
		static private string safeTopLevelCaptionFormat;
		static private bool showingException = false;


		private Application(){//For signiture compatablity. Prevents the auto creation of public constructor
		}

		// --- (public) Properties ---
		public static bool AllowQuit 
		{
			// according to docs return false if embbedded in a
			// browser, not (yet?) embedded in a browser
			get { return true; }
		}

		[MonoTODO]
		public static string CommonAppDataPath 
		{
			get 
			{
				//FIXME:
				return "";
			}
		}

		[MonoTODO]
		public static RegistryKey CommonAppDataRegistry 
		{
			get 
			{
				throw new NotImplementedException ();
			}
		}
	
		[MonoTODO]
		public static string CompanyName 
		{
			get 
			{
				AssemblyCompanyAttribute[] attrs =(AssemblyCompanyAttribute[]) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute),true);
				if (attrs != null && attrs[0] != null)
					return attrs[0].Company;
				return "";
			}
		}
	
		[MonoTODO]
		public static CultureInfo CurrentCulture 
		{
			get 
			{
				return CultureInfo.CurrentCulture;
			}
			set 
			{
				Thread.CurrentThread.CurrentCulture = value;
			}
		}
	
		[MonoTODO]
		public static InputLanguage CurrentInputLanguage 
		{
			get { throw new NotImplementedException (); }
			set {return;}
		}
	
		[MonoTODO]
		public static string ExecutablePath 
		{
			get 
			{
				return Assembly.GetExecutingAssembly().Location;			
			}
		}
	
		[MonoTODO]
		public static string LocalUserAppDataPath 
		{
			get 
			{
				//FIXME:
				return "";
			}
		}
	
		public static bool MessageLoop 
		{
			get 
			{
				return messageLoopStarted;
			}
		}

		[MonoTODO]
			//.NET version 1.1
		public static void EnableVisualStyles () 
		{
			return;
		}
			
		[MonoTODO]
		public static string ProductName 
		{
			get 
			{
				AssemblyProductAttribute[] attrs =(AssemblyProductAttribute[]) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute),true);
				if (attrs != null && attrs[0] != null)
					return attrs[0].Product;
				return "";
			}
		}
	
		[MonoTODO]
		public static string ProductVersion 
		{
			get 
			{
				AssemblyVersionAttribute[] attrs =(AssemblyVersionAttribute[]) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyVersionAttribute),true);
				if (attrs != null && attrs[0] != null)
					return attrs[0].Version;
				return "";
			}
		}
	
		[MonoTODO]
		public static string SafeTopLevelCaptionFormat 
		{
			get 
			{
				return safeTopLevelCaptionFormat;
			}
			set 
			{
				safeTopLevelCaptionFormat = value;
			}
		}
	
		[MonoTODO]
		public static string StartupPath 
		{
			get 
			{
				//FIXME:
				return "";
			}
		}
	
		[MonoTODO]
		public static string UserAppDataPath 
		{
			get 
			{
				//FIXME:
				return "";
			}
		}
	
		[MonoTODO]
			// Registry key not yet defined
		public static RegistryKey UserAppDataRegistry 
		{
			get { throw new NotImplementedException (); }
		}
	
		// --- Methods ---
		public static void AddMessageFilter (IMessageFilter value) 
		{
			messageFilters.Add (value);
		}

		//Compact Framework	
		public static void DoEvents () 
		{
			MSG msg = new MSG();

			while (Win32.PeekMessageA (ref msg, (IntPtr) 0,  0, 0, (uint)PeekMessageFlags.PM_REMOVE) != 0) {
				if (msg.message==Msg.WM_PAINT) {
					Win32.TranslateMessage(ref msg);
					Win32.DispatchMessageA(ref msg);
				}
			}
		}

		//Compact Framework	
		public static void Exit () 
		{
			Win32.PostQuitMessage (0);
		}
	
		public static void ExitThread () 
		{
			messageLoopStopRequest = true;
		}
	
		[MonoTODO]
		public static ApartmentState OleRequired () 
		{
			throw new NotImplementedException ();
		}
	
		[MonoTODO]
		public static void OnThreadException (Exception t) 
		{			
			
			if(Application.ThreadException != null) 
				Application.ThreadException(null, new ThreadExceptionEventArgs(t));
			else{							
				
				if (!showingException)	{
					
					showingException = true;
				
					Form	excepForm = new Form();
					excepForm.ClientSize = new System.Drawing.Size(400, 250);				
					
					TextBox txtLabel = new TextBox();		
					txtLabel.Location = new System.Drawing.Point(30, 30);					
					txtLabel.ReadOnly = true;					
					txtLabel.Multiline = true;
					txtLabel.Size = new System.Drawing.Size(310, 50);		 
					txtLabel.Text = "The application has produced an exception. Press 'Continue' if you want the application to try to continue its execution";										
					excepForm.Controls.Add(txtLabel); 					
					
					TextBox txtError = new TextBox();		
					txtError.Location = new System.Drawing.Point(30, 110);					
					txtError.ReadOnly = true;					
					txtLabel.Multiline = true;
					txtError.Size = new System.Drawing.Size(310, 50);		
					txtError.Text = t.Message;										
					excepForm.Controls.Add(txtError);
					
					StackButton stackbtn = new StackButton(t);		
					stackbtn.Location = new System.Drawing.Point(30, 200);					
					stackbtn.Size = new System.Drawing.Size(100, 30);		
					stackbtn.Text = "Stack Trace";										
					excepForm.Controls.Add(stackbtn); 
					
					ContinueButton continuebtn = new ContinueButton(excepForm);		
					continuebtn.Location = new System.Drawing.Point(160, 200);					
					continuebtn.Size = new System.Drawing.Size(100, 30);		
					continuebtn.Text = "Continue";										
					excepForm.Controls.Add(continuebtn);    		    	    												
					
					QuitButton quitbtn = new QuitButton();		
					quitbtn.Location = new System.Drawing.Point(290, 200);					
					quitbtn.Size = new System.Drawing.Size(100, 30);		
					quitbtn.Text = "Quit";										
					excepForm.Controls.Add(quitbtn);    		    	    												
					
					excepForm.ShowDialog();							
					showingException = false;
				}							
				
			}
			
		}
		
		
		public static void RemoveMessageFilter (IMessageFilter value)
		{
			messageFilters.Remove (value);
		}

		static private void ApplicationFormClosed (object o, EventArgs args)
		{
			Win32.PostQuitMessage (0);
		}

		//Compact Framework
		static public void Run ()
		{
			MSG msg = new MSG();

			messageLoopStarted = true;


			while (!messageLoopStopRequest && 
				Win32.GetMessageA (ref msg, 0, 0, 0) != 0) 
			{

				bool dispatchMessage = true;

				Message message = new Message ();
				message.HWnd = msg.hwnd;
				message.Msg = (int) msg.message;//
				message.WParam = msg.wParam;
				message.LParam = msg.lParam;

				IEnumerator e = messageFilters.GetEnumerator();

				while (e.MoveNext()) 
				{
					IMessageFilter filter = 
						(IMessageFilter) e.Current;

					// if PreFilterMessage returns true
					// the message should not be dispatched
					if (filter.PreFilterMessage (ref message))
						dispatchMessage = false;
				}

				Control receiver = Control.FromChildHandle ( message.HWnd );
				if ( receiver != null ) 
				{
					dispatchMessage = ! receiver.PreProcessMessage ( ref message );
				}

				if (dispatchMessage) 
				{
					Win32.TranslateMessage (ref msg);
					Win32.DispatchMessageA (ref msg);
				}
				//if (Idle != null)
				//Idle (null, new EventArgs());
			}

			//if (ApplicationExit != null)
			//ApplicationExit (null, new EventArgs());
		}

		public static void Run (ApplicationContext context) 
		{
			applicationContext = context;
			applicationContext.MainForm.Show ();
			applicationContext.ThreadExit += new EventHandler( ApplicationFormClosed );
			//			applicationContext.MainForm.Closed += //
			//			    new EventHandler (ApplicationFormClosed);
			Run();
		}

		//[TypeAttributes.BeforeFieldInit]
		public static void Run (Form mainForm)
			// Documents say this parameter name should be mainform, 
			// but the verifier says context.
		{
			mainForm.CreateControl ();
			ApplicationContext context = new ApplicationContext (
				mainForm);
			Run (context);
		}
		
		internal static void enterModalLoop ( Form mainForm )
		{
			mainForm.ExitModalLoop = false;

			MSG msg = new MSG();
			while( Win32.GetMessageA( ref msg, 0, 0, 0 ) != 0 ) 
			{

				if ( mainForm.ExitModalLoop )
					break;

				Message message = new Message ();
				message.HWnd = msg.hwnd;
				message.Msg = (int) msg.message;
				message.WParam = msg.wParam;
				message.LParam = msg.lParam;

				Control receiver = Control.FromChildHandle ( message.HWnd );
				if ( receiver != null )
					if ( receiver.PreProcessMessage ( ref message ) )
						continue;

				Win32.TranslateMessage (ref msg);
				Win32.DispatchMessageA (ref msg);
			}
			
		}
		
		internal static void exitModalLoop ( Form mainForm )
		{
			mainForm.ExitModalLoop = true;

			Win32.PostMessage( IntPtr.Zero, Msg.WM_NULL, 0, 0 ); 
		}

		// --- Events ---
		public static event EventHandler ApplicationExit;
		public static event EventHandler Idle;
		public static event ThreadExceptionEventHandler ThreadException;
		public static event EventHandler ThreadExit;
		
		
		// StackButton
		internal class StackButton : System.Windows.Forms.Button{
				
				private Exception excep = null;			
				
				public StackButton(Exception t) : base(){
					excep = t;					
				}				
				
				protected override void OnClick(EventArgs e) 	{	
					MessageBox.Show(excep.StackTrace, "Stack Trace");
				}
		}
		
		// QuitButton
		internal class QuitButton : System.Windows.Forms.Button{								
				
				public QuitButton() : base(){}				
				
				protected override void OnClick(EventArgs e) 	{	
					Application.ExitThread();
					Application.Exit();
				}
		}
		
		// ContinueButton
		internal class ContinueButton : System.Windows.Forms.Button{								
		
				private Form form = null;
				
				public ContinueButton(Form frm) : base(){form=frm;}				
				
				protected override void OnClick(EventArgs e) 	{	
					form.Close();
				}
		}


	}
}
