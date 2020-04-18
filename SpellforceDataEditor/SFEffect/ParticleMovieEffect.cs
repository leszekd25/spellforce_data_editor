using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public enum MoviePlayMode { kDrwPlayOnce = 0, kDrwPlayLooped = 1, kDrwPlayBounce = 2, kDrwPlayClamped = 3, kDrwPlayStretched = 4, kDrwPlayContinuous = 5}
    public enum MoviePathMode { kDrwPathLinear = 0, kDrwPathCosine = 1, kDrwPathSine = 2, kDrwPathParabola = 3, kDrwPathNegParabola = 4, kDrwPathRandom = 5}

    public enum MovieDimMode { kDrwDimTime = 0, kDrwDimParticle = 1, kDrwDimPower = 2, kDrwDimTimeToEnd = 3, kDrwDimTimeScaled = 4, kDrwDimTimeAbsolute = 5, kDrwDimRandom = 6, kDrwDimTargetSize = 7, kDrwDimPlayer = 8 }
    public class ParticleMovieEffect
    {
        public float Start = 0;
        public float End = 1;

        public MoviePlayMode Play = MoviePlayMode.kDrwPlayLooped;
        public MovieDimMode Dim = MovieDimMode.kDrwDimTime;
        public MoviePathMode Path = MoviePathMode.kDrwPathLinear;
        public float Trail = 0;
        public bool UseCustomPath = false;
        public bool UseCustomTrail = false;
        public ParticlePath CustomPath = null;
        public ParticleTrail CustomTrail = null;

        private float GetX(float t) // t can be any value, X from 0 to 1
        {
            if (End == Start)
                return 0;

            t -= Start;
            // 1. get t to proper value based on play mode
            switch (Play)
            {
                case MoviePlayMode.kDrwPlayOnce:
                    if (t < Start)
                        t = Start;
                    if (t > End)
                        t = End;
                    break;
                case MoviePlayMode.kDrwPlayLooped:
                    t = (float)Math.IEEERemainder(t, End - Start);
                    break;
                case MoviePlayMode.kDrwPlayBounce:
                    t = (float)Math.IEEERemainder(t, End - Start);
                    if (((int)(t / (End - Start))) % 2 == 1)
                        t = End - t;
                    break;
                case MoviePlayMode.kDrwPlayClamped:
                    if (t > End)
                        t = End;
                    if (t < Start)
                        t = Start;
                    break;
                case MoviePlayMode.kDrwPlayStretched:    // this is done elsewhere
                case MoviePlayMode.kDrwPlayContinuous:
                    throw new Exception("GetX(): Unsupported mode");
            }

            return (t - Start) / (End - Start);
        }

        private float GetCurveY(float X)   // X from 0 to 1, Y from 0 to 1
        {
            switch (Path)
            {
                case MoviePathMode.kDrwPathLinear:
                    return X;
                case MoviePathMode.kDrwPathCosine:
                    return (float)(Math.Cos(X * Math.PI * 2) / 2 + 0.5);
                case MoviePathMode.kDrwPathSine:
                    return (float)(Math.Sin(X * Math.PI * 2) / 2 + 0.5);
                case MoviePathMode.kDrwPathParabola:
                    return X * X;
                case MoviePathMode.kDrwPathNegParabola:
                    return 1 - (X * X);
                case MoviePathMode.kDrwPathRandom:
                    return MathUtils.Randf();
            }

            return 0;
        }

        public float GetStrength(float t)  // t can be any, strength from 0 to 1
        {
            if (CustomPath != null)
                return (float)CustomPath.Values.Get(GetX(t));

            return GetCurveY(GetX(t));
        }

        public virtual object GetInterpolated(float t)
        {
            return GetStrength(t);
        }

        public virtual SFEffect.effect_controls.ParticleMovieEffectControl NewControl()
        {
            return new effect_controls.ParticleMovieEffectControl();
        }
    }
}
