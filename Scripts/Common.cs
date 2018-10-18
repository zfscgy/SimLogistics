using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimLogistics.Common
{
    using Base;
    using Paras;
    using Display;

    /// <summary>
    /// Describe a path with vertices
    /// </summary>
    public class Path
    {
        private Vector2[] actualVertices;
        private Vector2[] vertices;
        private float[] mileages;
        bool isMileagesRaw = false;
        public Path()
        {
            actualVertices = null;
            vertices = null;
            mileages = null;
        }
        /// <summary>
        /// Using an array of vertices to initialize a Path
        /// For example. Input [(0,0), (0,1), (1,1), (1,3)]
        /// the mileages should be [0, 1, 2, 4]
        /// </summary>
        /// <param name="_actualVertices">Array of vertices</param>
        public Path(Vector2[] _actualVertices)
        {
            actualVertices = _actualVertices;
            vertices = new Vector2[_actualVertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = actualVertices[i] / Settings.distanceScale;
            }

            mileages = new float[vertices.Length + 1];
            mileages[0] = 0f;
            mileages[vertices.Length] = float.MaxValue;
            for (int i = 1; i < vertices.Length; i++)
            {
                mileages[i] = mileages[i - 1] + Vector2.Distance(actualVertices[i - 1], actualVertices[i]);
            }
        }


        /// <summary>
        /// Using an array of vertices and an array of floats to initialize a path
        /// For example. Input [(0,0), (0,1), (1,1), (1,3)]
        /// the mileages we calculated should be [0, 1, 2, 4]
        /// But path is not always straight, so the mileage maybe [0, 1.2, 2.5, 5.1]
        /// In order to avoid contradictory, we can specify mileage using parameter
        /// </summary>
        /// <param name="_actualVertices"></param>
        /// <param name="_mileages"></param>
        public Path(Vector2[] _actualVertices, float[] _mileages)
        {
            actualVertices = _actualVertices;
            vertices = new Vector2[_actualVertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = actualVertices[i] / Settings.distanceScale;
            }
            mileages = _mileages;
        }


        /// <summary>
        /// Get the number of vertex of the path
        /// </summary>
        /// <returns>Integer, number of vertex</returns>
        public int Length() { return vertices.Length; }
        public Vector2 GetVertex(int index) { return vertices[index]; }
        public float GetDistance() { return mileages[vertices.Length - 1]; }
        public float GetDistance(int vertexIndex) { return mileages[vertexIndex]; }
        public Vector2 GetPosition(int vertexIndex, float over)
        {
            if(vertexIndex == vertices.Length - 1)
            {
                return vertices[vertexIndex];
            }
            else
            {
                float dBetweenVertices = Vector2.Distance(vertices[vertexIndex], vertices[vertexIndex + 1]);
                return over / dBetweenVertices * vertices[vertexIndex + 1] + (1 - over / dBetweenVertices) * vertices[vertexIndex];
            }
        }

        /// <summary>
        /// Combine two paths into one, link the second path's head to the first path's rear
        /// Just to merge two vertices' array
        /// </summary>
        /// <param name="pathFirst">first path</param>
        /// <param name="pathSecond">second path</param>
        /// <returns>merged path</returns>
        public static Path Combine(Path pathFirst, Path pathSecond)
        {
            Vector2[] allVertices = new Vector2[pathFirst.Length() + pathSecond.Length() - 1];
            pathFirst.vertices.CopyTo(allVertices, 0);
            pathSecond.vertices.CopyTo(allVertices, pathFirst.Length() - 1);
            return new Path(allVertices);
        }
    }



    public class Transportation : DisplayableBase
    {
        public TransportationType Type { get; private set; }
        /// <summary>
        /// Returns the maximum capacity of the transportation
        /// </summary>
        /// <returns>Maximum capacity</returns>
        public float Capacity { get; private set; }

        protected float m_load;
        /// <summary>
        /// Returns the load of a specific transportation. Load should not be greater than the capacity
        /// </summary>
        public float Load { get { return m_load; } }

        protected float m_speed;
        private float inner_speed;
        public float Speed
        {
            get { return m_speed; }
            set { m_speed = value; inner_speed = Settings.timeScale * Settings.timeStep / Settings.distanceScale * value; }
        }

        public Vector2 Position { get; private set; }

        public TransportationState State { get; private set; }

        protected SupplyTask m_task;
        public SupplyTask Task
        {
            get { return m_task; }
            set
            {
                if(State == TransportationState.waiting)
                {
                    m_task = value;
                }
            }
        }

        protected Path path;
        protected float currentDistance;
        protected int currentVertex;

        /// <summary>
        /// An function delegate. 
        /// When arrived destination, execute this.
        /// </summary>
        public delegate void DestArrived();
        protected DestArrived destArrived;



        Transportation(TransportationType _type, float _capacity, float _speed)
        {
            Type = _type;
            Capacity = _capacity;
            Speed = _speed;
        }

        public static Transportation GetTransportation(TransportationType _type)
        {
            return new Transportation(_type, TransportationParas.TransportationCapacity[_type], TransportationParas.TransportationSpeed[_type]);
        }

        public void ArriveDestination()
        {
            State = TransportationState.returning;
        }

        public void ArriveHome()
        {
            State = TransportationState.waiting;
        }


        /// <summary>
        /// Move one step
        /// </summary>
        public void MoveStep()
        {
            switch (State)
            {
                case TransportationState.carrying:
                    currentDistance += inner_speed;
                    if (path.GetDistance(currentVertex + 1) < currentDistance) currentVertex++;
                    if (currentDistance >= path.GetDistance())
                    {
                        State = TransportationState.returning;
                    }
                    break;
                case TransportationState.returning:
                    currentDistance -= inner_speed;
                    if (path.GetDistance(currentVertex) > currentDistance) currentVertex--;
                    if (currentVertex < 0)
                    {
                        currentVertex = 0;
                        State = TransportationState.waiting;
                    }
                    break;
            }
        }

        public TransportationState SetOff(Path _path, DestArrived _destArrived)
        {
            path = _path;
            destArrived = _destArrived;
            currentDistance = 0f;
            currentVertex = 0;
            TransportationState previousState = State;
            State = TransportationState.carrying;
            return previousState;
        }

        Vector3 DisplayableBase.GetPosition()
        {
            if (path == null) return new Vector3(0, 1000, 0);
            Vector2 pos2d = path.GetPosition(currentVertex, currentDistance - path.GetDistance(currentVertex));
            return new Vector3(pos2d.x, 0, pos2d.y);
        }

        Vector3 DisplayableBase.GetForwarding()
        {
            if (path == null) return new Vector3(1, 0, 0);
            Vector2 pos2d;
            if (currentVertex == path.Length() - 1)
            {
                pos2d = path.GetVertex(currentVertex) - path.GetVertex(currentVertex - 1);
            }
            else
            {
                pos2d = path.GetVertex(currentVertex + 1) - path.GetVertex(currentVertex);
            }
            if (State == TransportationState.carrying)
            {
                return (this as DisplayableBase).GetPosition() + new Vector3(pos2d.x, 0, pos2d.y);
            }
            else
            {
                return (this as DisplayableBase).GetPosition() - new Vector3(pos2d.x, 0, pos2d.y);
            }
        }

        bool DisplayableBase.GetState()
        {
            return State != TransportationState.waiting;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class DemandPoint
    {
        public float Demand { get; private set; }
        public float CurrentSupply { get; private set; }
        /// <summary>
        /// The DemandPoint's index in the points array of the RoadNetwork
        /// </summary>
        public int Position { get; private set; }
        public DemandPoint(int _position, float _demand)
        {
            Position = _position;
            Demand = _demand;
            CurrentSupply = 0.0f;
        }
        public void GetSupply(float volume)
        {
            CurrentSupply += volume;
        }        
    }


    public class SupplyTask
    {
        public float m_startTime;
        private float innerStartTime;
        public float StartTime { get { return m_startTime; }
            private set { m_startTime = value; innerStartTime = value / Settings.timeScale; } }
        public float EndTime { get; private set; }
        private Transportation transportation;
        public delegate void TaskFinished(Transportation freedTp);
        public Transportation GetTransportation()
        {
            return transportation;
        }
        private DemandPoint demandPoint;
        public DemandPoint GetDemandPoint()
        {
            return demandPoint;
        }
        private Path path;
        public Path GetPath()
        {
            return path;
        }
        public SupplyTask(float _startTime, Transportation _transportation, DemandPoint _demandPoint, Path _path)
        {
            StartTime = _startTime;
            EndTime = 0f;
            transportation = _transportation;
            demandPoint = _demandPoint;
            path = _path;
        }
        public float EstimateReturnTime()
        {
            return StartTime + 2 * path.GetDistance() / transportation.Speed;
        }
        public void StartTask()
        {
            Debug.Log("Task started:");
            transportation.SetOff(path, TransportationArrive);
        }
        public void TransportationArrive()
        {
            EndTime = Time.time;
            demandPoint.GetSupply(transportation.Load);
        }
    }

    public class SupplyCenter
    {
        public int Position { get; private set; }
        private Dictionary<TransportationType, List<Transportation>[]> Transportations = 
            new Dictionary<TransportationType, List<Transportation>[]>();
        private List<SupplyTask> Tasks = new List<SupplyTask>();
        private int currentWaitingIndex = 0;
        public bool IsScheduled { get; private set; }
        private Task[] rawTasks;
        public SupplyCenter(int _position, int[] _transportations, HeadQuater headQuater)
        {
            Position = _position;
            for(int i = 0; i < _transportations.Length; i++)
            {
                TransportationType type = (TransportationType)i;
                Transportations.Add(type, new List<Transportation>[]
                {
                    new List<Transportation>(), new List<Transportation>()
                }); 
                for(int j = 0; j < _transportations[i]; j++)
                {
                    Transportation newTp = Transportation.GetTransportation(type);
                    Transportations[type][0].Add(newTp);
                    headQuater.RegisterTransportation(newTp);
                }
            }
        }

        public void AssignTask(Task[] _rawTasks)
        {
            rawTasks = _rawTasks;
        }

        private void ResetQueue()
        {
            foreach(TransportationType tpTy in System.Enum.GetValues(typeof(TransportationType)))
            {
                if (!Transportations.ContainsKey(tpTy)) continue;
                Transportations[tpTy][0].AddRange(Transportations[tpTy][1]);
                Transportations[tpTy][1].Clear();
            }
        }

        public bool Schedule(Map map, Dictionary<int, DemandPoint> DemandPoints)
        {
            foreach(Task t in rawTasks)
            {
                float time = t.time;
                Transportation tp;
                TransportationType ty = t.transportationType;
                List<SupplyTask> OnGoingTasks = new List<SupplyTask>();
                if(Transportations[ty][0].Count > 0)
                {
                    tp = Transportations[ty][0][0];
                    Transportations[ty][1].Add(tp);
                    Transportations[ty][0].RemoveAt(0);
                    SupplyTask newTask = new SupplyTask(time, tp, DemandPoints[t.dp], map.GetPath(t.path));
                    OnGoingTasks.Add(newTask);
                    Tasks.Add(newTask);
                }
                else
                {
                    Transportation lastFreedTp = null;
                    for(int i = OnGoingTasks.Count - 1; i >= 0; i --)
                    {
                        SupplyTask st = OnGoingTasks[i];
                        if(st.EstimateReturnTime() > time)
                        {
                            Transportation freedTp = lastFreedTp = st.GetTransportation();
                            OnGoingTasks.RemoveAt(i);
                            Transportations[freedTp.Type][0].Add(freedTp);
                            Transportations[freedTp.Type][1].Remove(freedTp);
                        }
                    }
                    if (lastFreedTp == null) return false;
                    SupplyTask newTask = new SupplyTask(time, lastFreedTp, DemandPoints[t.dp], map.GetPath(t.path));
                    OnGoingTasks.Add(newTask);
                    Tasks.Add(newTask);
                }
            }
            IsScheduled = true;
            ResetQueue();
            return true;
        }


        public void Update(float time)
        {
            Debug.Log("Current task index: " + currentWaitingIndex);
            //Debug.Log("Current task start time: " + Tasks[currentWaitingIndex].StartTime);
            while(currentWaitingIndex < Tasks.Count && Tasks[currentWaitingIndex].StartTime < time)
            {
                Tasks[currentWaitingIndex++].StartTask();
            }
        }

        private void TaskFinished(Transportation freedTp)
        {
            Transportations[freedTp.Type][0].Add(freedTp);
            Transportations[freedTp.Type][1].Remove(freedTp);
        }
    }

    public class Map
    {
        private List<MapPoint> Points;
        private Dictionary<uint, Path> pathDict = new Dictionary<uint, Path>();
        public Map(Vector2[] _points, List<List<ushort>> paths)
        {
            Points = new List<MapPoint>();
            foreach(Vector2 point in _points)
            {
                Points.Add(new MapPoint(point));
            }
            foreach(List<ushort> nodeList in paths)
            {
                Vector2[] pathVertices = new Vector2[nodeList.Count];
                for(int i = 0; i < nodeList.Count; i++)
                {
                    pathVertices[i] = Points[nodeList[i]].actualPosition;
                }
                pathDict.Add(((uint)nodeList[0] << 16) + nodeList[nodeList.Count - 1], new Path(pathVertices));
            }
        }

        public Map(MapPoint[] mapPoints, List<List<ushort>> paths)
        {
            Points = new List<MapPoint>(mapPoints);
            foreach (List<ushort> nodeList in paths)
            {
                Vector2[] pathVertices = new Vector2[nodeList.Count];
                for (int i = 0; i < nodeList.Count; i++)
                {
                    pathVertices[i] = Points[nodeList[i]].actualPosition;
                }
                pathDict.Add(((uint)nodeList[0] << 16) + nodeList[nodeList.Count - 1], new Path(pathVertices));
            }
        }

        public MapPoint GetPoint(int index)
        {
            return Points[index];
        }

        public int NPoint()
        {
            return Points.Count;
        }

        public Path GetPath(ushort from, ushort to)
        {
            return pathDict[((uint)from << 16) + (uint)to];
        }

        public Path GetPath(ushort[] points)
        {
            Path path = GetPath(points[0], points[1]);
            for (int i = 2; i < points.Length; i++)
            {
                path = Path.Combine(path, GetPath(points[i - 1], points[i]));
            }
            return path;
        }
    }

    public class HeadQuater
    {
        private readonly Map m_map;
        private Dictionary<int, DemandPoint> DemandPoints = new Dictionary<int, DemandPoint>();
        private Dictionary<int, SupplyCenter> SupplyCenters = new Dictionary<int, SupplyCenter>();
        public List<Transportation> Transportations { get; private set; } 
        public HeadQuater(Map _map, DP[] dps, SC[] scs)
        {
            m_map = _map;
            Transportations = new List<Transportation>();
            foreach (DP dp in dps)
            {
                DemandPoints.Add(dp.position, new DemandPoint(dp.position, dp.demand));
            }
            foreach(SC sc in scs)
            {
                SupplyCenters.Add(sc.position, new SupplyCenter(sc.position, sc.numTransportations, this));
            }
        }

        public void RegisterTransportation(Transportation _tp)
        {
            Transportations.Add(_tp);
        }

        public bool AssignTask(Task[] tasks)
        {
            foreach (SupplyCenter supplyCenter in SupplyCenters.Values)
            {
                List<Task> TasksForThisCenter = new List<Task>();
                foreach(Task task in tasks)
                {
                    if(task.sc == supplyCenter.Position)
                    {
                        TasksForThisCenter.Add(task);
                    }
                }
                supplyCenter.AssignTask(TasksForThisCenter.ToArray());
                if (!supplyCenter.Schedule(m_map, DemandPoints)) { Debug.Log("Schedule failed."); return false; }
            }
            return true;
        }
        public void Update(float time)
        {
            foreach(SupplyCenter sc in SupplyCenters.Values)
            {
                sc.Update(time);
            }
            foreach(Transportation tp in Transportations)
            {
                tp.MoveStep();
            }
        }
    }
}
