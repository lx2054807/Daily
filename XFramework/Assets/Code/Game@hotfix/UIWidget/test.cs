using System;

namespace Code.UIWidget
{
    public class GoodClass
    {
        public virtual void GoodFunc()
        {
            Console.WriteLine("This is a good function!!!");
        }
    }

    public class NiceClass : GoodClass
    {
        public override void GoodFunc()
        {
            Console.WriteLine("This is a nice function");
        }
    }

    public class GreatClass : GoodClass
    {
        
    }

    public class start
    {
        public void main()
        {
            NiceClass nc = new NiceClass();
            nc.GoodFunc();
            GreatClass gc = new GreatClass();
            gc.GoodFunc();
            GoodClass goc = new NiceClass();
            goc.GoodFunc();
        }
    }
}