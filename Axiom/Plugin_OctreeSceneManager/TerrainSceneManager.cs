using System;
using System.Collections;
using System.Data;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.MathLib;
using Axiom.Media;

namespace Axiom.SceneManagers.Octree {
	/// <summary>
	/// Summary description for TerrainSceneManager.
	/// </summary>
	public class TerrainSceneManager : OctreeSceneManager {

        #region Fields

        protected TerrainRenderable[,] tiles;
        protected int tileSize;
        protected int numTiles;
        protected Vector3 scale;
        protected Material terrainMaterial;
        protected SceneNode terrainRoot;

        #endregion Fields
        
        public TerrainSceneManager() {
		}

        #region SceneManager members

        public override void LoadWorldGeometry(string fileName) {
            TerrainOptions options = new TerrainOptions();

            DataSet optionData = new DataSet();
            optionData.ReadXml(fileName);
            DataTable table = optionData.Tables[0];
            DataRow row = table.Rows[0];

            string terrainFileName = "";
            string detailTexture = "";
            string worldTexture = "";

            if(table.Columns["Terrain"] != null) {
                terrainFileName = (string)row["Terrain"];
            }

            if(table.Columns["DetailTexture"] != null) {
                detailTexture = (string)row["DetailTexture"];
            }

            if(table.Columns["WorldTexture"] != null) {
                worldTexture = (string)row["WorldTexture"];
            }

            if(table.Columns["MaxMipMapLevel"] != null) {
                options.maxMipmap = Convert.ToInt32(row["MaxMipMapLevel"]);
            }

            if(table.Columns["DetailTile"] != null) {
                options.detailTile = Convert.ToInt32(row["DetailTile"]);
            }

            if(table.Columns["MaxPixelError"] != null) {
                options.maxPixelError = Convert.ToInt32(row["MaxPixelError"]);
            }

            if(table.Columns["TileSize"] != null) {
                options.size = Convert.ToInt32(row["TileSize"]);
            }

            if(table.Columns["ScaleX"] != null) {
                options.scalex = Convert.ToSingle(row["ScaleX"]);
            }

            if(table.Columns["ScaleY"] != null) {
                options.scaley = Convert.ToSingle(row["ScaleY"]);
            }

            if(table.Columns["ScaleZ"] != null) {
                options.scalez = Convert.ToSingle(row["ScaleZ"]);
            }

            if(table.Columns["VertexNormals"] != null) {
                options.isLit = ((string)row["VertexNormals"]) == "yes" ? true : false;
            }
            
            scale = new Vector3(options.scalex, options.scaley, options.scalez);
            tileSize = options.size;

            // load the heightmap
            Image image = Image.FromFile(terrainFileName);

            // TODO: Check terrain size for 2^n + 1

            // get the data from the heightmap
            options.data = image.Data;

            options.worldSize = image.Width;

            float maxx = options.scalex * options.worldSize;
            float maxy = 255 * options.scaley;
            float maxz = options.scalez * options.worldSize;

            Resize(new AxisAlignedBox(Vector3.Zero, new Vector3(maxx, maxy, maxz)));

            terrainMaterial = CreateMaterial("Terrain");

            if(worldTexture != "") {
                terrainMaterial.GetTechnique(0).GetPass(0).CreateTextureUnitState(worldTexture, 0);
            }

            if(detailTexture != "") {
                terrainMaterial.GetTechnique(0).GetPass(0).CreateTextureUnitState(detailTexture, 1);
            }

            terrainMaterial.Lighting = options.isLit;
            terrainMaterial.Load();

            terrainRoot = (SceneNode)RootSceneNode.CreateChild("TerrainRoot");

            numTiles = (options.worldSize - 1) / (options.size - 1);

            tiles = new TerrainRenderable[numTiles, numTiles]; 

            int p = 0, q = 0;

            for(int j = 0; j < options.worldSize - 1; j += (options.size - 1)) {
                p = 0;

                for(int i = 0; i < options.worldSize - 1; i += (options.size - 1)) {
                    options.startx = i;
                    options.startz = j;

                    string name = string.Format("Tile[{0},{1}]", p, q);

                    SceneNode node = (SceneNode)terrainRoot.CreateChild(name);
                    TerrainRenderable tile = new TerrainRenderable();
                    tile.Name = name;
                    
                    tile.SetMaterial(terrainMaterial);
                    tile.Init(options);

                    tiles[p,q] = tile;

                    node.AttachObject(tile);

                    p++;
                }

                q++;
            }

            int size1 = tiles.GetLength(0);
            int size2 = tiles.GetLength(1);

            for(int j = 0; j < size1; j++) {
                for(int i = 0; i < size2; i++) {
                    if(j != size1 - 1) {
                        ((TerrainRenderable)tiles[i,j]).SetNeighbor(Neighbor.South, (TerrainRenderable)tiles[i, j + 1]);
                        ((TerrainRenderable)tiles[i,j + 1]).SetNeighbor(Neighbor.North, (TerrainRenderable)tiles[i, j]);
                    }

                    if(i != size2 - 1) {
                        ((TerrainRenderable)tiles[i,j]).SetNeighbor(Neighbor.East, (TerrainRenderable)tiles[i + 1, j]);
                        ((TerrainRenderable)tiles[i + 1,j]).SetNeighbor(Neighbor.West, (TerrainRenderable)tiles[i, j]);
                    }
                }
            }

            if(options.isLit) {
                for(int j = 0; j < size1; j++) {
                    for(int i = 0; i < size2; i++) {
                        ((TerrainRenderable)tiles[i,j]).CalculateNormals();
                    }
                }
            }
        }

        /// <summary>
        ///     Updates all the TerrainRenderables LOD.
        /// </summary>
        /// <param name="camera"></param>
        protected override void UpdateSceneGraph(Camera camera) {
            base.UpdateSceneGraph (camera);
        }

        /// <summary>
        ///     Aligns TerrainRenderable neighbors, and renders them.
        /// </summary>
        protected override void RenderVisibleObjects() {

            for(int i = 0; i < tiles.GetLength(0); i++) {
                for(int j = 0; j < tiles.GetLength(1); j++) {
                    tiles[i, j].AlignNeighbors();
                }
            }

            base.RenderVisibleObjects ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public override void FindVisibleObjects(Camera camera) {
            base.FindVisibleObjects (camera);
        }

        /// <summary>
        ///     Returns the TerrainRenderable that contains the given pt.
        //      If no tile exists at the point, it returns 0
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public TerrainRenderable GetTerrainTile(Vector3 point) {
            return null;
        }

        #endregion SceneManager members
	}
}
