using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase
{
	protected Car _car = null;

	public ControllerBase(Car car)
	{
		_car = car;
	}

	public abstract void Setup();
	public abstract void Tick(float dt);
}
