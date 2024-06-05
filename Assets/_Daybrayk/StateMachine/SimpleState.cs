using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
	//zspublic class SimpleState
	//zs{
	//zs	string _name;
	//zs	public string name => _name;
	//zs
	//zs	public bool hasBeginState { get; private set; } = false;
	//zs	protected StateDelegate beginState;
	//zs
	//zs	public bool hasUpdateState { get; private set; } = false;
	//zs	protected StateDelegate updateState;
	//zs
	//zs	public bool hasEndState { get; private set; } = false;
	//zs	protected StateDelegate endState;
	//zs
	//zs	public SimpleState(string stateName)
    //zs    {
	//zs		_name = stateName;
    //zs    }
	//zs
	//zs	public SimpleState(string stateName, StateDelegate begin, StateDelegate update, StateDelegate end) : this(stateName)
    //zs    {
	//zs		beginState = begin;
	//zs		updateState = update;
	//zs		endState = end;
    //zs    }
	//zs
	//zs	public void BeginState()
    //zs    {
	//zs		beginState?.Invoke();
    //zs    }
	//zs
	//zs	public void UpdateState()
    //zs    {
	//zs		updateState?.Invoke();
    //zs    }
	//zs
	//zs	public void EndState()
    //zs    {
	//zs		endState?.Invoke();
    //zs    }
	//zs
	//zs	public delegate void StateDelegate();
	//zs}
}