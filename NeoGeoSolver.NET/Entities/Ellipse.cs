﻿using NeoGeoSolver.NET.Solver;

namespace NeoGeoSolver.NET.Entities;

public class Ellipse : Entity {

	public Point c = new();
	public Param r0 = new("r0");
	public Param r1 = new("r1");
	private ExpressionBasis2d _basis = new();

	public Ellipse()
	{
		_basis.SetPosParams(c.x, c.y);
	}

	public override IEnumerable<Param> parameters {
		get {
			yield return r0;
			yield return r1;
			foreach(var p in _basis.parameters) yield return p;
		}
	}

	public override IEnumerable<Expression> equations {
		get {
			foreach(var e in _basis.equations) yield return e;
		}
	}

	public ExpressionVector Center()
	{
		return c.exp;
	}

	public override ExpressionVector PointOn(Expression t) {
		var angle = t * 2.0 * Math.PI;
		return _basis.TransformPosition(new ExpressionVector(Expression.Cos(angle) * Expression.Abs(r0), Expression.Sin(angle) * Expression.Abs(r1), 0.0));
	}
}
