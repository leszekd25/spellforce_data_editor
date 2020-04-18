using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticleTrail: ParticleMovieEffect
    {
        public float Min = 0;
        public float Max = 0;

        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleTrailControl();
        }
    }
}
