using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Components.Test;
using Runtime.Graphics;
using Runtime.Objects;
using Runtime.Scenes;
using static System.MathF;

namespace FeatureTestProject
{
    [Runtime.Plugins.DartEntryPoint("Main")]
    public class EntryPoint
    {

        static EntryPoint()
        {
        }
        public static void Main()
        {
            Runtime.Scenes.Scene.main.Instantiate(
                new GameObjectFactory()
                    .AddComponent<ParticleTest>()
                    .Build()
            );
            Camera cam = new Camera();
            //  GuiWindow.Enable(new GUIPerformanceWindow());
            Runtime.Scenes.Scene.main.Instantiate(
                new GameObjectFactory().AddComponent<Transform>().AddComponent(cam).AddComponent<FlightCamera>().Build());
            cam.SetAsMain();

        }
    }
    class ParticleTest : Component
    {
        class MyParticleType : ParticleType
        {
            public override Vector4 GetColor(float age)
            {
                float a = PI * age;
                return new Vector4(Cos(a), -Cos(a), Sin(a), Sin(a) / (1 + a));
            }

            public override float GetLifetime()
            {
                return 4;
            }

            public override float GetSize(float age)
            {
                float a = PI * age;
                return (1 - age) * Sin(a) * .3f;
            }
            public override float GetFriction()
            {
                return 0.01f;
            }
        };
        MyParticleType? pt;
        public override void Load()
        {
            pt = new MyParticleType();
            Scene.main.GetManager<ParticleSystemManager>()?.UpdateParticleType(pt);
        }

        public override void Update()
        {
        }
    }
}

