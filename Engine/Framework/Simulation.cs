﻿using System.Collections.Generic;  
using BEPUphysics;    
using ECS.Data;      
using Entitas;
using FixMath.NET;                           
using Lockstep.Framework.Pathfinding;

namespace Lockstep.Framework
{
    public class Simulation
    {                 
        private Systems _systems;


        public const int FRAMERATE = 20;

        public Space Space { get; }


        public int FrameDelay { get; set; }

        public uint FrameCounter { get; private set; }

                                         
        private uint _lastFramePointer;

        private Fix64Random _random;                  

        private readonly Dictionary<uint, Frame> _frames = new Dictionary<uint, Frame>();
             
        public bool CanSimulate
        {
            get
            {
                lock (_frames)
                {
                    return _lastFramePointer - FrameCounter - FrameDelay > 0;
                }
            }
        }

        public Simulation(ICollection<IService> services)
        {
            Space = new Space();

            _systems = new LockstepSystems(Contexts.sharedInstance, services);
        }
          

        public void Init(int seed)
        {  
            _random = new Fix64Random(seed);    
            
            _systems.Initialize();

            GridManager.Initialize();               
        }


        public void AddFrame(Frame frame)
        {
            lock (_frames)
            {      
                _frames[_lastFramePointer++] = frame;
            }
        }             
                   
        public ulong CalculateChecksum()
        {
            ulong hash = 3;      
            return hash;
        }


        public Fix64 NextRandom()
        {
            return _random.Next();
        }


        public void Simulate()
        {
            Frame currentFrame;
            lock (_frames)
            {
                currentFrame = _frames[FrameCounter++];
            }

            Contexts.sharedInstance.input.SetFrame(currentFrame.SerializedInputs);

            _systems.Execute();
            _systems.Cleanup();

            //if (!CanSimulate)
            //{
            //    return;
            //}

            //Frame currentFrame;
            //lock (_frames)
            //{
            //    currentFrame = _frames[FrameCounter++];
            //}

            //lock (_pendingEntities)
            //{                                         
            //    foreach (var entity in _pendingEntities)
            //    {
            //        entity.ID = _entityCounter;
            //        _entities[_entityCounter] = entity;
            //        if (entity is ILockstepAgent agent)
            //        {
            //            Space.Add(agent.Body);
            //        }

            //        _entityCounter++;
            //    }
            //    _pendingEntities.Clear();
            //} 

            //foreach (var serializedInput in currentFrame.SerializedInputs)
            //{
            //    _commandHandler.Handle(serializedInput);   
            //}  

            //foreach (var entity in _entities.Values)
            //{
            //    entity.Simulate();                        
            //}
                      
            //Space.Update();

        }      
    }
}