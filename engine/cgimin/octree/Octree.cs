using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Engine.cgimin.helpers;
using Engine.cgimin.camera;

namespace Engine.cgimin.octree
{

    public class Octree
    {
        private static int drawCountStatistic;

        public static int MaxIterationDepth = 5;

        internal List<Octree> siblings;

        internal Vector3 bMin;
        internal Vector3 bMax;

        internal Vector3 mid;
        internal float midRadius;

        internal List<OctreeEntity> enteties;

        private int iteration;

        private static List<OctreeEntity> transparentEntity = new List<OctreeEntity>();
        private static List<OctreeEntity> postProcessingEntity = new List<OctreeEntity>();

        public Octree(Vector3 boundsMin, Vector3 boundsMax, int iterationDepth = 1)
        {
            iteration = iterationDepth;

            bMin = boundsMin;
            bMax = boundsMax;

            mid = (bMin + bMax) / 2;
            midRadius = (mid - bMax).Length;

            // Acht Siblings werden erstellt. null als Indikator, dass noch kein Objekt einsortiert wurde.
            siblings = new List<Octree>();
            for (int i = 0; i < 8; i++) siblings.Add(null);
        }


        public void AddEntity(OctreeEntity entity) {
            
            // Wenn auf der ersten iterations-Tiefe...
            if (iteration == 1) {
                if (enteties == null) enteties = new List<OctreeEntity>();
                // wird eine Referenz auf das Objekt gesichert
                enteties.Add(entity);
            }

            Vector3 pos = new Vector3(entity.Transform.M41, entity.Transform.M42, entity.Transform.M43);
            float radius = entity.Object3d.radius;
            
            // Test, ob sich das Objekt innerhalb der eigenen Bounding-Box befindet
            if (GeometryHelpers.SphereAARectangleIntersect(pos, radius, bMin, bMax)) {

                // Bei maximaler Iterationstiefe (unterster Ebene) wird die Objekt-Referenz angahängt  
                if (iteration == MaxIterationDepth)
                {
                    if (enteties == null) enteties = new List<OctreeEntity>();
                    enteties.Add(entity);
                }
                else
                {
                // Wenn nicht wird ermittelt, in welchem Kind-Sibling sich das Objekt befindet 
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 dif = (bMax - bMin) / 2;
                        Vector3 bMinSub = bMin;
                        Vector3 bMaxSub = (bMin + bMax) / 2;

                        // Bounding Min / Max, je nach "i" - Wert
                        if (i % 2 == 1) { bMinSub.X += dif.X; bMaxSub.X += dif.X; }
                        if ((i / 2) % 2 == 1) { bMinSub.Y += dif.Y; bMaxSub.Y += dif.Y; }
                        if (i >= 4) { bMinSub.Z += dif.Z; bMaxSub.Z += dif.Z; }

                        // Wenn die Bounding-Box eines Sibling geschnitten wird... 
                        if (GeometryHelpers.SphereAARectangleIntersect(pos, radius, bMinSub, bMaxSub))
                        {
                            if (siblings[i] == null) siblings[i] = new Octree(bMinSub, bMaxSub, iteration + 1);
                            // ... dort einsortieren!
                            siblings[i].AddEntity(entity);
                        }
                    }
                }
            } 
        }


        public void Draw()
        {
            if (iteration == 1)
            {
                // Auf der ersten iterations-Ebene wird zunächst für alle Objekte die Eigenschaft "drawn" auf false gestellt.
                drawCountStatistic = 0;
                int len = enteties.Count;
                for (int i = 0; i < len; i++) enteties[i].drawn = false;
                transparentEntity.Clear();
                postProcessingEntity.Clear();
            }

            if (iteration == MaxIterationDepth)
            {
                // Bei maximaler Iterationstiefe wird gezeichnet...
                int len = enteties.Count;
                for (int i = 0; i < len; i++)
                {
                    // insofern das Objekt nicht schon gezeichnet wurde. Dies ist evtl. der Fall wenn Objekt auch ein Nachbar-Sibling schneidet, 
                    // also auch dort referenziert ist... 
                    if (enteties[i].drawn == false)
                    {
                        if (!enteties[i].MaterialSetting.transparent && !enteties[i].MaterialSetting.postProcessing)
                        {
                            enteties[i].Object3d.Transformation = enteties[i].Transform;
                            enteties[i].Material.DrawWithSettings(enteties[i].Object3d, enteties[i].MaterialSetting);
                            enteties[i].drawn = true;
                            drawCountStatistic++;
                        } else
                        {
                            if (enteties[i].MaterialSetting.transparent) transparentEntity.Add(enteties[i]);
                            if (enteties[i].MaterialSetting.postProcessing) postProcessingEntity.Add(enteties[i]);
                        }
                    }
                }
            }
            else
            {
                // Wenn wir noch nicht auf der untersten Ebene angekommen sind, dann rekursiver Aufruf aller 8
                // Kinder Siblings.
                // Ist der Sibling null oder außerhalb der Kamera, kann er komplett ausgespart werden.   
                for (int i = 0; i < 8; i++)
                {
                    if (siblings[i] != null && Camera.SphereIsInFrustum(siblings[i].mid, siblings[i].midRadius))
                    {
                        siblings[i].Draw();
                    }
                }
            }

            if (iteration == 1)
            {
                transparentEntity = transparentEntity.OrderByDescending(obj => (Camera.Position - obj.Transform.ExtractTranslation()).LengthSquared).ToList();
                for (int i = 0; i < transparentEntity.Count; i++)
                {
                    transparentEntity[i].Object3d.Transformation = transparentEntity[i].Transform;
                    transparentEntity[i].Material.DrawWithSettings(transparentEntity[i].Object3d, transparentEntity[i].MaterialSetting);
                    transparentEntity[i].drawn = true;
                    drawCountStatistic++;
                }
            }
        }


        public void DrawPostProcessingEnteties()
        {
            for (int i = 0; i < postProcessingEntity.Count; i++)
            {
                postProcessingEntity[i].Object3d.Transformation = postProcessingEntity[i].Transform;
                postProcessingEntity[i].Material.DrawWithSettings(postProcessingEntity[i].Object3d, postProcessingEntity[i].MaterialSetting);
                postProcessingEntity[i].drawn = true;
                drawCountStatistic++;
            }

        }


    }
}
