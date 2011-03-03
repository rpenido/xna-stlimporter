/* Simples XNA Content Pipeline Importer for STL files
 * http://code.google.com/p/xna-stlimporter/
 * 
 * @author: Rômulo Penido | romulo DOT penido AT gmail DOT com
 * 
 * This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. */



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace Simples.Content.Pipeline.STLImporter
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// </summary>
    [ContentImporter(".stl", DisplayName = "STLImporter", DefaultProcessor = "ModelProcessor")]
    public class STLImporter : ContentImporter<NodeContent>
    {

        private MeshBuilder meshBuilder;

        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader sr = new BinaryReader(fs);
            
            NodeContent root = new NodeContent();
            sr.ReadBytes(80);
            int faceCount = sr.ReadInt32();
            /***/
            meshBuilder = MeshBuilder.StartMesh("");
            meshBuilder.SwapWindingOrder = true;
            /***/

            int normalsChannelIndex = meshBuilder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal(0));
            int[] verticesIndex = new int[faceCount * 3];
            Vector3[] normalList = new Vector3[faceCount];                
            for (int i = 0; i < faceCount; i++)
            {
                normalList[i] = new Vector3(sr.ReadSingle(), sr.ReadSingle(), sr.ReadSingle());
                
                for (int j = i*3; j < (i*3)+3; j++)
                {
                    
                    Vector3 vertice = new Vector3(sr.ReadSingle(),
                        sr.ReadSingle(), sr.ReadSingle());

                    verticesIndex[j] = meshBuilder.CreatePosition(vertice);                    
                }
                sr.ReadInt16();
            }
            for (int i = 0; i < faceCount; i++)
            {
                meshBuilder.SetVertexChannelData(normalsChannelIndex, normalList[i]);
                for (int j = i*3; j < (i*3)+3; j++)
                {
                    meshBuilder.AddTriangleVertex(verticesIndex[j]);
                }

            }

            MeshContent mesh = meshBuilder.FinishMesh();
            root.Children.Add(mesh);

           return root;
        }
    }
}
