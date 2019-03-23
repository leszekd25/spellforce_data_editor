/*
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
        protected List<Object3D> children = new List<Object3D>();
        protected bool modified = false;
        protected bool visible = true;

        public Vector3 Position { get { return position; } set { position = value; modified = true; } }
        public Quaternion Rotation { get { return rotation; } set { rotation = value; modified = true; } }
        public Vector3 Scale { get { return scale; } set { scale = value; modified = true; } }
        public Matrix4 ModelMatrix { get { return modelMatrix; } }
        public Object3D Parent { get { return parent; } set
            {
                if (parent != null)
                    parent.Children.Remove(this);
                parent = value;
                if(parent != null)
                    parent.Children.Add(this);
            } }
        public List<Object3D> Children { get { return children; } }
        public virtual bool Modified { get { return modified||(parent != null && parent.Modified); } }
        public bool Visible { get { return visible; } set
            {
                visible = value;
                foreach (Object3D obj in children)
                    obj.Visible = value;
            }
        }

        public Object3D()
        {
            parent = null;
            visible = true;
        }

        //an object's 3D representation isn't updated until update_modelMatrix is called
        //as seen later, matrix updating is only performed strictly before frame render
        public virtual void update_modelMatrix()
        {
            Matrix4 temp_matrix;
            Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
            Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
            Matrix4 scale_matrix = Matrix4.CreateScale(scale);
            temp_matrix = scale_matrix * rotation_matrix * translation_matrix;

            if (parent == null)
                modelMatrix = temp_matrix;
            else
                modelMatrix = temp_matrix * parent.modelMatrix;

            foreach (Object3D obj in children)
                obj.update_modelMatrix();

            modified = false;
        }

        public static float DegToRad(int deg)
        {
            return 3.141526f * deg / 180.0f;
        }

        public static int RadToDeg(float rad)
        {
            return (int)(rad * 180 / 3.141526f);
        }

        public void SetAnglePlane(int angle_deg)
        {
            if(parent == null)
            Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2)   // only when has parent
                     * Quaternion.FromAxisAngle(new Vector3(0, 0, 1), DegToRad(angle_deg));
        }
    }

    public class ObjectSimple3D: Object3D
    {
        protected SFModel3D mesh = null;

        public SFModel3D Mesh { get { return mesh; } set { mesh = value; } }
    }
}
