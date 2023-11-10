using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Configuration;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);

    //nieuw
    void TekenSelf(Graphics g);
    bool Collides(Point p);

    string toText();

    public ISchetsTool toSchetsTool(string[] list);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;
    protected int dikte; // om de dikte aan te passen 


    public virtual void MuisVast(SchetsControl s, Point p)
    {   startpunt = p;
        dikte = s.PenDikte;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
    public abstract void TekenSelf(Graphics g);
    public abstract bool Collides(Point p);

    public abstract ISchetsTool toSchetsTool(string[] list);
    public virtual string toText()
    {
        return this.ToString() + " " + this.startpunt.ToString() + " " + kwast.ToString() + dikte.ToString();
    }
}

public class TekstTool : StartpuntTool
{
    string tekst;
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }
    
    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, this.startpunt, StringFormat.GenericTypographic);
            //gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
    /*
    public override void MuisLos(SchetsControl s, Point p)
    {
        base.MuisLos(s, p); //kwast maken
        //this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p); //bezig en dat is tekenen op de graphics (maakbitmapgraphics maakt een graphics)
        this.eindpunt = p;
        this.Compleet2(s.MaakSchetsTools()); //pakt de lijst die wij hebben gemaakt en geeft die mee aan compleet2
        s.Teken();
        s.Invalidate();
    }
    */
    public override void TekenSelf(Graphics g)
    {
        //ToDo
    }

    public override StartpuntTool toSchetsTool(string[] list)
    {
        //TODO
        return this;
    }
    public override string toText()
    {
        
        return base.toText() + tekst;
    }

    public override bool Collides(Point p) { return false; }
}

public abstract class TweepuntTool : StartpuntTool
{
    protected Point eindpunt;
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                            , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                            );
    }    

    public static Pen MaakPen(Brush b, int dikte)
    {   Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {   base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   base.MuisLos(s, p); //kwast maken
        //this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p); //bezig en dat is tekenen op de graphics (maakbitmapgraphics maakt een graphics)
        this.eindpunt = p;
        this.Compleet2(s.MaakSchetsTools()); //pakt de lijst die wij hebben gemaakt en geeft die mee aan compleet2
        s.Teken();
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);
        
    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {   this.Bezig(g, p1, p2);

    }

    public virtual void Compleet2(List<ISchetsTool> s)
    {
        s.Add((ISchetsTool)MemberwiseClone()); 
    }

    public override TweepuntTool toSchetsTool(string[] list)
    {
        //TODO
        return this;
    }
    public override string toText()
    {
        //TODO
        return base.toText() + " " + this.eindpunt.ToString();

    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void TekenSelf(Graphics g)
    {
        this.Compleet(g, this.startpunt, this.eindpunt);
    }
    public override bool Collides(Point p)
    {  
     if (startpunt.X < eindpunt.X)
     {
         if (startpunt.Y < eindpunt.Y)
         { 
             if (startpunt.X < p.X && p.X < eindpunt.X & startpunt.Y < p.Y && p.Y < eindpunt.Y)
             { return true; }
        
         }
         else if (eindpunt.Y < startpunt.Y)
         {
             if (startpunt.X < p.X && p.X < eindpunt.X & eindpunt.Y < p.Y && p.Y < startpunt.Y)
             { return true; }
         }
     }
     else if (startpunt.X > eindpunt.X)
     {
         if (startpunt.Y < eindpunt.Y)
         {
             if (startpunt.X > p.X && p.X > eindpunt.X & startpunt.Y < p.Y && p.Y < eindpunt.Y)
             { return true; }
         }
         else if (eindpunt.Y < startpunt.Y)
         {
             if (startpunt.X > p.X && p.X > eindpunt.X & eindpunt.Y < p.Y && p.Y < startpunt.Y)
             { return true; }
         }
     }

     return false;
    }
}

public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {   g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

public class LijnTool : TweepuntTool
{
    public LijnTool() { }

    public LijnTool(Point start, Point eind, Brush b, int dik) 
    { 
        startpunt = start;
        eindpunt = eind;
        kwast = b;
        dikte = dik;
    }

    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(this.kwast,dikte), p1, p2);
    }

    public override void TekenSelf(Graphics g)
    {
        this.Compleet(g, this.startpunt, this.eindpunt);
    }
    public override bool Collides(Point p) 
    {
        int teller = Math.Abs((eindpunt.X - startpunt.X) * (startpunt.Y - p.Y) - (startpunt.X - p.X) * (eindpunt.Y - startpunt.Y));
        double noemer = Math.Sqrt((eindpunt.X - startpunt.X) * (eindpunt.X - startpunt.X) + (eindpunt.Y - startpunt.Y) * (eindpunt.Y - startpunt.Y));
        double afstand = teller / noemer;
        if (afstand < 10)
        {
            return true;
        }
        else
            return false;
    }
}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }
    List<LijnTool> lijnen;

    public override void Compleet2(List<ISchetsTool> s)
    {
        lijnen.Add(new LijnTool(this.startpunt, this.eindpunt, kwast, dikte));
    }

    public override void MuisVast(SchetsControl s, Point p)
    {
        lijnen = new List<LijnTool>();
        base.MuisVast(s, p);
        base.Compleet2(s.MaakSchetsTools());

    }

    public override void MuisDrag(SchetsControl s, Point p)
    {
        this.MuisLos(s, p);
        startpunt = p;
        dikte = s.PenDikte;

    }


    public override void TekenSelf(Graphics g)
    {
        foreach(LijnTool t in lijnen)
        {
            t.TekenSelf(g);
                            //this.Compleet(g, this.startpunt, this.eindpunt);
        }
    }

    public override bool Collides(Point p)
    {
        bool collide;
        foreach(LijnTool l in lijnen)
        {
          collide =  l.Collides(p);
            if (collide) return true;
        }

        return false;
    }
}
    
public class GumTool : PenTool
{
    
    public override string ToString() { return "gum"; }

    public override void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(Color.White);
        this.eindpunt = p;
        this.Compleet2(s.MaakSchetsTools()); //pakt de lijst die wij hebben gemaakt en geeft die mee aan compleet2
        s.Teken();
        s.Invalidate();
    }
}

public class GumTool2 : ISchetsTool
{
    public void MuisVast(SchetsControl s, Point p) { }
    public void MuisDrag(SchetsControl s, Point p) { }
    
    
    public void MuisLos(SchetsControl s, Point p) 
    {
        s.RemoveVorm(p);
        s.Invalidate();
    }
    
    public void Letter(SchetsControl s, char c) { }

    //nieuw
    public void TekenSelf(Graphics g) { }
    public override string ToString() { return "vormgum"; }
    public bool Collides(Point p) { return false; }

    public  ISchetsTool toSchetsTool(string[] list)
    {
        //TODO
        return this;
    }
    public string toText()
    {
        //TODO
        return "";
    }
}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   //Je kan een cirkel tekenen met een rectangle dus doen we dat ook lekker
        g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void TekenSelf(Graphics g)
    {
        this.Compleet(g, this.startpunt, this.eindpunt);
    }
    public override bool Collides(Point p)
    {
       GraphicsPath path = new GraphicsPath();
       path.AddEllipse(TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
       return path.IsVisible(p); 
        

       /* if (startpunt.X < eindpunt.X)
        {
            if (startpunt.Y < eindpunt.Y)
            {
                if (startpunt.X < p.X && p.X < eindpunt.X & startpunt.Y < p.Y && p.Y < eindpunt.Y)
                { return true; }
            }
            else if (eindpunt.Y < startpunt.Y)
            {
                if (startpunt.X < p.X && p.X < eindpunt.X & eindpunt.Y < p.Y && p.Y < startpunt.Y)
                { return true; }
            }
        }
        else if (startpunt.X > eindpunt.X)
        {
            if (startpunt.Y < eindpunt.Y)
            {
                if (startpunt.X > p.X && p.X > eindpunt.X & startpunt.Y < p.Y && p.Y < eindpunt.Y)
                { return true; }
            }
            else if (eindpunt.Y < startpunt.Y)
            {
                if (startpunt.X > p.X && p.X > eindpunt.X & eindpunt.Y < p.Y && p.Y < startpunt.Y)
                { return true; }
            }
        }


        return false;
       */
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "bol"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    { //Je kan een cirkel tekenen met een rectangle dus doen we dat ook lekker
        g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void TekenSelf(Graphics g)
    {
        this.Compleet(g, this.startpunt, this.eindpunt);
    }
}

