﻿using System.Collections.Generic;
using LiteNetLib.Utils;
using Lockstep.Framework;              
using Lockstep.Framework.Networking;             
using Lockstep.Framework.Networking.Serialization;
using Lockstep.Framework.Services;
using UnityEngine;                            

public class LockstepSimulator : MonoBehaviour
{                                                                        
    private Simulation _simulation;      
    private bool _simulationStarted;

    private void Awake()
    {
        _simulation = new Simulation(
            new List<IService>
            {
                new DefaultInputParseService(),
                new UnityViewService()
            })
            {
                FrameDelay = 2,
            };
    }     

    private void Start()
    {
        LockstepNetwork.Instance.MessageReceived += NetworkOnMessageReceived;
    }

    private void NetworkOnMessageReceived(MessageTag messageTag, NetDataReader reader)
    {
        switch (messageTag)
        {
            case MessageTag.Init:
                var pkt = new Init();
                pkt.Deserialize(reader);
                Time.fixedDeltaTime = 1f/pkt.TargetFPS;

                _simulation.Init(pkt.Seed);

                _simulationStarted = true;
                break;
            case MessageTag.Frame:
                //Server sends in ReliableOrdered-mode, so we only care about the latest frame
                //Also possible could be high-frequency unreliable messages and use the redundant frames to fill up the framebuffer in case a frame was lost during transmission
                var pkg = new FramePackage(1);
                pkg.Deserialize(reader);

                _simulation.AddFrame(pkg.Frames[0]);

                //TODO: only for debugging
                _simulation.Simulate();
                break;
        }
    }            
            

    void FixedUpdate()
    {
        if (!_simulationStarted)
        {
            return;
        }   

        //simulation.Simulate(); 

        //if (simulation.FrameCounter % 10 == 0)
        //{
        //    LockstepNetwork.Instance.SendChecksum(new Checksum{FrameNumber = simulation.FrameCounter, Value = simulation.CalculateChecksum()});
        //}                               
    }     
}
