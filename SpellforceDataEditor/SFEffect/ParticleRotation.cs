using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public enum RotationMode { X, Y, Z }
    public class ParticleRotation: ParticleMovieEffect
    {
        public RotationMode Axis = RotationMode.Z;
        public float Angle = -1;
        public float Min = 0;
        public float Max = 360;

        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleRotationControl();
        }

        public override object GetInterpolated(float t)
        {
            if (Axis == RotationMode.X)
                return OpenTK.Quaternion.FromAxisAngle(new OpenTK.Vector3(1, 0, 0), Min + (Max - Min) * GetStrength(t));
            else if (Axis == RotationMode.Y)
                return OpenTK.Quaternion.FromAxisAngle(new OpenTK.Vector3(0, 1, 0), Min + (Max - Min) * GetStrength(t));
            else if (Axis == RotationMode.Z)
                return OpenTK.Quaternion.FromAxisAngle(new OpenTK.Vector3(0, 0, 1), Min + (Max - Min) * GetStrength(t));

            return OpenTK.Quaternion.Identity;
        }
    }
}
