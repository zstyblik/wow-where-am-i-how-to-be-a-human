using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.POI;
using Styx.Pathing;
using Styx.Plugins;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Bot.CustomClasses
{
    public class WhereAmI : HBPlugin
    {
        private Random rGen = new Random();

        //private static LocalPlayer Me { get { return ObjectManager.Me; } }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        public override string Name { get { return "Where am i?"; } }
        public override string Author { get { return "Cartman"; } }
        public override Version Version { get { return new Version(1, 0); } }
        public override bool WantButton { get { return true; } }

        public Stopwatch CheckMapTimer = new Stopwatch();
        public Stopwatch HoldMapTimer = new Stopwatch();

        public bool IsMapOpened = false;

        public int WhereAmITimeout = 0;
        public int HoldMapTimeout = 0;

        public override void Pulse()
        {
            if (!CheckMapTimer.IsRunning)
            {
                CheckMapTimer.Reset();
                CheckMapTimer.Start();
                WhereAmITimeout = rGen.Next(60, 300);
            }

            if (Styx.CommonBot.Frames.AuctionFrame.Instance.IsVisible || Styx.CommonBot.Frames.MailFrame.Instance.IsVisible)
            {
                return;
            }

            if (!Me.Combat && CheckMapTimer.Elapsed.TotalSeconds > WhereAmITimeout)
            {
                if (!HoldMapTimer.IsRunning)
                {
                    HoldMapTimer.Reset();
                    HoldMapTimer.Start();
                    HoldMapTimeout = rGen.Next(1, 3);
                }
                if (!IsMapOpened)
                {
                    WoWMovement.MoveStop();
                    Lua.DoString("RunMacroText('/click MiniMapWorldMapButton');");
                    IsMapOpened = true;
                }
                Logging.Write("Checking where the heck am i?");
                if (HoldMapTimer.Elapsed.TotalSeconds > HoldMapTimeout)
                {
                    Logging.Write("Ok, got it!");
                    if (IsMapOpened)
                    {
                        Lua.DoString("RunMacroText('/click WorldMapFrameCloseButton');");
                        IsMapOpened = false;
                    }
                    CheckMapTimer.Reset();
                    WhereAmITimeout = 0;
                    HoldMapTimer.Reset();
                    HoldMapTimeout = 0;
                }
            }

            if (IsMapOpened && !Me.Combat) {
                return;
            }

            if (IsMapOpened && Me.Combat)
            {
                Logging.Write("I'm in combat! close Map");
                if (IsMapOpened)
                {
                    Lua.DoString("RunMacroText('/click WorldMapFrameCloseButton');");
                    IsMapOpened = false;
                }
                CheckMapTimer.Reset();
                WhereAmITimeout = 0;
                HoldMapTimer.Reset();
                HoldMapTimeout = 0;
                return;
            }
        }
    }
}
