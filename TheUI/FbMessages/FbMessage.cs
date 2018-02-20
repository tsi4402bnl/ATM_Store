namespace TheUI
{
    public abstract class FbMessage
    {
        public string Id { get; set; }
        public bool IsParsed { get; set; }

        protected abstract int Parse();

        protected FbMessage(FbEventData msg)
        {
            IsParsed = false;
            OPERATION = msg.operation;
            PATH = msg.path;
            DATA = "/" + msg.data;
        }

        protected int ParseId(string propName)
        {
            IsParsed = false;
            string endStr = "/" + propName;

            if (PATH.Length == 0) return -1;

            int endPos = PATH.LastIndexOf(endStr);
            if (endPos == PATH.Length - endStr.Length)
            {
                int idPos = PATH.LastIndexOf("/", endPos - 1) + 1;
                if (idPos != -1 && idPos < endPos)
                {
                    Id = PATH.Substring(idPos, endPos - idPos);
                    IsParsed = true;
                }
            }

            return IsParsed ? 0 : -1;
        }

        protected int ParseId()
        {
            IsParsed = false;

            if (PATH.Length == 0) return -1;

            int idPos = PATH.LastIndexOf("/") + 1;
            if (idPos != -1)
            {
                Id = PATH.Substring(idPos);
                IsParsed = true;
            }

            return IsParsed ? 0 : -1;
        }

        protected Fb_Operations OPERATION { get; }
        protected string PATH { get; }
        protected string DATA { get; }
    }
}