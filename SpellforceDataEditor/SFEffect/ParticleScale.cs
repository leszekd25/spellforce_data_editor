using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticleScale: ParticleMovieEffect
    {
        public OpenTK.Vector3 Min = OpenTK.Vector3.One;
        public OpenTK.Vector3 Max = OpenTK.Vector3.One;

        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleScaleControl();
        }

        public override object GetInterpolated(float t)
        {
            return Min + (Max - Min) * GetStrength(t);
        }
    }
}
