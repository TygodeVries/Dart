using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.VR.Native
{
    internal class NativeMethods
    {
        [DllImport("opengl32.dll", EntryPoint = "wglGetProcAddress")]
        public static extern IntPtr WglGetProcAddress(string procName);

        [DllImport("libGL.so.1")]
        public static extern IntPtr GlxGetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procName);

        [DllImport("libGL.so.1")]
        public static extern IntPtr GlxGetProcAddressARB([MarshalAs(UnmanagedType.LPStr)] string procName);

        [DllImport("libGL.dylib")]
        public static extern IntPtr NSGLGetProcAddress(string procName);
    }
}
