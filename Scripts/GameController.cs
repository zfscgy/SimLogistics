using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimLogistics.Common
{
    using Common;
    using WorldMaker;
    using DataReader;
    using Display;
    using Base;
    public class GameController : MonoBehaviour
    {
        public SimpleWorldMaker simpleWorldMaker;
        public EditorDataReader editorDataReader;
        public DisplayController displayController;

        private HeadQuater headQuater;

        private bool isInitialized = false;
        private void Start()
        {
            simpleWorldMaker.GenerateMap();
            Debug.Log("Map generated.");
            Map map = simpleWorldMaker.Map;
            headQuater = new HeadQuater(map, simpleWorldMaker.DemandPoints, new Base.SC[] { simpleWorldMaker.supplyCenter });
            Debug.Log("headquater created.");
            headQuater.AssignTask(editorDataReader.AllTasks);
            Debug.Log("Tasks scheduled.");
            displayController.Init(headQuater.Transportations);
            Debug.Log("Displayer initialized.");
            isInitialized = true;
        }
        private int fixedFrameCount = 0;
        private float totalTimePassed = 0f;
        private void FixedUpdate()
        {
            if (!isInitialized) return;
            headQuater.Update(totalTimePassed);
            totalTimePassed += Settings.timeStep * Settings.timeScale;
        }
    }
}
