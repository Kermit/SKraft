/*
 * Kod oparty o przyk³adowy kod z MSDN na licencji MS-PL
 */

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

        /// <summary>
        /// Override the Process method to store the ContentIdentity of the model root node.
        /// </summary>
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            rootIdentity = input.Identity;

            ModelContent model = base.Process(input, context);
            return model;
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
