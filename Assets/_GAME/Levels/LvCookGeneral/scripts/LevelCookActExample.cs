using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class LevelCookActExample : LevelCookActAbstract
    {
        [SerializeField] private CookModuleCategorizeIngredient _modulePickIngredient;

        protected override Task PreloadAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task SetUpOnInitLevelAsync()
        {
            await _modulePickIngredient.SetUp();
        }

        private void OnEndModulePickIngredient(ICookModule module)
        {
            Debug.Log("End module pick ingredient");
            EndAct();
        }

        public override void Unload()
        {
        }

        protected override void OnStartAct()
        {
            _modulePickIngredient.StartModule(OnEndModulePickIngredient).RunWithLogException();
        }
    }
}