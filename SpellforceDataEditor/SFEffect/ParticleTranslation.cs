using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticleTranslation: ParticleMovieEffect
    {
        public OpenTK.Vector3 Min = OpenTK.Vector3.Zero;
        public OpenTK.Vector3 Max = OpenTK.Vector3.Zero;

        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleTranslationControl();
        }

        public override object GetInterpolated(float t)
        {
            return Min + (Max - Min) * GetStrength(t);
        }
    }
}
