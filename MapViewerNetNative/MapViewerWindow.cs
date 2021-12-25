using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using SFEngine.SF3D;
using SFEngine.SF3D.Physics;
using SFEngine.SF3D.SceneSynchro;
using SFEngine.SF3D.SFRender;
using SFEngine.SFLua;
using SFEngine.SFCFF;
using SFEngine.SFUnPak;
using SFEngine.SFResources;
using SFEngine.SFMap;

namespace MapViewerNetNative
{
    public struct SpecialKeysPressed
    {
        public bool Ctrl;
        public bool Shift;
    }

    public class MapViewerWindow: GameWindow
    {
        SFMap map = null;

        bool mouse_pressed = false;      // if true, mouse is pressed and in render window
        MouseButton mouse_last_pressed = MouseButton.Left;  // last mouse button pressed
        Vector2 mouse_current_pos = new Vector2(0, 0);   // while moving, this keeps track of mouse position

        bool dynamic_render = true;     // animations will work if this is enabled

        bool mouse_on_view = false;      // if true, mouse is in render window
        Vector2 scroll_mouse_start = new Vector2(0, 0);
        bool mouse_scroll = false;
        public float zoom_level = 1.0f;
        float camera_speed_factor = 1.0f;

        bool[] arrows_pressed = new bool[] { false, false, false, false };  // left, right, up, down
        bool[] rotation_pressed = new bool[] { false, false, false, false };// left, right, up, down
        SpecialKeysPressed special_pressed = new SpecialKeysPressed();

        double cur_time = 0.0;
        int updates_this_second = 0;

        public MapViewerWindow(): base(800, 600, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 8), "Map Viewer", GameWindowFlags.Default, DisplayDevice.Default, 4, 2, OpenTK.Graphics.GraphicsContextFlags.Default)
        {
            Load += OnWindowLoad;
            UpdateFrame += OnWindowUpdate;
            RenderFrame += OnWindowRender;
            MouseDown += OnWindowMouseDown;
            MouseMove += OnWindowMouseMove;
            MouseWheel += OnWindowMouseScroll;
            MouseUp += OnWindowMouseUp;
            KeyDown += OnWindowKeyPress;
            KeyUp += OnWindowKeyRelease;
            Resize += OnWindowResize;

            VSync = (SFEngine.Settings.VSync ? VSyncMode.On : VSyncMode.Off);
        }

        private void AddCameraZoom(int delta)
        {
            if (delta < 0)
            {
                zoom_level *= 1.1f;
                if (zoom_level > 6)
                    zoom_level = 6;
            }
            else
            {
                zoom_level *= 0.9f;
                if (zoom_level < 0.1f)
                    zoom_level = 0.1f;
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
                    continue;
                // 25 * zoom_level

                if (pos.X < xmin)
                    xmin = pos.X;
                else if (pos.X + 16 > xmax)
                    xmax = pos.X + 16;
                if (pos.Z < ymin)
                    ymin = pos.Z;
                else if (pos.Z + 16 > ymax)
                    ymax = pos.Z + 16;
                if (chunk_node.MapChunk.aabb.a.Y < zmin)
                    zmin = chunk_node.MapChunk.aabb.a.Y;
                if (chunk_node.MapChunk.aabb.b.Y > zmax)
                    zmax = chunk_node.MapChunk.aabb.b.Y;
            }
            BoundingBox aabb = new BoundingBox(new Vector3(xmin, zmin, ymin), new Vector3(xmax, zmax, ymax));
            //SF3D.Physics.BoundingBox aabb = SF3D.Physics.BoundingBox.FromPoints(SFRenderEngine.scene.camera.Frustum.frustum_vertices);

            SFRenderEngine.scene.atmosphere.sun_light.SetupLightView(aabb);
            SFRenderEngine.scene.atmosphere.sun_light.ShadowDepth = max_dist;
        }

        public void SetCameraElevation(float h)
        {
            // preserve lookat
            Vector3 cur_lookat = SFRenderEngine.scene.camera.Lookat - SFRenderEngine.scene.camera.position;

            SFRenderEngine.scene.camera.SetPosition(
                new Vector3(
                    SFRenderEngine.scene.camera.position.X,
                    h + map.heightmap.GetRealZ(SFRenderEngine.scene.camera.position.Xz),
                    SFRenderEngine.scene.camera.position.Z)
                );

            SFRenderEngine.scene.camera.SetLookat(SFRenderEngine.scene.camera.position + cur_lookat);
        }

        // moves camera to given map coordinate, preserving camera elevation
        public void SetCameraMapPos(SFCoord pos)
        {
            SFRenderEngine.scene.camera.SetPosition(new Vector3(pos.x, 0, map.height - 1 - pos.y));
            AdjustCameraZ();
        }

        // moves camera to an arbitrary point in the world
        public void SetCameraWorldPos(Vector3 pos)
        {
            SFRenderEngine.scene.camera.SetPosition(pos);
        }

        // moves camera to a given point on the map, preserving camera elevation
        // not limited to grid points
        public void SetCameraWorldMapPos(Vector2 pos)
        {
            SFRenderEngine.scene.camera.SetPosition(new Vector3(pos.X, 0, pos.Y));
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
                return;
            SFRenderEngine.scene.delta_timer.Restart();
            if (!force_load)
                return;
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
                return;
            if (map != null)
            {
                foreach (var unit in map.unit_manager.units)
                {
                    foreach (SceneNodeAnimated anim_node in unit.node.Children)
                    {
                        anim_node.SetAnimation(null, false);
                        anim_node.SetSkeletonSkin(anim_node.Skeleton, anim_node.Skin);
                    }
                }
            }
        }

        void OnWindowLoad(object sender, EventArgs e)
        {
            // load settings and initialize file system
            SFEngine.Settings.Load();
            if (!SFUnPak.game_directory_specified)
                throw new Exception("FAILED TO LOAD GAME DIRECTORY");

            // load SQL stuff
            SFLuaEnvironment.LoadSQL(false);


            // load gamedata
            SFCategoryManager.gamedata.Load(SFUnPak.game_directory_name + "\\data\\GameData.cff");
            SFCategoryManager.manual_SetGamedata();

            // find all resources
            if (!SFResourceManager.ready)
                SFResourceManager.FindAllMeshes();


            // create scene and initialize rendering engine

            MakeCurrent();
            SFRenderEngine.scene.Init();
            SFRenderEngine.Initialize(new Vector2(800, 600));
            SFRenderEngine.scene.atmosphere.SetSunLocation(135, 60);
            SFRenderEngine.SetObjectFadeRange(SFEngine.Settings.ObjectFadeMin, SFEngine.Settings.ObjectFadeMax);

            SFRenderEngine.scene.root.Visible = true;

            // create and generate map
            map = new SFMap();
            if (map.Load(SFUnPak.game_directory_name + "\\map\\campaign\\000_Greyfell.map") != 0)
                throw new Exception("FAILED TO LOAD MAP");

            SFRenderEngine.scene.map = map;

            map.selection_helper.SetCursorPosition(new SFCoord(1, 1));
            map.selection_helper.SetCursorVisibility(true);

            SetCameraWorldMapPos(new Vector2(map.width / 2, map.height / 2));
            SetCameraAzimuthAltitude((float)((90 * Math.PI) / 180.0f), (float)((-70 * Math.PI) / 180.0f));
            zoom_level = 1;
            AdjustCameraZ();

            SFEngine.Settings.DynamicMap = dynamic_render;
            if (SFEngine.Settings.DynamicMap)
                EnableAnimation(true);

            GC.Collect();
        }

        void OnWindowResize(object sender, EventArgs e)
        {
            SFRenderEngine.ResizeView(new Vector2(Width, Height));
            MakeCurrent();
        }

        void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Middle)
            {
                scroll_mouse_start = new Vector2(e.X, e.Y);
                mouse_scroll = true;
                return;
            }
            mouse_pressed = true;
            mouse_last_pressed = e.Button;
        }

        void OnWindowMouseMove(object sender, MouseMoveEventArgs e)
        {
            mouse_current_pos = new Vector2(e.X, e.Y);
        }

        void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Middle)
            {
                scroll_mouse_start = new Vector2(0, 0);
                mouse_scroll = false;
                return;
            }
            mouse_pressed = false;
        }

        void OnWindowMouseScroll(object sender, MouseWheelEventArgs e)
        {
            AddCameraZoom(e.Delta);
        }

        void OnWindowKeyPress(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    arrows_pressed[0] = true;
                    break;
                case Key.Right:
                    arrows_pressed[1] = true;
                    break;
                case Key.Up:
                    arrows_pressed[2] = true;
                    break;
                case Key.Down:
                    arrows_pressed[3] = true;
                    break;
                case Key.Home:
                    rotation_pressed[0] = true;
                    break;
                case Key.End:
                    rotation_pressed[1] = true;
                    break;
                case Key.PageUp:
                    rotation_pressed[2] = true;
                    break;
                case Key.PageDown:
                    rotation_pressed[3] = true;
                    break;
                case Key.Insert:
                    AddCameraZoom(-1);
                    break;
                case Key.Delete:
                    AddCameraZoom(1);
                    break;
                case Key.G:
                    if (e.Control)
                    {
                        SFEngine.Settings.DisplayGrid = !SFEngine.Settings.DisplayGrid;
                        SFRenderEngine.RecompileMainShaders();
                    }
                    break;
                case Key.H:
                    if (e.Control)
                    {
                        SFEngine.Settings.VisualizeHeight = !SFEngine.Settings.VisualizeHeight;
                        SFRenderEngine.RecompileMainShaders();
                    }
                    break;
                case Key.F:
                    if (e.Control)
                    {
                        //SFRenderEngine.prepare_dump = true;
                        SFRenderEngine.render_shadowmap_depth = !SFRenderEngine.render_shadowmap_depth;
                    }
                    break;
                case Key.ControlLeft:
                    special_pressed.Ctrl = true;
                    break;
                case Key.ShiftLeft:
                    special_pressed.Shift = true;
                    break;
                default:
                    break;
            }
        }

        void OnWindowKeyRelease(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    arrows_pressed[0] = false;
                    break;
                case Key.Right:
                    arrows_pressed[1] = false;
                    break;
                case Key.Up:
                    arrows_pressed[2] = false;
                    break;
                case Key.Down:
                    arrows_pressed[3] = false;
                    break;
                case Key.Home:
                    rotation_pressed[0] = false;
                    break;
                case Key.End:
                    rotation_pressed[1] = false;
                    break;
                case Key.PageUp:
                    rotation_pressed[2] = false;
                    break;
                case Key.PageDown:
                    rotation_pressed[3] = false;
                    break;
                case Key.ControlLeft:
                    special_pressed.Ctrl = false;
                    break;
                case Key.ShiftLeft:
                    special_pressed.Shift = false;
                    break;
                default:
                    break;
            }
        }


        void OnWindowUpdate(object sender, FrameEventArgs e)
        {
            ProcessEvents(true);

            cur_time += e.Time;
            if(cur_time > 1.0)
            {
                cur_time -= 1.0;
                Title = "Map Viewer (fps: " + updates_this_second + ")";
                updates_this_second = 0;
            }

            if (map == null)
                return;

            // rotating view by mouse
            if (mouse_scroll)
            {
                Vector2 scroll_translation = (mouse_current_pos - scroll_mouse_start) * (float)e.Time / 250f;
                if (scroll_translation != Vector2.Zero)
                    SetCameraAzimuthAltitude(SFRenderEngine.scene.camera.Direction.X - scroll_translation.X, SFRenderEngine.scene.camera.Direction.Y - scroll_translation.Y);
            }

            // moving view by arrow keys
            Vector2 movement_vector = new Vector2(0, 0);
            if (arrows_pressed[0])
                movement_vector += new Vector2(1, 0);
            if (arrows_pressed[1])
                movement_vector += new Vector2(-1, 0);
            if (arrows_pressed[2])
                movement_vector += new Vector2(0, -1);
            if (arrows_pressed[3])
                movement_vector += new Vector2(0, +1);

            if (movement_vector != new Vector2(0, 0))
            {
                movement_vector = SFEngine.MathUtils.RotateVec2(movement_vector, SFRenderEngine.scene.camera.Direction.X + (float)(Math.PI / 2));
                movement_vector *= 60.0f * camera_speed_factor * (float)e.Time;
                MoveCameraWorldMapPos(SFRenderEngine.scene.camera.position.Xz + movement_vector);
            }

            // rotating view by home/end/pageup/pagedown
            movement_vector = new Vector2(0, 0);
            if (rotation_pressed[0])
                movement_vector += new Vector2(-1, 0);
            if (rotation_pressed[1])
                movement_vector += new Vector2(1, 0);
            if (rotation_pressed[2])
                movement_vector += new Vector2(0, -1);
            if (rotation_pressed[3])
                movement_vector += new Vector2(0, 1);

            if (movement_vector != new Vector2(0, 0))
            {
                movement_vector *= 2.0f * (float)e.Time;
                SetCameraAzimuthAltitude(SFRenderEngine.scene.camera.Direction.X - movement_vector.X, SFRenderEngine.scene.camera.Direction.Y - movement_vector.Y);
            }

            SFRenderEngine.scene.camera.Update(0);
            
            // heavy tasks
            map.ocean.SetPosition(SFRenderEngine.scene.camera.position);
            SFRenderEngine.UpdateVisibleChunks();
            map.selection_helper.Update();

            SFRenderEngine.scene.Update((float)e.Time);

            SFRenderEngine.ui.Update();
            if (SFEngine.Settings.EnableCascadeShadows)
            {
                SFRenderEngine.scene.atmosphere.sun_light.CalculateCascadeLightMatrix(SFRenderEngine.scene.camera);
            }
            UpdateSunFrustum();
        }

        void OnWindowRender(object sender, FrameEventArgs e)
        {
            SFRenderEngine.RenderScene();
            SwapBuffers();

            updates_this_second += 1;
        }
    }
}
