//
// System.Windows.Forms.IDataGridEditingService.cs
//
// Author:
// William Lamb (wdlamb@notwires.com)
// Dennis Hayes (dennish@raytek.com)
//
// (C) 2002 Ximian, Inc. http://www.ximian.com
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

namespace System.Windows.Forms {

	public interface IDataGridEditingService {

		// There is no documentation for this interface's members!
		// Only a note saying that it supports the .NET infrastructure
		// and is not intended to be used directly from your code.
		// The following methods had their own listing in the documentation;
		// I don't know what other methods and properties there may be.

		bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber);
		bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort);
	}
}