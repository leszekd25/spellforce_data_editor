﻿/*
 * SFResource is an interface for a resource type object
 * Resource must be possible to load, initialize, and to dispose of
 * Resources are meant to be unique across the application
 * * */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFResources
{
    public interface SFResource
    {
        int Load(MemoryStream ms);
        void Init();
        void Dispose();
    }
}
