﻿using System.Diagnostics;
using NeoGeoSolver.NET.Entities;

using NeoGeoSolver.NET.Solver;

namespace NeoGeoSolver.NET.Constraints;

public abstract class Constraint {
	private List<Constraint> _usedInConstraints = new();
	public virtual IEnumerable<Param> parameters { get { yield break; } }
	public virtual IEnumerable<Expression> equations { get { yield break; } }

	private enum Option {
		Default
	}

	protected virtual Enum optionInternal { get { return Option.Default; } set { } }

	protected void AddEntity<T>(T e) where T : IEntity {
		if(e is Entity) (e as Entity).AddConstraint(this);
	}

	protected void AddConstraint(Constraint c) {
		c._usedInConstraints.Add(this);
	}

	public virtual void ChooseBestOption() {
		OnChooseBestOption();
	}

	protected virtual void OnChooseBestOption() {
		var type = optionInternal.GetType();
		var names = Enum.GetNames(type);
		if(names.Length < 2) return;
		
		var minValue = -1.0;
		var bestOption = 0;
		
		for(var i = 0; i < names.Length; i++) {
			optionInternal = (Enum)Enum.Parse(type, names[i]);
			var exprs = equations.ToList();
			
			var curValue = exprs.Sum(e => Math.Abs(e.Eval()));
			// TODO		Debug.Log(String.Format("check option {0} (min: {1}, cur: {2})\n", optionInternal, min_value, cur_value));
			if(minValue < 0.0 || curValue < minValue) {
				minValue = curValue;
				bestOption = i;
			}
		}
		optionInternal = (Enum)Enum.Parse(type, names[bestOption]);
		// TODO		Debug.Log("best option = " + optionInternal.ToString());
	}

	public IEntity GetEntity(int i) {
		return sketch.feature.detail.GetObjectById(ids[i]) as IEntity;
	}

	protected Constraint GetConstraint(int i) {
		return sketch.feature.detail.GetObjectById(ids[i]) as Constraint;
	}

	private int GetEntitiesCount() {
		return ids.Count;
	}

	public bool HasEntitiesOfType(EntityType type, int required) {
		var count = 0;
		for(var i = 0; i < GetEntitiesCount(); i++) {
			var e = GetEntity(i);
			if(e.type == type) count++;
		}
		return count == required;
	}

	protected IEntity GetEntityOfType(EntityType type, int index) {
		var curIndex = 0;
		for(var i = 0; i < GetEntitiesCount(); i++) {
			var e = GetEntity(i);
			if(e.type != type) continue;
			if(curIndex == index) return e;
			curIndex++;
		}
		return null;
	}

	protected void SetEntity(int i, IEntity e) {
		var ent = GetEntity(i) as Entity;
		if(ent != null) {
			ent.RemoveConstraint(this);
		}
		ent = GetEntity(i) as Entity;
		if(ent != null) {
			ent.AddConstraint(this);
		}
	}
}
