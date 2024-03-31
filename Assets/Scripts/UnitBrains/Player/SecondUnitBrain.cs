using System;
using System.Collections.Generic;
using Model;
using Model.Runtime.Projectiles;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;

        private List<Vector2Int> TargetsOutReach = new List<Vector2Int>();
        private const int MaxTargets = 4;
        private static int idValue = 0;
        private int unitId = idValue++;

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            int currentTemperature = GetTemperature();

            if (currentTemperature >= OverheatTemperature)
                return;

            IncreaseTemperature();

            for (int i = 0; i <= currentTemperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int target = Vector2Int.zero;
            if (TargetsOutReach.Count > 0)
                target = TargetsOutReach[0];
            else
                target = unit.Pos;

            if (IsTargetInRange(target))
                return unit.Pos;
            else
                return unit.Pos.CalcNextStepTowards(target);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> _allTargets = new List<Vector2Int>();
            List<Vector2Int> _targetsForAttack = new List<Vector2Int>();

            int indexCurrentTargetForAttack = unitId % MaxTargets;
            Vector2Int closestTarget = new();
            int minWeight = int.MaxValue;

            foreach (var target in GetAllTargets())
                _allTargets.Add(target);

            SortByDistanceToOwnBase(_allTargets);

            for (int i = 0; i < _allTargets.Count; i++)
            {
                int weight = Math.Abs(i - indexCurrentTargetForAttack);

                if (IsTargetInRange(_allTargets[i]))
                {
                    if (weight < minWeight)
                    {
                        minWeight = weight;
                        closestTarget = _allTargets[i];
                        _targetsForAttack.Add(closestTarget);
                    }
                }
                else
                    TargetsOutReach.Add(_allTargets[i]);
            }

            if (_allTargets.Count == 1)
            {
                var enemyBaseTarget = runtimeModel.RoMap.Bases[
                IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
                _targetsForAttack.Add(enemyBaseTarget);
            }

            return _targetsForAttack;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}