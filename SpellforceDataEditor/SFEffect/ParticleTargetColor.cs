using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticleTargetColor: ParticleMovieEffect
    {
        public OpenTK.Vector4 Min = OpenTK.Vector4.One;
        public OpenTK.Vector4 Max = OpenTK.Vector4.One;

        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleTargetColorControl();
        }

        public override object GetInterpolated(float t)
        {
            return Min + (Max - Min) * GetStrength(t);
        }
    }
}
