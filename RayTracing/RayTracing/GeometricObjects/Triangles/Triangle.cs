﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.GeometricObjects.Base;
using RayTracing.Util;
using RayTracing.Util.Math;

namespace RayTracing.GeometricObjects.Triangles
{
    class Triangle : GeometricObject
    {
        private Vec3 v0, v1, v2;
        private Vec3 normal;
        private static double kEpsilin = 0.0001;

        public override BBox BoundingBox
        {
            get
            {
                double delta = 0.000001;
                double x0 = Math.Min(Math.Min(v0.X, v1.X), v2.X) - delta;
                double x1 = Math.Max(Math.Max(v0.X, v1.X), v2.X) + delta;
                double y0 = Math.Min(Math.Min(v0.Y, v1.Y), v2.Y) - delta;
                double y1 = Math.Max(Math.Max(v0.Y, v1.Y), v2.Y) + delta;
                double z0 = Math.Min(Math.Min(v0.Z, v1.Z), v2.Z) - delta;
                double z1 = Math.Max(Math.Max(v0.Z, v1.Z), v2.Z) + delta;

                return new BBox(x0, x1, y0, y1, z0, z1);
            }
        }

        public Triangle() :
            base()
        {
            v0 = new Vec3(0, 0, 0);
            v1 = new Vec3(0, 0, 1);
            v2 = new Vec3(1, 0, 0);
            normal = new Vec3(0, 1, 0);
        }

        public Triangle(Vec3 v0, Vec3 v1, Vec3 v2) :
            base()
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            normal = Vec3.Cross((v1 - v0), (v2 - v0));
            normal.Normalize();
        }

        public Triangle(Triangle other) :
            base(other)
        {
            v0 = other.v0.Clone();
            v1 = other.v1.Clone();
            v2 = other.v2.Clone();
            normal = other.normal.Clone();
        }

        public override GeometricObject Clone()
        {
            return new Triangle(this);
        }
        
        public override bool Hit(Ray ray, ref double tmin, ShadeRec sr)
        {
            double a = v0.X - v1.X, b = v0.X - v2.X, c = ray.D.X, d = v0.X - ray.O.X;
            double e = v0.Y - v1.Y, f = v0.Y - v2.Y, g = ray.D.Y, h = v0.Y - ray.O.Y;
            double i = v0.Z - v1.Z, j = v0.Z - v2.Z, k = ray.D.Z, l = v0.Z - ray.O.Z;

            double m = f * k - g * j, n = h * k - g * l, p = f * l - h * j;
            double q = g * i - e * k, s = e * j - f * i;

            double invDenom = 1.0 / (a * m + b * q + c * s);

            double e1 = d * m - b * n - c * p;
            double beta = e1 * invDenom;

            if (beta < 0.0)
                return false;

            double r = e * l - h * i;
            double e2 = a * n + d * q + c * r;
            double gamma = e2 * invDenom;

            if (gamma < 0.0)
                return false;

            if (beta + gamma > 1.0)
                return false;

            double e3 = a * p - b * r + d * s;
            double t = e3 * invDenom;

            if (t < kEpsilin)
                return false;

            tmin = t;
            sr.Normal = normal;
            sr.LocalHitPoint = ray.HitPoint(t);

            return true;
        }

        public override bool ShadowHit(Ray ray, ref double tmin)
        {
            if (!shadows)
                return false;

            double a = v0.X - v1.X, b = v0.X - v2.X, c = ray.D.X, d = v0.X - ray.O.X;
            double e = v0.Y - v1.Y, f = v0.Y - v2.Y, g = ray.D.Y, h = v0.Y - ray.O.Y;
            double i = v0.Z - v1.Z, j = v0.Z - v2.Z, k = ray.D.Z, l = v0.Z - ray.O.Z;

            double m = f * k - g * j, n = h * k - g * l, p = f * l - h * j;
            double q = g * i - e * k, s = e * j - f * i;

            double invDenom = 1.0 / (a * m + b * q + c * s);

            double e1 = d * m - b * n - c * p;
            double beta = e1 * invDenom;

            if (beta < 0.0)
                return false;

            double r = e * l - h * i;
            double e2 = a * n + d * q + c * r;
            double gamma = e2 * invDenom;

            if (gamma < 0.0)
                return false;

            if (beta + gamma > 1.0)
                return false;

            double e3 = a * p - b * r + d * s;
            double t = e3 * invDenom;

            if (t < kEpsilin)
                return false;

            tmin = t;

            return true;
        }
    }
}