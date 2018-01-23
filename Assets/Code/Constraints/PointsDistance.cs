﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PointsDistance : ValueConstraint {

	public ExpVector p0 {
		get {
			var e = GetObjectById(p0id) as IEntity;
			return e.PointsInPlane(sketch.plane).FirstOrDefault();
		}
	}
	public ExpVector p1 {
		get {
			var e = GetObjectById(p1id) as IEntity;
			return e.PointsInPlane(sketch.plane).FirstOrDefault();
		}
	}

	public Id p0id { get; set; }
	public Id p1id { get; set; }

	public PointsDistance(Sketch sk) : base(sk) { }

	public PointsDistance(Sketch sk, IEntity p0, IEntity p1) : base(sk) {
		p0id = p0.id;
		p1id = p1.id;
		Satisfy();
	}

	public override IEnumerable<Exp> equations {
		get {
			yield return (p1 - p0).Magnitude() - value.exp;
		}
	}

	protected override void OnDraw(LineCanvas canvas) {
		Vector3 p0p = p0.Eval();
		Vector3 p1p = p1.Eval();
		Vector3 dir = p1p - p0p;
		Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;
		float dist = Vector3.Dot(pos - p0p, perp);
		
		Vector3 p2 = p0p + perp * dist;
		Vector3 p3 = p1p + perp * dist;
		
		float s = Mathf.Sign(dist);
		Vector3 m = perp * s;
		canvas.DrawLine(p0p, p2 + m);
		canvas.DrawLine(p1p, p3 + m);

		float len = Vector3.Dot(pos - p0p, dir) / Vector3.Dot(dir, dir);
		Vector3 u = dir.normalized;
		Vector3 v = perp.normalized;

		var a = (len < 0f || len > 1f) ? -1.0f : 1.0f;

		canvas.DrawLine(p2, p2 + a * u * 1.5f + v * 0.5f);
		canvas.DrawLine(p2, p2 + a * u * 1.5f - v * 0.5f);
		canvas.DrawLine(p3, p3 - a * u * 1.5f + v * 0.5f);
		canvas.DrawLine(p3, p3 - a * u * 1.5f - v * 0.5f);

		if(len < 0f) {
			p2 += dir * len;
			p3 += u * 2.0f; 
		}
		if(len > 1f) {
			p3 += dir * (len - 1f);
			p2 -= u * 2.0f;
		}
		canvas.DrawLine(p2, p3);
	}

	protected override Matrix4x4 OnGetBasis() {
		var p0pos = p0.Eval();
		var p1pos = p1.Eval();
		var pos = (p0pos + p1pos) * 0.5f;
		var dir = p0pos - p1pos;
		var ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		var rot = Quaternion.AngleAxis(ang, Vector3.forward);
		return Matrix4x4.TRS(pos, rot, Vector3.one); 
	}

}
