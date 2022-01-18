using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    //check if the attached gameobject has Health component
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
    }
}