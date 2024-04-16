using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SFEngine.SF3D.Physics;
using SFEngine.SF3D.SceneSynchro;
using SFEngine.SF3D.SFRender;
using SFEngine.SFCFF;
using SFEngine.SFLua;
using SFEngine.SFMap;
using SFEngine.SFResources;
using SFEngine.SFUnPak;
using System;

namespace MapViewerNetNative
{
    public struct SpecialKeysPressed
    {
        public bool Ctrl;
        public bool Shift;
    }

    public class MapViewerWindow : GameWindow
    {
        SFMap map = null;

        Vector2 mouse_current_pos = new Vector2(0, 0);   // while moving, this keeps track of mouse position

        bool dynamic_render = true;     // animations will work if this is enabled

        Vector2 scroll_mouse_start = new Vector2(0, 0);
        bool mouse_scroll = false;
        public float zoom_level = 1.0f;
        float camera_speed_factor = 1.0f;

        bool[] arrows_pressed = new bool[] { false, false, false, false };  // left, right, up, down
        bool[] rotation_pressed = new bool[] { false, false, false, false };// left, right, up, down

        double cur_time = 0.0;
        int updates_this_second = 0;

        public MapViewerWindow() : base(
            new GameWindowSettings() { UpdateFrequency = 0 },
            new NativeWindowSettings() { ClientSize = (1024, 768), RedBits = 8, GreenBits = 8, BlueBits = 8, AlphaBits = 8, DepthBits = 24, StencilBits = 8, Title = "MapViewer", WindowState = OpenTK.Windowing.Common.WindowState.Normal, API = OpenTK.Windowing.Common.ContextAPI.OpenGL, APIVersion = new Version(4, 2), Flags = ContextFlags.Default })
            //base(1024, 768, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 8), "Map Viewer", GameWindowFlags.Default, DisplayDevice.Default, 4, 2, OpenTK.Graphics.GraphicsContextFlags.Default)
        {
            UpdateFrame += OnWindowUpdate;
            MouseDown += OnWindowMouseDown;
            MouseMove += OnWindowMouseMove;
            MouseWheel += OnWindowMouseScroll;
            MouseUp += OnWindowMouseUp;
            KeyDown += OnWindowKeyPress;
            KeyUp += OnWindowKeyRelease;
            Resize += OnWindowResize;

            VSync = (SFEngine.Settings.VSync ? VSyncMode.On : VSyncMode.Off);

            OnWindowLoad();
        }

        private void AddCameraZoom(float delta)
        {
            if (delta < 0)
            {
                zoom_level *= 1.1f;
                if (zoom_level > 6)
                {
                    zoom_level = 6;
                }
            }
            else
            {
                zoom_level *= 0.9f;
                if (zoom_level < 0.1f)
                {
                    zoom_level = 0.1f;
                }
            }
            AdjustCameraZ();
        }

        private void AdjustCameraZ()
        {
            if (map != null)
            {
                SetCameraElevation(25 * zoom_level);
            }
        }

        private void UpdateSunFrustum()
        {
            Vector2 p = new Vector2(SFRenderEngine.scene.camera.position.X, SFRenderEngine.scene.camera.position.Z);
            // calculate light bounding box

            // calculate visible heightmap bounding box, using chunks that are close enough
            float max_dist = Math.Max(
                60, 50 * zoom_level * (float)Math.Min(
                    3.0f, Math.Max(
                        0.6f, 1.0f / (0.001f + Math.Abs(
                            Math.Tan(
                                SFRenderEngine.scene.camera.Direction.Y))))));

            float xmin, xmax, ymin, ymax, zmin, zmax;
            xmin = 9999; ymin = 9999; xmax = -9999; ymax = -9999; zmin = 9999; zmax = -9999;
            foreach (SceneNodeMapChunk chunk_node in map.heightmap.visible_chunks)
            {
                Vector3 pos = chunk_node.position;

                if (max_dist < (p - new Vector2(pos.X + 8, pos.Z + 8)).Length)
                {
                    continue;
                }

                if (pos.X < xmin)
                {
                    xmin = pos.X;
                }
                else if (pos.X + 16 > xmax)
                {
                    xmax = pos.X + 16;
                }

                if (pos.Z < ymin)
                {
                    ymin = pos.Z;
                }
                else if (pos.Z + 16 > ymax)
                {
                    ymax = pos.Z + 16;
                }

                if (chunk_node.MapChunk.aabb.a.Y < zmin)
                {
                    zmin = chunk_node.MapChunk.aabb.a.Y;
                }

                if (chunk_node.MapChunk.aabb.b.Y > zmax)
                {
                    zmax = chunk_node.MapChunk.aabb.b.Y;
                }
            }
            BoundingBox aabb = new BoundingBox(new Vector3(xmin, zmin, ymin), new Vector3(xmax, zmax, ymax));

            SFRenderEngine.scene.atmosphere.sun_light.SetupLightView(aabb);
            SFRenderEngine.scene.atmosphere.sun_light.ShadowDepth = max_dist;
        }

        public void SetCameraElevation(float h)
        {
            // preserve lookat
            Vector3 cur_lookat = SFRenderEngine.scene.camera.Lookat - SFRenderEngine.scene.camera.position;

            SFRenderEngine.scene.camera.Position = new Vector3(
                    SFRenderEngine.scene.camera.position.X,
                    h + map.heightmap.GetRealZ(SFRenderEngine.scene.camera.position.Xz),
                    SFRenderEngine.scene.camera.position.Z);

            SFRenderEngine.scene.camera.SetLookat(SFRenderEngine.scene.camera.position + cur_lookat);
        }

        // moves camera to given map coordinate, preserving camera elevation
        public void SetCameraMapPos(SFCoord pos)
        {
            SFRenderEngine.scene.camera.Position = new Vector3(pos.x, 0, map.height - 1 - pos.y);
            AdjustCameraZ();
        }

        // moves camera to an arbitrary point in the world
        public void SetCameraWorldPos(Vector3 pos)
        {
            SFRenderEngine.scene.camera.Position = pos;
        }

        // moves camera to a given point on the map, preserving camera elevation
        // not limited to grid points
        public void SetCameraWorldMapPos(Vector2 pos)
        {
            SFRenderEngine.scene.camera.Position = new Vector3(pos.X, 0, pos.Y);
            AdjustCameraZ();
        }

        public void MoveCameraWorldMapPos(Vector2 pos)
        {
            Vector3 cur_lookat = SFRenderEngine.scene.camera.Lookat - SFRenderEngine.scene.camera.position;
            SetCameraWorldMapPos(pos);
            SFRenderEngine.scene.camera.SetLookat(SFRenderEngine.scene.camera.position + cur_lookat);
        }

        // sets camera angles (this also modifies direction)
        // 0, 0 = UnitX
        // 270, 0 = UnitZ
        public void SetCameraAzimuthAltitude(float azimuth, float altitude)
        {
            SFRenderEngine.scene.camera.SetAzimuthAltitude(new Vector2(azimuth, altitude));
        }

        // sets camera direction (this also modifies angle)
        public void SetCameraLookAt(Vector3 pos)
        {
            SFRenderEngine.scene.camera.SetLookat(pos);
        }

        // attempts to center camera on the selected map position, preserving camera angle
        public void SetCameraViewPoint(SFCoord pos)
        {
            SetCameraWorldMapPos(new Vector2(pos.x, map.height - 1 - pos.y + 10));
            SetCameraAzimuthAltitude((float)((90 * Math.PI) / 180.0f), (float)((-70 * Math.PI) / 180.0f));
            zoom_level = 1;
            AdjustCameraZ();
        }

        public void ResetCamera()
        {
            SetCameraWorldMapPos(new Vector2(map.width / 2, map.height / 2));
            SetCameraAzimuthAltitude((float)((90 * Math.PI) / 180.0f), (float)((-70 * Math.PI) / 180.0f));
            zoom_level = 1;
            AdjustCameraZ();
        }
        public void EnableAnimation(bool force_load = false)
        {
            dynamic_render = SFEngine.Settings.DynamicMap;
            if (!SFEngine.Settings.DynamicMap)
            {
                return;
            }

            SFRenderEngine.scene.delta_timer.Restart();
            if (!force_load)
            {
                return;
            }

            if (map != null)
            {
                foreach (var unit in map.unit_manager.units)
                {
                    map.unit_manager.RestartAnimation(unit);
                }
            }
        }

        // disables unit animation
        public void DisableAnimation(bool force_unload = false)
        {
            dynamic_render = false;
            SFRenderEngine.scene.delta_timer.Stop();
            if (!force_unload)
            {
                return;
            }

            if (map != null)
            {
                foreach (var unit in map.unit_manager.units)
                {
                    foreach (SceneNodeAnimated anim_node in unit.node.children)
                    {
                        anim_node.SetAnimation(null, false);
                        // anim_node.SetSkeletonSkin(anim_node.Skeleton, anim_node.Skin);
                    }
                }
            }
        }

        void OnWindowLoad()
        {
            // load settings and initialize file system
            SFEngine.Settings.Load();
            SFEngine.Settings.EditorMode = false;
            if (!SFUnPak.game_directory_specified)
            {
                throw new Exception("FAILED TO LOAD GAME DIRECTORY");
            }
            SFResourceManager.ListAllFilesystemResources();

            // load SQL stuff
            SFLuaEnvironment.LoadSQL(false);


            // load gamedata
            SFCategoryManager.gamedata.Load(SFUnPak.game_directory_name + "\\data\\GameData.cff");
            SFCategoryManager.manual_SetGamedata();

            // find all resources
            SFResourceManager.ListAllPakResources();


            // create scene and initialize rendering engine

            MakeCurrent();
            SFRenderEngine.scene.Init();
            SFRenderEngine.Initialize(new Vector2(800, 600));
            SFRenderEngine.ResetTextures();
            SFRenderEngine.scene.GenerateMissingMesh();
            SFRenderEngine.scene.atmosphere.SetSunLocation(135, 60);
            SFRenderEngine.SetObjectFadeRange(SFEngine.Settings.ObjectFadeMin, SFEngine.Settings.ObjectFadeMax);

            SFRenderEngine.scene.root.Visible = true;

            // create and generate map
            string map_name = "";
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.DefaultExt = ".map";
            ofd.Filter = "MAP files (*.map)|*.map";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                throw new Exception("DID NOT SELECT MAP");
            }

            map_name = ofd.FileName;


            map = new SFMap();
            if (map.Load(ofd.FileName) != 0)
            {
                throw new Exception("FAILED TO LOAD MAP");
            }

            SFRenderEngine.scene.map = map;

            SetCameraWorldMapPos(new Vector2(map.width / 2, map.height / 2));
            SetCameraAzimuthAltitude((float)((90 * Math.PI) / 180.0f), (float)((-70 * Math.PI) / 180.0f));
            zoom_level = 1;
            AdjustCameraZ();

            SFEngine.Settings.DynamicMap = dynamic_render;
            if (SFEngine.Settings.DynamicMap)
            {
                EnableAnimation(true);
            }

            GC.Collect();
        }

        void OnWindowResize(ResizeEventArgs e)
        {
            SFRenderEngine.ResizeView(e.Size);
            MakeCurrent();
        }

        void OnWindowMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Middle)
            {
                scroll_mouse_start = MousePosition;
                mouse_scroll = true;
                return;
            }
        }

        void OnWindowMouseMove(MouseMoveEventArgs e)
        {
            mouse_current_pos = new Vector2(e.X, e.Y);
        }

        void OnWindowMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Middle)
            {
                scroll_mouse_start = new Vector2(0, 0);
                mouse_scroll = false;
                return;
            }
        }

        void OnWindowMouseScroll(MouseWheelEventArgs e)
        {
            AddCameraZoom(e.Offset.Y);
        }

        void OnWindowKeyPress(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Left:
                    arrows_pressed[0] = true;
                    break;
                case Keys.Right:
                    arrows_pressed[1] = true;
                    break;
                case Keys.Up:
                    arrows_pressed[2] = true;
                    break;
                case Keys.Down:
                    arrows_pressed[3] = true;
                    break;
                case Keys.Home:
                    rotation_pressed[0] = true;
                    break;
                case Keys.End:
                    rotation_pressed[1] = true;
                    break;
                case Keys.PageUp:
                    rotation_pressed[2] = true;
                    break;
                case Keys.PageDown:
                    rotation_pressed[3] = true;
                    break;
                case Keys.Insert:
                    AddCameraZoom(-1);
                    break;
                case Keys.Delete:
                    AddCameraZoom(1);
                    break;
                default:
                    break;
            }
        }

        void OnWindowKeyRelease(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Left:
                    arrows_pressed[0] = false;
                    break;
                case Keys.Right:
                    arrows_pressed[1] = false;
                    break;
                case Keys.Up:
                    arrows_pressed[2] = false;
                    break;
                case Keys.Down:
                    arrows_pressed[3] = false;
                    break;
                case Keys.Home:
                    rotation_pressed[0] = false;
                    break;
                case Keys.End:
                    rotation_pressed[1] = false;
                    break;
                case Keys.PageUp:
                    rotation_pressed[2] = false;
                    break;
                case Keys.PageDown:
                    rotation_pressed[3] = false;
                    break;
                default:
                    break;
            }
        }


        void OnWindowUpdate(FrameEventArgs e)
        {
            cur_time += e.Time;
            if (cur_time > 1.0)
            {
                cur_time -= 1.0;
                Title = "Map Viewer (fps: " + updates_this_second + ")";
                updates_this_second = 0;
            }

            if (map == null)
            {
                return;
            }

            // rotating view by mouse
            if (mouse_scroll)
            {
                Vector2 scroll_translation = (mouse_current_pos - scroll_mouse_start) * (float)e.Time / 250f;
                if (scroll_translation != Vector2.Zero)
                {
                    SetCameraAzimuthAltitude(SFRenderEngine.scene.camera.Direction.X - scroll_translation.X, SFRenderEngine.scene.camera.Direction.Y - scroll_translation.Y);
                }
            }

            // moving view by arrow keys
            Vector2 movement_vector = new Vector2(0, 0);
            if (arrows_pressed[0])
            {
                movement_vector += new Vector2(1, 0);
            }

            if (arrows_pressed[1])
            {
                movement_vector += new Vector2(-1, 0);
            }

            if (arrows_pressed[2])
            {
                movement_vector += new Vector2(0, -1);
            }

            if (arrows_pressed[3])
            {
                movement_vector += new Vector2(0, +1);
            }

            if (movement_vector != new Vector2(0, 0))
            {
                SFEngine.MathUtils.RotateVec2Mirrored(in movement_vector, SFRenderEngine.scene.camera.Direction.X + (float)(Math.PI / 2), out movement_vector);
                movement_vector *= 60.0f * camera_speed_factor * (float)e.Time;
                MoveCameraWorldMapPos(SFRenderEngine.scene.camera.position.Xz + movement_vector);
            }

            // rotating view by home/end/pageup/pagedown
            movement_vector = new Vector2(0, 0);
            if (rotation_pressed[0])
            {
                movement_vector += new Vector2(-1, 0);
            }

            if (rotation_pressed[1])
            {
                movement_vector += new Vector2(1, 0);
            }

            if (rotation_pressed[2])
            {
                movement_vector += new Vector2(0, -1);
            }

            if (rotation_pressed[3])
            {
                movement_vector += new Vector2(0, 1);
            }

            if (movement_vector != new Vector2(0, 0))
            {
                movement_vector *= 2.0f * (float)e.Time;
                SetCameraAzimuthAltitude(SFRenderEngine.scene.camera.Direction.X - movement_vector.X, SFRenderEngine.scene.camera.Direction.Y - movement_vector.Y);
            }

            SFRenderEngine.scene.camera.Update(0);

            // heavy tasks
            map.ocean.SetPosition(SFRenderEngine.scene.camera.position);
            SFRenderEngine.scene.UpdateVisibleChunks(map.heightmap);

            SFRenderEngine.scene.Update((float)e.Time);

            SFRenderEngine.ui.Update();
            UpdateSunFrustum();

            // pure render stuff
            SFRenderEngine.RenderScene();
            SwapBuffers();

            updates_this_second += 1;
        }
    }
}
