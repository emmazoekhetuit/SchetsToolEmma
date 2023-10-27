using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;


public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);

    //nieuw
    void TekenSelf(Graphics g);
    bool Collides(Point p);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
    public abstract void TekenSelf(Graphics g);
    public abstract bool Collides(Point p);
}

public class TekstTool : StartpuntTool
{
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
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            //gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
    public override void TekenSelf(Graphics g)
    {
        //ToDo
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
        s.Add((ISchetsTool)this.MemberwiseClone()); 
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
    public override bool Collides(Point p) { return false; }
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
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }

    public override void TekenSelf(Graphics g)
    {
        this.Compleet(g, this.startpunt, this.eindpunt);
    }
    public override bool Collides(Point p) { return false; }
}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }
    List<LijnTool> lijnen = new List<LijnTool>(); 

    public override void MuisDrag(SchetsControl s, Point p)
    {   this.MuisLos(s, p);
        this.MuisVast(s, p);
       // lijnen.Add(    );
    }

    public override void TekenSelf(Graphics g)
    {
        foreach(LijnTool t in lijnen)
        {
            t.TekenSelf(g);
                            //this.Compleet(g, this.startpunt, this.eindpunt);
        }
    }
}
    
public class GumTool : PenTool
{

    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
    }

    public override void TekenSelf(Graphics g)
    {
        this.Compleet(g, this.startpunt, this.eindpunt);
    }
}

public class GumTool2 : ISchetsTool
{
    public void MuisVast(SchetsControl s, Point p) { }
    public void MuisDrag(SchetsControl s, Point p) { }
    
    
    public void MuisLos(SchetsControl s, Point p) 
    {
        s.RemoveVorm(p);
    }
    
    public void Letter(SchetsControl s, char c) { }


    //nieuw
    public void TekenSelf(Graphics g) { }
    public override string ToString() { return "vormgum"; }
    public bool Collides(Point p) { return false; }
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

    public override bool Collides(Point p) { return false; }
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

