namespace TUVienna.CS_CUP.Runtime
{

	/**
	 * Defines the Scanner interface, which CUP uses in the default
	 * implementation of <code>lr_parser.scan()</code>.  Integration
	 * of scanners implementing <code>Scanner</code> is facilitated.
	 *
	 * @version last updated 23-Jul-1999
	 * @author David MacMahon <davidm@smartsc.com>
     * translated to C# 08.09.2003 by Samuel Imriska
	 */

	/* *************************************************
	  Interface Scanner
  
	  Declares the next_token() method that should be
	  implemented by scanners.  This method is typically
	  called by lr_parser.scan().  End-of-file can be
	  indicated either by returning
	  <code>new Symbol(lr_parser.EOF_sym())</code> or
	  <code>null</code>.
	 ***************************************************/
	public interface Scanner 
	{
		/** Return the next token, or <code>null</code> on end-of-file. */
		 Symbol next_token();
	}
}