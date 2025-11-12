using System;
using CADability;
using CADability.GeoObject;

namespace CellCAD.Core.Geometry
{
    public static class PouchCellBuilder
    {
        /// Builds a rounded-rectangle footprint on XY using only Line segments (version-safe).
        public static GeoObjectList Build2DFootprint(double lengthMm, double widthMm, double cornerRadiusMm)
        {
            lengthMm = Math.Max(1, lengthMm);
            widthMm = Math.Max(1, widthMm);
            double r = Math.Max(0, Math.Min(cornerRadiusMm, 0.5 * Math.Min(lengthMm, widthMm)));

            double hx = 0.5 * lengthMm;
            double hy = 0.5 * widthMm;

            var list = new GeoObjectList();

            // add one straight segment
            static void AddLine(GeoObjectList dst, GeoPoint a, GeoPoint b)
                => dst.Add(Line.TwoPoints(a, b));

            // approximate an arc with N short segments (counter-clockwise, radians)
            static void AddArc(GeoObjectList dst, GeoPoint c, double rad, double a0, double a1, int segments = 16)
            {
                if (segments < 2) segments = 2;
                double da = (a1 - a0) / segments;
                GeoPoint prev = new GeoPoint(c.x + rad * Math.Cos(a0), c.y + rad * Math.Sin(a0), c.z);
                for (int i = 1; i <= segments; i++)
                {
                    double a = a0 + i * da;
                    GeoPoint cur = new GeoPoint(c.x + rad * Math.Cos(a), c.y + rad * Math.Sin(a), c.z);
                    AddLine(dst, prev, cur);
                    prev = cur;
                }
            }

            // corner centers
            GeoPoint cTR = new GeoPoint(hx - r, hy - r, 0);
            GeoPoint cTL = new GeoPoint(-hx + r, hy - r, 0);
            GeoPoint cBL = new GeoPoint(-hx + r, -hy + r, 0);
            GeoPoint cBR = new GeoPoint(hx - r, -hy + r, 0);

            // edge endpoints
            GeoPoint topR = new GeoPoint(hx - r, hy, 0);
            GeoPoint topL = new GeoPoint(-hx + r, hy, 0);
            GeoPoint leftT = new GeoPoint(-hx, hy - r, 0);
            GeoPoint leftB = new GeoPoint(-hx, -hy + r, 0);
            GeoPoint botL = new GeoPoint(-hx + r, -hy, 0);
            GeoPoint botR = new GeoPoint(hx - r, -hy, 0);
            GeoPoint rightB = new GeoPoint(hx, -hy + r, 0);
            GeoPoint rightT = new GeoPoint(hx, hy - r, 0);

            // go CCW around the rectangle
            AddLine(list, topR, topL);                           // top edge

            // TL corner: top (π/2) -> left (π)
            AddArc(list, cTL, r, Math.PI * 0.5, Math.PI);

            // left edge
            AddLine(list, leftT, leftB);

            // BL corner: left (π) -> bottom (3π/2)
            AddArc(list, cBL, r, Math.PI, Math.PI * 1.5);

            // bottom edge
            AddLine(list, botL, botR);

            // BR corner: bottom (3π/2) -> right (2π)
            AddArc(list, cBR, r, Math.PI * 1.5, Math.PI * 2.0);

            // right edge
            AddLine(list, rightB, rightT);

            // TR corner: right (0) -> top (π/2)
            AddArc(list, cTR, r, 0.0, Math.PI * 0.5);

            return list;
            // (we’re returning 3D lines already on XY; no Path/2D/Ellipse dependencies)
        }
    }
}
