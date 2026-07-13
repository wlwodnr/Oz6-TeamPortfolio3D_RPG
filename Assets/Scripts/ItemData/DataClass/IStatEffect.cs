using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType { StatUp, LifeSteal }
public interface IStatEffect
{
    List<StatModifier> StatModifiers { get; }
}
