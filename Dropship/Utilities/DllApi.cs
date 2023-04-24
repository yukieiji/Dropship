using System;
using System.Runtime.InteropServices;

namespace Dropship.Utilities;

public static class DllApi
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(
        IntPtr hWnd, string text, string caption, int options);
}
