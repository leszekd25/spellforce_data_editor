using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using SpellforceDataEditor.SFMap;
using SpellforceDataEditor.SF3D.SFRender;
using SpellforceDataEditor.special_forms;
using SpellforceDataEditor.SF3D.SceneSynchro;
using System.Threading;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapMinimap : UserControl
    {
        /// <summary>
        /// Reference to the currently loaded map
        /// </summary>
        SFMap map;

        Bitmap cameraPositionTexture;
        int oldCameraX = 0;
        int oldCameraY = 0;
        int oldSize = 0;
        Color squareColor;
        int squareSize = 10;

        // store the minimapSettings setting
        // and default value
        public bool IsShown = true;

        Thread threadUpdateCamera;
        bool isUpdateCameraThreadRunning = false;

        public MapMinimap()
        {
            InitializeComponent();
            squareColor = Color.FromArgb(255, 255, 255, 255);
        }

        public void LoadMap(SFMap map)
        {
            this.map = map;
            if (IsShown)
            {
                Show();
                GenerateMinimap();
                generateTransparentCameraPosTex();
                UpdateCamera();
            }
        }

        public void CloseMap()
        {
            map = null;
            Hide();
        }

        private void pictureBoxCameraPosition_MouseClick(object sender, MouseEventArgs e)
        {
            if (map == null)
                return;

            // camera jump
            float minimap_x = map.width * ((float)e.X / (float)this.pictureBoxCameraPosition.Width);
            float minimap_y = map.height * ((float)e.Y / (float)this.pictureBoxCameraPosition.Height);
            float dist = Vector3.Distance(SFRenderEngine.scene.camera.Position, SFRenderEngine.scene.camera.Lookat);
            Vector3 direction = Vector3.Normalize(SFRenderEngine.scene.camera.Lookat);

            SFRenderEngine.scene.camera.Position = new Vector3(minimap_x, SFRenderEngine.scene.camera.Lookat.Z, minimap_y);
            //SFRenderEngine.scene.camera.Lookat = new Vector3(minimap_x, SFRenderEngine.scene.camera.Lookat.Z, minimap_y);
            SFRenderEngine.scene.camera.Direction = SFRenderEngine.scene.camera.Direction;

            // render new position
            MainForm.mapedittool.update_render = true;

            // update camera position texture
            UpdateCamera();
        }

        public void GenerateMinimap()
        {
            if (!Visible)
                return;

            if (map == null)
                return;

           var thread = new Thread(
                () =>
                {
                    float xPercentage, yPercentage, xRelative, yRelative;
                    byte tile_id;

                    int width = pictureBoxMapTexture.Width;
                    int height = pictureBoxMapTexture.Height;

                    Bitmap bitmap = new Bitmap(width, height);

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            // converting minimap coords to real map coords
                            xPercentage = (float)x / (float)width;
                            yPercentage = (float)y / (float)height;
                            xRelative = ((float)map.width * xPercentage);
                            yRelative = (float)map.height * yPercentage;

                            tile_id = map.heightmap.GetTile(new SFCoord((int)xRelative, (int)yRelative));

                            if (tile_id >= 224)
                                tile_id -= 223;

                            bitmap.SetPixel(x, height - 1 - y, map.heightmap.texture_manager.tile_average_color[tile_id]);
                        }
                    }

                    if (pictureBoxMapTexture.Image != null)
                        pictureBoxMapTexture.Image.Dispose();
                    pictureBoxMapTexture.Image = bitmap;
                }
            );
            thread.Start();
        }

        public void UpdateCamera()
        {
            // improvements:
            // include camera rotation
            // current mapping viewport to square is not really accurate..

            if (!Visible)
                return;

            if (map == null)
                return;

            if (cameraPositionTexture == null)
                return;

            // only run one thread at a time
            if (isUpdateCameraThreadRunning)
                return;

            isUpdateCameraThreadRunning = true;
            try
            {
                threadUpdateCamera = new Thread(
                   () =>
                   {
                       try
                       {
                           Bitmap bitmap = (Bitmap)cameraPositionTexture.Clone();

                           int cameraPositionTexture_width = cameraPositionTexture.Width;
                           int cameraPositionTexture_height = cameraPositionTexture.Height;

                           float zoomScale = MainForm.mapedittool.zoom_level;
                           int size = (int)(this.squareSize * zoomScale);

                           int newX = (int)(pictureBoxMapTexture.Width * (SFRenderEngine.scene.camera.Position.X / map.width));
                           int newY = (int)(pictureBoxMapTexture.Height * (SFRenderEngine.scene.camera.Position.Z / map.height));

                           // reset pixel at old camera pos
                           int deleteRadius = oldSize + 2;
                           for (int x = oldCameraX - deleteRadius; x < oldCameraX + deleteRadius; x++)
                           {
                               // valid x coord
                               if (x < 0 || x >= cameraPositionTexture_width)
                                   continue;

                               for (int y = oldCameraY - deleteRadius; y < oldCameraY + deleteRadius; y++)
                               {
                                   // valid y coord
                                   if (y < 0 || y >= cameraPositionTexture_height)
                                       continue;
                                   // clear pixel
                                   bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                               }
                           }
                           // save size
                           oldSize = size;

                           // create camera square
                           int squareSize = size;
                           int horizontalLine1Y = newY - squareSize;
                           int horizontalLine2Y = newY + squareSize;
                           int verticalLine1X = newX - squareSize;
                           int verticalLine2X = newX + squareSize;

                           // draw horizontal line 1 and line 2
                           bool line1 = false;
                           bool line2 = false;
                           if (horizontalLine1Y > 0 && horizontalLine1Y < cameraPositionTexture_height)
                               line1 = true;
                           if (horizontalLine2Y > 0 && horizontalLine2Y < cameraPositionTexture_height)
                               line2 = true;
                           if (line1 && line2)
                           {
                               for (int x = newX - squareSize; x <= newX + squareSize; x++)
                               {
                                   if (x > 0 && x < cameraPositionTexture_width)
                                   {
                                       bitmap.SetPixel(x, horizontalLine1Y, squareColor);
                                       bitmap.SetPixel(x, horizontalLine2Y, squareColor);
                                   }
                               }
                           }
                           else if (line1)
                           {
                               for (int x = newX - squareSize; x < newX + squareSize; x++)
                               {
                                   if (x > 0 && x < cameraPositionTexture_width)
                                   {
                                       bitmap.SetPixel(x, horizontalLine1Y, squareColor);
                                   }
                               }
                           }
                           else
                           {
                               for (int x = newX - squareSize; x <= newX + squareSize; x++)
                               {
                                   if (x > 0 && x < cameraPositionTexture_width)
                                   {
                                       bitmap.SetPixel(x, horizontalLine2Y, squareColor);
                                   }
                               }
                           }

                           // draw vertical lines 1 and 2
                           line1 = false;
                           line2 = false;
                           if (verticalLine1X > 0 && verticalLine1X < cameraPositionTexture_width)
                               line1 = true;
                           if (verticalLine2X > 0 && verticalLine2X < cameraPositionTexture_width)
                               line2 = true;
                           if (line1 && line2)
                           {
                               for (int y = newY - squareSize; y < newY + squareSize; y++)
                               {
                                   if (y > 0 && y < cameraPositionTexture_height)
                                   {
                                       bitmap.SetPixel(verticalLine1X, y, squareColor);
                                       bitmap.SetPixel(verticalLine2X, y, squareColor);
                                   }
                               }
                           }
                           else if (line1)
                           {
                               for (int y = newY - squareSize; y < newY + squareSize; y++)
                               {
                                   if (y > 0 && y < cameraPositionTexture_height)
                                   {
                                       bitmap.SetPixel(verticalLine1X, y, squareColor);
                                   }
                               }
                           }
                           else
                           {
                               for (int y = newY - squareSize; y < newY + squareSize; y++)
                               {
                                   if (y > 0 && y < cameraPositionTexture_height)
                                   {
                                       bitmap.SetPixel(verticalLine2X, y, squareColor);
                                   }
                               }
                           }

                           // track position
                           oldCameraX = newX;
                           oldCameraY = newY;

                           // set image
                           pictureBoxCameraPosition.Image = bitmap;
                           isUpdateCameraThreadRunning = false;

                       }
                       catch (Exception e)
                       {
                           System.Diagnostics.Debug.Print(e.Message + " ");
                       }
                   }
                );
                      
                threadUpdateCamera.Start();
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message + " ");
            }
        }

        /// <summary>
        /// Generate a transparent image used as template for the camera position texture
        /// </summary>
        private void generateTransparentCameraPosTex()
        {
            int width = pictureBoxCameraPosition.Width;
            int height = pictureBoxCameraPosition.Height;
            cameraPositionTexture = new Bitmap(width, height);

            // size of the square displaying camera position
            squareSize = pictureBoxCameraPosition.Height / 20;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cameraPositionTexture.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                }
            }
        }

        public void ShowMinimap()
        {
            Show();
            IsShown = true;
            GenerateMinimap();
            generateTransparentCameraPosTex();
            UpdateCamera();
        }

        public void HideMinimap()
        {
            Hide();
            IsShown = false;
        }

        public void ResizeMinimap(int size)
        {
            this.Height = size;
            this.Width = size;
            this.pictureBoxCameraPosition.Width = size;
            this.pictureBoxCameraPosition.Height = size;
            this.pictureBoxMapTexture.Height = size;
            this.pictureBoxMapTexture.Width = size;
            GenerateMinimap();
            generateTransparentCameraPosTex();
            UpdateCamera();
        }
    }
}