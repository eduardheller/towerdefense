using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.camera;

namespace Engine.cgimin.object3d
{

    public class BaseObject3D
    {

        // Die Transformation (Position, Rotation, Skalierung) des Objekts
        public Matrix4 Transformation = Matrix4.Identity;

        // Listen, die mit den 3D-Daten des Objekts befüllt werden
		public List<Vector3> Positions = new List<Vector3>();
		public List<Vector3> Normals = new List<Vector3>();
		public List<Vector2> UVs = new List<Vector2>();
		public List<Vector3> Tangents = new List<Vector3>();
		public List<Vector3> BiTangents = new List<Vector3>();

        // Die Index-Liste
		public List<int> Indices = new List<int>();

        // Vertex-Array-Object "VAO"
        public int Vao;

        private int allBufferVBO;
        private int indexBuffer;

        // Object Radius
        public float radius;

        public int IndexBuffer
        {
            get
            {
                return indexBuffer;
            }

            set
            {
                indexBuffer = value;
            }
        }


        // Generiert das Vertex-Array-Objekt
        public void CreateVAO()
        {
            // Liste mit dem kompletten Datensatz
            List<float> allData = new List<float>();

            // "Interleaved", d.h. pro Vertex sind die Daten für die jeweilige Position, Normalen und UV-Koordinaten direkt hintereinander angelegt
            // In der Schleife wird auch der Objekt-Radius ermittelt
            float sqrRadiusMax = 0.0f;
            for (int i = 0; i < Positions.Count; i++) {

                float sqrRadius = Positions[i].LengthSquared;
                if (sqrRadius > sqrRadiusMax) sqrRadiusMax = sqrRadius;

                allData.Add(Positions[i].X);
                allData.Add(Positions[i].Y);
                allData.Add(Positions[i].Z);

                allData.Add(Normals[i].X);
                allData.Add(Normals[i].Y);
                allData.Add(Normals[i].Z);

                allData.Add(UVs[i].X);
                allData.Add(UVs[i].Y);

                allData.Add(Tangents[i].X);
                allData.Add(Tangents[i].Y);
                allData.Add(Tangents[i].Z);

                allData.Add(BiTangents[i].X);
                allData.Add(BiTangents[i].Y);
                allData.Add(BiTangents[i].Z);
            }


            // radius wird aus dem squqred radius berechnet
            radius = (float)Math.Sqrt(sqrRadiusMax);

            // Generierung des VBO für die "Interleaved" Daten
            GL.GenBuffers(1, out allBufferVBO);

            // Buffer wird "gebunden", folgende OpenGL-Befehle beziehen sich auf diesen Buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, allBufferVBO);

            // Daten werden in das VBO hochgeladen, in diesem Schritt werden die Daten in den Grafikspeicher geschoben
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(allData.Count  * sizeof(float)), allData.ToArray(), BufferUsageHint.StaticDraw);

            // BindBuffer auf 0, dmait sich folgende Buffer-Operationen nicht den VBO überschreiben (Statemachine)
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            // Generierung des Index Buffer
            GL.GenBuffers(1, out indexBuffer);

            // Buffer wird "gebunden", folgende OpenGL-Befehle beziehen sich auf diesen Buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);

            // Index-Daten werden in den Element-Buffer geschoben, ebenfalls in den Grafikspeicher
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(uint) * Indices.Count), Indices.ToArray(), BufferUsageHint.StaticDraw);

            // BindBuffer auf 0, dmait sich folgende Operationen nicht den Element-Buffer überschreiben (Statemachine) 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            

            // Generierung des Vertex-Array-Objects
            GL.GenVertexArrays(1, out Vao);

            // Vertex-Array-Objekt wird "gebunden", folgende Befehle beziehen sich auf unseren VAO. Wichtig für die folgenden beiden Aufrufe von "BindBuffer"
            GL.BindVertexArray(Vao);

            // BindBuffer Befehle: Werden vom VAO "aufgenommen".
            // Zuerst unser oben generierter Element-Buffer...
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);

            // ... Dann unser interleaved VBO.
            GL.BindBuffer(BufferTarget.ArrayBuffer, allBufferVBO);

            // Es folgen fünf Aufrufe von GL.VertexAttribPointer, diese mussen erst "enabled" werden
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);

            // Die Beschreibung unserer "interleaved" Datenstruktur. Damit der Shader später mit der Datenstruktur umgehen kann
            // Die Zuordnung zum "Index", dem erste Parameter, wird dann auch vom Shader berücksichtigt.

            // An Index-Stelle 0 (also als erstes) kommen die Positionsdaten. Der letzte Parameter gibt an, an welcher Bytestelle im Vertx-Datenblock sich die Daten befinden 
            int strideSize = Vector3.SizeInBytes * 4 + Vector2.SizeInBytes;

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, strideSize, 0);

            // An Index-Stelle 1 folgen die Normalen. Der letzte Parameter gibt an, dass sie nach den Positionsdaten folgen also an Bytestelle der Größe eines Vector3
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, strideSize, Vector3.SizeInBytes);

            // An Index-Stelle 2 folgen die Normalen. Der letzte Parameter gibt an, dass sie nach den Positionsdaten und den Normalendaten folgen also an Bytestelle der Größe eines Vector3 * 2
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, strideSize, Vector3.SizeInBytes * 2);

            // An Index-Stelle 3 folgen die Tangenten.
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, true, strideSize, Vector3.SizeInBytes * 2 + Vector2.SizeInBytes);

            // An Index-Stelle 4 folgen die BiTangenten.
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, true, strideSize, Vector3.SizeInBytes * 3 + Vector2.SizeInBytes);

            // BindVertexArray auf 0, damit folgende Operationen nicht unseren Vao überschreiben (Statemachine)
            GL.BindVertexArray(0);


            // Hinweis: Das generierte VAO gibt eine Datenstruktur vor, die auch vom Shader berücksichtigt werden muss bezüglich der per GL.VertexAttribPointer definierten Index-Stellen.
            // Das Datenformat Position an Stelle 0, Normale an Stelle 1, und UV an Stelle 2 sollte also in dieser Form von unseren Shadern benutzt werden.

        }


        // Fügt ein Dreieck unserem Mesh hinzu, muss aufgerufen werden, bevor die finale Datenstruktur mit CreateVAO() in den Grafikspeicher geschrieben wird.
        public void addTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 n1, Vector3 n2, Vector3 n3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            int index = Positions.Count;

            Positions.Add(v1);
            Positions.Add(v2);
            Positions.Add(v3);

            Normals.Add(n1);
            Normals.Add(n2);
            Normals.Add(n3);

            UVs.Add(uv1);
            UVs.Add(uv2);
            UVs.Add(uv3);


            // Tangenten / BiTangenten berechnen
            Vector3 deltaPos1 = v2 - v1;
            Vector3 deltaPos2 = v3 - v1;

            Vector2 deltaUV1 = uv2 - uv1;
            Vector2 deltaUV2 = uv3 - uv1;

            float f;
            if (Math.Abs(deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y) < 0.0001f)
            {
                f = 1.0f;
            }
            else
            {
                f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);
            }

            Vector3 tangent = new Vector3(f * (deltaUV2.Y * deltaPos1.X - deltaUV1.Y * deltaPos2.X),
                                          f * (deltaUV2.Y * deltaPos1.Y - deltaUV1.Y * deltaPos2.Y),
                                          f * (deltaUV2.Y * deltaPos1.Z - deltaUV1.Y * deltaPos2.Z));
            tangent.Normalize();

            tangent = (tangent - n1 * Vector3.Dot(n1, tangent)).Normalized();
            Vector3 biTangent = new Vector3(f * (-deltaUV2.X * deltaPos1.X + deltaUV1.X * deltaPos2.X),
                                            f * (-deltaUV2.X * deltaPos1.Y + deltaUV1.X * deltaPos2.Y),
                                            f * (-deltaUV2.X * deltaPos1.Z + deltaUV1.X * deltaPos2.Z));
            biTangent.Normalize();


            if (Vector3.Dot(Vector3.Cross(n1, tangent), biTangent) < 0.0f)
            {
                tangent = tangent * -1.0f;
            }

            Tangents.Add(tangent);
            Tangents.Add(tangent);
            Tangents.Add(tangent);

            BiTangents.Add(biTangent);
            BiTangents.Add(biTangent);
            BiTangents.Add(biTangent);

            Indices.Add(index);
            Indices.Add(index + 2);
            Indices.Add(index + 1);
        }

        public void addTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).Normalized();
            addTriangle(v1, v2, v3, normal, normal, normal, uv1, uv2, uv3);
        }

        public void setListData(List<Vector3> positions,
                                List<int> indices,
                                List<Vector3> normals,
                                List<Vector2> uvs,
                                List<Vector3> tangents,
                                List<Vector3> biTangents)
        {
            Positions = positions;
            Indices = indices;
            Normals = normals;
            UVs = uvs;
            Tangents = tangents;
            BiTangents = biTangents;
        }


        public void MidPointToCenter()
        {
            int len = Positions.Count;

            Vector3 average = new Vector3(0, 0, 0);
            for (int i = 0; i < len; i++)
            {
                average += Positions[i];
            }
            average.X = average.X / len;
            average.Y = average.Y / len;
            average.Z = average.Z / len;
           
            Transformation *= Matrix4.CreateTranslation(average.X, average.Y, average.Z);

            for (int i = 0; i < len; i++)
            {
                Positions[i] -= average;
            }

        }


        public void averageTangents()
        {
            int len = Positions.Count;

            for (int i = 0; i < len - 1; i++)
            {
                for (int o = i + 1; o < len; o++) {

                    if (Positions[i] == Positions[o] && Normals[i] == Normals[o] && UVs[i] == UVs[o])
                    {
                        Vector3 tanI = Tangents[i];
                        Tangents[i] += Tangents[o];
                        Tangents[o] += tanI;

                        Vector3 biTanI = BiTangents[i];
                        BiTangents[i] += BiTangents[o];
                        BiTangents[o] += biTanI;
                    }
                }
            }
        }


        public bool IsInView() {

            Vector3 pos = new Vector3(Transformation.M41, 
                                      Transformation.M42, 
                                      Transformation.M43);

            return Camera.SphereIsInFrustum(pos, radius);
        }

        // Gibt den Grafikspeicher wieder frei
        public virtual void UnLoad()
        {
            if (GL.IsBuffer(allBufferVBO)) GL.DeleteBuffer(allBufferVBO);
            if (GL.IsBuffer(IndexBuffer)) GL.DeleteBuffer(IndexBuffer);
            if (GL.IsVertexArray(Vao)) GL.DeleteVertexArray(Vao);
        }
    }
}
