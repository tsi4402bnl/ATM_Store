using System;
using System.Threading;

namespace TheUI
{
    class FbMessageParser
    {
        private Thread t;
        private MainWindow mw;
        public FbMessageParser(MainWindow mw)
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
                    if (fbMessage.data.Length == 0) continue;
                    if (fbMessage.data.Substring(1, FbItemMessage.TABLE_NAME.Length + 1) == FbItemMessage.TABLE_NAME + "/")
                    {
                        new FbItemMessage(fbMessage).Respond();
                    }
                    if (path.substr(1, FbItemMessage::TableName().size() + 1) == FbItemMessage::TableName() + "/")
                    {
                        FbItemMessage(fbMessage).Respond();
                    }
                    else if (path.substr(1, FbCategoryMessage::TableName().size() + 1) == FbCategoryMessage::TableName() + "/")
                    {
                        FbCategoryMessage(fbMessage).Respond();
                    }
                    else if (path.substr(1, FbSupplierMessage::TableName().size() + 1) == FbSupplierMessage::TableName() + "/")
                    {
                        FbSupplierMessage(fbMessage).Respond();
                    }
                }
                Thread.Sleep(250);
            }
        }
    }



}
