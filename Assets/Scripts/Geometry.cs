using UnityEngine;
using System.Collections.Generic;
namespace Geometry {
	
	public class Triangulator
	{
		//Triangulator class taken from the Unity Community Wiki.
		//All credit goes to runevision for this class.
		private List<Vector2> m_points = new List<Vector2>();
	 
		public Triangulator (Vector2[] points) {
			m_points = new List<Vector2>(points);
		}
	 
		public int[] Triangulate() {
			List<int> indices = new List<int>();
	 
			int n = m_points.Count;
			if (n < 3)
				return indices.ToArray();
	 
			int[] V = new int[n];
			if (Area() > 0) {
				for (int v = 0; v < n; v++)
					V[v] = v;
			}
			else {
				for (int v = 0; v < n; v++)
					V[v] = (n - 1) - v;
			}
	 
			int nv = n;
			int count = 2 * nv;
			for (int v = nv - 1; nv > 2; ) {
				if ((count--) <= 0)
					return indices.ToArray();
	 
				int u = v;
				if (nv <= u)
					u = 0;
				v = u + 1;
				if (nv <= v)
					v = 0;
				int w = v + 1;
				if (nv <= w)
					w = 0;
	 
				if (Snip(u, v, w, nv, V)) {
					int a, b, c, s, t;
					a = V[u];
					b = V[v];
					c = V[w];
					indices.Add(a);
					indices.Add(b);
					indices.Add(c);
					for (s = v, t = v + 1; t < nv; s++, t++)
						V[s] = V[t];
					nv--;
					count = 2 * nv;
				}
			}
	 
			indices.Reverse();
			return indices.ToArray();
		}
	 
		private float Area () {
			int n = m_points.Count;
			float A = 0.0f;
			for (int p = n - 1, q = 0; q < n; p = q++) {
				Vector2 pval = m_points[p];
				Vector2 qval = m_points[q];
				A += pval.x * qval.y - qval.x * pval.y;
			}
			return (A * 0.5f);
		}
	 
		private bool Snip (int u, int v, int w, int n, int[] V) {
			int p;
			Vector2 A = m_points[V[u]];
			Vector2 B = m_points[V[v]];
			Vector2 C = m_points[V[w]];
			if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
				return false;
			for (p = 0; p < n; p++) {
				if ((p == u) || (p == v) || (p == w))
					continue;
				Vector2 P = m_points[V[p]];
				if (InsideTriangle(A, B, C, P))
					return false;
			}
			return true;
		}
	 
		private bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
			float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
			float cCROSSap, bCROSScp, aCROSSbp;
	 
			ax = C.x - B.x; ay = C.y - B.y;
			bx = A.x - C.x; by = A.y - C.y;
			cx = B.x - A.x; cy = B.y - A.y;
			apx = P.x - A.x; apy = P.y - A.y;
			bpx = P.x - B.x; bpy = P.y - B.y;
			cpx = P.x - C.x; cpy = P.y - C.y;
	 
			aCROSSbp = ax * bpy - ay * bpx;
			cCROSSap = cx * apy - cy * apx;
			bCROSScp = bx * cpy - by * cpx;
	 
			return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
		}
	}
	
		//theory for this comes from:
		//https://www.topcoder.com/community/competitive-programming/tutorials/geometry-concepts-line-intersection-and-its-applications/
		//WARNING: Intersections at endpoints are allowed. Shouldn't be an issue in practice, but it's there.
	public class IntersectionChecker{
		private bool CheckOneIntersection(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4){
			//checks whether two line segments intersect in the XY plane (setting z=0)
			//get line equations as Ax + By = C
			float A1 = point2.y - point1.y;
			float B1 = point1.x - point2.x;
			float C1 = A1 * point1.x + B1 * point1.y;
			float A2 = point4.y - point3.y;
			float B2 = point3.x - point4.x;
			float C2 = A2 * point3.x + B2 * point3.y;
			
			//solve the system of simultaneous equations for the intersection
			float delta = A1 * B2 - A2 * B1;

			if (delta == 0) 
				return false; //lines are parallel

			//find intersection point
			float x = (B2 * C1 - B1 * C2) / delta;
			float y = (A1 * C2 - A2 * C1) / delta;
			
			//check if it is within segments by elimination of cases
			if (point1.x <= point2.x){
				if ((x < point1.x) || (point2.x < x)){
					return false;
				}
			} else{
				if ((x < point2.x) || (point1.x < x)){
					return false;
				}
			}
			
			if (point3.x <= point4.x){
				if ((x < point3.x) || (point4.x < x)){
					return false;
				}
			} else{
				if ((x < point4.x) || (point3.x < x)){
					return false;
				}
			}
			if (point1.y <= point2.y){
				if ((y < point1.y) || (point2.y < y)){
					return false;
				}
			} else{
				if ((y < point2.y) || (point1.y < y)){
					return false;
				}
			}
			
			if (point3.y <= point4.y){
				if ((y < point3.y) || (point4.y < y)){
					return false;
				}
			} else{
				if ((y < point4.y) || (point3.y < y)){
					return false;
				}
			}
			//no need to check the y's too
			return true;
		}
		
		public bool CheckIfAddable(Vector3 point1, List<Vector3> points){
			//returns true if adding the point does not create a self-intersecting polygon
			//false otherwise
			Vector3 lastPoint = points[points.Count - 1];
			for(int i=0;i<points.Count-2;i++){
				if (CheckOneIntersection(point1, lastPoint, points[i], points[i+1])){
					return false;
				}
			}
			return true;
		}
		
	}	
}