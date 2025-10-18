using Runtime.Graphics.Pipeline;
using Runtime.Logging;
using Runtime.VR.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
using Silk.NET.Core;
using System.Text;
namespace Runtime.VR
{
    public unsafe class VRRenderPass : RenderPass
    {

        //https://github.com/stride3d/stride/blob/master/sources/engine/Stride.VirtualReality/OpenXR/OpenXRUtils.cs
        XR xr;
        Instance instance;
        public override void Start()
        {

            // Setup our app
            base.Start();
            xr = new XR(new VRContext());

            unsafe
            {
                ApplicationInfo appInfo = default;
                appInfo.ApplicationVersion = 1;
                appInfo.EngineVersion = 1;
                appInfo.ApiVersion = new Version64(1, 0, 10);

                byte* appNamePtr = appInfo.ApplicationName;
                byte* engNamePtr = appInfo.EngineName;
                
                // Clear buffers
                for (int i = 0; i < 128; i++)
                {
                    appNamePtr[i] = 0;
                    engNamePtr[i] = 0;
                }

                // Copy strings as UTF8 into buffers (up to 127 bytes + null terminator)
                var appNameBytes = Encoding.UTF8.GetBytes(System.AppDomain.CurrentDomain.FriendlyName);
                var engNameBytes = Encoding.UTF8.GetBytes("Dart");

                for (int i = 0; i < appNameBytes.Length && i < 127; i++)
                    appNamePtr[i] = appNameBytes[i];
                appNamePtr[Math.Min(appNameBytes.Length, 127)] = 0; // null terminate

                for (int i = 0; i < engNameBytes.Length && i < 127; i++)
                    engNamePtr[i] = engNameBytes[i];
                engNamePtr[Math.Min(engNameBytes.Length, 127)] = 0; // null terminate

                InstanceCreateInfo instanceCreateInfo = new InstanceCreateInfo
                {
                    Type = StructureType.InstanceCreateInfo,
                    ApplicationInfo = appInfo
                };

                instance = default;
                Result result = xr.CreateInstance(instanceCreateInfo, ref instance);
                if (result != Result.Success)
                {
                    Debug.Error($"Failed to start VR! {result}");
                }
                
            }


            // Find the device

            ulong systemId = default;

            var getSystemInfo = new SystemGetInfo
            {
                Type = StructureType.TypeSystemGetInfo,
                FormFactor = FormFactor.HeadMountedDisplay
            };

            Result sysResult = xr.GetSystem(instance, ref getSystemInfo, ref systemId);
            if (sysResult != Result.Success)
            {
                Debug.Error($"Failed to get OpenXR system: {sysResult}");
                return;
            }

            Debug.Log($"OpenXR system acquired (ID: {systemId}).");

        }
        public override void Pass()
        {
            throw new NotImplementedException();
        }
    }
}
