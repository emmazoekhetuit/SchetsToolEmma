using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

public class SchetsControl : UserControl
{
    private Schets schets;
    private Color penkleur;
    private int pendikte = 3;

    public Color PenKleur
    {
        get { return penkleur; }
    }

    public int PenDikte
    {
        get { return pendikte; }
    }
    public Schets Schets
    {
        get { return schets; }
    }
    public SchetsControl()
    {
        this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }

    private void teken(object o, PaintEventArgs pea)
    {
        schets.Teken(pea.Graphics);
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(this.ClientSize);
        this.Invalidate();
    }
    public Graphics MaakBitmapGraphics()
    {
        Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
    }

    public List<ISchetsTool> MaakSchetsTools()
    {
        List<ISchetsTool> s = schets.SchetsTools;
        return s;
    }
    public void Teken()
    {
        schets.Teken2();
    }

    public void RemoveVorm(Point p)
    {
        schets.RemoveVorm(p);
    }
    public void Schoon(object o, EventArgs ea)
    {
        schets.Schoon();
        this.Invalidate();
    }
    public void Roteer(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
    }
    public void VeranderKleur(object obj, EventArgs ea)
    {
        string kleurNaam = ((ComboBox)obj).Text;
        penkleur = Color.FromName(kleurNaam);

    }

    public void VeranderDikte(object obj, EventArgs ea)
    {
        int a;
        try
        {
            if(((TextBox)obj).Text != "")
            {
                a = int.Parse(((TextBox)obj).Text);
            }
            else { a = pendikte; }
        }
        catch
        {
            MessageBox.Show("Dit is niet correct ingevuld. Je kunt alleen een cijfer invullen", "Waarschuwing");
           ((TextBox)obj).Text = "3";
            a = 3;
        }

        pendikte = a;

    }


    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // SchetsControl
        // 
        Name = "SchetsControl";
        Load += SchetsControlLoad;
        ResumeLayout(false);
    }

    public void Save(object o, EventArgs ea)
    {
        schets.Save();
    }

    public void Opslaan(object o, EventArgs ea)
    {
        //TODO manier om filenaam te doen
        schets.Opslaan("");
    }

    public void Open(object o, EventArgs ea)
    {
        //TOOD manier om filenaam te openen
        schets.Open("");
    }


    public void SchetsControlLoad(object obj, EventArgs ea)
    {
        int a;
        try
        {
            if (((TextBox)obj).Text != "")
            {
                a = int.Parse(((TextBox)obj).Text);
            }
            else { a = pendikte; }
        }
        catch
        {
            MessageBox.Show("Dit is niet correct ingevuld. Je kunt alleen een cijfer invullen", "Waarschuwing");
            ((TextBox)obj).Text = "3";
            a = 3;
        }
        pendikte = a;

    }
}