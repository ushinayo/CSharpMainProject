using System.Collections.Generic;
using Model.Runtime.Projectiles;
using Unity.VisualScripting;
using UnityEngine;

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
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            int currentTemperature = GetTemperature();

            if (currentTemperature >= OverheatTemperature)
                return;
           
            for (int i = 0; i <= currentTemperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        //Определи какая из целей в result находится ближе всего к нашей базе.
        //Используй подход, который мы разобрали в 5-ом уроке «Подготовка к домашнему заданию».

        //Для определения расстояния от конкретной цели до нашей базы,
        //используй метод DistanceToOwnBase.Ты не увидишь его реализации
        //в этом скрипте, но не волнуйся, это не помешает тебе его вызвать.
        //Этот метод принимает цель, расстояние от которой до базы
        //мы хотим узнать, а возвращает как раз это расстояние.

        //После того как ты найдешь ближайшую к базе цель,
        //в том случае если она действительно была найдена,
        //очисти список result и добавь в него эту цель.
        //Верни список result.

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = GetReachableTargets();

            Vector2Int target = Vector2Int.zero;
            float distance = float.MaxValue;

            foreach (var j in result) 
            { 
                float range = DistanceToOwnBase(j);
                if(distance > range)
                {
                    target = j;
                    distance = range;
                }
            }

            result.Clear();
            if (distance < float.MaxValue)
                result.Add(target);

            return result;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
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
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}