using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapWeatherManager
    {
        public byte[] weather { get; private set; } = null;

        public SFMapWeatherManager()
        {
            weather = new byte[8];
            weather[0] = 100;
        }

        public void Normalize()   // make sure the sum of all terms == 100
        {
            int sum = 0;
            for (int i = 0; i < weather.Length; i++)
                sum += weather[i];
            if(sum == 0)
            {
                weather[0] = 100;
                return;
            }

            float dif = sum / 100f;
            byte[] new_weather = new byte[weather.Length];

            int cur_w = 0;
            float cur_sum = 0;
            for(int i = 0; i < 100; i++)
            {
                while (cur_sum >= weather[cur_w])
                {
                    cur_sum -= weather[cur_w];
                    cur_w += 1;
                    if (cur_w == weather.Length)       // should never happen?
                        throw new Exception("Invalid weather calculation...");
                }

                new_weather[cur_w] += 1;
                cur_sum += dif;
            }

            weather = new_weather;
        }
    }
}
