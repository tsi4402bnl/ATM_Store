using System;

namespace TheUI
{
    class FbItemMessage : FbMessage
    {
        private FbEventData fbMessage;

        public FbItemMessage(FbEventData fbMessage)
        {
            this.fbMessage = fbMessage;
        }

        public const string TABLE_NAME = "items";

        public void Respond()
        {
            throw new NotImplementedException();
        }
    }



}
