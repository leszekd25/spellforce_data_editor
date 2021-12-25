using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SFEngine.SF3D.SceneSynchro
{
    public class TexturedGeometryListElementSimple
    {
        public SceneNodeSimple node;
        public int submodel_index;
    }

    public class TexturedGeometryListElementAnimated
    {
        public SceneNodeAnimated node;
        public int submodel_index;
    }
}
