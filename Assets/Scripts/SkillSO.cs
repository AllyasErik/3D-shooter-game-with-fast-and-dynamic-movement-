using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillSO", order = 1)]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public GameObject skillPrefab;

    public int damage;
    public float cooldown;

    public SkillIndex skillIndex;
}

public enum SkillIndex
{
    First, Second, Third
}