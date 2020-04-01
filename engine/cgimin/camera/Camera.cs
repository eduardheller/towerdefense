using System;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using Engine.cgimin.helpers;

namespace Engine.cgimin.camera
{
    public class Camera
    {
        // Enumeration für die 6 Clipping-Ebenen
        public enum planeEnum : int
        {
            NEAR_PLANE = 0,
            FAR_PLANE = 1,
            LEFT_PLANE = 2,
            RIGHT_PLANE = 3,
            TOP_PLANE = 4,
            BOTTOM_PLANE = 5
        };

        // Struct für die Ebene
        public struct Plane
        {
            public float d;
            public Vector3 normal;
        }

        // frustum Clipping-Ebenen
        private static List<Plane> planes;

        // Matrizen für die Kamera Transformation (Position / Rotation) ...
        private static Matrix4 view;

        // ... und für die perspektivische Projektion
        private static Matrix4 projection;

        // Position der Kamera wird extra zwischengespeichert
        public static Vector3 position;
        private static Vector3 target;
        private static Vector3 positionRestrictionPos = Vector3.Zero;
        private static Vector3 positionRestrictionNeg = Vector3.Zero;
        private static bool lerpOrientation;

        // für das steuern der fly-camera
        public static Vector3 orientation;
        private static Vector3 targetOrientation;

        // behalten der Kamera-Werte, wenn die Kamera aufgrund vom Shadow-Mapping "umgebogen" wird
        private static int savedScreenWidth;
        private static int savedScreenHeight;
        private static float savedFov;
        private static float savedNear;
        private static float savedFar;
        private static Matrix4 savedProjection;
        private static Matrix4 savedView;
        public static bool useOtherView = false;
        public static float lerpStrenght; 

        public static float Fov { get { return savedFov; } }



        public static void Init(float pLerpStrenght = 5f)
        {
            Planes = new List<Plane>();
            for (int i = 0; i < 6; i++) Planes.Add(new Plane());
            lerpStrenght = pLerpStrenght;
            projection = Matrix4.Identity;
            view = Matrix4.Identity;
            CamPosition = Vector3.Zero;
            orientation = new Vector3((float)Math.PI, 0f, 0f);
            Target = CamPosition;
            targetOrientation = orientation;
            LerpOrientation = false;
        }

        public static void InitPositionRestriction(Vector3 _positionRestrictionPos, Vector3 _positionRestrictionNeg)
        {
            positionRestrictionPos = _positionRestrictionPos;
            positionRestrictionNeg = _positionRestrictionNeg;
        }


        // width, height = Größe des Screens in Pixeln, fov = "Field of View", der Öffnungswinkel der Kameralinse
        public static void SetWidthHeightFov(int width, int height, float fov, float near = 1, float far = 1000)
        {
            savedScreenWidth = width;
            savedScreenHeight = height;
            savedFov = fov;
            savedNear = near;
            savedFar = far;
            projection = Matrix4.Identity;
            float aspectRatio = width / (float)height;
            Matrix4.CreatePerspectiveFieldOfView((float)(fov * Math.PI / 180.0f), aspectRatio, near, far, out projection);

            savedProjection = projection;
            savedView = Transformation;

            CreateViewFrustumPlanes(Transformation * projection);
        }


        // zurücksetzen vom view-port und den die gespeicherten Kamera-Werte
        public static void SetBackToLastCameraSettings()
        {
            view = savedView;
            projection = savedProjection;
            GL.Viewport(0, 0, savedScreenWidth, savedScreenHeight);
            SetWidthHeightFov(savedScreenWidth, savedScreenHeight, savedFov, savedNear, savedFar);

        }

        // direktes setzen der Projektionsmatrix für das Shadow-Mapping
        public static void SetProjectionMatrix(Matrix4 pProjection)
        {
            projection = pProjection;
        }

        // direktes setzen der Transformationsmatrix für das Shadow-Mapping
        public static void SetTransformMatrix(Matrix4 transform)
        {
            view = transform;
        }


        public static void Move(float speed, float x, float y, float z)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin((float)orientation.X), 0, (float)Math.Cos((float)orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, speed);

            Target += offset;

            CreateViewFrustumPlanes(Transformation * projection);
        }

        public static void SetToPosition(Vector3 pos)
        {
            CamPosition = pos;
            Target = pos;
        }

        public static void LerpToPosition(Vector3 pos)
        {
            Target = pos;
        }

        public static void MoveForward(float speed)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin((float)orientation.X), 0, (float)Math.Cos((float)orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset.X = (float)(Math.Sin((float)orientation.X) * Math.Cos((float)orientation.Y));
            offset.Y = (float)Math.Sin((float)orientation.Y);
            offset.Z = (float)(Math.Cos((float)orientation.X) * Math.Cos((float)orientation.Y));

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, speed);

            Target += offset;

            CreateViewFrustumPlanes(Transformation * projection);
        }

        public static void AddRotation(float speed, float x, float y)
        {
            x = x * speed;
            y = y * speed;

            targetOrientation.X = (orientation.X + x) % ((float)Math.PI * 2.0f);
            targetOrientation.Y = Math.Max(Math.Min(orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);

            CreateViewFrustumPlanes(Transformation * projection);
        }



        public static void CreateViewFrustumPlanes(Matrix4 mat)
        {
            // left
            Plane plane = new Plane();
            plane.normal.X = mat.M14 + mat.M11;
            plane.normal.Y = mat.M24 + mat.M21;
            plane.normal.Z = mat.M34 + mat.M31;
            plane.d = mat.M44 + mat.M41;
            Planes[(int)planeEnum.LEFT_PLANE] = plane;

            // right
            plane = new Plane();
            plane.normal.X = mat.M14 - mat.M11;
            plane.normal.Y = mat.M24 - mat.M21;
            plane.normal.Z = mat.M34 - mat.M31;
            plane.d = mat.M44 - mat.M41;
            Planes[(int)planeEnum.RIGHT_PLANE] = plane;

            // bottom
            plane = new Plane();
            plane.normal.X = mat.M14 + mat.M12;
            plane.normal.Y = mat.M24 + mat.M22;
            plane.normal.Z = mat.M34 + mat.M32;
            plane.d = mat.M44 + mat.M42;
            Planes[(int)planeEnum.BOTTOM_PLANE] = plane;

            // top
            plane = new Plane();
            plane.normal.X = mat.M14 - mat.M12;
            plane.normal.Y = mat.M24 - mat.M22;
            plane.normal.Z = mat.M34 - mat.M32;
            plane.d = mat.M44 - mat.M42;
            Planes[(int)planeEnum.TOP_PLANE] = plane;

            // near
            plane = new Plane();
            plane.normal.X = mat.M14 + mat.M13;
            plane.normal.Y = mat.M24 + mat.M23;
            plane.normal.Z = mat.M34 + mat.M33;
            plane.d = mat.M44 + mat.M43;
            Planes[(int)planeEnum.NEAR_PLANE] = plane;

            // far
            plane = new Plane();
            plane.normal.X = mat.M14 - mat.M13;
            plane.normal.Y = mat.M24 - mat.M23;
            plane.normal.Z = mat.M34 - mat.M33;
            plane.d = mat.M44 - mat.M43;
            Planes[(int)planeEnum.FAR_PLANE] = plane;

            // Normalisieren
            for(int i = 0; i< 6; i++)
            {                
                plane = Planes[i];

                float length = plane.normal.Length;
                plane.normal.X = plane.normal.X / length;
                plane.normal.Y = plane.normal.Y / length;
                plane.normal.Z = plane.normal.Z / length;
                plane.d = plane.d / length;

                Planes[i] = plane;
            }
        }


        private static float SignedDistanceToPoint(int planeID, Vector3 pt)
        {
            return Vector3.Dot(Planes[planeID].normal, pt) + Planes[planeID].d;
        }


        public static bool SphereIsInFrustum(Vector3 center, float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                if (SignedDistanceToPoint(i, center) < -radius)
                {
                    return false;
                }
            }
            return true;
        }

		public static void SetupFog(float fogStart, float fogEnd, Vector3 fogColor)
		{
			FogStart = fogStart;
			FogEnd = fogEnd;
			FogColor = fogColor;
		}

		public static float FogStart {
			get;
			set;
		}

		public static float FogEnd {
			get;
			set;
		}

		public static Vector3 FogColor {
			get;
			set;
		}

        // Getter
        public static Vector3 Position
        {
            get { return CamPosition; }
        }

        public static Matrix4 View
        {
            get { return view; }
            set { view = value; }
        }

        public static Vector3 GetLookAt()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)orientation.X) * Math.Cos((float)orientation.Y));
            lookat.Y = (float)Math.Sin((float)orientation.Y);
            lookat.Z = (float)(Math.Cos((float)orientation.X) * Math.Cos((float)orientation.Y));
            return lookat;
        }

        public static void SetOrientation(Vector3 look)
        {
            targetOrientation = look;
        }

        public static Matrix4 Transformation
        {
            get {
                if (!useOtherView)
                {
                    Vector3 lookat = new Vector3();

                    lookat.X = (float)(Math.Sin((float)orientation.X) * Math.Cos((float)orientation.Y));
                    lookat.Y = (float)Math.Sin((float)orientation.Y);
                    lookat.Z = (float)(Math.Cos((float)orientation.X) * Math.Cos((float)orientation.Y));

                    view = Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
                }        
                return view;
            }
        }

        public static void Update(FrameEventArgs e)
        {

            if(lerpOrientation) orientation = Vector3.Lerp(orientation, targetOrientation, (float)e.Time * lerpStrenght);
            else orientation = targetOrientation;
            CamPosition = Vector3.Lerp(CamPosition, Target, (float)e.Time * lerpStrenght);
        }


        public static Matrix4 PerspectiveProjection
        {
            get { return projection; }
        }

        public static Vector3 CamPosition
        {
            get
            {
                return position;
            }

            set
            {
                if (positionRestrictionPos != Vector3.Zero || positionRestrictionNeg != Vector3.Zero)
                {
                    float camPosX = Clamp(value.X, positionRestrictionNeg.X, positionRestrictionPos.X);
                    float camPosY = Clamp(value.Y, positionRestrictionNeg.Y, positionRestrictionPos.Y);
                    float camPosZ = Clamp(value.Z, positionRestrictionNeg.Z, positionRestrictionPos.Z);

                    float targetX = Clamp(Target.X, positionRestrictionNeg.X, positionRestrictionPos.X);
                    float targetY = Clamp(Target.Y, positionRestrictionNeg.Y, positionRestrictionPos.Y);
                    float targetZ = Clamp(Target.Z, positionRestrictionNeg.Z, positionRestrictionPos.Z);

                    position = new Vector3(camPosX, camPosY, camPosZ);
                    Target = new Vector3(targetX, targetY, targetZ);

                }
                else
                {
                    position = value;
                }
            }
        }

        public static List<Plane> Planes
        {
            get
            {
                return planes;
            }

            private set
            {
                planes = value;
            }
        }

        public static Vector3 Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        public static bool LerpOrientation
        {
            get
            {
                return lerpOrientation;
            }

            set
            {
                lerpOrientation = value;
            }
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
