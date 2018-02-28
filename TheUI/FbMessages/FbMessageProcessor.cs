﻿using System;
using System.Threading;

namespace TheUI
{
    class FbMessageProcessor
    {
        private Thread t;
        private MainWindow mw;
        public FbMessageProcessor(MainWindow mw)
        {
            this.mw = mw;
            t = new Thread(new ParameterizedThreadStart(Loop));
            t.Start();
        }
        private void Loop(object obj)
        {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                while (mw.IsFbMessagePending())
                {
                    FbEventData fbMessage = mw.FetchNextFbMessage();
                }
                Thread.Sleep(100);
            }
        }

        private bool TryParse<T>(FbEventData fbMessage) where T : FbMessage, new()
        {
            T tempMsg = new T();
            if (fbMessage.path.Substring(1, tempMsg.TABLE_NAME.Length + 1) == tempMsg.TABLE_NAME + "/")
            {
                try
                {
                    T msg = (T)Activator.CreateInstance(typeof(T), fbMessage);
                    if (!msg.IsParsed) LogParseFailedMsg(fbMessage);
                    //else if (fbMessage.operation == Fb_Operations.fb_delete) mw.RemoveProperties(msg.Id, (dynamic)msg);
                    //else mw.AddProperties(msg.Id, (dynamic)msg);
                    return true;
                }
                catch (Exception e)
                {
                    mw.Log("Message parse failed: " + e.Message);
                }
            }

            return false;
        }

        private void LogParseFailedMsg(FbEventData fbMessage)
        {
            mw.Log("Failed to parse message. Path: " + fbMessage.path + ", data: " + fbMessage.data);
        }
    }
}
