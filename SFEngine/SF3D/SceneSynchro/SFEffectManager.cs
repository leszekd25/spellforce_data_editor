using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SF3D.SceneSynchro
{
    public class SFEffect      //DrwParticleMovie
    {
        public float CurrentTime = 0.0f;
        public float MinTime = 0.0f;
        public float MaxTime = 1.0f;
        public int ParticleNum = 1;


        public List<SceneNode> Particles = new List<SceneNode>();

        public void Process(float dt)
        {
            CurrentTime += dt;
            if(CurrentTime > MaxTime)
            {
                float t = CurrentTime - (MaxTime - MinTime);
                CurrentTime = (float)(t - Math.Floor(t));
            }
        }

        public void Init(SceneNode owner)
        {
            for(int i = 0; i < ParticleNum; i++)
            {
                SceneNode n = new SceneNode("p_" + i.ToString());
                Particles.Add(n);
                n.SetParent(owner);
            }
            CurrentTime = MinTime;
        }

        public void Clear()
        {
            for(int i = 0; i < Particles.Count; i++)
            {
                if (Particles[i].Parent != null)
                {
                    Particles[i].SetParent(null);
                    Particles[i].Dispose();
                }
            }
            Particles.Clear();
            CurrentTime = MinTime;
        }
    }

    public class SFEffectManager
    {
        public SFScene scene = null;
        public List<SFEffect> effects = new List<SFEffect>();

        public SFEffect AddEffect()
        {
            SFEffect new_effect = new SFEffect();
            effects.Add(new_effect);
            return new_effect;
        }

        public void Process(float dt)
        {
            for(int i = 0; i < effects.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine("processing effect #" + (i + 1).ToString() + ", time = " + effects[i].CurrentTime);
                effects[i].Process(dt);
            }    
        }

        public void Clear()
        {
            for(int i = 0; i < effects.Count; i++)
            {
                effects[i].Clear();
            }
            effects.Clear();
        }
    }
}
