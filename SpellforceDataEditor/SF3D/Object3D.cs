﻿/*
 * Object3D exists in 3D space
 * With such object, a transform matrix is associated, which defines its position, orientation and scale
 * Every object that exists in 3D space must inherit from Object3D
 * Every object may inherit from parent object, which modifies transform accordingly
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D
{
    public class Object3D
    {
        protected Vector3 position = new Vector3(0);
        protected Quaternion rotation = new Quaternion(0, 0, 0, 1);
        protected Vector3 scale = new Vector3(1);
        protected Matrix4 modelMatrix = new Matrix4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        protected Object3D parent = null;
        protected bool modified = false;
        protected bool visible = true;

        public Vector3 Position { get { return position; } set { position = value; modified = true; } }
        public Quaternion Rotation { get { return rotation; } set { rotation = value; modified = true; } }
        public Vector3 Scale { get { return scale; } set { scale = value; modified = true; } }
        public Matrix4 ModelMatrix { get { return modelMatrix; } }
        public Object3D Parent { get { return parent; } }
        public bool Modified { get { return modified; } }
        public bool Visible { get { return visible; } set { visible = value; } }

        public Object3D()
        {
            update_modelMatrix();
            parent = null;
            visible = true;
        }

        public void update_modelMatrix()
        {
            Matrix4 temp_matrix;
            Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
            Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
            Matrix4 scale_matrix = Matrix4.CreateScale(scale);
            temp_matrix = translation_matrix * rotation_matrix * scale_matrix;

            if (parent == null)
                modelMatrix = temp_matrix;
            else
                modelMatrix = temp_matrix * parent.modelMatrix;
            //children update matrices here

            modified = false;
        }
    }

    public class ObjectSimple3D: Object3D
    {
        protected SFModel3D mesh = null;

        public SFModel3D Mesh { get { return mesh; } set { mesh = value; } }
    }
}
