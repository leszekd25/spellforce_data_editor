/*
 * objectAnimated3D is an object which is displayed using underlying skin and skeleton
 * Every time skeleton is updated according to its animation, skin bone matrices are manipulated to modify skin mesh (in shader)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public class objectAnimated: Object3D
    {
        public SFSkeleton skeleton { get; private set; } = null;
        public SFModelSkin skin { get; private set; } = null;
        public SFAnimation animation { get; private set; } = null;
        public Matrix4[] bone_transforms = null;
        public float anim_current_time { get; private set; } = 0;
        public bool anim_playing { get; set; } = false;

        public void SetSkeletonSkin(SFSkeleton _skeleton, SFModelSkin _skin)
        {
            if (_skeleton != null)
            {
                bone_transforms = new Matrix4[_skeleton.bone_count];
                for (int i = 0; i < _skeleton.bone_count; i++)
                    bone_transforms[i] = Matrix4.Identity;
            }
            else
                bone_transforms = null;
            skeleton = _skeleton;
            skin = _skin;

        }

        public void SetAnimation(SFAnimation _animation, bool play = true)
        {
            animation = _animation;
            anim_current_time = 0f;
            anim_playing = play;
            if (play == true)
                update_transforms();
        }

        public void step_animation(float dt)
        {
            anim_current_time += dt;
            if (animation == null)
                return;
            if (anim_current_time >= animation.max_time)
                anim_current_time -= animation.max_time;

            update_transforms();
        }

        private void update_transforms()
        {
            for (int i = 0; i < bone_transforms.Length; i++)
                bone_transforms[i] = animation.CalculateBoneMatrix(i, anim_current_time).to_mat4();
            skeleton.CalculateTransformation(bone_transforms, ref bone_transforms);
            for (int i = 0; i < bone_transforms.Length; i++)
                bone_transforms[i] = skeleton.bone_inverted_matrices[i] * bone_transforms[i];
        }

    }

    //a bit broken probably, that's what you get for not implementing doubly linked tree :^)
    public class ObjectBoneAnchor: objectAnimated
    {
        public objectAnimated parent_obj { get; private set; } = null;
        public int bone_index { get; private set; } = -1;
        public override bool Modified { get { return true; } }

        public override void update_modelMatrix()
        {
            Matrix4 temp_matrix;
            Matrix4 translation_matrix = Matrix4.CreateTranslation(position);
            Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(rotation);
            Matrix4 scale_matrix = Matrix4.CreateScale(scale);
            temp_matrix = translation_matrix * rotation_matrix * scale_matrix;

            if (parent_obj == null)
                modelMatrix = temp_matrix;
            else if (bone_index != -1)
                modelMatrix = temp_matrix * parent_obj.skeleton.bone_reference_matrices[bone_index]
                    * parent_obj.bone_transforms[bone_index] * parent_obj.ModelMatrix;

                //children update matrices here
        }

        public void SetBone(objectAnimated obj, string name)
        {
            bone_index = -1;
            parent_obj = null;
            if (obj == null)
                return;
            if (obj.skeleton == null)
                return;
            parent_obj = obj;
            for(int i = 0; i < obj.skeleton.bone_count; i++)
            {
                if(obj.skeleton.bone_names[i].Contains(name))
                {
                    bone_index = i;
                    break;
                }
            }
        }
    }
}
