using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public class ParticlePath: ParticleMovieEffect
    {
        public SF3D.InterpolatedDouble Values = new SF3D.InterpolatedDouble();    // t from 0 to 1 (probably)
    }
}
