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
                    if (fbMessage.path.Length == 0) continue;

                    if (fbMessage.path.Substring(1, FbItemMessage.TABLE_NAME.Length + 1) == FbItemMessage.TABLE_NAME + "/")
                    {
                        FbItemMessage msg = new FbItemMessage(fbMessage);
                        if (!msg.IsParsed) LogParseFailedMsg(fbMessage);
                        else if (fbMessage.operation == Fb_Operations.fb_delete) mw.RemoveItemProperties(msg.Id);
                        else mw.AddProperties(msg.Id, msg);
                    }
                    else if (fbMessage.path.Substring(1, FbCategoryMessage.TABLE_NAME.Length + 1) == FbCategoryMessage.TABLE_NAME + "/")
                    {
                        FbCategoryMessage msg = new FbCategoryMessage(fbMessage);
                        if (!msg.IsParsed) LogParseFailedMsg(fbMessage);
                        else if (fbMessage.operation == Fb_Operations.fb_delete) mw.RemoveCategoryProperties(msg.Id);
                        else mw.AddProperties(msg.Id, msg);
                    }
                    else if (fbMessage.path.Substring(1, FbSupplierMessage.TABLE_NAME.Length + 1) == FbSupplierMessage.TABLE_NAME + "/")
                    {
                        FbSupplierMessage msg = new FbSupplierMessage(fbMessage);
                        if (!msg.IsParsed) LogParseFailedMsg(fbMessage);
                        else if (fbMessage.operation == Fb_Operations.fb_delete) mw.RemoveSupplierProperties(msg.Id);
                        else mw.AddProperties(msg.Id, msg);
                    }
                }
                Thread.Sleep(100);
            }
        }
        private void LogParseFailedMsg(FbEventData fbMessage)
        {
            mw.Log("Failed to parse message. Path: " + fbMessage.path + ", data: " + fbMessage.data);
        }
    }
}
