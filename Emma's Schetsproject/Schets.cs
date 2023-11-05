using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class Schets
{
    private Bitmap bitmap;
    private List<ISchetsTool> schetsTools = new List<ISchetsTool>();
        
    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }

    public List<ISchetsTool> SchetsTools
    {
        get { return schetsTools; }
    }

    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }

    public void RemoveVorm(Point p)
    {
        for (int i = schetsTools.Count - 1; i >= 0 ; i--)
        {
            if (schetsTools[i].Collides(p))
            {
                schetsTools.RemoveAt(i);
                Teken2();
                break;
            }
        }
    }
    
    public void Teken2()
    {
        Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);//FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        for (int i = 0; i < schetsTools.Count; i++)
        {
           schetsTools[i].TekenSelf(g);
        }

    }
    public void Teken(Graphics gr)
    {
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()
    {
        List<ISchetsTool> st = SchetsTools;
        st.Clear();

        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }

    //Rotate moet veranderd worden
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }

    public void Save()
    {
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "png image (.png)|.png|jpg image (.jpg)|.jpg|bmp image (.bmp)|.bmp|All files (.)|.";
        ImageFormat format = ImageFormat.Png;
        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            bitmap.Save(sfd.FileName, format);
        }
    }
}