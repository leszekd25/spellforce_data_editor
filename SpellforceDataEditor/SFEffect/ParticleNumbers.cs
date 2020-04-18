using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticleNumbers: ParticleMovieEffect
    {
        public override SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleNumbersControl();
        }
    }
}
