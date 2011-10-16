using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace SKraft.Cubes
{
    class PrimitiveCube
    {
        VertexPositionColor[] vertex = new VertexPositionColor[5];
        short[] indices = new short[6];

        public PrimitiveCube(GraphicsDeviceManager graphics)
        {
            vertex[0].Position = new Vector3(0f, 0f, 0f);
            vertex[0].Color = Color.White;
            vertex[1].Position = new Vector3(5f, 0f, 0f);
            vertex[1].Color = Color.White;
            vertex[2].Position = new Vector3(10f, 0f, 0f);
            vertex[2].Color = Color.White;
            vertex[3].Position = new Vector3(5f, 0f, -5f);
            vertex[3].Color = Color.White;
            vertex[4].Position = new Vector3(10f, 0f, -5f);
            vertex[4].Color = Color.White;


            indices[0] = 3;
            indices[1] = 1;
            indices[2] = 0;
            indices[3] = 4;
            indices[4] = 2;
            indices[5] = 1;

            
        }

        public void Draw(GraphicsDeviceManager graphics)
        {
            VertexBuffer vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), vertex.Length, BufferUsage.None);

            vertexBuffer.SetData<VertexPositionColor>(vertex);

            IndexBuffer lineListIndexBuffer = new IndexBuffer(
                graphics.GraphicsDevice,
                IndexElementSize.SixteenBits,
                sizeof(short) * indices.Length,
                BufferUsage.None);

            lineListIndexBuffer.SetData<short>(indices);
            BasicEffect ef = new BasicEffect(graphics.GraphicsDevice);

            foreach (EffectPass pass in )
            {
                pass.Begin();

                ZmPocz.kartaGraficzna.VertexDeclaration = deklaracjaWierzcholkow;
                //ZmPocz.kartaGraficzna.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, wierzcholki,
                //    0, wierzcholki.Length, indeksy, 0, indeksy.Length / 3);

                ZmPocz.kartaGraficzna.Vertices[0].SetSource(BuforWierzcholkow, 0, VertexPositionNormalColored.SizeInBytes);

                ZmPocz.kartaGraficzna.DrawUserPrimitives(PrimitiveType.TriangleList, wierzcholki, 0, wierzcholki.Length / 3);
                pass.End();
            }
            graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertex.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
        }
    }
}
