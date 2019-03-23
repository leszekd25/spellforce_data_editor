using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using System.IO;
using SpellforceDataEditor.SFMap;

namespace SpellforceDataEditor.special_forms
{
    public partial class MapEditorForm : Form
    {
        SFMap.SFMap map = null;
        SF3D.SFRender.SFRenderEngine render_engine = new SF3D.SFRender.SFRenderEngine();
        SFCFF.SFGameData gamedata = new SFCFF.SFGameData();

        bool mouse_pressed = false;
        Point mouse_pos = Cursor.Position;

        public MapEditorForm()
        {
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(OpenMap.ShowDialog() == DialogResult.OK)
            {
                gamedata.Unload();
                if (gamedata.Load(SFUnPak.SFUnPak.game_directory_name + "\\data\\GameData.cff") != 0)
                    return;
                map = new SFMap.SFMap();
                if(map.Load(OpenMap.FileName, render_engine, gamedata, StatusText) != 0)
                    StatusText.Text = "Failed to load map";
                render_engine.AssignHeightMap(map.heightmap);
                RenderWindow.Invalidate();
            }
        }

        private void MapEditorForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("game_directory.txt"))
            {
                string gamedir = File.ReadAllText("game_directory.txt");
                int result = SFUnPak.SFUnPak.SpecifyGameDirectory(gamedir); //render_engine.SpecifyGameDirectory(dname);
                if (result == 0)
                {
                    //ready = true;
                    SFResources.SFResourceManager.FindAllMeshes();
                }
                else
                    Close();
                //return result;
            }

            render_engine.Initialize(new Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));
            render_engine.camera.Position = new Vector3(0, 25, 12);
            render_engine.camera.Lookat = new Vector3(0, 0, 0);
            render_engine.scene_manager.gamedata = gamedata;

            RenderWindow.Invalidate();
        }

        private void RenderWindow_Paint(object sender, PaintEventArgs e)
        {
            RenderWindow.MakeCurrent();
            render_engine.RenderFrame();
            RenderWindow.SwapBuffers();
        }

        private void RenderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_pressed = true;
            EnableAnimation();
            //set cursor position
            Cursor.Position = new Point(this.Location.X + RenderWindow.Location.X + (RenderWindow.Size.Width / 2), this.Location.Y + RenderWindow.Location.Y + (RenderWindow.Size.Height / 2));
            mouse_pos = Cursor.Position;
        }

        private void RenderWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_pressed = false;
            DisableAnimation();
        }

        private void RenderWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_pressed)
                return;

            float mult_f = 0.003f;
            //get mouse delta
            float mx, my;
            mx = Cursor.Position.X - mouse_pos.X; my = Cursor.Position.Y - mouse_pos.Y;
            //produce angle difference
            //render_engine.camera.Direction += new Vector2(mx * mult_f, -my * mult_f);
            //set cursor position
            Cursor.Position = new Point(this.Location.X + RenderWindow.Location.X + (RenderWindow.Size.Width / 2), this.Location.Y + RenderWindow.Location.Y + (RenderWindow.Size.Height / 2));
        }

        private void RenderWindow_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (!mouse_pressed)
                return;

            float cam_speed = 61; //limited by framerate

            //calculate movement vector
            //Vector3 cam_move = (((render_engine.camera.Lookat - render_engine.camera.Position).Normalized()) * cam_speed) / render_engine.scene_manager.frames_per_second;

            if (e.KeyChar == 'w')
                render_engine.camera.translate(new Vector3(0, 0, -cam_speed / render_engine.scene_manager.frames_per_second));
            else if (e.KeyChar == 's')
                render_engine.camera.translate(new Vector3(0, 0, cam_speed / render_engine.scene_manager.frames_per_second));
            if (e.KeyChar == 'a')
                render_engine.camera.translate(new Vector3(-cam_speed / render_engine.scene_manager.frames_per_second, 0, 0));
            else if (e.KeyChar == 'd')
                render_engine.camera.translate(new Vector3(cam_speed / render_engine.scene_manager.frames_per_second, 0, 0));


            if (e.KeyChar == 'c')
            {
                SFCoord cam_c = new SFCoord((short)render_engine.camera.Position.X, (short)render_engine.camera.Position.Z);
                cam_c.y = map.height - cam_c.y - 1;
                System.Diagnostics.Debug.WriteLine("CAMERA MAP XYZ: " + cam_c.ToString() + "|" + map.heightmap.GetZ(cam_c));
                //map.heightmap.GetIslandByHeight(cam_c, 120);
            }

            AdjustCameraZ();
            /*System.Diagnostics.Debug.WriteLine(render_engine.camera.Position+", CHUNK "
                + ((int)(render_engine.camera.Position.X/16)).ToString()+" "
                + ((int)(render_engine.camera.Position.Z/16)).ToString());*/
        }

        private void EnableAnimation()
        {
            TimerAnimation.Enabled = true;
            TimerAnimation.Interval = 1000 / render_engine.scene_manager.frames_per_second;
            TimerAnimation.Start();
            render_engine.scene_manager.delta_timer.Restart();
        }

        private void DisableAnimation()
        {
            TimerAnimation.Stop();
            TimerAnimation.Enabled = false;
            render_engine.scene_manager.delta_timer.Stop();
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            TimerAnimation.Start();
            render_engine.scene_manager.LogicStep();
            RenderWindow.Invalidate();
        }

        private void AdjustCameraZ()
        {
            if(map != null)
            {
                Vector2 p = new Vector2(render_engine.camera.Position.X, render_engine.camera.Position.Z);
                float z = map.heightmap.GetRealZ(p);
                render_engine.camera.translate(new Vector3(0, 25+z - render_engine.camera.Position.Y, 0));
            }
        }

        private void MapEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (map != null)
            {
                map.Unload();
                gamedata.Unload();
                render_engine.scene_manager.ClearScene();
                render_engine.AssignHeightMap(null);
                if (MainForm.viewer != null)
                    MainForm.viewer.ResetScene();
                SFResources.SFResourceManager.DisposeAll();
            }
        }
    }
}
