using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticleSound: ParticleMovieEffect
    {
        public string Name = "";
        public float Length = -1;

        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleSoundControl();
        }
    }
}
