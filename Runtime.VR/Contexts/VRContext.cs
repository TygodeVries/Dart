using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using Runtime.Graphics;
using Runtime.VR.Native;
using Silk.NET.Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.VR.Contexts
{
    internal class VRContext : INativeContext
    {
        public nint GetProcAddress(string proc, int? slot = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return NativeMethods.WglGetProcAddress(proc);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                IntPtr ptr = NativeMethods.GlxGetProcAddress(proc);
                if (ptr == IntPtr.Zero)
                    ptr = NativeMethods.GlxGetProcAddressARB(proc);
                return ptr;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return NativeMethods.NSGLGetProcAddress(proc);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform");
            }
        }

        public bool IsExtensionPresent(string extension)
        {
            return GL.GetString(StringName.Extensions).Contains(extension);
        }

        public bool TryGetProcAddress(string proc, out nint addr, int? slot = null)
        {
            IGLFWGraphicsContext context = RenderCanvas.main.Context;
            IGLContext glContext = (IGLContext)context;

            addr = glContext.GetProcAddress(proc, slot);
            return addr != IntPtr.Zero;
        }

        public string? GetPlatform()
        {
            return "OpenTK";
        }

        public void Dispose()
        {
            
        }
    }
}
