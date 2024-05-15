using Model.Runtime.ReadOnly;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitBrains;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.UnitBrains
{
    internal class UnitsCoordinator
    {
    }
}
public class UnitsCoordinator : BaseUnitBrain
{
    private IReadOnlyRuntimeModel _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
    private IReadOnlyUnit _runtimePos;
    private Vector2Int _playerBase;
    private Vector2Int _enemyBase;
    private Vector2Int _unitPos;
    private int _health;

    public UnitsCoordinator()
    {
    }
 
    public Vector2Int FindTargetEnemy()
    {
        IEnumerable<Vector2Int> unitPositions = GetAllTargets();
        Vector2Int closestEnemyPosition = new Vector2Int();
        float closestDistance = float.MaxValue;

        foreach (var unitPos in unitPositions)
        {
            Vector2 baseToUnitVector = _unitPos - _playerBase;
            Vector2 baseToBaseVector = _enemyBase - _playerBase;
            Vector2 normalizedBaseToBaseVector = baseToBaseVector.normalized;

            float dotProduct = Vector2.Dot(normalizedBaseToBaseVector, baseToUnitVector);
            float distanceToBase = baseToUnitVector.magnitude;

            if (dotProduct <= 0)
            { 
                if (distanceToBase < closestDistance)
                {
                    closestDistance = distanceToBase;
                    closestEnemyPosition = unitPos;
                }
            }
        }

        if (closestDistance != float.MaxValue)
        {
            Debug.Log($"Ближайший враг для атаки находится на позиции: {closestEnemyPosition}");
            return closestEnemyPosition;
        }
        else
        {
            Debug.Log("На нашей стороне враги не найдены.");
            return Vector2Int.zero;
        }
    }
}