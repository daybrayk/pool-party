using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
    public static class PhysicsUtil
    {
        #region Constants
        public const float GravitationConstant = 6.674E-11f;
        public const float CoulombsConstant = 8.987E9f;
        #endregion

        #region Kinematic Equations
        public static Vector3 KinematicSqrFinalVelocity(Vector3 vi, Vector3 a, float d)
        {
            float vfx = Mathf.Pow(vi.x, 2) + (2 * a.x * d);
            float vfy = Mathf.Pow(vi.y, 2) + (2 * a.y * d);
            float vfz = Mathf.Pow(vi.z, 2) + (2 * a.z * d);
            
            return new Vector3(vfx, vfy, vfz);
        }
        
        public static Vector3 KinematicFinalVelocity(Vector3 vi, Vector3 a, float t)
        {
            float vfx = vi.x + (a.x * t);
            float vfy = vi.y + (a.y * t);
            float vfz = vi.z + (a.z * t);
            
            return new Vector3(vfx, vfy, vfz);
        }
        
        public static float KinematicDistance(Vector3 vi, Vector3 vf, float t)
        {
            float dx = (vi.x + vf.x) / 2 * t;
            float dy = (vi.y + vf.y) / 2 * t;
            float dz = (vi.z + vf.z) / 2 * t;

            return new Vector3(dx, dy, dz).magnitude;
        }

        public static float KinematicDistanceWithAcc(Vector3 vi, Vector3 a, float t)
        {
            float dx = vi.x * t + 0.5f * (a.x * Mathf.Pow(t, 2));
            float dy = vi.y + t * 0.5f * (a.x * Mathf.Pow(t, 2));
            float dz = vi.y + t * 0.5f * (a.z * Mathf.Pow(t, 2));

            return new Vector3(dx, dy, dz).magnitude;
        }
        #endregion

        #region Forces
        public static Vector3 ForceUniversalGravitation(float m1, float m2, Vector3 r)
        {

            float fx = GravitationConstant * ((m1 * m2 ) / Mathf.Pow(r.x, 2));
            float fy = GravitationConstant * ((m1 * m2) / Mathf.Pow(r.y, 2));
            float fz = GravitationConstant * ((m1 * m2) / Mathf.Pow(r.z, 2));

            return new Vector3(fx, fy, fz);
        }

        public static Vector3 ForceCoulombsLaw(float q1, float q2, Vector3 r)
        {
            float fx = CoulombsConstant * ((q1 * q2 / Mathf.Pow(r.x, 2)));
            float fy = CoulombsConstant * ((q1 * q2 / Mathf.Pow(r.y, 2)));
            float fz = CoulombsConstant * ((q1 * q2 / Mathf.Pow(r.z, 2)));

            return new Vector3(fx, fy, fz);
        }

        public static Vector3 ForceHookesLaw(float k, Vector3 x)
        {
            float fx = k * x.x;
            float fy = k * x.y;
            float fz = k * x.z;

            return new Vector3(fx, fy, fz);
        }

        public static Vector3 SumOfForces(params Vector3[] forces)
        {
            Vector3 sum = Vector3.zero;
            for(int i = 0; i < forces.Length; i++)
            {
                sum += forces[i];
            }

            return sum;
        }
        #endregion

        #region Energy
        public static float EnergyKinetic(float m, float v)
        {
            return 0.5f * m * (v * v);
        }

        public static float EnergyKinetic(float m, Vector3 v)
        {
            return EnergyKinetic(m, v.magnitude);
        }

        public static float EnergyGravPotential(float m, float g)
        {
            return m * g;
        }

        public static float EnergyGravPotential(float m, Vector3 g)
        {
            return EnergyGravPotential(m, g.magnitude);
        }

        public static float EnergyElasticPotential(float k, float x)
        {
            return 0.5f * k * (x * x);
        }

        public static float EnergyElasticPotential(float k, Vector3 offset)
        {
            return EnergyElasticPotential(k, offset.magnitude);
        }
        #endregion
    }
}
