/*
 * Kod oparty o przyk³adowy kod z MSDN na licencji MS-PL
 */

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Drawing.Design;
using System.Drawing;

namespace SKraftPipeline
{
    /// <summary>
    /// Content Pipeline processor applies the SKraftShader.fx shader
    /// onto models, so they can be drawn using hardware instancing.
    /// </summary>
    [ContentProcessor(DisplayName = "SKraftPipeline")]
    public class SKraftPipeline : ModelProcessor
    {
        ContentIdentity rootIdentity;
        List<Vector3> vertices = new List<Vector3>();
        List<BoundingBox> boxs = new List<BoundingBox>();
        List<BoundingSphere> spheres = new List<BoundingSphere>();
        ContentProcessorContext context;

        /// <summary>
        /// Override the Process method to store the ContentIdentity of the model root node.
        /// </summary>
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            rootIdentity = input.Identity;
            string modelName = input.Identity.SourceFilename.Substring(input.Identity.SourceFilename.LastIndexOf("\\") + 1);
            this.context = context;
            Dictionary<string, object> ModelData = new Dictionary<string, object>();

            ModelContent baseModel = base.Process(input, context);
            GenerateData(input);
            ModelData.Add("BBox", boxs);
            ModelData.Add("BSphere", spheres);
            baseModel.Tag = ModelData;

            return baseModel;
        }

        private void GenerateData(NodeContent node)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                MeshHelper.OptimizeForCache(mesh);

                // Look up the absolute transform of the mesh.
                Matrix absoluteTransform = mesh.AbsoluteTransform;

                int i = 0;

                // Loop over all the pieces of geometry in the mesh.
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                    Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                    // Loop over all the indices in this piece of geometry.
                    // Every group of three indices represents one triangle.
                    List<Vector3> thisVerts = new List<Vector3>();
                    List<int> ind = new List<int>();

                    Vector3 vertex = Vector3.Zero;

                    foreach (int index in geometry.Indices)
                    {
                        // Look up the position of this vertex.
                        vertex = Vector3.Transform(geometry.Vertices.Positions[index], absoluteTransform);

                        // Store this data.
                        min = Vector3.Min(min, vertex);
                        max = Vector3.Max(max, vertex);

                        thisVerts.Add(vertex);

                        ind.Add(i++);
                    }

                    boxs.Add(new BoundingBox(min, max));
                    spheres.Add(BoundingSphere.CreateFromBoundingBox(boxs[boxs.Count - 1]));
                }
            }

            // Recursively scan over the children of this node.
            foreach (NodeContent child in node.Children)
            {
                GenerateData(child);
            }
        }

        /// <summary>
        /// Override the ConvertMaterial method to apply our custom InstancedModel.fx shader.
        /// </summary>
        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {            
            EffectMaterialContent newMaterial = new EffectMaterialContent();            
            newMaterial.Effect = new ExternalReference<EffectContent>("SKraftShader.fx", rootIdentity);            

            BasicMaterialContent basicMaterial = material as BasicMaterialContent;
            if ((basicMaterial != null) && (basicMaterial.Texture != null))
            {
                newMaterial.Textures.Add("Texture", basicMaterial.Texture);
            }
            
            return base.ConvertMaterial(newMaterial, context);
        }
    }
}
