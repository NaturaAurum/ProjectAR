using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectAR.Util.Event
{
    public class EventManager
    {
        public delegate void Handler( params object[] args );

        private static Hashtable listeners = new Hashtable();

        public static void Listen( EventMessage message, Handler action )
        {
            var actions = listeners[ message ] as Handler;
            if(actions != null)
            {
                listeners[ message ] = actions + actions;
            }
            else
            {
                listeners[ message ] = action;
            }
        }

        public static void Remove(EventMessage message, Handler action )
        {
            var actions = listeners[ message ] as Handler;
            if(actions != null)
            {
                listeners[ message ] = actions - action;
            }
        }

        public static void Send(EventMessage message, params object[] args )
        {
            var actions = listeners[ message ] as Handler;
            if(actions != null)
            {
                actions( args );
            }
        }
    }

    public enum EventMessage
    {
        // For Demo
        Feed = 0,
        Feeded,

        Thorwed,
        ChangeGameState,
    }
}
