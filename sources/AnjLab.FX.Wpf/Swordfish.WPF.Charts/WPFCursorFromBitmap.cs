// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\WPFCursorFromBitmap.cs                     **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Runtime.InteropServices;
using SysSystem = System;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	/// <summary>
	/// This class converts a win32 bitmap to a WPF Cursor
	/// </summary>
	public class WPFCursorFromBitmap : SafeHandle
	{
		// ********************************************************************
		// Methods
		// ********************************************************************
		#region Methods

		/// <summary>
		/// Creates a WPF cursor from a win32 bitmap
		/// </summary>
		/// <param name="cursorBitmap"></param>
		/// <returns></returns>
        public static SysSystem.Windows.Input.Cursor CreateCursor(SysSystem.Drawing.Bitmap cursorBitmap)
		{
			WPFCursorFromBitmap csh = new WPFCursorFromBitmap(cursorBitmap);
            return SysSystem.Windows.Interop.CursorInteropHelper.Create(csh);
		}

		/// <summary>
		/// Hidden contructor. Accessed only from the static method.
		/// </summary>
		/// <param name="cursorBitmap"></param>
        protected WPFCursorFromBitmap(SysSystem.Drawing.Bitmap cursorBitmap)
			: base((IntPtr)(-1), true)
		{
			handle = cursorBitmap.GetHicon();
		}

		/// <summary>
		/// Releases the bitmap handle
		/// </summary>
		/// <returns></returns>
		protected override bool ReleaseHandle()
		{
			bool result = DestroyIcon(handle);
			handle = (IntPtr)(-1);
			return result;
		}

		/// <summary>
		/// Imported from user32.dll. Destroys an icon GDI object.
		/// </summary>
		/// <param name="hIcon"></param>
		/// <returns></returns>
        [DllImport("user32")]
		private static extern bool DestroyIcon(IntPtr hIcon);

		#endregion Methods

		// ********************************************************************
		// Properties
		// ********************************************************************
		#region Properties

		/// <summary>
		/// Gets if the handle is valid or not
		/// </summary>
		public override bool IsInvalid
		{
			get
			{
				return handle == (IntPtr)(-1);
			}
		}

		#endregion Properties
	}
}
